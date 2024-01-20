using Avalonia.Controls;
using ClassLibrary6.History;

namespace TeacherSoftware.Logic.Nodes;

public class UrlNode:TextBlock
{
    public Url Url{ get; set; }
    public UrlNode(Url url)
    {
        Url = url;
    }

    public void ChangeFilter(bool activated)
    {
        if(activated){}
        else{}
    }
}