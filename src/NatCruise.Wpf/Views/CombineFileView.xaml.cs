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

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for CombineFileView.xaml
    /// </summary>
    public partial class CombineFileView : UserControl
    {
        public CombineFileView()
        {
            InitializeComponent();
        }

        // HACK binding to a set up radio buttons is a nightmire
        // Changing the IsChecked state on one radio button causes all the others
        // in the group to change too. Even worse when you throw a value converter in there too
        // I could probably get the binding to work but after wrestleing with it for a day
        // I decided to go with this solution of click handlers and DataContextChanged handlers
        // since it minimized the behind the sceans magic. 

        private void SelectResolveDestAsIs(object sender, RoutedEventArgs e)
        {
            var radioBtn = (RadioButton)sender;
            var host = (System.Windows.Controls.Panel)radioBtn.Parent;
            var conflict = host.DataContext as CruiseDAL.V3.Sync.Conflict;

            if (conflict != null)
            {
                conflict.ConflictResolution = CruiseDAL.V3.Sync.ConflictResolutionType.ChoseDest;
            }
        }

        private void SelectResolveDestWithEdits(object sender, RoutedEventArgs e)
        {
            var radioBtn = (RadioButton)sender;
            var host = (System.Windows.Controls.Panel)radioBtn.Parent;
            var conflict = host.DataContext as CruiseDAL.V3.Sync.Conflict;

            if (conflict != null)
            {
                conflict.ConflictResolution = CruiseDAL.V3.Sync.ConflictResolutionType.ModifyDest;
            }
        }

        private void SelectResolveSourceAsIs(object sender, RoutedEventArgs e)
        {
            var radioBtn = (RadioButton)sender;
            var host = (System.Windows.Controls.Panel)radioBtn.Parent;
            var conflict = host.DataContext as CruiseDAL.V3.Sync.Conflict;

            if (conflict != null)
            {
                conflict.ConflictResolution = CruiseDAL.V3.Sync.ConflictResolutionType.ChoseSource;
            }
        }

        private void SelectResolveSourceWithEdits(object sender, RoutedEventArgs e)
        {
            var radioBtn = (RadioButton)sender;
            var host = (System.Windows.Controls.Panel)radioBtn.Parent;
            var conflict = host.DataContext as CruiseDAL.V3.Sync.Conflict;

            if (conflict != null)
            {
                conflict.ConflictResolution = CruiseDAL.V3.Sync.ConflictResolutionType.ModifySource;
            }
        }

        private void SelectedConflictChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var stackPannel = (StackPanel)sender;

            //var radioButtons = stackPannel.Children.OfType<RadioButton>().ToArray();
            //foreach (var radioButton in radioButtons)
            //{
            //    radioButton.Background = System.Windows.Media.Brushes.Blue;
            //}

            //var conflict = e.NewValue as CruiseDAL.V3.Sync.Conflict;
            //if(conflict != null)
            //{
            //    var resolution = conflict.ConflictResolution;

            //    radioButtons[0].IsChecked = resolution == CruiseDAL.V3.Sync.ConflictResolutionType.ChoseDest;
            //    radioButtons[1].IsChecked = resolution == CruiseDAL.V3.Sync.ConflictResolutionType.ModifyDest;
            //    radioButtons[2].IsChecked = resolution == CruiseDAL.V3.Sync.ConflictResolutionType.ChoseSource;
            //    radioButtons[3].IsChecked = resolution == CruiseDAL.V3.Sync.ConflictResolutionType.ModifySource;
            //}
            //else
            //{
            //    radioButtons[0].IsChecked = false;
            //    radioButtons[1].IsChecked = false;
            //    radioButtons[2].IsChecked = false;
            //    radioButtons[3].IsChecked = false;
            //}
        }
    }
}
