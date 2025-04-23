using Azure;
using Azure.AI.DocumentIntelligence;
using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Build5Nines.SharpVector;

// This sample demonstrates how to use the Document Intelligence client library to analyze a document using the prebuilt-read model.
string endpoint = "https://<resource-name>.cognitiveservices.azure.com/";
string apiKey = "<your-key>";
string filePath = "document.pdf"; // Can be .pdf, .docx, .jpg, etc.

// Create timers to measure how long it takes to run the code
var overallTimer = new System.Diagnostics.Stopwatch();
var stepTimer = new System.Diagnostics.Stopwatch();
overallTimer.Start();


// Create a DocumentIntelligenceClient
var credential = new AzureKeyCredential(apiKey);
var client = new DocumentIntelligenceClient(new Uri(endpoint), credential);

var vdb = new BasicMemoryVectorDatabase();





// Read the file into a BinaryData object
Console.WriteLine("Reading file...");
stepTimer.Start();

using var stream = File.OpenRead(filePath);
byte[] buffer = new byte[stream.Length];
await stream.ReadAsync(buffer, 0, buffer.Length);
var binaryData = BinaryData.FromBytes(buffer);

stepTimer.Stop();
Console.WriteLine($"File loaded into memory: {stepTimer.ElapsedMilliseconds} ms");

Console.WriteLine("Analyzing document with Azure Document Intelligence...");
stepTimer.Restart();

// Analyze the document using the prebuilt-read model
var operation = await client.AnalyzeDocumentAsync(
    WaitUntil.Completed,
    "prebuilt-read",
    binaryData);

var docResult = operation.Value;

stepTimer.Stop();
Console.WriteLine($"Document analysis completed: {stepTimer.ElapsedMilliseconds} ms");

stepTimer.Restart();
Console.WriteLine("Loading SharpVector database...");

foreach (var page in docResult.Pages)
{
    var sb = new StringBuilder();
    foreach (var line in page.Lines)
    {
        sb.AppendLine(line.Content);
    }

    // Add the text to the vector database
    // Let's use the Page Number as the metadata
    // Note: In a real-world scenario, you might want to use more meaningful metadata
    var textMetadata = page.PageNumber.ToString();
    vdb.AddText(sb.ToString(), textMetadata);
}

stepTimer.Stop();
Console.WriteLine($"SharpVector database loaded: {stepTimer.ElapsedMilliseconds} ms");





// Console.WriteLine("");
// Console.WriteLine("Loading PDF File into vector database...");
// stepTimer.Restart();
// // read pdf file with PdfPig locally
// var vdb2 = new BasicMemoryVectorDatabase();
// using (var pdfDocument = UglyToad.PdfPig.PdfDocument.Open(filePath))
// {
//     foreach (var page in pdfDocument.GetPages())
//     {
//         // Add the text to the vector database
//         // Let's use the Page Number as the metadata
//         // Note: In a real-world scenario, you might want to use more meaningful metadata
//         var metadata = page.Number.ToString();
//         vdb.AddText(page.Text, metadata);
//     }
// }
// stepTimer.Stop();
// Console.WriteLine($"Vector database loaded: {stepTimer.ElapsedMilliseconds} ms");








Console.WriteLine("");
Console.WriteLine("Searching in SharpVector database for \"Azure ML\" with similarity score > 0.5...");
stepTimer.Restart();

var query = "Azure ML";
var semanticResults = vdb.Search(
    query,
    threshold: 0.5f // Set a threshold for the similarity score to only match results above this value
    );

stepTimer.Stop();
Console.WriteLine($"Search completed: {stepTimer.ElapsedMilliseconds} ms");


Console.WriteLine("Top Matching Results:");
foreach (var result in semanticResults.Texts)
{
    //var text = result.Text;
    var metadata = result.Metadata;
    var similarity = result.VectorComparison;
    Console.WriteLine($" - Page: {metadata} - Similarity: {similarity}");
}


Console.WriteLine("");
Console.WriteLine("Searching in SharpVector database for \"Why use a Cloud Adoption Framework strategy\", top 3 results...");
stepTimer.Restart();

query = "Why use a Cloud Adoption Framework strategy";
semanticResults = vdb.Search(
    query,
    pageCount: 3 // Set the number of top results to return
    );

stepTimer.Stop();
Console.WriteLine($"Search completed: {stepTimer.ElapsedMilliseconds} ms");


Console.WriteLine("Top Matching Results:");
foreach (var result in semanticResults.Texts)
{
    //var text = result.Text;
    var metadata = result.Metadata;
    var similarity = result.VectorComparison;
    Console.WriteLine($" - Page: {metadata} - Similarity: {similarity}");
}

overallTimer.Stop();
Console.WriteLine("");
Console.WriteLine($"Overall processing time: {overallTimer.ElapsedMilliseconds} ms");