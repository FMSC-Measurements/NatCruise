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
        public string CuttingUnitCode { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }
        public string DefaultLiveDead { get; set; }

        public int Frequency { get; set; }

        public int InsuranceFrequency { get; set; }

        public int KZ { get; set; }

        [Field("Description")]
        public string TallyDescription { get; set; }

        [Field("HotKey")]
        public string TallyHotKey { get; set; }

        [Field("StratumMethod")]
        public string Method { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        [Field("sgMinKPI")]
        public int MinKPI { get; set; }

        [Field("sgMaxKPI")]
        public int MaxKPI { get; set; }

        private int _treeCount;
        private int _sumKPI;

        public int TreeCount
        {
            get { return _treeCount; }
            set { SetProperty(ref _treeCount, value); }
        }

        public int SumKPI
        {
            get { return _sumKPI; }
            set { SetProperty(ref _sumKPI, value); }
        }

        public override string ToString()
        {
            return $"St:{StratumCode} Sg:{SampleGroupCode} Sp:{SpeciesCode} LD:{LiveDead}";
        }
    }
}