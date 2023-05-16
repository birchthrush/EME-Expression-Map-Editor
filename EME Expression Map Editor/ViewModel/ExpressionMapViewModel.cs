using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using EME_Expression_Map_Editor.Command;
using EME_Expression_Map_Editor.Model;
using EME_Expression_Map_Editor.Model.XmlFileManagement;
using GongSolutions.Wpf.DragDrop;

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class ExpressionMapViewModel : ViewModelBase
    {
        ExpressionMap _map = new ExpressionMap();

        #region ExpressionMap Properties

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
            set
            {
                _selectedSlot = value;
                OnPropertyChanged(nameof(SelectedSlot));
            }
        }

        private int _selectedSlotIndex;
        public int SelectedSlotIndex
        {
            get => _selectedSlotIndex;
            set
            {
                _selectedSlotIndex = value;
                OnPropertyChanged(nameof(SelectedSlotIndex));
            }
        }

        private ObservableCollection<ArticulationViewModel> _articulations = new ObservableCollection<ArticulationViewModel>();
        public ObservableCollection<ArticulationViewModel> Articulations
        {
            get => _articulations;
            set => _articulations = value;
        }

        private int _selectedArticulationIndex;
        public int SelectedArticulationIndex
        {
            get => _selectedArticulationIndex;
            set
            {
                _selectedArticulationIndex = value;
                OnPropertyChanged(nameof(SelectedArticulationIndex));
            }
        }


        public IList<ArticulationViewModel> Group1Options { get => ArticulationGroupOptions(0); }
        public IList<ArticulationViewModel> Group2Options { get => ArticulationGroupOptions(1); }
        public IList<ArticulationViewModel> Group3Options { get => ArticulationGroupOptions(2); }
        public IList<ArticulationViewModel> Group4Options { get => ArticulationGroupOptions(3); }
        public List<ArticulationViewModel> ArticulationGroupOptions(int group)
        {
            List<ArticulationViewModel> arts = new List<ArticulationViewModel>();
            arts.Add(ArticulationViewModel.Blank);
            foreach (var item in Articulations)
                if (item.Group == group)
                    arts.Add(item);
            return arts;
        }

        #endregion

        #region Commands relating to SoundSlots

        public ICommand AddSoundSlotCommand { get; private set; }
        public void AddSoundSlot(int idx)
        {
            if (SoundSlots.Count == 0)
            {
                // List is empty: add new slot with default parameters
                SoundSlots.Add(new SoundSlotViewModel(new SoundSlot()));
                SelectedSlotIndex = 0;
            }
            else
            {
                if (idx < 0 || idx >= SoundSlots.Count - 1)
                    idx = SoundSlots.Count - 1;

                var slot_vm = (SoundSlotViewModel)SoundSlots[idx].GetPrototype();
                slot_vm.Color = SoundSlot.GetNextColor(SoundSlots[idx].Color);
                SoundSlots.Insert(idx + 1, slot_vm);
                SelectedSlotIndex = idx + 1;
            }

            OnPropertyChanged(nameof(SoundSlots));
        }

        public ICommand RemoveSoundSlotCommand { get; private set; }
        public void RemoveSoundSlot()
        {
            foreach (var slot in SoundSlots.ToList().Where(x => x.IsSelected))
                SoundSlots.Remove(slot); 
        }


        public ICommand ChangeColorCommand { get; private set; }
        private void ChangeColor(int col)
        {
            foreach (var item in SoundSlots)
            {
                if (item.IsSelected)
                {
                    item.Color = col; 
                    if (Common.KeyModifiers.CascadeKeyActive())
                        col = SoundSlot.GetNextColor(col);
                }
            }
        }

        public ICommand AssignArticulationCommand {  get; private set; }
        private void AssignArticulation(ArticulationViewModel art)
        {
            if (Common.KeyModifiers.CascadeKeyActive() && !art.Equals(ArticulationViewModel.Blank))
            {
                var art_list = ArticulationGroupOptions(art.Group);
                int idx = art_list.IndexOf(art);
                foreach (var slot in SoundSlots.Where(x => x.IsSelected))
                {
                    slot.SetArticulation(art_list[idx]);

                    // Increment & wrap around index: skip 0, which is always the blank articulation
                    if (idx == 0 || idx == art_list.Count - 1)
                        idx = 1;
                    else
                        ++idx; 
                }
            }
            else
            {
                foreach (var slot in SoundSlots.Where(x => x.IsSelected))
                    slot.SetArticulation(art); 
            }
        }

        #endregion

        #region Commands relating to Articulations


        public ICommand AddArticulationCommand { get; private set; }
        private void AddArticulation(int idx)
        {

            if (Articulations.Count == 0)
            {
                // Articulation list empty: add new with default parameters
                Articulations.Add(new ArticulationViewModel(new Articulation()));
                SelectedArticulationIndex = 0; 
            }
            else
            {
                if (idx < 0 || idx >= Articulations.Count)
                    idx = Articulations.Count - 1;

                // Add new Articulation with similar properties as the preceding one: 
                ArticulationViewModel art_vm = (ArticulationViewModel)Articulations[idx].GetPrototype();
                art_vm.Group = Articulations[idx].Group;

                Articulations.Insert(idx + 1, art_vm); 
                SelectedArticulationIndex = idx + 1; 
            }


            OnPropertyChanged(nameof(Articulations)); 
            RefreshArticulationGroupOptions();
        }

        public ICommand RemoveArticulationCommand { get; private set; }
        private void RemoveArticulation()
        {
            foreach (var art in Articulations.ToList().Where(art => art.IsSelected))
                Articulations.Remove(art); 
            RefreshArticulationGroupOptions();
        }

        public ICommand ChangeArticulationDisplayTypeCommand { get; private set; }
        private void ChangeArticulationDisplayType(int display_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected) 
                    item.DisplayType = display_type;
        }

        public ICommand ChangeArticulationTypeCommand { get; private set; }
        private void ChangeArticulationType(int art_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected)
                    item.ArticulationType = art_type;
        }

        public ICommand ChangeGroupCommand { get; private set; }
        private void ChangeGroup(int group)
        {
            Dictionary<SoundSlotViewModel, ArticulationViewModel> mapper = new Dictionary<SoundSlotViewModel, ArticulationViewModel>(); 

            foreach (var art in Articulations)
            {
                if (art.IsSelected)
                {
                    art.Group = group;
                    foreach (var slot in SoundSlots)
                    {
                        if (slot.ContainsArticulation(art))
                        {
                            // SoundSlots affected by the group change will have the pre-change Art set to Blank
                            // Post-change Arts will be applied later after sorting and updating Articulation list
                            slot.UnassignArticulation(art); 
                            mapper.Add(slot, art);
                        }
                    }
                }
            }

            SortArticulationsByGroup();

            // Re-assign: 
            foreach (var pair in mapper)
                pair.Key.SetArticulation(pair.Value);   
        }

        public void ArticulationNameChangedHandler(ArticulationViewModel art)
        {
            foreach (var slot in SoundSlots)
            {
                // Assigns the same articulation again: doesn't actually change the Slot, but
                // forces the UI to refresh its displayed name. 
                if (slot.ContainsArticulation(art))
                    slot.SetArticulation(art); 
            }
        }

        public void RefreshArticulationGroupOptions()
        {
            OnPropertyChanged(nameof(Group1Options));
            OnPropertyChanged(nameof(Group2Options));
            OnPropertyChanged(nameof(Group3Options));
            OnPropertyChanged(nameof(Group4Options));
        }

        private void SortArticulationsByGroup()
        {
            ObservableCollection<ArticulationViewModel> sorted = new ObservableCollection<ArticulationViewModel>();
            for (int i = Articulation.MinGroup; i <= Articulation.MaxGroup; ++i)
            {
                foreach (var item in Articulations)
                    if (item.Group == i)
                        sorted.Add(item);
            }

            Articulations = sorted;
            OnPropertyChanged(nameof(Articulations));
            RefreshArticulationGroupOptions(); 
        }

        private List<T> GetSortedList<T>(List<T> src, IList<T> ordering)
        {
            src.Sort((x, y) => ordering.IndexOf(x).CompareTo(ordering.IndexOf(y)));
            return src; 
        }


        #endregion

        public override ViewModelBase GetPrototype()
        {
            throw new NotImplementedException();
        }

        private static ExpressionMapViewModel _instance = new ExpressionMapViewModel();
        public static ExpressionMapViewModel Instance { get { return _instance; } }

        private ExpressionMapViewModel() 
        {

            #if DEBUG
                Console.WriteLine("Loading ExpressionMap VM in DEBUG mode: fetching sample data");
                GenerateTestData();
                ExtractViewModels();
            #endif

            // SoundSlot Grid Commands: 
            AddSoundSlotCommand = new CustomCommand<int>(AddSoundSlot);
            RemoveSoundSlotCommand = new NoParameterCommand(RemoveSoundSlot);
            ChangeColorCommand = new CustomCommand<int>(ChangeColor);
            AssignArticulationCommand = new CustomCommand<ArticulationViewModel>(AssignArticulation);

            // Articulation Grid Commands: 
            AddArticulationCommand = new CustomCommand<int>(AddArticulation);
            RemoveArticulationCommand = new NoParameterCommand(RemoveArticulation);
            ChangeArticulationDisplayTypeCommand = new CustomCommand<int>(ChangeArticulationDisplayType);
            ChangeArticulationTypeCommand = new CustomCommand<int>(ChangeArticulationType);
            ChangeGroupCommand = new CustomCommand<int>(ChangeGroup);

            // Drop handlers: 
            ArticulationDropHandler = new CustomDropHandler(DefaultDragOver, DropArticulations); 
        }

        #region Drag-And-Drop Handlers
        private DefaultDropHandler _defaultDropHandler = new DefaultDropHandler();
        private void DefaultDragOver(IDropInfo dropInfo)
        {
            _defaultDropHandler.DragOver(dropInfo);
        }

        public IDropTarget ArticulationDropHandler { get; private set; }
        private void DropArticulations(IDropInfo dropInfo)
        {
            _defaultDropHandler.Drop(dropInfo); 
            SortArticulationsByGroup(); 
        }

        #endregion


        private void ExtractViewModels()
        {
            Name = _map.Name;

            Dictionary<Articulation, ArticulationViewModel> art_map = new Dictionary<Articulation, ArticulationViewModel>();
            art_map.Add(Articulation.Blank, ArticulationViewModel.Blank); 

            _articulations.Clear(); 
            foreach (var art in _map.Articulations)
            {
                ArticulationViewModel art_vm = new ArticulationViewModel(art);
                Articulations.Add(art_vm); 
                art_map.Add(art, art_vm);        
            }

            _soundSlots.Clear();
            foreach (var slot in _map.SoundSlots)
            {
                SoundSlotViewModel slot_vm = new SoundSlotViewModel(slot);

                slot_vm.Art1 = art_map[slot.Articulations[0]];
                slot_vm.Art2 = art_map[slot.Articulations[1]];
                slot_vm.Art3 = art_map[slot.Articulations[2]];
                slot_vm.Art4 = art_map[slot.Articulations[3]];

                SoundSlots.Add(slot_vm); 
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
