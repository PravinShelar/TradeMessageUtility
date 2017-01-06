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
        public const string emptyColumnValue = "-";

        #endregion

        #region Private Variable

        private List<string> columns = null;
        private List<string> columnsBasedOnPeriod = null;
        private static readonly Random random = new Random();
        private int dealerNoteNumber = 0;
        private bool generateNegativeData = false;

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

            #region List 8--> DSWP 3M LIBOR with negative data
            generateNegativeData = true;
            useCaseHeader = "List 8--> DSWP 3M LIBOR with negative data";
            isBrokenDatedTrade = false;
            generatestubData = false;
            generateRollsOnData = false;
            isDefault = false;
            FillDataTable(useCaseNumber, useCaseHeader, isDefault, isBrokenDatedTrade, indexTenor, generatestubData, generateRollsOnData);
            useCaseNumber++;

            #endregion

            File.WriteAllText(Path.Combine(AppSettings.DirectoryName, string.Format("{0}.txt", Path.GetFileName(AppSettings.DirectoryName))), "Successfully generated sample trade messages.");
        }

        public void CompareLogMessages(string directoryToMonitor)
        {
            var folders = Directory.GetDirectories(directoryToMonitor);

            //TODO : Check if file does not exist
            string cssFolderPath = Path.Combine(string.Format(@"{0}\{1}", Directory.GetParent(directoryToMonitor).FullName, "Reports"));

            string serverName = "stg01";
            string anotherServerName = "stg51";

            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder, string.Format("{0}_*.txt", serverName));

                if (files.Length == 0)
                {
                    LogMessageComparer.createTerminateHtmlFile(folder, string.Format("Terminate_{0}.html", Path.GetFileName(folder)), cssFolderPath);
                }

                foreach (var file in files)
                {
                    string filepath2 = Path.Combine(folder, Path.GetFileName(file).Replace(serverName, anotherServerName));

                    if (File.Exists(filepath2))
                    {
                        LogMessageComparer.CompareLogMessage(file, filepath2, folder, cssFolderPath);

                    }
                    else
                    {
                        LogMessageComparer.createTerminateHtmlFile(folder, string.Format("Terminate_{0}.html", Path.GetFileName(folder)), cssFolderPath);
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
                        return DateTime.Today.AddDays((forloopIndex + 1) * (-4)).Date.ToString(DateFormat);
                    }
                    else
                    {
                        return DateTime.Today.AddDays((forloopIndex + 1) * (7)).Date.ToString(DateFormat);
                    }

                case "Maturity Date":
                    if ((forloopIndex + 1) % 2 == 0)
                    {
                        if (!isBrokenDatedTrade)
                        {
                            return (DateTime.Today.AddDays((forloopIndex + 1) * (-4)).Date.AddYears(periodInYears)).ToString(DateFormat);
                        }
                        else
                        {
                            return (DateTime.Today.AddDays((forloopIndex + 1) * (-4)).Date.AddYears(periodInYears).Date.AddDays((forloopIndex + 1) * (-4))).ToString(DateFormat);
                        }

                    }
                    else
                    {
                        if (!isBrokenDatedTrade)
                        {
                            return (DateTime.Today.AddDays((forloopIndex + 1) * (7)).Date.AddYears(periodInYears)).ToString(DateFormat);
                        }
                        else
                        {
                            return (DateTime.Today.AddDays((forloopIndex + 1) * (7)).Date.AddYears(periodInYears).Date.AddDays((forloopIndex + 1) * (15))).ToString(DateFormat);
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
                        //if (forloopIndex % 2 == 0 && forloopIndex < 8)
                        //{
                        //    List<string> rollsOnData = new List<string>() { "EOM", "IMM" };
                        //    int i = random.Next(0, rollsOnData.Count);
                        //    if (i == rollsOnData.Count)
                        //    {
                        //        i = i - 1;
                        //    }
                        //    return rollsOnData[i];
                        //}
                        //else
                        //{
                        //    return random.Next(1, 30).ToString();
                        //}

                        DateTime date;
                        if ((forloopIndex + 1) % 2 == 0)
                        {
                            date = (DateTime.Today.AddDays((forloopIndex + 1) * (-4)).Date.AddYears(periodInYears));
                        }
                        else
                        {
                            date = (DateTime.Today.AddDays((forloopIndex + 1) * (7)).Date.AddYears(periodInYears));
                        }

                        return date.Day != 31 ? date.Day.ToString() : "EOM";

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
                        if (generateNegativeData)
                        {
                            return "GBLO";
                        }
                        else
                        {
                            //var fixedPayCenters = new List<string> { "GBLO", "USNY" };
                            //int index = random.Next(0, fixedPayCenters.Count);
                            //if (index == fixedPayCenters.Count)
                            //{
                            //    index = index - 1;
                            //}
                            //return fixedPayCenters[index];
                            return "USNY";
                        }
                    }

                case "Effective Date Convention": return "NONE";
                case "Fixed Calc Date Convention":
                case "Term Date Convention":
                case "Fixed Pay Date Convention":
                case "Float Calc Date Convention":
                case "Float Pay Date Convention":
                    if (isDefault)
                    {
                        return "MODFOLLOW";
                    }
                    else
                    {
                        if (generateNegativeData)
                        {
                            return "M";
                        }
                        else
                        {
                            return "FOLLOW";
                        }
                    }

                case "Reset Date Convention":
                    if (isDefault)
                    {
                        return "MODFOLLOW";
                    }
                    else
                    {
                        if (generateNegativeData)
                        {
                            return "M";
                        }
                        else
                        {
                            return "FOLLOW";
                        }
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
                        if (generateNegativeData)
                        {
                            return "6";
                        }
                        else
                        {
                            return "12M";
                        }

                    };
                default: return string.Empty;
            }
        }

        #endregion

    }

}
