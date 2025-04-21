using Backpack.SqlBuilder;
using CruiseDAL;
using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using NatCruise.Wpf.CheckCruise.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;


namespace NatCruise.Wpf.CheckCruise.Data
{

    public class CheckCruiseCopier : DatabaseCopier
    {
        private Dictionary<string, string> TreeAuditRuleIDMaps { get; set; }
        private Dictionary<string, string> TreeIDMapps { get; set; }

        public Guid? GuidMap { get; set; } = Guid.NewGuid();
        public IReadOnlyList<UnitStratum> SelectedUnitStrata { get; set; }
        public IReadOnlyCollection<string> SelectedUnitCodes { get; set; }
        public IReadOnlyCollection<string> SelectedStratumCodes { get; set; }

        public bool IncludeLogRecords { get; set; }

        public bool IncludeTreeMeasurments { get; set; }

        protected CheckCruiseCopier()
        {
            Tables = new[]
            {
                // design tables sale, cruise, cutting unit, stratum, sample group, subpopulation IDs need to change
                new CopyTableConfig(nameof(Sale), action: CopySale),
                new CopyTableConfig(nameof(Cruise), action: CopyCruise),
                new CopyTableConfig(nameof(CuttingUnit), action: CopyCuttingUnit),
                new CopyTableConfig(nameof(Stratum), action: CopyStratum),
                new CopyTableConfig(nameof(CuttingUnit_Stratum), action: CopyCuttingUnitStratum),
                new CopyTableConfig(nameof(SampleGroup), action: CopySampleGroup),
                new CopyTableConfig(typeof(Species)),
                new CopyTableConfig(typeof(Species_Product)),
                new CopyTableConfig(nameof(SubPopulation), action: CopySubpopulation),

                new CopyTableConfig(nameof(FixCNTTallyPopulation), action: CopyFixCNTTallyPopulation),

                // field data. PlotID values need to be changed
                new CopyTableConfig(nameof(Plot), action: CopyPlot), // also copies plot_strata and plotLocation

                // tree and Dependant tables. TreeID values need to be changed
                new CopyTableConfig(nameof(Tree), action: CopyTree, canCopy: () => IncludeTreeMeasurments),
                new CopyTableConfig(typeof(TreeMeasurment),
                        configRead: x => x.Join("Tree AS t", "USING (TreeID)").Where("t.CruiseID = @p1"),
                        configOverrides: (x) =>
                        {
                            x.Add(nameof(TreeMeasurment.TreeID), GetOverrideTreeID);
                            return x;
                        },
                        canCopy: () => IncludeTreeMeasurments
                ),

                new CopyTableConfig(typeof(TreeFieldValue),
                        configRead: x=> x.Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;"),
                        configOverrides: (x) =>
                                {
                                    x.Add(nameof(TreeFieldValue.TreeID), GetOverrideTreeID);
                                    return x;
                                },
                        canCopy: () => IncludeTreeMeasurments
                ),
                new CopyTableConfig(typeof(TreeLocation),
                        configRead: x => x.Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;"),
                        configOverrides: (x) =>
                        {
                            x.Add(nameof(TreeLocation.TreeID), GetOverrideTreeID);
                            return x;
                        },
                        canCopy: () => IncludeTreeMeasurments
                ),
                new CopyTableConfig(nameof(Log), action: CopyLogs),
                new CopyTableConfig(nameof(TallyLedger), CopyTallyLedger),  // do we need TallyLedger.. maybe for processing but I don't think we need the counts. Lets keep it unless it causes an issue which I don't think it will

                // meta design tables
                new CopyTableConfig(typeof(TallyDescription)),
                new CopyTableConfig(typeof(TallyHotKey)),
                new CopyTableConfig(typeof(LogFieldSetup)), 
                new CopyTableConfig(typeof(TreeFieldSetup)),
                new CopyTableConfig(typeof(TreeFieldHeading)),

                new CopyTableConfig(typeof(TreeDefaultValue)),

                    // audit rules, treeAuditRuleID values need to be changed
                new CopyTableConfig(nameof(TreeAuditRule), action: CopyTreeAuditRules), 
                new CopyTableConfig(nameof(TreeAuditRuleSelector), action: CopyTreeAuditRuleSelector, dependsOn: new[] {nameof(TreeAuditRule)}),

                new CopyTableConfig(typeof(LogGradeAuditRule)),
                new CopyTableConfig(typeof(BiomassEquation)),
                new CopyTableConfig(typeof(CruiseDAL.V3.Models.Reports)),
                //new CopyTableConfig(typeof(ValueEquation)),
                new CopyTableConfig(typeof(VolumeEquation)),

                new CopyTableConfig(typeof(StratumTemplate)),
                new CopyTableConfig(typeof(StratumTemplateTreeFieldSetup)),
                new CopyTableConfig(typeof(StratumTemplateLogFieldSetup)),

            };
        }

        

