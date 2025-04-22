---
title: Semantic Search
---
# :material-file-search: Semantic Search

Once text items and their associated metadata have been added to the vector database, the database can be used for semantic search to find matching text items for a given query.

The `BasicMemoryVectorDatabase` and `MemoryVectorDatabase<>` classes both contain `.Search` and `.SearchAsync` methods that can be used to perform semantic search on the database:

=== "Sync"

    ```csharp
    var query = "some text to search";
    var results = vdb.Search(query);
    ```

=== "Async"

    ```csharp
    var query = "some text to search";
    var results = await vdb.SearchAsync(query);
    ```

## Metadata Filters

The `.Search` and `.SearchAsync` methods also include the ability to pre-filter the search results based on a boolean evaluation of the `Metadata` for the text item. This check is run before the vector similarity search is performed, and can help increase search performance on large datasets.

Here are a couple examples of using the `filter` parameter to perform `Metadata` filtering when performing semantic searches:

=== "Sync"

    ```csharp
    var vdb = new BasicMemoryVectorDatabase();

    // load text and metadata into database

    var query = "some text to search";
    var results = vdb.Search(
        query,
        filter: (metadata) => {
            // perform some operation to check metadata
            // return true or false
            return metadata.Contains("B59");
        }
    );
    ```

=== "Async"

    ```csharp
    var vdb = new MemoryVectorDatabase<Person>();

    // load text and metadata into database

    var query = "some text to search";
    var results = vdb.SearchAsync(
        query,
        filter: async (metadata) => {
            // perform some operation to check metadata
            // return true or false
            return metadata.LastName == "Pietschmann";
        }
    );
    ```

!!! info "OpenAI and Ollama Support"

    This functionality works the same with both [:simple-openai: OpenAI and :simple-ollama: Ollama supported vector databases](../../embeddings/index.md) too.

## Paging

The `.Search` and `.SearchAsync` methods also include the ability to perform paging on the text items returned from the semantic search. This is performed after the similarity search and the `filter` has been applied to the search results. This is done using the optional `pageCount` and `pageIndex` paramters.

Here are a couple examples of using the `pageCount` and `pageIndex` parameters to perform paging with the semantic search results:

=== "Sync"

    ```csharp
    var vdb = new BasicMemoryVectorDatabase();

    // load text and metadata into database

    var query = "some text to search";
    var results = vdb.Search(
        query,
        pageIndex: 0, // return first page of results (default: 0)
        pageCount: 6  // limit length of this page of results (default: unlimited)
    );
    ```

=== "Async"

    ```csharp
    var vdb = new MemoryVectorDatabase<Person>();

    // load text and metadata into database

    var query = "some text to search";
    var results = vdb.SearchAsync(
        query,
        pageIndex: 0, // return first page of results (default: 0)
        pageCount: 6  // limit length of this page of results (default: unlimited)
    );
    ```

The `pageIndex` and `pageIndex` paramters are optional, and can be used individually or together.
