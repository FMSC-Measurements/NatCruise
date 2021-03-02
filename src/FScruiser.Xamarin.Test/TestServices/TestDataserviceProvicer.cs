using CruiseDAL;
using Moq;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.TestServices
{
    public class TestDataserviceProvicer : IDataserviceProvider
    {
        public string DatabasePath => throw new NotImplementedException();

        public string CruiseID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CruiseDatastore_V3 GetDatabase()
        {
            throw new NotImplementedException();
        }

        public IDataservice GetDataservice(Type type)
        {
            throw new NotImplementedException();
        }

        public T GetDataservice<T>() where T : class, IDataservice
        {
            var mock = new Mock<T>();
            return mock.Object;
        }

        public void OpenDatabase(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
