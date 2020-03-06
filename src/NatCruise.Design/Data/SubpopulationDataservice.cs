using CruiseDAL;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Design.Data
{
    public class SubpopulationDataservice : ISubpopulationDataservice
    {
        public SubpopulationDataservice(string path)
        {
            Database = new CruiseDatastore(path);
        }

        protected CruiseDatastore Database { get; }

        public void AddSubpopulation(Subpopulation subpopulation)
        {
            Database.Execute("INSERT OR IGNORE INTO SpeciesCode (Species) VALUES (@p1);", subpopulation.Species);

            Database.Execute2(
                "INSERT INTO Subpopulation (" +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead" +
                ") VALUES ( " +
                "@StratumCode, " +
                "@SampleGroupCode, " +
                "@Species, " +
                "@LiveDead" +
                ");", subpopulation);
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
                "AND ifNull(Species, '') = ifNull(@p3, '') " +
                "AND ifNull(LiveDead, '') = ifNull(@p4, '');",
                stratumCode, sampleGroupCode, species, livedead) > 0;
        }

        public IEnumerable<Subpopulation> GetSubpopulations(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<Subpopulation>("SELECT sp.* FROM SubPopulation AS sp WHERE StratumCode = @p1 AND SampleGroupCode = @p2;",
                stratumCode, sampleGroupCode);
        }

        public bool HasTreeCounts(string stratumCode, string sampleGroupCode, string species, string livedead)
        {
            return Database.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger WHERE StratumCode = @p1 AND SampleGroupCode = @p2 AND Species = @p3 AND LiveDead = @p4;",
                stratumCode, sampleGroupCode, species, livedead) > 0;
        }

        public void UpdateSubpopulation(Subpopulation subpopulation)
        {
            Database.Update(subpopulation);
        }
    }
}