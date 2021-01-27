using NatCruise.Models;
using System;

namespace NatCruise.Data
{
    public interface IDataserviceProvider
    {
        string DatabasePath { get; }

        Cruise Cruise { get; }

        string CruiseID { get; }

        void OpenDatabase(string filePath);

        IDataservice GetDataservice(Type type);

        T GetDataservice<T>() where T : IDataservice;
    }
}