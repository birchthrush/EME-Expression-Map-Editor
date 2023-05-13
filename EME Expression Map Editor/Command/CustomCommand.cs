using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EME_Expression_Map_Editor.Command
{
    public class CustomCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CustomCommand(Action<T> act)
        {
            _action = act;
        }

        public bool CanExecute(object? parameter)
            => true;

        public void Execute(object? parameter)
        {
            if (parameter != null)
                _action((T)parameter); 
        }
    }

    public class NoParameterCommand : ICommand
    {
        private readonly Action _action;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public NoParameterCommand(Action act)
        {
            _action = act;
        }

        public bool CanExecute(object? parameter)
            => true;

        public void Execute(object? parameter)
            => _action(); 
    }

    public class CustomDropHandler : IDropTarget
    {
        private readonly Action<IDropInfo> _drag;
        private readonly Action<IDropInfo> _drop;

        public CustomDropHandler(Action<IDropInfo> drag_over, Action<IDropInfo> drop)
        {
            _drag = drag_over;
            _drop = drop;
        }
        public void DragOver(IDropInfo dropInfo)
            => _drag(dropInfo);

        public void Drop(IDropInfo dropInfo)
            => _drop(dropInfo); 
    }
}
