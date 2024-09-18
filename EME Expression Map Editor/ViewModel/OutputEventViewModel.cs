using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EME_Expression_Map_Editor.Model; 

namespace EME_Expression_Map_Editor.ViewModel
{
    internal class OutputEventViewModel : ViewModelBase
    {
        private OutputEvent _event = new OutputEvent(); 

        // Note that in order to be consistent with Cubase's standards, Program Changes are displayed
        // as 1-128, *not* as the binary accurate 0-127 - hence the special clauses and offset-by-one. 
        public string Data1
        {
            get
            {
                if (_event.EventType == OutputEvent.NoteEvent)
                    return MidiNote.MidiNoteToString(_event.Data1);
                else if (_event.EventType == OutputEvent.ProgramChangeEvent)
                    return (_event.Data1 + 1).ToString();
                else
                    return _event.Data1.ToString();
            }
            set
            {
                // Verify input is either numeric value or valid note name before proceeding
                string str = value.Trim(); 
                if (MidiNote.IsValidNoteName(str) || Int32.TryParse(str, out int _))
                {
                    int n = MidiNote.TryParse(value);
                    if (_event.EventType == OutputEvent.ProgramChangeEvent)
                        _event.Data1 = n - 1;  // Program Change offset
                    else
                        _event.Data1 = n;
                }

                OnPropertyChanged(nameof(Data1));
            }
        }

        // Will be hidden & disabled when using Program Change events
        public string Data2
        {
            get
            {
                if (_event.EventType == OutputEvent.ProgramChangeEvent)
                    return string.Empty; 
                else
                    return _event.Data2.ToString();
            }
            set
            {
                if (_event.EventType != OutputEvent.ProgramChangeEvent && Int32.TryParse(value, out int n))
                {
                    _event.Data2 = n;
                    OnPropertyChanged(nameof(Data2));
                }
            }
        }

        public int EventType
        {
            get
            {
                if (_event.EventType == OutputEvent.NoteEvent)
                    return 0;
                if (_event.EventType == OutputEvent.ControllerEvent)
                    return 1;
                else
                    return 2;
            }
            set
            {
                if (value == 0)
                    _event.EventType = OutputEvent.NoteEvent;
                else if (value == 1)
                    _event.EventType = OutputEvent.ControllerEvent;
                else
                    _event.EventType = OutputEvent.ProgramChangeEvent;
                OnPropertyChanged(nameof(EventType));
                OnPropertyChanged(nameof(Data1));
                OnPropertyChanged(nameof(Data2));   
            }
        }

        // Mapping of numeric to human-readable representations of available Event Types
        // Note that for the purposes of the UI we use simple numbering, not the arbitrary number codes used internally by the ExpressionMap spec (defined in OutputEvent model layer) 
        private static Dictionary<int, string> _eventTypeOptions = new Dictionary<int, string>()
        {
            {0, "Note" },
            {1, "Controller" },
            {2, "Program Change" }
        };
        public static Dictionary<int, string> EventTypeOptions
        {
            get => _eventTypeOptions; 
        }
        
        // Convenience property for the Batch Processing window: returns Event Type in string form
        public string EventTypeAsString
        {
            get => EventTypeOptions[EventType]; 
        }

        // Convenience property for Batch Processing Window
        public int Occurrences { get; set; }

        public void Increment(bool data1, bool data2)
        {
            if (data1)
                _event.Data1++;
            if (data2)
                _event.Data2++; 
        }

        public override object Clone()
        {
            var copy = new OutputEventViewModel(_event.Duplicate());
            copy.Occurrences = this.Occurrences;
            return copy; 
        }

        public OutputEvent UnpackModel()
            => _event.Duplicate();

        public bool SameDataAs(OutputEventViewModel e)
            => _event.EventType == e._event.EventType 
            && _event.Data1 == e._event.Data1 
            && _event.Data2 == e._event.Data2; 

        public OutputEventViewModel()
        {
            
        }

        public OutputEventViewModel(OutputEvent oe)
        {
            _event = oe; 
        }
    }
}
