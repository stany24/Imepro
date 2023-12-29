using System.Collections.Generic;

namespace TeacherApp;

public class StudentNode
{
    public ProcessNode ProcessNode;
    public BrowsersNode BrowsersNode;
    public string Title { get; }
    public int Id { get; }

    public StudentNode(string title)
    {
        Title = title;
    }
}

public class ProcessesNode
{
    public List<ProcessNode> ProcessNodes;
}

public class ProcessNode
{
    public string Title;
    public ProcessNode(string title)
    {
        Title = title;
    }
}

public class BrowsersNode
{
    public List<BrowserNode> BrowserNodes;
}

public class BrowserNode
{
    public string Title;
    public BrowserNode(string title)
    {
        Title = title;
    }
}