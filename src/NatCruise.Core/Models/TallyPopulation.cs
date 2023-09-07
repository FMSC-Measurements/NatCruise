using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Linq;

namespace NatCruise.Models
{
    [Table("TallyPopulation")]
    public class TallyPopulation : BindableBase
    {
        private int _treeCount;
        private int _sumKPI;
        private int _treeCountPlot;
        private int _sumKPIPlot;
        private int _treeCountCruise;
        private int _sumKPICruise;

        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }
        public string DefaultLiveDead { get; set; }

        public int Frequency { get; set; }

        public int InsuranceFrequency { get; set; }

        public int KZ { get; set; }

        [Field("Description", PersistanceFlags = PersistanceFlags.Never)]
        public string TallyDescription { get; set; }

        [Field("HotKey", PersistanceFlags = PersistanceFlags.Never)]
        public string TallyHotKey { get; set; }

        [Field("StratumMethod", PersistanceFlags = PersistanceFlags.Never)]
        public string Method { get; set; }

        [Field("sgMinKPI", PersistanceFlags = PersistanceFlags.Never)]
        public int MinKPI { get; set; }

        [Field("sgMaxKPI", PersistanceFlags = PersistanceFlags.Never)]
        public int MaxKPI { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeCount
        {
            get { return _treeCount; }
            set { SetProperty(ref _treeCount, value); }
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeCountPlot
        {
            get => _treeCountPlot;
            set => SetProperty(ref _treeCountPlot, value);
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int TreeCountCruise
        {
            get => _treeCountCruise;
            set => SetProperty(ref _treeCountCruise, value);
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int SumKPI
        {
            get { return _sumKPI; }
            set { SetProperty(ref _sumKPI, value); }
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int SumKPIPlot
        {
            get => _sumKPIPlot;
            set => SetProperty(ref _sumKPIPlot, value);
        }

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public int SumKPICruise
        {
            get => _sumKPICruise;
            set => SetProperty(ref _sumKPICruise, value);
        }

        public bool IsClickerTally => SampleSelectorType == CruiseMethods.CLICKER_SAMPLER_TYPE;

        [Field(PersistanceFlags = PersistanceFlags.Never)]
        public string SampleSelectorType { get; set; }

        public override string ToString()
        {
            return $"St:{StratumCode} Sg:{SampleGroupCode} Sp:{SpeciesCode} LD:{LiveDead}";
        }
    }
}