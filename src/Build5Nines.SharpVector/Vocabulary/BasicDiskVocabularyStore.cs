using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using Build5Nines.SharpVector.Vocabulary;

namespace Build5Nines.SharpVector.Vocabulary;

/// <summary>
/// Disk-backed vocabulary store that persists token->index mapping.
/// </summary>
public class BasicDiskVocabularyStore<TKey> : IVocabularyStore<TKey, int>, IDisposable
    where TKey : notnull
{
    private readonly string _rootPath;
    private readonly string _indexPath;
    private readonly string _walPath;

    private readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);

    private ConcurrentDictionary<TKey, int> _vocab = new();
    private ConcurrentDictionary<TKey, int> _cache = new();
    private readonly ConcurrentQueue<IEnumerable<TKey>> _pending = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundFlushTask;

    public int Count => _cache.Count;

    public BasicDiskVocabularyStore(string rootPath)
    {
        _rootPath = rootPath;
        Directory.CreateDirectory(rootPath);
        _indexPath = Path.Combine(rootPath, "vocabulary.json");
        _walPath = Path.Combine(rootPath, "vocabulary.wal");
        RecoverFromWalOrIndex();
        _backgroundFlushTask = Task.Run(BackgroundFlusherAsync);
    }

    public void Update(IEnumerable<TKey> tokens)
    {
        var tokenList = tokens as IList<TKey> ?? tokens.ToList();
        AppendWalRecord(tokenList);

        _rwLock.EnterWriteLock();
        try
        {
            foreach (var token in tokenList)
            {
                if (!_cache.ContainsKey(token))
                {
                    _cache[token] = _cache.Count;
                }
            }
            _pending.Enqueue(tokenList);
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public async Task UpdateAsync(IEnumerable<TKey> tokens)
    {
        await Task.Run(() => Update(tokens));
    }

    public bool TryGetValue(TKey token, out int index) => _cache.TryGetValue(token, out index);

    public async Task SerializeToJsonStreamAsync(Stream stream)
    {
        await JsonSerializer.SerializeAsync(stream, _vocab);
    }

    public async Task DeserializeFromJsonStreamAsync(Stream stream)
    {
        var loaded = await JsonSerializer.DeserializeAsync<ConcurrentDictionary<TKey, int>>(stream);
        if (loaded != null)
        {
            _vocab = loaded;
        }
    }

    private void Persist()
    {
        const int maxRetries = 5;
        int attempt = 0;
        while (true)
        {
            try
            {
                using var fs = new FileStream(_indexPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                JsonSerializer.Serialize(fs, _vocab);
                fs.Flush(true);
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

    private void LoadIfExists()
    {
        if (!File.Exists(_indexPath)) return;
        if (new FileInfo(_indexPath).Length == 0) return;
        using var fs = File.OpenRead(_indexPath);
        var loaded = JsonSerializer.Deserialize<ConcurrentDictionary<TKey, int>>(fs);
        if (loaded != null)
        {
            _vocab = loaded;
            _cache = new ConcurrentDictionary<TKey, int>(loaded);
        }
    }

    private void RecoverFromWalOrIndex()
    {
        LoadIfExists();
        if (!File.Exists(_walPath)) return;
        using var fs = new FileStream(_walPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var br = new BinaryReader(fs);
        while (fs.Position < fs.Length)
        {
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var tokenJson = br.ReadString();
                var token = JsonSerializer.Deserialize<TKey>(tokenJson)!;
                if (!_vocab.ContainsKey(token))
                {
                    var idx = _vocab.Count;
                    _vocab[token] = idx;
                    _cache[token] = idx;
                }
            }
        }
        File.WriteAllBytes(_walPath, Array.Empty<byte>());
        Persist();
    }

    private void AppendWalRecord(IList<TKey> tokens)
    {
        Directory.CreateDirectory(_rootPath);
        using var fs = new FileStream(_walPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        fs.Seek(0, SeekOrigin.End);
        using var bw = new BinaryWriter(fs);
        bw.Write(tokens.Count);
        foreach (var token in tokens)
        {
            bw.Write(JsonSerializer.Serialize(token));
        }
        bw.Flush();
        fs.Flush(true);
    }

    private async Task BackgroundFlusherAsync()
    {
        var token = _cts.Token;
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (_pending.IsEmpty)
                {
                    await Task.Delay(25, token);
                    continue;
                }

                var batch = new List<TKey>();
                while (_pending.TryDequeue(out var tks))
                {
                    batch.AddRange(tks);
                }

                // Apply to persistent vocab
                foreach (var tk in batch)
                {
                    if (!_vocab.ContainsKey(tk))
                    {
                        _vocab[tk] = _vocab.Count;
                    }
                }

                Persist();
                File.WriteAllBytes(_walPath, Array.Empty<byte>());
            }
            catch (OperationCanceledException) { }
            catch { await Task.Delay(100, token); }
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
            try
            {
                var batch = new List<TKey>();
                while (_pending.TryDequeue(out var tks)) batch.AddRange(tks);
                foreach (var tk in batch)
                {
                    if (!_vocab.ContainsKey(tk))
                    {
                        _vocab[tk] = _vocab.Count;
                    }
                }
                Persist();
                File.WriteAllBytes(_walPath, Array.Empty<byte>());
            }
            catch { }
            _cts.Dispose();
            _rwLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
