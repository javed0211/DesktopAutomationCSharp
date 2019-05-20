using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace SALTPages.Utility
{
    public class ExcelUtility
    {
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Function to connect specified excel or csv file
        /// And gets the used range object
        /// </summary>
        /// <param name="fileToConnect"></param>
        /// <returns>used range object</returns>
        public Excel.Worksheet ConnectToExcelFile(FileInfo fileToConnect)
        {
            if (fileToConnect.FullName != null)
            {
                log.Info(fileToConnect + " file has identifed");
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook xlWorkbook = excelApp.Workbooks.Open(fileToConnect.FullName);
                string sheet = xlWorkbook.ActiveSheet.Name;
                Excel.Worksheet xlWorksheet = xlWorkbook.Sheets[sheet];
                return xlWorksheet;
            }
            else
            {
                log.Info(fileToConnect + " File is not avaiable to connect");
                return null;
            }
        }

        /// <summary>
        /// Function to get used row count and coloumn count from excel file or csv file
        /// </summary>
        /// <param name="fileToConnect"></param>
        /// <returns>rowAndColCount int array</returns>
        public int[] ExcelUsedRowAndColCount(Excel.Worksheet xlWorksheet)
        {
            Excel.Range xlRange = xlWorksheet.UsedRange;
            if (xlRange != null)
            {
                int rowCount = xlRange.Rows.Count;
                log.Info("Number of rows used in excel = " + rowCount);
                int colCount = xlRange.Columns.Count;
                log.Info("Number of coloumns used in excel = " + colCount);
                int[] rowAndColCount = new int[] { rowCount, colCount };
                return rowAndColCount;
            }
            else
            {
                return null;
            }
        }

        public List<string> SearchForValueAndGivesRowAndColValueInExcelForMultipleID(Excel.Worksheet workSheet, int[] rowAndColCount, string ValueToSearch)
        {
            int rowCount = rowAndColCount[0];
            int colCount = rowAndColCount[1];
            int icount = 1;
            int jcount = 1;
            bool flag = false;
            List<string> excelValues = new List<string>();
            string valueInSheet = " ";
            int row = 0, j = 0;
            for (icount = 1; icount <= rowCount; icount++)
            {
                for (jcount = 1; jcount <= colCount; jcount++)
                {
                    valueInSheet = Convert.ToString(workSheet.Cells[icount, jcount].Value);
                    if (valueInSheet == ValueToSearch)
                    {
                        flag = true;
                        row = icount;
                        j = jcount;
                    }
                    if (flag)
                    {
                        break;

                    }
                }
                if (flag)
                {
                    break;
                }
            }

            for (int k = row; k <= rowCount; k++)
            {
                try
                {
                    valueInSheet = Convert.ToString(workSheet.Cells[k, j].Value);
                    if (valueInSheet == ValueToSearch)
                    {
                        excelValues.Add(k.ToString());
                    }
                }
                catch
                {
                    log.Error(ValueToSearch+"not found in excel");
                    return null;
                }
            }
            return excelValues;
        }
       
        public string[] SearchForValueAndGivesRowAndColValueInExcel(Excel.Worksheet workSheet,int[] rowAndColCount,string ValueToSearch)
        {
            int rowCount = rowAndColCount[0];
            int colCount = rowAndColCount[1];
            int icount = 1;
            int jcount = 1;
            int flag = 0;
            string[] excelValues = null;
            string valueInSheet = " ";
            for (icount = 1; icount <= rowCount; icount++)
            {
                for (jcount = 1; jcount <= colCount; jcount++)
                {
                    valueInSheet = Convert.ToString(workSheet.Cells[icount, jcount].Value);
                    if (string.IsNullOrEmpty(valueInSheet))
                    {
                        valueInSheet = "";
                        if (valueInSheet == ValueToSearch)
                        {
                            flag = 1;
                            excelValues = new string[] { valueInSheet, icount.ToString(), jcount.ToString() };
                            break;
                        }
                    }
                    if (valueInSheet.Trim() == ValueToSearch)
                    {
                        flag = 1;
                        excelValues = new string[] { valueInSheet, icount.ToString(), jcount.ToString() };                        
                        break;
                    }
                }
                if (flag == 1)
                {
                    log.Info("value " + ValueToSearch + " is present in " + workSheet.Name + " at cell [" + icount + "," + jcount + "]");
                    break;
                }
            }
            if(flag==0)
            {
                log.Info("value " + ValueToSearch + " is not present in " + workSheet.Name + " at cell [" + icount + "," + jcount + "]");
            }
            return excelValues;
        }

        public string[] ReadColumnsWithFixedRowNumber(Excel.Worksheet workSheet, int[] rowAndColCount, int rowNum, string expectedValue)
        {
            int rowCount = rowAndColCount[0];
            int colCount = rowAndColCount[1];
            int kCount = 1;
            string valueInSheet = " ";
            for (kCount = 1; kCount <= colCount; kCount++)
            {
                valueInSheet = Convert.ToString(workSheet.Cells[rowNum, kCount].Value);
                if (valueInSheet == expectedValue)
                {
                    log.Info(expectedValue + " is present in the " + workSheet + " file");
                    break;
                }
            }
            if (valueInSheet == expectedValue)
            {
                log.Info(expectedValue + " is present in the " + workSheet + " file");
                return new string[] { valueInSheet, rowNum.ToString(), kCount.ToString() };
            }
            else
            {
                log.Error(expectedValue + " is not present in the " + workSheet + " file");
                return null;
            }

        }

        public string[] ReadRowsWithFixedColNumber(Excel.Worksheet workSheet, int[] rowAndColCount, int colNum, string expectedValue)
        {
            int rowCount = rowAndColCount[0];
            int colCount = rowAndColCount[1];
            int LCount = 1;
            string valueInSheet = " ";
            for (LCount = 1; LCount <= rowCount; LCount++)
            {
                valueInSheet = Convert.ToString(workSheet.Cells[LCount, colNum].Value);
                if (valueInSheet == expectedValue)
                {
                    log.Info(expectedValue + " is present in the " + workSheet + " file");
                    break;
                }
            }
            if (valueInSheet == expectedValue)
            {
                log.Info(expectedValue + " is present in the " + workSheet + " file");
                return new string[] { valueInSheet, LCount.ToString(), colNum.ToString() };
            }
            else
            {
                log.Error(expectedValue + " is not present in the " + workSheet + " file");
                return null;
            }
        }


        /// <summary>
        /// Function to get specified file from shared drive
        /// </summary>
        /// <param name="pathofFile"></param>
        /// <returns>Latest file from directory</returns>
        public FileInfo getTheLatestFileFromServer(string filePath)
        {
            FileInfo result = null;
            DirectoryInfo path = new DirectoryInfo(@"\\gb-pb-gec-vt1\SALT\" + filePath);
            if (path.Exists)
            {
                var list = path.GetFiles();
                if (list.Count() > 0)
                {
                    result = list.OrderByDescending(f => f.LastWriteTime).First();
                }
                return result;
            }
            else
            {
                log.Error(@"\\gb-pb-gec-vt1\SALT\" + filePath + " Path not found");
                return result;
            }
        }
        
        /// <summary>
        /// This method retrieves the data from the text file into a datatable
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public DataTable GetHeathrowData(string fullName)
        {
            try
            {
                string path = fullName;
                //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                string Fulltext = "";
                List<string> data = new List<string>();
                List<string> headers = new List<string>();
                DataTable dt = new DataTable("Reports");
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        data = Fulltext.Split('\n').ToList(); //split full file text into rows 
                        data = data.Select(x => x.Replace("\t\t", ",")).ToList();
                        headers = data[0].Split(',').ToList();
                        headers = headers.Select(x => x.Trim('\r')).ToList();

                        string[] rows = new string[data.Count - 2];
                        data.CopyTo(1, rows, 0, data.Count - 2);

                        foreach (var item in headers)
                            dt.Columns.Add(item);

                        foreach (var item in rows)
                        {
                            DataRow dr = dt.NewRow();
                            int i = 0;
                            foreach (var rowValue in item.Split(','))
                            {
                                dr[i] = rowValue;
                                i++;
                            }
                            dt.Rows.Add(dr);
                        }

                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                log.Error("An exception occured while retrieving data from text file.\nException Message : " + ex.Message);
                return null;
            }
        }
    }
}
