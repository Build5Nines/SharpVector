using System;
using Build5Nines.SharpVector;
using Microsoft.ML.OnnxRuntimeGenAI;
using System.Threading.Tasks;

// Sample: GenAI RAG (Retrieval Augmented Generation) with ONNX Model
//
// The ONNX Generative AI code started with the code from this article:
// https://build5nines.com/build-a-generative-ai-app-in-c-with-phi-3-mini-llm-and-onnx/
//
// Then it was extended with Build5Nines.SharpVector to add in the
// RAG (Retrieval Augmented Generation) model support.
//
// Follow these steps to use this sample:
// 
// Step 1: Before you run the code, you need to download the ONNX model.
// See the article for details on how to download the model.
// Then set the 'modelPath' variable to the path where the model is stored.
//
// Step 2: Download text files and place them in a directory.
// Then set the 'ragFilesDirectoryPath' variable to the path of the directory.
//
// Step 3: Run the code and type a prompt to get a response from the
// AI assitant using RAG and the ONNX model.
//

class Program
{
    // The path to the directory containing the text files to load into the Vector Database
    private string ragFilesDirectoryPath = @"C:\YourDirectoryPath";

    // The absolute path to the folder where the Phi-3 model is stored (folder to the ".onnx" file)
    private string modelPath = $"C:\\onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";

    // System prompt will be used to instruct the AI how to response to the user prompt
    private string systemPrompt = "You are a knowledgeable and friendly assistant made by Build5Nines named Jarvis. Answer the following question as clearly and concisely as possible, providing any relevant information and examples.";

    static void Main(string[] args)
    {
        // ************************************************************************************************
        // CREATE AND LOAD VECTOR DATABASE
        // ************************************************************************************************

        // Create a new BasicMemoryVectorDatabase to store the text vectors
        var vectorDatabase = new BasicMemoryVectorDatabaseAsync();

        // Timer for measuring the time it takes to load the database
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();

        // Load Vector Database from files
        // Get the names of all the files in the directory
        string[] files = Directory.GetFiles(ragFilesDirectoryPath);
        // Loop through the file names and load the database asynchronously
        Parallels.ForEach(files, async file =>
        {
            var filename = Path.GetFileName(file);
            // Load the file into the database
            await vectorDatabase.AddDocumentAsync(File.ReadAllText(filename), new TextChunkingOptions<string>
            {
                Method = TextChunkingMethod.Paragraph,
                // Set the metadata to the file name
                RetrieveMetadata = (chunk) => filename
            });
        });

        // Stop the timer and print the time it took to load the database
        timer.Stop();
        Console.WriteLine($"Loaded {files.Length} documents in {timer.ElapsedMilliseconds} ms");




        // Create a new Model object from the ONNX model file
        var model = new Model(modelPath);
        var tokenizer = new Tokenizer(model);

        // Create a loop for taking input from the user
        while (true) {
            // Get user prompt
            Console.Write("Type Prompt then Press [Enter] or CTRL-C to Exit: ");
            var userPrompt = Console.ReadLine();
            
            // show in console that the assistant is responding
            Console.WriteLine("");
            Console.Write("Assistant: ");

        
            // ************************************************************************************************
            // RETRIEVAL AUGMENTED GENERATION (RAG) PART
            // ************************************************************************************************
            // DO THE VECTOR SEARCH TO LOAD ADDITIONAL CONTEXT

            // Search the Vector Database for the user prompt
            // To get the most relevant results
            var vectorDataResults = await vectorDatabase.Search(
                userPrompt,     // User Prompt
                pageCount: 2,  // Number of results to return
                threshold: 0.3f // Threshold for the vector comparison
                );

            // Loop through the vector search results and print them
            var ragContext = string.Empty;
            Console.WriteLine("Vector Search Results:");
            foreach (var result in vectorDataResults.Texts)
            {
                // Add the text to the RAG context
                ragContext += result.Text + "\n\n";
                // Print the metadata, vector comparison, and text of the result to the console
                Console.WriteLine($"Document: {result.Metadata}");
                Console.WriteLine($"Vector Comparison: {result.VectorComparison}");
                Console.WriteLine($"Text: {result.Text}");
                Console.WriteLine("");
            }
            Console.WriteLine("");

            // ************************************************************************************************
            // GENERATIVE AI PART
            // ************************************************************************************************

            // Build the Prompt
            // Single User Prompt with System Prompt and RAG Context
            var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{ragContext}\n\n{userPrompt}<|end|><|assistant|>";

            // Tokenize the prompt
            var tokens = tokenizer.Encode(fullPrompt);
            
            // Set generator params
            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", 2048);
            generatorParams.SetSearchOption("past_present_share_buffer", false);
            generatorParams.SetInputSequences(tokens);

            // Generate the response
            var generator = new Generator(model, generatorParams);
            Console.WriteLine("Generative AI Response:");
            // Output response as each token in generated
            while (!generator.IsDone()) {
                generator.ComputeLogits();
                generator.GenerateNextToken();
                var outputTokens = generator.GetSequence(0);
                var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
                var output = tokenizer.Decode(newToken);
                Console.Write(output);
            }
            Console.WriteLine();
        }
    }
}