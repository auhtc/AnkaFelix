using AUHTC.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AUHTC
{
    public partial class App : Application
    {
        private static SerialPortViewModel viewModel;
        public static SerialPortViewModel ViewModel
        {
            get { return viewModel; }
        }

        private static List<string> portNames;
        public static List<string> PortNames
        {
            get { return portNames; }
        }

        private static List<int> baudRates;
        public static List<int> BaudRates
        {
            get { return baudRates; }
        }

        private static string defaultPortName;
        public static string DefaultPortName
        {
            get { return defaultPortName; }
        }

        private static int defaultBaudRate;
        public static int DefaultBaudRate
        {
            get { return defaultBaudRate; }
        }

        public App()
        {
            portNames = AUHTC.Properties.Settings.Default.PortNames.Split(';').ToList();
            baudRates = AUHTC.Properties.Settings.Default.BaudRates.Split(';').ToList().Select(b => Int32.Parse(b)).ToList();

            defaultPortName = AUHTC.Properties.Settings.Default.DefaultPortName;
            defaultBaudRate = AUHTC.Properties.Settings.Default.DefaultBaudRate;

            viewModel = new SerialPortViewModel();
        }
    }
}
