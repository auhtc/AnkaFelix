using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            foreach (string item in App.ViewModel.ReadAllMapFromDB())
            {
                MapList.Items.Add(item);
            }
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
            ParentAnka.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ParentAnka.Show();
        }

        private void EditMap_Click(object sender, RoutedEventArgs e)
        {
            NewAndEdit.IsEnabled = false;
            EditControls.IsEnabled = true;
        }

        private void NewMap_Click(object sender, RoutedEventArgs e)
        {
            AddNewMap();

            mapnameTextBox.Text = string.Empty;
            offsetTextbox1.Text = string.Empty;
            offsetTextbox2.Text = string.Empty;
            NewAndEdit.IsEnabled = false;
            EditControls.IsEnabled = true;
        }

        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            Regex rgx_Offset = new Regex(@"(\d{2}\.\d{5}\,\d{2}\.\d{5})");

            if (string.IsNullOrEmpty(mapnameTextBox.Text)) // Mapname Unique olsun eğer varsa save çalışsın yoksa add calışsın
            {
                MessageBox.Show("Harita Adı Girilmedi!");
                mapnameTextBox.Focus();
            }
            else if (!rgx_Offset.IsMatch(offsetTextbox1.Text))
            {
                MessageBox.Show("İlk Offset Değeri İstenilen Şablona Uymadı!\n\nİstenilen Şablon:\nXX.XXXXX,XX.XXXXX");
                offsetTextbox1.Focus();
            }
            else if (!rgx_Offset.IsMatch(offsetTextbox2.Text))
            {
                MessageBox.Show("İkinci Offset Değeri İstenilen Şablona Uymadı!\n\nİstenilen Şablon:\nXX.XXXXX,XX.XXXXX");
                offsetTextbox2.Focus();
            }
            else
            {
                string sourceFile = MapImage.Source.ToString().Substring(8);
                string extension = System.IO.Path.GetExtension(sourceFile);
                string destinationFile = "/MediaFiles/Maps/" + mapnameTextBox.Text + extension;
                if (!File.Exists(System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\", "/") + "/../../.." + destinationFile))
                {
                    File.Copy(sourceFile, System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\", "/") + "/../../.." + destinationFile, true); // Dosya varsa hata veriyor
                }
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
            if (MapList.SelectedIndex != -1)
            {
                string FileLocation;
                var setting = App.ViewModel.ReadMapFromDB(MapList.SelectedItem.ToString());
                mapnameTextBox.Text = setting.MapName;
                offsetTextbox1.Text = setting.Offset1X.ToString().Replace(',', '.') + "," + setting.Offset1Y.ToString().Replace(',', '.');
                offsetTextbox2.Text = setting.Offset2X.ToString().Replace(',', '.') + "," + setting.Offset2Y.ToString().Replace(',', '.');
                NewAndEdit.IsEnabled = false;
                EditControls.IsEnabled = true;
                FileLocation = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("\\", "/") + "/../../..";
                MapImage.Source = new BitmapImage(new Uri(FileLocation + setting.MapLocation));
            }
        }

        private void AddNewMap()
        {
            OpenFileDialog map = new OpenFileDialog();

            map.Filter = "Resim Dosyası (*.png)|*.png|Resim Dosyası (*.jpg)|*.jpg"; // |Tüm dosyalar (*.*)|*.*
            map.Multiselect = false;
            map.RestoreDirectory = true;
            map.Title = "Harita Seçiniz";

            if (map.ShowDialog() == true)
            {
                MapImage.Source = new BitmapImage(new Uri(map.FileName));
            }
        }
    }
}
