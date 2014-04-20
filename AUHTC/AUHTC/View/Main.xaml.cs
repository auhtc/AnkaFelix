using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace AUHTC.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : INotifyPropertyChanged
    {
        AnkaFelix ParentAnka;
        public Main(AnkaFelix parent)
        {
            InitializeComponent();
            this.DataContext = this;
            ParentAnka = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private double mainopacity;
        public double MainOpacity
        {
            get { return mainopacity / 700; }
            set
            {
                if (mainopacity != value)
                {
                    mainopacity = value;
                    NotifyPropertyChanged("MainOpacity");
                }
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate
            {
                while (mainopacity != 700)
                {
                    Thread.Sleep(2);
                    MainOpacity = mainopacity + 1;
                }
            }));
            thread.Start();
        }
    }
}
