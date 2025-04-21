---
title: Data Management

---
# Data Management

Since `Build5Nines.SharpVector` is a database, it also has data management methods available. These methods enable you to add, remove, and update the text documents that are vectorized and indexed within the semantic database.

## Get Text ID

Every text item within a `Build5Nines.SharpVector` database is assigned a unique identifier (ID). There are a few ways to get access to the ID of the text items.

=== ".AddText()"

    When adding an individual text item to the vector database, the ID value will be returned:

    ```csharp
    var id = vdb.AddText(txt, metadata);

    var id = await vdb.AddTextAsync(txt, metadata);
    ```

=== ".Search()"

    

## Update Text and Metadata

```csharp
vdb.UpdateTextAndMetadata(id, newTxt, newMetadata);
```
