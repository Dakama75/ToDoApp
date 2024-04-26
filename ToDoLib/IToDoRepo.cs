/// <summary>
/// Interface with CRUD methods, storing todo model in file
/// </summary>
public interface IToDoRepo
{
    string getPath();
    List<Quest> read();
    void save(Quest quest);
    void delete(Quest quest);
    void deleteAll();
    void upDate(Quest quest);
}
