using System;
using System.ComponentModel;

namespace Typhoon.WPClient
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _lineOne;
        private string _lineTwo;
        private string _lineThree;
        
        public string LineOne
        {
            get
            {
                return _lineOne;
            }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }
        public string LineTwo
        {
            get
            {
                return _lineTwo;
            }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }
        public string LineThree
        {
            get
            {
                return _lineThree;
            }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}