using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ChartFinal
{
    class GraphDataModel : INotifyPropertyChanged
    {
        int _case;
        DateTime _time;

        public int Case
        {
            get
            {
                return _case;

            }

            set
            {
                _case = value;
                NotifyPropertyChanged("Case");
            }
        }

        public DateTime Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }



}
