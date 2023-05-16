using System;
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

        private SoundSlot _slot;

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

        public void SetArticulation(ArticulationViewModel art_vm)
        {
            UnassignArticulation(art_vm); 

            if (art_vm.Group == 0) Art1 = art_vm;
            if (art_vm.Group == 1) Art2 = art_vm;
            if (art_vm.Group == 2) Art3 = art_vm;
            if (art_vm.Group == 3) Art4 = art_vm;
        }

        public void UnassignArticulation(ArticulationViewModel art_vm)
        {
            if (Art1.Equals(art_vm)) Art1 = ArticulationViewModel.Blank;
            if (Art2.Equals(art_vm)) Art2 = ArticulationViewModel.Blank;
            if (Art3.Equals(art_vm)) Art3 = ArticulationViewModel.Blank;
            if (Art4.Equals(art_vm)) Art4 = ArticulationViewModel.Blank;
        }

        private void UnassignAllArticulations()
        {
            Art1 = ArticulationViewModel.Blank;
            Art2 = ArticulationViewModel.Blank;
            Art3 = ArticulationViewModel.Blank;
            Art4 = ArticulationViewModel.Blank;
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

        public string RemoteKey
        {
            get => _slot.RemoteKey == SoundSlot.NoRemoteKey ? "-" : _slot.RemoteKey.ToString(); 
            set
            {
                if (Int32.TryParse(value, out int n))
                    _slot.RemoteKey = n;
                else if (value.Equals("-") || value.Equals(string.Empty))
                    _slot.RemoteKey = SoundSlot.NoRemoteKey;

                OnPropertyChanged("RemoteKey");
            }
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
            get => _slot.MinPitch.ToString();
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _slot.MinPitch = n;
                    OnPropertyChanged(nameof(MinPitch));
                }
            }
        }

        public string MaxPitch
        {
            get => _slot.MaxPitch.ToString();
            set
            {
                if (Int32.TryParse(value, out int n))
                {
                    _slot.MaxPitch = n;
                    OnPropertyChanged(nameof(MaxPitch));
                }
            }
        }

        #endregion

        #region Commands and related functions

        public ICommand AddOutputEventCommand { get; private set; }
        public void AddOutputEvent(int idx)
        {
            if (OutputEvents.Count == 0)
            {
                OutputEvents.Add(new OutputEventViewModel(new OutputEvent()));
                SelectedEventIndex = 0; 
            }
            else
            {
                idx = Math.Clamp(idx, 0, OutputEvents.Count - 1);
                var event_vm = (OutputEventViewModel)OutputEvents[idx].GetPrototype(); 

                OutputEvents.Insert(idx + 1, event_vm);
                SelectedEventIndex = idx + 1; 
            }
        }
        public ICommand RemoveOutputEventCommand { get; private set; }

        /*
        public void RemoveOutputEvent()
        {
            int pre_idx = SelectedEventIndex;
            foreach (var item in OutputEvents.ToList().Where(x => x.IsSelected))
                OutputEvents.Remove(item);
            SelectedEventIndex = Math.Clamp(pre_idx, -1, OutputEvents.Count - 1); 
        }
        */

        #endregion

        public override ViewModelBase GetPrototype()
        {
            var slot_vm = new SoundSlotViewModel(new SoundSlot()); 
            return slot_vm;
        }

        public SoundSlotViewModel Duplicate()
        {
            var slot_vm = new SoundSlotViewModel(_slot.Duplicate());
            
            slot_vm.Art1 = this.Art1;
            slot_vm.Art2 = this.Art2; 
            slot_vm.Art3 = this.Art3;
            slot_vm.Art4 = this.Art4;
            
            return slot_vm;
        }
        
        public SoundSlotViewModel(SoundSlot slot)
        {            
            _slot = slot;

            AddOutputEventCommand = new CustomCommand<int>(AddOutputEvent);
            //RemoveOutputEventCommand = new NoParameterCommand(RemoveOutputEvent);
            RemoveOutputEventCommand = new NoParameterCommand(() => { SelectedEventIndex = Common.RemoveItem(OutputEvents, SelectedEventIndex, Common.DoNothing); });
        }
    }
}
