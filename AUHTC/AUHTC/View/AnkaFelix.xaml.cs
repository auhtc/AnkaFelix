using AUHTC.Model;
using AUHTC.View;
using System;
using System.Windows;
using System.Windows.Media;
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
            tm.Interval = new TimeSpan(0, 0, 1);
            tm.Start();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AUHTC.View.Settings settingsWindow = new AUHTC.View.Settings(this);
            settingsWindow.ShowDialog();
        }

        private void mapStart_Click(object sender, RoutedEventArgs e)
        {
            if (App.AllConstants.Setting == null)
            {
                MessageBox.Show("Map Ayarları Yapılmamış");
                AUHTC.View.Settings settingsWindow = new AUHTC.View.Settings(this);
                settingsWindow.ShowDialog();
            }
            else if (!App.ViewModel.ReadDataFromPort(Properties.Settings.Default.DefaultPortName, Properties.Settings.Default.DefaultBaudRate.ToString()))
            //else if(false)
            {
                return;
            }
            else
            {
                byte[] a = App.AllConstants.Setting.MapImage;
                ImageSource map = App.ViewModel.Byte2Image(a);
                AUHTC.View.Race mapWindow = new AUHTC.View.Race(this, map);
                mapWindow.ShowDialog();
            }

            this.Hide();
        }

        private void tm_Tick(object sender, EventArgs e)
        {
            mn.Close();
            this.Visibility = Visibility.Visible;
            ((DispatcherTimer)sender).Stop();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.EndDataRead();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.ViewModel.EndDataRead();
        }
    }
}
