using Microsoft.Win32;

namespace NatCruise.Wpf.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string SelectCruiseFile()
        {
            var dialog = new OpenFileDialog()
            {
                DefaultExt = "*.crz3",
                Filter = "cruise v3 files (*.crz3)|*.crz3|" +
                            "cruise file v2 (*.cruise)|*.cruise|" +
                            "All cruise file types|*.cruise;*.crz3",
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }
            else
            { return null; }
        }

        //public string SelectCruiseFileDestination()
        //{
        //    return SelectCruiseFileDestination((string)null);
        //}

        public string SelectCruiseFileDestination(string defaultDir = null, string defaultFileName = null)
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExt = "*.crz3",
                Filter = "cruise v3 files (*.crz3)|*.crz3|" +
                            "cruise file v2 (*.cruise)|*.cruise",
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