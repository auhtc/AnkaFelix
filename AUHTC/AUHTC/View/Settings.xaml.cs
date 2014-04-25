using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        AnkaFelix ParentAnka;

        public Settings(AnkaFelix parent)
        {
            InitializeComponent();
            ParentAnka = parent;

            //TODO Veritabanından haritalar çekilip listboxa doldurulacak
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

            // Race içindeki string format = "mm:ss.fff"; satırı kısmına oku yaz yapılcak. Boşluk yasak . : / kullanılabilir.
        }

        private void OkeyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            this.Close();
            ParentAnka.IsEnabled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ParentAnka.IsEnabled = true;
        }

        private void EditMap_Click(object sender, RoutedEventArgs e)
        {
            NewAndEdit.IsEnabled = false;
            EditControls.IsEnabled = true;
        }

        private void NewMap_Click(object sender, RoutedEventArgs e)
        {
            AddNewMap();
        }

        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(mapnameTextBox.Text))
            {
                MessageBox.Show("Harita Adı Girilmedi!");
                mapnameTextBox.Focus();
            }
            else if (string.IsNullOrEmpty(offsetTextbox1.Text))//TODO regex yazılacak: else if(regex.IsMatch(offsetTextbox1.Text))
            {
                MessageBox.Show("İlk Offset Değeri Girilmedi!");
                offsetTextbox1.Focus();
            }
            else if (string.IsNullOrEmpty(offsetTextbox2.Text))//TODO regex yazılacak: else if(regex.IsMatch(offsetTextbox2.Text))
            {
                MessageBox.Show("İkinci Offset Değeri Girilmedi!");
                offsetTextbox2.Focus();
            }
            else
            {
                string sourceFile = MapImage.Source.ToString().Substring(8);
                string destinationFile = "/MediaFiles/Maps/" + mapnameTextBox.Text;
                File.Copy(sourceFile, destinationFile);  //Test edilecek

                App.ViewModel.SaveMapToDB(mapnameTextBox.Text, destinationFile, offsetTextbox1.Text, offsetTextbox2.Text);
                EditControls.IsEnabled = false;
                NewAndEdit.IsEnabled = true;
            }
        }

        private void MapImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditControls.IsEnabled)
            {
                AddNewMap();
            }
        }

        private void MapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO seçilen haritanın değerleri Harita Ayarlarındaki ilgili yerlere doldurulacak
        }

        private void AddNewMap()
        {
            OpenFileDialog map = new OpenFileDialog();

            map.Filter = "*(*.png)|*.png|Tüm dosyalar (*.*)|*.*";
            map.Multiselect = false;
            map.RestoreDirectory = true;
            map.Title = "Harita Seçiniz";

            if (map.ShowDialog() == true)
            {
                mapnameTextBox.Text = string.Empty;
                offsetTextbox1.Text = string.Empty;
                offsetTextbox2.Text = string.Empty;
                NewAndEdit.IsEnabled = false;
                EditControls.IsEnabled = true;
                MapImage.Source = new BitmapImage(new Uri(map.FileName));
            }
        }
    }
}