        public CheckCruiseCopier(IReadOnlyList<UnitStratum> selectedUnits)
            : this()
        {
            SelectedUnitStrata = selectedUnits;

            SelectedUnitCodes = selectedUnits.Select(x => x.CuttingUnitCode).Distinct().ToHashSet();
            SelectedStratumCodes = selectedUnits.Select(x => x.StratumCode).Distinct().ToHashSet();
        }

        //protected override ValueOverrideConfig GetValueOverrides(string destinatinCruiseID)
        //{
        //    var overrides = base.GetValueOverrides(destinatinCruiseID);
        //    overrides.Add("TreeID", GetOverrideTreeID);

        //    return;
        //}

        private string GetOverrideTreeID(object origValue)
        {
            if (TreeIDMapps == null) { throw new InvalidOperationException("TreeIDMapps not set"); }
            var origTreeID = (string)origValue;

            if (TreeIDMapps.TryGetValue(origTreeID, out var newTreeID))
            {
                return newTreeID;
            }
            throw new ArgumentException($"No TreeID map found for {origTreeID}");
        }



        private void CopyCruise(DbConnection source, DbConnection dest, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            var cruise = source.From<Cruise>().Where("CruiseID = @p1;").Query(srcCruiseID).FirstOrDefault();

            cruise.CruiseNumber = cruise.SaleNumber + "CHECK";
            cruise.Purpose = "Check Cruise";
            cruise.Remarks = "";
            cruise.MeasurementYear = DateTime.Now.Year.ToString();
            cruise.CreatedBy = Environment.UserName;
            cruise.Created_TS = null;

            dest.Insert(cruise, valueOverrides: valueOverrides, option: option);
        }

