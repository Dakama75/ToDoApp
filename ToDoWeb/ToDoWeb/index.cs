using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace ToDoWeb
{
    public class Index : PageModel
    {
        public List<Quest> Message { get; set; }
        private ILogger<Index> logger;
        private QuestsRepoApi questsRepoApi;

        public Index(ILogger<Index> logger)
        {
            this.logger = logger;
            questsRepoApi = new QuestsRepoApi(logger);
        }
        public IActionResult OnGet()
        {
            Message = this.questsRepoApi.read();
            if (Message.Count == 0)
            {
                return Page();
            }
            if (Message[0].Title.Equals("Unable to connect with api"))
            {
                return RedirectToPage("error");
            }
            return Page();
        }
        public string ChangeColor(string color)
        {
            logger.LogDebug("Color: " + color);
            switch (color)
            {
                case "high":
                    color = "#DC4C64";
                    break;
                case "medium":
                    color = "#E4A11B";
                    break;
                case "low":
                    color = "#14A44D";
                    break;
                default:
                    color = "#54B4D3";
                    break;
            }
            return color;
        }
        public void OnPost(string inputTitle, string inputPriority, bool isUpdate = false, int inputId = 0)
        {
            logger.LogInformation("Post recived");
            Quest newQuest = new();
            logger.LogDebug("id: " + inputId);
            if (inputId == 0)
            {
                logger.LogDebug("inputTitle: " + inputTitle);
                logger.LogDebug("inputPriority: " + inputPriority);
                newQuest.Title = inputTitle;
                newQuest.Priority = inputPriority;
                try
                {
                    newQuest.Id = this.questsRepoApi.read().Last<Quest>().Id + 1;
                }
                catch (Exception e)
                {
                    newQuest.Id = 1;
                    Console.WriteLine(newQuest.ToString());
                }
                this.questsRepoApi.save(newQuest);
            }
            else
            {
                newQuest.Title = inputTitle;
                newQuest.Priority = inputPriority;
                newQuest.Id = inputId;
                if (isUpdate)
                {
                    questsRepoApi.upDate(newQuest);
                }else
                {
                    questsRepoApi.delete(newQuest);
                }
            }

            Message = this.questsRepoApi.read();
        }


    }
}
