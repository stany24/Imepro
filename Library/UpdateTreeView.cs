using System.Collections.Specialized;
using System.Drawing;
using Color = SixLabors.ImageSharp.Color;

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
                TreeNode current;
                if (nodeProcess != null) { current = nodeProcess.Nodes.Add(process); }
                else { current = treeProcess.Nodes.Add(process); }

                if (filterEnabled)
                {
                    if (!alertedProcesses.Contains(process)) continue;
                    current.BackColor = Color.Red;
                    while (current.Parent != null)
                    {
                        current = current.Parent;
                        current.BackColor = Color.Red;
                    }
                }
                else
                {
                    if (ignoredProcesses.Contains(process)) { current.BackColor = Color.Yellow; }
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
            foreach (var browser in browsers.Where(browser => browser.Value.Count > 0))
            {
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
        /// <param name="nodeBrowsers"></param>
        /// <param name="alertedUrls"></param>
        public static void ApplyUrlFilter(TreeNode nodeBrowsers, StringCollection alertedUrls)
        {
            for (int i = 0; i < nodeBrowsers.Nodes.Count; i++)
            {
                for (int j = 0; j < nodeBrowsers.Nodes[i].Nodes.Count; j++)
                {
                    foreach (string? t in alertedUrls)
                    {
                        if (!nodeBrowsers.Nodes[i].Nodes[j].Text.ToLower().Contains(t)) continue;
                        TreeNode nodeUrl = nodeBrowsers.Nodes[i].Nodes[j];
                        nodeUrl.BackColor = Color.Red;
                        while (nodeUrl.Parent != null)
                        {
                            nodeUrl = nodeUrl.Parent;
                            nodeUrl.BackColor = Color.Red;
                        }
                    }
                }
            }
        }
    }
}
