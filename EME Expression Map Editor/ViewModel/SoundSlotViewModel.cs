﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using EME_Expression_Map_Editor.Command;
using EME_Expression_Map_Editor.Model;
namespace EME_Expression_Map_Editor.ViewModel
{

    internal class SoundSlotViewModel : ViewModelBase
    {
        private static readonly string AnyChannelLabel = "Any";

        private SoundSlot _slot = new SoundSlot();

        private ObservableCollection<OutputEventViewModel> _outputEvents = new ObservableCollection<OutputEventViewModel>(); 
        public ObservableCollection<OutputEventViewModel> OutputEvents
        {
            get => _outputEvents;
            set => _outputEvents = value;
        }

        private OutputEventViewModel? _selectedEvent;
        public OutputEventViewModel? SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));
            }
        }

        private int _selectedEventIndex;
        public int SelectedEventIndex
        {
            get => _selectedEventIndex;
            set
            {
                _selectedEventIndex = value;
                OnPropertyChanged(nameof(SelectedEventIndex));
            }
        }


        #region SoundSlot Properties

        private ArticulationViewModel _art1 = ArticulationViewModel.Blank;
        public ArticulationViewModel Art1
        {
            get => _art1; 
            set 
            {
                if (value == null)
                    _art1 = ArticulationViewModel.Blank; 
                else
                    _art1 = value;
                OnPropertyChanged(nameof(Art1));
            }
        }
        private ArticulationViewModel _art2 = ArticulationViewModel.Blank;
        public ArticulationViewModel Art2
        {
            get => _art2;
            set
            {
                if (value == null)
                    _art2 = ArticulationViewModel.Blank;
                else
                    _art2 = value;
                OnPropertyChanged(nameof(Art2));
            }
        }
        private ArticulationViewModel _art3 = ArticulationViewModel.Blank;
        public ArticulationViewModel Art3
        {
            get => _art3;
            set
            {
                if (value == null)
                    _art3 = ArticulationViewModel.Blank;
                else
                    _art3 = value;
                OnPropertyChanged(nameof(Art3));
            }
        }
        private ArticulationViewModel _art4 = ArticulationViewModel.Blank;
        public ArticulationViewModel Art4
        {
            get => _art4;
            set
            {
                if (value == null)
                    _art4 = ArticulationViewModel.Blank;
                else
                    _art4 = value;
                OnPropertyChanged(nameof(Art4));
            }
        }

        public bool ContainsArticulation(ArticulationViewModel a)
            => Art1.Equals(a) || Art2.Equals(a) || Art3.Equals(a) || Art4.Equals(a); 

        public void SetArticulation(ArticulationViewModel art_vm, int group)
        {
            if (group == 0)
                Art1 = art_vm;
            else if (group == 1)
                Art2 = art_vm;
            else if (group == 2)
                Art3 = art_vm;
            else
                Art4 = art_vm; 
        }

        public void RemoveArticulation(int group)
            => SetArticulation(ArticulationViewModel.Blank, group); 

        public void RemoveAllArticulations()
        {
            RemoveArticulation(0);
            RemoveArticulation(1);
            RemoveArticulation(2);
            RemoveArticulation(3);
        }

        public string Name
        {
            get => _slot.Name;
            set
            {
                _slot.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // For consistency with Cubase, Program Changes will be displayed as 1-128, not the more binary accurate 0-127. 
        // Hence the special clauses and offsets. 
        public string RemoteKey
        {
            get
            {
                if (_slot.RemoteKey == SoundSlot.NoRemoteKey)
                    return "-";
                else if (ExpressionMapViewModel.Instance.ShowRemoteKeysAsProgramChanges)
                    return (_slot.RemoteKey + 1).ToString(); 
                else
                    return MidiNote.MidiNoteToString(_slot.RemoteKey);
            }
            set
            {
                int n = MidiNote.TryParse(value);

                if (ExpressionMapViewModel.Instance.ShowRemoteKeysAsProgramChanges)
                    n -= 1;  // Program change offset

                if (n >= 0)
                    _slot.RemoteKey = n;
                else
                    _slot.RemoteKey = SoundSlot.NoRemoteKey; 
                OnPropertyChanged(nameof(RemoteKey)); 
            }
        }

        public void UnassignRemoteKey()
        {
            _slot.RemoteKey = SoundSlot.NoRemoteKey; 
            OnPropertyChanged(nameof(RemoteKey));
        }

        // Slot's midi channel: displayed as 1-16 on UI for readability
        // An offset of (-1) is applied to convert to 0-15 formatting internally
        // Model layer handles range checks
        public string Channel
        {
            get => _slot.Channel == SoundSlot.AnyChannel ? AnyChannelLabel : (_slot.Channel + 1).ToString();
            set
            {
                int channel = 0;
                
                if (Int32.TryParse(value, out channel))
                    _slot.Channel = channel - 1;
                else if (value.ToLower().Equals(AnyChannelLabel.ToLower()))
                    _slot.Channel = SoundSlot.AnyChannel;
                OnPropertyChanged(nameof(Channel));
            }
        }

        // Read-only property with human-readable Midi Channel options for UI display
        // Options are channel 1-16 or 'Any'
        public static ObservableCollection<string> ChannelOptions
        {
            get
            {
                ObservableCollection<string> options = new ObservableCollection<string>();
                options.Add(AnyChannelLabel);
                for (int i = 1; i <= 16; ++i)
                    options.Add(i.ToString());
                return options;
            }
        }

        // Color codes are defined in Global Resources
        // ExpressionMap format uses 1-16 internally:
        // ViewModel uses 0-15 for easier indexing in resource file, hence (+/-1) offset in translation between layers
        public int Color
        {
            get => _slot.Color - 1;
            set
            {
                if (_slot.Color != (value + 1))
                {
                    _slot.Color = (value + 1);
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        private int _velocityFactor
        {
            get => Common.FactorToPercentage(_slot.VelocityFactor);
            set
            {
                _slot.VelocityFactor = Common.PercentageToFactor(value);
                OnPropertyChanged(nameof(_velocityFactor));
            }
        }

        public string VelocityFactor
        {
            get => _velocityFactor.ToString() + "%";
            set => _velocityFactor = Common.ParsePercentage(value, _velocityFactor);
        }

        private int _lengthFactor
        {
            get => Common.FactorToPercentage(_slot.LengthFactor);
            set
            {
                _slot.LengthFactor = Common.PercentageToFactor(value);
                OnPropertyChanged(nameof(_lengthFactor));
            }
        }

        public string LengthFactor
        {
            get => _lengthFactor.ToString() + "%";
            set => _lengthFactor = Common.ParsePercentage(value, _lengthFactor);
        }

        public string MinVelocity
        {
            get => _slot.MinVelocity.ToString();
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _slot.MinVelocity = n;
                    OnPropertyChanged(nameof(MinVelocity));
                }
            }
        }

        public string MaxVelocity
        {
            get => _slot.MaxVelocity.ToString();
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _slot.MaxVelocity = n;
                    OnPropertyChanged(nameof(MaxVelocity));
                }
            }
        }

        public string Transpose
        {
            get => _slot.Transpose.ToString();
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _slot.Transpose = n;
                    OnPropertyChanged(nameof(Transpose));
                }
            }
        }

        public string MinPitch
        {
            get => MidiNote.MidiNoteToString(_slot.MinPitch); 
            set
            {
                _slot.MinPitch = MidiNote.TryParse(value); 
                OnPropertyChanged(nameof(MinPitch));
            }
        }

        public string MaxPitch
        {
            get => MidiNote.MidiNoteToString(_slot.MaxPitch);
            set
            {
                _slot.MaxPitch= MidiNote.TryParse(value);
                OnPropertyChanged(nameof(MaxPitch));
            }
        }

        #endregion

        #region Commands and related functions

        public ICommand? AddOutputEventCommand { get; private set; } 

        public ICommand? RemoveOutputEventCommand { get; private set; }

        #endregion

        public override object Clone()
        {
            var slot_vm = new SoundSlotViewModel(_slot.Duplicate());

            slot_vm.OutputEvents.Clear();
            foreach (var e in OutputEvents)
                slot_vm.OutputEvents.Add((OutputEventViewModel)e.Clone()); 

            slot_vm.Art1 = this.Art1;
            slot_vm.Art2 = this.Art2;
            slot_vm.Art3 = this.Art3;
            slot_vm.Art4 = this.Art4;

            return slot_vm;
        }

        public SoundSlot UnpackModel()
        {
            return _slot.Duplicate(); 
        }

        private void AddOutputEventPost(OutputEventViewModel src, OutputEventViewModel dest)
        {
            dest.EventType = src.EventType;
            dest.Data1 = src.Data1;
            dest.Data2 = src.Data2; 
        }

        private void InitCommands()
        {
            AddOutputEventCommand = new CustomCommand<int>((n) => { SelectedEventIndex = Common.AddItem(OutputEvents, SelectedEventIndex, AddOutputEventPost); });
            RemoveOutputEventCommand = new NoParameterCommand(() => { SelectedEventIndex = Common.RemoveItem(OutputEvents, SelectedEventIndex, (n) => n.IsSelected, Common.DoNothing); });
        }
        
        public SoundSlotViewModel()
        {
            InitCommands(); 
        }

        public SoundSlotViewModel(SoundSlot slot)
        {            
            _slot = slot;

            foreach (var e in slot.OutputEvents)
                OutputEvents.Add(new OutputEventViewModel(e)); 

            InitCommands();
        }
    }
}
