using EME_Expression_Map_Editor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EME_Expression_Map_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ExpressionMapEditor : Window
    {
        private ExpressionMapViewModel _vm;

        public ExpressionMapEditor()
        {
            InitializeComponent();
            this._vm = ExpressionMapViewModel.Instance;
            this.DataContext = _vm;
        }

        private void SoundSlotGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.SoundSlotSelectionChangedHandler(); 
        }


        /*
        private void SoundSlotGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = 0; 
            foreach (var slot in _vm.SoundSlots)
            {
                if (slot.IsSelected)
                {
                    _vm.SelectedSlotIndex = idx;
                    return;
                }
                else
                    ++idx; 
            }
        }
        */
    }
}
