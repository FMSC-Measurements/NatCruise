using FMSC.ORM.EntityModel.Attributes;

namespace NatCruise.Models
{
    public class Cruise
    {
        [PrimaryKeyField]
        public string Cruise_CN { get; set; }

        public string CruiseID { get; set; }

        public string Purpose { get; set; }

        public string Remarks { get; set; }

        [Field(SQLExpression = "Sale.Name", Alias = "SaleName")]
        public string SaleName { get; set; }

        public string SaleNumber { get; set; }

        public override string ToString()
        {
            return $"{SaleName} {SaleNumber} {Purpose}";
        }
    }
}