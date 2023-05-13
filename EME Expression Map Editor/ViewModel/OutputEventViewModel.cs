using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EME_Expression_Map_Editor.Model; 

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class OutputEventViewModel : ViewModelBase
    {
        private OutputEvent _event; 

        public OutputEventViewModel(OutputEvent oe)
        {
            _event = oe; 
        }
    }
}
