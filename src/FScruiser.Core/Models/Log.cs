using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;
using FScruiser.Util;

namespace FScruiser.Models
{
    [Table("Log_V3")]
    public class Log : INPC_Base, IHasTreeID
    {
        [Field("LogID")]
        public string LogID { get; set; }

        [Field("TreeID")]
        public string TreeID { get; set; }

        private int _logNumber;

        [Field("LogNumber")]
        public int LogNumber
        {
            get { return _logNumber; }
            set { _logNumber = value; }
        }

        private string _grade;

        [Field("Grade")]
        public string Grade
        {
            get { return _grade; }
            set { _grade = value; }
        }

        private double _seenDefect;

        [Field("SeenDefect")]
        public double SeenDefect
        {
            get { return _seenDefect; }
            set { _seenDefect = value; }
        }

        private double _percentRecoverable;

        [Field("PercentRecoverable")]
        public double PercentRecoverable
        {
            get { return _percentRecoverable; }
            set { _percentRecoverable = value; }
        }

        private int _length;

        [Field("Length")]
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private string _exportGrade;

        [Field("ExportGrade")]
        public string ExportGrade
        {
            get { return _exportGrade; }
            set { _exportGrade = value; }
        }

        private double _smallEndDiameter;

        [Field("SmallEndDiameter")]
        public double SmallEndDiameter
        {
            get { return _smallEndDiameter; }
            set { _smallEndDiameter = value; }
        }

        private double _largeEndDiameter;

        [Field("LargeEndDiameter")]
        public double LargeEndDiameter
        {
            get { return _largeEndDiameter; }
            set { _largeEndDiameter = value; }
        }

        private double _grossCubicFoot;

        [Field("GrossCubicFoot")]
        public double GrossCubicFoot
        {
            get { return _grossCubicFoot; }
            set { _grossCubicFoot = value; }
        }

        private double _netBoardFoot;

        [Field("NetBoardFoot")]
        public double NetBoardFoot
        {
            get { return _netBoardFoot; }
            set { _netBoardFoot = value; }
        }

        private double _grossBoardFoot;

        [Field("GrossBoardFoot")]
        public double GrossBoardFoot
        {
            get { return _grossBoardFoot; }
            set { _grossBoardFoot = value; }
        }

        private double _netCubicFoot;

        [Field("NetCubicFoot")]
        public double NetCubicFoot
        {
            get { return _netCubicFoot; }
            set { _netCubicFoot = value; }
        }

        private double _boardFootRemoved;

        [Field("BoardFootRemoved")]
        public double BoardFootRemoved
        {
            get { return _boardFootRemoved; }
            set { _boardFootRemoved = value; }
        }

        private double _cubicFootRemoved;

        [Field("CubicFootRemoved")]
        public double CubicFootRemoved
        {
            get { return _cubicFootRemoved; }
            set { _cubicFootRemoved = value; }
        }

        private double _dibClass;

        [Field("DIBClass")]
        public double DIBClass
        {
            get { return _dibClass; }
            set { _dibClass = value; }
        }

        private double _barkThickness;

        [Field("BarkThickness")]
        public double BarkThickness
        {
            get { return _barkThickness; }
            set { _barkThickness = value; }
        }

        private string _createdBy;

        [Field("CreatedBy")]
        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        [Field("ErrorCount")]
        public int ErrorCount { get; set; }
    }
}