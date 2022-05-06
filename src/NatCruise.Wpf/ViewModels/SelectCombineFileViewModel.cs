using CruiseDAL.V3.Sync;
using NatCruise.Data;
using NatCruise.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NatCruise.Wpf.ViewModels
{
    public class SelectCombineFileViewModel : ViewModelBase
    {
        public SelectCombineFileViewModel(IFileDialogService fileDialogService, IDataserviceProvider dataserviceProvider)
        {
            FileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            DataserviceProvider = dataserviceProvider ?? throw new ArgumentNullException(nameof(dataserviceProvider));

            Options = new CruiseSyncOptions();

            var curFilePath = dataserviceProvider.DatabasePath;
            var curFileDir = System.IO.Path.GetDirectoryName(curFilePath);
            var curFileNameWithOutExtention = System.IO.Path.GetFileNameWithoutExtension(curFilePath);

            var defaultNewFilePath = System.IO.Path.Combine(curFileDir, curFileNameWithOutExtention + "Combined.crz3");
            NewCombineFilePath = defaultNewFilePath;

        }

        public IFileDialogService FileDialogService { get; }
        public IDataserviceProvider DataserviceProvider { get; }

        public bool UseCurrentCruiseFile { get; set; }

        public string NewCombineFilePath { get; set; }

        public ObservableCollection<CruiseFile> CruiseFiles { get; } = new ObservableCollection<CruiseFile>();

        public CruiseSyncOptions Options { get; protected set; }

        public async Task AddFile()
        {
            var path = await FileDialogService.SelectCruiseFileAsync();

            if (string.IsNullOrEmpty(path) is false && System.IO.File.Exists(path))
            {
                if (CruiseFiles.Any(x => x.FilePath == path)) { return; }
                if (UseCurrentCruiseFile)
                {
                    var curCruiseFileName = DataserviceProvider.DatabasePath;
                    if(path == curCruiseFileName) { return; }
                }

                var fileName = System.IO.Path.GetFileName(path);

                var cruiseFile = new CruiseFile
                {
                    FileName = fileName,
                    FilePath = path,
                };

                CruiseFiles.Add(cruiseFile);
            }
        }

        public void RemoveFile(CruiseFile cruiseFile)
        {
            if (cruiseFile is null) { throw new ArgumentNullException(nameof(cruiseFile)); }

            CruiseFiles.Remove(cruiseFile);
        }
    }

    public class CruiseFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}