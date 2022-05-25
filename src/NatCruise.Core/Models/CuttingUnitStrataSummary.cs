using FMSC.ORM.EntityModel.Attributes;
using System.Collections.Generic;

namespace NatCruise.Models
{
    public class CuttingUnitStrataSummary
    {
        public string CuttingUnitCode { get; set; }

        [Field(Alias = "HasPlotStrata", SQLExpression =
@"(SELECT count(*) > 0
    FROM CuttingUnit_Stratum AS cust
    JOIN Stratum USING (StratumCode, CruiseID)
    JOIN LK_CruiseMethod USING (Method)
    WHERE IsPlotMethod = 1
        AND cust.CuttingUnitCode = CuttingUnit.CuttingUnitCode
        AND cust.CruiseID = CuttingUnit.CruiseID)")]
        public bool HasPlotStrata { get; set; }

        [Field(Alias = "HasTreeStrata", SQLExpression =
        @"(SELECT count(*) > 0
    FROM CuttingUnit_Stratum AS cust
    JOIN Stratum USING (StratumCode, CruiseID)
    JOIN LK_CruiseMethod USING (Method)
    WHERE IsPlotMethod = 0
        AND cust.CuttingUnitCode = CuttingUnit.CuttingUnitCode
        AND cust.CruiseID = CuttingUnit.CruiseID)")]
        public bool HasTreeStrata { get; set; }

        public IEnumerable<string> Methods { get; set; }
    }
}