
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using System.Net;

namespace ToDoWeb
{
    public class QuestsRepoApi : IToDoRepo
    {
        private RestClient _restClient;
        private RestClientOptions _restClientOptions;
        private ILogger<Index> _logger;
        public QuestsRepoApi(ILogger<Index> logger)
        {
            this._restClientOptions = new RestClientOptions("http://localhost:5000");
            this._restClient = new RestClient(this._restClientOptions);
            this._logger = logger;
        }
        public void delete(Quest quest)
        {
            var request = new RestRequest("/api/Quest/DelQuest", Method.Delete);
            request.AddJsonBody(quest);
            var response = this._restClient.Execute(request);
        }

        public void deleteAll()
        {
            throw new NotImplementedException();
        }

        public string getPath()
        {
            throw new NotImplementedException();
        }

        public void setPath(FilePath fileName)
        {
            var request = new RestRequest("/api/Quest/GetFileName", Method.Get);
            request.AddJsonBody(fileName);
            var response = _restClient.Execute(request);
            _logger.LogInformation("QuestsRepoApi: setPath");
            _logger.LogInformation(response.Content);
            // try{

            // logger.Debug(response.Result);
            // logger.Info("QuestsRepoApi: getPath");
            // } catch (Exception e){
            //     logger.Fatal("getPath sie wyjebalo " + e.Message);
            // }
        }


        public List<Quest> read()
        {
            Console.WriteLine("Read");
            try
            {

                var request = new RestRequest("/api/Quest/ShowAllEntries", Method.Get);
                var keyBytes = System.Text.Encoding.UTF8.GetBytes("Kur*/wa");
                request.AddHeader("authorization", Convert.ToBase64String(keyBytes));
                var response = _restClient.GetAsync(request);
                // Checks if response is faulted, then returns empty lis
                Console.WriteLine(response.Result.StatusCode);
                if (response.Result.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    List<Quest> emptyList = new List<Quest>();
                    emptyList.Clear();
                    return emptyList;
                }
                else
                {
                    var content = response.Result.Content;
                    var quests = JsonConvert.DeserializeObject<List<Quest>>(content);
                    return quests;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                List<Quest> emptyList = new List<Quest>();
                var quest = new Quest();
                quest.Title = "Unable to connect with api";
                emptyList.Add(quest);
                return emptyList;
            }
        }

        public void save(Quest quest)
        {
            try
            {

                var request = new RestRequest("/api/Quest/NewQuest", Method.Post);
                request.AddJsonBody(quest);
                var response = _restClient.PostAsync(request).Result;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{e.Message}");
            }
        }

        public void upDate(Quest quest)
        {
            try
            {

                var request = new RestRequest("/api/Quest/UpDateQuest", Method.Patch);

                var questToUpdate = quest;
                request.AddJsonBody(questToUpdate);

                var response = _restClient.PatchAsync(request).Result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }
    }
}
