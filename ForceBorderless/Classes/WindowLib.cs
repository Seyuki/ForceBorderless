using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Class containing windows related static methods
    /// </summary>
    public static class WindowLib
    {
        #region Properties

        /// <summary>
        /// RECT struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Window styles
        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; // Child window
        public static int WS_BORDER = 0x00800000; // Simple border
        public static int WS_DLGFRAME = 0x00400000; // Double border without title bar
        public static int WS_THICKFRAME = 0x00040000; // Sizable border
        public static int WS_SYSMENU = 0x00080000; // Menu bar
        public static int WS_MINIMIZE = 0x20000000; // Minimize button
        public static int WS_MAXIMIZEBOX = 0x00010000; // Maximize button
        public static int SWP_NOMOVE = 0x2;
        public static int SWP_NOSIZE = 0x1;
        public static int SWP_FRAMECHANGED = 0x20;
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; // Window with title bar

        #endregion Properties

        #region Methods

        /// <summary>
        /// Changes window style
        /// </summary>
        /// <param name="hWnd">Window hWnd</param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Gets window style
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("user32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Moves and resizes window
        /// </summary>
        /// <param name="hWnd">Window hWnd</param>
        /// <param name="X">X location</param>
        /// <param name="Y">Y location</param>
        /// <param name="nWidth">new width</param>
        /// <param name="nHeight">new height</param>
        /// <param name="bRepaint">Does the window need a repaint or not</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// Get the rect of the windows passed as a parameter
        /// </summary>
        /// <param name="hWnd">hWnd (window) to get RECT</param>
        /// <param name="lpRect">Window RECT</param>
        /// <returns>true if successful, false if failed</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// Get the foreground window
        /// </summary>
        /// <returns>Foreground window's hWnd</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Get the window's process Id
        /// </summary>
        /// <param name="hWnd">Window</param>
        /// <param name="lpdwProcessId">Process ID</param>
        /// <returns> window's process Id</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Get the hWnd attached to the process (by Name)
        /// </summary>
        /// <param name="ProcessName">Name of the process</param>
        /// <returns>hWnd attached to the process</returns>
        public static IntPtr GethWnd(string ProcessName)
        {
            Process[] Processes = Process.GetProcessesByName(ProcessName);
            return (Processes.Length > 0) ? Processes[0].MainWindowHandle : IntPtr.Zero;
        }

        /// <summary>
        /// Get the process to which hWnd is attached
        /// </summary>
        /// <param name="hWnd">hWnd to get process</param>
        /// <returns>Process name</returns>
        public static string GetProcessName(IntPtr hWnd)
        {
            try
            {
                uint actProcessId = 0;
                GetWindowThreadProcessId(hWnd, out actProcessId);
                Process actProcess = Process.GetProcessById((int)actProcessId);
                return actProcess.ProcessName;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the process' name in the foreground
        /// </summary>
        /// <returns>Process name</returns>
        public static string GetForegroundProcessName()
        {
            return GetProcessName(GetForegroundWindow());
        }

        #endregion Methods
    }
}
