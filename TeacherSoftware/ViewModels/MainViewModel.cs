using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClassLibrary6.History;
using TeacherSoftware.Logic.Nodes;

namespace TeacherSoftware.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<StudentNode> Nodes { get; } = new();
    public bool TrayIconVisible { get; set; } = false;

    public void NewStudent(int id)
    {
        Nodes.Add(new StudentNode(id));
    }
    
    public void RemoveStudent(int id)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].Id != id) continue;
            Nodes.RemoveAt(i);
            break;
        }
    }

    public void UpdateProcesses(int id,Dictionary<int, string> processes)
    {
        foreach (StudentNode node in Nodes)
        {
            if (node.Id != id) continue;
            node.UpdateProcesses(processes);
            break;
        }
    }

    public void UpdateBrowsers(int id,History history)
    {
        foreach (StudentNode node in Nodes)
        {
            if (node.Id != id) continue;
            
            node.UpdateBrowsers(history);
        }
    }
}