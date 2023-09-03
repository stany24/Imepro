using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.Linq;

namespace ApplicationCliente
{
    public class Browser : Panel
    {
        public List<Tab> tabs;
        private readonly Button btnNewTab;

        public Browser()
        {
            Dock = DockStyle.Fill;
            tabs = new List<Tab>();
            btnNewTab = new Button() {
                Text = "new"};
            btnNewTab.Click += new EventHandler(NewTab);
            Controls.Add(btnNewTab);
            NewTab(new object(), new EventArgs());
            UpdateLocations();
        }

        private void UpdateLocations()
        {
            int CurrentOffset = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].Maximize(new Point(CurrentOffset, 0));
                CurrentOffset += tabs[i].TAB_MAXIMIZED_WIDTH_PX;
            }
            btnNewTab.Location = new Point(CurrentOffset,0);
        }

        private void NewTab(object sender, EventArgs e)
        {
            Tab tab = new();
            tab.Disposed += new EventHandler(RemoveTabFromList);
            tab.webview.VisibleChanged += new EventHandler(HideOtherTabs);
            Controls.Add(tab);
            tabs.Add(tab);
            UpdateLocations();
        }

        private void HideOtherTabs(object sender, EventArgs e)
        {
            foreach (Tab tab in tabs)
            {
                tab.Hide();
            }
            //(sender as Tab).Show();
        }

        private void RemoveTabFromList(object sender, EventArgs e)
        {
            tabs.Remove(sender as Tab);
            UpdateLocations();
        }
    }

    public class Tab:Panel
    {
        private Button btnBack;
        private Button btnForward;
        private Button btnRefresh;
        private TextBox tbxUrl;
        private Button btnEnter;
        private readonly Button btnWebsiteName;
        private readonly Button btnClose;
        public CustomWebView2 webview;

        readonly private int ButtonHeightPixel = 21; //to fit the button size to the textbox height
        readonly private int OffsetPixel = 10;

        private const int BTN_TAB_NAME_MAXIMIZED_WIDTH_PX = 100;
        private const int BTN_TAB_NAME_MINIMIZED_WIDTH_PX = 40;
        private const int TAB_HEIGHT_PX = 30;
        public int TAB_MAXIMIZED_WIDTH_PX = BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int TAB_MINIMIZED_WIDTH_PX = BTN_TAB_NAME_MINIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int BTN_CLOSE_TAB_WIDTH_PX = 30;

        public Tab()
        {
            webview = new CustomWebView2();

            btnWebsiteName = new Button() {
                Text = "new tab"};
            btnClose = new Button() {
                Text = "X"};
            
            btnClose.Click += new EventHandler(CloseTab);
            btnWebsiteName.Click += new EventHandler(ShowTab);
            Controls.Add(btnWebsiteName);
            Controls.Add(btnClose);

            Dock = DockStyle.Fill;
            btnBack = new()
            {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(0 + OffsetPixel, 50 + OffsetPixel),
                Text = "<-"
            };
            btnForward = new()
            {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnBack.Location.X + btnBack.Size.Width + OffsetPixel, btnBack.Location.Y),
                Text = "->"
            };
            btnRefresh = new()
            {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnForward.Location.X + btnForward.Size.Width + OffsetPixel, btnBack.Location.Y),
                Text = "O"
            };
            tbxUrl = new()
            {
                Size = new Size(8 * ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(btnRefresh.Location.X + btnRefresh.Width + OffsetPixel, btnBack.Location.Y)
            };
            btnEnter = new()
            {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(tbxUrl.Location.X + tbxUrl.Size.Width + OffsetPixel, btnBack.Location.Y)
            };

            btnBack.Click += new EventHandler(webview.MoveBack_Click);
            btnForward.Click += new EventHandler(webview.MoveForward_Click);
            btnRefresh.Click += new EventHandler(webview.Reload_Click);
            btnEnter.Click += new EventHandler(Search_Click);
            webview.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(UrlChanged);

            Controls.Add(btnBack);
            Controls.Add(btnForward);
            Controls.Add(btnRefresh);
            Controls.Add(tbxUrl);
            Controls.Add(btnEnter);
        }

        private void ShowTab(object sender, EventArgs e)
        {
            Show();
        }

        public void Search_Click(object sender, EventArgs e)
        {
            try { webview.CoreWebView2.Navigate(btnWebsiteName.Text); }
            catch { webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + btnWebsiteName.Text + "&atb=v320-1&ia=web"); }
        }

        public void UrlChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            btnWebsiteName.Text = webview.Source.ToString();
        }

        private void CloseTab(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Minimize()
        {
            Size = new Size(TAB_MINIMIZED_WIDTH_PX,TAB_HEIGHT_PX);
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, 0);
        }

        public void Maximize(Point origin)
        {
            Size = new Size(TAB_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = origin;
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            origin.Offset(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX,0);
            btnClose.Location = origin;
        }
    }

    public class CustomWebView2 : WebView2
    {
        public CustomWebView2()
        {
            CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(NavigateToDefaultBrowser);
            
            NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(VerifyNavigation);
        }

        private void NavigateToDefaultBrowser(object sender, EventArgs e)
        {
            CoreWebView2.Navigate("https://duckduckgo.com/");
        }

        public void MoveBack_Click(object sender, EventArgs e)
        {
            CoreWebView2.GoBack();
        }

        public void MoveForward_Click(object sender, EventArgs e)
        {
            CoreWebView2.GoForward();
        }

        public void Reload_Click(object sender, EventArgs e)
        {
            CoreWebView2.Reload();
        }


        public void VerifyNavigation(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            e.Cancel = true;
            foreach (string uri in GetAutorisedWebsites().Where(e.Uri.StartsWith))
            {
                e.Cancel = false;
            }
        }

        private List<string> GetAutorisedWebsites()
        {
            try { return JsonSerializer.Deserialize<List<string>>(Properties.Settings.Default.AutorisedWebsites); }
            catch { return new List<string>() { "https://duckduckgo.com/" }; }
            
        }
    }
}
