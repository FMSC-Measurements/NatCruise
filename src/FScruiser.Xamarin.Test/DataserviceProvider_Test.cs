using CruiseDAL;
using FluentAssertions;
using FScruiser.Services;
using FScruiser.XF.Services;
using FScruiser.XF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF
{
    public class DatastoreProvider_Test : TestBase
    {
        public DatastoreProvider_Test(ITestOutputHelper output) : base(output)
        {
        }

        //[Theory]
        //[InlineData(typeof(ICuttingUnitDatastore), typeof(CuttingUnitDatastore))]
        //[InlineData(typeof(IPlotDatastore), typeof(CuttingUnitDatastore))]
        //[InlineData(typeof(ITreeDatastore), typeof(CuttingUnitDatastore))]
        //[InlineData(typeof(ISampleSelectorDataService), typeof(SampleSelectorRepository))]
        //public void Get_Test(Type typeIn, Type typeExpected)
        //{
        //    var cruiseDatastore = new CruiseDatastore_V3();
        //    var cuDatastore = new CuttingUnitDatastore(cruiseDatastore);
        //    var samplerDatastore = new SampleSelectorRepository();

        //    var datastoreProvider = new DataserviceProvider(base.App)
        //    {
        //        CruiseDatastore = cruiseDatastore,
        //        CuttingUnitDatastore = cuDatastore,
        //        SampleSelectorDataService = samplerDatastore,
        //    };

        //    var result = datastoreProvider.Get(typeIn);
        //    result.Should().BeOfType(typeExpected);
        //}
    }
}
