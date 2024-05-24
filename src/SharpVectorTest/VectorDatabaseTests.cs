namespace SharpVectorTest;

using Build5Nines.SharpVector;
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
        var vdb = new MemoryVectorDatabaseAsync<double>();
        
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
        var vdb = new MemoryVectorDatabaseAsync<double>();
        
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

        var searchTasks = new Task<IVectorTextResult<double>>[15];
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
        var vdb = new MemoryVectorDatabaseAsync<double>();
        
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
        var vdb = new MemoryVectorDatabaseAsync<double?>();
        
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

    
}


public class EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase<TMetadata>
     : MemoryVectorDatabaseBase<
        int, 
        TMetadata,
        MemoryDictionaryVectorStoreAsync<int, TMetadata>,
        DictionaryVocabularyStoreAsync<string>,
        IntIdGenerator,
        BasicTextPreprocessor,
        BagOfWordsVectorizerAsync<string, int>,
        EuclideanDistanceVectorComparerAsync
        >
{
    public EuclideanDistanceVectorComparerAsyncMemoryVectorDatabase()
        : base(
            new MemoryDictionaryVectorStoreAsync<int, TMetadata>(),
            new DictionaryVocabularyStoreAsync<string>()
            )
    { }
}