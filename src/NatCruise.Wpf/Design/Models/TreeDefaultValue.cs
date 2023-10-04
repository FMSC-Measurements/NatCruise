using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Design.Models
{
    [Table("TreeDefaultValue")]
    public class TreeDefaultValue : BindableBase
    {
        private string _speciesCode;
        private string _primaryProduct;
        private double? _cullPrimary;
        private double? _cullPrimaryDead;
        private double? _hiddenPrimary;
        private double? _hiddenPrimaryDead;
        private string _treeGrade;
        private string _treeGradeDead;
        private double? _cullSecondary;
        private double? _hiddenSecondary;
        private double? _recoverable;
        private int? _merchHeightLogLength;
        private string _merchHeightsType;
        private double? _formClass;
        private double? _barkThicknessRatio;
        private double? _averageZ;
        private double? _referenceHeightPercent;

        public string SpeciesCode
        {
            get => _speciesCode;
            set => SetProperty(ref _speciesCode, value);
        }

        public string PrimaryProduct
        {
            get => _primaryProduct;
            set => SetProperty(ref _primaryProduct, value);
        }

        public double? CullPrimary
        {
            get => _cullPrimary;
            set => SetProperty(ref _cullPrimary, value);
        }

        public double? CullPrimaryDead
        {
            get => _cullPrimaryDead;
            set => SetProperty(ref _cullPrimaryDead, value);
        }

        public double? HiddenPrimary
        {
            get => _hiddenPrimary;
            set => SetProperty(ref _hiddenPrimary, value);
        }

        public double? HiddenPrimaryDead
        {
            get => _hiddenPrimaryDead;
            set => SetProperty(ref _hiddenPrimaryDead, value);
        }

        public string TreeGrade
        {
            get => _treeGrade;
            set => SetProperty(ref _treeGrade, value);
        }

        public string TreeGradeDead
        {
            get => _treeGradeDead;
            set => SetProperty(ref _treeGradeDead, value);
        }

        public double? CullSecondary
        {
            get => _cullSecondary;
            set => SetProperty(ref _cullSecondary, value);
        }

        public double? HiddenSecondary
        {
            get => _hiddenSecondary;
            set => SetProperty(ref _hiddenSecondary, value);
        }

        public double? Recoverable
        {
            get => _recoverable;
            set => SetProperty(ref _recoverable, value);
        }

        public int? MerchHeightLogLength
        {
            get => _merchHeightLogLength;
            set => SetProperty(ref _merchHeightLogLength, value);
        }

        public string MerchHeightType
        {
            get => _merchHeightsType;
            set => SetProperty(ref _merchHeightsType, value);
        }

        public double? FormClass
        {
            get => _formClass;
            set => SetProperty(ref _formClass, value);
        }

        public double? BarkThicknessRatio
        {
            get => _barkThicknessRatio;
            set => SetProperty(ref _barkThicknessRatio, value);
        }

        public double? AverageZ
        {
            get => _averageZ;
            set => SetProperty(ref _averageZ, value);
        }

        public double? ReferenceHeightPercent
        {
            get => _referenceHeightPercent;
            set => SetProperty(ref _referenceHeightPercent, value);
        }

        public string CreatedBy
        {
            get;
            set;
        }
    }
}