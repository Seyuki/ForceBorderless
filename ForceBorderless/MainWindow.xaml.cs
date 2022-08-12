using ForceBorderless.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.Win32;
using MahApps.Metro.Controls;

namespace ForceBorderless
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public static ViewModel viewModel;

        /// <summary>
        /// Notify icon
        /// </summary>
        System.Windows.Forms.NotifyIcon Notify = new System.Windows.Forms.NotifyIcon();

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            // Window initialization
            InitializeComponent();

            // Load localization strings
            LocalizationLib.SetLanguageResourceDictionary(this);

            // Adds the data context
            viewModel = new ViewModel(SnackbarAdd);
            this.DataContext = viewModel;

            // Notify icon
            try
            {
                System.Windows.Forms.ContextMenu NotifyMenu = new System.Windows.Forms.ContextMenu();
                System.Windows.Forms.MenuItem NotifyMenu_Show = new System.Windows.Forms.MenuItem();
                System.Windows.Forms.MenuItem NotifyMenu_Quit = new System.Windows.Forms.MenuItem();

                NotifyMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NotifyMenu_Show });
                NotifyMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { NotifyMenu_Quit });

                NotifyMenu_Show.Index = 0;
                NotifyMenu_Show.Text = (string)FindResource("Show");
                NotifyMenu_Show.Click += new EventHandler(this.ShowApp);
                NotifyMenu_Quit.Index = 1;
                NotifyMenu_Quit.Text = (string)FindResource("Quit");
                NotifyMenu_Quit.Click += new EventHandler(this.QuitApp);

                Notify.Icon = Properties.Resources.notificon;
                Notify.ContextMenu = NotifyMenu;
                Notify.DoubleClick += new EventHandler(this.ShowApp);
                Notify.Text = (string)FindResource("WindowTitle");
                Notify.Visible = true;

            }
            catch { }

            // Application autostart
            string[] args = Environment.GetCommandLineArgs();

            foreach (string arg in args)
            {
                if (arg.ToLower() == "-autostart")
                    this.Hide();
            }
        }

        #region Window events

        /// <summary>
        /// Hiding window instead of minimizing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                this.Hide();
        }

        /// <summary>
        /// Closing window event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Dismiss notify icon
            Notify.Dispose();

            // Stops process listener thread
            WindowHandler.StopListener();

            // Saves processes whitelist
            Settings.Save(viewModel);
        }

        #endregion Window events

        #region Notify Icon

        /// <summary>
        /// Show window if hidden
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void ShowApp(object Sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Quit application
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void QuitApp(object Sender, EventArgs e)
        {
            this.Close();
        }

        #endregion Notify Icon
    }
}
