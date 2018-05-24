using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Log_File_Comparison
{
    public partial class Form1 : Form
    {
        public string fileName = "";
        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
        }

       
        private void BrowseFiles(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = (openFileDialog1.FileName);
            }

        }
        //public bool GetLogAsTXT(int maxLogsToReturn, int timePeriodInDays, string searchNarrative, string custId, int maxLineCount, string outputFileName)
        //{
        //    //based on the code above:-
        //    if (lat.GetLogAsTXT(100, 32, "Mid - Sale", "", 200, outputFile))
        //        // CBELogAsTXT main method to call to get the data from the SQL DB in Azure
        
        //}

        private void parseFilesButton_Click(object sender, EventArgs e)
        {
            
            long fileSize;
            long stringSize;
            long progress = 0;
            string filePath = textBoxFilePath.Text;
            string columnNames = "";
            //int counter = 0;
            int SD = 0;
            int SCO = 1;
            DataTable logFileDataTable = new DataTable();
            logFileDataTable.Columns.Add("SD");
            logFileDataTable.Columns.Add("SCO");

            StreamReader streamReader = new StreamReader(filePath);
            string[] logFileData = new string[File.ReadAllLines(filePath).Length];

            FileInfo f = new FileInfo(filePath);
            fileSize = f.Length;

            //progress bar
            loadingForm loadAndParse = new loadingForm(dataGridViewFiles, this, fileSize, progress);
            loadAndParse.Show();

            string probId = "";
            int columnCount = 2; 
            bool endOfLog = false;
            string[] filteredLogfile = new  string[2]; 
            string line = streamReader.ReadLine();
            logFileData = line.Split('\n');

            stringSize = line.Length + 2;
            progress += stringSize;
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                logFileData = line.Split('\n');
                stringSize = line.Length + 2;
                progress += stringSize;
                loadAndParse.setFilterprogress(progress);

                if (loadAndParse.iscanceled())
                {
                    loadAndParse.Close();
                    break;
                }
                Application.DoEvents();
                //get the first pipe and go back 3. Get the last pipe and +1
                foreach (var logLine in logFileData)
                {
                    if (logLine.StartsWith("-"))
                    {
                        continue;
                    }
                    if (logLine.Length < 50)
                    {
                        if (logLine.Contains("Obtaining"))
                        {
                            probId = logLine.Substring(0, 7);
                            continue;
                        }
                        else if (logLine.Contains("End TXT"))
                        {
                            endOfLog = true;
                        }
                        else
                            continue;
                    }
                    if (!endOfLog)
                    {
                        if (logLine.Substring(12, 1) == " ")
                        {
                            continue;
                        }
                        if (logLine.Substring(35, 3) == " ??")
                        {
                            continue;
                        }
                        filteredLogfile[SD] = logFileData[0].Substring(36, 2);
                        filteredLogfile[SCO] = logFileData[0].Substring(logFileData[0].LastIndexOf('|') + 1);
                    }


                }//end foreach loop

                //If it hasn't reached the end of a log file
                AddTableData(SD, SCO, logFileDataTable, endOfLog, filteredLogfile);

                //If it has reached the end of a log file
                EndOfLogFile(ref columnNames, SD, SCO, logFileDataTable, probId, columnCount, ref endOfLog, filteredLogfile);
                
            }//end while loop   

            loadAndParse.Close();
            MessageBox.Show("Your files have been parsed and saved");

        }
        
        private void EndOfLogFile(ref string columnNames,  int SD, int SCO, DataTable logFileDataTable, string probId, int columnCount, ref bool endOfLog, string[] filteredLogfile)
        {
            if (endOfLog)
            {
                if (logFileDataTable != null)
                {
                    dataGridViewFiles.DataSource = logFileDataTable;
                    if (logFileDataTable.Rows.Count != 0)
                    {
                        SaveLogToTxtFile(ref columnNames, probId, columnCount);
                        dataGridViewFiles.DataSource = null;

                        logFileDataTable.Rows.Clear();
                        filteredLogfile[SD] = null;
                        filteredLogfile[SCO] = null;
                    }
                }
                endOfLog = false;
            }
        }

        private static void AddTableData(int SD, int SCO, DataTable logFileDataTable, bool endOfLog, string[] filteredLogfile)
        {
            if (!endOfLog)
            {
                if (filteredLogfile != null)
                {
                    if (filteredLogfile[SD] != "   ")
                    {
                        if (filteredLogfile[SD] != null)
                        {
                            if (filteredLogfile[SCO] != "Versions")
                            {
                                logFileDataTable.Rows.Add(
                                   filteredLogfile[SD],
                                   filteredLogfile[SCO]
                                   );
                            }
                        }


                    }
                }

            }
        }

        private void SaveLogToTxtFile(ref string columnNames, string probId, int columnCount)
        {
            using (StreamWriter sw = new StreamWriter("p" + probId + ".txt"))
            {

                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dataGridViewFiles.Columns[i].Name.ToString() + ",";
                }
                //A quick hack to stop the column names from being added each time a new log file is created
                if (columnNames.Length > 7)
                {
                    columnNames = columnNames.Substring(0, 7);
                }
                sw.WriteLine(columnNames);
                for (int i = 0; i < dataGridViewFiles.RowCount; i++)
                {
                    string rowdata = "";
                    for (int j = 0; j < columnCount; j++)
                    {

                        if (dataGridViewFiles.Rows[i].Cells[j].Value != null)
                        {
                            rowdata += dataGridViewFiles.Rows[i].Cells[j].Value.ToString() + ",";
                        }
                        else
                        {
                            continue;
                        }
                       
                    }
                    sw.WriteLine(rowdata);
                    //break out of loop to avoid null object reference
                    if (i == dataGridViewFiles.RowCount)
                        break;
                }
            }
        }
     
        //to open notepad
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hwc, IntPtr hwp);
        private void Notepad_Click(object sender, EventArgs e)
        {
            Process p = Process.Start("notepad.exe");
            Thread.Sleep(500);
            p.WaitForInputIdle();
            SetParent(p.MainWindowHandle, this.Handle);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
                this.Close();
        }

        //this is to allow the debugging of the cluster details form
        private void chart1_Click(object sender, EventArgs e)
        {          
            ClusterDetailsForm frm2 = new ClusterDetailsForm();
            frm2.ShowDialog();       
        }
    }
}
