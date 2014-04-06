using AUHTC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AUHTC.ViewModel
{
    public class MapViewModel : INotifyPropertyChanged
    {
        Regex regex = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");

        public event PropertyChangedEventHandler PropertyChanged;

        private MapModel mapModel;

        private int temp_x = 0;
        private int temp_y = 0;

        private ObservableCollection<MapModel> dataCollection = new ObservableCollection<MapModel>();
        public ObservableCollection<MapModel> DataCollection
        {
            get { return dataCollection; }
            set
            {
                if (value != dataCollection)
                {
                    dataCollection = value;
                    NotifyPropertyChanged("DataCollection");
                }
            }
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void func()
        {
            mapModel = new MapModel();
            mapModel.KoorX = temp_x++;
            mapModel.KoorY = temp_y++;
            DataCollection.Add(mapModel);
        }
    }
}
