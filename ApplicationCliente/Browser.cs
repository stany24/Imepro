using LibraryData;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ApplicationCliente
{
    public class Browser : Panel
    {
        private readonly List<Tab> tabs;
        private readonly Button btnNewTab;
        public event EventHandler<NewTabEventArgs> NewTabEvent;

        public Browser()
        {
            Dock = DockStyle.Fill;
            tabs = new List<Tab>();
            btnNewTab = new Button()
            {
                Text = "new"
            };
            btnNewTab.Click += new EventHandler(NewTab);
            Controls.Add(btnNewTab);
            NewTab(new object(), new EventArgs());
        }

        private void UpdateLocations()
        {
            int CurrentOffset = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].Preview.Maximize(CurrentOffset);
                CurrentOffset += tabs[i].Preview.Width;
            }
            btnNewTab.Location = new Point(CurrentOffset, 0);
        }

        private void NewTab(object sender, EventArgs e)
        {
            Tab tab = new(Width, Height);
            tab.Webview.Disposed += new EventHandler(RemoveTabFromList);
            tab.Webview.VisibleChanged += new EventHandler(HideOtherTabs);
            tab.Webview.NewTabEvent += new EventHandler<NewTabEventArgs>(SignalNewTab);
            Resize += new EventHandler(ResizeAllWebview);
            Controls.Add(tab.Webview);
            Controls.Add(tab.Preview);
            Controls.Add(tab.ControlBar);
            tabs.Add(tab);
            UpdateLocations();
        }

        private void SignalNewTab(object sender, NewTabEventArgs e)
        {
            NewTabEvent.Invoke(this, new NewTabEventArgs(e.Url));
        }

        private void ResizeAllWebview(object sender, EventArgs e)
        {
            foreach (Tab tab in tabs)
            {
                tab.ResizeWebview(Width, Height);
            }
        }

        private void HideOtherTabs(object sender, EventArgs e)
        {
            if (!(sender as CustomWebView2).Visible) { return; }
            foreach (Tab tab in tabs)
            {
                if (tab.Webview != (sender as CustomWebView2)) { tab.HideTab(); }
            }
        }

        private void RemoveTabFromList(object sender, EventArgs e)
        {
            CustomWebView2 closedTab = sender as CustomWebView2;
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].Webview == closedTab)
                {
                    tabs.Remove(tabs[i]);
                    if (i < tabs.Count) { tabs[i].ShowTab(new object(), new EventArgs()); break; }
                    if (i > 0) { tabs[i - 1].ShowTab(new object(), new EventArgs()); break; }
                }
            }
            UpdateLocations();
        }
    }

    public class NewTabEventArgs : EventArgs
    {
        public Url Url { get; set; }
        public NewTabEventArgs(Url newUrl) { Url = newUrl; }
    }

    public class Tab
    {
        public CustomWebView2 Webview { get; set; }
        public TabPreview Preview { get; set; }
        public ControlBar ControlBar { get; set; }

        private const int GAP_BETWEEN_PREVIEW_AND_CONTROL_BAR_PX = 10;
        private const int GAP_BETWEEN_CONTROL_BAR_AND_WEBVIEW_PX = 10;

        public Tab(int width, int height)
        {
            Preview = new TabPreview();
            ControlBar = new ControlBar()
            {
                Location = new Point(0, Preview.Height + GAP_BETWEEN_PREVIEW_AND_CONTROL_BAR_PX)
            };
            Webview = new CustomWebView2()
            {
                Location = new Point(0, ControlBar.Location.Y + ControlBar.Height + GAP_BETWEEN_CONTROL_BAR_AND_WEBVIEW_PX)
            };
            ResizeWebview(width, height);

            ControlBar.btnBack.Click += new EventHandler(Webview.MoveBack_Click);
            ControlBar.btnForward.Click += new EventHandler(Webview.MoveForward_Click);
            ControlBar.btnRefresh.Click += new EventHandler(Webview.Reload_Click);
            ControlBar.btnEnter.Click += new EventHandler(Search_Click);

            Preview.btnClose.Click += new EventHandler(CloseTab);
            Preview.btnWebsiteName.Click += new EventHandler(ShowTab);

            Webview.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(UrlChanged);
        }

        public void ResizeWebview(int width, int height)
        {
            Webview.Size = new Size(width, height - Webview.Location.Y);
        }

        public void ShowTab(object sender, EventArgs e)
        {
            Webview.Show();
            ControlBar.Show();
        }

        public void HideTab()
        {
            Webview.Hide();
            ControlBar.Hide();
        }

        public void Search_Click(object sender, EventArgs e)
        {
            if (ControlBar.tbxUrl.Text.ToLower().StartsWith("https://") || ControlBar.tbxUrl.Text.ToLower().StartsWith("http://"))
            {
                try { Webview.CoreWebView2.Navigate(ControlBar.tbxUrl.Text); }
                catch { Webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + ControlBar.tbxUrl.Text + "&atb=v320-1&ia=web"); }
            }
            else
            {
                try { Webview.CoreWebView2.Navigate("https://" + ControlBar.tbxUrl.Text); }
                catch { Webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + ControlBar.tbxUrl.Text + "&atb=v320-1&ia=web"); }
            }
        }

        public void UrlChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Preview.btnWebsiteName.Text = Webview.Source.ToString();
        }

        private void CloseTab(object sender, EventArgs e)
        {
            Preview.Dispose();
            Webview.Dispose();
            ControlBar.Dispose();
        }
    }

    public class ControlBar : Panel
    {
        readonly public Button btnBack;
        readonly public Button btnForward;
        readonly public Button btnRefresh;
        readonly public TextBox tbxUrl;
        readonly public Button btnEnter;

        private const int CONTROL_BAR_HEIGHT_PX = 25;
        private const int CONTROL_BAR_WIDTH_PX = 400;
        private const int BUTTON_HEIGHT_PX = 21; //to fit the button size to the textbox height
        private const int GAP_OF_BUTTON_FROM_CONTROL_BAR_TOP_PX = (CONTROL_BAR_HEIGHT_PX - BUTTON_HEIGHT_PX) / 2;
        private const int GAP_BETWEEN_BUTTON_PX = 10;

        public ControlBar()
        {
            Size = new Size(CONTROL_BAR_WIDTH_PX, CONTROL_BAR_HEIGHT_PX);
            btnBack = new()
            {
                Size = new Size(BUTTON_HEIGHT_PX, BUTTON_HEIGHT_PX),
                Location = new Point(0 + GAP_BETWEEN_BUTTON_PX, GAP_OF_BUTTON_FROM_CONTROL_BAR_TOP_PX),
                Text = "<-"
            };
            btnForward = new()
            {
                Size = new Size(BUTTON_HEIGHT_PX, BUTTON_HEIGHT_PX),
                Location = new Point(btnBack.Location.X + btnBack.Size.Width + GAP_BETWEEN_BUTTON_PX, btnBack.Location.Y),
                Text = "->"
            };
            btnRefresh = new()
            {
                Size = new Size(BUTTON_HEIGHT_PX, BUTTON_HEIGHT_PX),
                Location = new Point(btnForward.Location.X + btnForward.Size.Width + GAP_BETWEEN_BUTTON_PX, btnBack.Location.Y),
                Text = "O"
            };
            tbxUrl = new()
            {
                Size = new Size(8 * BUTTON_HEIGHT_PX, BUTTON_HEIGHT_PX),
                Location = new Point(btnRefresh.Location.X + btnRefresh.Width + GAP_BETWEEN_BUTTON_PX, btnBack.Location.Y)
            };
            btnEnter = new()
            {
                Size = new Size(BUTTON_HEIGHT_PX, BUTTON_HEIGHT_PX),
                Location = new Point(tbxUrl.Location.X + tbxUrl.Size.Width + GAP_BETWEEN_BUTTON_PX, btnBack.Location.Y)
            };
            Controls.Add(btnBack);
            Controls.Add(btnForward);
            Controls.Add(btnRefresh);
            Controls.Add(tbxUrl);
            Controls.Add(btnEnter);
        }
    }

    public class TabPreview : Panel
    {
        public readonly Button btnWebsiteName;
        public readonly Button btnClose;

        private const int BTN_TAB_NAME_MAXIMIZED_WIDTH_PX = 100;
        private const int TAB_HEIGHT_PX = 30;
        private const int TAB_MAXIMIZED_WIDTH_PX = BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int BTN_CLOSE_TAB_WIDTH_PX = 30;

        public TabPreview()
        {
            btnWebsiteName = new Button()
            {
                Text = "new tab"
            };
            btnClose = new Button()
            {
                Text = "X"
            };
            Size = new Size(TAB_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            Controls.Add(btnWebsiteName);
            Controls.Add(btnClose);
        }

        public void Maximize(int OffsetToTheRight)
        {
            Location = new Point(OffsetToTheRight, 0);
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(btnWebsiteName.Width, 0);
        }
    }

    public class CustomWebView2 : WebView2
    {
        public event EventHandler<NewTabEventArgs> NewTabEvent;

        public CustomWebView2()
        {
            CoreWebView2InitializationCompleted += new EventHandler<CoreWebView2InitializationCompletedEventArgs>(NavigateToDefaultBrowser);
            EnsureCoreWebView2Async();
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
            NewTabEvent.Invoke(this, new NewTabEventArgs(new Url(DateTime.Now, e.Uri.ToString())));
        }

        private List<string> GetAutorisedWebsites()
        {
            try { return JsonSerializer.Deserialize<List<string>>(Properties.Settings.Default.AutorisedWebsites); }
            catch { return new List<string>() { "https://duckduckgo.com/", "https://github.com" }; }
        }
    }

    class DefaultBrowser
    {
        public DefaultBrowser(string browserUrl, string searchUrlStart, string searchUrlEnd)
        {
            BrowserUrl = browserUrl;
            SearchUrlStart = searchUrlStart;
            SearchUrlEnd = searchUrlEnd;
        }

        public string BrowserUrl { get; set; }
        public string SearchUrlStart { get; set; }
        public string SearchUrlEnd { get; set; }
    }
}
