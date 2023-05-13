using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EME_Expression_Map_Editor.Model; 

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class ArticulationViewModel : ViewModelBase
    {
        private Articulation _articulation; 

        public ArticulationViewModel(Articulation art) 
        {
            _articulation = art; 
        }
    }
}
