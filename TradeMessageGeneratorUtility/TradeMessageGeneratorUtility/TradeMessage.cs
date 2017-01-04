using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

namespace TradeMessageGenerator
{
    public class TradeMessage
    {
        #region Constants

        public const string DateFormat = "MM/dd/yyyy";
        public const string DataSetName = "TradeWeb";
        public const Char sohValue = '\u0001';
        public const Char keyValuePairSeparator = '=';
        public const Char configValueSeparator = ',';

        #endregion

        #region Private Variable

        private List<string> columns = null;
        private List<string> columnsBasedOnPeriod = null;

        private static readonly Random random = new Random();

        private int dealerNoteNumber = 0;

        #endregion

        #region Public Methods

        public void GenerateCombination()
        {
            //This method is supposed to generate trade messages with 52 attributes as mentioned in excel document (DSWPCompression w Sample Msg.xls)
            //and other required validation from the provided word document file (Compression_Spreadsheet attributes and values_DSWP.DOC).

            //Get all columnNames
            columns = getColumnNames(AppSettings.ColumnNames);

            //Get name of the columns which needs to be fill at runtime
            columnsBasedOnPeriod = getColumnNames(AppSettings.RuntimeValuesForColumns);



            #region List 1 --> DSWP 3M LIBOR Benchmark trade with Effective date in the past and future

            string useCaseHeader = "List 1 --> DSWP 3M LIBOR Benchmark trade with Effective date in the past and future";
            int useCaseNumber = 1;
            bool isDefault = true;
            string indexTenor = "3M";
            bool isBrokenDatedTrade = false;
            bool generatestubData = false;
            bool generateRollsOnData = false;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 2 --> DSWP 6M LIBOR Benchmark trade with Effective date in the past and future

            useCaseHeader = "List 2 --> DSWP 6M LIBOR Benchmark trade with Effective date in the past and future";
            indexTenor = "6M";
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 3 --> DSWP 3M LIBOR broken dated trades with no stubs provided by user

            useCaseHeader = "List 3 --> DSWP 3M LIBOR broken dated trades with no stubs provided by user";
            indexTenor = "3M";
            isBrokenDatedTrade = true;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 4 --> DSWP 3M LIBOR broken dated trades with Short Initial Stub provided by user

            useCaseHeader = "List 4 --> DSWP 3M LIBOR broken dated trades with Short Initial Stub provided by user";
            isBrokenDatedTrade = true;
            generatestubData = true;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 5 --> DSWP 3M LIBOR broken dated trades with Long Initial Stub provided by user

            useCaseHeader = "List 5 --> DSWP 3M LIBOR broken dated trades with Long Initial Stub provided by user";
            isBrokenDatedTrade = true;
            generatestubData = true;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 6 --> DSWP 3M LIBOR with different Rolls On

            useCaseHeader = "List 6 --> DSWP 3M LIBOR with different Rolls On";
            isBrokenDatedTrade = false;
            generatestubData = false;
            generateRollsOnData = true;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            #region List 7--> DSWP 3M LIBOR with some fields different from default

            useCaseHeader = "List 7--> DSWP 3M LIBOR with some fields different from default";
            isBrokenDatedTrade = false;
            generatestubData = false;
            generateRollsOnData = false;
            isDefault = false;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion


        }

