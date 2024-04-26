using System.Net;
using Newtonsoft.Json;
using NLog;
using NLog.LayoutRenderers;
using RestSharp;

/// <summary>
/// Repository with JSON CRUD methods for api
/// </summary>

class QuestsRepoApiJson : IToDoRepo
{
    private readonly RestClientOptions options;
    private RestClient client;
    private string fileName;
    private Logger logger;

    /// <summary>
    /// Repository with JSON CRUD methods for api
    /// </summary>
    /// <param name="fileName">Name of the file to work on</param>
    /// <param name="logger"></param>
    public QuestsRepoApiJson(string fileName, Logger logger)
    {
        this.logger = logger;
        this.fileName = fileName;
        logger.Info("QuestsRepoApi: constructor");
        this.options = new RestClientOptions("http://localhost:5000");
        this.client = new RestClient(this.options);
    }

    /// <summary>
    /// Deletes a task by given ID
    /// </summary>
    /// <param name="quest">quest to delete</param>
    public void delete(Quest quest)
    {
        var request = new RestRequest("/api/Quest/DelQuest", Method.Delete);
        request.AddJsonBody(quest);
        var response = this.client.Execute(request);

        logger.Info("QuestsRepoApi: delete");
        logger.Info(response.Content);
    }

    /// <summary>
    /// Deletes current file
    /// </summary>
    public void deleteAll()
    {
        var request = new RestRequest("/api/Quest/DelAllQuests", Method.Delete);
        var response = this.client.Execute(request);
        logger.Info("QuestsRepoApi: deleteAll");
        logger.Info(response.Content);
    }

    public string getPath()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sends name of file to server
    /// </summary>
    /// <param name="fileName"></param>
    public void setPath(FilePath fileName)
    {
        var request = new RestRequest("/api/Quest/GetFileName", Method.Get);
        request.AddJsonBody(fileName);
        var response = client.Execute(request);
        logger.Info("QuestsRepoApi: setPath");
        logger.Info(response.Content);
        // try{

        // logger.Debug(response.Result);
        // logger.Info("QuestsRepoApi: getPath");
        // } catch (Exception e){
        //     logger.Fatal("getPath sie wyjebalo " + e.Message);
        // }
    }

    // public async void test(){
    //     logger.Info("QuestsRepoApi: test");
    //     var options = new RestClientOptions("http://localhost:5000");
    //     var client = new RestClient(options);
    //     var request = new RestRequest("/api/Quest/ShowAllEntries", Method.Get);
    //     logger.Info("asd");
    //     this.response = await client.GetAsync(request);
    //     logger.Info(response.Content);
    // }

    /// <summary>
    /// Reads tasks from JSON file and return them
    /// </summary>
    /// <returns>List of Quest objects</returns>
    public List<Quest> read()
    {
        logger.Info("QuestsRepoApi: read");
        var request = new RestRequest("/api/Quest/ShowAllEntries", Method.Get);
        var response = client.GetAsync(request);
        logger.Debug(response.Result.StatusCode);
        // Checks if response is faulted, then returns empty lis
        if (response.Result.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            logger.Fatal("response.IsFaulted");
            List<Quest> emptyList = new List<Quest>();
            emptyList.Clear();
            return emptyList;
        }
        else
        {
            var content = response.Result.Content;
            var quests = JsonConvert.DeserializeObject<List<Quest>>(content);
            logger.Info("quests: " + quests[0].ToString());
            return quests;
        }
    }

    /// <summary>
    /// Sends quest to api to be saved
    /// </summary>
    /// <param name="quest"></param>
    public void save(Quest quest)
    {
        var request = new RestRequest("/api/Quest/NewQuest", Method.Post);
        request.AddJsonBody(quest);
        var response = client.PostAsync(request).Result;
        logger.Info("QuestsRepoApi: save");
        logger.Info(response.Content);
    }

    /// <summary>
    /// Updates the data in existing task by given id
    /// </summary>
    /// <param name="quest"></param>
    public void upDate(Quest quest)
    {
        var request = new RestRequest("/api/Quest/UpDateQuest", Method.Patch);

        var questToUpdate = quest;

        System.Console.WriteLine("Title (keep/enter): ");
        string tittle = Console.ReadLine();
        logger.Debug(tittle);
        // Chcecks if user wants to keep the old title or enter new one
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

        request.AddJsonBody(questToUpdate);

        var response = client.PatchAsync(request).Result;
        logger.Info("QuestsRepoApi: upDate");
        logger.Info(response.Content);
    }
}
