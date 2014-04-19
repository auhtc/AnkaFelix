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

namespace AUHTC.View
{
    /// <summary>
    /// Interaction logic for Race.xaml
    /// </summary>
    public partial class Race : Window
    {
        public Race()
        {
            InitializeComponent();
        }
        DispatcherTimer timer1 = new DispatcherTimer();
        int tik = 60;
        int min = 1;
        int number;


        void timer1_Tick(Object sender, EventArgs e)
        {
            if (tik < 10)
            {
                CountdownTextBlock.Text = "0" + min.ToString() + ":" + "0" + tik.ToString();
                CountdownTextBlock.FontFamily = new FontFamily("/Fonts/digital-7.ttf#Digital-7");
            }
            else
                if (tik == 60)
                {
                    CountdownTextBlock.Text = "0" + min.ToString() + ":" + "00";
                    CountdownTextBlock.FontFamily = new FontFamily("/Fonts/digital-7.ttf#Digital-7");
                }
                else
                {
                    CountdownTextBlock.Text = "0" + min.ToString() + ":" + tik.ToString();
                    CountdownTextBlock.FontFamily = new FontFamily("/Fonts/digital-7.ttf#Digital-7");
                }
            if (tik > 0)
            {
                tik--;
                if (min > 0)
                    min--;

            }
            //else

            //    NavigationService.GoBack();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer1.Interval = new TimeSpan(0, 0, 0, 1);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
        }
    }
}
