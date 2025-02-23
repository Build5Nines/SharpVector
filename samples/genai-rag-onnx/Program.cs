using System;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Data;
using Microsoft.ML.OnnxRuntimeGenAI;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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
// Example: git clone "https://github.com/hashicorp/terraform-docs-common.git" "C:/tfdocs"
//
//
// Step 3: Run the code and type a prompt to get a response from the
// AI assitant using RAG and the ONNX model.
//

class Program
{
    static async Task Main(string[] args)
    {
        // The path to the directory containing the text files to load into the Vector Database
        var ragFilesDirectoryPath = "C:\\tfdocs";

        // The absolute path to the folder where the Phi-3 model is stored (folder to the ".onnx" file)
        var modelPath = "C:\\Users\\chris\\Desktop\\csharp-onnx-phi3mini\\onnx2\\cpu_and_mobile\\cpu-int4-rtn-block-32";

        // System prompt will be used to instruct the AI how to response to the user prompt
        var systemPrompt = "You are a knowledgeable and friendly assistant made by Build5Nines named Jarvis. Answer the following question as clearly and concisely as possible, providing any relevant information and examples.";


        // ************************************************************************************************
        // CREATE AND LOAD VECTOR DATABASE
        // ************************************************************************************************

        // Create a new BasicMemoryVectorDatabase to store the text vectors
        var vectorDatabase = new BasicMemoryVectorDatabase();

        // Timer for measuring the time it takes to load the database
        var loadVectorTimer = new System.Diagnostics.Stopwatch();
        loadVectorTimer.Start();

        // Load Vector Database from files
        // Get the names of all the files in the directory and it's sub-directories
        var files = Directory.GetFiles(ragFilesDirectoryPath, "*.*", SearchOption.AllDirectories);
        // only read in Text and Markdown files
        files = files.Where(f => f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".mdx", StringComparison.OrdinalIgnoreCase)).ToArray();

        // Loop through the file names and load the database asynchronously

        var vectorDataLoader = new TextDataLoader<int, string>(vectorDatabase);
        Parallel.ForEach(files, async file =>
        {
            Console.WriteLine($"Loading {file}");
            if (File.Exists(file))
            {
                // Load the file into the database
                var fileContents = File.ReadAllText(file);
                await vectorDataLoader.AddDocumentAsync(fileContents, new TextChunkingOptions<string>
                {
                    Method = TextChunkingMethod.Paragraph,
                    // Set the metadata to the file name
                    RetrieveMetadata = (chunk) => file
                });
            }
        });

        // Stop the timer and print the time it took to load the database
        loadVectorTimer.Stop();
        Console.WriteLine($"Loaded {files.Length} documents in {loadVectorTimer.ElapsedMilliseconds} ms");



        Console.WriteLine("Loading AI model...");

        // Create a new Model object from the ONNX model file
        var model = new Model(modelPath);
        var tokenizer = new Tokenizer(model);

        // Create a loop for taking input from the user
        while (true) {
            // Get user prompt
            Console.Write("Type Prompt then Press [Enter] or CTRL-C to Exit: ");
            var userPrompt = Console.ReadLine();
            
        
            // ************************************************************************************************
            // RETRIEVAL AUGMENTED GENERATION (RAG) PART
            // ************************************************************************************************
            // DO THE VECTOR SEARCH TO LOAD ADDITIONAL CONTEXT

            Console.WriteLine("Searching Vector Database...");


            // Timer for measuring the time it takes to search the vector database
            var searchVectorTimer = new System.Diagnostics.Stopwatch();
            searchVectorTimer.Start();

            // Search the Vector Database for the user prompt
            // To get the most relevant results
            var vectorDataResults = await vectorDatabase.SearchAsync(
                userPrompt,     // User Prompt
                pageCount: 8,  // Number of results to return
                threshold: 0.3f // Threshold for the vector comparison
                );

            // Stop the timer and print the time it took to load the database
            searchVectorTimer.Stop();
            Console.WriteLine($"Vector search took {searchVectorTimer.ElapsedMilliseconds} ms");

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
                Console.WriteLine($"Text Chunk Length: {result.Text.Length}");
                Console.WriteLine("");
            }
            Console.WriteLine("");

            // ************************************************************************************************
            // GENERATIVE AI PART
            // ************************************************************************************************

            // Build the Prompt
            var maxPromptLength = 4096; /// max_length - context length configured for the model

            // Make sure RAG Context isn't too long (truncate it)
            var maxAllowedContextLength = maxPromptLength - systemPrompt.Length - userPrompt.Length - 46; // the last number factors in the chat prompt format used
            if (ragContext.Length > maxAllowedContextLength)
            {
                Console.WriteLine("RAG Context too long, truncating it...");
                ragContext = ragContext.Substring(0, maxAllowedContextLength);
            }

            // Chat format - Single User Prompt with System Prompt and RAG Context
            var fullPrompt = $"<|system|>{systemPrompt}<|end|><|user|>{ragContext}\n\n{userPrompt}<|end|><|assistant|>";

            // Tokenize the prompt
            var tokens = tokenizer.Encode(fullPrompt);
            
            // Set generator params
            var generatorParams = new GeneratorParams(model);
            generatorParams.SetSearchOption("max_length", maxPromptLength);
            generatorParams.SetSearchOption("past_present_share_buffer", false);
            //generatorParams.SetInputSequences(tokens);

            // Generate the response
            Console.WriteLine("AI is thinking...");
            var generator = new Generator(model, generatorParams);
            generator.AppendTokenSequences(tokens);

            // show in console that the assistant is responding
            Console.WriteLine("");
            Console.Write("Assistant: ");         

            // Output response as each token in generated
            while (!generator.IsDone()) {
                //generator.ComputeLogits();
                generator.GenerateNextToken();
                var output = GetOutputTokens(generator, tokenizer);
                Console.Write(output);
            }
            Console.WriteLine();
        }  
    }

    private static string GetOutputTokens(Generator generator, Tokenizer tokenizer)
    {
        var outputTokens = generator.GetSequence(0);
        var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
        return tokenizer.Decode(newToken);
    }
}