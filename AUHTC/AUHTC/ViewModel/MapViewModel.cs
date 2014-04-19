using System;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace AUHTC.ViewModel
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private Point Offset1 { get; set; }
        private Point Offset2 { get; set; }
        private double RatioX { get; set; }
        private double RatioY { get; set; }

        public StreamReader ReadFile;
        
        Regex regex = new Regex(@"(\$[A-Z]{1,},[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4})");

        public event PropertyChangedEventHandler PropertyChanged;

        private Thickness koor;
        public Thickness Koor
        {
            get { return koor; }
            set
            {
                if (value != koor)
                {
                    koor = value;
                    NotifyPropertyChanged("Koor");
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
        private void Marker(int x,int y)
        {
            //TODO MapImage.top ve MapImage.left ihtiyaç! Kayma oluyor markerda
            Koor = new Thickness(y, x, 0, 0);
        }

        public bool ReadData()
        {
            string text = ReadFile.ReadLine();
            Offset1 = new Point(3978242, 3281838); //TODO ayar dosyasından okunacak
            Offset2 = new Point(3978033, 3282213); //TODO ayar dosyasından okunacak

            Point Offset = new Point((Offset2.X - Offset1.X), (Offset2.Y - Offset1.Y));
            RatioX = 507 / Offset.X; //TODO 507 yerine MapImage.height şart
            RatioY = 700 / Offset.Y; //TODO 700 yerine MapImage.width şart
            Regex regex = new Regex(@"\$[A-Z]{1,},[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4}");
                if (text == null)
                    return false;

                if (regex.IsMatch(text))
                {
                    string[] words = text.Split(',');
                    int KoorX = (int.Parse(words[3].Substring(0, 2)) * 100000) + (int)((float.Parse((words[3].Replace(".", ",")).Substring(2, words[3].Length - 3)) / 60) * 100000);
                    int KoorY = (int.Parse(words[5].Substring(0, 3)) * 100000) + (int)((float.Parse((words[5].Replace(".", ",")).Substring(3, words[5].Length - 4)) / 60) * 100000);
                    Marker((int)((KoorX - Offset1.X) * RatioX), (int)((KoorY - Offset1.Y) * RatioY));
                }
                return true;
        }
    }
}
