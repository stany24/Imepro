using System.Collections.Generic;
using ClassLibrary6.History;

namespace TeacherSoftware.Logic.Nodes;

public class StudentNode
{
    public int Id { get; set; }
    public string Name{ get; set; }
    public bool Checked { get; set; }
    public ProcessesNode? Processes{ get; set; }
    public List<BrowserNode>? Browser{ get; set; }

    public StudentNode(int id,string name)
    {
        Id = id;
        Name = name;
        Checked = false;
    }

    public void NewUrl(BrowserName name, Url url)
    {
        Browser ??= new List<BrowserNode>();
        
        BrowserNode? browser = Browser.Find(browserNode => browserNode.Name == name);
        if (browser != null)
        {
            browser.AddUrl(url);
        }
        else
        {
            Browser.Add(new BrowserNode(name,url));
        }
    }

    public void UpdateProcesses(Dictionary<int, string> processes)
    {
        Processes ??= new ProcessesNode();
        Processes.UpdateProcesses(processes);
    }
}