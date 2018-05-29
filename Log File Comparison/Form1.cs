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
using System.Windows.Forms.DataVisualization.Charting;

namespace Log_File_Comparison
{
    public partial class Form1 : Form
    {
        public string LogFileName = "";

        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
            PieChart();
            TestLog();
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
            int SD = 0;
            int message = 1;

            DataTable logFileDataTable = new DataTable();
            logFileDataTable.Columns.Add("SD");
            logFileDataTable.Columns.Add("Message");



            if (File.Exists(filePath))
            {
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
                string[] filteredLogfile = new string[2];
                string line = streamReader.ReadLine();
                logFileData = line.Split('\n');
                int lineCounter = 0;
                bool ObtainingFound = false;
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
                                ObtainingFound = true;
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
                            filteredLogfile[message] = logFileData[0].Substring(logFileData[0].LastIndexOf('|') + 1);
                        }
                       
                        if (lineCounter > 30)
                        {
                            if (!ObtainingFound)
                            {
                                MessageBox.Show("Please check the file is in the correct format");
                                loadAndParse.Close();
                                return;
                            }
                            
                        }

                    }//end foreach loop

                    //If it hasn't reached the end of a log file
                    AddTableData(SD, message, logFileDataTable, endOfLog, filteredLogfile);

                    //If it has reached the end of a log file
                    EndOfLogFile(ref columnNames, SD, message, logFileDataTable, probId, columnCount, ref endOfLog, filteredLogfile);
                    lineCounter++;
                }//end while loop   

                loadAndParse.Close();
                MessageBox.Show("Your files have been parsed and saved");
            }
            else
                MessageBox.Show("Are you sure the file exists??");
            PieChart();

        }
        
        //End of log file reached. Save and clear the rows for the next log.
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

                                string rowSignature = "";
                                var results = from myRow in logFileDataTable.AsEnumerable()                       
                                            select myRow;
                                var topRows = results.Take(10);
                                foreach (var item in topRows)
                                {
                                    rowSignature += item.ToString();
                                }

                        dataGridViewFiles.DataSource = null;

                        logFileDataTable.Rows.Clear();
                        filteredLogfile[SD] = null;
                        filteredLogfile[SCO] = null;
                    }
                }
                endOfLog = false;
            }
        }
        private void TestLog()
        {
            LogFile l1 = new LogFile { name = "log1", Signature = "AAAAAAAAAA", LDvalue = 100 };
            LogFile l2 = new LogFile { name = "log2", Signature = "AAAAAAAAAB", LDvalue = 100 };
            LogFile l3 = new LogFile { name = "log3", Signature = "AAAAAAAABB", LDvalue = 100 };
            LogFile l4 = new LogFile { name = "log4", Signature = "AAAAAAABBB", LDvalue = 100 };
            LogFile l5 = new LogFile { name = "log5", Signature = "AAAAAABBBB", LDvalue = 100 };
            LogFile l6 = new LogFile { name = "log6", Signature = "AAAAABBBBB", LDvalue = 100 };
            LogFile l7 = new LogFile { name = "log7", Signature = "AAAAAAAAAA", LDvalue = 100 };
            LogFile l8 = new LogFile { name = "log8", Signature = "AAAAAAAABA", LDvalue = 100 };
            LogFile l9 = new LogFile { name = "log9", Signature = "AAAAAAABAA", LDvalue = 100 };
            LogFile l10 = new LogFile { name = "log10", Signature = "BBAAAAAAAA", LDvalue = 100 };

           
            var LogList = new List<LogFile>();

            LogList.Add(l1); LogList.Add(l2); LogList.Add(l3); LogList.Add(l4); LogList.Add(l5);
            LogList.Add(l6); LogList.Add(l7); LogList.Add(l8); LogList.Add(l9); LogList.Add(l10);

                

            int index = 0;
            List<Cluster> clusterList = new List<Cluster>();
            string source, target;
            double result = 0;
            int minCluster = 3;
            double minTolerance = 80;
            foreach (LogFile log in LogList)
            {
                Cluster cluster = new Cluster();

                if (!logisinclusterlist(clusterList, log))
                {

                    cluster.MainLog.name = log.name;
                    cluster.MainLog.Signature = log.Signature;

                    for (int nextindex = index + 1; nextindex < LogList.Count(); nextindex++)
                    {
                        if (!logisinclusterlist(clusterList, LogList[nextindex]))
                        {


                            if (LogList[nextindex].Signature == log.Signature)
                            {
                                cluster.MatchedLogs.Add(LogList[nextindex]);

                            }
                            else
                            {
                                source = log.Signature;
                                target = LogList[nextindex].Signature;
                                result = (source.ToLower().CalculateSimilarity(target.ToLower()) * 100);
                                if (result >= minTolerance)
                                {
                                    LogList[nextindex].LDvalue = Convert.ToInt32(result);
                                    cluster.MatchedLogs.Add(LogList[nextindex]);
                                }
                            }
                        }

                    }
                }
                Console.WriteLine(cluster.MainLog);
                if (cluster.MatchedLogs.Count() + 1 >= minCluster)
                {
                    clusterList.Add(cluster);
                }
                index++;
            }
        }
        private static bool logisinclusterlist(List<Cluster> clusterList, LogFile log)
        {
            bool inlist = false;
            foreach (var cluster in clusterList)
            {
                if (log.name.Equals(cluster.MainLog.name))
                {
                    inlist = true;
                    break;
                }
                else
                {
                    foreach (LogFile ml in cluster.MatchedLogs)
                    {
                        if (ml.name.Equals(log.name))
                        {
                            inlist = true;
                            break;
                        }
                    }
                    if (inlist)
                    {
                        break;
                    }
                }
            }
            return inlist;
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
                    columnNames += dataGridViewFiles.Columns[i].Name.ToString();
                }
                //A quick hack to stop the column names from being added each time a new log file is created
                if (columnNames.Length > 7)
                {
                    columnNames = columnNames.Substring(2, 7);
                }
                sw.WriteLine(columnNames);
                for (int i = 0; i < dataGridViewFiles.RowCount; i++)
                {
                    string rowdata = "";
                    for (int j = 0; j < columnCount; j++)
                    {

                        if (dataGridViewFiles.Rows[i].Cells[j].Value != null)
                        {
                            rowdata += dataGridViewFiles.Rows[i].Cells[j].Value.ToString();
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


        public void PieChart()
        {
            string[] xValues = { "Cluster1", "Cluster2", "Cluster3", "Cluster4", "Cluster5" };
            int[] yValues = { 6, 9, 11, 4, 2 };
            //3D Pie chart
            chart1.Series[0].ChartType = SeriesChartType.Pie;
            chart1.Series[0].Points.DataBindXY(xValues, yValues);
            chart1.Legends[0].Enabled = true;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            //chart1.Focus();
        }

        public void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            HitTestResult hit = chart1.HitTest(e.X, e.Y, ChartElementType.DataPoint);
            if (hit.PointIndex >= 0 && hit.Series != null)
            {
                DataPoint dp = chart1.Series[0].Points[hit.PointIndex];
                labelPieChart.Text = "Value #" + hit.PointIndex + " = " + dp.XValue;
                
            }
            else labelPieChart.Text = "";
            
        }
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            HitTestResult hit = chart1.HitTest(e.X, e.Y);
            var dp = hit.Object as DataPoint;
            Cursor = (dp is null) ? Cursors.Default : Cursors.Hand;
        }
        private void viewResultsButton_Click(object sender, EventArgs e)
        {
            int minLinesLength = 0;
            int minCluster = 0;
            int minTolerance = 0;

            minLinesLength = Convert.ToInt32(linesNumericUpDown.Value);
            minCluster = Convert.ToInt32(clusterNumericUpDown.Value);
            minTolerance = Convert.ToInt32(toleranceNumericUpDown.Value);

        }
        public void btnDetails_Click(object sender, EventArgs e)
        {
            HitTestResult hit = chart1.HitTest((e as MouseEventArgs).X, (e as MouseEventArgs).Y);
            string cluster = "";
            cluster = labelPieChart.Text;
            if (cluster == "")
            {
                MessageBox.Show("Please select a cluster from the pie chart first");
            }
            else
            {
                MessageBox.Show("You selected the cluster " + cluster);

                LogFileName = cluster;
                ClusterDetailsForm frm2 = new ClusterDetailsForm();              
                frm2.Owner = this;
                frm2.LogName(cluster);
                frm2.ShowDialog();
            }
        }
    }
}
