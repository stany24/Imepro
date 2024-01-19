using ClassLibrary6.History;

namespace TeacherSoftware.Logic.Nodes;

public class UrlNode
{
    public Url Url{ get; set; }
    public UrlNode(Url url)
    {
        Url = url;
    }
}