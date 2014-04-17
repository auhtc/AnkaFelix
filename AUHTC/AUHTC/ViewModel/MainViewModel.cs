using System;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace AUHTC.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private delegate void NoArgDelegate();
        public MainViewModel()
        {
            Thread thread = new Thread(new ThreadStart(delegate
            {
                while (mainopacity != 700)
                {
                    Thread.Sleep(2);
                    MainOpacity = mainopacity + 1;
                }

                //AnkaFelix frm = new AnkaFelix();
                //frm.Show();
            }));
            thread.Start();
        }

        private double mainopacity;
        public double MainOpacity
        {
            get { return mainopacity / 700; }
            set
            {
                if (value != mainopacity)
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
    }
}
