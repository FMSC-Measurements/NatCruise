using NatCruise.Design.ViewModels;
using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace NatCruise.Design.Views
{
    

    /// <summary>
    /// Interaction logic for SpeciesListView.xaml
    /// </summary>
    public partial class SpeciesListView : UserControl
    {
        //ComboBox _fiaCombobox;

        public SpeciesListView()
        {
            InitializeComponent();

            _fiaCombobox.IsTextSearchEnabled = true;
            _fiaCombobox.PreviewTextInput += _fiaCombobox_PreviewTextInput;
            _fiaCombobox.PreviewKeyUp += _fiaCombobox_PreviewKeyUp;
            _fiaCombobox.DropDownClosed += _fiaCombobox_DropDownClosed;
            DataObject.AddPastingHandler(_fiaCombobox, _fiaCombobox_Pasting);

            
        }

        private void _fiaCombobox_DropDownClosed(object sender, EventArgs e)
        {
            ApplyFilter(null);
        }

        private bool _fiaComboboxFilter(object item)
        {
            if (_fiaFilter == null) { return true; }
            if(item == null) { return false; }
            var itemText = item.ToString();
            var isMatch = _fiaFilter.IsMatch(itemText);
            return isMatch;
        }

        private void _fiaCombobox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                ComboBox cmb = (ComboBox)sender;

                cmb.IsDropDownOpen = true;

                ApplyFilter(cmb.Text);
            }
        }

        private void _fiaCombobox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, pastedText);


            ApplyFilter(fullText);
        }

        Regex _fiaFilter;

        // for getting combobox EditTextBox
        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void _fiaCombobox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            if (!string.IsNullOrEmpty(cmb.Text))
            {
                string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, e.Text);
                ApplyFilter(fullText);
            }
            else
            {
                ApplyFilter(e.Text);
            }

            
        }

        private void ApplyFilter(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                _fiaFilter = new Regex(filter, RegexOptions.None, TimeSpan.FromMilliseconds(100));

            }
            else
            {
                _fiaFilter = null;
            }

            _fiaCombobox.Items.Filter = _fiaComboboxFilter;
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldvm = e.OldValue as SpeciesListViewModel;
            if (oldvm != null)
            {
                oldvm.SpeciesAdded -= HandleSpeciesAdded;
            }
            var newvm = e.NewValue as SpeciesListViewModel;
            if (newvm != null)
            {
                newvm.SpeciesAdded += HandleSpeciesAdded;
            }
        }

        private void HandleSpeciesAdded(object sender, EventArgs e)
        {
            _newSpeciesTextBox.Clear();
        }

        private void _speciesDetailsPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVM = e.OldValue as SpeciesDetailViewModel;
            if (oldVM != null)
            {
                oldVM.ContractSpeciesAdded -= SpDetailVM_ContractSpeciesAdded;
            }
            var newVM = e.NewValue as SpeciesDetailViewModel;
            if(newVM != null)
            {
                newVM.ContractSpeciesAdded += SpDetailVM_ContractSpeciesAdded;
            }


            void SpDetailVM_ContractSpeciesAdded(object sender, EventArgs e)
            {
                _ctrSpTextBox.Clear();
                _prodComboBox.SelectedIndex = 0;
            }

        }
    }
}
