# Project Guide for AI Coding Agents (Copilot)

This document gives AI coding assistants (like GitHub Copilot) essential context to work effectively in the SharpVector repo: architecture overview, key entry points, docs locations, conventions, and safe practices for adding features or fixing bugs.

## Overview

- Primary package: `Build5Nines.SharpVector` (NuGet: Build5Nines.SharpVector) targeting .NET 8+.
- Optional integrations:
  - `Build5Nines.SharpVector.OpenAI` — embeddings via OpenAI/Azure OpenAI.
  - `Build5Nines.SharpVector.Ollama` — embeddings via local Ollama server.
- Playground and samples are provided for demos and manual testing.

- Branches: active development occurs on `dev`; confirm before broad changes.
- CI: GitHub Actions workflow `build-release.yml` builds and releases NuGet packages.

## Documentation Locations

- Public docs site sources: `docs/` (MkDocs)
  - Index: `docs/docs/index.md`
  - Get Started: `docs/docs/get-started/`
  - Concepts, Persistence, Text Chunking, Samples, etc. under `docs/docs/`
- Root README: `README.md` — high-level intro and NuGet info.
- Project-specific docs inside src:
  - `src/Build5Nines.SharpVector/docs/` — internal docs snippets.

When adding features, update both code and related docs (MkDocs under `docs/docs/...`). Keep docs concise with examples and cross-links.

- `src/SharpVector.sln` — solution file.
- `src/Build5Nines.SharpVector/` — core library
  - Embeddings interfaces: `Embeddings/IEmbeddingsGenerator.cs`, `Embeddings/IBatchEmbeddingsGenerator.cs`
  - Core DB abstractions:
    - `IVectorDatabase.cs` — main interface.
    - `VectorDatabaseBase.cs` — common logic.
    - `MemoryVectorDatabaseBase.cs`, `MemoryVectorDatabase.cs`, `BasicMemoryVectorDatabase.cs` — in-memory implementations.
    - Disk persistence: `BasicDiskMemoryVectorDatabaseBase.cs`, `BasicDiskVectorDatabase.cs`, `DatabaseFile.cs`, `DatabaseInfo.cs`
  - Vector comparison (search metrics): `VectorCompare/`
    - `IVectorComparer.cs`
    - `CosineSimilarityVectorComparerAsync.cs` (default)
    - `EuclideanDistanceVectorComparerAsync.cs`
  - Preprocessing & Vectorization pipeline: `Preprocessing/`, `Vectorization/`, `Vocabulary/`, `VectorStore/`, `Id/`
  - Extensions: `IVectorDatabaseExtensions.cs`
- `src/Build5Nines.SharpVector.OpenAI/` — OpenAI embeddings
  - `Embeddings/OpenAIEmbeddingsGenerator.cs`
  - Memory DB wrappers using OpenAI: `OpenAIMemoryVectorDatabase*.cs`
- `src/Build5Nines.SharpVector.Ollama/` — Ollama embeddings
  - `Embeddings/OllamaEmbeddingsGenerator.cs`
  - Memory DB wrappers using Ollama: `OllamaMemoryVectorDatabase*.cs`
- Playground & samples
  - `src/Build5Nines.SharpVector.Playground/` — demo app, configurable via `appsettings.json`
  - `samples/` and `src/*ConsoleTest`, `*Test` projects — usage examples and tests.

## Typical Usage (Core Library)

- Create DB: `var vdb = new BasicMemoryVectorDatabase();`
- Add text: `vdb.AddText("some text", metadata);` (sync/async variants)
- Search: `var results = vdb.Search("query text");` (uses cosine similarity by default)
- Custom embeddings: Provide your own `IEmbeddingsGenerator` or use OpenAI/Ollama packages.
- Change comparison metric: Supply an `IVectorComparer` (e.g., Euclidean distance) to the DB.

Minimal example:

```csharp
using Build5Nines.SharpVector;

var vdb = new BasicMemoryVectorDatabase();
vdb.AddText("Hello SharpVector", metadata: "sample");
var results = vdb.Search("Hello");
```

## Key Design Concepts

- In-memory first: Default DB stores vectors in memory for speed. Disk-backed options exist for persistence.
- Pluggable pipeline:
  - Embeddings generation — interfaces allow external providers.
  - Preprocessing — text normalization/tokenization configurable under `Preprocessing/`.
  - Vector comparison — swapable similarity metrics via `IVectorComparer`.
- Metadata support: Store arbitrary metadata alongside each text entry.
- Async support: Async APIs exist for scalable operations.

## Conventions & Coding Guidelines

- Language/Runtime: C#, .NET 8+. Use async/await where appropriate.
- Style: Match existing patterns. Avoid wide refactors; make minimal, focused changes.
- Naming: Prefer descriptive names; avoid single-letter variables.
- Comments: Keep code clear; avoid inline comments unless necessary for clarity.
- Errors/exceptions: Use specific exception types like `DatabaseFileException` where applicable.
- Tests: When adding/altering behavior, include or update tests in `src/*Test` projects.

- API stability: Prefer additive changes; avoid breaking public types/methods.
- Nullability: Follow existing project settings; respect nullable context in projects.
- Performance: Avoid allocations in tight loops; prefer spans/arrays where safe.

## How to Add Features Safely

