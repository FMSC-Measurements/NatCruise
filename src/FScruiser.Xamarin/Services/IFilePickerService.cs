using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public interface IFilePickerService
    {
        Task<string> PickCruiseFileAsync();
    }
}