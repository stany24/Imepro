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
        }

        private List<string> AutorisedWebsites;
        private List<Uri> History = new List<Uri>();

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try{webView.Source = new Uri(tbxUrl.Text);}
            catch{webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q="+tbxUrl.Text+"&atb=v320-1&ia=web");}
        }

        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            History.Add(webView.Source);
            tbxUrl.Text = webView.Source.ToString();
        }

        private void btnMoveBack_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.GoBack();
        }

        private void btnMoveForward_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.GoForward();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.Reload();
        }

        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            e.Cancel = true;
            foreach (string uri in AutorisedWebsites)
            {
                if (e.Uri.StartsWith(uri)) { e.Cancel = false; }
            }
        }
    }
}
