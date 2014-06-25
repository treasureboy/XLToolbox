﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLToolbox.Error;

namespace XLToolbox
{
    /// <summary>
    /// Interaction logic for WindowRuntimeError.xaml
    /// </summary>
    public partial class WindowRuntimeError : Window
    {
        Reporter Reporter { get; set; }
        public WindowRuntimeError()
        {
            InitializeComponent();
        }

        public WindowRuntimeError(Reporter r) : this()
        {
            Reporter = r;
            this.DataContext = r;
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            WindowErrorReport w = new WindowErrorReport(Reporter);
            w.ShowDialog();
        }
    }
}
