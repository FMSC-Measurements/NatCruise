using NatCruise.Services;
using System.ComponentModel;

namespace NatCruise.Wpf.Services
{
    public interface IWpfApplicationSettingService : IApplicationSettingService
    {
        string LastOpenCruiseDir { get; set; }
        string LastOpenTemplateDir { get; set; }
        string DefaultOpenCruiseDir { get; }
        string DefaultOpenTemplateDir { get; }

        bool IsSuperuserMode { get; set; }
    }
}