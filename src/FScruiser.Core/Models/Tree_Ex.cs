using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;

namespace FScruiser.Models
{
    public class Tree_Ex : Tree, IHasTreeID
    {
        private static readonly string DEFAULT_GRADE = "00";
        private static readonly string DEFAULT_CLEAR_FACE = "";
        private static readonly string DEFAULT_DEFECT_CODE = "";

        private double _seenDefectSecondary;
        private double _recoverablePrimary;
        private double _hiddenPrimary;
        private string _grade = DEFAULT_GRADE;
        private double _heightToFirstLiveLimb;
        private double _poleLength;
        private string _clearFace = DEFAULT_CLEAR_FACE;
        private double _crownRatio;
        private double _dbh;
        private double _drc;
        private float _totalHeight;
        private double _merchHeightPrimary;
        private double _merchHeightSecondary;
        private double _formClass;
        private double _upperStemDiameter;
        private double _upperStemHeight;
        private double _dbhDoubleBarkThickness;
        private double _topDibPrimary;
        private double _topDibSecondary;
        private string _defectCode = DEFAULT_DEFECT_CODE;
        private double _diameterAtDefect;
        private double _voidPercent;
        private double _slope;
        private double _aspect;
        private bool _isFallBuckScale;
        private string _remarks = "";
        private string _initials;

        //[Field(Name = "TreeID")]
        //public string TreeID { get; set; }

        [Field("IsFallBuckScale")]
        public bool IsFallBuckScale
        {
            get { return _isFallBuckScale; }
            set { SetValue(ref _isFallBuckScale, value); }
        }

        [Field("SeenDefectPrimary")]
        public double SeenDefectPrimary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field("SeenDefectSecondary")]
        public double SeenDefectSecondary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field("RecoverablePrimary")]
        public double RecoverablePrimary
        {
            get { return _recoverablePrimary; }
            set { SetValue(ref _recoverablePrimary, value); }
        }

        [Field("HiddenPrimary")]
        public double HiddenPrimary
        {
            get { return _hiddenPrimary; }
            set { SetValue(ref _hiddenPrimary, value); }
        }

        [Field("Grade")]
        public string Grade
        {
            get { return _grade; }
            set { SetValue(ref _grade, value); }
        }

        [Field("HeightToFirstLiveLimb")]
        public double HeightToFirstLiveLimb
        {
            get { return _heightToFirstLiveLimb; }
            set { SetValue(ref _heightToFirstLiveLimb, value); }
        }

        [Field("PoleLength")]
        public double PoleLength
        {
            get { return _poleLength; }
            set { SetValue(ref _poleLength, value); }
        }

        [Field("ClearFace")]
        public string ClearFace
        {
            get { return _clearFace; }
            set { SetValue(ref _clearFace, value); }
        }

        [Field("CrownRatio")]
        public double CrownRatio
        {
            get { return _crownRatio; }
            set { SetValue(ref _crownRatio, value); }
        }

        [Field("DBH")]
        public double DBH
        {
            get { return _dbh; }
            set { SetValue(ref _dbh, value); }
        }

        [Field("DRC")]
        public double DRC
        {
            get { return _drc; }
            set { SetValue(ref _drc, value); }
        }

        [Field("TotalHeight")]
        public virtual float TotalHeight
        {
            get { return _totalHeight; }
            set { SetValue(ref _totalHeight, value); }
        }

        [Field("MerchHeightPrimary")]
        public double MerchHeightPrimary
        {
            get { return _merchHeightPrimary; }
            set { SetValue(ref _merchHeightPrimary, value); }
        }

        [Field("MerchHeightSecondary")]
        public double MerchHeightSecondary
        {
            get { return _merchHeightSecondary; }
            set { SetValue(ref _merchHeightSecondary, value); }
        }

        [Field("FormClass")]
        public double FormClass
        {
            get { return _formClass; }
            set { SetValue(ref _formClass, value); }
        }

        [Field("UpperStemDiameter")]
        public double UpperStemDiameter
        {
            get { return _upperStemDiameter; }
            set { SetValue(ref _upperStemDiameter, value); }
        }

        [Field("UpperStemHeight")]
        public double UpperStemHeight
        {
            get { return _upperStemHeight; }
            set { SetValue(ref _upperStemHeight, value); }
        }

        [Field("DBHDoubleBarkThickness")]
        public double DBHDoubleBarkThickness
        {
            get { return _dbhDoubleBarkThickness; }
            set { SetValue(ref _dbhDoubleBarkThickness, value); }
        }

        [Field("TopDIBPrimary")]
        public double TopDIBPrimary
        {
            get { return _topDibPrimary; }
            set { SetValue(ref _topDibPrimary, value); }
        }

        [Field("TopDIBSecondary")]
        public double TopDIBSecondary
        {
            get { return _topDibSecondary; }
            set { SetValue(ref _topDibSecondary, value); }
        }

        [Field("DefectCode")]
        public string DefectCode
        {
            get { return _defectCode; }
            set { SetValue(ref _defectCode, value); }
        }

        [Field("DiameterAtDefect")]
        public double DiameterAtDefect
        {
            get { return _diameterAtDefect; }
            set { SetValue(ref _diameterAtDefect, value); }
        }

        [Field("VoidPercent")]
        public double VoidPercent
        {
            get { return _voidPercent; }
            set { SetValue(ref _voidPercent, value); }
        }

        [Field("Slope")]
        public double Slope
        {
            get { return _slope; }
            set { SetValue(ref _slope, value); }
        }

        [Field("Aspect")]
        public double Aspect
        {
            get { return _aspect; }
            set { SetValue(ref _aspect, value); }
        }

        [Field("Remarks")]
        public string Remarks
        {
            get { return _remarks; }
            set { SetValue(ref _remarks, value); }
        }

        [Field("Initials")]
        public string Initials
        {
            get { return _initials; }
            set { SetValue(ref _initials, value); }
        }
    }
}