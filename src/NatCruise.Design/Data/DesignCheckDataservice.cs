using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.Data
{
    public class DesignCheckDataservice : CruiseDataserviceBase, IDesignCheckDataservice
    {
        public DesignCheckDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public IEnumerable<DesignCheck> GetDesignChecks()
        {
            return GetCuttingUnitStratumChecks()
                .Concat(GetStratumSampleGroupChecks())
                .Concat(GetStratumUnitChecks())
                .Concat(GetSampleGroupSubpopChecks())
                .Concat(GetSubPopTDVChecks());
        }

        public IEnumerable<DesignCheck> GetCuttingUnitStratumChecks()
        {
            return Database.Query<DesignCheck>(
@"SELECT
    'Cutting Unit' AS Category,
    'Error' AS Level,
    'Cutting Unit ' || cu.CuttingUnitCode || ' Has Not Been Assigned To Any Strata' AS Message,
    cu.CuttingUnitID AS RecordID
FROM CuttingUnit AS cu
LEFT JOIN CuttingUnit_Stratum AS cust USING (CruiseID, CuttingUnitCode)
WHERE cu.CruiseID = @p1
    AND cust.RowID IS NULL;
", CruiseID);
        }

        public IEnumerable<DesignCheck> GetStratumSampleGroupChecks()
        {
            return Database.Query<DesignCheck>(
@"SELECT
    'Stratum' AS Category,
    'Error' AS Level,
    'Stratum ' || st.StratumCode || ' Has No SampleGroups' AS Message,
    StratumID AS RecordID
FROM Stratum AS st
LEFT JOIN SampleGroup AS sg USING (CruiseID, StratumCode)
WHERE st.CruiseID = @p1
 AND sg.RowID IS NULL;
", CruiseID);
        }

        public IEnumerable<DesignCheck> GetStratumUnitChecks()
        {
            return Database.Query<DesignCheck>(
@"SELECT
    'Stratum' AS Category,
    'Error' AS Level,
    'Stratum ' || st.StratumCode || ' Has Not Been Assigned Any Cutting Units' AS Message,
    StratumID AS RecordID
FROM Stratum AS st
LEFT JOIN CuttingUnit_Stratum AS cust USING (CruiseID, StratumCode)
WHERE st.CruiseID = @p1
    AND cust.RowID IS NULL;
", CruiseID);
        }

        public IEnumerable<DesignCheck> GetSampleGroupSubpopChecks()
        {
            return Database.Query<DesignCheck>(
@"SELECT
    'Sample Group' AS Category,
    'Error' AS Level,
    'Sample Group ' || sg.SampleGroupCode || ' in Stratum ' || sg.StratumCode || ' Has No Subpopulations' AS Message,
    SampleGroupID AS RecordID
FROM SampleGroup AS sg
LEFT JOIN Subpopulation AS subp USING (CruiseID, StratumCode, SampleGroupCode)
WHERE sg.CruiseID = @p1
    AND subp.RowID IS NULL;
", CruiseID);
        }

        public IEnumerable<DesignCheck> GetSubPopTDVChecks()
        {
            return Database.Query<DesignCheck>(
@"SELECT
    'SubPopulation' AS Category,
    'Warning' AS Level,
    'SubPopulation ' || SpeciesCode || ' ' || LiveDead || ' in SG ' || SampleGroupCode || ' Stratum ' || StratumCode || ' Has No Applyable Tree Defaults' AS Message,
    SubPopulationID AS RecordID
FROM (
        SELECT
            subp.*,
            (SELECT TreeDefaultValue_CN FROM TreeDefaultValue AS tdv
            WHERE  tdv.CruiseID = subp.CruiseID
                AND (SpeciesCode = subp.SpeciesCode OR SpeciesCode IS NULL)
                AND (PrimaryProduct = sg.PrimaryProduct OR PrimaryProduct IS NULL)
            ORDER BY PrimaryProduct DESC, SpeciesCode DESC
            LIMIT 1
            ) AS TreeDefaultValue_CN
        FROM SubPopulation AS subp
        JOIN SampleGroup AS sg USING (CruiseID, StratumCode, SampleGroupCode)
        WHERE CruiseID = @p1
    )
WHERE TreeDefaultValue_CN IS NULL;
    ", CruiseID);
        }
    }
}