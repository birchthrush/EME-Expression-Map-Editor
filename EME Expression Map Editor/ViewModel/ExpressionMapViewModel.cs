using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using EME_Expression_Map_Editor.Command;
using EME_Expression_Map_Editor.Model;
using EME_Expression_Map_Editor.View;
using EME_ExpressionMapEditor;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class ExpressionMapViewModel : ViewModelBase
    {
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

        public SoundSlotViewModel? FirstSelectedSlot
        {
            get
            {
                if (SoundSlots.Count == 0) 
                    return null;

                var selection = SoundSlots.Where(x => x.IsSelected);
                if (selection.Any())
                    return selection.First();
                else
                    return null; 
            }
        }

        public void SoundSlotSelectionChangedHandler()
            => OnPropertyChanged(nameof(FirstSelectedSlot)); 



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

        #region Commands and functions relating to popup windows

        private void CreatePopupWindow(PopupType type, Action<Window> pre_function, Action<Window> post_function)
        {
            Window prev_main = Application.Current.MainWindow;
            Window? popup = null;
            if (type == PopupType.NUMBER_INPUT)
                popup = new NumberInputWindow();
            else if (type == PopupType.OUTPUT_EVENT_REPLACEMENT)
                popup = new BatchProcessingWindow(); 
            else
                throw new Exception("Attempting to create undefined window type");

            // Redundant null check to suppress warnings
            if (popup == null)
                return; 

            popup.Owner = prev_main;
            popup.WindowStartupLocation = WindowStartupLocation.CenterOwner; 
            Application.Current.MainWindow = popup;

            // Perform initializer functions
            pre_function(popup);

            popup.ShowDialog(); 

            // Teardown function
            post_function(popup);

            Application.Current.MainWindow = prev_main; 
        }

        public ICommand AddSlotsPopupCommand { get; private set; }
        private void AddSlotsPopup_Post(Window win)
        {
            var data = win.DataContext as NumberInputViewModel; 
            if (data != null && data.WindowStatus == WindowReturnCode.OK)
            {
                for (int i = 0; i < data.Number; ++i)
                    SelectedSlotIndex = Common.AddItem(SoundSlots, SelectedSlotIndex, AddSoundSlotPost);
            }
        }

        public ICommand AddArticulationsPopupCommand { get; private set; }
        private void AddArticulationsPopup_Post(Window win)
        {
            var data = win.DataContext as NumberInputViewModel;
            if (data != null && data.WindowStatus == WindowReturnCode.OK)
            {
                for (int i = 0; i < data.Number; ++i)
                    SelectedArticulationIndex = Common.AddItem(Articulations, SelectedArticulationIndex, AddArticulationPost); 
            }
        }

        public ICommand BatchProcessingPopupCommand { get; private set; }
        private void BatchProcessingPopup_Pre(Window win)
        {
            var data = win.DataContext as BatchProcessingViewModel;
            if (data != null && data.WindowStatus == WindowReturnCode.OK)
                data.Initialize(SoundSlots.Where(x => x.IsSelected).ToList()); 
        }
        private void BatchProcessingPopup_Post(Window win)
        {
            var data = win.DataContext as BatchProcessingViewModel;
            if (data != null && data.WindowStatus != WindowReturnCode.CANCEL)
            {
                foreach (var src_oe in data.OutputEvents.Where(x => x.IsSelected).ToList())
                    foreach (var slot in SoundSlots.Where(x => x.IsSelected).ToList()) 
                        foreach (var oe in slot.OutputEvents.ToList())
                            if (oe.EventType == src_oe.EventType && oe.Data1 == src_oe.Data1 && oe.Data2 == src_oe.Data2)
                            {
                                if (data.WindowStatus == WindowReturnCode.DELETE)
                                    slot.OutputEvents.Remove(oe);
                                else if (data.WindowStatus == WindowReturnCode.OK && data.ReplacementEvent != null)
                                {
                                    oe.EventType = data.ReplacementEvent.EventType;
                                    oe.Data1 = data.ReplacementEvent.Data1;
                                    oe.Data2 = data.ReplacementEvent.Data2;
                                }
                            }
            }
        }

        #endregion

        #region Commands relating to SoundSlots

        public ICommand AddSoundSlotCommand { get; private set; }
        public ICommand RemoveSoundSlotCommand { get; private set; }
        public ICommand DuplicateSoundSlotCommand { get; private set; }
        public void DuplicateSoundlot()
        {
            var selection = SoundSlots.Where(x => x.IsSelected).ToList();
            if (selection.Any())
            {
                int idx = SoundSlots.IndexOf(selection.Last()) + 1;
                foreach (var slot in selection)
                    SoundSlots.Insert(idx++, (SoundSlotViewModel)slot.Clone()); 
            }
        }


        public ICommand SetColorCommand { get; private set; }
        private void SetColor(int col)
        {
            foreach (var item in SoundSlots.Where(x => x.IsSelected).ToList())
            {
                item.Color = col;
                if (Common.KeyModifiers.CascadeKeyActive())
                    col = SoundSlot.GetNextColor(col);
            }
        }

        private void SetArticulation(ArticulationViewModel art, int group)
        {
            if (Common.KeyModifiers.CascadeKeyActive() && !art.Equals(ArticulationViewModel.Blank))
            {
                var art_list = ArticulationGroupOptions(art.Group);
                int idx = art_list.IndexOf(art);
                foreach (var slot in SoundSlots.Where(x => x.IsSelected))
                {
                    slot.SetArticulation(art_list[idx], group);

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
                    slot.SetArticulation(art, group); 
            }
        }

        public ICommand SetArticulation1Command { get; private set; }
        public ICommand SetArticulation2Command { get; private set; }
        public ICommand SetArticulation3Command { get; private set; }
        public ICommand SetArticulation4Command { get; private set; }

        public ICommand SetChannelCommand { get; private set; }
        private void SetChannel()
        {
            if (SoundSlots.Count == 0 || FirstSelectedSlot == null) 
                return;

            int c_idx = SoundSlotViewModel.ChannelOptions.IndexOf(FirstSelectedSlot.Channel); 

            foreach (var slot in SoundSlots.Where(x => x.IsSelected))
            {
                slot.Channel = SoundSlotViewModel.ChannelOptions[c_idx];

                if (Common.KeyModifiers.CascadeKeyActive() && c_idx > 0)
                {
                    ++c_idx;
                    if (c_idx >= SoundSlotViewModel.ChannelOptions.Count)
                        c_idx = 1; 
                }
            }
        }

        public ICommand PropagateOutputMappingCommand { get; private set; }
        public void PropagateOutputMapping()
        {
            var source = FirstSelectedSlot; 
            if (source == null) 
                return; 

            foreach (var slot in SoundSlots.Where(x => x.IsSelected))
            {
                if (!slot.Equals(source))
                {
                    slot.OutputEvents.Clear();
                    foreach (var e in source.OutputEvents)
                        slot.OutputEvents.Add((OutputEventViewModel)e.Clone());               
                }
            }
        }

        public ICommand ChangeRemoteKeysDisplayTypeCommand { get; private set; }
        private void ChangeRemoteKeysDisplayType()
        {
            SoundSlotViewModel.DisplayRemoteKeyAsNoteValue = !SoundSlotViewModel.DisplayRemoteKeyAsNoteValue;
            foreach (var slot in SoundSlots)
                slot.RemoteKey = slot.RemoteKey; 
        }

        public ICommand IncrementRemoteKeysCommand { get; private set; }
        private void IncrementRemoteKeys()
        {
            if (FirstSelectedSlot != null)
            {
                int n = MidiNote.TryParse(FirstSelectedSlot.RemoteKey);
                foreach (var slot in SoundSlots.Where(x => x.IsSelected))
                    if (!slot.Equals(FirstSelectedSlot))
                        slot.RemoteKey = (++n).ToString(); 
            }
        }

        private void AddSoundSlotPost(SoundSlotViewModel src, SoundSlotViewModel dest)
        {
            dest.Color = (src.Color + 1) % 15;
        }

        #endregion

        #region Commands relating to Articulations

        public ICommand AddArticulationCommand { get; private set; }
        public ICommand RemoveArticulationCommand { get; private set; }
        public ICommand RemoveUnusedArticulationsCommand { get; private set; }

        private bool ArticulationIsUsed(ArticulationViewModel art_vm)
        {
            foreach (var slot in SoundSlots)
                if (slot.ContainsArticulation(art_vm))
                    return true;
            return false;
        }

        private bool ArticulationIsUnused(ArticulationViewModel art_vm)
            => !ArticulationIsUsed(art_vm);

        public ICommand SetArticulationDisplayTypeCommand { get; private set; }
        private void SetArticulationDisplayType(int display_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected) 
                    item.DisplayType = display_type;
        }

        public ICommand SetArticulationTypeCommand { get; private set; }
        private void SetArticulationType(int art_type)
        {
            foreach (var item in Articulations)
                if (item.IsSelected)
                    item.ArticulationType = art_type;
        }

        public ICommand SetGroupCommand { get; private set; }
        private void SetGroup(int group)
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
                            slot.RemoveArticulation(art.Group); 
                            mapper.Add(slot, art);
                        }
                    }
                }
            }

            SortArticulationsByGroup();

            // Re-assign: 
            foreach (var pair in mapper)
                pair.Key.SetArticulation(pair.Value, pair.Value.Group);   
        }

        public void ArticulationNameChangedHandler(ArticulationViewModel art)
        {
            foreach (var slot in SoundSlots)
            {
                // Assigns the same articulation again: doesn't actually change the Slot, but
                // forces the UI to refresh its displayed name. 
                if (slot.ContainsArticulation(art))
                {
                    slot.RemoveArticulation(art.Group); 
                    slot.SetArticulation(art, art.Group); 
                }
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

        private void AddArticulationPost(ArticulationViewModel src, ArticulationViewModel dest)
        {
            dest.ArticulationType = src.ArticulationType;
            dest.DisplayType = src.DisplayType;
            dest.Group = src.Group;

            RefreshArticulationGroupOptions();
        }

        private List<T> GetSortedList<T>(List<T> src, IList<T> ordering)
        {
            src.Sort((x, y) => ordering.IndexOf(x).CompareTo(ordering.IndexOf(y)));
            return src; 
        }

        #endregion

        #region Commands relating to OutputEvents
        
        public ICommand IncrementData1NthOutputEventCommand { get; private set; }
        public ICommand IncrementData2NthOutputEventCommand { get; private set; }
        private void IncrementNthOutputEvent(int n, bool inc_data1, bool inc_data2)
        {
            int times = 1; 
            foreach (var slot in SoundSlots.Where(x => x.IsSelected))
            {
                if (!slot.Equals(FirstSelectedSlot) && n < slot.OutputEvents.Count)
                {
                    for (int i = 0; i < times; ++i)
                        slot.OutputEvents[n].Increment(inc_data1, inc_data2);
                    ++times; 
                }
            }
        }

        public ICommand CopyOutputEventCommand { get; private set; }
        public ICommand CopyOutputEventIncrementData1Command { get; private set; }
        public ICommand CopyOutputEventIncrementData2Command { get; private set; }
        private void CopyOutputEvent(int idx, bool inc_data1, bool inc_data2)
        {
            var src = FirstSelectedSlot;
            if (src == null || idx >= src.OutputEvents.Count)
                return;

            var prototype = (OutputEventViewModel) src.OutputEvents[idx].Clone();         
            
            foreach (var slot in SoundSlots.Where(x => x.IsSelected))
            {
                if (!slot.Equals(src))
                {
                    prototype.Increment(inc_data1, inc_data2); 
                    slot.OutputEvents.Add((OutputEventViewModel)prototype.Clone());
                }
            }
        }

        #endregion


        public override object Clone()
        {
            throw new NotImplementedException();
        }


        private static ExpressionMapViewModel _instance = new ExpressionMapViewModel();
        public static ExpressionMapViewModel Instance { get { return _instance; } }


        private ExpressionMapViewModel()
        {

#if DEBUG
            Console.WriteLine("Loading ExpressionMap VM in DEBUG mode: fetching sample data");
            ExpressionMap? map = GenerateTestData();
            if (map != null)
                ExtractViewModels(map);
#endif

            
            ExpressionMap unpacked = CreateExpressionMapFromViewModels();
            Reset();
            ExtractViewModels(unpacked); 

            // Popup Window Commands: 
            AddSlotsPopupCommand = new NoParameterCommand(() => { CreatePopupWindow(PopupType.NUMBER_INPUT, Common.DoNothing, AddSlotsPopup_Post); });
            AddArticulationsPopupCommand = new NoParameterCommand(() => { CreatePopupWindow(PopupType.NUMBER_INPUT, Common.DoNothing, AddArticulationsPopup_Post); });
            BatchProcessingPopupCommand = new NoParameterCommand(() => { CreatePopupWindow(PopupType.OUTPUT_EVENT_REPLACEMENT, BatchProcessingPopup_Pre, BatchProcessingPopup_Post); });

            // SoundSlot Grid Commands: 
            AddSoundSlotCommand = new CustomCommand<int>((n) => { SelectedSlotIndex = Common.AddItem(SoundSlots, n, AddSoundSlotPost); });
            RemoveSoundSlotCommand = new NoParameterCommand(() => { SelectedSlotIndex = Common.RemoveItem(SoundSlots, SelectedSlotIndex, (n) => n.IsSelected, () => { OnPropertyChanged(nameof(SoundSlots)); }); });
            DuplicateSoundSlotCommand = new NoParameterCommand(DuplicateSoundlot);
            SetColorCommand = new CustomCommand<int>(SetColor);
            SetArticulation1Command = new CustomCommand<ArticulationViewModel>((art_vm) => SetArticulation(art_vm, 0));
            SetArticulation2Command = new CustomCommand<ArticulationViewModel>((art_vm) => SetArticulation(art_vm, 1));
            SetArticulation3Command = new CustomCommand<ArticulationViewModel>((art_vm) => SetArticulation(art_vm, 2));
            SetArticulation4Command = new CustomCommand<ArticulationViewModel>((art_vm) => SetArticulation(art_vm, 3));
            SetChannelCommand = new NoParameterCommand(SetChannel);
            PropagateOutputMappingCommand = new NoParameterCommand(PropagateOutputMapping);
            ChangeRemoteKeysDisplayTypeCommand = new NoParameterCommand(ChangeRemoteKeysDisplayType);
            IncrementRemoteKeysCommand = new NoParameterCommand(IncrementRemoteKeys);

            // Articulation Grid Commands: 
            AddArticulationCommand = new CustomCommand<int>((n) => { SelectedArticulationIndex = Common.AddItem(Articulations, n, AddArticulationPost); }); 
            RemoveArticulationCommand = new NoParameterCommand(() => { SelectedArticulationIndex = Common.RemoveItem(Articulations, SelectedArticulationIndex, (n) => n.IsSelected, RefreshArticulationGroupOptions); });
            RemoveUnusedArticulationsCommand = new NoParameterCommand(() => { SelectedArticulationIndex = Common.RemoveItem(Articulations, SelectedArticulationIndex, ArticulationIsUnused, RefreshArticulationGroupOptions); });
            SetArticulationDisplayTypeCommand = new CustomCommand<int>(SetArticulationDisplayType);
            SetArticulationTypeCommand = new CustomCommand<int>(SetArticulationType);
            SetGroupCommand = new CustomCommand<int>(SetGroup);

            // OutputEvent Grid Commands
            IncrementData1NthOutputEventCommand = new CustomCommand<int>((n) => { IncrementNthOutputEvent(n, true, false); });
            IncrementData2NthOutputEventCommand = new CustomCommand<int>((n) => { IncrementNthOutputEvent(n, false, true); });
            CopyOutputEventCommand = new CustomCommand<int>((n) => { CopyOutputEvent(n, false, false); });
            CopyOutputEventIncrementData1Command = new CustomCommand<int>((n) => { CopyOutputEvent(n, true, false); });
            CopyOutputEventIncrementData2Command = new CustomCommand<int>((n) => { CopyOutputEvent(n, false, true); });

            // Drop handlers: 
            ArticulationDropHandler = new CustomDropHandler(DefaultDragOver, DropArticulations);

            // File I/O: 
            LoadFileCommand = new NoParameterCommand(LoadFile);
            SaveFileCommand = new NoParameterCommand(SaveFile); 
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

        #region Commands and functions related to File I/O and translation to/from Model Layer

        public ICommand LoadFileCommand { get; private set; }
        public void LoadFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = XmlConstants.FileExtension;
            ofd.Filter = "ExpressionMap Files|*" + XmlConstants.FileExtension;

            Nullable<bool> result = ofd.ShowDialog(); 

            if (result == true)
            {
                using (XmlReader reader = ExpressionMapReader.CreateStandardXmlReader(ofd.FileName))
                {
                    try
                    {
                        ExpressionMap map = new ExpressionMap();
                        ExpressionMapReader.ReadExpressionMap(reader, map);
                        Reset(); 
                        ExtractViewModels(map); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error occurred when reading " + ofd.FileName + ":\n" + ex.Message);
                    }
                    finally
                    {
                        Refresh(); 
                    }
                }
            }
        }

        public ICommand SaveFileCommand { get; private set; }
        public void SaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = XmlConstants.FileExtension;
            sfd.Filter = "ExpressionMap Files|*" + XmlConstants.FileExtension;

            Nullable<bool> result = sfd.ShowDialog();

            if (result == true)
            {
                using (var writer = ExpressionMapWriter.CreateStandardXmlWriter(sfd.FileName))
                {
                    try
                    {
                        ExpressionMap map = CreateExpressionMapFromViewModels();
                        ExpressionMapWriter.WriteExpressionMap(writer, map); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error occurred when writing " + sfd.FileName + ":\n" + ex.Message); 
                    }
                    finally
                    {

                    }                
                }
            }
        }
        
        private void Reset()
        {
            Name = string.Empty;
            Articulations.Clear();
            SoundSlots.Clear(); 
        }

        private void ExtractViewModels(ExpressionMap map)
        {
            Name = map.Name;

            Dictionary<Articulation, ArticulationViewModel> art_map = new Dictionary<Articulation, ArticulationViewModel>();
            art_map.Add(Articulation.Blank, ArticulationViewModel.Blank); 

            _articulations.Clear(); 
            foreach (var art in map.Articulations)
            {
                ArticulationViewModel art_vm = new ArticulationViewModel(art);
                Articulations.Add(art_vm); 
                art_map.Add(art, art_vm);        
            }

            _soundSlots.Clear();
            foreach (var slot in map.SoundSlots)
            {
                SoundSlotViewModel slot_vm = new SoundSlotViewModel(slot);

                slot_vm.Art1 = art_map[slot.Articulations[0]];
                slot_vm.Art2 = art_map[slot.Articulations[1]];
                slot_vm.Art3 = art_map[slot.Articulations[2]];
                slot_vm.Art4 = art_map[slot.Articulations[3]];

                foreach (var oe in slot.OutputEvents)
                    slot_vm.OutputEvents.Add(new OutputEventViewModel(oe)); 

                SoundSlots.Add(slot_vm); 
            }
        }

        private ExpressionMap CreateExpressionMapFromViewModels()
        {
            ExpressionMap map = new ExpressionMap();
            map.Name = this.Name;

            foreach (var art_vm in Articulations)
            {
                map.Articulations.Add(art_vm.UnpackModel()); 
            }

            foreach (var slot_vm in SoundSlots)
            {
                var slot = slot_vm.UnpackModel();

                slot.Articulations[0] = slot_vm.Art1.UnpackModel();
                slot.Articulations[1] = slot_vm.Art2.UnpackModel();
                slot.Articulations[2] = slot_vm.Art3.UnpackModel();
                slot.Articulations[3] = slot_vm.Art4.UnpackModel();

                slot.OutputEvents.Clear();

                foreach (var oe in slot_vm.OutputEvents)
                    slot.OutputEvents.Add(oe.UnpackModel()); 

                map.SoundSlots.Add(slot); 
            }

            map.RemapArticulations(); 

            return map; 
        }

        private ExpressionMap? GenerateTestData()
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
                    return xm; 
                }
                else
                    throw new Exception("Failed to access project directory with test data"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("*** Error when reading file:");
                Console.WriteLine(ex);
            }

            return null; 
        }

        #endregion
    }

}
