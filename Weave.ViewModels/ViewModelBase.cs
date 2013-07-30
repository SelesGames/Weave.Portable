using System.ComponentModel;

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