1. Identify extension point:
   - New similarity metric → implement `IVectorComparer` under `src/Build5Nines.SharpVector/VectorCompare/` and wire via constructor/config.
   - New embeddings provider → implement `IEmbeddingsGenerator` (and optionally `IBatchEmbeddingsGenerator`) under a new package or existing OpenAI/Ollama.
   - Persistence enhancement → extend `BasicDiskMemoryVectorDatabaseBase` and update `DatabaseFile` handling.
2. Keep public APIs stable; prefer additive changes.
3. Update docs in `docs/docs/...` with short “Getting Started” and example.
4. Run tests and add focused unit tests for new logic.

Example wiring for custom comparer:

```csharp
var comparer = new EuclideanDistanceVectorComparerAsync();
var vdb = new BasicMemoryVectorDatabase(vectorComparer: comparer);
```

## Bug Fix Workflow

- Reproduce with a minimal sample (Playground or `*ConsoleTest`).
- Locate source by interfaces:
  - Insert/search issues → `MemoryVectorDatabase*`, `VectorStore/`, `Vectorization/`.
  - Similarity result issues → `VectorCompare/*`.
  - Embeddings/provider issues → `Embeddings/*`, OpenAI/Ollama projects.
- Add small unit tests or use BenchmarkDotNet samples for perf-sensitive changes.
- Keep changes minimal; do not alter unrelated behavior.

When fixing bugs, add a regression test under the closest `*Test` project and keep scope tight.

## Performance Notes

- Benchmark artifacts: `BenchmarkDotNet.Artifacts/` and `src/BenchmarkDotNet.Artifacts/` contain previous perf runs.
- Optimize critical paths:
  - Avoid unnecessary allocations in comparison loops.
  - Prefer span/array operations where safe.
  - Batch embeddings when provider supports it.

## Build & Run

- Build solution:

```bash
dotnet build src/SharpVector.sln -c Release
```

- Run Playground:

```bash
dotnet run --project src/Build5Nines.SharpVector.Playground -c Debug
```

- Run tests (adjust if test projects differ):

```bash
dotnet test src/SharpVector.sln
```

Common test projects (names may vary):
- `src/SharpVectorTest/` — unit tests for core library.
- `src/SharpVectorPerformance/` — benchmarks, see `BenchmarkDotNet.Artifacts/`.


## Docs Authoring (MkDocs)

- MkDocs config: `docs/mkdocs.yml`
- Local preview (requires Python + requirements):

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r docs/requirements.txt
mkdocs serve -f docs/mkdocs.yml
```

- Theme overrides in `docs/overrides/`.

When adding a feature, include a short “Getting Started” snippet and cross-link to relevant concepts under `docs/docs/`.

## External Integrations

- OpenAI: configure API keys via environment or appsettings in sample apps.
- Ollama: ensure local Ollama server is running and accessible.

OpenAI/Azure OpenAI configuration (samples):

- Environment variables (example):

```bash
export OPENAI_API_KEY="..."
export AZURE_OPENAI_ENDPOINT="https://<your-endpoint>.openai.azure.com"
export AZURE_OPENAI_API_KEY="..."
```

- `appsettings.json` keys for Playground:

```json
{
  "OpenAI": {
    "ApiKey": "...",
    "Model": "text-embedding-3-large"
  },
  "AzureOpenAI": {
    "Endpoint": "https://<your-endpoint>.openai.azure.com",
    "ApiKey": "...",
    "Deployment": "text-embedding-3-large"
  },
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "Model": "nomic-embed-text"
  }
}
```

## Useful Entry Points (Code Navigation)

- `Build5Nines.SharpVector/IVectorDatabase.cs` — core interface for DB operations.
- `Build5Nines.SharpVector/BasicMemoryVectorDatabase.cs` — easiest reference implementation.
- `Build5Nines.SharpVector/VectorCompare/*` — similarity algorithms.
- `Build5Nines.SharpVector.OpenAI/OpenAIMemoryVectorDatabase*.cs` — OpenAI integration.
- `Build5Nines.SharpVector.Ollama/OllamaMemoryVectorDatabase*.cs` — Ollama integration.

Also see:
- `Build5Nines.SharpVector/VectorStore/*` — storage mechanics for vector entries.
- `Build5Nines.SharpVector/Vectorization/*` — local vectorization pipeline.
- `Build5Nines.SharpVector/Preprocessing/*` — normalization/tokenization steps.
- `Build5Nines.SharpVector/Vocabulary/*` — vocabulary handling if applicable.

## Common Pitfalls

- Mismatched vector dimensions between provider and store; validate sizes.
- Forgetting to persist when using disk-backed DB — ensure save/load paths.
- Not disposing resources for external providers (HTTP clients, etc.).
- Assuming synchronous search; prefer async variants in large datasets.

- Vector dimension mismatches between embeddings providers and DB entries.
- Forgetting to persist when using disk-backed DB implementations.
- Not disposing external providers (HTTP clients) in integration packages.

## Maintainers & Attribution

- Maintained by Build5Nines / Chris Pietschmann. MIT licensed.
- Follow CODE_OF_CONDUCT.md. Keep PRs focused and well-documented.

---

If you are an AI agent assisting here: prefer targeted edits, update docs, add tests, and keep public APIs stable. When unsure, propose alternatives and ask for confirmation before broad refactors.

## Contribution & PR Guidance

- Discuss significant changes via issue first; target the `dev` branch.
- Keep PRs focused and small; include tests and docs updates.
- Run `dotnet format` if configured to keep style consistent.
- Ensure CI passes; include clear description and rationale.
