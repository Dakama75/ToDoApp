using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using LiteDB;
using NLog;

/// <summary>
/// Repository with CRUD methods for LiteDb
/// </summary>
class QuestsRepoLiteDb : IToDoRepo
{
    private readonly string fileName;
    private readonly string directoryPath = Environment.CurrentDirectory + "\\todo_db";
    private readonly string collectionName;
    private Logger logger;

    /// <summary>
    /// Repository with CRUD methods for LiteDb
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="collectionName"></param>
    public QuestsRepoLiteDb(string filePath, Logger logger, string collectionName = "Quest")
    {
        this.fileName = filePath;
        this.collectionName = collectionName;
        this.logger = logger;
        logger.Info($"QuestRepoLiteDb, constructor - called");
        logger.Debug($"fileName: {fileName}; collectionName: {collectionName}");
    }

    /// <summary>
    /// Deletes a task by given ID
    /// </summary>
    /// <param name="quest"></param>
    public void delete(Quest quest)
    {
        logger.Info($"QuestRepoLiteDb, delete() - called");
        logger.Trace(
            $"quest: id - {quest.Id}, title - {quest.Title}, priority - {quest.Priority}"
        );
        logger.Debug($"Deleting quest with id: {quest.Id}");
        using (var db = new LiteDatabase($"{this.directoryPath}\\{this.fileName}"))
        {
            var col = db.GetCollection<Quest>(this.collectionName);
            col.DeleteMany(x => x.Id == quest.Id);
        }
    }

    /// <summary>
    /// Deletes current file
    /// </summary>
    public void deleteAll()
    {
        logger.Info($"QuestRepoLiteDb, deleteAll() - called");
        File.Delete($"{this.directoryPath}\\{this.fileName}");
        logger.Debug($"File deleted");
    }

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    /// <returns>directory path</returns>
    public string getPath()
    {
        logger.Info($"QuestRepoLiteDb, getPath() - called");
        return directoryPath;
    }

    /// <summary>
    /// Reads all quets from the database
    /// </summary>
    /// <returns>Return List of Quest objects</returns>
    public List<Quest> read()
    {
        logger.Info($"QuestRepoLiteDb, read() - called");
        List<Quest> listOfQuests = new();

        using (var db = new LiteDatabase($"{this.directoryPath}\\{this.fileName}"))
        {
            // Get a collection (or create, if doesn't exist)
            var col = db.GetCollection<Quest>(collectionName);
            listOfQuests = col.Query().ToList();
            logger.Debug($"listOfQuests: {listOfQuests.ToString()}");
        }

        for (int i = 0; i < listOfQuests.Count; i++)
        {
            listOfQuests[i].Id = i + 1;
            logger.Trace($"i: {i}");
            logger.Trace($"listOfQuests[i].Id: {listOfQuests[i].Id}");
        }

        if (listOfQuests == null || !listOfQuests.Any())
        {
            logger.Debug("ListOfQuest is empty");
            return new List<Quest>();
        }
        return listOfQuests;
    }

    /// <summary>
    /// Saves quest to the database
    /// </summary>
    /// <param name="quest"></param>
    public void save(Quest quest)
    {
        logger.Info($"QuestRepoLiteDb, save() - called");
        using (var db = new LiteDatabase($"{this.directoryPath}\\{this.fileName}"))
        {
            var col = db.GetCollection<Quest>(collectionName);
            logger.Debug("col :" + col.ToString());

            col.EnsureIndex(x => x.Id);

            var res = col.Insert(quest);
        }
    }

    /// <summary>
    /// Updates an existing Quest in the Database by it's id
    /// </summary>
    /// <param name="quest"></param>
    public void upDate(Quest quest)
    {
        logger.Info($"QuestRepoLiteDb, upDate() - called");
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

                using (var db = new LiteDatabase($"{this.directoryPath}\\{this.fileName}"))
                {
                    var col = db.GetCollection<Quest>(collectionName);
                    var res = col.FindById(quest.Id);
                    res.Title = tittle;
                    col.Update(res);
                }

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
                using (var db = new LiteDatabase($"{this.directoryPath}\\{this.fileName}"))
                {
                    var col = db.GetCollection<Quest>(collectionName);
                    var res = col.FindById(quest.Id);
                    res.Priority = priority;
                    col.Update(res);
                }
                break;
        }
    }
}
