using ForceBorderless.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Main ViewModel
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Force 16:9 ratio flag
        /// </summary>
        private bool _Force169;

        /// <summary>
        /// Auto start with windows flag
        /// </summary>
        private bool _AutoStart;

        /// <summary>
        /// Collection of whitelisted processes
        /// </summary>
        private ObservableCollection<ProcessInfos> _Processes;

        /// <summary>
        /// Pressed keys list
        /// </summary>
        private List<Key> _keys;

        /// <summary>
        /// Key Listener
        /// </summary>
        private KeyboardListener _KListener;

        /// <summary>
        /// Name of the process to add (manually)
        /// </summary>
        private string _ProcessToAdd;

        /// <summary>
        /// A process name is entered
        /// </summary>
        private bool _IsProcessToAdd;

        /// <summary>
        /// Process snackbar
        /// </summary>
        private Snackbar _SnackbarAdd;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// Force 16:9 ratio flag
        /// </summary>
        public bool Force169
        {
            get { return _Force169; }
            set
            {
                _Force169 = value;
                OnPropertyChanged("Force169");
                WindowHandler.Force169 = value;
            }
        }

        /// <summary>
        /// Auto start with windows flag
        /// </summary>
        public bool AutoStart
        {
            get { return _AutoStart; }
            set
            {
                Settings.SetAutostart(value);

                _AutoStart = value;
                OnPropertyChanged("AutoStart");
            }
        }

        /// <summary>
        /// Collection of whitelisted processes
        /// </summary>
        public ObservableCollection<ProcessInfos> Processes
        {
            get { return _Processes; }
            set { _Processes = value; }
        }

        /// <summary>
        /// Pressed keys list
        /// </summary>
        public List<Key> keys
        {
            get { return _keys; }
            set { _keys = value; }
        }

        /// <summary>
        /// Key Listener
        /// </summary>
        public KeyboardListener KListener
        {
            get { return _KListener; }
            set { _KListener = value; }
        }

        /// <summary>
        /// Name of the process to add (manually)
        /// </summary>
        public string ProcessToAdd
        {
            get { return _ProcessToAdd; }
            set
            {
                _ProcessToAdd = value;
                _IsProcessToAdd = !string.IsNullOrEmpty(value);

                OnPropertyChanged("ProcessToAdd");
                OnPropertyChanged("IsProcessToAdd");
            }
        }

        /// <summary>
        /// A process name is entered
        /// </summary>
        public bool IsProcessToAdd
        {
            get { return _IsProcessToAdd; }
        }

        #endregion Accessors

        #region Events

        /// <summary>
        /// Property value changed event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var evt = this.PropertyChanged;
            if (evt != null)
                evt(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property value changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Commands

        private ICommand _logsCommand;
        private ICommand _aboutCommand;
        private ICommand _addProcessCommand;

        /// <summary>
        /// Show logs Command
        /// </summary>
        public ICommand logsCommand
        {
            get { return _logsCommand ?? (_logsCommand = new CommandHandler(param => Settings.ShowLogs(), true)); }
        }

        /// <summary>
        /// Show about Command
        /// </summary>
        public ICommand aboutCommand
        {
            get { return _aboutCommand ?? (_aboutCommand = new CommandHandler(param => this.ShowAbout(), true)); }
        }

        /// <summary>
        /// Add Process Command
        /// </summary>
        public ICommand addProcessCommand
        {
            get { return _addProcessCommand ?? (_addProcessCommand = new CommandHandler(param => this.AddProcess(), true)); }
        }

        #endregion Commands

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModel(Snackbar Snackbar)
        {
            this._SnackbarAdd = Snackbar;

            this._AutoStart = Settings.GetAutoStart();
            this._Processes = new ObservableCollection<ProcessInfos>();
            this._keys = new List<Key>();
            this._KListener = new KeyboardListener();

            Settings.Load(this);

            // Start process listener thread
            WindowHandler.StartListener(this.Processes);

            // Keys events
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListener_KeyUp);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Shows 'About' window
        /// </summary>
        private void ShowAbout()
        {
            About AboutWindow = new About();
            AboutWindow.Owner = Application.Current.MainWindow;
            AboutWindow.DataContext = new AboutViewModel();
            AboutWindow.ShowDialog();
        }

        /// <summary>
        /// Add process to the list
        /// </summary>
        private void AddProcess()
        {
            string processName = this.ProcessToAdd;

            // Add process
            bool IsAdded = ProcessInfos.Add(this.ProcessToAdd, this.Processes);
            this.ProcessToAdd = string.Empty;

            // Show snackbar
            var messageQueue = _SnackbarAdd.MessageQueue;
            var message = Application.Current.MainWindow.FindResource(IsAdded ? "ProcessAdded" : "ProcessDuplicate");
            message = message.ToString().Replace("{name}", processName);

            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }

        #region Key events

        /// <summary>
        /// KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            try
            {
                SetKeyDown(args.Key);

                // Ctrl + F11 (Action on foreground app)
                if (IsKeyDown(Key.LeftCtrl) && IsKeyDown(Key.F11))
                {
                    WindowHandler.ChangeBorderStyle(WindowLib.GetForegroundWindow());
                }
                // Ctrl + F12 (Adds / Removes foreground app to / from list)
                else if (IsKeyDown(Key.LeftCtrl) && IsKeyDown(Key.F12))
                {
                    ProcessInfos.AddOrRemove(this.Processes);
                }
            }
            catch { }
        }

        /// <summary>
        /// KeyUp event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void KListener_KeyUp(object sender, RawKeyEventArgs args)
        {
            SetKeyUp(args.Key);
        }

        /// <summary>
        /// Cheking if key is pressed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns></returns>
        private bool IsKeyDown(Key key)
        {
            return keys.Contains(key);
        }

        /// <summary>
        /// Adds key to the pressed keys list
        /// </summary>
        /// <param name="key">Touche</param>
        private void SetKeyDown(Key key)
        {
            if (!keys.Contains(key))
                keys.Add(key);
        }

        /// <summary>
        /// Whithdraws key from the pressed keys list
        /// </summary>
        /// <param name="key">Touche</param>
        private void SetKeyUp(Key key)
        {
            if (keys.Contains(key))
                keys.Remove(key);
        }

        #endregion Key events

        #endregion Methods
    }
}
