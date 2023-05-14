﻿using System;
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

        private ObservableCollection<ArticulationViewModel> _articulations = new ObservableCollection<ArticulationViewModel>();
        public ObservableCollection<ArticulationViewModel> Articulations
        {
            get => _articulations;
            set => _articulations = value;
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

        /*
        public IList<Articulation> Group1Options { get => ArticulationGroupOptions(0); }
        public IList<Articulation> Group2Options { get => ArticulationGroupOptions(1); }
        public IList<Articulation> Group3Options { get => ArticulationGroupOptions(2); }
        public IList<Articulation> Group4Options { get => ArticulationGroupOptions(3); }

        public IList<Articulation> ArticulationGroupOptions(int group)
        {
            IList<Articulation> arts = _map.ArticulationGroup(group);
            arts.Insert(0, Articulation.Blank);
            return arts;
        }
        */

        #endregion

        #region Commands relating to SoundSlots

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

        #endregion

        #region Commands relating to Articulations

        public ICommand ChangeArticulationTypeCommand { get; private set; }
        private void ChangeArticulationDisplayType(int display_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected) 
                    item.DisplayType = display_type;
        }

        public ICommand ChangeArticulationDisplayTypeCommand { get; private set; }
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
            OnPropertyChanged(nameof(Group1Options));
            OnPropertyChanged(nameof(Group2Options));
            OnPropertyChanged(nameof(Group3Options));
            OnPropertyChanged(nameof(Group4Options));
        }

        private List<T> GetSortedList<T>(List<T> src, IList<T> ordering)
        {
            src.Sort((x, y) => ordering.IndexOf(x).CompareTo(ordering.IndexOf(y)));
            return src; 
        }


        #endregion


        public ExpressionMapViewModel() 
        {

            #if DEBUG
                Console.WriteLine("Loading ExpressionMap VM in DEBUG mode: fetching sample data");
                GenerateTestData();
                ExtractViewModels();
            #endif

            // SoundSlot Grid Commands: 
            ChangeColorCommand = new CustomCommand<int>(ChangeColor);

            // Articulation Grid Commands: 
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
