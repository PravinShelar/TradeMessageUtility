﻿using System;
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
            // Todo : Add List View Control Dynamically
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

                // Number of key value pairs in both files should be same.
                var tradeValues = UtilityHelper.GetTradeCodeValues();

                if (keyValuePairOfFirstFile.Length == keyValuePairOfSecondFile.Length)
                {
                    for (int i = 1; i < keyValuePairOfFirstFile.Length; i++)
                    {
                        //Sequence of key value pairs in both files should be same.

                        if (string.Equals(keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim(), keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[0].Trim()))
                        {
                            if (!keysToIgnor.Contains(keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim()))
                            {
                                if (!string.Equals(keyValuePairOfFirstFile[i], keyValuePairOfSecondFile[i]))
                                {
                                    numberOfDifferences++;
                                    string fileOneValue = string.Empty;
                                    int fileOneKey = Convert.ToInt32(keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim());
                                    if (tradeValues.ContainsKey(fileOneKey))
                                    {
                                        fileOneValue = string.Format("({0}) {1}", fileOneKey, tradeValues[fileOneKey]);
                                    }
                                    else
                                    {
                                        fileOneValue = fileOneKey.ToString();
                                    }

                                    string fileTwoValue = string.Empty;
                                    int fileTwoKey = Convert.ToInt32(keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[0].Trim());
                                    if (tradeValues.ContainsKey(fileTwoKey))
                                    {
                                        fileTwoValue = string.Format("({0}) {1}", fileTwoKey, tradeValues[fileTwoKey]);
                                    }
                                    else
                                    {
                                        fileTwoValue = fileTwoKey.ToString();
                                    }

                                    //string.Format("{0}. First File - [{1} : {2}], Second File - [{3} : {4}]", numberOfDifferences, fileOneValue, keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim(), fileTwoValue, keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim())
                                    string firstValue = string.Format("{0} : {1}", fileOneValue, keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim());
                                    string secondValue = string.Format("{0} : {1}", fileTwoValue, keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim());
                                    ListViewItem lstViewItem = new ListViewItem(new[] { firstValue, secondValue });
                                    listView1.Items.Add(lstViewItem);
                                }
                            }
                        }
                        else
                        {
                            //Console.WriteLine(string.Format("Files are not in proper sequence: {0}, {1}", keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim(), keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[0].Trim()));
                        }
                    }
                }
                else
                {
                    // Console.WriteLine("Need to check if this is valid scenario");
                }


                lblDiff.Text = string.Format("Number of differences in both Files = {0}", numberOfDifferences);
            }
        }


    }


}