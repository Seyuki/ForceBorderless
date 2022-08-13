using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Static class containing thread and methods for forcing borderless
    /// and restore default window style
    /// </summary>
    public static class WindowHandler
    {
        #region Properties & Fields

        /// <summary>
        /// List of hWnd
        /// </summary>
        private static List<hWndInfos> _hWndList { get; set; } = new List<hWndInfos>();

        /// <summary>
        /// List of already handled hWnd
        /// </summary>
        private static List<IntPtr> _ExcludedhWnd { get; set; } = new List<IntPtr>();

        /// <summary>
        /// Last handled hWnd
        /// </summary>
        private static IntPtr _lasthWnd { get; set; }

        /// <summary>
        /// Last handler success flag
        /// </summary>
        private static bool _IsSuccessful { get; set; } = true;

        /// <summary>
        /// Thread that listens to running processes
        /// </summary>
        private static Thread _ProcessListener;

        /// <summary>
        /// Processes whitelist
        /// </summary>
        private static ObservableCollection<ProcessInfos> _Whitelist;

        /// <summary>
        /// Force 16:9 ratio flag
        /// </summary>
        public static bool Force169;

        #endregion Properties & Fields

        #region Methods

        /// <summary>
        /// Start process listener thread
        /// </summary>
        public static void StartListener(ObservableCollection<ProcessInfos> Whitelist)
        {
            _Whitelist = Whitelist;

            _ProcessListener = new Thread(ListenProcesses);
            _ProcessListener.Start();
        }

        /// <summary>
        /// Stop process listener thread if running
        /// </summary>
        public static void StopListener()
        {
            if (_ProcessListener != null && _ProcessListener.IsAlive)
                _ProcessListener.Abort();
        }

        /// <summary>
        /// Parcours la liste des processus actifs pour forcer le traitement sur les processus de la liste blanche
        /// </summary>
        private static void ListenProcesses()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);

                    if (_Whitelist.Count > 0)
                    {
                        // Vérification du besoin de traiter la fenêtre actuelle
                        IntPtr hWnd = WindowLib.GetForegroundWindow();

                        if (!_ExcludedhWnd.Any(x => x == hWnd) && (hWnd != _lasthWnd || (hWnd == _lasthWnd && _IsSuccessful == false)))
                        {
                            uint actProcessId = 0;
                            WindowLib.GetWindowThreadProcessId(hWnd, out actProcessId);
                            Process actProcess = Process.GetProcessById((int)actProcessId);

                            if (_Whitelist.Any(i => i.Name.ToUpper() == actProcess.ProcessName.ToUpper()))
                            {
                                ChangeBorderStyle(hWnd, actProcess);
                            }

                            _lasthWnd = hWnd;
                            _IsSuccessful = true;
                        }
                        // Nettoyage des handle stockés
                        foreach (IntPtr StockedhWnd in _ExcludedhWnd)
                        {
                            uint StockedProcessId = 0;
                            WindowLib.GetWindowThreadProcessId(StockedhWnd, out StockedProcessId);

                            if (StockedProcessId == 0) { _ExcludedhWnd.Remove(StockedhWnd); }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Change window style (switch between borderless and default style)
        /// </summary>
        /// <param name="hWndGame">Window hWnd to handle</param>
        public static void ChangeBorderStyle(IntPtr hWndGame, Process ProcessGame = null)
        {
            Settings.HandlingMode mode = (ProcessGame == null) ? Settings.HandlingMode.MANUAL : Settings.HandlingMode.AUTO;

            try
            {
                int actStyle = WindowLib.GetWindowLong(hWndGame, WindowLib.GWL_STYLE);

                if (actStyle == 0)
                {
                    Settings.WriteInLogs(Settings.ErrorCategory.WARNING, mode, $"Can't get window style of '{WindowLib.GetProcessName(hWndGame)}'! Trying again...");
                    if (ProcessGame != null) { _IsSuccessful = false; }
                    return;
                }

                if (ProcessGame != null)
                {
                    if (!ProcessGame.Responding)
                    {
                        Settings.WriteInLogs(Settings.ErrorCategory.WARNING, mode, $"Process '{WindowLib.GetProcessName(hWndGame)}' not responding! Trying again...");
                        _IsSuccessful = false;
                        return;
                    }

                    // MainWindow.ExcludedProcesses.Add(ProcessGame.Id);
                    _ExcludedhWnd.Add(hWndGame);
                }

                if (_hWndList.Any(i => i.id == hWndGame))
                {
                    // Get process info
                    hWndInfos hWndInfo = _hWndList.Find(i => i.id == hWndGame);

                    // Restablishing window style
                    WindowLib.SetWindowLong(hWndGame, WindowLib.GWL_STYLE, hWndInfo.Style);

                    // Restablishing window dimension and location
                    WindowLib.MoveWindow(hWndGame, hWndInfo.LocationX, hWndInfo.LocationY, hWndInfo.Width, hWndInfo.Height, true);

                    // Deleting process info (to refresh them when called again)
                    _hWndList.Remove(hWndInfo);

                    // Write info to logs
                    Settings.WriteInLogs(Settings.ErrorCategory.INFO, mode, $"Successfully restored old style of '{WindowLib.GetProcessName(hWndGame)}'!");
                }
                else
                {
                    // Add process infos
                    WindowLib.RECT rect = new WindowLib.RECT();
                    WindowLib.GetWindowRect(hWndGame, ref rect);
                    hWndInfos hWndInfo = new hWndInfos(hWndGame, actStyle, rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
                    _hWndList.Add(hWndInfo);

                    // Deleting title bar
                    WindowLib.SetWindowLong(hWndGame, WindowLib.GWL_STYLE, (actStyle & ~WindowLib.WS_CAPTION & ~WindowLib.WS_THICKFRAME & ~WindowLib.WS_SYSMENU & ~WindowLib.WS_MINIMIZE & ~WindowLib.WS_MAXIMIZEBOX));

                    // Changing window dimension and location
                    if (WindowHandler.Force169)
                    {
                        int width = (int)SystemParameters.WorkArea.Height / 9 * 16;
                        int xPos = ((int)SystemParameters.WorkArea.Width - width) / 2;
                        WindowLib.MoveWindow(hWndGame, xPos, 0, width, (int)SystemParameters.WorkArea.Height, true);
                    }
                    else
                    {
                        WindowLib.MoveWindow(hWndGame, 0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, true);
                    }
                    
                    // Write info to logs
                    Settings.WriteInLogs(Settings.ErrorCategory.INFO, mode, $"Successfully handled '{WindowLib.GetProcessName(hWndGame)}'!");
                }
            }
            catch
            {
                Settings.WriteInLogs(Settings.ErrorCategory.ERROR, mode, $"An error occured when changing style of '{WindowLib.GetProcessName(hWndGame)}'!");
            }
        }

        #endregion Methods
    }
}
