using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Vishleshan_Astra.Astra_Core.Action;
using Microsoft.Win32;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;

namespace Vishleshan_Astra.Astra.Infrastructure
{
    class ActivityService
    {
        List<KeyValuePair<string, DateTime>> FileMetadata { get; set; }

        public List<Tuple<string, string, DateTime, DateTime>> CheckFileSanity()
        {
            if (!CheckConfiguration())
                return new List<Tuple<string, string, DateTime, DateTime>>();

            var lstFilesWithDiscrepancy = new List<Tuple<string, string, DateTime, DateTime>>();
            foreach (var objFileMetadata in FileMetadata)
            {
                var dtCheckIn = objFileMetadata.Value;
                var dtLastModified = File.GetLastWriteTime(objFileMetadata.Key);
                if (dtLastModified < dtCheckIn)
                    lstFilesWithDiscrepancy.Add(Tuple.Create(Path.GetFileName(objFileMetadata.Key), objFileMetadata.Key, dtCheckIn, dtLastModified));
            }

            return lstFilesWithDiscrepancy;
        }

        public string PublishListOfUnsafeFiles(List<Tuple<string, string, DateTime, DateTime>> lstUnsafeFiles)
        {
            var strLogDirectory = GetDirectoryToLog();
            strLogDirectory = Path.Combine(path1: strLogDirectory, path2: "UnsafeFiles");

            if (!Directory.Exists(strLogDirectory))
                Directory.CreateDirectory(strLogDirectory);

            return SaveToCSVFile(lstUnsafeFiles, strLogDirectory);
        }

        public bool CheckConfiguration()
        {
            GetFilesToBeVerified();
            if (!FileMetadata.Any())
                return false;
            return true;
        }

        private static string GetDirectoryToLog()
        {
            var strCurrentBinDirectory = Environment.CurrentDirectory;
            return Directory.GetParent(path: strCurrentBinDirectory).Parent?.FullName;
        }

        private static string SaveToCSVFile(List<Tuple<string, string, DateTime, DateTime>> lstFilesWithDiscrepancy, string strDirectory)
        {
            if (!lstFilesWithDiscrepancy.Any())
                return string.Empty;

            var lstLines = new List<string> { "File Name,File Path,Check In Date,Last Modified Date" };
            foreach (var (strFileName, strFilePath, dtCheckIn, dtLastModified) in lstFilesWithDiscrepancy)
            {
                lstLines.Add(item: $"{strFileName},{strFilePath},{dtCheckIn},{dtLastModified}");
            }

            var strFileFullname = Path.Combine(strDirectory, $"UnsafeFiles-{DateTime.Now:dd'-'MMM'-'yyyy'T'HH'-'mm'-'ss}.csv");
            File.WriteAllLines(strFileFullname, lstLines.ToArray());
            return strFileFullname;
        }

        private void GetFilesToBeVerified()
        {
            var lstFilesToBeChecked = new List<string>();
            var lstValidConfigurations = GetConfigurationsForCurrentEnvironment();
            foreach (var strFilePath in lstValidConfigurations)
            {
                var strValidatedFile = CheckIfFileExist(strFilePath.Key);
                lstFilesToBeChecked.Add(strValidatedFile);
            }

            FileMetadata = lstValidConfigurations.Where(kv => lstFilesToBeChecked.Contains(kv.Key)).ToList();
        }

        private List<KeyValuePair<string, DateTime>> GetConfigurationsForCurrentEnvironment()
        {
            RegistryKey keyMachine = Registry.Users.OpenSubKey(@".DEFAULT\Software\Philips Healthcare\CT", false);
            if (keyMachine == null)
                return new List<KeyValuePair<string, DateTime>>();

            var machineType = (MachineType)Convert.ToInt32(keyMachine.GetValue("SystemType"));
            var lstFiles = new List<KeyValuePair<string, DateTime>>();
            switch (machineType)
            {
                case MachineType.Client:
                    var clientSection = ConfigurationManager.GetSection("clientConfiguration") as NameValueCollection;
                    foreach (var key in clientSection.AllKeys)
                    {
                        lstFiles.Add(new KeyValuePair<string, DateTime>(key, DateTime.ParseExact(clientSection.GetValues(key).FirstOrDefault(), "dd-MMM-yy", CultureInfo.InvariantCulture)));
                    }
                    break;
                case MachineType.Server:
                    var serverSection = ConfigurationManager.GetSection("serverConfiguration") as NameValueCollection;
                    foreach (var key in serverSection.AllKeys)
                    {
                        lstFiles.Add(new KeyValuePair<string, DateTime>(key, DateTime.ParseExact(serverSection.GetValues(key).FirstOrDefault(), "dd-MMM-yy", CultureInfo.InvariantCulture)));
                    }
                    break;
            }

            return lstFiles;
        }

        private string CheckIfFileExist(object objFilePath)
        {
            var strFilePath = Convert.ToString(objFilePath);
            if (string.IsNullOrWhiteSpace(strFilePath)) throw new ArgumentException(message: "File Path cannot be blank ");

            var strFileName = Path.GetFileName(strFilePath);
            if (string.IsNullOrWhiteSpace(strFileName)) throw new ArgumentException(message: $"File Name cannot be blank >> Path: {strFilePath}");

            return strFilePath;
        }

        private enum MachineType
        {
            Server = 3,
            Client = 4
        }
    }
}
