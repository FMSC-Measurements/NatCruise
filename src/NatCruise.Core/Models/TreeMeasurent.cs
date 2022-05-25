using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Models
{
    [Table("TreeMeasurment")]
    public class TreeMeasurment : BindableBase
    {
        private const string DEFAULT_GRADE = "00";
        private const string DEFAULT_CLEAR_FACE = "";
        private const string DEFAULT_DEFECT_CODE = "";

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
        private string _remarks;
        private string _initials;

        [Field(Name = "TreeID")]
        public string TreeID { get; set; }

        [Field(Name = "IsFallBuckScale")]
        public bool IsFallBuckScale
        {
            get { return _isFallBuckScale; }
            set { SetProperty(ref _isFallBuckScale, value); }
        }

        [Field(Name = "SeenDefectPrimary")]
        public double SeenDefectPrimary
        {
            get { return _seenDefectSecondary; }
            set { SetProperty(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "SeenDefectSecondary")]
        public double SeenDefectSecondary
        {
            get { return _seenDefectSecondary; }
            set { SetProperty(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "RecoverablePrimary")]
        public double RecoverablePrimary
        {
            get { return _recoverablePrimary; }
            set { SetProperty(ref _recoverablePrimary, value); }
        }

        [Field(Name = "HiddenPrimary")]
        public double HiddenPrimary
        {
            get { return _hiddenPrimary; }
            set { SetProperty(ref _hiddenPrimary, value); }
        }

        [Field(Name = "Grade")]
        public string Grade
        {
            get { return _grade; }
            set { SetProperty(ref _grade, value); }
        }

        [Field(Name = "HeightToFirstLiveLimb")]
        public double HeightToFirstLiveLimb
        {
            get { return _heightToFirstLiveLimb; }
            set { SetProperty(ref _heightToFirstLiveLimb, value); }
        }

        [Field(Name = "PoleLength")]
        public double PoleLength
        {
            get { return _poleLength; }
            set { SetProperty(ref _poleLength, value); }
        }

        [Field(Name = "ClearFace")]
        public string ClearFace
        {
            get { return _clearFace; }
            set { SetProperty(ref _clearFace, value); }
        }

        [Field(Name = "CrownRatio")]
        public double CrownRatio
        {
            get { return _crownRatio; }
            set { SetProperty(ref _crownRatio, value); }
        }

        [Field(Name = "DBH")]
        public double DBH
        {
            get { return _dbh; }
            set { SetProperty(ref _dbh, value); }
        }

        [Field(Name = "DRC")]
        public double DRC
        {
            get { return _drc; }
            set { SetProperty(ref _drc, value); }
        }

        [Field(Name = "TotalHeight")]
        public virtual float TotalHeight
        {
            get { return _totalHeight; }
            set { SetProperty(ref _totalHeight, value); }
        }

        [Field(Name = "MerchHeightPrimary")]
        public double MerchHeightPrimary
        {
            get { return _merchHeightPrimary; }
            set { SetProperty(ref _merchHeightPrimary, value); }
        }

        [Field(Name = "MerchHeightSecondary")]
        public double MerchHeightSecondary
        {
            get { return _merchHeightSecondary; }
            set { SetProperty(ref _merchHeightSecondary, value); }
        }

        [Field(Name = "FormClass")]
        public double FormClass
        {
            get { return _formClass; }
            set { SetProperty(ref _formClass, value); }
        }

        [Field(Name = "UpperStemDiameter")]
        public double UpperStemDiameter
        {
            get { return _upperStemDiameter; }
            set { SetProperty(ref _upperStemDiameter, value); }
        }

        [Field(Name = "UpperStemHeight")]
        public double UpperStemHeight
        {
            get { return _upperStemHeight; }
            set { SetProperty(ref _upperStemHeight, value); }
        }

        [Field(Name = "DBHDoubleBarkThickness")]
        public double DBHDoubleBarkThickness
        {
            get { return _dbhDoubleBarkThickness; }
            set { SetProperty(ref _dbhDoubleBarkThickness, value); }
        }

        [Field(Name = "TopDIBPrimary")]
        public double TopDIBPrimary
        {
            get { return _topDibPrimary; }
            set { SetProperty(ref _topDibPrimary, value); }
        }

        [Field(Name = "TopDIBSecondary")]
        public double TopDIBSecondary
        {
            get { return _topDibSecondary; }
            set { SetProperty(ref _topDibSecondary, value); }
        }

        [Field(Name = "DefectCode")]
        public string DefectCode
        {
            get { return _defectCode; }
            set { SetProperty(ref _defectCode, value); }
        }

        [Field(Name = "DiameterAtDefect")]
        public double DiameterAtDefect
        {
            get { return _diameterAtDefect; }
            set { SetProperty(ref _diameterAtDefect, value); }
        }

        [Field(Name = "VoidPercent")]
        public double VoidPercent
        {
            get { return _voidPercent; }
            set { SetProperty(ref _voidPercent, value); }
        }

        [Field(Name = "Slope")]
        public double Slope
        {
            get { return _slope; }
            set { SetProperty(ref _slope, value); }
        }

        [Field(Name = "Aspect")]
        public double Aspect
        {
            get { return _aspect; }
            set { SetProperty(ref _aspect, value); }
        }

        [IgnoreField]
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        [IgnoreField]
        public string Initials
        {
            get => _initials;
            set => SetProperty(ref _initials, value);
        }
    }
}
