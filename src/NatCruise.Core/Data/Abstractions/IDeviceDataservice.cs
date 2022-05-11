using NatCruise.Models;
using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IDeviceDataservice : IDataservice
    {
        IEnumerable<Device> GetDevices();

        IEnumerable<Device> GetOtherDevices();
    }
}