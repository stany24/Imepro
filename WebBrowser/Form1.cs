using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class Browser : Form
    {
        public Browser(List<string> autorisedWebsites)
        {
            InitializeComponent();
            AutorisedWebsites = autorisedWebsites;
            visitedUrls.Add(new Uri("https://duckduckgo.com/"));
        }

        public Browser()
        {
            InitializeComponent();
            visitedUrls.Add(new Uri("https://duckduckgo.com/"));
        }

        private List<string> AutorisedWebsites = new List<string>();
        private List<Uri> visitedUrls = new List<Uri>();
        private int currentUrlId = 0;

        private void btnSearch_Click(object sender, EventArgs e)
        {

            while (currentUrlId < visitedUrls.Count)
            {
                visitedUrls.RemoveAt(visitedUrls.Count - 1);
            }
            try
            {
                webView.Source = new Uri(tbxUrl.Text);
                visitedUrls.Add(new Uri(tbxUrl.Text));

            }
            catch
            {
                webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q="+tbxUrl.Text+"&atb=v320-1&ia=web");
                visitedUrls.Add(new Uri("https://duckduckgo.com/?t=ffab&q=" + tbxUrl.Text + "&atb=v320-1&ia=web"));
            }
            currentUrlId++;
        }

        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            tbxUrl.Text = webView.Source.ToString();
        }

        private void btnMoveBack_Click(object sender, EventArgs e)
        {
            if(currentUrlId== 0) { return; }
            currentUrlId--;
            webView.Source = visitedUrls[currentUrlId];
        }

        private void btnMoveForward_Click(object sender, EventArgs e)
        {
            if(currentUrlId == visitedUrls.Count) { return; }
            currentUrlId++;
            webView.Source = visitedUrls[currentUrlId];
        }
    }
}
