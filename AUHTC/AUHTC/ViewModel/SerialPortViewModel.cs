using AUHTC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                    NotifyPropertyChanged("DataCollection");
                }
            }
        }

        public void ReadDataFromPort(string portName, string baudRate)
        {
            serial = new SerialPort();
            dataCollection = new ObservableCollection<SerialDataModel>();
            try
            {
                serial.PortName = portName;
                serial.BaudRate = Convert.ToInt32(baudRate);
                serial.NewLine = "\r";
                serial.Open();

                serial.DataReceived += new SerialDataReceivedEventHandler(RecieveData);
            }
            catch
            {
            }
        }

        private void RecieveData(object sender, SerialDataReceivedEventArgs e)
        {
            serialData = serial.ReadLine();
            List<string> list = serialData.Split('$').ToList();

            foreach (string item in list)
            {
                if (regex.IsMatch(item))
                {
                    try
                    {
                        serialDataModel = new SerialDataModel();
                        serialDataModel.Name = item.Split(',')[0];
                        serialDataModel.Value = Convert.ToInt32(item.Split(',')[1]);
                        DataCollection.Add(serialDataModel);
                    }
                    catch
                    {
                        serial.Close();
                    }
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

        internal string EndDataRead()
        {
            string x = "";
            foreach (SerialDataModel item in dataCollection)
            {
                x += item.Name + "-" + item.Value.ToString() + "\n";
            }
            serial.Close();
            return x;
        }
    }
}