using Microsoft.AspNetCore.Mvc;

namespace ToDoWeb
{
    public class IndexController : Controller
    {
        [Route("/Home/Test", Name = "Custom")]
        public string Test()
        {
            Console.WriteLine("Usun");
            return "Usun";
        }
    }
}
