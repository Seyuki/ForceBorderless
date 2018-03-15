using System;
using System.Reflection;
using System.Windows.Input;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// ViewModel for the 'About' Window
    /// </summary>
    public class AboutViewModel
    {
        #region Properties

        /// <summary>
        /// Application version
        /// </summary>
        private string _Version;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// Application version
        /// </summary>
        public string Version
        {
            get { return _Version; }
        }

        #endregion Accessors

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AboutViewModel()
        {
            Version Version = Assembly.GetExecutingAssembly().GetName().Version;
            this._Version = $"{Version.Major}.{Version.Minor}.{Version.Build}.{Version.Revision}";
        }

        #endregion Constructor

        #region Commands

        /// <summary>
        /// href Command
        /// </summary>
        private ICommand _GoToCommand;

        /// <summary>
        /// href Command
        /// </summary>
        public ICommand GoToCommand
        {
            get
            {
                return _GoToCommand ?? (_GoToCommand = new CommandHandler(param => GoToUrl((string)param), true));
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Open browser with specified url
        /// </summary>
        /// <param name="url">Website url</param>
        public void GoToUrl(string url) => System.Diagnostics.Process.Start(url);

        #endregion Methods
    }
}
