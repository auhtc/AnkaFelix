using AUHTC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        #region Properties And Variables

        public StreamReader ReadFile = null;

        private SettingsModel settings;

        private Regex rgx_StandartData;
        private Regex rgx_GPSData;

        private SerialPort serialPort;
        private EntityViewModel database;

        private string recievedData;

        private Point Offset1 { get; set; }
        private Point Offset2 { get; set; }
        private double RatioX { get; set; }
        private double RatioY { get; set; }

        int sayi = 0;

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

        private ImageSource bateryImage1;
        public ImageSource BateryImage1
        {
            get { return bateryImage1; }
            set
            {
                if (value != bateryImage1)
                {
                    bateryImage1 = value;
                    NotifyPropertyChanged("BateryImage1");
                }
            }
        }

        private ImageSource bateryImage2;
        public ImageSource BateryImage2
        {
            get { return bateryImage2; }
            set
            {
                if (value != bateryImage2)
                {
                    bateryImage2 = value;
                    NotifyPropertyChanged("BateryImage2");
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
        #endregion

        #region ctor

        public SerialPortViewModel()
        {
            database = new EntityViewModel();
            serialPort = new SerialPort();
            settings = database.GetSettingsByMapName(App.CurrentMapName);

            rgx_StandartData = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");
            rgx_GPSData = new Regex(@"(\$GPRMC,[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4})");

            Offset1 = new Point(settings.Offset1X, settings.Offset1Y);
            Offset2 = new Point(settings.Offset2X, settings.Offset2Y);

            Point Offset = new Point((Offset2.X - Offset1.X), (Offset2.Y - Offset1.Y));

            RatioX = App.AllConstants.MapHeight / Offset.X;
            RatioY = App.AllConstants.MapWidth / Offset.Y;
        }

        #endregion

        #region Methods

        public bool ReadDataFromPort(string portName, string baudRate)
        {
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
                        Properties.Settings.Default.DefaultPortName = openPort; //TODO -- çalışmıyor olabilir test edilecek
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
                recievedData = serialPort.ReadLine().Trim('$');  //trim işlemi alınan veriye göre değiştirilebilir test edilecek!
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
                            string[] words = recievedData.Split(',');
                            Marker(words[3], words[5]);// Okul mapi için koordinat verisinden sadece 4. ve 6. yı kullanıyorum
                            // Hollanda da meridyen farkı bilmem ne fark gösterme şansı var..

                            // Örnek Kullanım
                            BateryPercent(sayi++, 100 - sayi);
                            sayi %= 100;

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
                //TODO Araç bağlantısı koparsa bağlantıyı komple kapatıyor. ---YAPILDI TEST EDİLECEK
            }
        }

        private void SaveDataToDb(ProcessedDataModel data)
        {
            database.AddSerialDataToDB(data);
        }

        internal void EndDataRead()
        {
            serialPort.Close();
        }

        private void Marker(string koor1, string koor2)
        {
            //TODO MapImage.top ve MapImage.left ihtiyaç! Kayma oluyor markerda
            int KoorX = (int)App.AllConstants.MapHeight + (int)(((int.Parse(koor1.Substring(0, 2)) * 100000) + (int)((float.Parse((koor1.Replace(".", ",")).Substring(2, koor1.Length - 3)) / 60) * 100000) - Offset1.X) * RatioX);
            int KoorY = (int)App.AllConstants.MapWidth + (int)(((int.Parse(koor2.Substring(0, 3)) * 100000) + (int)((float.Parse((koor2.Replace(".", ",")).Substring(3, koor2.Length - 4)) / 60) * 100000) - Offset1.Y) * RatioY);
            Koor = new Thickness(KoorY, KoorX, 0, 0);
        }

        private void BateryPercent(int bat1, int bat2)
        {
            BateryImage1 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat1 - 1) - ((bat1 - 1) % 10)) + ".png") as ImageSource;
            BateryImage2 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat2 - 1) - ((bat2 - 1) % 10)) + ".png") as ImageSource;
        }

        internal bool ReadData()
        {
            string text = ReadFile.ReadLine();

            if (text == null)
                return false;

            if (rgx_GPSData.IsMatch(text))
            {
                string[] words = text.Split(',');
                Marker(words[3], words[5]); // Okul mapi için koordinat verisinden sadece 4. ve 6. yı kullanıyorum
                // Hollanda da meridyen farkı bilmem ne fark gösterme şansı var..
                BateryPercent(sayi++, 100 - sayi); // Örnek Kullanım
                if (sayi == 100)
                    sayi = 0;
            }
            return true;

        }

        internal void SaveMapToDB(string mapName, string mapLocation, string offset1, string offset2)
        {
            SettingsModel MapData = new SettingsModel();
            MapData.MapLocation = mapLocation;
            MapData.MapName = mapName;
            MapData.Offset1X = float.Parse(offset1.Split(',')[0].Replace('.', ','));
            MapData.Offset1Y = float.Parse(offset1.Split(',')[1].Replace('.', ','));
            MapData.Offset2X = float.Parse(offset2.Split(',')[0].Replace('.', ','));
            MapData.Offset2Y = float.Parse(offset2.Split(',')[1].Replace('.', ','));

            settings.MapImage = File.ReadAllBytes(mapLocation);

            database.SaveSettingsToDB(MapData);
            //TODO Fonksiyon yazılacak.
        }
        internal List<string> ReadAllMapFromDB()
        {
            List<string> Liste = new List<string>();

            foreach (var item in database.GetAllSettings())
            {
                Liste.Add(item.MapName);
            }
            return Liste;
        }
        internal SettingsModel ReadMapFromDB(string name)
        {
            return database.GetSettingsByMapName(name);
        }

        #endregion

        #region Property Changed Event Processes

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}