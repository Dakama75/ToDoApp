using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Newtonsoft.Json;
using NLog;

/// <summary>
/// Repository with CRUD methods for JSON
/// </summary>
class QuestsRepo : IToDoRepo
{
    private readonly string fileName;
    private readonly string directoryPath = Environment.CurrentDirectory + "\\todo_json";
    private Logger logger;

    /// <summary>
    /// Repository with CRUD methods for JSON
    /// </summary>
    /// <param name="fileName">Name  of the file to work on</param>
    public QuestsRepo(string fileName, Logger logger)
    {
        this.fileName = fileName;
        this.logger = logger;
        logger.Info("QuestRepo, constructor - called");
        logger.Debug($"fileName: {fileName};");
    }

    /// <summary>
    /// Deletes a task by given ID
    /// </summary>
    /// <param name="quest"></param>
    public void delete(Quest quest)
    {
        logger.Info($"QuestRepo, delete() - called");
        List<Quest> questList = this.read();
        questList.RemoveAt(quest.Id - 1);
        logger.Debug($"Deleting quest with id: {quest.Id - 1}");
        this.WriteToJson(questList);
    }

    /// <summary>
    /// Deletes current file
    /// </summary>
    public void deleteAll()
    {
        logger.Info($"QuestRepo, deleteAll() - called");
        File.Delete($"{this.directoryPath}\\{this.fileName}");
        logger.Debug($"File deleted");
    }

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    /// <returns>directory path</returns>
    public string getPath()
    {
        logger.Info($"QuestRepo, getPath() - called");
        return directoryPath;
    }

    /// <summary>
    /// Reads tasks from JSON file and return them as a  list of Quest objects
    /// </summary>
    /// <returns></returns>
    public List<Quest> read()
    {
        logger.Info($"QuestRepo, read() - called");
        try
        {
            using (StreamReader r = new StreamReader($"{this.directoryPath}\\{this.fileName}"))
            {
                string jsonString = r.ReadToEnd();
                List<Quest> quests = JsonConvert.DeserializeObject<List<Quest>>(jsonString);

                logger.Debug($"quests: {quests.ToString()}");

                for (int i = 0; i < quests.Count; i++)
                {
                    quests[i].Id = i + 1;
                    logger.Trace($"i: {i}");
                    logger.Trace($"listOfQuests[i].Id: {quests[i].Id}");
                }
                r.Close();
                return quests;
            }
        }
        catch (Exception e)
        {
            logger.Error("QuestRepo.read()" + e.Message);

            List<Quest> emptyList = new List<Quest>();
            emptyList.Clear();
            return emptyList;
        }
    }

    /// <summary>
    /// Saves task to a JSON file
    /// </summary>
    /// <param name="quest"></param>

    public void save(Quest quest)
    {
        logger.Info($"QuestRepo, save() - called");
        List<Quest> questList = this.read();

        questList.Add(quest);

        this.WriteToJson(questList);
    }

    /// <summary>
    /// Updates the data in existing task by given id
    /// </summary>
    /// <param name="quest"></param>

    public void upDate(Quest quest)
    {
        logger.Info($"QuestRepo, upDate() - called");
        List<Quest> questList = this.read();
        Quest questToUpdate = questList[quest.Id - 1];
        System.Console.WriteLine("Title (keep/enter): ");
        string tittle = Console.ReadLine();
        logger.Debug(tittle);
        switch (tittle)
        {
            case "keep"
            or "":
                logger.Trace("Tittle keep");
                break;
            default:
                logger.Trace("Tittle change");
                questToUpdate.Title = tittle;
                break;
        }
        System.Console.WriteLine("Priority (keep/enter): ");
        string priority = Console.ReadLine();
        logger.Debug(priority);
        switch (priority)
        {
            case "keep"
            or "":
                logger.Trace("Priority keep");
                break;
            default:
                logger.Trace("Priority change");
                questToUpdate.Priority = priority;
                break;
        }

        questList[quest.Id - 1] = questToUpdate;

        this.WriteToJson(questList);
    }

    /// <summary>
    /// Writes List of Quest objects to JSON file
    /// </summary>
    /// <param name="questList"></param>
    private void WriteToJson(List<Quest> questList)
    {
        logger.Info($"QuestRepo, WriteToJson() - called");

        try
        {
            string output = JsonConvert.SerializeObject(questList);
            logger.Debug($"output: {output}");
            using (StreamWriter sw = new StreamWriter($"{this.directoryPath}\\{this.fileName}"))
            {
                sw.Write(output);
                sw.Close();
            }
        }
        catch (System.Exception e)
        {
            logger.Error("QuestRepo.WriteToJson()" + e.Message);
        }
    }
}
