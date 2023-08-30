using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace LibraryData
{
    public class Browser:Control
    {
        private readonly WebView2 webView = new();
        private readonly Button btnBack = new();
        private readonly Button btnForward = new();
        private readonly Button btnRefresh = new();
        private readonly TextBox tbxUrl = new();
        private readonly Button btnEnter = new();
        public Browser(List<string> autorisedWebsites)
        {
            this.Controls.Add(webView);
            this.Controls.Add(btnBack);
            this.Controls.Add(btnForward);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(tbxUrl);
            this.Controls.Add(btnEnter);
            this.btnBack.MouseClick += new MouseEventHandler(this.MoveBack_Click);
            this.btnForward.MouseClick += new MouseEventHandler(this.MoveForward_Click);
            this.btnRefresh.MouseClick += new MouseEventHandler(this.Reload_Click);
            this.btnEnter.MouseClick += new MouseEventHandler(this.Search_Click);
            btnBack.Size = new Size(ButtonHeight,ButtonHeight);
            btnForward.Size = new Size(ButtonHeight,ButtonHeight);
            btnRefresh.Size = new Size(ButtonHeight,ButtonHeight);
            btnEnter.Size = new Size(ButtonHeight,ButtonHeight);
            tbxUrl.Size = new Size(ButtonHeight,4*ButtonHeight);
            btnBack.Location = new Point(0, 0);
            btnForward.Location = new Point(btnBack.Size.Width + offset, 0);
            btnRefresh.Location = new Point(btnForward.Location.X + btnForward.Size.Width + offset);
            tbxUrl.Location = new Point(btnRefresh.Location.X +btnRefresh.Width+offset, 0);
            btnEnter.Location = new Point(tbxUrl.Location.X + tbxUrl.Size.Width+offset,0);
            AutorisedWebsites = autorisedWebsites;
        }

        private int ButtonHeight;
        private int offset = 20;
        readonly private List<string> AutorisedWebsites;
        readonly private List<Url> History = new List<Url>();

        private void Search_Click(object sender, EventArgs e)
        {
            try { webView.Source = new Uri(tbxUrl.Text); }
            catch { webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q=" + tbxUrl.Text + "&atb=v320-1&ia=web"); }
        }

        private void UrlChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            History.Add(new Url(DateTime.Now,webView.Source.ToString()));
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
