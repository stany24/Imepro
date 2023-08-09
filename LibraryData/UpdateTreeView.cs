using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryData
{
    public static class UpdateTreeView
    {
        public static void UpdateProcess(DataForTeacher student, TreeNode nodeProcess, TreeView treeProcess, bool filterEnabled, StringCollection alertedProcesses, StringCollection ignoredProcesses)
        {

            foreach (KeyValuePair<int, string> process in student.Processes)
            {
                TreeNode current;
                if (nodeProcess != null) { current = nodeProcess.Nodes.Add(process.Value); }
                else { current = treeProcess.Nodes.Add(process.Value); }

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
                    if (ignoredProcesses.Contains(process.Value)) { current.BackColor = Color.Yellow; }
                }
            }
        }

        public static void UpdateUrls(DataForTeacher student, TreeNode nodeNavigateurs, TreeView treeNavigateurs)
        {
            foreach (KeyValuePair<string, List<Url>> browser in student.Urls.AllBrowser)
            {
                if (browser.Value.Count > 0)
                {
                    TreeNode nodeBrowser;
                    if (nodeNavigateurs != null)
                    {
                        if (!nodeNavigateurs.Nodes.Find(browser.Key, false).Any()) { nodeNavigateurs.Nodes.Add(browser.Key, browser.Key); }
                        nodeBrowser = nodeNavigateurs.Nodes.Find(browser.Key, false)[0];
                    }
                    else
                    {
                        if (!treeNavigateurs.Nodes.Find(browser.Key, false).Any()) { treeNavigateurs.Nodes.Add(browser.Key, browser.Key); }
                        nodeBrowser = treeNavigateurs.Nodes.Find(browser.Key, false)[0];
                    }
                    for (int i = nodeBrowser.Nodes.Count; i < browser.Value.Count; i++)
                    {
                        nodeBrowser.Nodes.Add(browser.Value[i].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Fonction qui active les filtre dans les TreeViews
        /// </summary>
        /// <param name="NodeBrowsers"></param>
        public static void ApplyUrlFilter(TreeNode NodeBrowsers, StringCollection alertedUrls)
        {
            for (int i = 0; i < NodeBrowsers.Nodes.Count; i++)
            {
                for (int j = 0; j < NodeBrowsers.Nodes[i].Nodes.Count; j++)
                {
                    for (int k = 0; k < alertedUrls.Count; k++)
                    {
                        if (NodeBrowsers.Nodes[i].Nodes[j].Text.ToLower().Contains(alertedUrls[k]))
                        {
                            TreeNode NodeUrl = NodeBrowsers.Nodes[i].Nodes[j];
                            NodeUrl.BackColor = Color.Red;
                            while (NodeUrl.Parent != null)
                            {
                                NodeUrl = NodeUrl.Parent;
                                NodeUrl.BackColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }
    }
}
