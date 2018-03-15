using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Static class containing methods related to settings and user params
    /// </summary>
    public static class Settings
    {
        #region Fields

        private static string _SettingsPath = @"Seyuki\ForceBorderless";
        private static string _WhitelistName = "whitelist.txt";
        private static string _LogsName = "logs.txt";

        #endregion Fields

        #region Logs enum

        public enum ErrorCategory { INFO, WARNING, ERROR };
        public enum HandlingMode { AUTO, MANUAL };

        #endregion Logs enum

        #region Methods

        /// <summary>
        /// Get autostart configuration
        /// </summary>
        /// <returns>autostart flag</returns>
        public static bool GetAutoStart()
        {
            bool isAutoStart = false;

            try
            {
                string Key_autostart = Assembly.GetExecutingAssembly().GetName().Name;

                RegistryKey REG_Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                foreach (string REG_Value_Name in REG_Key.GetValueNames())
                {
                    if (REG_Value_Name == Key_autostart)
                    {
                        isAutoStart = true;
                        break;
                    }
                }

                REG_Key.Close();
            }
            catch { }

            return isAutoStart;
        }

        /// <summary>
        /// Change autostart configuration : settings and registry changes
        /// </summary>
        /// <param name="autostart">autostart flag</param>
        public static void SetAutostart(bool autostart)
        {
            try
            {
                string Key_autostart = Assembly.GetExecutingAssembly().GetName().Name;

                // If a registry key already exists, we delete it
                RegistryKey REG_Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                foreach (string REG_Value_Name in REG_Key.GetValueNames())
                {
                    if (REG_Value_Name == Key_autostart)
                    {
                        REG_Key.DeleteValue(Key_autostart);
                        break;
                    }
                }

                // if autostart, we add the key
                if (autostart)
                {
                    REG_Key.SetValue(Key_autostart, "\"" + Assembly.GetExecutingAssembly().Location + "\" -autostart");
                }

                REG_Key.Close();
            }
            catch
            {
                MessageBox.Show("Unable to make changes in the registry!" , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load processes whitelist stocked on disk in the collection
        /// </summary>
        /// <param name="Whitelist">Collection to fill</param>
        public static void LoadWhitelist(ObservableCollection<ProcessInfos> Whitelist)
        {
            // Get entire path of whitelist file
            string WhitelistFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath, _WhitelistName);

            // Checking if whitelist file exists
            if (File.Exists(WhitelistFile))
            {
                // Read file
                using (StreamReader sr = new StreamReader(WhitelistFile))
                {
                    while (sr.Peek() >= 0)
                    {
                        Whitelist.Add(new ProcessInfos(sr.ReadLine()));
                    }
                }
            }
        }

        /// <summary>
        /// Save processes whitelist on disk
        /// </summary>
        /// <param name="Whitelist">Collection of whitelisted processes</param>
        public static void SaveWhitelist(ObservableCollection<ProcessInfos> Whitelist)
        {
            // Create directory if not exists
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath));

            // Get entire path of whitelist file
            string WhitelistFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath, _WhitelistName);

            // Write processes name in file
            using (StreamWriter writer = new StreamWriter(WhitelistFile))
            {
                foreach (ProcessInfos Process in Whitelist)
                {
                    writer.WriteLine(Process.Name);
                }
            }
        }

        /// <summary>
        /// Show application logs
        /// </summary>
        public static void ShowLogs()
        {
            // Get entire path of logs file
            string LogsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath, _LogsName);

            if (File.Exists(LogsFile))
                System.Diagnostics.Process.Start(LogsFile);
        }

        /// <summary>
        /// Write a line in logs file
        /// </summary>
        /// <param name="category">Error category</param>
        /// <param name="mode">Handling mode</param>
        /// <param name="message">Message to write in logs</param>
        public static void WriteInLogs(ErrorCategory category, HandlingMode mode, string message)
        {
            // Create directory if not exists
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath));

            // Get entire path of logs file
            string LogsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SettingsPath, _LogsName);

            // Write message in logs
            using (StreamWriter logs = new StreamWriter(LogsFile, true))
            {
                string msgDate = DateTime.Now.ToString();
                string msgCategory = $"[{category.ToString()}]";
                string msgMode = $"{mode.ToString()}".PadRight(6);

                logs.WriteLine($"{msgDate} - {msgCategory} {msgMode} : {message}");
            }
        }

        #endregion Methods
    }
}
