using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Class containing whitelist processes infos and methods
    /// </summary>
    public class ProcessInfos
    {
        #region Properties

        /// <summary>
        /// Process name
        /// </summary>
        private string _Name;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// Process name
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = FormatName(value); }
        }

        #endregion Accessors

        #region Constructors

        /// <summary>
        /// Constructor with process name
        /// </summary>
        /// <param name="Name">Process name</param>
        public ProcessInfos(string Name) => this.Name = Name;

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Format name (exclude .exe)
        /// </summary>
        /// <param name="name">Process name</param>
        /// <returns></returns>
        private static string FormatName(string name)
        {
            if (name.ToLower().EndsWith(".exe"))
                name = name.Substring(0, name.Length - 4);

            return name;
        }

        /// <summary>
        /// Add a process to the whitelist
        /// </summary>
        /// <param name="Name">Name of the process to add</param>
        /// <param name="Whitelist">Collection where the new process need to be added</param>
        /// <returns>true if added</returns>
        public static bool Add(string Name, ObservableCollection<ProcessInfos> Whitelist)
        {
            // Check if process name is not empty
            if (string.IsNullOrEmpty(Name)) return false;

            // Format name
            Name = FormatName(Name);

            // Check if the process doesn't already exists
            foreach (ProcessInfos Process in Whitelist)
            {
                if (Process.Name.ToLower() == Name.ToLower()) return false;
            }

            // Add process to the collection
            Whitelist.Add(new ProcessInfos(Name));

            return true;
        }

        /// <summary>
        /// Add or remove foreground application process to / from the whitelist
        /// </summary>
        /// <param name="Whitelist">Collection where the new process need to be added / removed</param>
        public static void AddOrRemove(ObservableCollection<ProcessInfos> Whitelist)
        {
            // Get process name
            string Name = WindowLib.GetForegroundProcessName();

            // Check if the process is in the collection
            foreach (ProcessInfos Process in Whitelist)
            {
                if (Process.Name == Name)
                {
                    Whitelist.Remove(Process);
                    return;
                };
            }

            // Add process to the collection
            Whitelist.Add(new ProcessInfos(Name));
        }

        #endregion Methods
    }
}
