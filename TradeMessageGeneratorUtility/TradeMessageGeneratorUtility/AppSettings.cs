using System;
using System.Configuration;

namespace TradeMessageGenerator
{
    public static class AppSettings
    {
        public static string ColumnNames { get { return ConfigurationManager.AppSettings["columnNames"]; } }
        public static string RuntimeValuesForColumns { get { return ConfigurationManager.AppSettings["runtimeValuesForColumns"]; } }
        public static string DirectoryName { get { return ConfigurationManager.AppSettings["directoryName"]; } }
        public static int NumberOfRecords { get { return Convert.ToInt32(ConfigurationManager.AppSettings["numberOfRecords"]); } }
        public static string KeysToIngnor { get { return ConfigurationManager.AppSettings["keysToIngnor"]; } }
        public static string DirectoryToMonitor { get { return ConfigurationManager.AppSettings["directoryToMonitor"]; } }
    }
}
