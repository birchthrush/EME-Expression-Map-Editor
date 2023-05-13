﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using EME_Expression_Map_Editor.Command;
using EME_Expression_Map_Editor.Model;
using EME_Expression_Map_Editor.Model.XmlFileManagement;

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

        #endregion

        #region Commands relating to Articulations

        public ICommand PropagateArticulationTypeCommand { get; private set; }
        private void PropagateArticulationDisplayType(int display_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected) 
                    item.DisplayType = display_type;


            /*
            if (SelectedArticulations == null)
                return;

            foreach (ArticulationPresenter art in SelectedArticulations)
                art.DisplayType = display_type;

            OnPropertyChanged("SoundSlots");
            */
        }

        public ICommand PropagateArticulationDisplayTypeCommand { get; private set; }
        private void PropagateArticulationType(int art_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected)
                    item.ArticulationType = art_type;
            /*
            if (SelectedArticulations == null)
                return;

            foreach (ArticulationPresenter art in SelectedArticulations)
                art.ArticulationType = art_type;

            OnPropertyChanged("SoundSlots");
            */
        }

        public ICommand TestCommand { get; private set; }
        private void Test(System.Collections.IList items)
        {
            if (items == null || items.Count == 0)
                return; 

            List<ArticulationViewModel> list = items.Cast<ArticulationViewModel>().ToList();
            list = GetSortedList<ArticulationViewModel>(list, Articulations);

            var target = list.First().DisplayType; 
            
            foreach (ArticulationViewModel art in list)
            {
                art.DisplayType = target;
                art.Description = "Test";
            }
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

            PropagateArticulationDisplayTypeCommand = new CustomCommand<int>(PropagateArticulationDisplayType);
            PropagateArticulationTypeCommand = new CustomCommand<int>(PropagateArticulationType);


            TestCommand = new CustomCommand<System.Collections.IList>(Test); 
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
