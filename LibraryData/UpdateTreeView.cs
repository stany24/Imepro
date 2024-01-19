using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryData
{
    public static class UpdateTreeView
    {
        /// <summary>
        /// Function to update a treeview with the processes a student is running.
        /// </summary>
        /// <param name="processes">A dictionary of process with the id as key and the name as value.</param>
        /// <param name="nodeProcess">The parent node of all the processes.</param>
        /// <param name="treeProcess">The treeview you want to update.</param>
        /// <param name="filterEnabled">If the filters are enabled or not.</param>
        /// <param name="alertedProcesses">List of alerted processes.</param>
        /// <param name="ignoredProcesses">List of ignored processes.</param>
        public static void UpdateProcess(Dictionary<int, string> processes, TreeNode nodeProcess, TreeView treeProcess, bool filterEnabled, StringCollection alertedProcesses, StringCollection ignoredProcesses)
        {
            foreach (string process in processes.Select(process => process.Value))
            {
                if (ignoredProcesses.Contains(process)) { continue; }
                TreeNode current;
                if (nodeProcess != null) { current = nodeProcess.Nodes.Add(process); }
                else { current = treeProcess.Nodes.Add(process); }
                
                if (filterEnabled && alertedProcesses.Contains(process))
                {
                    current.BackColor = Color.Red;
                    while (current.Parent != null)
                    {
                        current = current.Parent;
                        current.BackColor = Color.Red;
                    }
                }
            }
        }

        /// <summary>
        /// Function to Update a treeview with the historique of urls.
        /// </summary>
        /// <param name="browsers">A dictionnary with the name of the browser a key and list of url as values</param>
        /// <param name="nodeBrowser">The treenode containing all browser.</param>
        /// <param name="treeBrowser">The treeview you want to update.</param>
        public static void UpdateUrls(Dictionary<BrowserName, List<Url>> browsers, TreeNode nodeBrowser, TreeView treeBrowser)
        {
            foreach (KeyValuePair<BrowserName, List<Url>> browser in browsers)
            {
             if (browser.Value.Count <= 0) {continue; }
                TreeNode Browser;
                if (nodeBrowser != null)
                {
                    if (!nodeBrowser.Nodes.Find(browser.Key.ToString(), false).Any()) { nodeBrowser.Nodes.Add(browser.Key.ToString(), browser.Key.ToString()); }
                    Browser = nodeBrowser.Nodes.Find(browser.Key.ToString(), false)[0];
                }
                else
                {
                    if (!treeBrowser.Nodes.Find(browser.Key.ToString(), false).Any()) { treeBrowser.Nodes.Add(browser.Key.ToString(), browser.Key.ToString()); }
                    Browser = treeBrowser.Nodes.Find(browser.Key.ToString(), false)[0];
                }
                for (int i = Browser.Nodes.Count; i < browser.Value.Count; i++)
                {
                    Browser.Nodes.Add(browser.Value[i].ToString());
                }
            }
        }

        /// <summary>
        /// Fonction to enable filters for urls
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
