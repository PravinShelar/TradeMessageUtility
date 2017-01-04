using System;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace TradeMessageGenerator
{
    public static class ExcelFileGenerator
    {

        //This method will create a excel file with the messages generated and store it in /TradeWeb folder
        //with it's session ID.

        #region Public Static Methods

        public static bool CreateExcel(DataSet ds, string fullFileName, string useCaseHeader)
        {
            Excel.Application excelApp;
            Excel.Workbook excelWorkbook;
            Excel.Worksheet excelWorksheet;
            Excel.Range excelCellrange;

            try
            {
                excelApp = new Excel.Application();

                // for making Excel visible
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                // Creation a new Workbook
                excelWorkbook = excelApp.Workbooks.Add(Type.Missing);

                //excelWorkbook = excelApp.Workbooks.Open(AppSettings.ExcelFilePath);

                foreach (DataTable table in ds.Tables)
                {
                    excelWorksheet = excelWorkbook.Sheets.Add();
                    excelWorksheet.Name = table.TableName;

                    int rowStartingCell = 1;

                    for (int i = 1; i < table.Columns.Count + 1; i++)
                    {
                        excelWorksheet.Cells[rowStartingCell, i] = table.Columns[i - 1].ColumnName;
                    }

                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        for (int k = 0; k < table.Columns.Count; k++)
                        {
                            excelWorksheet.Cells[j + rowStartingCell + 1, k + 1] = table.Rows[j].ItemArray[k].ToString();
                        }
                    }

                    // To resize columns
                    excelCellrange = excelWorksheet.Range[excelWorksheet.Cells[rowStartingCell, 1], excelWorksheet.Cells[table.Rows.Count, table.Columns.Count]];
                    excelCellrange.EntireColumn.AutoFit();

                    //To display worksheet header
                    if (rowStartingCell == 4)
                    {
                        excelCellrange = excelWorksheet.Range[excelWorksheet.Cells[2, 1], excelWorksheet.Cells[2, table.Columns.Count]];
                        FormattingExcelCells(excelCellrange, "#EBEADC", System.Drawing.Color.RosyBrown, true);
                        excelWorksheet.Cells[2, 1] = useCaseHeader;
                    }

                    //To format column headers
                    excelCellrange = excelWorksheet.Range[excelWorksheet.Cells[rowStartingCell, 1], excelWorksheet.Cells[1, table.Columns.Count]];
                    FormattingExcelCells(excelCellrange, true);

                }

                //excelWorkbook.Save();
                excelWorkbook.SaveAs(fullFileName);
                excelWorkbook.Close();
                excelApp.Quit();
                return true;
            }
            catch (Exception ex)
            {
                //handle exception
                return false;
            }
            finally
            {
                excelWorksheet = null;
                excelCellrange = null;
                excelWorkbook = null;
            }

        }

        #endregion

        #region Private Static Methods

        private static void FormattingExcelCells(Excel.Range range, bool IsFontbold)
        {
            if (IsFontbold == true)
            {
                range.Font.Bold = IsFontbold;
            }
        }

        private static void FormattingExcelCells(Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            range.Merge();
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }

        #endregion

    }
}
