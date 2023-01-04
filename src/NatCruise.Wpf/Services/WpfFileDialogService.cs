using Microsoft.Win32;
using NatCruise.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Task<IEnumerable<string>> SelectCruiseFilesAsync()
        {
            return Task.Run(SelectCruiseFiles);
        }

        public IEnumerable<string> SelectCruiseFiles()
        {
            var initialDir = AppSettings.LastOpenCruiseDir;
            if (!System.IO.Directory.Exists(initialDir))
            {
                initialDir = AppSettings.DefaultOpenCruiseDir;
            }

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = initialDir,
                DefaultExt = "*.crz3",
                Filter = "Cruise Files V3 (*.crz3)|*.crz3|" +
                            "FScruiser Database Files (*.crz3db)|*.crz3db",
                Multiselect = true,
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var filePaths = dialog.FileNames;

                var filePath = filePaths.FirstOrDefault();
                if(filePath != null)
                {
                    var fileDir = Path.GetDirectoryName(filePath);
                    AppSettings.LastOpenCruiseDir = fileDir;
                }

                return filePaths;
            }
            else
            { return Enumerable.Empty<string>(); }
        }

        public string SelectCruiseFile()
        {
            var initialDir = AppSettings.LastOpenCruiseDir;
            if(!System.IO.Directory.Exists(initialDir))
            {
                initialDir = AppSettings.DefaultOpenCruiseDir;
            }

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = initialDir,
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
            var initialDir = AppSettings.LastOpenTemplateDir;
            if(!System.IO.Directory.Exists(initialDir))
            { initialDir = AppSettings.DefaultOpenTemplateDir; }

            var dialog = new OpenFileDialog()
            {
                InitialDirectory = initialDir,
                DefaultExt = "*.crz3t",
                Filter = "All Template Files|*.crz3t;*.cut|V3 Template File|*.crz3t| V2 Template File|*.cut",
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