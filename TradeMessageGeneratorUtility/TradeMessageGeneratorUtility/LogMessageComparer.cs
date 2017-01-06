using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMessageGenerator
{
    public static class LogMessageComparer
    {
        public const Char sohValue = '\u0001';
        public const Char keyValuePairSeparator = '=';
        public const Char configValueSeparator = ',';
        public const string emptyColumnValue = "-";

        public static void CompareLogMessage(string logFileOne, string logFileTwo, string folder, string cssFolderPath)
        {
            string serverName = "stg01";

            List<string> outputLines = new List<string>();
            string[] keyValuePairOfFirstFile = null, keyValuePairOfSecondFile = null;
            var contentsOne = File.ReadAllText(logFileOne);
            if (!string.IsNullOrEmpty(contentsOne))
            {
                keyValuePairOfFirstFile = contentsOne.Split(sohValue);
            }

            var contentsTwo = File.ReadAllText(logFileTwo);
            if (!string.IsNullOrEmpty(contentsTwo))
            {
                keyValuePairOfSecondFile = contentsTwo.Split(sohValue);
            }

            int numberOfDifferences = 0;
            if (keyValuePairOfFirstFile != null && keyValuePairOfSecondFile != null)
            {

                List<string> keysToIgnor = new List<string>(AppSettings.KeysToIngnor.Split(configValueSeparator));
                var tradeValues = UtilityHelper.GetTradeCodeValues();

                getHtmlPageHeader(ref outputLines, cssFolderPath);
                outputLines.Add("<h1>Log Message Comparison Report</h1>");
                outputLines.Add("<div class='pad10A'>");
                outputLines.Add("<div class='table-responsive'>");
                outputLines.Add("<table class='table text-center' id='table1'>");
                outputLines.Add("<thead>");
                outputLines.Add("<tr class='text-center'>");
                //outputLines.Add("<th></th>");
                outputLines.Add("<th>Tagname</th>");
                outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(logFileOne)));
                outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(logFileTwo)));
                outputLines.Add("<th>Status</th>");
                outputLines.Add("</tr>");
                outputLines.Add("</thead>");
                outputLines.Add("<tbody>");

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
                                            createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], false, cssFolderPath);
                                        }
                                        else
                                        {
                                            createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], true, cssFolderPath);
                                        }
                                    }
                                }
                                else if (arrOne.Length > arrTwo.Length)
                                {
                                    for (int i = 0; i < arrOne.Length; i++)
                                    {
                                        if (i >= arrTwo.Length)
                                        {
                                            createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], emptyColumnValue, false, cssFolderPath);
                                        }
                                        else
                                        {
                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {

                                                createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], false, cssFolderPath);
                                            }
                                            else
                                            {
                                                createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], true, cssFolderPath);
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
                                            createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, emptyColumnValue, arrTwo[i], false, cssFolderPath);
                                        }
                                        else
                                        {
                                            if (!string.Equals(arrOne[i], arrTwo[i]))
                                            {
                                                createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], false, cssFolderPath);
                                            }
                                            else
                                            {
                                                createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, arrOne[i], arrTwo[i], true, cssFolderPath);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (!string.Equals(finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck]))
                                {

                                    createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck], false, cssFolderPath);
                                }
                                else
                                {
                                    createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, finalFirstServerFileValues[keytoCheck], finalSecondServerFileValues[keytoCheck], true, cssFolderPath);
                                }
                            }

                            finalSecondServerFileValues.Remove(keytoCheck);
                        }
                        else
                        {

                            createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, finalFirstServerFileValues[keytoCheck], emptyColumnValue, false, cssFolderPath);
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

                        createTableRow(ref outputLines, ref numberOfDifferences, keyWithAttributeName, emptyColumnValue, finalSecondServerFileValues[r.Key], false, cssFolderPath);
                    }
                }
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

        private static void createTableRow(ref List<string> outputLines, ref int numberOfDifferences, string keyWithAttributeName, string fileOneValue, string fileTwoValue, bool status, string cssFolderPath)
        {
            string imageColumn = string.Empty;
            if (status == true)
            {
                imageColumn = string.Format("<td align='center'><img src='{0}/images/ico_pass.gif' alt='Pass'>", cssFolderPath);

            }
            else
            {
                numberOfDifferences++;
                imageColumn = string.Format("<td align='center'><img src='{0}/images/ico_fail.gif' alt='Fail'>", cssFolderPath);
            }

            outputLines.Add("<tr>");
            //outputLines.Add(string.Format("<td class='text-left'>{0}. </td><td class='text-left'>{1}</td><td class='text-left'>{2}</td><td class='text-left'>{3}</td>{4}", numberOfDifferences, keyWithAttributeName, fileOneValue, fileTwoValue, imageColumn));
            outputLines.Add(string.Format("<td class='text-left'>{0}</td><td class='text-left'>{1}</td><td class='text-left'>{2}</td>{3}", keyWithAttributeName, fileOneValue, fileTwoValue, imageColumn));
            outputLines.Add("</tr>");
        }
    }
}
