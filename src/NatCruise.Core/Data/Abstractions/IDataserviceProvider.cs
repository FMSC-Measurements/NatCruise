using System;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public interface IDataserviceProvider
    {
        string CruiseFilePath { get; }

        Task OpenFileAsync(string filePath);

        void OpenFile(string filePath);

        IDataservice GetDataservice(Type type);

        T GetDataservice<T>() where T : IDataservice;
    }
}