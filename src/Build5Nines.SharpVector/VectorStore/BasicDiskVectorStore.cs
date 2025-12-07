using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

namespace Build5Nines.SharpVector.VectorStore;

/// <summary>
/// Disk-backed vector store. Persists items to files and supports streaming reads.
/// Uses the vocabulary key type as the document type to satisfy the IVectorStore contract.
/// </summary>
public class BasicDiskVectorStore<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>
    : IVectorStoreWithVocabulary<TId, TMetadata, TVocabularyStore, TVocabularyKey, TVocabularyValue>, IDisposable
    where TId : notnull
    where TVocabularyKey : notnull
    where TVocabularyValue : notnull
    where TVocabularyStore : IVocabularyStore<TVocabularyKey, TVocabularyValue>
{
    private readonly string _rootPath;
    private readonly string _indexPath;
    private readonly string _itemsPath;
    private readonly string _walPath;

    private readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);

    private readonly ConcurrentDictionary<TId, long> _index = new();
    private readonly ConcurrentDictionary<TId, VectorTextItem<TVocabularyKey, TMetadata>> _cache = new();

    private readonly ConcurrentQueue<(TId id, VectorTextItem<TVocabularyKey, TMetadata>? item, bool isDelete)> _pending = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundFlushTask;

    public TVocabularyStore VocabularyStore { get; }

    public int Count => _cache.Count;

    public BasicDiskVectorStore(string rootPath, TVocabularyStore vocabularyStore)
    {
        _rootPath = rootPath;
        _indexPath = Path.Combine(rootPath, "index.json");
        _itemsPath = Path.Combine(rootPath, "items.bin");
        _walPath = Path.Combine(rootPath, "wal.log");
        Directory.CreateDirectory(rootPath);
        VocabularyStore = vocabularyStore;
        RecoverFromWalOrIndex();
        _backgroundFlushTask = Task.Run(BackgroundFlusherAsync);
    }

    public IEnumerable<TId> GetIds() => _cache.Keys;

    public IVectorTextItem<TVocabularyKey, TMetadata> Get(TId id)
    {
        // First check cache for fast read
        if (_cache.TryGetValue(id, out var cached)) return cached;

        _rwLock.EnterReadLock();
        try
        {
            if (!_index.TryGetValue(id, out var offset)) throw new KeyNotFoundException();
            using var fs = File.OpenRead(_itemsPath);
            fs.Seek(offset, SeekOrigin.Begin);
            var item = ReadItem(fs);
            _cache[id] = item;
            return item;
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public void Set(TId id, VectorTextItem<TVocabularyKey, TMetadata> item)
    {
        // Write-Ahead Log entry to ensure durability (A in ACID)
        AppendWalRecord(id, item, isDelete: false);

        // Update memory state atomically
        _rwLock.EnterWriteLock();
        try
        {
            _cache[id] = item;
            _pending.Enqueue((id, item, false));
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public async Task SetAsync(TId id, VectorTextItem<TVocabularyKey, TMetadata> item)
    {
        Set(id, item);
        await Task.Yield();
    }

    public IVectorTextItem<TVocabularyKey, TMetadata> Delete(TId id)
    {
        var existing = Get(id);

        // WAL for delete
        AppendWalRecord(id, item: null, isDelete: true);

        _rwLock.EnterWriteLock();
        try
        {
            _cache.TryRemove(id, out _);
            _pending.Enqueue((id, null, true));
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }

        return existing;
    }

    public bool ContainsKey(TId id) => _cache.ContainsKey(id);

    public async Task SerializeToJsonStreamAsync(Stream stream)
    {
        await JsonSerializer.SerializeAsync(stream, _index);
    }

    public async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        var loaded = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<TId, long>>(stream);
        if (loaded != null)
        {
            foreach (var kv in loaded) _index[kv.Key] = kv.Value;
        }
    }

    public IEnumerator<KeyValuePair<TId, VectorTextItem<TVocabularyKey, TMetadata>>> GetEnumerator()
    {
        foreach (var key in _cache.Keys)
        {
            yield return new KeyValuePair<TId, VectorTextItem<TVocabularyKey, TMetadata>>(key, (VectorTextItem<TVocabularyKey, TMetadata>)Get(key));
        }
    }

    System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async IAsyncEnumerator<KeyValuePair<TId, VectorTextItem<TVocabularyKey, TMetadata>>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (var key in _cache.Keys)
        {
            yield return new KeyValuePair<TId, VectorTextItem<TVocabularyKey, TMetadata>>(key, (VectorTextItem<TVocabularyKey, TMetadata>)Get(key));
            await Task.Yield();
        }
    }

    private void PersistIndex()
    {
        const int maxRetries = 5;
        int attempt = 0;
        while (true)
        {
            try
            {
                using var fs = new FileStream(_indexPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                JsonSerializer.Serialize(fs, _index);
                fs.Flush(true); // ensure durability of index checkpoint
                break;
            }
            catch (IOException)
            {
                attempt++;
                if (attempt >= maxRetries) throw;
                Thread.Sleep(10 * attempt);
            }
        }
    }

    private void LoadIndexIfExists()
    {
        if (!File.Exists(_indexPath)) return;
        using var fs = File.OpenRead(_indexPath);
        var loaded = JsonSerializer.Deserialize<ConcurrentDictionary<TId, long>>(fs);
        if (loaded != null)
        {
            foreach (var kv in loaded) _index[kv.Key] = kv.Value;
        }
    }

    private void RecoverFromWalOrIndex()
    {
        // Load index checkpoint if present
        LoadIndexIfExists();

        // Replay WAL to recover any operations after the last checkpoint
        if (!File.Exists(_walPath)) return;
        using var fs = new FileStream(_walPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var br = new BinaryReader(fs);
        while (fs.Position < fs.Length)
        {
            bool isDelete = br.ReadBoolean();
            var idJson = br.ReadString();
            var id = JsonSerializer.Deserialize<TId>(idJson)!;
            if (isDelete)
            {
                _index.TryRemove(id, out _);
                _cache.TryRemove(id, out _);
            }
            else
            {
                var itemJson = br.ReadString();
                var item = JsonSerializer.Deserialize<VectorTextItem<TVocabularyKey, TMetadata>>(itemJson)!;

                // Append item to items file to bring storage up-to-date
                using var ofs = new FileStream(_itemsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                ofs.Seek(0, SeekOrigin.End);
                var offset = ofs.Position;
                WriteItem(ofs, item);
                ofs.Flush(true);
                _index[id] = offset;
                _cache[id] = item;
            }
        }
        // After successful replay, truncate WAL (commit)
        File.WriteAllBytes(_walPath, Array.Empty<byte>());
        PersistIndex();
    }

    private void AppendWalRecord(TId id, VectorTextItem<TVocabularyKey, TMetadata>? item, bool isDelete)
    {
        Directory.CreateDirectory(_rootPath);
        using var fs = new FileStream(_walPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        fs.Seek(0, SeekOrigin.End);
        using var bw = new BinaryWriter(fs);
        bw.Write(isDelete);
        bw.Write(JsonSerializer.Serialize(id));
        if (!isDelete && item != null)
        {
            bw.Write(JsonSerializer.Serialize(item));
        }
        bw.Flush();
        fs.Flush(true); // fsync for WAL to guarantee durability
    }

    private async Task BackgroundFlusherAsync()
    {
        var token = _cts.Token;
        while (!token.IsCancellationRequested)
        {
            try
            {
                // Batch flush pending operations
                if (_pending.IsEmpty)
                {
                    await Task.Delay(25, token);
                    continue;
                }

                using var itemsFs = new FileStream(_itemsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                while (_pending.TryDequeue(out var op))
                {
                    if (op.isDelete)
                    {
                        _index.TryRemove(op.id, out _);
                    }
                    else if (op.item is not null)
                    {
                        itemsFs.Seek(0, SeekOrigin.End);
                        var offset = itemsFs.Position;
                        WriteItem(itemsFs, op.item);
                        _index[op.id] = offset;
                    }
                }
                itemsFs.Flush(true);
                PersistIndex();

                // After index checkpoint, truncate WAL safely
                File.WriteAllBytes(_walPath, Array.Empty<byte>());
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch
            {
                // best-effort: wait and retry
                await Task.Delay(100, token);
            }
        }
    }

    public void Dispose()
    {
        try
        {
            _cts.Cancel();
            _backgroundFlushTask.Wait(1500);
        }
        catch { }
        finally
        {
            // Attempt a final flush of pending operations synchronously
            try
            {
                using var itemsFs = new FileStream(_itemsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                while (_pending.TryDequeue(out var op))
                {
                    if (op.isDelete)
                    {
                        _index.TryRemove(op.id, out _);
                    }
                    else if (op.item is not null)
                    {
                        itemsFs.Seek(0, SeekOrigin.End);
                        var offset = itemsFs.Position;
                        WriteItem(itemsFs, op.item);
                        _index[op.id] = offset;
                    }
                }
                itemsFs.Flush(true);
                PersistIndex();
                File.WriteAllBytes(_walPath, Array.Empty<byte>());
            }
            catch { }

            _cts.Dispose();
            _rwLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    private static void WriteItem(FileStream fs, VectorTextItem<TVocabularyKey, TMetadata> item)
    {
        using var bw = new BinaryWriter(fs, System.Text.Encoding.UTF8, leaveOpen: true);
        var json = JsonSerializer.Serialize(item);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        bw.Write(bytes.Length);
        bw.Write(bytes);
    }

    private static VectorTextItem<TVocabularyKey, TMetadata> ReadItem(FileStream fs)
    {
        using var br = new BinaryReader(fs, System.Text.Encoding.UTF8, leaveOpen: true);
        int len = br.ReadInt32();
        var bytes = br.ReadBytes(len);
        var json = System.Text.Encoding.UTF8.GetString(bytes);
        var item = JsonSerializer.Deserialize<VectorTextItem<TVocabularyKey, TMetadata>>(json)!;
        return item;
    }
}

public class BasicDiskVectorStore<TId, TMetadata>
    : BasicDiskVectorStore<TId, TMetadata, IVocabularyStore<string, int>, string, int>
    where TId : notnull
{
    public BasicDiskVectorStore(string rootPath, IVocabularyStore<string, int> vocabularyStore)
        : base(rootPath, vocabularyStore)
    { }
}
