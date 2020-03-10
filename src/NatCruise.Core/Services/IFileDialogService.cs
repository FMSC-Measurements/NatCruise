using System.Threading.Tasks;

namespace NatCruise.Services
{
    public interface IFileDialogService
    {
        Task<string> SelectCruiseFileAsync();

        Task<string> SelectCruiseFileDestinationAsync(string defaultDir = null, string defaultFileName = null);
    }
}