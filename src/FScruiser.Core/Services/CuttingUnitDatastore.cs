using CruiseDAL;
using CruiseDAL.Schema;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public partial class CuttingUnitDatastore : ICuttingUnitDatastore
    {
        private const int NUMBER_OF_TALLY_ENTRIES_PERPAGE = 20;

        protected readonly string PLOT_METHODS = String.Join(", ", CruiseMethods.PLOT_METHODS
            .Append(CruiseMethods.THREEPPNT)
            .Append(CruiseMethods.FIXCNT)
            .Select(x => "'" + x + "'").ToArray());

        private CruiseDatastore_V3 _database;

        public CruiseDatastore_V3 Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnDatabaseChanged();
            }
        }

        protected string CurrentTimeStamp => DateTime.Now.ToString();
        protected string UserName => "AndroidUser";

        public CuttingUnitDatastore(string path)
        {
            var database = new CruiseDatastore_V3(path ?? throw new ArgumentNullException(nameof(path)));

            Database = database;
        }

        public CuttingUnitDatastore(CruiseDatastore_V3 database)
        {
            Database = database;
        }

        private void OnDatabaseChanged()
        {
            var database = Database;
            if (database == null) { return; }

            //DatabaseUpdater.Update(database);
        }

        public string GetCruisePurpose()
        {
            return Database.ExecuteScalar<string>("SELECT Purpose FROM Sale LIMIT 1;");
        }

        #region units

        public CuttingUnit_Ex GetUnit(string code)
        {
            var unit = Database.From<CuttingUnit_Ex>()
                .Where("Code = @p1")
                .Query(code).FirstOrDefault();

            unit.HasPlotStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit_Stratum AS cust " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "WHERE cust.CuttingUnitCode = @p1 " +
                $"AND st.Method IN ({PLOT_METHODS});", code) > 0;

            unit.HasTreeStrata = Database.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit_Stratum AS [cust] " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "WHERE cust.CuttingUnitCode = @p1 " +
                $"AND st.Method NOT IN ({PLOT_METHODS});", code) > 0;

            return unit;
        }

        public IEnumerable<CuttingUnit> GetUnits()
        {
            return Database.From<CuttingUnit>()
                .Query().ToArray();
        }

        #endregion units

        #region strata

        public string GetCruiseMethod(string stratumCode)
        {
            return Database.ExecuteScalar<string>("SELECT Method FROM Stratum WHERE Code = @p1;", stratumCode);
        }

        public IEnumerable<string> GetStratumCodesByUnit(string unitCode)
        {
            var stratumCodes = Database.ExecuteScalar<string>(
                "SELECT group_concat(StratumCode, ',') FROM CuttingUnit_Stratum " +
                "WHERE CuttingUnitCode = @p1;", unitCode);

            return stratumCodes.Split(',');
        }

        public IEnumerable<Stratum> GetStrataByUnitCode(string unitCode)
        {
            return Database.Query<Stratum>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method NOT IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        public IEnumerable<StratumProxy> GetStrataProxiesByUnitCode(string unitCode)
        {
            return Database.Query<StratumProxy>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method NOT IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        public IEnumerable<StratumProxy> GetPlotStrataProxies(string unitCode)
        {
            return Database.Query<StratumProxy>(
                "SELECT " +
                "st.* " +
                "FROM Stratum AS st " +
                "JOIN CuttingUnit_Stratum AS cust ON st.Code = cust.StratumCode " +
                $"WHERE CuttingUnitCode = @p1 AND st.Method IN ({PLOT_METHODS})",
                new object[] { unitCode })
                .ToArray();
        }

        #endregion strata

        #region sampleGroup

        //public IEnumerable<SampleGroup> GetSampleGroupsByUnitCode(string unitCode)
        //{
        //    return Database.From<SampleGroup>()
        //        .Join("Stratum", "USING (Stratum_CN)")
        //        .Join("CuttingUnitStratum", "USING (Stratum_CN)")
        //        .Join("CuttingUnit", "USING (CuttingUnit_CN)")
        //        .Where("CuttingUnit.Code = @p1").Query(unitCode).ToArray();
        //}

        public IEnumerable<string> GetSampleGroupCodes(string stratumCode)
        {
            var sgCode = Database.ExecuteScalar<string>("SELECT group_concat(SampleGroupCode,',') FROM SampleGroup_V3 " +
                "WHERE StratumCode = @p1;", stratumCode);

            return sgCode.Split(',');
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxiesByUnit(string unitCode)
        {
            throw new NotImplementedException();
        }

        public SampleGroup GetSampleGroup(string stratumCode, string sgCode)
        {
            return Database.Query<SampleGroup>(
                "SELECT " +
                    "sg.*, " +
                    "st.Method AS StratumMethod " +
                "FROM SampleGroup_V3 AS sg " +
                "JOIN Stratum AS st ON sg.StratumCode = st.Code " +
                "WHERE sg.StratumCode = @p1 AND sg.SampleGroupCode = @p2;",
                new object[] { stratumCode, sgCode })
                .FirstOrDefault();
        }

        public IEnumerable<SampleGroupProxy> GetSampleGroupProxies(string stratumCode)
        {
            return Database.From<SampleGroupProxy>()
                .Where("StratumCode = @p1")
                .Query(stratumCode);
        }

        public SampleGroupProxy GetSampleGroupProxy(string stratumCode, string sampleGroupCode)
        {
            return Database.From<SampleGroupProxy>()
                .Where("StratumCode = @p1 AND SampleGroupCode = @p2")
                .Query(stratumCode, sampleGroupCode).FirstOrDefault();
        }

        public SamplerInfo GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SamplerInfo>(
                "SELECT " +
                    "sg.StratumCode," +
                    "sg.SampleGroupCode, " +
                    "st.Method, " +
                    "sg.SamplingFrequency, " +
                    "sg.InsuranceFrequency, " +
                    "sg.KZ, " +
                    "ss.SampleSelectorState, " +
                    "ss.SampleSelectorType " +
                "FROM SampleGroup_V3 AS sg " +
                "JOIN Stratum AS st ON sg.StratumCode = st.Code " +
                "LEFT JOIN SamplerState AS ss USING (StratumCode, SampleGroupCode) " +
                "WHERE sg.StratumCode = @p1 " +
                "AND sg.SampleGroupCode = @p2;",
                new object[] { stratumCode, sampleGroupCode }).FirstOrDefault();
        }

        public void UpdateSamplerState(SamplerInfo samplerState)
        {
            Database.Execute2(
                "INSERT INTO SamplerState ( " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "SampleSelectorType, " +
                    "SampleSelectorState " +
                ") VALUES ( " +
                    "@StratumCode, " +
                    "@SampleGroupCode, " +
                    "@SampleSelectorType, " +
                    "@SampleSelectorState" +
                ") ON CONFLICT (StratumCode, SampleGroupCode) " +
                "DO UPDATE SET " +
                    "SampleSelectorType = @SampleSelectorType, " +
                    "SampleSelectorState = @SampleSelectorState " +
                "WHERE StratumCode = @StratumCode " +
                "AND SampleGroupCode = @SampleGroupCode;",
                samplerState);
        }

        //private string SELECT_TALLYPOPULATION_CORE =
        //    "WITH tallyPopTreeCounts AS (" +
        //        "SELECT CuttingUnitCode, " +
        //            "StratumCode, " +
        //            "SampleGroupCode, " +
        //            "Species, " +
        //            "LiveDead, " +
        //            "sum(TreeCount) AS TreeCount, " +
        //            "sum(KPI) AS SumKPI " +
        //        "FROM TallyLedger AS tl " +
        //        "GROUP BY " +
        //            "CuttingUnitCode, " +
        //            "StratumCode, " +
        //            "SampleGroupCode, " +
        //            "ifnull(Species, ''), " +
        //            "ifnull(LiveDead, ''))\r\n" +

        //        "SELECT " +
        //            "tp.Description, " +
        //            "tp.StratumCode, " +
        //            "st.Method AS StratumMethod, " +
        //            "tp.SampleGroupCode, " +
        //            "tp.Species, " +
        //            "tp.LiveDead, " +
        //            "tp.HotKey, " +
        //            "ifnull(tl.TreeCount, 0) AS TreeCount, " +
        //            "ifnull(tl.SumKPI, 0) AS SumKPI, " +
        //            //"sum(tl.KPI) SumKPI, " +
        //            "sg.SamplingFrequency AS Frequency, " +
        //            "sg.MinKPI AS sgMinKPI, " +
        //            "sg.MaxKPI AS sgMaxKPI, " +
        //            "sg.UseExternalSampler " +
        //            //$"ss.SampleSelectorType == '{CruiseMethods.CLICKER_SAMPLER_TYPE}' AS IsClickerTally " +
        //        "FROM TallyPopulation AS tp " +
        //        "JOIN SampleGroup_V3 AS sg USING (StratumCode, SampleGroupCode) " +
        //        //"Left JOIN SamplerState ss USING (StratumCode, SampleGroupCode) " +
        //        "JOIN Stratum AS st ON tp.StratumCode = st.Code " +
        //        "JOIN CuttingUnit_Stratum AS cust ON tp.StratumCode = cust.StratumCode AND cust.CuttingUnitCode = @p1 " +
        //        "LEFT JOIN tallyPopTreeCounts AS tl " +
        //            "ON tl.CuttingUnitCode = @p1 " +
        //            "AND tp.StratumCode = tl.StratumCode " +
        //            "AND tp.SampleGroupCode = tl.SampleGroupCode " +
        //            "AND ifnull(tp.Species, '') = ifnull(tl.Species, '') " +
        //            "AND ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, '') ";

        //public IEnumerable<TallyPopulation> GetTallyPopulationsByUnitCode(string unitCode)
        //{
        //    return Database.Query<TallyPopulation>(
        //        SELECT_TALLYPOPULATION_CORE +
        //        $"WHERE st.Method NOT IN ({PLOT_METHODS})"
        //        , new object[] { unitCode }).ToArray();

        //    //return Database.From<TallyPopulation>()
        //    //    .Join("Tally", "USING (Tally_CN)")
        //    //    .Join("SampleGroup", "USING (SampleGroup_CN)")
        //    //    .LeftJoin("TreeDefaultValue", "USING (TreeDefaultValue_CN)")
        //    //    .Join("Stratum", "USING (Stratum_CN)")
        //    //    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
        //    //    .Where($"CuttingUnit.Code = @p1 AND Stratum.Method NOT IN ({PLOT_METHODS})")
        //    //    .Query(unitCode).ToArray();
        //}

        //public TallyPopulation GetTallyPopulation(string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead)
        //{
        //    //var tPops = Database.QueryGeneric(
        //    //    "SELECT * FROM TallyPopulation AS tp " +
        //    //    "WHERE tp.StratumCode = @p2 " +
        //    //        "AND tp.SampleGroupCode = @p3 " +
        //    //        "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
        //    //        "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
        //    //    , new  { p1 = unitCode, p2 = stratumCode, p3= sampleGroupCode, p4 = species, p5 = liveDead }).ToArray();

        //    return Database.Query<TallyPopulation>(
        //        SELECT_TALLYPOPULATION_CORE +
        //        "WHERE tp.StratumCode = @p2 " +
        //            "AND tp.SampleGroupCode = @p3 " +
        //            "AND ifNull(tp.Species, '') = ifNull(@p4,'') " +
        //            "AND ifNull(tp.LiveDead, '') = ifNull(@p5,'')"
        //        , new object[] { unitCode, stratumCode, sampleGroupCode, species, liveDead }).FirstOrDefault();
        //}

        //private class SampleGroupStratumInfo
        //{
        //    [Field("SgCode")]
        //    public string SgCode { get; set; }

        //    [Field("StCode")]
        //    public string StCode { get; set; }

        //    [Field("Method")]
        //    public string Method { get; set; }
        //}

        //public IEnumerable<TallyPopulation_Plot> GetPlotTallyPopulationsByUnitCode(string unitCode, int plotNumber)
        //{
        //    var tallyPops = Database.Query<TallyPopulation_Plot>(
        //        SELECT_TALLYPOPULATION_CORE +
        //        $"WHERE st.Method IN ({PLOT_METHODS})"
        //        , new object[] { unitCode }).ToArray();

        //    foreach (var pop in tallyPops)
        //    {
        //        pop.InCruise = GetIsTallyPopInCruise(unitCode, plotNumber, pop.StratumCode);
        //        pop.IsEmpty = Database.ExecuteScalar<int>("SELECT ifnull(IsEmpty, 0) FROM Plot_Stratum " +
        //            "WHERE CuttingUnitCode = @p1 AND PlotNumber = @p2 AND StratumCode = @p3;",
        //            unitCode, plotNumber, pop.StratumCode) == 1;
        //    }

        //    return tallyPops;
        //}

        //private bool GetIsTallyPopInCruise(string unitCode, int plotNumber, string stratumCode)
        //{
        //    return Database.ExecuteScalar<bool?>(
        //        "SELECT EXISTS (" +
        //            "SELECT * " +
        //            "FROM Plot_Stratum " +
        //            "WHERE StratumCode = @p1 " +
        //                "AND CuttingUnitCode = @p2 " +
        //                "AND PlotNumber = @p3);",
        //        stratumCode, unitCode, plotNumber) ?? false;
        //}

        #endregion sampleGroup

        #region subpopulation

        public IEnumerable<SubPopulation> GetSubPopulations(string stratumCode, string sampleGroupCode)
        {
            return Database.Query<SubPopulation>("SELECT * FROM SubPopulation " +
                "WHERE StratumCode = @p1 AND SampleGroupCode = @p2;", stratumCode, sampleGroupCode).ToArray();
        }

        #endregion subpopulation

        #region TreeFields

        //public IEnumerable<TreeFieldSetup> GetTreeFieldsByUnitCode(string unitCode)
        //{
        //    return Database.Query<TreeFieldSetup>(
        //        "SELECT " +
        //        "Field, " +
        //        "Heading, " +
        //        "min(FieldOrder) AS FieldOrder " +
        //        "FROM TreeFieldSetup_V3 AS tfs " +
        //        "JOIN CuttingUnit_Stratum AS cust USING (StratumCode) " +
        //        "WHERE CuttingUnitCode = @p1 AND min(FieldOrder) >= 0 " +
        //        "GROUP BY CuttingUnitCode, Field " +
        //        "ORDER BY min(FieldOrder);",
        //        new object[] { unitCode }).ToArray();
        //}

        public IEnumerable<TreeFieldSetup> GetTreeFieldsByStratumCode(string stratumCode)
        {
            return Database.Query<TreeFieldSetup>(
                "SELECT " +
                "Field, " +
                "Heading, " +
                "FieldOrder " +
                "FROM TreeFieldSetup_V3 AS tfs " +
                "WHERE StratumCode = @p1 AND FieldOrder >= 0 " +
                "GROUP BY Field " +
                "ORDER BY FieldOrder;",
                new object[] { stratumCode }).ToArray();
        }

        #endregion TreeFields

        #region Tree

        public IEnumerable<Tree> GetTreesByUnitCode(string unitCode)
        {
            return Database.Query<Tree_Ex>(
                "SELECT t.*, tm.* FROM Tree AS t " +
                "LEFT JOIN TreeMeasurment AS tm USING (TreeID) " +
                "JOIN CuttingUnit AS cu ON cu.Code = t.CuttingUnitCode " +
                "WHERE CuttingUnit.Code = @p1 AND PlotNumber IS NULL",
                unitCode).ToArray();
        }

        public TreeStub GetTreeStub(string treeID)
        {
            return QueryTreeStub()
                .Where("TreeID = @p1")
                .Query(treeID).FirstOrDefault();
        }

        private IQuerryAcceptsJoin<TreeStub> QueryTreeStub()
        {
            return Database.From<TreeStub>()
                .LeftJoin("TreeMeasurment", "USING (TreeID)");
        }

        public IEnumerable<TreeStub> GetTreeStubsByUnitCode(string unitCode)
        {
            return QueryTreeStub()
                .Where("CuttingUnitCode = @p1 AND PlotNumber IS NULL")
                .Query(unitCode);
        }

        //private Guid CreateTree(IDbConnection conn, IDbTransaction trans, string unitCode, string stratumCode, string sampleGroupCode, string species, string liveDead, string countMeasure, int treeCount = 1, int kpi = 0, bool stm = false)
        //{
        //    var tree_guid = Guid.NewGuid();
        //    return Database.ExecuteScalar<Guid>(conn, CREATE_TREE_COMMAND
        //        //+"SELECT Tree_GUID FROM Tree WHERE Tree_CN == last_insert_rowid();"
        //        , new object[] { tree_guid,
        //        unitCode,
        //        stratumCode,
        //        sampleGroupCode,
        //        species,
        //        liveDead,
        //        countMeasure,
        //        treeCount,
        //        kpi,
        //        (stm) ? "Y" : "N"},
        //        trans);
        //}

        //public int GetNextTreeNumber(string unitCode)
        //{
        //    return Database.ExecuteScalar<int>("SELECT max(TreeNumber) + 1 FROM Tree JOIN CuttingUnit USING (CuttingUnit_CN) WHERE CuttingUnit.Code = @p1 AND Plot_CN IS NULL;", unitCode);
        //}

        //public void InsertTree(Tree tree)
        //{
        //    if (tree.IsPersisted == true) { throw new InvalidOperationException("tree is persisted, should be calling update instead of insert"); }
        //    Database.Insert(tree);
        //}

        //public Task InsertTreeAsync(Tree tree)
        //{
        //    return Task.Run(() => InsertTree(tree));
        //}

        #endregion Tree

        #region Tree Audits and ErrorLog

        public IEnumerable<TreeError> GetTreeErrorsByUnit(string cuttingUnitCode)
        {
            return Database.Query<TreeError>(
                "SELECT " +
                "te.TreeID, " +
                "te.Field, " +
                "te.Level, " +
                "te.Message, " +
                "te.Resolution " +
                "FROM TreeError AS te " +
                "JOIN Tree_V3 AS t USING (TreeID) " +
                "WHERE t.CuttingUnitCode = @p1;",
                new object[] { cuttingUnitCode }).ToArray();
        }

        public IEnumerable<PlotError> GetPlotErrorsByUnit(string cuttingUnitCode)
        {
            return Database.Query<PlotError>("SELECT * FROM PlotError AS pe " +
                "WHERE pe.CuttingUnitCode = @p1;",
                cuttingUnitCode).ToArray();
        }

       

        //public IEnumerable<TreeAuditRule> GetTreeAuditRules(string stratum, string sampleGroup, string species, string livedead)
        //{
        //    return Database.Query<TreeAuditRule>("SELECT * FROM TreeAuditValue " +
        //        "JOIN TreeDefaultValueTreeAuditValue USING (TreeAuditValue_CN) " +
        //        "JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
        //        "JOIN SampleGroup ON TDV.PrimaryProduct = SampleGroup.PrimaryProduct " +
        //        "JOIN Stratum USING (Stratum_CN) " +
        //        "WHERE Stratum.Code = @p1 " +
        //        "AND SampleGroup.Code = @p2 " +
        //        "AND TDV.Species = @p3 " +
        //        "AND TDV.LiveDead = @p4;", new object[] { stratum, sampleGroup, species, livedead });
        //}

        //public void UpdateTreeErrors(string tree_GUID, IEnumerable<ValidationError> errors)
        //{
        //    Database.Execute("DELETE FROM ErrorLog WHERE TableName = 'Tree' " +
        //        "AND CN_Number = (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1) " +
        //        "AND Suppress = 0;", tree_GUID);

        //    foreach (var error in errors)
        //    {
        //        Database.Execute("INSERT OR IGNORE INTO ErrorLog (TableName, CN_Number, ColumnName, Level, Message, Program) " +
        //            "VALUES ('Tree', (SELECT Tree_CN FROM Tree WHERE Tree_GUID = @p1), @p2, @p3, @p4, 'FScruiser');",
        //            tree_GUID,
        //            error.Property,
        //            error.Level.ToString(),
        //            error.Message);
        //    }
        //}

        #endregion Tree Audits and ErrorLog

        #region Tally Entry

//        const string QUERY_TALLYENTRY_BASE =
//            "SELECT " +
//                    "tl.TreeID, " +
//                    "tl.TallyLedgerID, " +
//                    "tl.CuttingUnitCode, " +
//                    "tl.StratumCode, " +
//                    "tl.SampleGroupCode, " +
//                    "tl.Species, " +
//                    "tl.LiveDead, " +
//                    "tl.TreeCount, " +
//                    "tl.Reason, " +
//                    "tl.KPI, " +
//                    "tl.EntryType, " +
//                    "tl.Remarks, " +
//                    "tl.Signature, " +
//                    "tl.CreatedDate, " +
//                    "t.TreeNumber, " +
//                    "t.CountOrMeasure, " +
//                    "tl.STM, " +
//                    "(SELECT count(*) FROM TreeError AS te WHERE tl.TreeID IS NOT NULL AND Level = 'E' AND te.TreeID = tl.TreeID AND Resolution IS NULL) AS ErrorCount, " +
//                    "(SELECT count(*) FROM TreeError AS te WHERE tl.TreeID IS NOT NULL AND Level = 'W' AND te.TreeID = tl.TreeID AND Resolution IS NULL) AS WarningCount " +
//                "FROM TallyLedger AS tl " +
//                "LEFT JOIN Tree_V3 AS t USING (TreeID) ";

//        public TallyEntry GetTallyEntry(string tallyLedgerID)
//        {
//            return Database.Query<TallyEntry>(
//                QUERY_TALLYENTRY_BASE +
//                "WHERE tl.TallyLedgerID = @p1;",
//                new object[] { tallyLedgerID })
//                .FirstOrDefault();
//        }

//        public IEnumerable<TallyEntry> GetTallyEntriesByUnitCode(string unitCode)
//        {
//            return Database.Query<TallyEntry>(
//                QUERY_TALLYENTRY_BASE +
//                "WHERE tl.CuttingUnitCode = @p1 AND tl.PlotNumber IS NULL " +
//                "ORDER BY tl.CreatedDate DESC;",
//                new object[] { unitCode })
//                .ToArray();

//            //From<TallyEntry>()
//            ////.Where("UnitCode = @p1 AND PlotNumber IS NULL ")
//            //.Where("UnitCode = @p1")
//            //.OrderBy("TimeStamp DESC")
//            //.Limit(NUMBER_OF_TALLY_ENTRIES_PERPAGE, 0 * NUMBER_OF_TALLY_ENTRIES_PERPAGE)
//            //.Query(unitCode);
//        }

//        public IEnumerable<TallyEntry> GetTallyEntries(string unitCode, int plotNumber)
//        {
//            return Database.Query<TallyEntry>(
//                QUERY_TALLYENTRY_BASE +
//                "WHERE tl.CuttingUnitCode = @p1" +
//                "AND tl.PolotNumber = @p2;",
//                new object[] { unitCode, plotNumber })
//                .ToArray();

//            //return Database.From<TallyEntry>()
//            //    .LeftJoin("Tree", "USING (Tree_GUID)")
//            //    .Where("UnitCode = @p1 AND PlotNumber = @p2 ")
//            //    .OrderBy("TimeStamp DESC")
//            //    .Query(unitCode, plotNumber);
//        }

//        public void InsertTallyLedger(TallyLedger tallyLedger)
//        {
//            var tallyLedgerID = tallyLedger.TallyLedgerID ?? Guid.NewGuid().ToString();

//            Database.Execute2(
//                "INSERT INTO TallyLedger (" +
//                    "TallyLedgerID, " +
//                    "CuttingUnitCode, " +
//                    "StratumCode, " +
//                    "SampleGroupCode, " +
//                    "PlotNumber, " +
//                    "Species, " +
//                    "LiveDead," +
//                    "TreeCount, " +
//                    "KPI, " +
//                    "ThreePRandomValue, " +
//                    "TreeID, " +
//                    "CreatedBy, " +
//                    "Reason, " +
//                    "Signature, " +
//                    "Remarks, " +
//                    "EntryType" +
//                ") VALUES ( " +
//                    "@TallyLedgerID, " +
//                    "@CuttingUnitCode, " +
//                    "@StratumCode, " +
//                    "@SampleGroupCode, " +
//                    "@PlotNumber, " +
//                    "@Species, " +
//                    "@LiveDead, " +
//                    "@TreeCount, " +
//                    "@KPI, " +
//                    "@ThreePRandomValue, " +
//                    "@TreeID, " +
//                    "@CreatedBy, " +
//                    "@Reason, " +
//                    "@Signature, " +
//                    "@Remarks, " +
//                    "@EntryType" +
//                ");",
//                new
//                {
//                    TallyLedgerID = tallyLedgerID,
//                    tallyLedger.CuttingUnitCode,
//                    tallyLedger.StratumCode,
//                    tallyLedger.SampleGroupCode,
//                    tallyLedger.PlotNumber,
//                    tallyLedger.Species,
//                    tallyLedger.LiveDead,
//                    tallyLedger.TreeCount,
//                    tallyLedger.KPI,
//                    tallyLedger.ThreePRandomValue,
//                    tallyLedger.TreeID,
//                    tallyLedger.CreatedBy,
//                    tallyLedger.Reason,
//                    tallyLedger.Signature,
//                    tallyLedger.Remarks,
//                    tallyLedger.EntryType,
//                });

//            tallyLedger.TallyLedgerID = tallyLedgerID;
//        }

//        public Task<TallyEntry> InsertTallyActionAsync(TallyAction tallyAction)
//        {
//            return Task.Factory.StartNew(() => InsertTallyAction(tallyAction));
//        }

//        public TallyEntry InsertTallyAction(TallyAction atn)
//        {
//            if (atn.IsInsuranceSample == true && atn.IsSample == false) { throw new InvalidOperationException("If action is insurance sample it must be sample aswell"); }

//            Database.BeginTransaction();
//            try
//            {
//                var tallyEntry = new TallyEntry(atn);

//                tallyEntry.TallyLedgerID = Guid.NewGuid().ToString();

//                if (atn.IsSample)
//                {
//                    tallyEntry.TreeID = tallyEntry.TallyLedgerID;

//                    tallyEntry.TreeNumber = Database.ExecuteScalar2<int>(
//                        "SELECT " +
//                        "ifnull(max(TreeNumber), 0) + 1 " +
//                        "FROM Tree_V3 " +
//                        "WHERE CuttingUnitCode = @CuttingUnitCode " +
//                        "AND ifnull(PlotNumber, -1) = ifnull(@PlotNumber, -1)",
//                        new { atn.CuttingUnitCode, atn.PlotNumber });

//                    Database.Execute2(
//                        "INSERT INTO Tree_V3 ( " +
//                            "TreeID, " +
//                            "CuttingUnitCode, " +
//                            "PlotNumber, " +
//                            "StratumCode, " +
//                            "SampleGroupCode, " +
//                            "Species, " +
//                            "LiveDead, " +
//                            "TreeNumber, " +
//                            "CountOrMeasure, " +
//                            "CreatedBy " +
//                        ") VALUES ( " +
//                            "@TreeID, " +
//                            "@CuttingUnitCode, " +
//                            "@PlotNumber, " +
//                            "@StratumCode, " +
//                            "@SampleGroupCode, " +
//                            "@Species, " +
//                            "@LiveDead, " +
//                            "@TreeNumber," +
//                            "@CountOrMeasure," +
//                            "@UserName " +
//                        ");" +
//                        "INSERT INTO TreeMeasurment (" +
//                            "TreeID" +
//                        ") VALUES ( " +
//                            "@TreeID" +
//                        ");",
//                        new
//                        {
//                            tallyEntry.TreeID,
//                            tallyEntry.TreeNumber,
//                            atn.CuttingUnitCode,
//                            atn.PlotNumber,
//                            atn.StratumCode,
//                            atn.SampleGroupCode,
//                            atn.Species,
//                            atn.LiveDead,
//                            tallyEntry.CountOrMeasure,
//                            UserName,
//                        });
//                }

//                Database.Execute2(
//                    "INSERT INTO TallyLedger ( " +
//                        "TreeID, " +
//                        "TallyLedgerID, " +
//                        "CuttingUnitCode, " +
//                        "PlotNumber, " +
//                        "StratumCode, " +
//                        "SampleGroupCode, " +
//                        "Species, " +
//                        "LiveDead, " +
//                        "TreeCount, " +
//                        "KPI, " +
//                        "STM, " +
//                        "ThreePRandomValue, " +
//                        "EntryType, " +
//                        "CreatedBy" +
//                    ") VALUES ( " +
//                        "@TreeID, " +
//                        "@TallyLedgerID, " +
//                        "@CuttingUnitCode, " +
//                        "@PlotNumber, " +
//                        "@StratumCode, " +
//                        "@SampleGroupCode, " +
//                        "@Species, " +
//                        "@LiveDead, " +
//                        "@TreeCount, " +
//                        "@KPI, " +
//                        "@STM, " +
//                        "@ThreePRandomValue," +
//                        "@EntryType," +
//                        "@CreatedBy" +
//                    ");",
//                    new
//                    {
//                        tallyEntry.TreeID,
//                        tallyEntry.TallyLedgerID,
//                        atn.CuttingUnitCode,
//                        atn.PlotNumber,
//                        atn.StratumCode,
//                        atn.SampleGroupCode,
//                        atn.Species,
//                        atn.LiveDead,
//                        atn.TreeCount,
//                        atn.KPI,
//                        atn.STM,
//                        atn.ThreePRandomValue,
//                        atn.EntryType,
//                        CreatedBy = UserName,
//                    });



//                Database.CommitTransaction();

//                return tallyEntry;
//            }
//            catch
//            {
//                Database.RollbackTransaction();
//                throw;
//            }
//        }

//        public void UpsertSamplerState(SamplerState samplerState)
//        {
//            var deviceID = DeviceInfo.GetUniqueDeviceID();

//            Database.Execute2(
//@"INSERT INTO SamplerState (
//    DeviceID,
//    StratumCode,
//    SampleGroupCode,
//    SampleSelectorType,
//    BlockState,
//    SystematicIndex,
//    Counter,
//    InsuranceIndex,
//    InsuranceCounter
//) VALUES (
//    @DeviceID,
//    @StratumCode,
//    @SampleGroupCode,
//    @SampleSelectorType,
//    @BlockState,
//    @SystematicIndex,
//    @Counter,
//    @InsuranceIndex,
//    @InsuranceCounter
//)
//ON CONFLICT (DeviceID, StratumCode, SampleGroupCode) DO
//UPDATE SET
//        BlockState = @BlockState,
//        SystematicIndex = @SystematicIndex,
//        Counter = @Counter,
//        InsuranceIndex = @InsuranceIndex,
//        InsuranceCounter = @InsuranceCounter
//    WHERE DeviceID = @DeviceID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode;",
//                new
//                {
//                    DeviceID = deviceID,
//                    samplerState.BlockState,
//                    samplerState.Counter,
//                    samplerState.InsuranceCounter,
//                    samplerState.InsuranceIndex,
//                    samplerState.SystematicIndex,
//                    samplerState.SampleSelectorType,
//                    samplerState.SampleGroupCode,
//                    samplerState.StratumCode,
//                }
//            );
//        }

//        public void DeleteTallyEntry(string tallyLedgerID)
//        {
//            Database.BeginTransaction();
//            try
//            {
//                Database.Execute("DELETE FROM TREE_V3 WHERE TreeID IN (SELECT TreeID FROM TallyLedger WHERE TallyLedgerID = @p1);", tallyLedgerID);
//                Database.Execute("DELETE FROM TallyLedger WHERE TallyLedgerID = @p1;", tallyLedgerID);

//                Database.CommitTransaction();
//            }
//            catch
//            {
//                Database.RollbackTransaction();
//                throw;
//            }
//        }

        #endregion Tally Entry

        public void LogMessage(string message, string level)
        {
            Database.LogMessage(message, level);

            //Database.Execute2("INSERT INTO MessageLog (Message, Level) VALUES (@message, @level);",
            //    new { Message = message, Level = level });
        }
    }
}