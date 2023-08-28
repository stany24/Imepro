﻿using System;
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
            webView.Source = visitedUrls[0];
        }

        private List<string> AutorisedWebsites = new List<string>();
        private List<Uri> visitedUrls = new List<Uri>();
        private int currentUrlId = 0;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try{webView.Source = new Uri(tbxUrl.Text);}
            catch{webView.Source = new Uri("https://duckduckgo.com/?t=ffab&q="+tbxUrl.Text+"&atb=v320-1&ia=web");}
        }

        private void webView_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            while (currentUrlId < visitedUrls.Count - 1)
            {
                visitedUrls.RemoveAt(visitedUrls.Count - 1);
            }
            visitedUrls.Add(webView.Source);
            currentUrlId++;
            btnMoveBack.Enabled = true;
            btnMoveForward.Enabled = false;
            tbxUrl.Text = webView.Source.ToString();
        }

        private void btnMoveBack_Click(object sender, EventArgs e)
        {
            if(currentUrlId== 0) { return; }
            currentUrlId--;
            webView.Source = visitedUrls[currentUrlId];
            btnMoveForward.Enabled = true;
            if (currentUrlId == 0) { btnMoveBack.Enabled = false; }
        }

        private void btnMoveForward_Click(object sender, EventArgs e)
        {
            if(currentUrlId == visitedUrls.Count-1) { return; }
            currentUrlId++;
            webView.Source = visitedUrls[currentUrlId];
            btnMoveBack.Enabled = true;
            if(currentUrlId == visitedUrls.Count-1) { btnMoveForward.Enabled = false; }
        }
    }
}
