using Microsoft.Web.WebView2.WinForms;
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
        private WebView2 OpenedTab;
        private Button btnBack;
        private Button btnForward;
        private Button btnRefresh;
        private TextBox tbxUrl;
        private Button btnEnter;
        private TabManager tabManager;
        readonly private int ButtonHeightPixel = 21; //to fit the button size to the textbox height
        readonly private int OffsetPixel = 10;
        readonly private List<string> AutorisedWebsites;

        public Browser(List<string> autorisedWebsites)
        {
            InitializeComponent();
            AutorisedWebsites = autorisedWebsites;
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            tabManager = new TabManager(Width);
            OpenedTab = tabManager.tabs[0].webview;
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

            Controls.Add(OpenedTab);
            Controls.Add(btnBack);
            Controls.Add(btnForward);
            Controls.Add(btnRefresh);
            Controls.Add(tbxUrl);
            Controls.Add(btnEnter);
            Controls.Add(tabManager);
            tabManager.NewFocusedTab += new EventHandler(ChangeTab);
            Resize += new EventHandler(UpdateWebViewLocation);
        }

        private void ChangeTab(object sender,EventArgs e)
        {
            OpenedTab = tabManager.FocusedTab.webview;
            btnBack.MouseClick += new MouseEventHandler(tabManager.FocusedTab.MoveBack_Click);
            btnForward.MouseClick += new MouseEventHandler(tabManager.FocusedTab.MoveForward_Click);
            btnRefresh.MouseClick += new MouseEventHandler(tabManager.FocusedTab.Reload_Click);
            btnEnter.MouseClick += new MouseEventHandler(tabManager.FocusedTab.Search_Click);
            OpenedTab.SourceChanged += new EventHandler<CoreWebView2SourceChangedEventArgs>(tabManager.FocusedTab.UrlChanged);
            OpenedTab.NavigationStarting += new EventHandler<CoreWebView2NavigationStartingEventArgs>(tabManager.FocusedTab.NavigationStarting);
        }

        private void UpdateWebViewLocation(object sender, EventArgs e)
        {
            tabManager.Size = new Size(Width,30);
            tabManager.Location = new Point(0, ButtonHeightPixel + 2*OffsetPixel);
            OpenedTab.Location = new Point(0, ButtonHeightPixel+ tabManager.Height + 3 * OffsetPixel);
            OpenedTab.Size = new Size(Width, Height - ButtonHeightPixel - 2 * OffsetPixel);
        }
    }

    public class TabManager : Panel
    {
        public List<Tab> tabs;
        public Tab FocusedTab;
        private readonly Button btnNewTab;
        private const int TABMANAGER_HEIGHT_PX = 30;
        public event EventHandler NewFocusedTab;

        public TabManager(int width)
        {
            tabs = new List<Tab>();
            
            btnNewTab = new Button() {
                Text = "new"};
            btnNewTab.Click += new EventHandler(NewTab);
            Controls.Add(btnNewTab);
            NewTab(new object(), new EventArgs());
            UpdateLocations(width);
            Show();
        }

        private void UpdateLocations(int width)
        {
            Size = new Size(width, TABMANAGER_HEIGHT_PX);
            int CurrentOffset = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].Location = new Point(CurrentOffset,0);
                CurrentOffset += tabs[i].Width;
            }
            btnNewTab.Location = new Point(CurrentOffset,0);
        }

        private void NewTab(object sender, EventArgs e)
        {
            Tab tab = new();
            tab.Disposed += new EventHandler(RemoveTabFromList);
            tab.DemandFocus += new EventHandler(ChangeFocus);
            tabs.Add(tab);
            Controls.Add(tabs[tabs.Count-1]);
            UpdateLocations(Width);
        }

        public void ChangeFocus(object sender,EventArgs e)
        {
            FocusedTab = sender as Tab;
            NewFocusedTab?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveTabFromList(object sender, EventArgs e)
        {
            tabs.Remove(sender as Tab);
            UpdateLocations(Width);
        }
    }

    public class Tab:Panel
    {
        private readonly Button btnWebsiteName;
        private readonly Button btnClose;
        public readonly WebView2 webview;

        private const int BTN_TAB_NAME_MAXIMIZED_WIDTH_PX = 100;
        private const int BTN_TAB_NAME_MINIMIZED_WIDTH_PX = 40;
        private const int TAB_HEIGHT_PX = 30;
        private const int TAB_MAXIMIZED_WIDTH_PX = BTN_TAB_NAME_MAXIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int TAB_MINIMIZED_WIDTH_PX = BTN_TAB_NAME_MINIMIZED_WIDTH_PX + BTN_CLOSE_TAB_WIDTH_PX;
        private const int BTN_CLOSE_TAB_WIDTH_PX = 30;

        public event EventHandler DemandFocus;

        public Tab()
        {
            btnWebsiteName = new Button() {
                Text = "new tab"};
            btnClose = new Button() {
                Text = "X"};
            webview = new() {
            Source = new Uri("https://duckduckgo.com/")};
            Maximize();
            btnClose.Click += new EventHandler(CloseTab);
            btnWebsiteName.Click += new EventHandler(ShowTab);
            Controls.Add(btnWebsiteName);
            Controls.Add(btnClose);
        }

        private void CloseTab(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void ShowTab(object sender,EventArgs e)
        {
            DemandFocus?.Invoke(this, EventArgs.Empty);
        }

        public void Minimize()
        {
            Size = new Size(TAB_MINIMIZED_WIDTH_PX,TAB_HEIGHT_PX);
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(BTN_TAB_NAME_MINIMIZED_WIDTH_PX, 0);
        }

        public void Maximize()
        {
            Size = new Size(TAB_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Size = new Size(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX, TAB_HEIGHT_PX);
            btnWebsiteName.Location = new Point(0, 0);
            btnClose.Size = new Size(BTN_CLOSE_TAB_WIDTH_PX, TAB_HEIGHT_PX);
            btnClose.Location = new Point(BTN_TAB_NAME_MAXIMIZED_WIDTH_PX, 0);
        }

        public void Search_Click(object sender, EventArgs e)
        {
            try { webview.Source = new Uri(btnWebsiteName.Text); }
            catch { webview.Source = new Uri("https://duckduckgo.com/?t=ffab&q=" + btnWebsiteName.Text + "&atb=v320-1&ia=web"); }
        }

        public void UrlChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            btnWebsiteName.Text = webview.Source.ToString();
        }

        public void MoveBack_Click(object sender, EventArgs e)
        {
            webview.CoreWebView2.GoBack();
        }

        public void MoveForward_Click(object sender, EventArgs e)
        {
            webview.CoreWebView2.GoForward();
        }

        public void Reload_Click(object sender, EventArgs e)
        {
            webview.CoreWebView2.Reload();
        }

        public void NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            /*
            e.Cancel = true;
            foreach (string uri in AutorisedWebsites.Where(e.Uri.StartsWith))
            {
                e.Cancel = false;
            }*/
        }
    }
}
