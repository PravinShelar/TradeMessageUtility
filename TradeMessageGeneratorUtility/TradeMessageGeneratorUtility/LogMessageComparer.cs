using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TradeMessageGenerator
{
    public static class LogMessageComparer
    {
        public const Char SohValue = '\u0001';
        public const Char KeyValuePairSeparator = '=';
        public const Char ConfigValueSeparator = ',';
        public const string EmptyColumnValue = "-";

        public static void CompareLogMessage(string logFileOne, string logFileTwo, string folder, string cssFolderPath)
        {
            string serverName = "stg01";
            List<string> outputLines = new List<string>();
            int numberOfDifferences = 0;

            List<string> keysToIgnor = new List<string>(AppSettings.KeysToIngnor.Split(ConfigValueSeparator));
            var tradeValues = UtilityHelper.GetTradeCodeValues();

            getHtmlPageHeader(ref outputLines, cssFolderPath);
            outputLines.Add("<h1>Log Message Comparison Report</h1>");
            outputLines.Add("<div class='pad10A'>");
            outputLines.Add("<div class='table-responsive'>");
            outputLines.Add("<table class='table text-center' id='table1'>");
            outputLines.Add("<thead>");
            outputLines.Add("<tr class='text-center'>");
            outputLines.Add("<th>Tagname</th>");
            outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(logFileOne)));
            outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(logFileTwo)));
            outputLines.Add("<th>Status</th>");
            outputLines.Add("</tr>");
            outputLines.Add("</thead>");
            outputLines.Add("<tbody>");

            DataTable dataTable = CompareLogMessage(logFileOne, logFileTwo, ref numberOfDifferences);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow drow = dataTable.Rows[i];
                bool status = false;
                if (string.Equals(drow[3].ToString().ToLower(), "true"))
                {
                    status = true;
                }
                else
                {
                    status = false;
                }

                createTableRow(ref outputLines, drow[0].ToString(), drow[1].ToString(), drow[2].ToString(), status, cssFolderPath);

            }

            outputLines.Add("</tbody>");
            outputLines.Add("</table>");
            outputLines.Add("</div>");
            outputLines.Add("<hr>");
            outputLines.Add(string.Format("<p> Number of differences in both Files = <b>{0}</b></ p > ", numberOfDifferences));
            outputLines.Add("<hr>");
            outputLines.Add("</div>");
            getHtmlPageFooter(ref outputLines);

            string outputfileName = string.Empty;
            if (numberOfDifferences == 0)
            {
                outputfileName = Path.GetFileName(logFileOne).Replace(serverName, "Pass_");
            }
            else
            {
                outputfileName = Path.GetFileName(logFileOne).Replace(serverName, "Fail_");
            }

            createHtmlFile(folder, outputLines, outputfileName);

        }

        public static DataTable CompareLogMessage(string logFileOne, string logFileTwo, ref int numberOfDifferences)
        {
            DataTable table = new DataTable();
            string[] keyValuePairOfFirstFile = null, keyValuePairOfSecondFile = null;
            var contentsOne = File.ReadAllText(logFileOne);
            if (!string.IsNullOrEmpty(contentsOne))
            {
                keyValuePairOfFirstFile = contentsOne.Split(SohValue);
            }

            var contentsTwo = File.ReadAllText(logFileTwo);
            if (!string.IsNullOrEmpty(contentsTwo))
            {
                keyValuePairOfSecondFile = contentsTwo.Split(SohValue);
            }

            if (keyValuePairOfFirstFile != null && keyValuePairOfSecondFile != null)
            {

                List<string> keysToIgnor = new List<string>(AppSettings.KeysToIngnor.Split(ConfigValueSeparator));
                var tradeValues = UtilityHelper.GetTradeCodeValues();

                Dictionary<int, string> finalFirstServerFileValues = new Dictionary<int, string>();
                for (int i = 1; i < keyValuePairOfFirstFile.Length - 1; i++)
                {
                    int fileOneKey = Convert.ToInt32(keyValuePairOfFirstFile[i].Split(KeyValuePairSeparator)[0].Trim());
                    string fileOneValue = keyValuePairOfFirstFile[i].Split(KeyValuePairSeparator)[1].Trim();

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
                    int fileOneKey = Convert.ToInt32(keyValuePairOfSecondFile[i].Split(KeyValuePairSeparator)[0].Trim());
                    string fileOneValue = keyValuePairOfSecondFile[i].Split(KeyValuePairSeparator)[1].Trim();

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


                table.Columns.Add("TagName", typeof(string));
                table.Columns.Add(Path.GetFileName(logFileOne), typeof(string));
                table.Columns.Add(Path.GetFileName(logFileTwo), typeof(string));
                table.Columns.Add("Status", typeof(bool));

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
                                            table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], false);
                                            numberOfDifferences++;
                                        }
                                        else
                                        {
                                            table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], true);
                                        }
                                    }
                                }
                                else if (arrOne.Length > arrTwo.Length)
                                {
                                    for (int i = 0; i < arrOne.Length; i++)
                                    {
                                        if (i >= arrTwo.Length)
                                        {
                                            table.Rows.Add(keyWithAttributeName, arrOne[i], EmptyColumnValue, false);
                                            numberOfDifferences++;
                                        }
                                        else
                                        {
                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {

                                                table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], false);
                                                numberOfDifferences++;
                                            }
                                            else
                                            {
                                                table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], true);
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
                                            table.Rows.Add(keyWithAttributeName, EmptyColumnValue, arrTwo[i], false);
                                            numberOfDifferences++;
                                        }
                                        else
                                        {
                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {
                                                table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], false);
                                                numberOfDifferences++;
                                            }
                                            else
                                            {
                                                table.Rows.Add(keyWithAttributeName, arrOne[i], arrTwo[i], true);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (!string.Equals(finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck]))
                                {

                                    table.Rows.Add(keyWithAttributeName, finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck], false);
                                    numberOfDifferences++;
                                }
                                else
                                {
                                    table.Rows.Add(keyWithAttributeName, finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck], true);
                                }
                            }

                            finalSecondServerFileValues.Remove(keytoCheck);
                        }
                        else
                        {

                            table.Rows.Add(keyWithAttributeName, finalFirstServerFileValues[keytoCheck], EmptyColumnValue, false);
                            numberOfDifferences++;
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

                        table.Rows.Add(keyWithAttributeName, EmptyColumnValue, finalSecondServerFileValues[r.Key], false);
                        numberOfDifferences++;
                    }
                }
            }
            return table;

        }

        public static void createTerminateHtmlFile(string folder, string outputfileName, string cssFolderPath)
        {
            List<string> outputLines = new List<string>();
            getHtmlPageHeader(ref outputLines, cssFolderPath);
            outputLines.Add("<h1>Unable to compare files as one of the server log file is not available.</h1>");
            getHtmlPageFooter(ref outputLines);
            createHtmlFile(folder, outputLines, outputfileName);
        }

        private static void createHtmlFile(string folder, List<string> outputLines, string outputfileName)
        {
            string htmlFilePath = Path.Combine(folder, outputfileName.Replace(".txt", ".html"));
            using (FileStream fs = new FileStream(htmlFilePath, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    foreach (var item in outputLines.ToArray())
                    {
                        w.WriteLine(item);
                    }
                }
            }
        }

        private static void getHtmlPageFooter(ref List<string> outputLines)
        {
            outputLines.Add("</div>");
            outputLines.Add("</div>");
            outputLines.Add("</section>");
            outputLines.Add("</body>");
            outputLines.Add("</html>");
        }

        private static void getHtmlPageHeader(ref List<string> outputLines, string cssFolderPath)
        {
            outputLines.Add("<html>");
            outputLines.Add("<head>");
            outputLines.Add("<meta charset='utf-8'>");
            outputLines.Add("<meta http-equiv='X-UA-Compatible' content='IE=edge'>");
            outputLines.Add("<meta name='viewport' content='width=device-width, initial-scale=1'>");
            outputLines.Add(string.Format("<link href='{0}/css/bootstrap.css' rel='stylesheet'>", cssFolderPath));
            outputLines.Add(string.Format("<link href='{0}/css/style.css' rel='stylesheet'>", cssFolderPath));
            outputLines.Add("</head>");
            outputLines.Add("<body>");
            outputLines.Add("<header class=''>");
            outputLines.Add("<div class='container'>");
            outputLines.Add("<div class='logo pull-left'>");
            outputLines.Add(string.Format("<img src ='{0}/images/logo.png' alt='Synechron' title='Synechron'>", cssFolderPath));
            outputLines.Add("</div>");
            outputLines.Add("</div>");
            outputLines.Add("</header>");
            outputLines.Add("<section>");
            outputLines.Add("<div class='container'>");
            outputLines.Add("<div class='detail-report'>");

        }

        private static void createTableRow(ref List<string> outputLines, string keyWithAttributeName, string fileOneValue, string fileTwoValue, bool status, string cssFolderPath)
        {
            string imageColumn = string.Empty;
            if (status == true)
            {
                imageColumn = string.Format("<td align='center'><img src='{0}/images/ico_pass.gif' alt='Pass'>", cssFolderPath);

            }
            else
            {
                imageColumn = string.Format("<td align='center'><img src='{0}/images/ico_fail.gif' alt='Fail'>", cssFolderPath);
            }

            outputLines.Add("<tr>");
            //outputLines.Add(string.Format("<td class='text-left'>{0}. </td><td class='text-left'>{1}</td><td class='text-left'>{2}</td><td class='text-left'>{3}</td>{4}", numberOfDifferences, keyWithAttributeName, fileOneValue, fileTwoValue, imageColumn));
            outputLines.Add(string.Format("<td class='text-left'>{0}</td><td class='text-left'>{1}</td><td class='text-left'>{2}</td>{3}", keyWithAttributeName, fileOneValue, fileTwoValue, imageColumn));
            outputLines.Add("</tr>");
        }
    }
}
