using EME_Expression_Map_Editor.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EME_Expression_Map_Editor.ViewModel
{
    class BatchProcessingViewModel : ViewModelBase
    {
        private ObservableCollection<OutputEventViewModel> _outputEvents = new ObservableCollection<OutputEventViewModel>();
        public ObservableCollection<OutputEventViewModel> OutputEvents
        {
            get => _outputEvents;
            set => _outputEvents = value; 
        }

        private OutputEventViewModel? _replacementEvent;
        public OutputEventViewModel? ReplacementEvent
        {
            get => _replacementEvent;
            set
            {
                _replacementEvent = value;
                OnPropertyChanged(nameof(ReplacementEvent));
            }
        }

        private void AddOutputEventIfUnique(OutputEventViewModel oe)
        {
            if (oe != null && !OutputEvents.Contains(oe))
                OutputEvents.Add((OutputEventViewModel)oe.Clone());
        }

        private void InitializeReplacementEvent()
        {
            if (OutputEvents.Count > 0)
                ReplacementEvent = (OutputEventViewModel)OutputEvents.First().Clone();
            else
                ReplacementEvent = new OutputEventViewModel(); 
        }

        public void Initialize(IList<SoundSlotViewModel> slots)
        {
            foreach (var slot in slots)
                foreach (var oe in slot.OutputEvents)
                    AddOutputEventIfUnique(oe);
            
            InitializeReplacementEvent();
        }

        private WindowReturnCode _windowStatus;
        public WindowReturnCode WindowStatus
        {
            get => _windowStatus;
            set => _windowStatus = value;
        }

        public ICommand CloseWindowCommand { get; private set; }
        private void CloseWindow(WindowReturnCode rc)
        {
            WindowStatus = rc;
            Application.Current.MainWindow.Close(); 
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public BatchProcessingViewModel()
        {
            CloseWindowCommand = new CustomCommand<WindowReturnCode>(CloseWindow); 
        }
    }
}
