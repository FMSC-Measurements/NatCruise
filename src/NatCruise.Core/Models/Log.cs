using FMSC.ORM.EntityModel.Attributes;
using Prism.Mvvm;

namespace NatCruise.Models
{
    [Table("Log")]
    public class Log : BindableBase
    {
        public string LogID { get; set; }

        public string TreeID { get; set; }

        private int _logNumber;
        public int LogNumber
        {
            get { return _logNumber; }
            set { _logNumber = value; }
        }

        private string _grade;
        public string Grade
        {
            get { return _grade; }
            set { _grade = value; }
        }

        private double _seenDefect;
        public double SeenDefect
        {
            get { return _seenDefect; }
            set { _seenDefect = value; }
        }

        private double _percentRecoverable;
        public double PercentRecoverable
        {
            get { return _percentRecoverable; }
            set { _percentRecoverable = value; }
        }

        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        private string _exportGrade;
        public string ExportGrade
        {
            get { return _exportGrade; }
            set { _exportGrade = value; }
        }

        private double _smallEndDiameter;
        public double SmallEndDiameter
        {
            get { return _smallEndDiameter; }
            set { _smallEndDiameter = value; }
        }

        private double _largeEndDiameter;
        public double LargeEndDiameter
        {
            get { return _largeEndDiameter; }
            set { _largeEndDiameter = value; }
        }

        private double _grossCubicFoot;
        public double GrossCubicFoot
        {
            get { return _grossCubicFoot; }
            set { _grossCubicFoot = value; }
        }

        private double _netBoardFoot;
        public double NetBoardFoot
        {
            get { return _netBoardFoot; }
            set { _netBoardFoot = value; }
        }

        private double _grossBoardFoot;
        public double GrossBoardFoot
        {
            get { return _grossBoardFoot; }
            set { _grossBoardFoot = value; }
        }

        private double _netCubicFoot;
        public double NetCubicFoot
        {
            get { return _netCubicFoot; }
            set { _netCubicFoot = value; }
        }

        private double _boardFootRemoved;
        public double BoardFootRemoved
        {
            get { return _boardFootRemoved; }
            set { _boardFootRemoved = value; }
        }

        private double _cubicFootRemoved;
        public double CubicFootRemoved
        {
            get { return _cubicFootRemoved; }
            set { _cubicFootRemoved = value; }
        }

        private double _dibClass;
        public double DIBClass
        {
            get { return _dibClass; }
            set { _dibClass = value; }
        }

        private double _barkThickness;
        public double BarkThickness
        {
            get { return _barkThickness; }
            set { _barkThickness = value; }
        }

        private string _createdBy;
        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public int ErrorCount { get; set; }
    }
}