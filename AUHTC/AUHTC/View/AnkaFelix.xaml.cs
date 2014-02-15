using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AUHTC
{
    public partial class AnkaFelix : Window
    {
        public AnkaFelix()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        private void connStart_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.ReadDataFromPort(PortNamesCombobox.SelectedItem.ToString(), BaudRatesCombobox.SelectedItem.ToString());
        }

        private void PortNamesCombobox_Loaded(object sender, RoutedEventArgs e)
        {
            PortNamesCombobox.SelectedItem = App.DefaultPortName;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.DefaultPortName = PortNamesCombobox.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void BaudRateCombobox_Loaded(object sender, RoutedEventArgs e)
        {
            BaudRatesCombobox.SelectedItem = App.DefaultBaudRate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = App.ViewModel;
        }

        private void connEnd_Click(object sender, RoutedEventArgs e)
        {
            string totalData = App.ViewModel.EndDataRead();
            //System.Collections.ObjectModel.ObservableCollection<Model.SerialDataModel> sm = App.ViewModel.DataCollection;
            MessageBox.Show(totalData);
            //Test for Github
        }
    }
}
