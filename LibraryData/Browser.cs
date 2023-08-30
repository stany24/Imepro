﻿using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Web.WebView2.Core;

namespace LibraryData
{
    public class Browser:Panel
    {
        private WebView2 webView;
        private Button btnBack;
        private Button btnForward;
        private Button btnRefresh;
        private TextBox tbxUrl;
        private Button btnEnter;
        private TabManager tabManager;
        public Browser(List<string> autorisedWebsites)
        {
            InitializeComponent();
            AutorisedWebsites = autorisedWebsites;
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            webView = new() {
                Source = new Uri("https://duckduckgo.com")
            };
            UpdateWebViewLocation(new object(), new EventArgs());
            btnBack = new() {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(0+OffsetPixel, 0+OffsetPixel),
                Text = "<-"};
            btnForward = new() {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnBack.Location.X+ btnBack.Size.Width + OffsetPixel, btnBack.Location.Y),
                Text = "->"};
            btnRefresh = new() {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnForward.Location.X + btnForward.Size.Width + OffsetPixel, btnBack.Location.Y),
                Text = "O"};
            tbxUrl = new() {
                Size = new Size(8 * ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnRefresh.Location.X + btnRefresh.Width + OffsetPixel, btnBack.Location.Y)};
            btnEnter = new() {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(tbxUrl.Location.X + tbxUrl.Size.Width + OffsetPixel, btnBack.Location.Y)};

            Controls.Add(webView);
            Controls.Add(btnBack);
            Controls.Add(btnForward);
            Controls.Add(btnRefresh);
            Controls.Add(tbxUrl);
            Controls.Add(btnEnter);
            btnBack.MouseClick += new MouseEventHandler(MoveBack_Click);
            btnForward.MouseClick += new MouseEventHandler(MoveForward_Click);
            btnRefresh.MouseClick += new MouseEventHandler(Reload_Click);
            btnEnter.MouseClick += new MouseEventHandler(Search_Click);
            webView.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(UrlChanged);
            webView.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(NavigationStarting);
            Resize += new EventHandler(UpdateWebViewLocation);
        }

        private void UpdateWebViewLocation(object sender, EventArgs e)
        {
            webView.Location = new Point(0, ButtonHeightPixel + 2 * OffsetPixel);
            webView.Size = new Size(Width, Height - ButtonHeightPixel - 2 * OffsetPixel);
            tabManager.Size = new Size(30,Size.Width);
        }

        readonly private int ButtonHeightPixel = 21; //to fit the button size to the textbox height
        readonly private int OffsetPixel = 10;
        readonly private List<string> AutorisedWebsites;
        readonly private List<Url> History = new();

        private void Search_Click(object sender, EventArgs e)
        {
            try { webView.Source = new Uri(tbxUrl.Text); }
            catch { webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q=" + tbxUrl.Text + "&atb=v320-1&ia=web"); }
        }

        private void UrlChanged(object sender, CoreWebView2SourceChangedEventArgs e)
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

        private void NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            e.Cancel = true;
            foreach (string uri in AutorisedWebsites.Where(uri => e.Uri.StartsWith(uri)))
            {
                e.Cancel = false;
            }
        }
    }

    public class TabManager : Panel
    {
        List<Tab> tabs;
        Button btnNewTab;

        public TabManager()
        {
            Size = new Size();
        }
    }

    public class Tab:Panel
    {
        Label lblWebsiteName;
        Button btnClose;
        WebView2 webview;

        public Tab(bool isMaximized)
        {
            lblWebsiteName = new Label();
            btnClose = new Button();
            if(isMaximized) { Maximize(); }
            else { Minimize(); }
        }

        public void Minimize()
        {
            Size = new Size(30, 50);
            lblWebsiteName.Size = new Size(30, 20);
            lblWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(20, 0);
        }

        public void Maximize()
        {
            Size = new Size(30, 100);
            lblWebsiteName.Size = new Size(30, 70);
            lblWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(80, 0);
        }
    }
}
