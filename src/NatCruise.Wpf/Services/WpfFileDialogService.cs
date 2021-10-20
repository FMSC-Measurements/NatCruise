using Microsoft.Win32;
using NatCruise.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class WpfFileDialogService : IFileDialogService
    {
        private IWpfApplicationSettingService AppSettings { get; }

        public WpfFileDialogService(IWpfApplicationSettingService appSettings)
        {
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public Task<string> SelectCruiseFileAsync()
        {
            return Task.Run(SelectCruiseFile);
        }

        public string SelectCruiseFile()
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = AppSettings.LastOpenCruiseDir,
                DefaultExt = "*.crz3",
                Filter = "All V3 file types|*.crz3;*.crz3t;|" +
                         "cruise files V3 (*.crz3)|*.crz3|" +
                         "template file V3 (*.crz3t)|*.crz3t|" +
                         "cruise file V2 (*.cruise)|*.cruise",
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePath = dialog.FileName;
                var fileDir = Path.GetDirectoryName(filePath);
                var extention = Path.GetExtension(filePath).ToLower();
                if(extention == ".crz3t")
                { AppSettings.LastOpenTemplateDir = fileDir; }
                else if(extention == ".crz3")
                { AppSettings.LastOpenCruiseDir = fileDir; }

                return filePath;
            }
            else
            { return null; }
        }

        public Task<string> SelectCruiseFileDestinationAsync(string defaultDir = null, string defaultFileName = null)
        {
            return Task.Run(() => SelectCruiseFileDestination(defaultDir, defaultFileName));
        }

        public string SelectCruiseFileDestination(string defaultDir = null, string defaultFileName = null)
        {
            defaultDir ??= AppSettings.LastOpenCruiseDir;

            var dialog = new SaveFileDialog()
            {
                DefaultExt = "*.crz3",
                Filter = "cruise files V3 (*.crz3)|*.crz3|" +
                            "cruise file V2 (*.cruise)|*.cruise",
                AddExtension = true,
                FileName = defaultFileName,
                InitialDirectory = defaultDir,
                OverwritePrompt = true,
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePath = dialog.FileName;
                var fileDir = Path.GetDirectoryName(filePath);
                AppSettings.LastOpenCruiseDir = fileDir;

                return filePath;
            }
            else
            {
                return null;
            }
        }

        public Task<string> SelectTemplateFileAsync()
        {
            return Task.Run(SelectTemplateFile);
        }

        public string SelectTemplateFile()
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = AppSettings.LastOpenTemplateDir,
                DefaultExt = "*.crz3t",
                Filter = "V3 Template File (*.crz3t)|*.crz3t| V2 Template File (*.cut)|*.cut",
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePath = dialog.FileName;
                var fileDir = Path.GetDirectoryName(filePath);
                AppSettings.LastOpenTemplateDir = fileDir;

                return filePath;
            }
            else
            { return null; }
        }

        public Task<string> SelectBackupFileDestinationAsync(string defaultDir = null, string defaultFileName = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> SelectTemplateFileDestinationAsync(string defaultDir = null, string defaultFileName = null)
        {
            return Task.Run(() => SelectTemplateFileDestination(defaultDir, defaultFileName));
        }

        public string SelectTemplateFileDestination(string defaultDir = null, string defaultFileName = null)
        {
            defaultDir ??= AppSettings.LastOpenTemplateDir;

            var dialog = new SaveFileDialog()
            {
                DefaultExt = "*.crz3t",
                Filter = "cruise v3 template files (*.crz3t)|*.crz3t",
                AddExtension = true,
                FileName = defaultFileName,
                InitialDirectory = defaultDir,
                OverwritePrompt = true,
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePath = dialog.FileName;
                var fileDir = Path.GetDirectoryName(filePath);
                AppSettings.LastOpenTemplateDir = fileDir;

                return filePath;
            }
            else
            {
                return null;
            }
        }

        public Task<string> SelectCruiseDatabaseAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}