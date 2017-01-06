using System;
using System.Collections.Generic;
using System.Configuration;
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


            lblFileOne.Text = lstFirstFiles.SelectedItem.ToString();
            lblFileTwo.Text = lstSecondFiles.SelectedItem.ToString();
            listView1.Columns.Add(lstFirstFiles.SelectedItem.ToString(), ((listView1.Width - 4) / 2));
            listView1.Columns.Add(lstSecondFiles.SelectedItem.ToString(), ((listView1.Width - 4) / 2));

            string firstFilePath = logFilePathServerOne[lstFirstFiles.SelectedItem.ToString()];
            string secondFilePath = logFilePathServerTwo[lstSecondFiles.SelectedItem.ToString()];

            string[] keyValuePairOfFirstFile = null, keyValuePairOfSecondFile = null;
            var contentsOne = File.ReadAllText(firstFilePath);
            if (!string.IsNullOrEmpty(contentsOne))
            {
                keyValuePairOfFirstFile = contentsOne.Split(sohValue);
            }

            var contentsTwo = File.ReadAllText(secondFilePath);
            if (!string.IsNullOrEmpty(contentsTwo))
            {
                keyValuePairOfSecondFile = contentsTwo.Split(sohValue);
            }

            if (keyValuePairOfFirstFile != null && keyValuePairOfSecondFile != null)
            {
                int numberOfDifferences = 0;
                List<string> keysToIgnor = new List<string>(AppSettings.KeysToIngnor.Split(configValueSeparator));

                var tradeValues = UtilityHelper.GetTradeCodeValues();

                Dictionary<int, string> finalFirstServerFileValues = new Dictionary<int, string>();
                for (int i = 1; i < keyValuePairOfFirstFile.Length - 1; i++)
                {
                    int fileOneKey = Convert.ToInt32(keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim());
                    string fileOneValue = keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim();
                    if (!finalFirstServerFileValues.ContainsKey(fileOneKey))
                    {
                        finalFirstServerFileValues.Add(fileOneKey, fileOneValue);
                    }
                    else
                    {
                        string existingValue = finalFirstServerFileValues[fileOneKey];
                        existingValue += string.Format(",{0}", fileOneValue);
                        finalFirstServerFileValues[fileOneKey] = existingValue;
                    }
                }

                Dictionary<int, string> finalSecondServerFileValues = new Dictionary<int, string>();
                for (int i = 1; i < keyValuePairOfSecondFile.Length - 1; i++)
                {
                    int fileOneKey = Convert.ToInt32(keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[0].Trim());
                    string fileOneValue = keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim();
                    if (!finalSecondServerFileValues.ContainsKey(fileOneKey))
                    {
                        finalSecondServerFileValues.Add(fileOneKey, fileOneValue);
                    }
                    else
                    {
                        string existingValue = finalFirstServerFileValues[fileOneKey];
                        existingValue += string.Format(",{0}", fileOneValue);
                        finalSecondServerFileValues[fileOneKey] = existingValue;
                    }
                }

                foreach (var item in finalFirstServerFileValues)
                {
                    int keytoCheck = item.Key;

                    if (!keysToIgnor.Contains(keytoCheck.ToString()))
                    {
                        string keyWithAttributeName = string.Empty;
                        if (tradeValues.ContainsKey(keytoCheck))
                        {
                            keyWithAttributeName = string.Format("({0}) {1}", keytoCheck, tradeValues[keytoCheck]);
                        }
                        else
                        {
                            keyWithAttributeName = keytoCheck.ToString();
                        }
                        if (finalSecondServerFileValues.ContainsKey(keytoCheck))
                        {
                            string firstServerFileValue = finalFirstServerFileValues[keytoCheck];
                            string secondServerFileValue = finalSecondServerFileValues[keytoCheck];

                            var arrOne = firstServerFileValue.Split(',');
                            var arrTwo = secondServerFileValue.Split(',');

                            if (arrOne.Length > 1 || arrTwo.Length > 1)
                            {
                                if (arrOne.Length == arrTwo.Length)
                                {
                                    for (int i = 0; i < arrOne.Length; i++)
                                    {
                                        if (!string.Equals(arrOne[i], arrTwo[i]))
                                        {
                                            numberOfDifferences++;
                                            ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, arrOne[i]), string.Format("{0}: {1}", keyWithAttributeName, arrTwo[i]) });
                                            listView1.Items.Add(lstViewItem);
                                            
                                        }
                                    }
                                }
                                else if (arrOne.Length > arrTwo.Length)
                                {
                                    for (int i = 0; i < arrOne.Length; i++)
                                    {
                                        if (i >= arrTwo.Length)
                                        {
                                            numberOfDifferences++;
                                            ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, arrOne[i]), string.Empty });
                                            listView1.Items.Add(lstViewItem);
                                        }
                                        else
                                        {

                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {
                                                numberOfDifferences++;
                                                ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, arrOne[i]), string.Format("{0}: {1}", keyWithAttributeName, arrTwo[i]) });
                                                listView1.Items.Add(lstViewItem);
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    for (int i = 0; i < arrTwo.Length; i++)
                                    {
                                        if (i >= arrOne.Length)
                                        {
                                            numberOfDifferences++;
                                            ListViewItem lstViewItem = new ListViewItem(new[] { string.Empty, string.Format("{0}: {1}", keyWithAttributeName, arrTwo[i]) });
                                            listView1.Items.Add(lstViewItem);
                                        }
                                        else
                                        {

                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {
                                                numberOfDifferences++;
                                                ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, arrOne[i]), string.Format("{0}: {1}", keyWithAttributeName, arrTwo[i]) });
                                                listView1.Items.Add(lstViewItem);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (!string.Equals(finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck]))
                                {
                                    numberOfDifferences++;
                                    ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, finalFirstServerFileValues[keytoCheck]), string.Format("{0}: {1}", keyWithAttributeName, finalSecondServerFileValues[keytoCheck]) });
                                    listView1.Items.Add(lstViewItem);
                                }
                            }

                            finalSecondServerFileValues.Remove(keytoCheck);
                        }
                        else
                        {
                            numberOfDifferences++;
                            ListViewItem lstViewItem = new ListViewItem(new[] { string.Format("{0}: {1}", keyWithAttributeName, finalFirstServerFileValues[keytoCheck]), string.Empty });
                            listView1.Items.Add(lstViewItem);
                        }
                    }

                }
                foreach (var r in finalSecondServerFileValues)
                {
                    if (!keysToIgnor.Contains(r.Key.ToString()))
                    {
                        string keyWithAttributeName = string.Empty;
                        if (tradeValues.ContainsKey(r.Key))
                        {
                            keyWithAttributeName = string.Format("({0}) {1}", r.Key, tradeValues[r.Key]);
                        }
                        else
                        {
                            keyWithAttributeName = r.Key.ToString();
                        }

                        ListViewItem lstViewItem = new ListViewItem(new[] { string.Empty, string.Format("{0}: {1}", r.Key, finalSecondServerFileValues[r.Key]) });
                        listView1.Items.Add(lstViewItem);
                        numberOfDifferences++;
                    }
                }


                lblDiff.Text = string.Format("Number of differences in both Files = {0}", numberOfDifferences);
            }
        }


    }


}
