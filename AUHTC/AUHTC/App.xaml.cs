using AUHTC.Model;
using AUHTC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AUHTC
{
    public partial class App : Application
    {
        #region Properties And Variables

        public static EntityViewModel Database { get; set; }

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

        private static Constants allConstants;
        public static Constants AllConstants
        {
            get { return allConstants; }
        }

        private static string currentMapName;
        public static string CurrentMapName
        {
            get { return currentMapName; }
            set { currentMapName = value; }
        }

        // Otomatik Pilot
        private static List<string> degiskenler;
        public static List<string> Degiskenler
        {
            get { return degiskenler; }
        }

        private static string defaultDegisken;
        public static string DefaultDegisken
        {
            get { return defaultDegisken; }
        }

        private static List<string> operatorler;
        public static List<string> Operatorler
        {
            get { return operatorler; }
        }

        private static string defaultOperator;
        public static string DefaultOperator
        {
            get { return defaultOperator; }
        }

        private static List<int> degerler;
        public static List<int> Degerler
        {
            get { return degerler; }
        }

        private static int defaultDeger;
        public static int DefaultDeger
        {
            get { return defaultDeger; }
        }

        private static List<string> islemler;
        public static List<string> Islemler
        {
            get { return islemler; }
        }

        private static string defaultIslem;
        public static string DefaultIslem
        {
            get { return defaultIslem; }
        }

        #endregion

        #region ctor

        public App()
        {
            portNames = AUHTC.Properties.Settings.Default.PortNames.Split(';').ToList();
            baudRates = AUHTC.Properties.Settings.Default.BaudRates.Split(';').ToList().Select(b => Int32.Parse(b)).ToList();
            
            defaultPortName = AUHTC.Properties.Settings.Default.DefaultPortName;
            defaultBaudRate = AUHTC.Properties.Settings.Default.DefaultBaudRate;
            currentMapName = AUHTC.Properties.Settings.Default.MapName;

            // Otomatik Pilot Combo
            degiskenler = AUHTC.Properties.Settings.Default.Degiskenler.Split(';').ToList();
            defaultDegisken = AUHTC.Properties.Settings.Default.DefaultDegisken;
            operatorler = AUHTC.Properties.Settings.Default.Operatorler.Split(';').ToList();
            defaultOperator = AUHTC.Properties.Settings.Default.DefaultOperator;
            degerler = AUHTC.Properties.Settings.Default.Degerler.Split(';').ToList().Select(b => Int32.Parse(b)).ToList();
            defaultDeger = AUHTC.Properties.Settings.Default.DefaultDeger;
            islemler = AUHTC.Properties.Settings.Default.Islemler.Split(';').ToList();
            defaultIslem = AUHTC.Properties.Settings.Default.DefaultIslem;

            Database = new EntityViewModel();
            allConstants = new Constants();
            viewModel = new SerialPortViewModel();
        }

        #endregion
    }
}
