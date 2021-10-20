using NatCruise.Services;

namespace NatCruise.Wpf.Services
{
    public interface IWpfApplicationSettingService : IApplicationSettingService
    {
        string LastOpenCruiseDir { get; set; }

        string LastOpenTemplateDir { get; set; }
    }
}