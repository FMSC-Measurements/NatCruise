using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NatCruise.Wpf.Views
{
    /// <summary>
    /// Interaction logic for NewCruiseSaveDialogView.xaml
    /// </summary>
    public partial class NewCruiseSaveDialogView : MetroWindow, INotifyPropertyChanged
    {
        private bool _useSaleFolder;
        private string _saleFolderName;
        private string _cruiseFileName;
        private string _cruiseFilePath;
        private string _cruiseFolder;

        public NewCruiseSaveDialogView()
        {
            InitializeComponent();
        }

        public string CruiseFolder
        {
            get => _cruiseFolder;
            set
            {
                SetProperty(ref _cruiseFolder, value);
                RefreshCruiseFilePath();
            }
        }

        public bool UseSaleFolder
        {
            get => _useSaleFolder;
            set
            {
                SetProperty(ref _useSaleFolder, value);
                RefreshCruiseFilePath();
            }
        }

        public string SaleFolderName
        {
            get => _saleFolderName;
            set
            {
                SetProperty(ref _saleFolderName, value);
                RefreshCruiseFilePath();
            }
        }

        public string CruiseFileName
        {
            get => _cruiseFileName;
            set
            {
                SetProperty(ref _cruiseFileName, value);
                RefreshCruiseFilePath();
            }
        }

        public string CruiseFilePath
        {
            get => _cruiseFilePath;
            set
            {
                SetProperty(ref _cruiseFilePath, value);
                _cruiseFilePathScrollViewer.ScrollToRightEnd();
            }
        }

        public void RefreshCruiseFilePath()
        {
            var cruiseFolder = CruiseFolder?.Trim();
            var cruiseFileName = CruiseFileName?.Trim();
            if (!string.IsNullOrEmpty(cruiseFolder)
                && !string.IsNullOrEmpty(cruiseFileName))
            {
                var ext = Path.GetExtension(cruiseFileName);
                if (ext == null || ext != ".crz3")
                {
                    cruiseFileName += ".crz3";
                }
                var saleFolder = SaleFolderName?.Trim();
                if (UseSaleFolder && !string.IsNullOrEmpty(saleFolder))
                {
                    cruiseFolder = Path.Combine(cruiseFolder, saleFolder);
                }
                try
                {
                    var cruiseFileInfo = new FileInfo(Path.Combine(cruiseFolder, cruiseFileName));

                    CruiseFilePath = cruiseFileInfo.FullName;
                    _okButton.IsEnabled = true;
                }
                catch
                {
                    _okButton.IsEnabled = false;
                }

            }
            else
            {
                CruiseFilePath = null;
                _okButton.IsEnabled = false;
            }
        }

        private void _browseCruiseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = "Select Cruise Folder",
            };

            var currentFolder = _cruiseFolderTextBox.Text;
            try
            {
                var di = new DirectoryInfo(currentFolder);
                if (!string.IsNullOrEmpty(currentFolder))
                {
                    folderBrowserDialog.SelectedPath = di.FullName;
                }
            }
            catch { }

            var result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _cruiseFolderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void _okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void _caneclButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #region INPC members
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        #endregion INPC members
    }


}
