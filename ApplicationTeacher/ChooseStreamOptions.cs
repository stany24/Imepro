using LibraryData;

namespace ApplicationTeacher
{
    public partial class ChooseStreamOptions : Form
    {
        private readonly List<DataForTeacher> StudentsToShare = new();

        public List<DataForTeacher> GetStudentToShare() { return StudentsToShare; }
        public ChooseStreamOptions(List<DataForTeacher> list)
        {
            InitializeComponent();
            lbxStudents.SelectionMode = SelectionMode.MultiExtended;
            lbxStudents.DataSource = list;
            foreach (KeyValuePair<string, List<string>> focus in Configuration.GetAllFocus()) { lbxFocus.Items.Add(focus); }
            lbxFocus.DisplayMember = "Key";
            lbxPriorite.DataSource = Enum.GetNames(typeof(Priority));
            lbxScreen.DataSource = Screen.AllScreens;
        }

        #region Selection

        /// <summary>
        /// Function that selects all students.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll(object sender, EventArgs e)
        {
            for (int i = 0; i < lbxStudents.Items.Count; i++)
            {
                lbxStudents.SetSelected(i, true);
            }
        }

        /// <summary>
        /// Function that unselects all students.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectNone(object sender, EventArgs e)
        {
            lbxStudents.SelectedItems.Clear();
        }

        /// <summary>
        /// Function that starts the stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginSharing_Click(object sender, EventArgs e)
        {
            if (lbxStudents.SelectedItems.Count == 0) { lblError.Text = "Selectionnez au moins 1 élève"; return; }
            if (lbxPriorite.SelectedItem == null) { lblError.Text = "Selectionnez la priorité"; return; }
            if (lbxFocus.SelectedItem == null) { lblError.Text = "Selectionnez le focus"; return; }
            foreach (DataForTeacher student in lbxStudents.SelectedItems)
            {
                StudentsToShare.Add(student);
            }
            List<string> focus = ((KeyValuePair<string, List<string>>)lbxFocus.SelectedItem).Value;
            Priority priorite = (Priority)Enum.Parse(typeof(Priority), lbxPriorite.SelectedItem.ToString());
            Properties.Settings.Default.ScreenToShareId = lbxScreen.SelectedIndex;
            Configuration.SetStreamOptions(new StreamOptions(priorite, focus));
        }

        #endregion
    }
}
