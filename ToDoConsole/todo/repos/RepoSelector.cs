using System.Drawing;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NLog;

/// <summary>
/// Repo to CRUD on list of todo files list
/// </summary>
class RepoSelector
{
    private Logger logger;

    /// <summary>
    /// Class used to choosing repo
    /// </summary>
    /// <param name="filePath">Path to file that stores avaible repos</param>
    public RepoSelector(Logger logger)
    {
        this.logger = logger;
    }

    private readonly string directoryPathJson = Environment.CurrentDirectory + "\\todo_json";
    private readonly string directoryPathDB = Environment.CurrentDirectory + "\\todo_db";

    /// <summary>
    /// Reads files from json and db directories
    /// </summary>
    /// <returns>List of file names from directory with JSON files</returns>
    public List<string> Read()
    {
        IEnumerable<string> ListOfJsonFiles = new List<string>();
        IEnumerable<string> ListOfDbFiles = new List<string>();

        // Exeption if folder doesn't exist for JSON
        try
        {
            ListOfJsonFiles = Directory.EnumerateFiles(directoryPathJson);
            logger.Trace("List of DB Files: " + ListOfJsonFiles.ToString());
        }
        catch
        {
            logger.Error("Folder doesn't exist, new one has been created");
            Directory.CreateDirectory(directoryPathJson);
        }

        // Exeption if folder doesn't exist for DB
        try
        {
            logger.Trace("List of DB Files: " + ListOfDbFiles.ToString());
            ListOfDbFiles = Directory.EnumerateFiles(directoryPathDB);
        }
        catch (Exception e)
        {
            logger.Error("Folder doesn't exist, new one has been created");
            Directory.CreateDirectory(directoryPathDB);
        }
        bool isAny = false;
        List<string> ListOfFileNames = new();
        if (ListOfJsonFiles.Any())
        {
            isAny = true;
            ListOfFileNames.Add("******** JSON ********");
            foreach (var item in ListOfJsonFiles)
            {
                ListOfFileNames.Add(Path.GetFileName(item));
            }
        }
        if (ListOfDbFiles.Any())
        {
            isAny = true;
            ListOfFileNames.Add("******* LiteDB *******");
            foreach (var item in ListOfDbFiles)
            {
                ListOfFileNames.Add(Path.GetFileName(item));
            }
        }
        if (!isAny)
        {
            ListOfFileNames.Add("No files in directory");
        }
        logger.Trace("Full list of files: " + ListOfFileNames.ToString());
        return ListOfFileNames;
    }
}
