using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    public class Cruise
    {
        [PrimaryKeyField]
        public string Cruise_CN { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SaleID { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string CruiseID { get; set; }

        public string Purpose { get; set; }

        public string Remarks { get; set; }

        public string CruiseNumber { get; set; }

        public bool? UseCrossStrataPlotTreeNumbering { get; set; }

        [Field(SQLExpression = "s.Name", Alias = "SaleName")]
        public string SaleName { get; set; }

        [Field(SQLExpression = "s.SaleNumber", Alias = "SaleNumber")]
        public string SaleNumber { get; set; }

        [Field(Alias = "HasPlotStrata", SQLExpression = "(SELECT count(*) > 0 FROM Stratum JOIN LK_CruiseMethod USING (Method) WHERE Stratum.CruiseID = Cruise.CruiseID AND LK_CruiseMethod.IsPlotMethod = 1)")]
        public bool HasPlotStrata { get; set; }

        public override string ToString()
        {
            return $"{SaleName} {SaleNumber} {Purpose}";
        }
    }
}