/// <summary>
/// Object use only to send name of file to api
/// </summary>
public class FilePath
{
    public string? path {get; set;}
/// <summary>
/// Object use only to send name of file to api
/// </summary>
/// <param name="path"></param>
    public FilePath(string path)
    {
        this.path = path;
    }
}