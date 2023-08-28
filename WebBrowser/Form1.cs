using System;
using System.Collections.Generic;
using System.Linq;
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

        readonly private List<string> AutorisedWebsites;
        readonly private List<Uri> History = new List<Uri>();

        private void Search_Click(object sender, EventArgs e)
        {
            try{webView.Source = new Uri(tbxUrl.Text);}
            catch{webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q="+tbxUrl.Text+"&atb=v320-1&ia=web");}
        }

        private void UrlChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            History.Add(webView.Source);
            tbxUrl.Text = webView.Source.ToString();
        }

        private void MoveBack_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.GoBack();
        }

        private void MoveForward_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.GoForward();
        }

        private void Reload_Click(object sender, EventArgs e)
        {
            webView.CoreWebView2.Reload();
        }

        private void NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            e.Cancel = true;
            foreach (string uri in AutorisedWebsites.Where(uri => e.Uri.StartsWith(uri)))
            {
                 e.Cancel = false;
            }
        }
    }
}
