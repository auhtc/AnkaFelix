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
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        AnkaFelix Parent;
        public Map(AnkaFelix parent)
        {
            InitializeComponent();
            this.DataContext = App.MapModel;
            Parent = parent;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Parent.Show();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = "does it work";
            App.MapModel.func();
            TextBox1.Text = "it works";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            screen.DataContext = App.MapModel;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Close();
            Parent.Show();
        }
    }
}
