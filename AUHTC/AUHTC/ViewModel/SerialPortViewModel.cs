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
        Regex regex = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");

        public event PropertyChangedEventHandler PropertyChanged;

        private SerialDataModel serialDataModel;
        private SerialPort serialPort;
        private string recievedData;

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

        public bool ReadDataFromPort(string portName, string baudRate)
        {
            serialPort = new SerialPort();
            try
            {
                serialPort.PortName = portName;
                serialPort.BaudRate = Convert.ToInt32(baudRate);
                serialPort.NewLine = "\r";
                serialPort.Open();

                DataCollection = new ObservableCollection<SerialDataModel>(); //Metod çağrıldığında nesne oluşur.
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
            //DataCollection = new ObservableCollection<SerialDataModel>();  // Event her handle edilişinde yeni nesne oluşur.
            try
            {
                recievedData = serialPort.ReadLine().Trim('$');
                System.Windows.Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal, (Action)delegate()
                    {
                        if (regex.IsMatch(recievedData))
                        {
                            serialDataModel = new SerialDataModel();
                            serialDataModel.Name = recievedData.Split(',')[0];
                            serialDataModel.Value = Convert.ToInt32(recievedData.Split(',')[1]);
                            serialDataModel.RecordDate = DateTime.Now;
                            DataCollection.Add(serialDataModel);
                            AddDataToDB(serialDataModel);
                        }
                    });
            }
            catch
            {
            }
        }

        private void AddDataToDB(SerialDataModel serialDataModel)
        {
            EntityViewModel database = new EntityViewModel();
            database.AddSerialDataToDB(serialDataModel);
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