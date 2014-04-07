﻿using AUHTC.Model;
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
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace AUHTC.ViewModel
{
    public class MapViewModel : INotifyPropertyChanged 
    {
        Regex regex = new Regex(@"([A-Z]{9},[0-9][0-9][0-9])");

        public event PropertyChangedEventHandler PropertyChanged;

        //private MapModel mapModel;

        private Thickness margin;// = new MapModel();
        public Thickness Margin
        {
            get { return margin; }
            set
            {
                if (value != margin)
                {
                    margin = value;
                    NotifyPropertyChanged("Margin");
                }
            }
        }



        private int temp_x = 0;
        private int temp_y = 0;

        private MapModel dataCollection;// = new MapModel();
        public MapModel DataCollection
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
            margin.Left = temp_x;
            margin.Top = temp_y;
            dataCollection = new MapModel();
            dataCollection.KoorX = temp_x++;
            dataCollection.KoorY = temp_y++;
            //DataCollection.Add(mapModel);
        }
    }
}