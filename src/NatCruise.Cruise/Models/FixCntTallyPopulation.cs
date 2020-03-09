using FMSC.ORM.EntityModel.Attributes;
using System.Collections.Generic;

namespace NatCruise.Cruise.Models
{
    //public enum FixCNTTallyField { Unknown, DBH, TotalHeight, DRC };

    public class FixCntTallyPopulation
    {
        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("Species")]
        public string Species { get; set; }

        [Field("LiveDead")]
        public string LiveDead { get; set; }

        [Field("Field")]
        public string FieldName { get; set; }

        [Field("Min")]
        public int Min { get; set; }

        [Field("Max")]
        public int Max { get; set; }

        [Field("IntervalSize")]
        public int IntervalSize { get; set; }

        public List<FixCNTTallyBucket> Buckets { get; set; }
    }
}