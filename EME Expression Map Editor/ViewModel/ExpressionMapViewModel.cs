using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EME_Expression_Map_Editor.Command;
using EME_Expression_Map_Editor.Model;
using EME_Expression_Map_Editor.Model.XmlFileManagement;

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class ExpressionMapViewModel : ViewModelBase
    {
        ExpressionMap _map = new ExpressionMap(); 

        private string _name = string.Empty;
        public string Name 
        {
            get => _name;
            set => _name = value;
        }

        private ObservableCollection<SoundSlotViewModel> _soundSlots = new ObservableCollection<SoundSlotViewModel>();
        public ObservableCollection<SoundSlotViewModel> SoundSlots
        {
            get => _soundSlots; 
            set => _soundSlots = value; 
        }

        private SoundSlotViewModel? _selectedSlot; 
        public SoundSlotViewModel? SelectedSlot
        {
            get => _selectedSlot; 
            set => _selectedSlot = value;
        }

        private ObservableCollection<ArticulationViewModel> _articulations = new ObservableCollection<ArticulationViewModel>();
        public ObservableCollection<ArticulationViewModel> Articulations
        {
            get => _articulations;
            set => _articulations = value;
        }
        

        public ExpressionMapViewModel() 
        {

            #if DEBUG
                Console.WriteLine("Loading ExpressionMap VM in DEBUG mode: fetching sample data");
                GenerateTestData();
                ExtractViewModels();
            #endif


        }

        private void ExtractViewModels()
        {
            Name = _map.Name;

            _soundSlots.Clear();
            foreach (var slot in _map.SoundSlots)
            {
                SoundSlots.Add(new SoundSlotViewModel(slot));
            }

            _articulations.Clear(); 
            foreach (var art in _map.Articulations)
            {
                Articulations.Add(new ArticulationViewModel(art));
            }

        }

        private void GenerateTestData()
        {
            try
            {
                ExpressionMap xm = new ExpressionMap();

                // Old manual code - replaced with iteration and explicit null checks to suppress warnings: 
                //DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;

                DirectoryInfo? dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                for (int i = 0; i < 3; ++i)
                    if (dir != null)
                        dir = dir.Parent; 
                if (dir != null)
                {
                    XmlReader reader = ExpressionMapReader.CreateStandardXmlReader(dir.FullName + "\\SampleTestData\\MSB Horn - All Variations.expressionmap");
                    ExpressionMapReader.ReadExpressionMap(reader, xm);
                    _map = xm;
                }
                else
                    throw new Exception("Failed to access project directory with test data"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** Error when reading file:");
                Console.WriteLine(ex);
            }
        }
    }

}
