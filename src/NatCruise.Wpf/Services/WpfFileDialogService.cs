using Microsoft.Win32;
using NatCruise.Services;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class WpfFileDialogService : IFileDialogService
    {
        public Task<string> SelectCruiseFileAsync()
        {
            return Task.Run(SelectCruiseFile);
        }

        public string SelectCruiseFile()
        {
            var dialog = new OpenFileDialog()
            {
                DefaultExt = "*.crz3",
                Filter = "All V3 file types|*.crz3;*.crz3t;|" +
                         "cruise files V3 (*.crz3)|*.crz3|" +
                         "template file V3 (*.crz3t)|*.crz3t|" +
                         "cruise file V2 (*.cruise)|*.cruise",
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
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
                return dialog.FileName;
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
            // TODO after full transition to crz3t, change default extention to crz3t
            var dialog = new OpenFileDialog()
            {
                DefaultExt = "*.cut",
                Filter = "V3 Template File (*.crz3t)|*.crz3t| V2 Template File (*.cut)|*.cut",
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
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
            var dialog = new SaveFileDialog()
            {
                DefaultExt = "*.crzt3",
                Filter = "cruise v3 template files (*.crz3t)|*.crz3t",
                AddExtension = true,
                FileName = defaultFileName,
                InitialDirectory = defaultDir,
                OverwritePrompt = true,
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}