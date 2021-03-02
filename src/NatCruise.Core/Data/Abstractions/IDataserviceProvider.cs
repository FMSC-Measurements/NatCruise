using CruiseDAL;
using NatCruise.Models;
using System;

namespace NatCruise.Data
{
    public interface IDataserviceProvider
    {
        string DatabasePath { get; }

        string CruiseID { get; set; }

        void OpenDatabase(string filePath);

        CruiseDatastore_V3 GetDatabase();

        IDataservice GetDataservice(Type type);

        T GetDataservice<T>() where T : class, IDataservice;
    }
}