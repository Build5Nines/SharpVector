# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## v2.0.0 (In Progress)

Added:

- Add data persistence capability

Breaking Change:

- Refactor `IVocabularyStore` to be used within `MemoryDictionaryVectorStoreWithVocabulary`. This simplifies implementation of `MemoryVectorDatabaseBase`, and helps to enable data persistence capability.

Notes:

- The breaking change only applies if the base classes are being used. If the `BasicMemoryVectorDatabase` is being used, this will likely not break applications that depend on this library. However, in some instances where explicitly depending on `VectorTextResult` it's properties (without using `var` in consuming code) there might be minor code changes needed when migrating from previous versions of the library.

## v1.0.1 (2025-02-06)

- Upgrade to .NET 8 or higher

### v1.0.0 (2024-05-24)

Added:

- Simplify object model by combining Async and non-Async classes, `BasicMemoryVectorDatabase` now support both synchronous and asynchronous operations.
- Refactored to remove unnecessary classes where the `Async` versions will work just fine.
- Improve async/await and multi-threading use

### v0.9.8-beta (2024-05-20)

Added:

- Added `Async` version of classes to support multi-threading
- Metadata is no longer required when calling `.AddText()` and `.AddTextAsync()`
- Refactor `IVectorSimilarityCalculator` to `IVectorComparer` and `CosineVectorSimilarityCalculatorAsync` to `CosineSimilarityVectorComparerAsync`
- Add new `EuclideanDistanceVectorComparerAsync`
- Fix `MemoryVectorDatabase` to no longer requird unused `TId` generic type
- Rename `VectorSimilarity` and `Similarity` properties to `VectorComparison`

### v0.9.5-beta (2024-05-18)

Added:

- Add `TextDataLoader` class to provide support for different methods of text chunking when loading documents into the vector database.

### v0.9.0-beta (2024-05-18)

Added:

- Introduced the `BasicMemoryVectorDatabase` class as the basic Vector Database implementations that uses a Bag of Words vectorization strategy, with Cosine similarity, a dictionary vocabulary store, and a basic text preprocessor.
- Add more C# Generics use, so the library is more customizable when used, and custom vector databases can be implemented if desired.
- Added `VectorTextResultItem.Similarity` so consuming code can inspect similarity of the Text in the vector search results.
- Update `.Search` method to support search result paging and threshold support for similarity comparison
- Add some basic Unit Tests

### v0.8.0-beta (2024-05-17)

Added:

- Initial release - let's do this!
