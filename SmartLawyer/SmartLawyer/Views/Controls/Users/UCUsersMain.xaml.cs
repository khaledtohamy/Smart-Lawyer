﻿using SmartLawyer.ViewModels;
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

namespace SmartLawyer.Views.Controls.Users
{
    /// <summary>
    /// Interaction logic for UCUsersMain.xaml
    /// </summary>
    public partial class UCUsersMain : UserControl
    {
        public UCUsersMain()
        {
            //DataContext = VMUsers.Create();
            InitializeComponent();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
