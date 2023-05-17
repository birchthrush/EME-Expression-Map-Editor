using EME_Expression_Map_Editor.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EME_Expression_Map_Editor.ViewModel
{


    internal  class NumberInputViewModel : ViewModelBase
    {
        private readonly int MAX_N = 256;
        private int _number = -1; 
        public string NumberAsString
        {
            get => _number >= 0 ? _number.ToString() : string.Empty;
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _number = Math.Clamp(n, 0, MAX_N);
                    OnPropertyChanged(nameof(NumberAsString));
                    OnPropertyChanged(nameof(Number));
                }
            }
        }
        public int Number
        {
            get => Math.Clamp(_number, 0, MAX_N); 
        }

        private WindowReturnCode _windowStatus;
        public WindowReturnCode WindowStatus
        {
            get => _windowStatus;
            set => _windowStatus = value;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        private void CloseWindow(WindowReturnCode rc)
        {
            WindowStatus = rc;
            Application.Current.MainWindow.Close();
        }

        public ICommand CloseWindowCommand { get; private set; }

        public NumberInputViewModel()
        {
            CloseWindowCommand = new CustomCommand<WindowReturnCode>(CloseWindow); 
        }
    }
}
