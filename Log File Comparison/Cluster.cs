using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log_File_Comparison
{
    public class Cluster
    {
        public LogFile MainLog { get; set; }
        public List<LogFile> MatchedLogs { get; set; }

        public Cluster()
        {
            MatchedLogs = new List<LogFile>();
            MainLog = new LogFile();
        }
    }
    public class LogFile
    {
        public string name { get; set; }
        public string Signature { get; set; }
        public int LDvalue { get; set; }
        public LogFile()
        {

        }
    }
    
}
