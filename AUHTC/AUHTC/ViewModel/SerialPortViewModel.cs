using AUHTC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AUHTC.ViewModel
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        #region Properties And Variables

        private Regex rgx_StandartData;
        private Regex rgx_GPSData;

        private SerialPort serialPort;

        private string recievedData;

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
            serialPort = new SerialPort();

            rgx_StandartData = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");
            rgx_GPSData = new Regex(@"(\$GPRMC,[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4})");

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

                            // Hiz OrtalamaHiz Sicaklik Batarya1 Batarya2 Time değişkenleri Bind için hazır.
                            // HesapOrtalamaHiz HesapKalanSure HesapHarcananEnerji HesapTahminiEnerji HesapBitirmeOrtalamaHiz değerleri bind için hazır hesaplanacaklar.

                            SaveDataToDb(data);
                        }
                        else if (rgx_GPSData.IsMatch(recievedData))
                        {
                            string[] words = recievedData.Split(',');
                            Marker(words[3], words[5]);
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
            App.Database.AddSerialDataToDB(data);
        }

        internal void EndDataRead()
        {
            serialPort.Close();
        }

        private void Marker(string koor1, string koor2)
        {
            int KoorX = (int)App.AllConstants.MapTop + (int)(((int.Parse(koor1.Substring(0, 2)) * 100000) + (int)((float.Parse((koor1.Replace(".", ",")).Substring(2, koor1.Length - 3)) / 60) * 100000) - App.AllConstants.Offset.X) * App.AllConstants.RatioX);
            int KoorY = (int)App.AllConstants.MapLeft + (int)(((int.Parse(koor2.Substring(0, 3)) * 100000) + (int)((float.Parse((koor2.Replace(".", ",")).Substring(3, koor2.Length - 4)) / 60) * 100000) - App.AllConstants.Offset.Y) * App.AllConstants.RatioY);
            Koor = new Thickness(KoorY, KoorX, 0, 0);
        }

        private void BateryPercent(int bat1, int bat2)
        {
            BateryImage1 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat1 - 1) - ((bat1 - 1) % 10)) + ".png") as ImageSource;
            BateryImage2 = new ImageSourceConverter().ConvertFromString("../../MediaFiles/Batarya/" + ((bat2 - 1) - ((bat2 - 1) % 10)) + ".png") as ImageSource;
        }

        internal void SaveMapToDB(int id, string mapName, byte[] mapImage, string offset1, string offset2)
        {
            SettingsModel MapData = new SettingsModel();
            MapData.Id = id;
            MapData.MapName = mapName;
            MapData.Offset1X = offset1.Split(',')[0];
            MapData.Offset1Y = offset1.Split(',')[1];
            MapData.Offset2X = offset2.Split(',')[0];
            MapData.Offset2Y = offset2.Split(',')[1];
            MapData.MapImage = mapImage;

            App.Database.SaveSettingsToDB(MapData);
        }
        
        public byte[] Image2Byte(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public ImageSource Byte2Image(byte[] byteArrayIn)
        {
            var image = new BitmapImage();
            if (byteArrayIn == null || byteArrayIn.Length == 0)
                return null;

            var mem = new MemoryStream(byteArrayIn);
            mem.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = null;
            image.StreamSource = mem;
            image.EndInit();
            image.Freeze();
            return image;
        }

        internal List<string> ReadAllMapFromDB()
        {
            List<string> Liste = new List<string>();

            foreach (var item in App.Database.GetAllSettings())
            {
                Liste.Add(item.MapName);
            }
            return Liste;
        }
        
        internal SettingsModel ReadMapFromDB(string name)
        {
            return App.Database.GetSettingsByMapName(name);
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