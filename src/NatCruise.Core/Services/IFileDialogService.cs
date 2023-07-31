using NatCruise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatCruise.Services
{
    public interface IFileDialogService
    {
        Task<string> SelectCruiseFileAsync();

        Task<IEnumerable<string>> SelectCruiseFilesAsync();

        Task<string> SelectCruiseDatabaseAsync();

        Task<string> SelectCruiseFileDestinationAsync(string defaultDir = null, string defaultFileName = null, string defaultSaleFolder = null);

        Task<string> SelectBackupFileDestinationAsync(string defaultDir = null, string defaultFileName = null);

        Task<string> SelectTemplateFileDestinationAsync(string defaultDir = null, string defaultFileName = null);

        Task<string> SelectTemplateFileAsync();
    }
}