using System;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Data
{
    public interface IDataserviceProvider
    {
        string CruiseFilePath { get; }

        Task OpenFileAsync(string filePath);

        void OpenFile(string filePath);

        object GetDataservice(Type type);

        T GetDataservice<T>();
    }
}