        public void CompareLogMessages(string directoryToMonitor)
        {
            var folders = Directory.GetDirectories(directoryToMonitor);
            string serverName = "stg01";
            string anotherServerName = "stg51";

            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder, string.Format("{0}_*.txt", serverName));
                foreach (var file in files)
                {
                    string filepath = file;
                    string filepath2 = Path.Combine(folder, Path.GetFileName(file).Replace(serverName, anotherServerName));

                    if (File.Exists(filepath2))
                    {
                        List<string> outputLines = new List<string>();
                        string[] keyValuePairOfFirstFile = null, keyValuePairOfSecondFile = null;
                        var contentsOne = File.ReadAllText(filepath);
                        if (!string.IsNullOrEmpty(contentsOne))
                        {
                            keyValuePairOfFirstFile = contentsOne.Split(sohValue);
                        }

                        var contentsTwo = File.ReadAllText(filepath2);
                        if (!string.IsNullOrEmpty(contentsTwo))
                        {
                            keyValuePairOfSecondFile = contentsTwo.Split(sohValue);
                        }

                        int numberOfDifferences = 0;
                        if (keyValuePairOfFirstFile != null && keyValuePairOfSecondFile != null)
                        {

                            List<string> keysToIgnor = new List<string>(AppSettings.KeysToIngnor.Split(configValueSeparator));

                            // Number of key value pairs in both files should be same.
                            var tradeValues = UtilityHelper.GetTradeCodeValues();
                            if (keyValuePairOfFirstFile.Length == keyValuePairOfSecondFile.Length)
                            {
                                outputLines.Add("<html>");
                                outputLines.Add("<head><style>table { border-collapse: collapse; width: 100%;}");
                                outputLines.Add("th { text-align: left; background-color: #f9ea4b; color: #4b4b4b; } ");
                                //th, td {    text-align: left;    padding: 8px;} 
                                outputLines.Add("td { text-align: left; padding: 10px;} ");
                                outputLines.Add("h3 {font-size: 20px; margin: 0; border-bottom: 1px solid #aaa; padding:15px 10px;}");
                                outputLines.Add("div {padding:10px;");
                                outputLines.Add("</style></head> ");
                                outputLines.Add("<body>");
                                outputLines.Add("<h3>Log Message Comparison Report</h3>");
                                outputLines.Add("<div>");
                                outputLines.Add("<table>");
                                outputLines.Add("<thead>");
                                outputLines.Add("<tr class='text-center'>");
                                outputLines.Add("<th></th>");
                                outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(file)));
                                outputLines.Add(string.Format("<th>{0}</th>", Path.GetFileName(filepath2)));
                                outputLines.Add("</tr>");
                                outputLines.Add("</thead>");
                                outputLines.Add("<tbody>");

                                for (int i = 1; i < keyValuePairOfSecondFile.Length; i++)
                                {
                                    //Sequence of key value pairs in both files should be same.
                                    //Console.WriteLine(string.Format("{0} - {1}",keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim(), keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim()));
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

                                                //Console.WriteLine(string.Format("{0}. First File - [{1} : {2}], Second File - [{3} : {4}]", numberOfDifferences, fileOneValue, keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim(), fileTwoValue, keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim()));
                                                //outputLines.Add(string.Format("{0}. First File - [{1} : {2}], Second File - [{3} : {4}]", numberOfDifferences, fileOneValue, keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim(), fileTwoValue, keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim()));

                                                var col1 = string.Format("{0} : {1}", fileOneValue, keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[1].Trim());
                                                var col2 = string.Format("{0} : {1}", fileTwoValue, keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[1].Trim());
                                                outputLines.Add("<tr>");
                                                outputLines.Add(string.Format("<td class='text-left'>{0}. </td><td class='text-left'>{1}</td><td class='text-left'>{2}</td>", numberOfDifferences, col1, col2));
                                                outputLines.Add("</tr>");
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    Console.WriteLine(string.Format("Files are not in proper sequence: {0}, {1}", keyValuePairOfFirstFile[i].Split(keyValuePairSeparator)[0].Trim(), keyValuePairOfSecondFile[i].Split(keyValuePairSeparator)[0].Trim()));
                                    //}
                                }
                                outputLines.Add("</tbody>");
                                outputLines.Add("</table>");
                                outputLines.Add("</div>");
                                outputLines.Add("<hr>");
                                outputLines.Add(string.Format("<p> Number of differences in both Files = <b>{0}</b></ p > ", numberOfDifferences));
                                outputLines.Add("<hr>");
                                outputLines.Add("</body>");
                                outputLines.Add("</html>");
                            }
                            //else
                            //{
                            //    Console.WriteLine("Need to check if this is valid scenario");
                            //}

                            //outputLines.Add("\n");
                            //outputLines.Add(string.Format("Number of differences in both Files = {0}", numberOfDifferences));

                        }
                        //Write output file
                        string outputfileName = string.Empty;
                        if (numberOfDifferences == 0)
                        {
                            outputfileName = Path.GetFileName(file).Replace(serverName, "Pass_");
                        }
                        else
                        {
                            outputfileName = Path.GetFileName(file).Replace(serverName, "Fail_");
                        }


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

                }
            }

        }

        #endregion

        #region Private Methods

        private void FillDataTable(int useCaseNumber, string useCaseHeader, bool isDefault, bool isBrokenDatedTrade, string indexTenor, bool generateStubData, bool generateRollsOnData)
        {
            //Create a DataSet with the existing DataTables
            DataSet ds = new DataSet(DataSetName);

            string excelWorksheetName = string.Format("DSWP_{0}_LIBOR_{1}", indexTenor, useCaseNumber);
            DataTable dataTable = new DataTable(excelWorksheetName);

            foreach (var columnName in columns)
            {
                dataTable.Columns.Add(columnName);
            }

            for (int i = 0; i < AppSettings.NumberOfRecords; i++)
            {
                int periodInYears = random.Next(1, 6);
                DataRow row = dataTable.NewRow();
                foreach (var columnName in columns)
                {
                    if (columnsBasedOnPeriod.Contains(columnName))
                    {
                        row[columnName] = getValueBasedOnInputs(useCaseNumber, columnName, indexTenor, i, periodInYears, isBrokenDatedTrade, generateStubData, generateRollsOnData);
                    }
                    else
                    {
                        row[columnName] = getValueByColumnName(columnName, isDefault);
                    }
                }
                dataTable.Rows.Add(row);
            }
            ds.Tables.Add(dataTable);
            string fullFileName = Path.Combine(AppSettings.DirectoryName, string.Format("{0}_SampleFile_{1}_{2}{3}{4}.xlsx", DataSetName, useCaseNumber, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year));
            //Commented to work Locally
            ExcelFileGenerator.CreateExcel(ds, fullFileName, useCaseHeader);
        }

        private List<string> getColumnNames(string appSettingValue)
        {
            if (appSettingValue.Contains(","))
            {
                return appSettingValue.Split(',').ToList();
            }
            return null;

        }

        private string getValueBasedOnInputs(int useCaseNumber, string colName, string indexTenor, int forloopIndex, int periodInYears, bool isBrokenDatedTrade, bool generateStubData, bool generateRollsOnData)
        {
            switch (colName)
            {
                case "Full Notional":
                    int fullNotional = random.Next(5000, 100000);
                    return fullNotional.ToString();
                case "Effective Date":
                    if ((forloopIndex + 1) % 2 == 0)
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (-4)).Date.ToString(DateFormat);
                    }
                    else
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (7)).Date.ToString(DateFormat);
                    }

                case "Maturity Date":
                    if ((forloopIndex + 1) % 2 == 0)
                    {
                        if (!isBrokenDatedTrade)
                        {
                            return (DateTime.Today.AddMonths((forloopIndex + 1) * (-4)).Date.AddYears(periodInYears)).ToString(DateFormat);
                        }
                        else
                        {
                            return (DateTime.Today.AddMonths((forloopIndex + 1) * (-4)).Date.AddYears(periodInYears).Date.AddDays((forloopIndex + 1) * (-4))).ToString(DateFormat);
                        }

                    }
                    else
                    {
                        if (!isBrokenDatedTrade)
                        {
                            return (DateTime.Today.AddMonths((forloopIndex + 1) * (7)).Date.AddYears(periodInYears)).ToString(DateFormat);
                        }
                        else
                        {
                            return (DateTime.Today.AddMonths((forloopIndex + 1) * (7)).Date.AddYears(periodInYears).Date.AddDays((forloopIndex + 1) * (15))).ToString(DateFormat);
                        }
                    }
                case "Rate/Spread":
                    var rateSpread = new List<string> { "2.0786", "1.0786", "1.11", "1.023", "2.233", "1.536", "2.232", "1.122", "1.05", "1.027", "2.444", "1.555" };
                    int index = random.Next(0, rateSpread.Count);
                    if (index == rateSpread.Count)
                    {
                        index = index - 1;
                    }
                    return rateSpread[index];
                case "BuySell":
                    if (forloopIndex % 2 == 0)
                    {
                        return "P";
                    }
                    else
                    {
                        return "R";
                    };
                case "Index Tenor":
                case "Reset Freq":
                case "Float Calc Freq":
                case "Float Coupon Freq":
                    return indexTenor;
                case "Id":
                    return string.Format("DSWP{0}_{1}", indexTenor, forloopIndex);
                case "Netting Trade ID":
                    return string.Format("RFN_{0}_NetId{1}", indexTenor, forloopIndex);
                case "Dealer Note":
                    string dealerNotePostFix = string.Empty;
                    if (forloopIndex % 2 == 0)
                    {
                        dealerNotePostFix = "Pay";
                    }
                    else
                    {
                        dealerNotePostFix = "Rec";
                    };
                    dealerNoteNumber++;
                    return string.Format("dswp{0} historic {1}Y_{2}_{3}", indexTenor, periodInYears, dealerNotePostFix, dealerNoteNumber);
                case "Fixed Stub Date":
                    if (!generateStubData)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return getStubDataByUseCaseNumber(forloopIndex, useCaseNumber, colName);
                    }
                case "Float Stub Date":
                    if (!generateStubData)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return getStubDataByUseCaseNumber(forloopIndex, useCaseNumber, colName);
                    }
                case "Stub Type":
                    if (!generateStubData)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return getStubDataByUseCaseNumber(forloopIndex, useCaseNumber, colName);
                    }
                case "Rolls On":
                    if (!generateRollsOnData)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        if (forloopIndex % 2 == 0 && forloopIndex < 8)
                        {
                            List<string> rollsOnData = new List<string>() { "EOM", "IMM" };
                            int i = random.Next(0, rollsOnData.Count);
                            if (i == rollsOnData.Count)
                            {
                                i = i - 1;
                            }
                            return rollsOnData[i];
                        }
                        else
                        {
                            return random.Next(1, 30).ToString();
                        }
                    }
                default: return string.Empty;
            }

        }

        private string getStubDataByUseCaseNumber(int forloopIndex, int useCaseNumber, string colName)
        {
            switch (colName)
            {
                case "Fixed Stub Date":
                    if ((forloopIndex + 1) % 2 == 0)
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (2)).Date.ToString(DateFormat);
                    }
                    else
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (3)).Date.ToString(DateFormat);
                    }
                case "Float Stub Date":
                    if ((forloopIndex + 1) % 2 == 0)
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (3)).Date.ToString(DateFormat);
                    }
                    else
                    {
                        return DateTime.Today.AddMonths((forloopIndex + 1) * (4)).Date.ToString(DateFormat);
                    }
                case "Stub Type":
                    if (useCaseNumber == 4)
                    {
                        return "ShortInitial";
                    }
                    else
                    {
                        return "LongInitial";
                    }
                default: return string.Empty;
            }
        }

        private string getValueByColumnName(string colName, bool isDefault)
        {
            switch (colName)
            {
                case "Compression Type": return "N";
                case "ClrService": return "LCH(FCM)";
                case "ClrMember": return "BAML";
                case "Currency": return "USD";
                case "Index": return "LIBOR";
                case "Fixed Day Count": return "30/360";
                case "Float Day Count": return "ACT/360";
                case "Fixed Pay Center":
                case "Term Center":
                case "Float Pay Center":
                case "Fixed Calc Center":
                case "Float Calc Center":
                case "Reset Center":
                    if (isDefault)
                    {
                        return "GBLO,USNY";
                    }
                    else
                    {
                        var fixedPayCenters = new List<string> { "GBLO", "USNY" };
                        int index = random.Next(0, fixedPayCenters.Count);
                        if (index == fixedPayCenters.Count)
                        {
                            index = index - 1;
                        }
                        return fixedPayCenters[index];
                    }

                //return "GBLO,USNY";
                //case "Term Center": return "GBLO,USNY";// Value of this column same as Fixed Pay Center
                //case "Float Pay Center": return "GBLO,USNY";// Value of this column same as Fixed Pay Center
                //case "Fixed Calc Center": return "GBLO,USNY";// Value of this column same as Fixed Pay Center
                //case "Float Calc Center": return "GBLO,USNY";// Value of this column same as Fixed Pay Center
                //case "Reset Center": return "GBLO,USNY";// Value of this column same as Fixed Pay Center

                case "Effective Date Convention": return "NONE";
                case "Fixed Calc Date Convention":
                case "Term Date Convention":
                case "Fixed Pay Date Convention":
                    if (isDefault)
                    {
                        return "MODFOLLOW";
                    }
                    else
                    {
                        return "FOLLOW";
                    }

                //  case "Term Date Convention": return "MODFOLLOW";// Value of this column same as Fixed Calc Date Convention
                // case "Fixed Pay Date Convention": return "MODFOLLOW";// Value of this column same as Fixed Calc Date Convention
                case "Float Calc Date Convention":
                case "Float Pay Date Convention":
                case "Reset Date Convention":
                    if (isDefault)
                    {
                        return "MODFOLLOW";
                    }
                    else
                    {
                        return "FOLLOW";
                    };
                //case "Float Pay Date Convention": return "";// Value of this column same as Float Calc Date Convention
                //case "Reset Date Convention": return "";// Value of this column same as Float Calc Date Convention

                //Same value for below 2 columns
                case "Fixed Pay Rel To":
                case "Float Pay Rel To": return "CalculationPeriodEndDate";

                case "Reset Rel To": return "CalculationPeriodStartDate";
                case "Fixing Center": return "GBLO";
                case "Fixing Date Convention": return "NONE";
                case "Fixing Day Type": return "Business";
                case "Fixing Date Offset": return "-2D";
                case "Internal Note": return "int note1";

                //Return empty string for below 4 columns
                case "Current Fixing":
                case "Float Spread":
                case "Book Id":
                case "Initial Fixing Center":
                case "Float Stub Begin Tenor":
                case "Float Stub End Tenor":
                    return string.Empty;

                //case "Reset Freq": return "";//Same as Index Tenor
                //case "Float Calc Freq": return "";
                //case "Float Coupon Freq": return "";
                case "Fixed Coupon Freq":
                case "Fixed Calc Freq":
                    if (isDefault)
                    {
                        return "6M";
                    }
                    else
                    {
                        return "12M";
                    };
                default: return string.Empty;
            }
        }



        #endregion

        #region Commented Code

        //public decimal fullNotional { get; set; }
        //public decimal rateSpread { get; set; }
        //public int Period { get; set; }
        //public DateTime effectiveDate { get; set; }
        //public DateTime maturityDate { get; set; }
        //private List<TradeMessage> tmessages;

        //public void Validate(List<TradeMessage> tList)
        //{
        //    //This method will validate all 51 fields from the word file while generating the combinatoin
        //}

        #endregion
    }

}
