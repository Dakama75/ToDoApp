using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers;
using RestSharp;

/// <summary>
/// Main class of the application, with most important methods
/// </summary>
class ToDoApp
{
    //Patterns for regex
    readonly string pattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9._-]*[a-zA-Z0-9])?\.(json|db|api)$";
    readonly string patternApi = @"^[a-zA-Z0-9](?:[a-zA-Z0-9._-]*[a-zA-Z0-9])?\.(json|db)";

    private string fileName;
    private Logger logger;
    private List<string> forbiddenNames = new List<string>();
    private RepoSelector repoSelector;

    /// <summary>
    /// Main class of the application, with most important methods
    /// </summary>
    /// <param name="logger"></param>
    public ToDoApp(Logger logger)
    {
        forbiddenNames.Add("con");
        forbiddenNames.Add("prn");
        forbiddenNames.Add("aux");
        forbiddenNames.Add("nul");
        forbiddenNames.Add("com");
        forbiddenNames.Add("lpt");
        this.logger = logger;
        this.repoSelector = new RepoSelector(logger);
    }

    /// <summary>
    /// Starting method, prints menu and calls methods to create new file or load existing one
    /// </summary>
    /// <returns></returns>
    public IToDoRepo Start()
    {
        logger.Info("ToDoApp, Start() - called");
        IToDoRepo quests;

        this.PrintAvaibleFiles(repoSelector.Read());

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        System.Console.WriteLine("\n******** ENTER EXISTING OR NEW FILE NAME (help) ********");
        Console.ResetColor();
        string fileName = Console.ReadLine();
        logger.Debug("fileName: " + fileName);
        QuestsRepoApiJson test = new(fileName, this.logger);
        MatchCollection matches = Regex.Matches(fileName, this.pattern);
        //Checks if file name is valid
        if (fileName.Contains('-'))
        {
            if (fileName.Split('-')[1] == "api")
            {
                matches = Regex.Matches(fileName, this.patternApi);
                logger.Info("Patern dziala");
            }
        }
        char[] nums = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        if (forbiddenNames.Contains(Path.GetFileNameWithoutExtension(fileName).TrimEnd(nums)))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(
                "Wrong file name. Name must end with .json or .db and must be valid windows 10 file name"
            );
            logger.Info("Wrong file name");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("\nPress enter to continue");
            Console.ResetColor();
            Console.ReadLine();
            return Start();
        }

        if (fileName.Contains("del "))
        {
            if (repoSelector.Read().Count() == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("No files to delete");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine("\nPress enter to continue");
                Console.ResetColor();
                Console.ReadLine();
                return Start();
            }
            var name = fileName.Split("del ")[1];
            this.DeleteFile(name);
            return Start();
        }
        //Switch to choose action in start menu
        switch (fileName)
        {
            case "help":
                System.Console.WriteLine(
                    $"\nQuit app - end \nDelete file - del or del [file name] \nFor access to file in api add \" -api\" after file name \nFile name must end with .json or .db and must be valid windows 10 file name"
                );
                break;
            case "del":
                if (repoSelector.Read().Count() == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("No files to delete");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    System.Console.WriteLine("\nPress enter to continue");
                    Console.ResetColor();
                    Console.ReadLine();
                    return Start();
                }
                System.Console.WriteLine("Name of file to delete:");
                var fileNameToDelete = Console.ReadLine();
                logger.Debug($"fileNameToDelete: {fileNameToDelete}");
                if (fileName.Contains('-')){
                    if(fileName.Split('-')[1] == "api"){
                        
                    }
                }else {
                    this.DeleteFile(fileNameToDelete);
                }
                return Start();
            case "end":
                Environment.Exit(0);
                break;
            default:
                break;
        }

        if (matches.Count == 0)
        {
            switch (fileName)
            {
                case "help"
                or "del":
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(
                        "Wrong file name. Name must end with .json or .db and must be valid windows 10 file name"
                    );
                    logger.Info("Wrong file name");
                    break;
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("\nPress enter to continue");
            Console.ResetColor();
            Console.ReadLine();
            return Start();
        }
    quests = new QuestsRepo(fileName, this.logger);
    // Chceks if choosen file is an api file
    if (fileName.Contains('-'))
        {
            if (fileName.Split('-')[1] == "api")
            {
                matches = Regex.Matches(fileName, this.patternApi);
                logger.Info("Switch for api");

                 try
                {
                    var temp = new QuestsRepoApiJson(fileName.Split('-')[0], this.logger);
                    temp.read();
                    logger.Trace("wywala sie wczesniej");
                }
                catch (System.Exception e)
                {
                    logger.Error("Failed to connect with api");
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(
                        "Failed to connect with api. Select JSON or DB instead"
                    );
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    System.Console.WriteLine("\nPress enter to continue");
                    Console.ResetColor();
                    Console.ReadLine();
                    logger.Error(e.Message);
                    return Start();
                }

                switch(Path.GetExtension(fileName.Split('-')[0])){
                    case ".json":
                        quests = new QuestsRepoApiJson(fileName.Split('-')[0], this.logger);
                        break;
                    case ".db":
                        System.Console.WriteLine("not implemented yet");
                        break;
                    default:
                        quests = new QuestsRepoApiJson(fileName.Split('-')[0], this.logger);
                        break;
                }
                test.setPath(new FilePath(fileName.Split('-')[0]));
            }
        }else {


        // Checks if is it JSON or DB file
        switch (Path.GetExtension(fileName))
        {
            case ".json":
                quests = new QuestsRepo(fileName, this.logger);
                break;
            case ".db":
                quests = new QuestsRepoLiteDb(fileName, this.logger);
                break;
            case ".api":
                try
                {
                    var temp = new QuestsRepoApiJson(fileName, this.logger);
                    temp.read();
                    logger.Trace("wypierdala sie wczesniej");
                }
                catch (System.Exception e)
                {
                    logger.Error("Failed to connect with api");
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(
                        "Failed to connect with api. Select JSON or DB instead"
                    );
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    System.Console.WriteLine("\nPress enter to continue");
                    Console.ResetColor();
                    Console.ReadLine();
                    logger.Error(e.Message);
                    return Start();
                }

                quests = new QuestsRepoApiJson(fileName, this.logger);
                test.setPath(new FilePath(fileName));
                break;
            default:
                quests = new QuestsRepo(fileName, this.logger);
                break;
        }
        }
        this.fileName = fileName;

        //If something went wrong with the file, it pop ups and asking if user want to open current directory or exit
        try
        {
            this.QuestListMenu(quests);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Access denied \n Open folder directory [y/n]?");
            switch (Console.ReadLine())
            {
                case "y"
                or "yes":
                    Process.Start("explorer.exe", quests.getPath());
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
            Console.ResetColor();

            logger.Error("It's here" + e.Message);
        }

        return quests;
    }

    /// <summary>
    /// Prints all available files in the directories
    /// </summary>
    /// <param name="listOfFilesNames">List with names of files</param>
    private void PrintAvaibleFiles(List<string> listOfFilesNames)
    {
        this.logger.Info("ToDoApp, Start() - called");

        Console.Clear();
        if (listOfFilesNames.Count == 0)
        {
            return;
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        // Prints all avaible files localy
        System.Console.WriteLine("********************* TODO LISTS LOCAL *********************");
        Console.ResetColor();

        foreach (var item in listOfFilesNames)
        {
            if (item[0].Equals('*'))
            {
                System.Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.WriteLine($"{item}");
                Console.ResetColor();
                continue;
            }
            System.Console.WriteLine($" – {item}");
        }
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        //Prints all avaible files in api
        System.Console.WriteLine("\n********************** TODO LISTS API **********************");
        Console.ResetColor();

        try
        {
            logger.Info("ToDoApp, showListOfFilesNames: dziala");
            var options = new RestClientOptions("http://localhost:5000");
            var client = new RestClient(options);
            var request = new RestRequest("/api/Quest/GetListOfFileNames");
            var response = client.GetAsync(request).Result.Content.Split(",");
            logger.Debug(response[0].Trim('[').Trim(']').Trim('"'));

            foreach (var item in response)
            {
                if (item.Trim('[').Trim(']').Trim('"')[0].Equals('*'))
                {
                    System.Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.WriteLine($"{item.Trim('[').Trim(']').Trim('"')}");
                    Console.ResetColor();
                    continue;
                }
                System.Console.WriteLine($" – {item.Trim('[').Trim(']').Trim('"')}");
            }
        }
        catch (Exception e)
        {
            logger.Error("ToDoApp, showListOfFilesNames: " + e.Message);
            logger.Error("Failed to connect with api");
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(
                        "Failed to connect with api"
                    );
                    Console.ResetColor();
        }

        return;
    }

    /// <summary>
    /// Method, that get data from input and send it to IToDoRepo.save()
    /// </summary>
    /// <param name="quests"></param>

    private void CollectDataToSave(IToDoRepo quests)
    {
        logger.Info("ToDoApp, callectDataToSave() - called");
        Quest newQuest = new Quest();
        // Setting id of a task
        try
        {
            newQuest.Id = quests.read().Last<Quest>().Id + 1;
        }
        catch (Exception e)
        {
            this.logger.Error("Collect data to save: " + e);
            newQuest.Id = 1;
        }

        Console.Clear();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        System.Console.WriteLine("******** New Task  ******** \n");
        Console.ResetColor();

        // Asking for tittle
        Console.WriteLine("Tittle:");
        newQuest.Title = Console.ReadLine();
        logger.Debug("newQuest.Tittle: " + newQuest.Title);
        // Asking for priority
        Console.Write("Priority (");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("low");
        Console.ResetColor();
        Console.Write("/");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("medium");
        Console.ResetColor();
        Console.Write("/");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("high");
        Console.ResetColor();
        Console.WriteLine("): ");
        newQuest.Priority = Console.ReadLine();
        logger.Debug("newQuest.Priority: " + newQuest.Priority);

        Console.Clear();

        //Saving
        quests.save(newQuest);
    }

    /// <summary>
    /// Method, that prints all quests
    /// </summary>
    /// <param name="quests"></param>
    private void PrintQuests(IToDoRepo quests)
    {
        logger.Info("ToDoApp, PrintQuests() - called");
        Console.Clear();
        if (quests.read().Count == 0)
        {
            CollectDataToSave(quests);
        }

        // Writing tasks
        string printTemp =
            $"*********** {Path.GetFileNameWithoutExtension(this.fileName).ToUpper()} ***********";
        Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(printTemp);
        Console.ResetColor();

        foreach (var quest in quests.read())
        {
            logger.Trace($"quest: {quest}");
            Color(quest.Priority);
            Console.WriteLine(
                $"{quest.Id}.\n Task: {quest.Title} \n Priority: {quest.Priority} \n"
            );
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < printTemp.Length; i++)
        {
            Console.Write('*');
        }
        System.Console.WriteLine();
        Console.ResetColor();
    }

    /// <summary>
    /// Menu with actions on ToDO list (creatnig, reading, updating and deleting qusts)
    /// </summary>
    /// <param name="questsRepo"></param>
    /// <returns></returns>
    private string QuestListMenu(IToDoRepo questsRepo)
    {
        logger.Info("ToDoApp, QuestListMeny() - called");

        this.PrintQuests(questsRepo);
        Console.WriteLine(
            "\nChoose action: \nNew task - new \nUpdate - update \nDelete - del \nDelete All - del all \nBack (choosing file) - back \nQuit app - end \n"
        );

        var action = Console.ReadLine().ToLower();
        logger.Debug("action: " + action);

        switch (action)
        {
            case "new":

                CollectDataToSave(questsRepo);
                this.PrintQuests(questsRepo);
                break;

            case "del":
                int id = 0;
                if (questsRepo.read().Count != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("id:");
                    var givenId = Console.ReadLine();
                    try
                    {
                        id = int.Parse(givenId) - 1;
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        logger.Debug("Can't parse givenId, givenId: " + givenId);
                        Console.WriteLine("Wrong ID, ID must be a number");
                        Thread.Sleep(1500);
                        Console.ResetColor();
                        break;
                    }
                    Console.ResetColor();
                }
                if (id > questsRepo.read().Count - 1 || id < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(
                        $"Error! Wrong ID, Highest id is {questsRepo.read().Count}"
                    );
                    logger.Info("Id out of range for delete");
                    Thread.Sleep(1500);

                    Console.ResetColor();
                    break;
                }

                questsRepo.delete(questsRepo.read()[id]);

                this.PrintQuests(questsRepo);

                break;
            case "del all"
            or "delall"
            or "dellall":

                questsRepo.deleteAll();
                Console.Clear();

                this.Start();

                break;
            case "update":
                int idUp = 0;
                if (questsRepo.read().Count != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("id:");
                    var givenId = Console.ReadLine();
                    try
                    {
                        idUp = int.Parse(givenId) - 1;
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        logger.Debug("Can't parse givenId, givenId: " + givenId);
                        Console.WriteLine("Wrong ID, ID must be a number");
                        Thread.Sleep(1500);
                        Console.ResetColor();
                        break;
                    }
                    Console.ResetColor();
                }
                else
                {
                    idUp = 0;
                }
                if (idUp > questsRepo.read().Count - 1 || idUp < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    logger.Info("Id out of range for update");
                    System.Console.WriteLine($"Wrong ID, Highest id is {questsRepo.read().Count}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
                }

                questsRepo.upDate(questsRepo.read()[idUp]);

                this.PrintQuests(questsRepo);
                break;
            case "back":
                Console.Clear();
                this.Start();
                this.PrintQuests(questsRepo);
                break;
            case "end":
                Environment.Exit(0);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                logger.Info("Unknown command");
                Console.WriteLine("Unknown command");
                Thread.Sleep(1000);
                Console.ResetColor();
                break;
        }
        return QuestListMenu(questsRepo);
    }

    /// <summary>
    /// Deletes JSON or DB file by given file name
    /// </summary>
    /// <param name="fileName"></param>
    private void DeleteFile(string fileName)
    {
        logger.Info("ToDoApp, DeleteFile() - called");
        logger.Debug("fileName: " + fileName);
        if (repoSelector.Read().Contains(fileName))
        {
            if (Path.GetExtension(fileName) == ".db")
            {
                File.Delete(Environment.CurrentDirectory + "\\todo_db\\" + fileName);
            }
            else
            {
                File.Delete(Environment.CurrentDirectory + "\\todo_json\\" + fileName);
            }
        }
        else
        {
            System.Console.WriteLine("File with given name doesn't exists");
            logger.Debug("Wrong file name");
            Thread.Sleep(2000);
        }
    }

    /// <summary>
    /// Method that changes color of task, depening of priority
    /// </summary>
    /// <param name="priority"></param>
    void Color(string priority)
    {
        logger.Info("ToDoApp, Color() - called");

        Console.ForegroundColor = priority switch
        {
            "low" => ConsoleColor.Green,
            "medium" => ConsoleColor.Yellow,
            "high" => ConsoleColor.Red,
            _ => ConsoleColor.Blue,
        };
    }
}
