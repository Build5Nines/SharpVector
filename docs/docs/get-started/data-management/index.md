---
title: Data Management

---
# :material-database-edit-outline: Data Management

Since `Build5Nines.SharpVector` is a database, it also has data management methods available. These methods enable you to add, remove, and update the text documents that are vectorized and indexed within the semantic database.

## Get Text Item ID

Every text item within a `Build5Nines.SharpVector` database is assigned a unique identifier (ID). There are a few ways to get access to the ID of the text items.

=== ".AddText()"

    When adding an individual text item to the vector database, the ID value will be returned:

    ```csharp
    var id = vdb.AddText(txt, metadata);

    var id = await vdb.AddTextAsync(txt, metadata);
    ```

=== ".Search()"

    When you perform a semantic search, the search results will contain the list of texts; each have an ID property.

    ```csharp
    var results = vdb.Search("query text");

    foreach(var text in results.Texts) {
        var id = text.Id;
        var text = text.Text;
        var metadata = text.Metadata;
        // do something here    
    }
    ```

=== "Enumerator"

    The `IVectorDatabase` classes implement `IEnumerable` so you can easily loop through all the text items that have been added to the database.

    ```csharp
    foreach(var item in vdb) {
        var id = item.Id;
        var text = item.Text;
        var metadata = item.Metadata;
        var vector = item.Vector;

        // do something here
    }
    ```

## Get

If you know the `id` of a Text item in the database, you can retrieve it directly.

### Get By Id

The `.GetText` method can be used to retrieve a text item from the vector database directly.

```csharp
vdb.GetText(id);
```

## Update

Once text items have been added to the database "Update" methods can be used to modify them.

### Update Text

The `.UpdateText` method can be used to update the `Text` value, and associated vectors will be updated.

```csharp
vdb.UpdateText(id, newTxt);
```

When the `Text` is updated, new vector embeddings are generated for the new text.

### Update Metadata

The `.UpdateTextMetadata` method can be used to update the `Metadata` for a given text item by `Id`.

```csharp
vdb.UpdateTextMetadata(id, newTxt);
```

When `Metadata` is updated, the vector embeddings are not updated.

### Update Text and Metadata

The `.UpdateTextAndMetadata` method can be used to update the `Text` and `Metadata` for a text item in the database for the given text item `Id`.

```csharp
vdb.UpdateTextAndMetadata(id, newTxt, newMetadata);
```

## Delete

The vector database supports the ability to delete text items.

### Delete Text

The `.DeleteText` method can be used to delete a text item form the database for the given `Id'.

```csharp
vdb.DeleteText(id);
```
