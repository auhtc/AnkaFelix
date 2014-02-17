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
        // Variables And Properties
        public event PropertyChangedEventHandler PropertyChanged;
        Regex regex = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");

        private SerialPort serial;
        private SerialDataModel serialDataModel;

        private string serialData;
        public string SerialData
        {
            get { return serialData; }
        }

        private ObservableCollection<SerialDataModel> dataCollection;
        public ObservableCollection<SerialDataModel> DataCollection
        {
            get { return dataCollection; }
            set
            {
                if (value != dataCollection)
                {
                    dataCollection = value;
                    NotifyPropertyChanged("DataCollection");
                }
            }
        }

        public void ReadDataFromPort(string portName, string baudRate)
        {
            serial = new SerialPort();
            try
            {
                serial.PortName = portName;
                serial.BaudRate = Convert.ToInt32(baudRate);
                serial.NewLine = "\r";
                serial.Open();

                //DataCollection = new ObservableCollection<SerialDataModel>(); //Metod çağrıldığında nesne oluşur.
                serial.DataReceived += new SerialDataReceivedEventHandler(RecieveData);
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
            }
        }

        private void RecieveData(object sender, SerialDataReceivedEventArgs e)
        {
            DataCollection = new ObservableCollection<SerialDataModel>();  // Event her handle edilişinde yeni nesne oluşur.

            try
            {
                serialData = serial.ReadLine().Trim('$');
                System.Windows.Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal, (Action)delegate()
                    {
                        if (regex.IsMatch(serialData))
                        {
                            serialDataModel = new SerialDataModel();
                            serialDataModel.Name = serialData.Split(',')[0];
                            serialDataModel.Value = Convert.ToInt32(serialData.Split(',')[1]);
                            DataCollection.Add(serialDataModel);
                        }
                    });
            }
            catch
            {
                EndDataRead();
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal string EndDataRead()
        {
            string x = "Bağlantı Sonlandı!\n";
            foreach (SerialDataModel item in DataCollection)
            {
                x += item.Name + "-" + item.Value.ToString() + "\n";
            }
            serial.Close();
            return x;
        }
    }
}