using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EME_Expression_Map_Editor.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = null; 
        protected void OnPropertyChanged (string property_name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));

        // Trigger PropertyChanged for *all* properties
        public void Refresh()
            => this.OnPropertyChanged(string.Empty); 

        public ViewModelBase()
        {

        }
    }
}
