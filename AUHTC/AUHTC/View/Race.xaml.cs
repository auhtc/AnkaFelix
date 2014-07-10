using System;
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
        AnkaFelix ParentAnka;
        public double sayi = 1366;
        bool RaceStatus = false;
        public Race(AnkaFelix parent,ImageSource mapImage)
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;

            MapImage.Source = mapImage;
            App.AllConstants.Inıt(MapImage);

            ParentAnka = parent;
        }

        DispatcherTimer timer1 = new DispatcherTimer();
        DateTime end;

        void timer1_Tick(Object sender, EventArgs e)
        {
            if (sayi == -1080)
            {
                sayi = this.Width + 360;
            }
            if (RaceStatus == true)
                CountdownTextBlock.Text = string.Format("{0:" + AUHTC.Properties.Settings.Default.CountDownFormat.Replace(":", "\\:").Replace(".", "\\.") + "}", end - DateTime.Now);
            SponsorPanel.Margin = new Thickness(sayi--, SponsorPanel.Margin.Top, SponsorPanel.Margin.Right, SponsorPanel.Margin.Bottom);
        }

        private void RaceStop_Click(object sender, RoutedEventArgs e)
        {
            RaceStatus = false;
            RaceStart.Visibility = Visibility.Visible;
            RaceStop.Visibility = Visibility.Hidden;
        }

        private void RaceStart_Click(object sender, RoutedEventArgs e)
        {
            end = DateTime.Now.AddMinutes(39);
            RaceStatus = true;
            RaceStop.Visibility = Visibility.Visible;
            RaceStart.Visibility = Visibility.Hidden;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            ParentAnka.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer1.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            switch (cmb.Margin.Top.ToString())
            {
                case "10": // Değişkenler OTO Pilot
                    cmb.SelectedItem = App.DefaultDegisken;
                    break;
                case "40": // Operatörler OTO Pilot
                    cmb.SelectedItem = App.DefaultOperator;
                    break;
                case "70": // Değerler OTO Pilot
                    cmb.SelectedItem = App.DefaultDeger;
                    break;
                case "100": // İşlemler OTO Pilot
                    cmb.SelectedItem = App.DefaultIslem;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            switch (btn.Margin.Right.ToString())
            {
                case "10":
                    // Sil Eventi
                    break;
                case "100":
                    // Ekle Eventi
                    //App.MapModel.AddRule(DegiskenCombo, OperatorCombo, DegerCombo, IslemCombo);
                    break;
            }
        }
    }
}
