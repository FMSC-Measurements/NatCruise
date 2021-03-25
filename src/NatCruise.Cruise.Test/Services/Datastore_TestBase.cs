using CruiseDAL;
using CruiseDAL.V3.Models;
using NatCruise.Test;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Services
{
    public class Datastore_TestBase : TestBase
    {
        public string CruiseID => Initializer.CruiseID;
        public string SaleID => Initializer.SaleID;

        protected string[] Units => Initializer.Units;
        protected Stratum[] Strata => Initializer.Strata;
        protected CuttingUnit_Stratum[] UnitStrata => Initializer.UnitStrata;
        protected string[] Species => Initializer.Species;
        protected SampleGroup[] SampleGroups => Initializer.SampleGroups;
        protected TreeDefaultValue[] TreeDefaults => Initializer.TreeDefaults;
        protected SubPopulation[] Subpops => Initializer.Subpops;
        public Stratum[] PlotStrata => Initializer.PlotStrata;
        public Stratum[] NonPlotStrata => Initializer.NonPlotStrata;

        protected DatastoreInitializer Initializer { get; }

        public Datastore_TestBase(ITestOutputHelper output) : base(output)
        {
            Initializer = new DatastoreInitializer();
        }

        protected CruiseDatastore_V3 CreateDatabase()
        {
            return Initializer.CreateDatabase();
        }

        protected CruiseDatastore_V3 CreateDatabase(string path, string cruiseID = null, string saleID = null)
        {
            return Initializer.CreateDatabase(path, cruiseID, saleID);
        }

        protected void InitializeDatabase(CruiseDatastore_V3 db,
            string cruiseID,
            string saleID,
            string[] units,
            CruiseDAL.V3.Models.Stratum[] strata,
            CruiseDAL.V3.Models.CuttingUnit_Stratum[] unitStrata,
            CruiseDAL.V3.Models.SampleGroup[] sampleGroups,
            string[] species,
            CruiseDAL.V3.Models.TreeDefaultValue[] tdvs,
            CruiseDAL.V3.Models.SubPopulation[] subPops)
        {
            DatastoreInitializer.InitializeDatabase(db, Initializer.DeviceID, cruiseID, saleID, units, strata, unitStrata, sampleGroups, species, tdvs, subPops);
        }
    }
}