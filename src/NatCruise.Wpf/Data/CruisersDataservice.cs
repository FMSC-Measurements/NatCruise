using CruiseDAL;
using NatCruise.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Wpf.Data
{
    public class CruisersDataservice : CruiseDataserviceBase, ICruisersDataservice
    {
        public CruisersDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public CruisersDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public bool PromptCruiserOnSample { get => false; set => throw new NotSupportedException(); }

        public void AddCruiser(string cruiser)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<string> GetCruisers()
        {
            return Database.QueryScalar2<string>(
@"SELECT DISTINCT Initials FROM TreeMeasurment JOIN Tree USING (TreeID) WHERE CruiseID = @CruiseID
UNION
SELECT DISTINCT Signature FROM TallyLedger WHERE CruiseID = @CruiseID;", new { CruiseID }).ToArray();
        }

        public void RemoveCruiser(string cruiser)
        {
            throw new NotImplementedException();
        }
    }
}