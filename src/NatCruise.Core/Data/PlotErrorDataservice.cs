using CruiseDAL;
using NatCruise.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class PlotErrorDataservice : CruiseDataserviceBase, IPlotErrorDataservice
    {
        public PlotErrorDataservice(IDataContextService dataContext) : base(dataContext)
        {
        }

        public PlotErrorDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public PlotErrorDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public IEnumerable<PlotError> GetPlotErrors(string unit, int plotNumber)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.CuttingUnitCode = @p1 " +
                "AND pe.PlotNumber = @p2" +
                "AND pe.CruiseID =  @p3;",
                unit, plotNumber, CruiseID).ToArray();
        }

        public IEnumerable<PlotError> GetPlotErrors(string plotID)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.PlotID = @p1;",
                plotID).ToArray();
        }
    }
}