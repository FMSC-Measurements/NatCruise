using System.Threading.Tasks;

namespace NatCruise.Services
{
    public interface IFileDialogService
    {
        Task<string> SelectCruiseFileAsync();
    }
}