namespace NatCruise.Models
{
    /// <summary>
    /// Extends Tree class to include tree measurement fields
    /// </summary>
    public class TreeEx : Tree
    {
        private const string DEFAULT_GRADE = "00";
        private const string DEFAULT_CLEAR_FACE = "";
        private const string DEFAULT_DEFECT_CODE = "";

        private double _seenDefectPrimary;
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
        private double _totalHeight;
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
        private int _errorCount;
        private int _warningCount;
        private int _treeCount;

        public bool IsFallBuckScale
        {
            get { return _isFallBuckScale; }
            set { SetProperty(ref _isFallBuckScale, value); }
        }

        public double SeenDefectPrimary
        {
            get { return _seenDefectPrimary; }
            set { SetProperty(ref _seenDefectPrimary, value); }
        }

        public double SeenDefectSecondary
        {
            get { return _seenDefectSecondary; }
            set { SetProperty(ref _seenDefectSecondary, value); }
        }

        public double RecoverablePrimary
        {
            get { return _recoverablePrimary; }
            set { SetProperty(ref _recoverablePrimary, value); }
        }

        public double HiddenPrimary
        {
            get { return _hiddenPrimary; }
            set { SetProperty(ref _hiddenPrimary, value); }
        }

        public string Grade
        {
            get { return _grade; }
            set { SetProperty(ref _grade, value); }
        }

        public double HeightToFirstLiveLimb
        {
            get { return _heightToFirstLiveLimb; }
            set { SetProperty(ref _heightToFirstLiveLimb, value); }
        }

        public double PoleLength
        {
            get { return _poleLength; }
            set { SetProperty(ref _poleLength, value); }
        }

        public string ClearFace
        {
            get { return _clearFace; }
            set { SetProperty(ref _clearFace, value); }
        }

        public double CrownRatio
        {
            get { return _crownRatio; }
            set { SetProperty(ref _crownRatio, value); }
        }

        public double DBH
        {
            get { return _dbh; }
            set { SetProperty(ref _dbh, value); }
        }

        public double DRC
        {
            get { return _drc; }
            set { SetProperty(ref _drc, value); }
        }

        public double TotalHeight
        {
            get { return _totalHeight; }
            set { SetProperty(ref _totalHeight, value); }
        }

        public double MerchHeightPrimary
        {
            get { return _merchHeightPrimary; }
            set { SetProperty(ref _merchHeightPrimary, value); }
        }

        public double MerchHeightSecondary
        {
            get { return _merchHeightSecondary; }
            set { SetProperty(ref _merchHeightSecondary, value); }
        }

        public double FormClass
        {
            get { return _formClass; }
            set { SetProperty(ref _formClass, value); }
        }

        public double UpperStemDiameter
        {
            get { return _upperStemDiameter; }
            set { SetProperty(ref _upperStemDiameter, value); }
        }

        public double UpperStemHeight
        {
            get { return _upperStemHeight; }
            set { SetProperty(ref _upperStemHeight, value); }
        }

        public double DBHDoubleBarkThickness
        {
            get { return _dbhDoubleBarkThickness; }
            set { SetProperty(ref _dbhDoubleBarkThickness, value); }
        }

        public double TopDIBPrimary
        {
            get { return _topDibPrimary; }
            set { SetProperty(ref _topDibPrimary, value); }
        }

        public double TopDIBSecondary
        {
            get { return _topDibSecondary; }
            set { SetProperty(ref _topDibSecondary, value); }
        }

        public string DefectCode
        {
            get { return _defectCode; }
            set { SetProperty(ref _defectCode, value); }
        }

        public double DiameterAtDefect
        {
            get { return _diameterAtDefect; }
            set { SetProperty(ref _diameterAtDefect, value); }
        }

        public double VoidPercent
        {
            get { return _voidPercent; }
            set { SetProperty(ref _voidPercent, value); }
        }

        public double Slope
        {
            get { return _slope; }
            set { SetProperty(ref _slope, value); }
        }

        public double Aspect
        {
            get { return _aspect; }
            set { SetProperty(ref _aspect, value); }
        }

        public string Remarks
        {
            get { return _remarks; }
            set { SetProperty(ref _remarks, value); }
        }

        public string Initials
        {
            get { return _initials; }
            set { SetProperty(ref _initials, value); }
        }

        public int KPI { get; set; }

        public bool STM { get; set; }

        public int TreeCount
        {
            get => _treeCount;
            set => SetProperty(ref _treeCount, value);
        }

        public int ErrorCount
        {
            get => _errorCount;
            set => SetProperty(ref _errorCount, value);
        }

        public int WarningCount
        {
            get => _warningCount;
            set => SetProperty(ref _warningCount, value);
        }
    }
}