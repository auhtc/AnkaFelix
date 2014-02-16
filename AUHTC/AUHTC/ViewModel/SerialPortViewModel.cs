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
                    //Debug.Write(value);
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

                DataCollection = new ObservableCollection<SerialDataModel>(); //Metod çağrıldığında nesne oluşur.
                serial.DataReceived += new SerialDataReceivedEventHandler(RecieveData);
            }
            catch
            {
            }
        }

        private void RecieveData(object sender, SerialDataReceivedEventArgs e)
        {
            //DataCollection = new ObservableCollection<SerialDataModel>();  // Event her handle edilişinde yeni nesne oluşur.
            serialData = serial.ReadLine();
            List<string> list = serialData.Split('$').ToList();

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal, (Action)delegate()
                    {
                        foreach (string item in list)
                        {
                            if (regex.IsMatch(item))
                            {
                                serialDataModel = new SerialDataModel();
                                serialDataModel.Name = item.Split(',')[0];
                                serialDataModel.Value = Convert.ToInt32(item.Split(',')[1]);
                                DataCollection.Add(serialDataModel);
                            }
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