using System.Collections.Generic;
using System.Linq;

namespace TeacherSoftware.Logic.Nodes;

public class ProcessesNode
{
    public List<ProcessNode> Processes { get; set; } = new();

    public void UpdateProcesses(Dictionary<int, string> processes)
    {
        //removing old processes
        for (int i = 0; i < Processes.Count; i++)
        {
            bool exists = processes.Keys.Contains<>(Processes[i]);
            if (exists) continue;
            Processes.RemoveAt(i);
            i--;
        }
        //adding new processes
        foreach (KeyValuePair<int, string> keyValuePair in from keyValuePair in processes let proc = Processes.Find(proc => proc.Id == keyValuePair.Key) where proc == null select keyValuePair)
        {
            Processes.Add(new ProcessNode(keyValuePair.Key,keyValuePair.Value));
        }
    }
}