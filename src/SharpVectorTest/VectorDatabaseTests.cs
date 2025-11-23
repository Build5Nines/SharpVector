namespace SharpVectorTest;

using System.Diagnostics;
using System.Threading.Tasks;
using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Embeddings;
using Build5Nines.SharpVector.Id;
using Build5Nines.SharpVector.Preprocessing;
using Build5Nines.SharpVector.VectorCompare;
using Build5Nines.SharpVector.Vectorization;
using Build5Nines.SharpVector.VectorStore;
using Build5Nines.SharpVector.Vocabulary;

[TestClass]
public class VectorDatabaseTests
{
    [TestMethod]
    public void BasicMemoryVectorDatabase_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "[some metadata here]");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(1, results.Texts.First().Id);
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_02()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.IsNull(results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_03()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_04()
    {
        var vdb = new MemoryVectorDatabase<double?>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.IsNull(results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_05()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire.", "metadata2");
        vdb.AddText("No emoji", "metadata3");
        
        var results = vdb.Search("🔥", pageCount: 1);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual(0.5773503184318542, results.Texts.First().VectorComparison);
        Assert.AreEqual("It's 🔥 Fire.", results.Texts.First().Text);
        Assert.AreEqual(2, results.Texts.First().Id);
        Assert.AreEqual("metadata2", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_06()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire", "metadata2");
        vdb.AddText("👑🔥 🏕️", "metadata3");
        
        var results = vdb.Search("🔥👑🏕️", pageCount: 1);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("👑🔥 🏕️", results.Texts.First().Text);
        Assert.AreEqual(3, results.Texts.First().Id);
        Assert.AreEqual("metadata3", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_SaveLoadAsync_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "[some metadata here]");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        var filename = "basicmemoryvectordatabase_saveload_01.b59vdb";
        vdb.SaveToFileAsync(filename).Wait();


        var newvdb = new BasicMemoryVectorDatabase();
        newvdb.LoadFromFileAsync(filename).Wait();
        results = newvdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_SaveLoad_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "[some metadata here]");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        var filename = "basicmemoryvectordatabase_saveload_01.b59vdb";
        vdb.SaveToFile(filename);


        var newvdb = new BasicMemoryVectorDatabase();
        newvdb.LoadFromFile(filename);
        results = newvdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_SaveLoad_TestIds()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "First");
        vdb.AddText("Build5Nines is awesome!", "Second");
        var results = vdb.Search("Lion King");

        Assert.AreEqual(2, results.Texts.Count());

        var filename = "BasicMemoryVectorDatabase_SaveLoad_TestIds.b59vdb";
        vdb.SaveToFile(filename);

        var newvdb = new BasicMemoryVectorDatabase();
        newvdb.LoadFromFile(filename);

        // Add a new text entry after loading
        // This should get the next available ID (3) and not overwrite existing entries
        newvdb.AddText("A new string that should be added, not replacing existing one.", "Third");

        results = newvdb.Search("Lion King");

        Assert.AreEqual(3, results.Texts.Count());
        var listOfTexts = results.Texts.OrderBy(x => x.Id).ToArray();
        Assert.AreEqual(listOfTexts[0].Id, 1);
        Assert.AreEqual(listOfTexts[0].Metadata, "First");
        Assert.AreEqual(listOfTexts[1].Id, 2);
        Assert.AreEqual(listOfTexts[1].Metadata, "Second");
        Assert.AreEqual(listOfTexts[2].Id, 3);
        Assert.AreEqual(listOfTexts[2].Metadata, "Third");
    }

    [TestMethod]
    public async Task BasicMemoryVectorDatabase_SaveLoadBinaryStreamAsync_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "[some metadata here]");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        var stream = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(stream);

        stream.Position = 0; // Reset the stream position to the beginning
        
        var newvdb = new BasicMemoryVectorDatabase();
        await newvdb.DeserializeFromBinaryStreamAsync(stream);
        results = newvdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("[some metadata here]", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void SimpleTest_01()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void SimpleTest_02()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        vdb.AddText("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        vdb.AddText("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        vdb.AddText("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        vdb.AddText("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        vdb.AddText("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        vdb.AddText("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        vdb.AddText("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        vdb.AddText("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        vdb.AddText("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        vdb.AddText("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        vdb.AddText("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        vdb.AddText("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        vdb.AddText("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        vdb.AddText("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        var results = vdb.Search("Lion King", pageCount: 5);

        Assert.AreEqual(5, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task SimpleTest_Async_01()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        var tasks = new Task[17];
        tasks[0] = vdb.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        tasks[1] =  vdb.AddTextAsync("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        tasks[2] =  vdb.AddTextAsync("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        tasks[3] =  vdb.AddTextAsync("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        tasks[4] =  vdb.AddTextAsync("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        tasks[5] =  vdb.AddTextAsync("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        tasks[6] =  vdb.AddTextAsync("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        tasks[7] =  vdb.AddTextAsync("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        tasks[8] =  vdb.AddTextAsync("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        tasks[9] =  vdb.AddTextAsync("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        tasks[10] =  vdb.AddTextAsync("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        tasks[11] =  vdb.AddTextAsync("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        tasks[12] =  vdb.AddTextAsync("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        tasks[13] =  vdb.AddTextAsync("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        tasks[14] =  vdb.AddTextAsync("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        tasks[15] =  vdb.AddTextAsync("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        tasks[16] =  vdb.AddTextAsync("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        await Task.WhenAll(tasks);

        var results = await vdb.SearchAsync("Lion King", pageCount: 5);

        Assert.AreEqual(5, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task SimpleTest_Async_02()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        var textTasks = new Task[17];
        textTasks[0] = vdb.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        textTasks[1] =  vdb.AddTextAsync("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        textTasks[2] =  vdb.AddTextAsync("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        textTasks[3] =  vdb.AddTextAsync("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        textTasks[4] =  vdb.AddTextAsync("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        textTasks[5] =  vdb.AddTextAsync("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        textTasks[6] =  vdb.AddTextAsync("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        textTasks[7] =  vdb.AddTextAsync("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        textTasks[8] =  vdb.AddTextAsync("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        textTasks[9] =  vdb.AddTextAsync("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        textTasks[10] =  vdb.AddTextAsync("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        textTasks[11] =  vdb.AddTextAsync("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        textTasks[12] =  vdb.AddTextAsync("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        textTasks[13] =  vdb.AddTextAsync("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        textTasks[14] =  vdb.AddTextAsync("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        textTasks[15] =  vdb.AddTextAsync("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        textTasks[16] =  vdb.AddTextAsync("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        await Task.WhenAll(textTasks);

        var searchTasks = new Task<IVectorTextResult<int, string, double>>[15];
        searchTasks[0] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[1] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[2] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[3] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[4] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[5] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[6] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[7] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[8] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[9] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[10] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[11] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[12] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[13] = vdb.SearchAsync("Lion King", pageCount: 5);
        searchTasks[14] = vdb.SearchAsync("Lion King", pageCount: 5);

        await Task.WhenAll(searchTasks);

        var results = await searchTasks[0];

        Assert.AreEqual(5, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task SimpleTest_Async_03()
    {
        var vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        await vdb.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.");

        var results = await vdb.SearchAsync("Lion King", pageCount: 5);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task SimpleTest_Async_04()
    {
        var vdb = new MemoryVectorDatabase<double?>();
        
        // // Load Vector Database with some sample text
        await vdb.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.");

        var results = await vdb.SearchAsync("Lion King", pageCount: 5);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.IsNull(results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void SimpleTest_MemoryVectorDatabase_UpdateTextAndMetadata_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "5.0");

        var newText = "The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.";
        vdb.UpdateTextAndMetadata(id, newText, "6.0");

        var results = vdb.Search("Lion King", threshold: 0.001f);
        Assert.AreEqual(0, results.Texts.Count());

        results = vdb.Search("Incredibles", threshold: 0.001f);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual(newText, results.Texts.First().Text);
        Assert.AreEqual("6.0", results.Texts.First().Metadata);
        Assert.AreEqual(0.11704113334417343, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void SimpleTest_IMemoryVectorDatabase()
    {
        IVectorDatabase<int, double> vdb = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void Text_Update_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));

        vdb.UpdateText(id, "The Lion King is a great movie!");

        results = vdb.Search("Lion King");
        Assert.AreEqual("The Lion King is a great movie!", results.Texts.First().Text);
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void Text_Update_01_Chinese()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // Load Vector Database with Chinese sample text and JSON metadata
        var id = vdb.AddText("狮子王是一部1994年的迪士尼动画电影，讲述一个小狮子辛巴必将继承非洲大草原王位的故事。", "{ value: \"元数据初始值\" }");
        
        // Verify that search returns the expected text
        var results = vdb.Search("狮子");
        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("狮子王"));
        
        // Update the text
        vdb.UpdateText(id, "狮子王是一部非常棒的电影！");
        
        // Verify that the text is updated but the metadata remains unchanged
        results = vdb.Search("狮子");
        Assert.AreEqual("狮子王是一部非常棒的电影！", results.Texts.First().Text);
        Assert.AreEqual("{ value: \"元数据初始值\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void Text_Update_01_English_and_Chinese()
    {
        var vdb = new MemoryVectorDatabase<string>();

        // Load the Vector Database with some initial sample texts.
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"Lion King Metadata\" }");
        vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic about a street urchin who finds a magic lamp.", "{ value: \"Aladdin Metadata\" }");
        vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's classic animated film about Ariel.", "{ value: \"Little Mermaid Metadata\" }");

        // Add additional texts to the database.
        vdb.AddText("Mulan is an epic tale of bravery and honor in ancient China.", "{ value: \"Mulan Metadata\" }");
        vdb.AddText("Crouching Tiger, Hidden Dragon is a martial arts masterpiece with breathtaking scenes.", "{ value: \"Crouching Metadata\" }");
        vdb.AddText("In the Mood for Love is a visually stunning film about forbidden romance.", "{ value: \"In the Mood Metadata\" }");
        
        // Add more Chinese texts.
        vdb.AddText("大闹天宫是一部经典 的中国动画电影，讲述孙悟空大闹天宫的故事。", "{ value: \"元数据新增1\" }");
        vdb.AddText("霸王别姬是一部关于 爱与背叛的中国史诗电影。", "{ value: \"元数据新增2\" }");

        // Verify that a search for "Lion King" returns the expected result.
        var lionResults = vdb.Search("Lion King");
        Assert.IsTrue(lionResults.Texts.Any(t => t.Text.Contains("Lion King")));
        Assert.AreEqual("{ value: \"Lion King Metadata\" }", lionResults.Texts.First().Metadata);

        // Verify that the Chinese texts were added.
        var daNaoResults = vdb.Search("部经典");
        Assert.IsTrue(daNaoResults.Texts.Any(t => t.Text.Contains("部经典")));
        Assert.AreEqual("{ value: \"元数据新增1\" }", daNaoResults.Texts.First().Metadata);

        var baiJieResults = vdb.Search("霸王别姬");
        Assert.IsTrue(baiJieResults.Texts.Any(t => t.Text.Contains("霸王别姬")));
        Assert.AreEqual("{ value: \"元数据新增2\" }", baiJieResults.Texts.First().Metadata);

        var baiJieResults2 = vdb.Search("宫故事");
        Assert.IsTrue(baiJieResults2.Texts.Any(t => t.Text.Contains("霸王别姬")));
        Assert.AreEqual("{ value: \"元数据新增1\" }", baiJieResults2.Texts.First().Metadata);
    }

    [TestMethod]
    public void Text_Metadata_String_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public void Text_Metadata_String_Update()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task SimpleTest_BasicMemoryVectorDatabase_UpdateMetadata_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("One", "1");
        var id = vdb.AddText("Two", "2");
        vdb.AddText("Three", "3");

        var results = await vdb.SearchAsync("Two");

        var item = results.Texts.First();

        Assert.AreEqual(2, id);
        Assert.AreEqual(id, item.Id, "ID should match the one returned by AddText.");

        vdb.UpdateTextMetadata(item.Id, "222");

        results = await vdb.SearchAsync("Two");

        Assert.AreEqual(3, results.Texts.Count());
        Assert.AreEqual("Two", results.Texts.First().Text);
        Assert.AreEqual("222", results.Texts.First().Metadata);

        results = await vdb.SearchAsync("One");

        Assert.AreEqual(3, results.Texts.Count());
        Assert.AreEqual("One", results.Texts.First().Text);
        Assert.AreEqual("1", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task SimpleTest_MemoryVectorDatabase_UpdateMetadata_01()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("One", "1");
        vdb.AddText("Two", "2");
        vdb.AddText("Three", "3");

        var results = await vdb.SearchAsync("Two");

        var item = results.Texts.First();

        Assert.AreEqual(2, item.Id);

        vdb.UpdateTextMetadata(item.Id, "222");

        results = await vdb.SearchAsync("Two", pageCount: 1);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("Two", results.Texts.First().Text);
        Assert.AreEqual("222", results.Texts.First().Metadata);

        results = await vdb.SearchAsync("One", pageCount: 1);

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("One", results.Texts.First().Text);
        Assert.AreEqual("1", results.Texts.First().Metadata);
    }

    [TestMethod] 
    public async Task SimpleTest_MemoryVectorDatabase_UpdateMetadata_02()
    {
        var vdb = new MockMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("One", "1");
        var id = vdb.AddText("Two", "2");
        vdb.AddText("Three", "3");

        Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", id.ToString(), "AddText ID should not be empty.");

        var results = await vdb.SearchAsync("Two");

        foreach(var i in results.Texts)
        {
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", i.Id.ToString(), $"Search ID ({i.Text}) should not be empty.");
        }

        var item = results.Texts.First();

        vdb.UpdateText(item.Id, "TwoTwo");
        vdb.UpdateTextMetadata(item.Id, "222");

        results = await vdb.SearchAsync("Two");

        foreach(var i in results.Texts)
        {
            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", i.Id.ToString(), $"Search ID ({i.Text}) should not be empty.");
        }
    }

    [TestMethod]
    public void SimpleTest_MemoryVectorDatabase_UpdateMetadata_03()
    {
        var vdb = new MockMemoryVectorDatabase();
        
        // Load Vector Database with Chinese sample text and JSON metadata
        var id = vdb.AddText("狮子王是一部1994年的迪士尼动画电影，讲述一个小狮子辛巴必将继承非洲大草原王位的故事。", "{ value: \"元数据初始值\" }");
        
        Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", id.ToString());

        // Verify that search returns the expected text
        var results = vdb.Search("狮子");
        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("狮子王"));
        
        // Update the text
        vdb.UpdateText(id, "狮子王是一部非常棒的电影！");
        
        // Verify that the text is updated but the metadata remains unchanged
        results = vdb.Search("狮子");
        Assert.AreEqual("狮子王是一部非常棒的电影！", results.Texts.First().Text);
        Assert.AreEqual("{ value: \"元数据初始值\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase_1()
    {
        var vdb = new EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        vdb.AddText("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        vdb.AddText("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        vdb.AddText("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        vdb.AddText("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        vdb.AddText("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        vdb.AddText("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        vdb.AddText("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        vdb.AddText("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        vdb.AddText("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        vdb.AddText("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        vdb.AddText("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        vdb.AddText("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        vdb.AddText("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        vdb.AddText("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        vdb.AddText("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        vdb.AddText("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        var results = vdb.Search("Lion King", pageCount: 5);

        Assert.AreEqual(5, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.StartsWith("The Lion King is a 1994 Disney"));
        Assert.AreEqual(5.0, results.Texts.First().Metadata);
        Assert.AreEqual(1.1491886377334595, results.Texts.First().VectorComparison);
    }

    [TestMethod]
    public async Task SerializeDeserializeStream_001()
    {
        var databaseOne = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        var textTasks = new Task[17];
        textTasks[0] = databaseOne.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        textTasks[1] =  databaseOne.AddTextAsync("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        textTasks[2] =  databaseOne.AddTextAsync("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        textTasks[3] =  databaseOne.AddTextAsync("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        textTasks[4] =  databaseOne.AddTextAsync("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        textTasks[5] =  databaseOne.AddTextAsync("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        textTasks[6] =  databaseOne.AddTextAsync("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        textTasks[7] =  databaseOne.AddTextAsync("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        textTasks[8] =  databaseOne.AddTextAsync("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        textTasks[9] =  databaseOne.AddTextAsync("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        textTasks[10] =  databaseOne.AddTextAsync("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        textTasks[11] =  databaseOne.AddTextAsync("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        textTasks[12] =  databaseOne.AddTextAsync("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        textTasks[13] =  databaseOne.AddTextAsync("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        textTasks[14] =  databaseOne.AddTextAsync("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        textTasks[15] =  databaseOne.AddTextAsync("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        textTasks[16] =  databaseOne.AddTextAsync("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        await Task.WhenAll(textTasks);

        var firstResult = await databaseOne.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, firstResult.Texts.Count());
        Assert.IsTrue(firstResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, firstResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, firstResult.Texts.First().VectorComparison);

        var stream = new MemoryStream();
        databaseOne.SerializeToBinaryStream(stream);
        
        stream.Position = 0; // move to beginning of stream

        var databaseTwo = new MemoryVectorDatabase<double>();
        databaseTwo.DeserializeFromBinaryStream(stream);

        var secondResult = await databaseTwo.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, secondResult.Texts.Count());
        Assert.IsTrue(secondResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, secondResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, secondResult.Texts.First().VectorComparison);

        // Compare both results
        Assert.AreEqual(firstResult.Texts.Count(), secondResult.Texts.Count());
    }

    
    [TestMethod]
    public async Task SaveLoadFile_001()
    {
        var databaseOne = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        var textTasks = new Task[17];
        textTasks[0] = databaseOne.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0);
        textTasks[1] =  databaseOne.AddTextAsync("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0);
        textTasks[2] =  databaseOne.AddTextAsync("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0);
        textTasks[3] =  databaseOne.AddTextAsync("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0);
        textTasks[4] =  databaseOne.AddTextAsync("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0);
        textTasks[5] =  databaseOne.AddTextAsync("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0);
        textTasks[6] =  databaseOne.AddTextAsync("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0);
        textTasks[7] =  databaseOne.AddTextAsync("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0);
        textTasks[8] =  databaseOne.AddTextAsync("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0);
        textTasks[9] =  databaseOne.AddTextAsync("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0);
        textTasks[10] =  databaseOne.AddTextAsync("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0);
        textTasks[11] =  databaseOne.AddTextAsync("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0);
        textTasks[12] =  databaseOne.AddTextAsync("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0);
        textTasks[13] =  databaseOne.AddTextAsync("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0);
        textTasks[14] =  databaseOne.AddTextAsync("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0);
        textTasks[15] =  databaseOne.AddTextAsync("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0);
        textTasks[16] =  databaseOne.AddTextAsync("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0);
        
        await Task.WhenAll(textTasks);

        var firstResult = await databaseOne.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, firstResult.Texts.Count());
        Assert.IsTrue(firstResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, firstResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, firstResult.Texts.First().VectorComparison);

        var fileName = "vector_database.b59vdb";
        await databaseOne.SaveToFileAsync(fileName);
      
        var databaseTwo = new MemoryVectorDatabase<double>();
        await databaseTwo.LoadFromFileAsync(fileName);

        var secondResult = await databaseTwo.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, secondResult.Texts.Count());
        Assert.IsTrue(secondResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, secondResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, secondResult.Texts.First().VectorComparison);

        // Compare both results
        Assert.AreEqual(firstResult.Texts.Count(), secondResult.Texts.Count());

        databaseTwo.AddText("NewNewNew", 4.5);
        var thirdResult = await databaseTwo.SearchAsync("NewNewNew", pageCount: 5);
        Assert.AreEqual("NewNewNew", thirdResult.Texts.First().Text);
        Assert.AreEqual(4.5, thirdResult.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task SaveLoadFile_002()
    {
        var databaseOne = new MemoryVectorDatabase<double>();
        
        // // Load Vector Database with some sample text
        var textTasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            textTasks.Add(databaseOne.AddTextAsync("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Aladdin is a 2019 live-action Disney adaptation of the 1992 animated classic of the same name about a street urchin who finds a magic lamp and uses a genie's wishes to become a prince so he can marry Princess Jasmine.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("The Little Mermaid is a 2023 live-action adaptation of Disney's 1989 animated film of the same name. The movie is about Ariel, the youngest of King Triton's daughters, who is fascinated by the human world and falls in love with Prince Eric.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Frozen is a 2013 Disney movie about a fearless optimist named Anna who sets off on a journey to find her sister Elsa, whose icy powers have trapped their kingdom in eternal winter.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Tangled is a 2010 Disney animated comedy adventure film based on the story of Rapunzel. The movie is about a long-lost princess with magical blonde hair who has been locked in a tower her entire life by Gothel, who wants to use Rapunzel's powers for herself.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Wreck-It Ralph is a 2012 Disney animated film about Ralph, a character who plays the bad guy in the arcade game Fix-It Felix Jr. for 30 years. Ralph is a muscular, 9-foot-tall character with spiky auburn hair, a pink nose, and large hands and feet. He wears burgundy overalls with a broken strap, a plaid shirt with ripped sleeves, and a teal undershirt.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Iron Man (2008) is a Marvel Studios action, adventure, and sci-fi movie about Tony Stark (Robert Downey Jr.), a billionaire inventor and weapons developer who is kidnapped by terrorists and forced to build a weapon. Instead, Tony uses his ingenuity to build a high-tech suit of armor and escape, becoming the superhero Iron Man. He then returns to the United States to refine the suit and use it to fight crime and terrorism.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Black Panther is a 2018 Marvel Studios movie about T'Challa, the heir to the isolated African nation of Wakanda, who returns home to take the throne after his father's death. However, T'Challa faces challenges from within his own country, including Killmonger, who wants to abandon Wakanda's isolationist policies and start a global revolution. T'Challa must team up with C.I.A. agent Everett K. Ross and the Dora Milaje, Wakanda's special forces, to prevent Wakanda from being drawn into a world war.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Black Panther: Wakanda Forever is a 2022 Marvel movie about the Wakandans fighting to protect their country from world powers after the death of King T'Challa. The movie is a sequel to the popular Black Panther and stars Chadwick Boseman as T'Challa, Letitia Wright as Shuri, Angela Bassett as Ramonda, and Tenoch Huerta Mejía as Namor.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("The Incredible Hulk is a 2008 Marvel movie about scientist Bruce Banner (Edward Norton) who turns into a giant green monster called the Hulk when he's angry or frightened. After a gamma radiation accident, Banner is on the run from the military while searching for a cure for his condition.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Hackers is a 1995 American crime thriller film about a group of high school hackers who discover a criminal plot to use a computer virus to destroy five oil tankers. The film stars Jonny Lee Miller, Angelina Jolie, Jesse Bradford, Matthew Lillard, Laurence Mason, Renoly Santiago, Lorraine Bracco, and Fisher Stevens. Iain Softley directed the film, which was made during the mid-1990s when the internet was becoming popular.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("WarGames is a 1983 American techno-thriller film about a high school computer hacker who accidentally accesses a top secret military supercomputer that controls the U.S. nuclear arsenal. The hacker, David Lightman (Matthew Broderick), starts a game of Global Thermonuclear War, triggering a false alarm that threatens to start World War III. David must convince the computer that he only wanted to play a game and not the real thing, with help from his girlfriend (Ally Sheedy) and a government official (Dabney Coleman)", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Cars is a 2006 Pixar movie about a rookie race car named Lightning McQueen who gets stranded in a small town while on his way to an important race. McQueen accidentally damages the road in Radiator Springs, a forgotten town on Route 66, and is forced to repair it. While there, he meets Sally, Mater, Doc Hudson, and other characters who help him learn that there's more to life than fame and trophies. McQueen finds friendship and love in the town, and begins to reevaluate his priorities. The movie teaches McQueen the importance of caring for others, integrity, and that winning isn't everything.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("The Incredibles is a 2004 Pixar animated action-adventure film about a family of superheroes who are forced to live a normal suburban life while hiding their powers. The movie is set in a retro-futuristic 1960s and has a runtime of 1 hour and 55 minutes.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Toy Story is a 1995 animated comedy film about the relationship between Woody, a cowboy doll, and Buzz Lightyear, an action figure. The film takes place in a world where toys come to life when humans are not present. Woody is the leader of the toys in Andy's room, including a Tyrannosaurus Rex and Mr. Potato Head. When Buzz becomes Andy's favorite toy, Woody becomes jealous and plots against him. When Andy's family moves, Woody and Buzz must escape the clutches of their neighbor, Sid Phillips, and reunite with Andy.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("In Toy Story 2, Andy's toys are left to their own devices while he goes to Cowboy Camp, and Woody is kidnapped by a toy collector named Al McWhiggin. Buzz Lightyear and the other toys set out on a rescue mission to save Woody before he becomes a museum toy.", 5.0));
            textTasks.Add(databaseOne.AddTextAsync("Iron Man 2 is a 2010 action-adventure fantasy film about Tony Stark (Robert Downey Jr.), a billionaire inventor and superhero who must deal with declining health, government pressure, and a vengeful enemy.", 5.0));
        }

        await Task.WhenAll(textTasks.ToArray());

        var firstResult = await databaseOne.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, firstResult.Texts.Count());
        Assert.IsTrue(firstResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, firstResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, firstResult.Texts.First().VectorComparison);

        var fileName = "vector_database.b59vdb";
        var timer = new Stopwatch();

        timer.Start();
        await databaseOne.SaveToFileAsync(fileName);
        timer.Stop();
        Console.WriteLine($"SaveLoadFile_002: Save File: {timer.ElapsedMilliseconds} ms");

        // This is a smoke test to just make sure that the save file is not taking way longer than expected
        Assert.IsTrue(timer.ElapsedMilliseconds < 300, $"SaveLoadFile_002: Save File took too long - Expected: < 300 - Actual: {timer.ElapsedMilliseconds} ms");
      
        var databaseTwo = new MemoryVectorDatabase<double>();
        timer.Restart();
        await databaseTwo.LoadFromFileAsync(fileName);
        timer.Stop();
        Console.WriteLine($"SaveLoadFile_002: Load File: {timer.ElapsedMilliseconds} ms");

        // This is a smoke test to just make sure that the load file is not taking way longer than expected
        Assert.IsTrue(timer.ElapsedMilliseconds < 300, $"SaveLoadFile_002: Load File took too long - Expected: < 300 - Actual: {timer.ElapsedMilliseconds} ms");

        var secondResult = await databaseTwo.SearchAsync("Lion King", pageCount: 5);
        Assert.AreEqual(5, secondResult.Texts.Count());
        Assert.IsTrue(secondResult.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual(5.0, secondResult.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, secondResult.Texts.First().VectorComparison);

        // Compare both results
        Assert.AreEqual(firstResult.Texts.Count(), secondResult.Texts.Count());

        databaseTwo.AddText("NewNewNew", 4.5);
        var thirdResult = await databaseTwo.SearchAsync("NewNewNew", pageCount: 5);
        Assert.AreEqual("NewNewNew", thirdResult.Texts.First().Text);
        Assert.AreEqual(4.5, thirdResult.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadDatabaseInfo_001()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        vdb.SaveToFile("DatabaseFile_LoadDatabaseInfo_001.b59vdb");

        var databaseInfo = await DatabaseFile.LoadDatabaseInfoAsync("DatabaseFile_LoadDatabaseInfo_001.b59vdb");

        Assert.AreEqual("Build5Nines.SharpVector", databaseInfo.Schema);
        Assert.AreEqual("1.0.0", databaseInfo.Version);
        Assert.AreEqual("Build5Nines.SharpVector.MemoryVectorDatabase`1[[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", databaseInfo.ClassType);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadDatabaseInfo_002()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var stream = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(stream);
        stream.Position = 0;

        var databaseInfo = await DatabaseFile.LoadDatabaseInfoFromZipArchiveAsync(stream);

        Assert.AreEqual("Build5Nines.SharpVector", databaseInfo.Schema);
        Assert.AreEqual("1.0.0", databaseInfo.Version);
        Assert.AreEqual("Build5Nines.SharpVector.MemoryVectorDatabase`1[[System.String, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", databaseInfo.ClassType);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadStream_002()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var stream = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(stream);
        stream.Position = 0;

        vdb = await DatabaseFile.Load<MemoryVectorDatabase<string>, string>(stream);

        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadStream_003()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var stream = new MemoryStream();
        await vdb.SerializeToBinaryStreamAsync(stream);
        stream.Position = 0;

        vdb = await DatabaseFile.Load<string>(stream);
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadFile_002()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var filename = "DatabaseFile_LoadFile_003.b59vdb";
        await vdb.SaveToFileAsync(filename);

        vdb = await DatabaseFile.Load<MemoryVectorDatabase<string>, string>(filename);

        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task DatabaseFile_LoadFile_003()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        var id = vdb.AddText("The Lion King is a 1994 Disney animated film about a young lion cub named Simba who is the heir to the throne of an African savanna.", "{ value: \"JSON Metadata Value\" }");
        
        var filename = "DatabaseFile_LoadFile_003.b59vdb";
        await vdb.SaveToFileAsync(filename);

        vdb = await DatabaseFile.Load<string>(filename);
        var results = vdb.Search("Lion King");

        Assert.AreEqual(1, results.Texts.Count());
        Assert.IsTrue(results.Texts.First().Text.Contains("Lion King"));
        Assert.AreEqual("{ value: \"JSON Metadata Value\" }", results.Texts.First().Metadata);
        Assert.AreEqual(0.3396831452846527, results.Texts.First().VectorComparison);

        vdb.UpdateTextMetadata(id, "{ value: \"New Value\" }");

        results = vdb.Search("Lion King");
        Assert.AreEqual("{ value: \"New Value\" }", results.Texts.First().Metadata);
    }

    [TestMethod]
    public void EmbeddingGeneratorMemoryVectorDatabase_001()
    {
        var db = new EmbeddingGeneratorMemoryVectorDatabase();
        db.AddText("Test string", "metadata");
    }


    [TestMethod]
    public void BasicMemoryVectorDatabase_LoopThroughAllTexts_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire.", "metadata2");
        vdb.AddText("No emoji", "metadata3");

        foreach(var item in vdb)
        {
            var id = item.Id;
            var text = item.Text;
            var metadata = item.Metadata;
            var vector = item.Vector;
            //Console.WriteLine($"ID: {item.Id}, Text: {item.Text}, Metadata: {item.Metadata}");
            vdb.UpdateText(item.Id, item.Text + " - Updated");
        }
    }

    [TestMethod]
    public void BasicMemoryVectorDatabase_Search_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire", "metadata2");
        vdb.AddText("👑🔥 🏕️", "metadata3");
        
        var results = vdb.Search("🔥👑🏕️", pageCount: 1, filter: (metadata) => {
            return metadata == "metadata1";
        });

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("The 👑 King", results.Texts.First().Text);
        Assert.AreEqual(1, results.Texts.First().Id);
        Assert.AreEqual("metadata1", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task BasicMemoryVectorDatabase_SearchAsync_01()
    {
        var vdb = new BasicMemoryVectorDatabase();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire", "metadata2");
        vdb.AddText("👑🔥 🏕️", "metadata3");

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        var results = await vdb.SearchAsync("🔥👑🏕️", pageCount: 1, filter: async (metadata) => {
            return metadata == "metadata1";
        });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("The 👑 King", results.Texts.First().Text);
        Assert.AreEqual(1, results.Texts.First().Id);
        Assert.AreEqual("metadata1", results.Texts.First().Metadata);
    }

    [TestMethod]
    public async Task BasicMemoryVectorDatabase_SearchAsync_02()
    {
        var vdb = new MemoryVectorDatabase<string>();
        
        // // Load Vector Database with some sample text
        vdb.AddText("The 👑 King", "metadata1");
        vdb.AddText("It's 🔥 Fire", "metadata2");
        vdb.AddText("👑🔥 🏕️", "metadata3");

        var query = "🔥👑🏕️";
        var results = await vdb.SearchAsync(
            query,
            filter: BasicMemoryVectorDatabase_SearchAsync_02_Filter
        );

        Assert.AreEqual(1, results.Texts.Count());
        Assert.AreEqual("The 👑 King", results.Texts.First().Text);
        Assert.AreEqual(1, results.Texts.First().Id);
        Assert.AreEqual("metadata1", results.Texts.First().Metadata);
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<bool> BasicMemoryVectorDatabase_SearchAsync_02_Filter(string? metadata)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return metadata == "metadata1";
    }
}

public class MockMemoryVectorDatabase
     : MemoryVectorDatabaseBase<
        Guid,
        string,
        MemoryDictionaryVectorStore<Guid, string>,
        GuidIdGenerator,
        CosineSimilarityVectorComparer
        >
{
    public MockMemoryVectorDatabase()
        : base(
            new MockEmbeddingsGenerator(),
            new MemoryDictionaryVectorStore<Guid, string>()
            )
    { }
}

public class MockEmbeddingsGenerator : IEmbeddingsGenerator
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<float[]> GenerateEmbeddingsAsync(string text)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var val = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
        return val;
    }
}

public class EmbeddingGeneratorMemoryVectorDatabase
     : MemoryVectorDatabaseBase<
        int,
        string,
        MemoryDictionaryVectorStore<int, string>,
        IntIdGenerator,
        CosineSimilarityVectorComparer
        >
{
    public EmbeddingGeneratorMemoryVectorDatabase()
        : base(
            new MockEmbeddingsGenerator(),
            new MemoryDictionaryVectorStore<int, string>()
            )
    { }
}

public class EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int, 
        TMetadata,
        MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>,
        DictionaryVocabularyStore<string>,
        string,
        int,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizer<string, int>,
        EuclideanDistanceVectorComparer
        >
{
    public EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase()
        : base(
            new MemoryDictionaryVectorStoreWithVocabulary<int, TMetadata, DictionaryVocabularyStore<string>, string, int>(
                new DictionaryVocabularyStore<string>()
            )
        )
    { }
}