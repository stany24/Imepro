using System.Collections.Generic;
using ClassLibrary6.History;

namespace TeacherSoftware.Logic.Nodes;

public class BrowserNode
{
    public BrowserName Name { get; set; }
    public List<UrlNode> Urls { get; set; }

    public BrowserNode(BrowserName name, Url url)
    {
        Name = name;
        Urls = new List<UrlNode>{new(url)};
    }

    public void AddUrl(Url url)
    {
        Urls.Add(new UrlNode(url));
    }
}