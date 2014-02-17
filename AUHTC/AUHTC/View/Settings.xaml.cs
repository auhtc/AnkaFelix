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

namespace AUHTC.View
{
    public partial class Settings : Window
    {
        AnkaFelix Parent;
        public Settings(AnkaFelix parent)
        {
            InitializeComponent();
            Parent = parent;
        }

        private void PortNamesCombobox_Loaded(object sender, RoutedEventArgs e)
        {
            PortNamesCombobox.SelectedItem = App.DefaultPortName;
        }

        private void BaudRateCombobox_Loaded(object sender, RoutedEventArgs e)
        {
            BaudRatesCombobox.SelectedItem = App.DefaultBaudRate;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.DefaultPortName = PortNamesCombobox.SelectedItem.ToString();
            Properties.Settings.Default.DefaultBaudRate = Convert.ToInt32(BaudRatesCombobox.SelectedItem);
            Properties.Settings.Default.Save();
        }

        private void OkeyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            this.Close();
            Parent.IsEnabled = true;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Parent.IsEnabled = true;
        }
    }
}
