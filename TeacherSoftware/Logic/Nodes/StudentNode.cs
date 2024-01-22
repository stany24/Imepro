using System.Collections.Generic;
using System.Linq;
using ClassLibrary6.History;

namespace TeacherSoftware.Logic.Nodes;

public class StudentNode
{
    public int Id { get; set; }
    public string Name{ get; set; }
    public bool Checked { get; set; }
    public ProcessesNode? Processes{ get; set; }
    public List<BrowserNode>? Browser{ get; set; }

    public StudentNode(int id)
    {
        Id = id;
        Checked = false;
    }

    public void UpdateBrowsers(History history)
    {
        Browser ??= new List<BrowserNode>();
        foreach (KeyValuePair<BrowserName, List<Url>> keyValuePair in history.AllBrowser)
        {
            BrowserNode? browser = Browser.Find(browser => browser.Name == keyValuePair.Key);
            if(browser == null){Browser.Add(new BrowserNode(keyValuePair.Key,keyValuePair.Value[0]));}
            browser = Browser[^1];
            for (int i = browser.Urls.Count; i < keyValuePair.Value.Count; i++)
            {
                browser.Urls.Add(new UrlNode(keyValuePair.Value[i]));
            }
        }
    }

    public void ApplyFilter(bool enabled)
    {
        
    }

    public void UpdateProcesses(Dictionary<int, string> processes)
    {
        Processes ??= new ProcessesNode();
        Processes.UpdateProcesses(processes);
    }
}