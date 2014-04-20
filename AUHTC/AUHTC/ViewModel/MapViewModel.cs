using System;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace AUHTC.ViewModel
{
    public class MapViewModel : INotifyPropertyChanged
    {
        int sayi = 0;
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

        private ImageSource bataryaimage1;
        public ImageSource BataryaImage1 //Bind için hazır
        {
            get { return bataryaimage1; }
            set
            {
                if (value != bataryaimage1)
                {
                    bataryaimage1 = value;
                    NotifyPropertyChanged("BataryaImage1");
                }
            }
        }

        private ImageSource bataryaimage2;
        public ImageSource BataryaImage2 //Bind için hazır
        {
            get { return bataryaimage2; }
            set
            {
                if (value != bataryaimage2)
                {
                    bataryaimage2 = value;
                    NotifyPropertyChanged("BataryaImage2");
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
        private void Marker(string koor1,string koor2)
        {
            //TODO MapImage.top ve MapImage.left ihtiyaç! Kayma oluyor markerda
            int KoorX = (int)(((int.Parse(koor1.Substring(0, 2)) * 100000) + (int)((float.Parse((koor1.Replace(".", ",")).Substring(2, koor1.Length - 3)) / 60) * 100000) - Offset1.X) * RatioX);
            int KoorY = (int)(((int.Parse(koor2.Substring(0, 3)) * 100000) + (int)((float.Parse((koor2.Replace(".", ",")).Substring(3, koor2.Length - 4)) / 60) * 100000) - Offset1.Y) * RatioY);
            Koor = new Thickness(KoorY, KoorX, 0, 0);
        }

        private void BataryaPercent(int bat1, int bat2)
        {
            BataryaImage1 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat1 - 1) - ((bat1 - 1) % 10)) + ".png") as ImageSource;
            BataryaImage2 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat2 - 1) - ((bat2 - 1) % 10)) + ".png") as ImageSource;
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
                    Marker(words[3],words[5]); // Okul mapi için koordinat verisinden sadece 4. ve 6. yı kullanıyorum
                    // Hollanda da meridyen farkı bilmem ne fark gösterme şansı var..
                    BataryaPercent(sayi++,100-sayi); // Örnek Kullanım
                    if (sayi == 100)
                        sayi = 0;
                }
                return true;
        }
    }
}
