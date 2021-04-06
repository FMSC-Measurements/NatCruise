using CruiseDAL;
using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public class SubpopulationDataservice : CruiseDataserviceBase, ISubpopulationDataservice
    {
        public SubpopulationDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public SubpopulationDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        public void AddSubpopulation(Subpopulation subpopulation)
        {
            Database.Execute("INSERT OR IGNORE INTO SpeciesCode (Species) VALUES (@p1);", subpopulation.SpeciesCode);

            Database.Execute2(
                "INSERT INTO Subpopulation (" +
                    "CruiseID," +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "SpeciesCode, " +
                    "LiveDead," +
                    "CreatedBy" +
                ") VALUES ( " +
                    "@CruiseID," +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@SpeciesCode, " +
                    "@LiveDead," +
                    "@DeviceID" +
                ");", new
                {
                    CruiseID,
                    subpopulation.StratumCode,
                    subpopulation.SampleGroupCode,
                    subpopulation.SpeciesCode,
                    subpopulation.LiveDead,
                    DeviceID,
                });
        }

        public void DeleteSubpopulation(Subpopulation subpopulation)
        {
            Database.Execute("DELETE FROM Subpopulation WHERE Subpopulation_CN = @p1;", subpopulation.Subpopulation_CN);
        }

        public bool Exists(string stratumCode, string sampleGroupCode, string species, string livedead)
        {
            return Database.ExecuteScalar<int>(
                "SELECT count(*) FROM Subpopulation " +
                "WHERE StratumCode = @p1 AND SampleGroupCode = @p2 " +
                "AND ifNull(SpeciesCode, '') = ifNull(@p3, '') " +
                "AND ifNull(LiveDead, '') = ifNull(@p4, '')" +
                "AND CruiseID = @p5;",
                stratumCode, sampleGroupCode, species, livedead, CruiseID) > 0;
        }

        public IEnumerable<Subpopulation> GetSubpopulations(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<Subpopulation>("SELECT sp.* FROM SubPopulation AS sp WHERE StratumCode = @p1 AND SampleGroupCode = @p2 AND CruiseID = @p3;",
                stratumCode, sampleGroupCode, CruiseID);
        }

        public bool HasTreeCounts(string stratumCode, string sampleGroupCode, string species, string livedead)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger WHERE StratumCode = @p1 AND SampleGroupCode = @p2 AND SpeciesCode = @p3 AND LiveDead = @p4 AND CruiseID = @p4;",
                stratumCode, sampleGroupCode, species, livedead, CruiseID) > 0;
        }

        public void UpdateSubpopulation(Subpopulation subpopulation)
        {
            Database.Update(subpopulation);
        }
    }
}