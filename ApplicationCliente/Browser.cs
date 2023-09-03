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
            NewTab(new object(), new EventArgs());
            NewTab(new object(), new EventArgs());
        }

        private void UpdateLocations()
        {
            int CurrentOffset = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].preview.Maximize(CurrentOffset);
                CurrentOffset += tabs[i].preview.TAB_MAXIMIZED_WIDTH_PX;
            }
            btnNewTab.Location = new Point(CurrentOffset,0);
        }

        private void NewTab(object sender, EventArgs e)
        {
            Tab tab = new();
            tab.webview.Disposed += new EventHandler(RemoveTabFromList);
            tab.webview.VisibleChanged += new EventHandler(HideOtherTabs);
            tabs.Add(tab);
            UpdateLocations();
        }

        private void HideOtherTabs(object sender, EventArgs e)
        {
            if ((sender as Tab).webview.Visible)
            {
                foreach (Tab tab in tabs)
                {
                    if (tab != (sender as Tab)) { tab.HideTab(); }
                }
            } 
        }

        private void RemoveTabFromList(object sender, EventArgs e)
        {
            tabs.Remove(sender as Tab);
            UpdateLocations();
        }
    }

    public class Tab
    {
        public CustomWebView2 webview;
        public TabPreview preview;
        public ControlBar controlBar;

        public Tab()
        {
            webview = new CustomWebView2();
            preview = new TabPreview();
            controlBar = new ControlBar();

            controlBar.btnBack.Click += new EventHandler(webview.MoveBack_Click);
            controlBar.btnForward.Click += new EventHandler(webview.MoveForward_Click);
            controlBar.btnRefresh.Click += new EventHandler(webview.Reload_Click);
            controlBar.btnEnter.Click += new EventHandler(Search_Click);

            preview.btnClose.Click += new EventHandler(CloseTab);
            preview.btnWebsiteName.Click += new EventHandler(ShowTab);
            
            webview.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(UrlChanged);
        }

        private void ShowTab(object sender, EventArgs e)
        {
            webview.Show();
            controlBar.btnBack.Show();
            controlBar.btnForward.Show();
            controlBar.btnRefresh.Show();
            controlBar.btnEnter.Show();
            controlBar.tbxUrl.Show();
        }

        public void HideTab()
        {
            webview.Hide();
            controlBar.btnBack.Hide();
            controlBar.btnForward.Hide();
            controlBar.btnRefresh.Hide();
            controlBar.btnEnter.Hide();
            controlBar.tbxUrl.Hide();
        }

        public void Search_Click(object sender, EventArgs e)
        {
            try { webview.CoreWebView2.Navigate(preview.btnWebsiteName.Text); }
            catch { webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + preview.btnWebsiteName.Text + "&atb=v320-1&ia=web"); }
        }

        public void UrlChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            preview.btnWebsiteName.Text = webview.Source.ToString();
        }

        private void CloseTab(object sender, EventArgs e)
        {
            preview.Dispose();
            webview.Dispose();
            controlBar.Dispose();
        }
    }

    public class ControlBar:Panel
    {
        readonly public Button btnBack;
        readonly public Button btnForward;
        readonly public Button btnRefresh;
        readonly public TextBox tbxUrl;
        readonly public Button btnEnter;

        readonly private int ButtonHeightPixel = 21; //to fit the button size to the textbox height
        readonly private int OffsetPixel = 10;

        public ControlBar()
        {
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
        private const int BTN_TAB_NAME_MINIMIZED_WIDTH_PX = 40;
        private const int TAB_HEIGHT_PX = 30;
        public int TAB_MAXIMIZED_WIDTH_PX = BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int TAB_MINIMIZED_WIDTH_PX = BTN_TAB_NAME_MINIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
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

            Controls.Add(btnWebsiteName);
            Controls.Add(btnClose);
        }

        public void Minimize()
        {
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, 0);
        }

        public void Maximize(int OffsetToTheRight)
        {
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(OffsetToTheRight, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + OffsetToTheRight, 0);
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
