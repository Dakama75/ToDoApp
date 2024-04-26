using System.Globalization;
using NLog;

/// <summary>
/// Class with main method
/// </summary>
class Program
{
    private static Logger logger;
    public static void Main(string[] args)
    {
        logger = LogManager.Setup()
            .LoadConfigurationFromFile("nlog.config")
            .GetCurrentClassLogger();

        logger.Error("program start");



        ToDoApp app = new ToDoApp(logger);
        app.Start();
    }
}