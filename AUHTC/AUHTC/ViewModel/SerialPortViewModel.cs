using AUHTC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AUHTC.ViewModel
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        public SerialPortViewModel()
        {
            database = new EntityViewModel();
            string test = database.GetSettings();
        }

        Regex rgx_StandartData = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");
        Regex rgx_GPSData = new Regex(@"(\$[A-Z]{1,},[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4})");

        public event PropertyChangedEventHandler PropertyChanged;

        private SerialPort serialPort;
        private EntityViewModel database;
        private string recievedData;

        int sayi = 0;
        private Point Offset1 { get; set; }
        private Point Offset2 { get; set; }
        private double RatioX { get; set; }
        private double RatioY { get; set; }

        //public StreamReader ReadFile;
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

        private ProcessedDataModel data;
        public ProcessedDataModel Data
        {
            get
            {
                return data;
            }
            set
            {
                if (value != data)
                {
                    data = value;
                    NotifyPropertyChanged("Data");
                }
            }
        }

        public bool ReadDataFromPort(string portName, string baudRate)
        {
            serialPort = new SerialPort();
            try
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = Convert.ToInt32(baudRate);
                serialPort.NewLine = "\r";
                serialPort.Open();

                serialPort.DataReceived += new SerialDataReceivedEventHandler(RecieveData);
                return true;
            }
            catch (Exception excp)
            {
                string message = excp.Message + "\n";
                string openPort = SerialPort.GetPortNames().ToList().FirstOrDefault();

                if (string.IsNullOrEmpty(openPort))
                {
                    message = "Kullanılabilir Bağlantı Noktası Yok!";
                    System.Windows.MessageBox.Show(message);
                }
                else
                {
                    message += "\n" + openPort + " bağlantı noktasını kullanmak ister misiniz?";
                    if (System.Windows.MessageBox.Show(message, "Uyarı!", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                    {
                        Properties.Settings.Default.DefaultPortName = openPort;
                        ReadDataFromPort(openPort, baudRate);
                    }
                }
                return false;
            }
        }

        private void RecieveData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                recievedData = serialPort.ReadLine().Trim('$');
                System.Windows.Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal, (Action)delegate()
                    {
                        data = new ProcessedDataModel();

                        if (rgx_StandartData.IsMatch(recievedData))
                        {
                            data.Name = recievedData.Split(',')[0];
                            data.Value = Convert.ToInt32(recievedData.Split(',')[1]);
                            data.RecordDate = DateTime.Now;
                            data.Type = "Standart Data";
                            SaveDataToDb(data);
                            // Hiz OrtalamaHiz Sicaklik Batarya1 Batarya2 Time değişkenleri Bind için hazır.
                            // Data hiç değişmemiş bind eder mi?
                            // HesapOrtalamaHiz HesapKalanSure HesapHarcananEnerji HesapTahminiEnerji HesapBitirmeOrtalamaHiz değerleri bind için hazır hesaplanacaklar.
                        }
                        else if (rgx_GPSData.IsMatch(recievedData))
                        {
                            //TODO recievedData App.MapModel.MapModel değişkenini dolduracak şekilde split edilecek

                            string[] words = recievedData.Split(',');
                            Marker(words[3], words[5]); // Okul mapi için koordinat verisinden sadece 4. ve 6. yı kullanıyorum
                            // Hollanda da meridyen farkı bilmem ne fark gösterme şansı var..
                            BataryaPercent(sayi++, 100 - sayi); // Örnek Kullanım
                            if (sayi == 100)
                            {
                                sayi = 0;
                            }
                            data.RecordDate = DateTime.Now;
                            data.Type = "GPS Data";
                            SaveDataToDb(data);
                        }
                        else
                        {
                            Data.Dump = recievedData;
                            data.RecordDate = DateTime.Now;
                            data.Type = "Incorrect Data";
                            SaveDataToDb(data);
                        }
                    });
            }
            catch
            {
                while (!serialPort.IsOpen)
                {
                    // TODO please wait gibi bişey eklenecek
                }
                RecieveData(sender, e);
                //EndDataRead(); //TODO Araç bağlantısı koparsa bağlantıyı komple kapatıyor. ---YAPILDI TEST EDİLECEK
            }
        }

        private void SaveDataToDb(ProcessedDataModel data)
        {
            database.AddDataToDB(data);
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void EndDataRead()
        {
            serialPort.Close();
        }

        private void Marker(string koor1, string koor2)
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
            string text = "";//ReadFile.ReadLine();

            Point Offset;

            Offset1 = new Point(3978242, 3281838); //TODO ayar dosyasından okunacak
            Offset2 = new Point(3978033, 3282213); //TODO ayar dosyasından okunacak

            Offset = new Point((Offset2.X - Offset1.X), (Offset2.Y - Offset1.Y));
            RatioX = 507 / Offset.X; //TODO 507 yerine MapImage.height şart
            RatioY = 700 / Offset.Y; //TODO 700 yerine MapImage.width şart

            if (text == null)
                return false;

            if (rgx_GPSData.IsMatch(text))
            {
                string[] words = text.Split(',');
                Marker(words[3], words[5]); // Okul mapi için koordinat verisinden sadece 4. ve 6. yı kullanıyorum
                // Hollanda da meridyen farkı bilmem ne fark gösterme şansı var..
                BataryaPercent(sayi++, 100 - sayi); // Örnek Kullanım
                if (sayi == 100)
                    sayi = 0;
            }
            return true;
        }

    }
}