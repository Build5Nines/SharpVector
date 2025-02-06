// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.OpenAI;

Console.WriteLine("Hello, World!");

var openAIUri = new Uri("https://b59-knowledge-oai.openai.azure.com/");
var openAIKey = "ae8f823052e74b5db7d6e95cc5af4109";
var modelName = "text-embedding-ada-002";

var openAIClient = new AzureOpenAIClient(openAIUri, new AzureKeyCredential(openAIKey));

var embeddingClient = openAIClient.GetEmbeddingClient(modelName);

var vdb = new BasicOpenAIMemoryVectorDatabase(embeddingClient);


 var jsonString = await File.ReadAllTextAsync("OpenAIConsoleTest/movies.json");

var importTimer = new Stopwatch();
importTimer.Start();    



using (JsonDocument document = JsonDocument.Parse(jsonString))
{
    JsonElement root = document.RootElement;
    JsonElement movies = root.GetProperty("movies");

    await Parallel.ForEachAsync(movies.EnumerateArray(), async (movie, cancellationToken) =>
    {
        Console.WriteLine($"Processing movie: {movie.GetProperty("title").GetString()}");
        
        var text = movie.GetProperty("description").GetString();
        var metadata = movie.GetProperty("title").GetString();

        if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(metadata))
        {
            await vdb.AddTextAsync(text, metadata);
        }
    });

    // foreach (JsonElement movie in movies.EnumerateArray())
    // {
    //     var text = movie.GetProperty("description").GetString();
    //     var metadata = movie.GetProperty("title").GetString();
        
    //     if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(metadata))
    //     {
    //         await vdb.AddTextAsync(text, metadata);
    //     }
    // }
}

importTimer.Stop();
Console.WriteLine("Movie data imported into Vector Database.");
Console.WriteLine($"Import took {importTimer.ElapsedMilliseconds} ms");

// Allow user to search for similar text 
Console.WriteLine("Type in prompt text, or type 'exit' to exit the app.");
Console.WriteLine("What movie or TV show are you looking for? Try describing it in a few words.");


while(true) {
    Console.Write("Prompt: ");
    var newPrompt = Console.ReadLine();
    if (newPrompt == "exit") {
        break;
    }
    
    Console.WriteLine(string.Empty);

    if (newPrompt != null) {
        IVectorTextResult<string> result;
        
        var timer = new Stopwatch();
        timer.Start();


        var pageSize = 3;
        // result = await vdb.Search(newPrompt,
        result = await vdb.SearchAsync(newPrompt,
            threshold: 0.001f, // 0.2f, // Cosine Similarity - Only return results with similarity greater than this threshold
            // threshold: (float)1.4f, // Euclidean Distance - Only return results with distance less than this threshold

            //pageIndex: 0, // Page index of the search results (default is 0; the first page)
            pageCount: pageSize // Number of search results per page or max number to return
            );

        timer.Stop();
        Console.WriteLine($"Search took {timer.ElapsedMilliseconds} ms");


        if (result == null || result.IsEmpty)
        {
            Console.WriteLine("No similar text found.");
        } else {
            Console.WriteLine("Similar Text Found!");

            var firstItemIndex = result.PageIndex * pageSize + 1;
            var lastItemIndex = firstItemIndex + (pageSize > result.Texts.Count() ? result.Texts.Count() : pageSize) - 1;

            Console.WriteLine($"Page: {result.PageIndex + 1} (Showing {firstItemIndex} to {lastItemIndex} of Total {result.TotalCount})");
            Console.WriteLine(string.Empty);
            foreach (var item in result.Texts)
            {
                Console.WriteLine($"Metadata: {item.Metadata}");
                Console.WriteLine($"Vector Comparison: {item.VectorComparison}");
                Console.WriteLine(item.Text);
                Console.WriteLine(string.Empty);
            }
        }
    }
}