using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EME_Expression_Map_Editor.Model; 

namespace EME_Expression_Map_Editor.ViewModel
{

    internal class SoundSlotViewModel : ViewModelBase
    {
        private static readonly string AnyChannelLabel = "Any"; 

        private SoundSlot _slot;

        private ObservableCollection<OutputEventViewModel>? _outputEvents; 
        public ObservableCollection<OutputEventViewModel>? OutputEvents
        {
            get
            {
                if (_outputEvents == null)
                {
                    ObservableCollection<OutputEventViewModel> events = new ObservableCollection<OutputEventViewModel>();
                    foreach (OutputEvent oe in _slot.OutputEvents)
                        events.Add(new OutputEventViewModel(oe));
                    _outputEvents = events;
                }
                
                return _outputEvents;
            }
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


        #region SoundSlot Properties
        public Articulation Art1
        {
            get => GetArticulation(0);
            set => SetArticulation(value, 0);
        }

        public Articulation Art2
        {
            get => GetArticulation(1);
            set => SetArticulation(value, 1);
        }

        public Articulation Art3
        {
            get => GetArticulation(2);
            set => SetArticulation(value, 2);
        }

        public Articulation Art4
        {
            get => GetArticulation(3);
            set => SetArticulation(value, 3);
        }

        private Articulation GetArticulation(int n)
        {
            if (_slot.Articulations[n] == null)
                return Articulation.Blank; 
            else
                return _slot.Articulations[n];
        }

        public void SetArticulation(Articulation art, int n)
        {
            if (art == null || art == Articulation.Blank)
                _slot.UnassignArticulation(n);
            else
                _slot.AssignArticulation(art, n);

            if (n == 0) OnPropertyChanged(nameof(Art1));
            if (n == 1) OnPropertyChanged(nameof(Art2));
            if (n == 2) OnPropertyChanged(nameof(Art3));
            if (n == 3) OnPropertyChanged(nameof(Art4));

            //OnPropertyChanged("Art" + (n + 1).ToString());
            //OnPropertyChanged("Art" + (n + 1).ToString() + "Idx");
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
        public ObservableCollection<string> ChannelOptions
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

        public SoundSlotViewModel(SoundSlot slot)
        {
            _slot = slot;
        }
    }
}
