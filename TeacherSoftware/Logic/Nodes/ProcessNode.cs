namespace TeacherSoftware.Logic.Nodes;

public class ProcessNode
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ProcessNode(int id, string name)
    {
        Id = id;
        Name = name;
    }
}