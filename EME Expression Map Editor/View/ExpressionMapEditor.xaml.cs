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
        public ExpressionMapEditor()
        {
            InitializeComponent();
            this.DataContext = ExpressionMapViewModel.Instance;
        }

        private void ArticulationGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (_presenter != null)
                _presenter.SelectedArticulations = ArticulationGrid.SelectedItems;
            */
        }

        private void SoundSlotGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (_presenter != null)
                _presenter.SelectedSlots = SoundSlotGrid.SelectedItems;
            */
        }
    }
}
