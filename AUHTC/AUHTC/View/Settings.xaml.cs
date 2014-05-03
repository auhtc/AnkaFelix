using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AUHTC.View
{
    public partial class Settings : Window
    {
        AnkaFelix ParentAnka;

        public Settings(AnkaFelix parent)
        {
            InitializeComponent();
            ParentAnka = parent;

            LoadMapsToList();
            countTextBox.Text = AUHTC.Properties.Settings.Default.CountDownFormat;
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
            Properties.Settings.Default.MapName = MapList.SelectedValue.ToString();
            AUHTC.Properties.Settings.Default.CountDownFormat = countTextBox.Text;
            Properties.Settings.Default.Save();
            App.CurrentMapName = MapList.SelectedValue.ToString();
            App.AllConstants.Setting = App.ViewModel.ReadMapFromDB(App.CurrentMapName);
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

        private void NewMap_Click(object sender, RoutedEventArgs e)
        {
            AddNewMap();

            mapnameTextBox.Text = string.Empty;
            offsetTextbox1.Text = string.Empty;
            offsetTextbox2.Text = string.Empty;
            NewButton.IsEnabled = false;
            CancelButton.Visibility = Visibility.Visible; EditButton.Visibility = Visibility.Hidden;
            EditControls.IsEnabled = true;
        }

        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            Regex rgx_Offset = new Regex(@"(\d{2}\.\d{5}\,\d{2}\.\d{5})");

            if (string.IsNullOrEmpty(mapnameTextBox.Text))
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
                int Mapid = (mapidTextBlock.Text != "Null") ? Convert.ToInt32(mapidTextBlock.Text) : 1;
                byte[] byteImage = App.ViewModel.Image2Byte((BitmapImage)MapImage.Source);
                App.ViewModel.SaveMapToDB(Mapid, mapnameTextBox.Text, byteImage, offsetTextbox1.Text, offsetTextbox2.Text);

                mapnameTextBox.Text = string.Empty;
                offsetTextbox1.Text = string.Empty;
                offsetTextbox2.Text = string.Empty;

                LoadMapsToList();
                EditControls.IsEnabled = false;
                NewButton.IsEnabled = true;
                CancelButton.Visibility = Visibility.Hidden; 
                EditButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadMapsToList()
        {
            MapList.Items.Clear();
            var array = App.ViewModel.ReadAllMapFromDB();

            foreach (string item in array)
            {
                MapList.Items.Add(item);
            }

            MapList.SelectedValue = Properties.Settings.Default.MapName;
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
                var setting = App.ViewModel.ReadMapFromDB(MapList.SelectedItem.ToString());
                mapidTextBlock.Text = setting.Id.ToString();
                mapnameTextBox.Text = setting.MapName;
                offsetTextbox1.Text = setting.Offset1X + "," + setting.Offset1Y;
                offsetTextbox2.Text = setting.Offset2X + "," + setting.Offset2Y;
                MapImage.Source = App.ViewModel.Byte2Image(setting.MapImage);
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

        private void CancelMap_Click(object sender, RoutedEventArgs e)
        {
            mapnameTextBox.Text = string.Empty;
            offsetTextbox1.Text = string.Empty;
            offsetTextbox2.Text = string.Empty;

            LoadMapsToList();
            NewButton.IsEnabled = true;
            CancelButton.Visibility = Visibility.Hidden;
            EditButton.Visibility = Visibility.Visible;
            EditControls.IsEnabled = false;
        }

        private void EditMap_Click(object sender, RoutedEventArgs e)
        {
            NewButton.IsEnabled = false;
            CancelButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Hidden;
            EditControls.IsEnabled = true;
        }
    }
}
