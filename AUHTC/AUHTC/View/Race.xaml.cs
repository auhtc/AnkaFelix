﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AUHTC.View
{
    /// <summary>
    /// Interaction logic for Race.xaml
    /// </summary>
    public partial class Race : Window
    {
        Thread thread;
        AnkaFelix ParentAnka;
        public Race(AnkaFelix parent)
        {
            InitializeComponent();
            this.DataContext = App.MapModel;
            ParentAnka = parent;
        }

        DispatcherTimer timer1 = new DispatcherTimer();
        DateTime end;
        string format = "mm:ss.fff";

        void timer1_Tick(Object sender, EventArgs e)
        {
            CountdownTextBlock.Text = string.Format("{0:" + format.Replace(":", "\\:").Replace(".", "\\.") + "}", end - DateTime.Now);
        }

        private void RaceStop_Click(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            timer1.Stop();
            App.MapModel.ReadFile = null;
            RaceStart.Visibility = Visibility.Visible;
            RaceStop.Visibility = Visibility.Hidden;
        }

        private void RaceStart_Click(object sender, RoutedEventArgs e)
        {
            end = DateTime.Now.AddMinutes(39);
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

            App.MapModel.ReadFile = File.OpenText("../../MediaFiles/c.txt");
            thread = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (!App.MapModel.ReadData())
                        thread.Abort();
                    Thread.Sleep(100);
                }
            }));
            thread.Start();
            RaceStop.Visibility = Visibility.Visible;
            RaceStart.Visibility = Visibility.Hidden;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (thread != null)
                thread.Abort();
            timer1.Stop();
            App.MapModel.ReadFile = null;
            ParentAnka.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //StreamReader sponsoroku = new StreamReader("../../MediaFiles/Sponsor/Sponsor.html");
            //string htmlicerik = sponsoroku.ReadToEnd().Replace("pathtoimage", "file://" + System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\", "/") + "/../../../MediaFiles/Sponsor");
            //SponsorWebBrowser.NavigateToString(htmlicerik);
            //sponsoroku.Close();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            switch (cmb.Margin.Top.ToString())
            {
                case "10":
                    cmb.SelectedItem = App.DefaultDegisken;
                    break;
                case "40":
                    cmb.SelectedItem = App.DefaultOperator;
                    break;
                case "70":
                    cmb.SelectedItem = App.DefaultDeger;
                    break;
                case "100":
                    cmb.SelectedItem = App.DefaultIslem;
                    break;
            }
        }
    }
}
