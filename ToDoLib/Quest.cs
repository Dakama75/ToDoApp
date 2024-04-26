/// <summary>
/// Model of Quest
/// </summary>
public class Quest
{
    public int Id { get; set; } // id of a task
    public string? Title { get; set; } // tittle of a task
    public string? Priority { get; set; } // priority of a task

    public override string ToString()
    {
        return $"id: {Id}, title: {Title}, priority: {Priority}";
    }
}
