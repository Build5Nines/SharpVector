using System.Diagnostics;
using System.Text.Json;
using Build5Nines.SharpVector;

public static class Program
{
    public static void Main(string[] args)
    {
        // Create a Vector Database with metadata of type string
        var vdb = new MemoryVectorDatabase<string>();

        // Parse Movie JSON data and add it to the Vector Database
        var jsonString = File.ReadAllText("movies.json");
        
        Console.WriteLine("Importing Movie data into Vector Database...");

        var importTimer = new Stopwatch();
        importTimer.Start();    

        using (JsonDocument document = JsonDocument.Parse(jsonString))
        {
            JsonElement root = document.RootElement;
            JsonElement movies = root.GetProperty("movies");

            foreach (JsonElement movie in movies.EnumerateArray())
            {
                var text = movie.GetProperty("description").GetString();
                var metadata = movie.GetProperty("title").GetString();
                
                if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(metadata))
                {
                    vdb.AddText(text, metadata);
                }
            }
        }
        
        importTimer.Stop();
        Console.WriteLine("Movie data imported into Vector Database.");
        Console.WriteLine($"Import took {importTimer.ElapsedMilliseconds} ms");








        // Paths to the large text files
        // string[] filePaths = { "file1.txt", "file2.txt", "file3.txt" };

        // foreach (var filePath in filePaths)
        // {
        //     try
        //     {
        //         // Read the content of the file
        //         string content = File.ReadAllText(filePath);

        //         // Split the content into paragraphs
        //         string[] paragraphs = SplitIntoParagraphs(content);

        //         // Add each paragraph to the vector database
        //         foreach (var paragraph in paragraphs)
        //         {
        //             int id = vdb.AddText(paragraph, filePath);
        //             Console.WriteLine($"Added paragraph from {filePath} with ID: {id}");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
        //     }
        // }


        // // Create a Vector Database with metadata of type double
        // var vdb = new MemoryVectorDatabase<double>();

        // // Load Vector Database with some sample text
        // vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        // vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        // vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        // vdb.AddText("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        // vdb.AddText("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        // vdb.AddText("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        // vdb.AddText("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        // vdb.AddText("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        // vdb.AddText("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        // vdb.AddText("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        // vdb.AddText("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        // vdb.AddText("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        // vdb.AddText("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        // vdb.AddText("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        // vdb.AddText("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        // vdb.AddText("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        // vdb.AddText("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);



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
                result = vdb.Search(newPrompt,
                    threshold: 0.001f, // 0.2f, // Only return results with similarity greater than this threshold
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
                        Console.WriteLine($"Vector Similarity: {item.Similarity}");
                        Console.WriteLine(item.Text);
                        Console.WriteLine(string.Empty);
                    }
                }
            }
        }
    }

    // /// <summary>
    // /// Method to split text into paragraphs
    // /// </summary>
    // /// <param name="text"></param>
    // /// <returns></returns>
    // private static string[] SplitIntoParagraphs(string text)
    // {
    //     // Splitting by double newline which typically indicates a paragraph break
    //     return text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);
    // }
}