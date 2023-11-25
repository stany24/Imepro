using System.Collections.Specialized;
using Avalonia.Controls;

namespace Library
{
    public static class UpdateTreeView
    {
        /// <summary>
        /// Function to update a tree-view with the processes a student is running.
        /// </summary>
        /// <param name="processes">A dictionary of process with the id as key and the name as value.</param>
        /// <param name="nodeProcess">The parent node of all the processes.</param>
        public static void UpdateProcess(Dictionary<int, string> processes, TreeViewItem nodeProcess)
        {
            nodeProcess.ItemsSource = null;
            nodeProcess.ItemsSource = processes;
        }

        /// <summary>
        /// Function to Update a tree-view with the history of urls.
        /// </summary>
        /// <param name="browsers">A dictionary with the name of the browser a key and list of url as values</param>
        /// <param name="nodeBrowser">The tree-node containing all browser.</param>
        public static void UpdateUrls(Dictionary<BrowserName, List<Url>> browsers, TreeViewItem nodeBrowser)
        {
            nodeBrowser.ItemsSource = null;
            nodeBrowser.ItemsSource = browsers;
        }

        /// <summary>
        /// Function to enable filters for urls
        /// </summary>
        /// <param name="nodeBrowsers"></param>
        /// <param name="alertedUrls"></param>
        public static void ApplyUrlFilter(TreeViewItem nodeBrowsers, StringCollection alertedUrls)
        {
        }
        
        /// <summary>
        /// Function to enable filters for processes
        /// </summary>
        /// <param name="nodeProcess"></param>
        /// <param name="alertedProcesses"></param>
        /// <param name="ignoredProcesses"></param>
        public static void ApplyProcessFilter(TreeViewItem nodeProcess, StringCollection alertedProcesses, StringCollection ignoredProcesses)
        {
        }
    }
}
