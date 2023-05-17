using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EME_Expression_Map_Editor.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, ICloneable
    {


        public event PropertyChangedEventHandler? PropertyChanged = null; 
        protected void OnPropertyChanged (string property_name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));

        // Trigger PropertyChanged for *all* properties
        public void Refresh()
            => this.OnPropertyChanged(string.Empty);

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public abstract object Clone(); 

        public ViewModelBase()
        {

        }
    }
}
