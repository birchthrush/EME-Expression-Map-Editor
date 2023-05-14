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

        private static ArticulationViewModel _blank = new ArticulationViewModel(Articulation.Blank); 
        public static ArticulationViewModel Blank
        {
            get => _blank; 
        }

        public bool IsBlank(ArticulationViewModel art)
            => Articulation.IsBlank(art._articulation); 

        public string Description
        {
            get => _articulation.Description;
            set
            {
                _articulation.Description = value;
                OnPropertyChanged(nameof(Description)); 
            }
        }

        public int Group
        {
            get => _articulation.Group;
            set 
            { 
                _articulation.Group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        // Options listed on UI in Group selection ComboBox: 
        // Key (0-3) represents internal representation of Group and is passed on to Group property as the actual value
        // Value (1-4) is the human-readable representation displayed on UI
        private static Dictionary<int, int> _groupOptions = new Dictionary<int, int>()
        {
            { 0, 1 },
            { 1, 2 },
            { 2, 3 },
            { 3, 4 }
        }; 

        public static Dictionary<int, int> GroupOptions
        {
            get => _groupOptions; 
        }

        // Articulation name, either symbol or text
        public string SymbolOrText
        {
            get => _articulation.DisplayType == Articulation.Display.Symbol ? _articulation.Symbol.ToString() : _articulation.Text;
            set
            {
                if (_articulation.DisplayType == Articulation.Display.Symbol)
                {
                    if (Int32.TryParse(value, out int n))
                        _articulation.Symbol = n;
                }
                else
                {
                    _articulation.Text = value;

                    if (Common.KeyModifiers.AlternativeInputKeyActive())
                        Description = value;
                    else
                        Description = ExpressionMapCommon.TextToDescription(value);
                }

                OnPropertyChanged(nameof(SymbolOrText));

                //OnPropertyChanged("SymbolOrText");
                //ExpressionMapPresenter.Instance.ArticulationNameChanged(_articulation);
            }
        }

        public int DisplayType
        {
            get => _articulation.DisplayType == Articulation.Display.Symbol ? 0 : 1;
            set
            {
                _articulation.DisplayType = ((value == 0) ? Articulation.Display.Symbol : Articulation.Display.Text);
                OnPropertyChanged(nameof(SymbolOrText));
                OnPropertyChanged(nameof(DisplayType));
            }
        }

        private static Dictionary<int, string> _displayTypeOptions = new Dictionary<int, string>()
        {
            {(int)Articulation.Display.Symbol, "Symbol" },
            {(int)Articulation.Display.Text, "Text" }
        };
        public static Dictionary<int, string> DisplayTypeOptions
        {
            get => _displayTypeOptions; 
        }

        public int ArticulationType
        {
            get => _articulation.ArticulationType == Articulation.ArtType.Attribute ? 0 : 1;
            set
            {
                if (value == (int)Articulation.ArtType.Attribute)
                    _articulation.ArticulationType = Articulation.ArtType.Attribute;
                else
                    _articulation.ArticulationType = Articulation.ArtType.Direction; 

                OnPropertyChanged(nameof(ArticulationType));
            }
        }

        private static Dictionary<int, string> _articulationTypeOptions = new Dictionary<int, string>()
        {
            { (int)Articulation.ArtType.Attribute, "Attribute" },
            { (int)Articulation.ArtType.Direction, "Direction" }
        }; 
        public static Dictionary<int, string> ArticulationTypeOptions
        {
            get => _articulationTypeOptions;
        }

        public static void AssignArticulationToSlot(ArticulationViewModel art, int n, SoundSlot slot)
        {
            slot.AssignArticulation(art._articulation, n); 
        }

        public override string ToString()
            => SymbolOrText.ToString();

        public ArticulationViewModel(Articulation art) 
        {
            _articulation = art; 
        }
    }
}
