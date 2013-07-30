using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Weave.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void Raise(params string[] p)
        {
            if (PropertyChanged != null)
            {
                foreach (var property in p)
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