        private void CopySale(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            //var valueOverrides = GetValueOverrides(destCruiseID);
            var cruiseID = destCruiseID ?? srcCruiseID;


            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleNumber) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleNumber)")
                    .Where("CruiseID = @p1")
                    .Query(srcCruiseID).FirstOrDefault();
                destination.Insert(sale, option: option);
            }
        }

        private void CopyCuttingUnit(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);
            valueOverrides.Add("CuttingUnitID", (x) => GetMappedGuidValue(x).ToString());

            foreach (var unitCode in SelectedUnitCodes)
            {
                var unit = source.From<CuttingUnit>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2").Query(srcCruiseID, unitCode).Single();

                destination.Insert(unit, valueOverrides: valueOverrides, option: option);
            }
        }

        private void CopyStratum(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);
            valueOverrides.Add("StratumID", (x) => GetMappedGuidValue(x).ToString());

            foreach (var stCode in SelectedStratumCodes)
            {
                var st = source.From<Stratum>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(srcCruiseID, stCode).Single();
                destination.Insert(st, valueOverrides: valueOverrides, option: option);
            }
        }

        private void CopyCuttingUnitStratum(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            foreach(var i in SelectedUnitStrata)
            {
                var cust = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND StratumCode = @p3").Query(srcCruiseID, i.CuttingUnitCode, i.StratumCode).Single();
                destination.Insert(cust, valueOverrides: valueOverrides, option: option);
            }
        }

        public void CopySampleGroup(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);
            valueOverrides.Add("SampleGroupID", (x) => GetMappedGuidValue(x).ToString());

            foreach (var st in SelectedStratumCodes)
            {
                var sampleGroups = source.From<SampleGroup>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(srcCruiseID, st).ToArray();
                foreach (var sg in sampleGroups)
                {
                    sg.SampleGroupID = Guid.NewGuid().ToString();
                    destination.Insert(sg, valueOverrides: valueOverrides, option: option);
                }
            }
        }

        public void CopySubpopulation(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            foreach (var st in SelectedStratumCodes)
            {
                var subPopulations = source.From<SubPopulation>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(srcCruiseID, st).ToArray();
                foreach (var sp in subPopulations)
                { 
                    sp.SubPopulationID = Guid.NewGuid().ToString();
                    destination.Insert(sp, valueOverrides: valueOverrides, option: option);
                }
            }
        }

        public void CopyFixCNTTallyPopulation(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            foreach (var st in SelectedStratumCodes)
            {
                var fixCntTallyPops = source.From<FixCNTTallyPopulation>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(srcCruiseID, st).ToArray();
                foreach(var fctp in fixCntTallyPops)
                {
                    destination.Insert(fctp, valueOverrides: valueOverrides, option: option);
                }
            }
        }

        public void CopyPlot(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);
            //valueOverrides.Add("PlotID", (x) => GetMappedGuidValue(x).ToString());

            foreach (var unitCode in SelectedUnitCodes)
            {
                var plots = source.From<Plot>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2").Query(srcCruiseID, unitCode).ToArray();

                foreach(var plot in plots)
                {
                    //var ogPlotID = ;
                    plot.PlotID = GetMappedGuidValue(plot.PlotID).ToString();

                    destination.Insert(plot, valueOverrides: valueOverrides, option: option);

                    var plotStrata = source.From<Plot_Stratum>().Where("CruiseID = @p1 AND PlotNumber = @p2 AND CuttingUnitCode = @p3").Query(srcCruiseID, plot.PlotNumber, plot.CuttingUnitCode);
                    foreach(var plst in plotStrata)
                    {
                        destination.Insert(plst, valueOverrides: valueOverrides, option: option);
                    }

                    var plotLocs = source.From<PlotLocation>().Where("PlotID = @p1").Query(plot.PlotID);
                    foreach(var p in plotLocs)
                    {
                        p.PlotID = plot.PlotID;
                        destination.Insert(p, valueOverrides: valueOverrides, option: option);
                    }
                }
            }
        }

        public void CopyTree(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            if(!IncludeTreeMeasurments) { return; }

            var treeIDMaps = new Dictionary<string, string>();

            var valueOverrides = GetValueOverrides(destCruiseID);
            //valueOverrides.Add("TreeID", () => Guid.NewGuid().ToString());

            var trees = source.From<Tree>().Where("CruiseID = @p1").Query(srcCruiseID);
            foreach(var tree in trees)
            {
                var treeID = tree.TreeID;
                var newTreeID = GetMappedGuidValue(treeID).ToString();
                treeIDMaps.Add(treeID, newTreeID);
                tree.TreeID = newTreeID;

                destination.Insert(tree, valueOverrides: valueOverrides, option: option);
            }

            TreeIDMapps = treeIDMaps;
        }

        private void CopyLogs(DbConnection source, DbConnection destination, string cruiseID, string destCruiseID, OnConflictOption conflictOption)
        {
            if (!IncludeTreeMeasurments || !IncludeLogRecords) { return; }
            var valueOverrides = GetValueOverrides(destCruiseID);
            //valueOverrides.Add(nameof(Log.TreeID), GetOverrideTreeID);

            var log = source.From<Log>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var l in log)
            {
                var origtreeID = l.TreeID;
                l.TreeID = GetOverrideTreeID(origtreeID);

                l.LogID = GetMappedGuidValue(l.LogID).ToString();


                destination.Insert(l, valueOverrides: valueOverrides, option: conflictOption);
            }
        }

        private void CopyTallyLedger(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            if (!IncludeTreeMeasurments) { return; }

            var valueOverrides = GetValueOverrides(destCruiseID);

            var tallyLedgers = source.From<TallyLedger>().Where("CruiseID = @p1").Query(srcCruiseID);
            foreach(var tl in tallyLedgers)
            {
                var origtreeID = tl.TreeID;
                if (origtreeID != null)
                {
                    tl.TreeID = TreeIDMapps[origtreeID];
                }

                tl.TallyLedgerID = GetMappedGuidValue(tl.TallyLedgerID).ToString();

                destination.Insert(tl, valueOverrides: valueOverrides, option: option);
            }

        }

        // when copying tree audit rules from a template to a cruise file, we need to change the treeAuditRuleID values so that 
        // cruises created by this template can co-exist with other cruises in the save database.
        public void CopyTreeAuditRules(DbConnection source, DbConnection destination, string cruiseID, string destCruiseID, OnConflictOption conflictOption)
        {
            var treeAuditRuleMapGuid = GuidMap ?? throw new InvalidOperationException(nameof(GuidMap) + " not set");
            var tarIDMaps = new Dictionary<string, string>();


            var tars = source.From<TreeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var tar in tars)
            {
                var tarID = new Guid(tar.TreeAuditRuleID);
                var newTarID = XorGuid(treeAuditRuleMapGuid, tarID).ToString();

                tarIDMaps.Add(tar.TreeAuditRuleID, newTarID);

                tar.TreeAuditRuleID = newTarID;
                if (destCruiseID != null)
                {
                    tar.CruiseID = destCruiseID;
                }

                destination.Insert(tar, persistKeyvalue: false, option: conflictOption);
            }

            TreeAuditRuleIDMaps = tarIDMaps;
        }

        public void CopyTreeAuditRuleSelector(DbConnection source, DbConnection destination, string cruiseID, string destCruiseID, OnConflictOption conflictOption)
        {
            var tarIDMaps = TreeAuditRuleIDMaps ?? throw new NullReferenceException(nameof(TreeAuditRuleIDMaps));

            var tarSels = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var tarSel in tarSels)
            {
                tarSel.TreeAuditRuleID = tarIDMaps[tarSel.TreeAuditRuleID];
                if (destCruiseID != null)
                {
                    tarSel.CruiseID = destCruiseID;
                }

                destination.Insert(tarSel, persistKeyvalue: false, option: conflictOption);
            }
        }


        protected Guid GetMappedGuidValue(object origGuidValue)
        {
            if (origGuidValue is string str)
            {
                return GetMappedGuidValue(str);
            }
            //else if (origGuidValue is Guid guid)
            //{
            //    return GetMappedGuidValue(guid);
            //}
            else
            {
                throw new ArgumentException("Invalid type for origGuidValue");
            }
        }

        public Guid GetMappedGuidValue(string origGuidValue)
        {
            var origGuid = new Guid(origGuidValue);
            var map = GuidMap ?? throw new InvalidOperationException(nameof(GuidMap) + " not set");
            return XorGuid(map, origGuid);
        }

        public Guid GetMappedGuidValue(Guid origGuid)
        {
            var map = GuidMap ?? throw new InvalidOperationException(nameof(GuidMap) + " not set");
            return XorGuid(map, origGuid);
        }

        protected static Guid XorGuid(Guid guid1, Guid guid2)
        {
            var guid1Bytes = guid1.ToByteArray();
            var guid2Bytes = guid2.ToByteArray();

            return XorGuid(guid1Bytes, guid2Bytes);
        }

        protected static Guid XorGuid(byte[] guid1, byte[] guid2)
        {
            var newGuidBytes = guid1.Zip(guid2, (x, y) => (byte)(x ^ y)).ToArray();

            return new Guid(newGuidBytes);
        }


    }
}
