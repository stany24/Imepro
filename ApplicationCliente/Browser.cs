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
        private int nextId = -1;

        private int getNextId()
        {
            nextId++;
            return nextId;
        }
        private List<Tab> tabs;
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
            tabs[0].preview.BackColor = Color.Blue;
            tabs[0].controlBar.BackColor = Color.Blue;
            NewTab(new object(), new EventArgs());
            tabs[1].preview.BackColor = Color.Red;
            tabs[1].controlBar.BackColor = Color.Red;
            NewTab(new object(), new EventArgs());
            tabs[2].preview.BackColor = Color.Yellow;
            tabs[2].controlBar.BackColor = Color.Yellow;
        }

        private void UpdateLocations()
        {
            int CurrentOffset = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].preview.Maximize(CurrentOffset);
                CurrentOffset += tabs[i].preview.Width;
            }
            btnNewTab.Location = new Point(CurrentOffset,0);
        }

        private void NewTab(object sender, EventArgs e)
        {
            Tab tab = new(getNextId());
            tab.webview.Disposed += new EventHandler(RemoveTabFromList);
            tab.webview.VisibleChanged += new EventHandler(HideOtherTabs);
            this.Resize += new EventHandler(ResizeAllWebview);
            Controls.Add(tab.webview);
            Controls.Add(tab.preview);
            Controls.Add(tab.controlBar);
            tabs.Add(tab);
            UpdateLocations();
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
            if ((sender as CustomWebView2).Visible)
            {
                foreach (Tab tab in tabs)
                {
                    if (tab.webview != (sender as CustomWebView2)) { tab.HideTab(); }
                }
            }
        }

        private void RemoveTabFromList(object sender, EventArgs e)
        {
            CustomWebView2 closedTab = sender as CustomWebView2;
            for(int i=0;i<tabs.Count;i++)
            {
                if (tabs[i].preview.Id == closedTab.Id) { tabs.Remove(tabs[i]);break; }
            }
            UpdateLocations();
        }
    }

    public class Tab
    {
        public int Id;
        public CustomWebView2 webview;
        public TabPreview preview;
        public ControlBar controlBar;

        public Tab(int id)
        {
            Id = id;
            webview = new CustomWebView2(Id){
                Location = new Point(0,100),
                Size = new Size(300,300)
            };
            preview = new TabPreview(Id);
            controlBar = new ControlBar(Id) {
                Location = new Point(0,50)};

            controlBar.btnBack.Click += new EventHandler(webview.MoveBack_Click);
            controlBar.btnForward.Click += new EventHandler(webview.MoveForward_Click);
            controlBar.btnRefresh.Click += new EventHandler(webview.Reload_Click);
            controlBar.btnEnter.Click += new EventHandler(Search_Click);

            preview.btnClose.Click += new EventHandler(CloseTab);
            preview.btnWebsiteName.Click += new EventHandler(ShowTab);
            
            webview.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(UrlChanged);
        }

        public void ResizeWebview(int width, int height)
        {
            webview.Size = new Size(width, height-webview.Location.Y);
        }

        private void ShowTab(object sender, EventArgs e)
        {
            webview.Show();
            controlBar.Show();
        }

        public void HideTab()
        {
            webview.Hide();
            controlBar.Hide();
        }

        public void Search_Click(object sender, EventArgs e)
        {
            if(controlBar.tbxUrl.Text.ToLower().StartsWith("https://") || controlBar.tbxUrl.Text.ToLower().StartsWith("http://"))
            {
                try { webview.CoreWebView2.Navigate(controlBar.tbxUrl.Text); }
                catch { webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + controlBar.tbxUrl.Text + "&atb=v320-1&ia=web"); }
            }
            else
            {
                string urlPart = controlBar.tbxUrl.Text;
                urlPart = "https://" + urlPart;
                try{webview.CoreWebView2.Navigate(urlPart);}
                catch{ webview.CoreWebView2.Navigate("https://duckduckgo.com/?t=ffab&q=" + controlBar.tbxUrl.Text + "&atb=v320-1&ia=web"); }
            }
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

        public int Id;
        readonly private int ButtonHeightPixel = 21; //to fit the button size to the textbox height
        readonly private int OffsetPixel = 10;

        public ControlBar(int id)
        {
            Id = id;
            Size = new Size(400, 40);

            btnBack = new()
            {
                Size = new Size(ButtonHeightPixel, ButtonHeightPixel),
                Location = new Point(0 + OffsetPixel, OffsetPixel),
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

        public int Id;
        private const int BTN_TAB_NAME_MAXIMIZED_WIDTH_PX = 100;
        private const int TAB_HEIGHT_PX = 30;
        private const int TAB_MAXIMIZED_WIDTH_PX = BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int BTN_CLOSE_TAB_WIDTH_PX = 30;

        public TabPreview(int id)
        {
            Id = id;
            btnWebsiteName = new Button()
            {
                Text = "new tab"
            };
            btnClose = new Button()
            {
                Text = "X"
            };
            Size = new Size(TAB_MAXIMIZED_WIDTH_PX, 30);
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
        public int Id;
        public CustomWebView2(int id)
        {
            Id = id;
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
        }

        private List<string> GetAutorisedWebsites()
        {
            try { return JsonSerializer.Deserialize<List<string>>(Properties.Settings.Default.AutorisedWebsites); }
            catch { return new List<string>() { "https://duckduckgo.com/","https://github.com" }; }
        }
    }
}
