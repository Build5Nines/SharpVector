namespace SharpVectorPerformance;

using System.Diagnostics;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class DiskVectorDatabasePerformance
{
	private BasicDiskVectorDatabase<string>? _db;
	private string _rootPath = Path.Combine(Path.GetTempPath(), "SharpVectorPerf", Guid.NewGuid().ToString("N"));

	[GlobalSetup]
	public void Setup()
	{
		Directory.CreateDirectory(_rootPath);
		_db = new BasicDiskVectorDatabase<string>(_rootPath);
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		try { if (Directory.Exists(_rootPath)) Directory.Delete(_rootPath, recursive: true); } catch { }
	}

	[Params(25)]
	public int ItemCount;

	[Benchmark]
	public async Task AddTexts()
	{
		var indices = Enumerable.Range(0, ItemCount);
		await Parallel.ForEachAsync(indices, async (i, ct) =>
		{
			var text = $"Sample text {i} fox {Random.Shared.Next(0, 100)}";
			await _db!.AddTextAsync(text, "meta");
		});
	}

	[Benchmark]
	public async Task Search()
	{
		// Ensure some data
		if (!_db!.GetIds().Any())
		{
			var indices = Enumerable.Range(0, 500);
			await Parallel.ForEachAsync(indices, async (i, ct) =>
			{
				await _db.AddTextAsync($"quick brown fox {i}", null);
			});
		}
		var results = await _db.SearchAsync("quick fox");
		// Touch results to avoid dead-code elimination
		_ = results.Texts.Take(10).Count();
	}

	[Benchmark]
	public void DeleteIds()
	{
		var ids = _db!.GetIds().Take(Math.Min(50, _db.GetIds().Count())).ToList();
		foreach (var id in ids)
		{
			_db.DeleteText(id);
		}
	}
}