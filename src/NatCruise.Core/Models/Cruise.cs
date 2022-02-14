using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    public class Cruise : BindableBase
    {
        private string _purpose;
        private string _remarks;
        private string _cruiseNumber;
        private bool? _useCrossStrataPlotTreeNumbering;
        private string _defaultUOM;

        [PrimaryKeyField]
        public string Cruise_CN { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SaleID { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string CruiseID { get; set; }

        public string Purpose
        {
            get => _purpose;
            set => SetProperty(ref _purpose, value);
        }

        [Field(SQLExpression = "LK_Purpose.ShortCode", Alias = "PurposeShortCode")]
        public string PurposeShortCode { get; set; }


        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public string CruiseNumber
        {
            get => _cruiseNumber;
            set => SetProperty(ref _cruiseNumber, value);
        }

        public bool? UseCrossStrataPlotTreeNumbering
        {
            get => _useCrossStrataPlotTreeNumbering;
            set => SetProperty(ref _useCrossStrataPlotTreeNumbering, value);
        }

        public string DefaultUOM
        {
            get => _defaultUOM;
            set => SetProperty(ref _defaultUOM, value);
        }

        [Field(Alias = "SaleName", SQLExpression = "(SELECT Name FROM Sale WHERE Cruise.SaleNumber = Sale.SaleNumber)")]
        public string SaleName { get; set; }

        [Field(PersistanceFlags = PersistanceFlags.OnInsert)]
        public string SaleNumber { get; set; }

        [Field(Alias = "HasPlotStrata", SQLExpression = "(SELECT count(*) > 0 FROM Stratum JOIN LK_CruiseMethod USING (Method) WHERE Stratum.CruiseID = Cruise.CruiseID AND LK_CruiseMethod.IsPlotMethod = 1)")]
        public bool HasPlotStrata { get; set; }

        public override string ToString()
        {
            return $"{SaleName} {SaleNumber} {Purpose}";
        }
    }
}