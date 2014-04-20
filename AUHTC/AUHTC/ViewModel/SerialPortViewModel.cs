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
using System.Windows.Threading;

namespace AUHTC.ViewModel
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        Regex rgx_StandartData = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");
        Regex rgx_GPSData = new Regex(@"(\$[A-Z]{1,},[0-9\.]{9},[A-Z],[0-9\.]{10},[A-Z],[0-9\.]{11},[A-Z],[0-9\.]{5},[0-9\.]{0,6},[0-9]{6},,,[0-9A-Z\*]{4})");

        public event PropertyChangedEventHandler PropertyChanged;

        private ProcessedDataModel processedData;
        private SerialPort serialPort;
        private EntityViewModel database;
        private string recievedData;

        //private ObservableCollection<ProcessedDataModel> dataCollection;
        //public ObservableCollection<ProcessedDataModel> DataCollection
        //{
        //    get { return dataCollection; }
        //    set
        //    {
        //        if (value != dataCollection)
        //        {
        //            dataCollection = value;
        //            NotifyPropertyChanged("DataCollection");
        //        }
        //    }
        //}

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
            database = new EntityViewModel();
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
                EndDataRead(); //TODO Araç bağlantısı koparsa bağlantıyı komple kapatıyor.
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
    }
}