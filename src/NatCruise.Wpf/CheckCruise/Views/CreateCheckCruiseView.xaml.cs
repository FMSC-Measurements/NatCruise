using Microsoft.Win32;
using NatCruise.Wpf.CheckCruise.ViewModels;
using NatCruise.Wpf.Util;
using Prism;
using System;
using System.IO;
using System.Windows.Controls;

namespace NatCruise.Wpf.CheckCruise.Views
{
    /// <summary>
    /// Interaction logic for CreateCheckCruiseView.xaml
    /// </summary>
    public partial class CreateCheckCruiseView : UserControl
    {
        

        protected CreateCheckCruiseView()
        {
            InitializeComponent();
        }

        public CreateCheckCruiseView(CreateCheckCruiseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void _browseCheckCruisePathButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var curFilePath = _checkCruisePathTextBox.Text;
            //string defatultPath = null;
            //if (!string.IsNullOrEmpty(curFilePath))
            //{
            //    Path.
            //}


            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "V3 Cruise File (.crz3)|*.*.crz3",
                DefaultExt = ".crz3",
                FileName = curFilePath,
            };

            if(saveFileDialog.ShowDialog(this.GetWindow()) ?? false)
            {
                _checkCruisePathTextBox.Text = saveFileDialog.FileName;
            }
        }

        //public event EventHandler IsActiveChanged;

        //protected override void OnInitialized(EventArgs e)
        //{
        //    base.OnInitialized(e);

        //    var viewModel = DataContext as CreateCheckCruiseViewModel;

        //}
    }
}