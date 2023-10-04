using FMSC.ORM.EntityModel.Attributes;
using System.Collections.Generic;

namespace NatCruise.Models
{
    //public enum FixCNTTallyField { Unknown, DBH, TotalHeight, DRC };

    public class FixCntTallyPopulation
    {
        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        [Field("Field")]
        public string FieldName { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public int IntervalSize { get; set; }

        public List<FixCNTTallyBucket> Buckets { get; set; }
    }
}