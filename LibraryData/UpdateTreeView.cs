using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryData
{
    public static class UpdateTreeView
    {
        public static void UpdateProcess(DataForTeacher student,TreeNode nodeProcess,TreeView treeProcess, bool filterEnabled, StringCollection alertedProcesses, StringCollection ignoredProcesses)
        {

            foreach (KeyValuePair<int, string> process in student.Processes)
            {
                TreeNode current;
                if (nodeProcess != null){current = nodeProcess.Nodes.Add(process.Value);}
                else{current = treeProcess.Nodes.Add(process.Value);}
                
                if (filterEnabled)
                {
                    if (alertedProcesses.Contains(process.Value))
                    {
                        current.BackColor = Color.Red;
                        while (current.Parent != null)
                        {
                            current = current.Parent;
                            current.BackColor = Color.Red;
                        }
                    }
                }
                else
                {
                    if (ignoredProcesses.Contains(process.Value))
                    {
                        current.BackColor = Color.Yellow;
                    }
                }
            }
        }

        public static void UpdateUrls(DataForTeacher student, TreeNode nodeNavigateurs,TreeView treeNavigateurs)
        {
            foreach (KeyValuePair<string, List<Url>> browser in student.Urls.AllBrowser)
            {
                if (browser.Value.Count > 0)
                {
                    TreeNode nodeBrowser;
                    if (nodeNavigateurs!= null){
                        if (nodeNavigateurs.Nodes.Find(browser.Key, false).Count() == 0) { nodeNavigateurs.Nodes.Add(browser.Key, browser.Key); }
                        nodeBrowser = nodeNavigateurs.Nodes.Find(browser.Key, false)[0];
                    }
                    else{
                        if (treeNavigateurs.Nodes.Find(browser.Key, false).Count() == 0) { treeNavigateurs.Nodes.Add(browser.Key, browser.Key); }
                        nodeBrowser = treeNavigateurs.Nodes.Find(browser.Key, false)[0];
                    }
                    for (int i = nodeBrowser.Nodes.Count; i < browser.Value.Count; i++)
                    {
                        nodeBrowser.Nodes.Add(browser.Value[i].ToString());
                    }
                }
            }
        }
    }
}
