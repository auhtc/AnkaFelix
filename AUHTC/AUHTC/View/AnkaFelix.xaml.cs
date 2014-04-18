﻿using AUHTC.View;
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
using System.Windows.Threading;

namespace AUHTC
{
    public partial class AnkaFelix : Window
    {

        Main mn;
        public AnkaFelix()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            Animation();
        }

        private void Animation()
        {
            this.Visibility = Visibility.Hidden;
            mn = new Main(this);
            mn.Show();
            DispatcherTimer tm = new DispatcherTimer();
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = new TimeSpan(0, 0, 5);
            tm.Start();
        }

        private void connStart_Click(object sender, RoutedEventArgs e)
        {
            ProcessedDataViews.DataContext = App.ViewModel.Data;

            if (App.ViewModel.ReadDataFromPort(Properties.Settings.Default.DefaultPortName, Properties.Settings.Default.DefaultBaudRate.ToString()))
            {
                SettingsButton.IsEnabled = false;
                connStart.Visibility = Visibility.Hidden;
                connEnd.Visibility = Visibility.Visible;
            }
        }

        private void connEnd_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.EndDataRead();
            SettingsButton.IsEnabled = true;
            connStart.Visibility = Visibility.Visible;
            connEnd.Visibility = Visibility.Hidden;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AUHTC.View.Settings settingsWindow = new AUHTC.View.Settings(this);
            this.IsEnabled = false;
            settingsWindow.ShowDialog();
        }

        private void mapStart_Click(object sender, RoutedEventArgs e)
        {
            AUHTC.View.Map mapWindow = new AUHTC.View.Map(this);
            this.Hide();
            mapWindow.ShowDialog();
        }

        private void tm_Tick(object sender, EventArgs e)
        {
            mn.Close();
            this.Visibility = Visibility.Visible;
        }
    }
}
