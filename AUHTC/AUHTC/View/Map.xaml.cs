using System;
using System.Windows;
using System.IO;
using System.Threading;

namespace AUHTC.View
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        Thread thread;
        AnkaFelix Parent;
        public Map(AnkaFelix parent)
        {
            InitializeComponent();
            this.DataContext = App.MapModel;
            Parent = parent;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.MapModel.ReadFile = File.OpenText("a.txt");
            thread = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if(!App.MapModel.ReadData())
                        thread.Abort();
                    Thread.Sleep(1);
                }
            }));
            thread.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            thread.Abort();
            App.MapModel.ReadFile = null;
            this.Close();
            Parent.Show();
        }
    }
}
