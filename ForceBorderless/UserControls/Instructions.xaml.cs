﻿using ForceBorderless.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForceBorderless.UserControls
{
    /// <summary>
    /// Interaction logic for Instructions.xaml
    /// </summary>
    public partial class Instructions : UserControl
    {
        public Instructions()
        {
            InitializeComponent();

            // Load localization strings
            LocalizationLib.SetLanguageResourceDictionary(this);
        }
    }
}
