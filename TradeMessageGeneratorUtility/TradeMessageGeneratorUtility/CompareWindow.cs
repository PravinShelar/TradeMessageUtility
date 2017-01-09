using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace TradeMessageGenerator
{
    public partial class CompareWindow : Form
    {
        public const Char sohValue = '\u0001';
        public const Char keyValuePairSeparator = '=';
        public const Char configValueSeparator = ',';
        public Dictionary<string, string> logFilePathServerOne = new Dictionary<string, string>();
        public Dictionary<string, string> logFilePathServerTwo = new Dictionary<string, string>();

        public CompareWindow()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblFileOne.Visible = false;
            lblFileTwo.Visible = false;
            lblDiff.Visible = false;

            List<string> serverOneFiles = new List<string>();
            List<string> serverTwoFiles = new List<string>();

            string serverName = "stg01";
            string anotherServerName = "stg51";
            var folders = Directory.GetDirectories(AppSettings.DirectoryToMonitor);

            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder, string.Format("{0}_*.txt", serverName));
                foreach (var file in files)
                {
                    string anotherFilePath = Path.Combine(folder, Path.GetFileName(file).Replace(serverName, anotherServerName));

                    if (File.Exists(anotherFilePath))
                    {
                        serverOneFiles.Add(Path.GetFileName(file));
                        logFilePathServerOne.Add(Path.GetFileName(file), file);
                        serverTwoFiles.Add(Path.GetFileName(anotherFilePath));
                        logFilePathServerTwo.Add(Path.GetFileName(anotherFilePath), anotherFilePath);
                    }
                }
            }
            lstFirstFiles.DataSource = serverOneFiles;
            lstSecondFiles.DataSource = serverTwoFiles;
        }

        private void lstFirstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSecondFiles.SelectedIndex != -1)
            {
                lstSecondFiles.SelectedIndex = lstFirstFiles.SelectedIndex;
            }
        }

        private void lstSecondFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSecondFiles.SelectedIndex != lstFirstFiles.SelectedIndex)
            {
                lstSecondFiles.SelectedIndex = lstFirstFiles.SelectedIndex;
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            lblFileOne.Visible = true;
            lblFileTwo.Visible = true;
            lblDiff.Visible = true;

            listView1.Clear();
            lblFileOne.Text = string.Empty;
            lblFileTwo.Text = string.Empty;
            lblDiff.Text = string.Empty;


            listView1.Scrollable = true;
            listView1.View = View.Details;

            //lblFileOne.Text = lstFirstFiles.SelectedItem.ToString();
            //lblFileTwo.Text = lstSecondFiles.SelectedItem.ToString();
            //listView1.Columns.Add(lstFirstFiles.SelectedItem.ToString(), ((listView1.Width - 4) / 2));
            //listView1.Columns.Add(lstSecondFiles.SelectedItem.ToString(), ((listView1.Width - 4) / 2));

            string firstFilePath = logFilePathServerOne[lstFirstFiles.SelectedItem.ToString()];
            string secondFilePath = logFilePathServerTwo[lstSecondFiles.SelectedItem.ToString()];

            listView1.Columns.Add("TagName", ((listView1.Width) / 4));
            listView1.Columns.Add(Path.GetFileName(firstFilePath), ((listView1.Width) / 4));
            listView1.Columns.Add(Path.GetFileName(secondFilePath), ((listView1.Width) / 4));
            listView1.Columns.Add("Status", ((listView1.Width) / 4));

            int numberOfDifferences = 0;
            var datatable = LogMessageComparer.CompareLogMessage(firstFilePath, secondFilePath, ref numberOfDifferences);

            // Clear the ListView control
            listView1.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                DataRow drow = datatable.Rows[i];
                // Define the list items
                ListViewItem lvi = new ListViewItem(drow[0].ToString());
                lvi.SubItems.Add(drow[1].ToString());
                lvi.SubItems.Add(drow[2].ToString());
                if (string.Equals(drow[3].ToString().ToLower(), "true"))
                {
                    lvi.SubItems.Add("Pass");
                }
                else
                {
                    lvi.SubItems.Add("Fail");
                }

                // Add the list items to the ListView
                listView1.Items.Add(lvi);
            }

            lblDiff.Text = string.Format("Number of differences in both Files = {0}", numberOfDifferences);
        }


    }


}
