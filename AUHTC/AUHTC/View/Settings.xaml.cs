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

            AyarOku();
        }

        private void AyarOku()
        {
            //StreamReader ayaroku;
            //string yazi = "0";
            //ayaroku = File.OpenText(dosya);
            //do
            //{
            //    yazi = ayaroku.ReadLine();
            //    if (yazi == null) { break; }
            //    if (yazi.Substring(0, 6) == "MapLoc") { MapImage.Source = yazi.Substring(7, yazi.Length - 7); label1.Text = yazi.Substring(7, yazi.Length - 7); continue; }
            //    if (yazi.Substring(0, 7) == "MapName") { mapnameTextBox.Text = yazi.Substring(8, yazi.Length - 8); continue; }
            //    if (yazi.Substring(0, 7) == "OffSet1") { offsetTextbox1.Text = yazi.Substring(8, yazi.Length - 8); continue; }
            //    if (yazi.Substring(0, 7) == "OffSet2") { offsetTextbox2.Text = yazi.Substring(8, yazi.Length - 8); continue; }
            //} while (yazi != null);

            //ayaroku.Close();


            // Bu fonksiyon regex öğrenmeden önce yazdğım düz mantık :D
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

            //StreamWriter ayaryaz;
            //ayaryaz = File.CreateText(file);
            //ayaryaz.WriteLine("MapLoc=" + MapImage.Source.ToString());
            //ayaryaz.WriteLine("MapName=" + mapnameTextBox.Text);
            //ayaryaz.WriteLine("OffSet1=" + offsetTextbox1.Text); //İçinde sayi,sayi şeklinde veri olmalı regex koysak mı?
            //ayaryaz.WriteLine("OffSet2=" + offsetTextbox2.Text); //İçinde sayi,sayi şeklinde veri olmalı regex koysak mı?
            //ayaryaz.Close();

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

        private void DeleteMap_Click(object sender, RoutedEventArgs e)
        {
            // Database den şuan açık map silinecek.
            // NewMap_Click çalışacak
        }

        private void EditMap_Click(object sender, RoutedEventArgs e)
        {
            //mapnameTextBox.IsEnabled = true;
            //offsetTextbox1.IsEnabled = true;
            //offsetTextbox2.IsEnabled = true;

            //OpenFileDialog resimac = new OpenFileDialog();
            //resimac.Title = "Lütfen ayar dosyasını nereye kaydedeceğinizi seçin.";
            //resimac.Filter = "PNG Resim(*.png)|*.png|JPEG Resim(*.jpg)|*.jpg";
            //resimac.FilterIndex = 2;
            //resimac.ShowDialog();
            //if (resimac.FileName.ToString() != "")
            //{
            //    MapImage.Source.SetValue(resimac.FileName);
            //}
            //else
            //{
            //    MessageBox.Show("Bir dosya seçmediniz.");
            //}
        }

        private void NewMap_Click(object sender, RoutedEventArgs e)
        {
            //MapImage.Source.SetValue((DependencyProperty)null, null);
            //mapnameTextBox.Text = "";
            //offsetTextbox1.Text = "";
            //offsetTextbox2.Text = "";
            //Yeni harita oluşturucaz.. Map için png seçici bir dialog açıcaz.
            // EditMap_Click çalışacak
        }
    }
}
