---
title: Metadata
---
# :material-database-cog-outline: Metadata

The `Build5Nines.SharpVector` vector database enables semantic search for `Text` that is stored in the database. Being able to semantically search text is an extremely useful way to lookup more information related to the text. For this purpose, `Metadata` is stored alongside the `Text` within the vector database. This way, when `Text` is found when performing a semantic search, then the matching `Metadata` is also retrieved.

## Adding Metadata

The `.AddText` and `.AddTextAsync` methods access 2 arguments:

- `text`: The `Text` that is added to the vector database and has vector embeddings generated for.
- `metadata`: This is additional data / information that is stored alongside the `Text`.

```csharp
vdb.AddText(text, metadata);

await vdb.AddText(text, metadata);
```

## JSON and String Metadata

When using the `BasicMemoryVectorDatabase` class, the `Metadata` values will always be of type `String`. This enables you to store a variety of values here, including:

- **JSON data**: You can serialize any data to a JSON string for storage in the `Metadata` associated with a text item in the database.
- **`String` value**: You can store any other string value as the `Metadata` associated with a text item in the database. This could be a URL, Filename, or other information.

!!! info "OpenAI and Ollama Support"
    When working with the [OpenAI](../../embeddings/openai/index.md) `BasicOpenAIMemoryVectorDatabase` and [Ollama](../../embeddings/ollama/index.md) `BasicOllamaMemoryVectorDatabase`, the `Metadata` data type is also `String`.

Here are some examples of storing `string` metadata and retrieving it from the database:

=== "JSON data"

    ```csharp
    // create vector database
    var vdb = new BasicMemoryVectorDatabase();

    // some text to store in the vector database
    var text = "some text value";
    // serialize an object to json to store as metadata
    var json = JsonSerializer.Serialize(new MyMetadata{
        Url = "https://build5nines.com",
        Author = "Chris Pietschmann"
    });

    // Add text with metadata to vector database
    vdb.AddText(text, json);

    // perform semantic search
    var results = vdb.Search("something to search", pageCount: 5);

    // Loop through search results
    foreach(var item in results.Texts) {
        var text = item.Text;
        var json = item.Metadata;
        var metadata = JsonSerializer.Deserialize<MyMetadata>(json);

        // do something with results and metadata
    }
    ```

=== "String value"

    ```csharp
    // create vector database
    var vdb = new BasicMemoryVectorDatabase();

    // some text to store in the vector database
    var text = "some text value";
    // some metadata to store
    var metadata = "https://build5nines.com";

    // Add text with metadata to vector database
    vdb.AddText(text, metadata);

    // perform semantic search
    var results = vdb.Search("something to search", pageCount: 5);

    // Loop through search results
    foreach(var item in results.Texts) {
        var text = item.Text;
        var metadata = item.Metadata;

        // do something with results and metadata
    }
    ```

## Custom Metadata Type

The `MemoryVectorDatabase<TMetadata>` generic class allows you to create a vector database that uses your own custom class as the metadata by defining that class using generics. This enables you to store a native .NET object as the metadata alongside the text in the vector database.

Here's an example of using the `MemoryVectorDatabase<TMetadata>` with a .NET class for the `Metadata`:

```csharp
// create vector database
var vdb = new MemoryVectorDatabase<MyMetadata>();

// some text to store in the vector database
var text = "some text value";
// an object to store as metadata
var metadata = new MyMetadata{
    Url = "https://build5nines.com",
    Author = "Chris Pietschmann"
};

// Add text with metadata to vector database
vdb.AddText(text, metadata);

// perform semantic search
var results = vdb.Search("something to search", pageCount: 5);

// Loop through search results
foreach(var item in results.Texts) {
    var text = item.Text;
    var metadata = item.Metadata;
    
    var url = metadata.Url;
    var author = metadata.Author;

    // do something with results and metadata
}
```

This will offer better performance with scenarios that require more complex metadata since you no longer need to handle serialization to/from JSON.

!!! info "OpenAI and Ollama Support"
    The `OpenAIMemoryVectorDatabase<TMetadata>` and `OllamaMemoryVectorDatabase<TMetadata>` generic classes can also be used to define your own `Metadata` type when working with [OpenAI and Ollama embeddings](../../embeddings/index.md).
