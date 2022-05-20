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
            get => _logNumber;
            set => SetProperty(ref _logNumber, value); 
        }

        private string _grade;
        public string Grade
        {
            get => _grade; 
            set => SetProperty(ref _grade, value);
        }

        private double _seenDefect;
        public double SeenDefect
        {
            get => _seenDefect;
            set => SetProperty(ref _seenDefect, value);
        }

        private double _percentRecoverable;
        public double PercentRecoverable
        {
            get => _percentRecoverable;
            set => SetProperty(ref _percentRecoverable, value);
        }

        private int _length;
        public int Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }

        private string _exportGrade;
        public string ExportGrade
        {
            get => _exportGrade;
            set => SetProperty(ref _exportGrade, value);
        }

        private double _smallEndDiameter;
        public double SmallEndDiameter
        {
            get => _smallEndDiameter;
            set => SetProperty(ref _smallEndDiameter, value);
        }

        private double _largeEndDiameter;
        public double LargeEndDiameter
        {
            get => _largeEndDiameter;
            set => SetProperty(ref _largeEndDiameter, value);
        }

        private double _grossCubicFoot;
        public double GrossCubicFoot
        {
            get => _grossCubicFoot;
            set => SetProperty(ref _grossCubicFoot, value);
        }

        private double _netBoardFoot;
        public double NetBoardFoot
        {
            get => _netBoardFoot;
            set => SetProperty(ref _netBoardFoot, value);
        }

        private double _grossBoardFoot;
        public double GrossBoardFoot
        {
            get => _grossBoardFoot;
            set => SetProperty(ref _grossBoardFoot, value);
        }

        private double _netCubicFoot;
        public double NetCubicFoot
        {
            get => _netCubicFoot;
            set => SetProperty(ref _netCubicFoot, value);
        }

        private double _boardFootRemoved;
        public double BoardFootRemoved
        {
            get => _boardFootRemoved;
            set => SetProperty(ref _boardFootRemoved, value);
        }

        private double _cubicFootRemoved;
        public double CubicFootRemoved
        {
            get =>_cubicFootRemoved;
            set => SetProperty(ref _cubicFootRemoved, value);
        }

        private double _dibClass;
        public double DIBClass
        {
            get => _dibClass;
            set => SetProperty(ref _dibClass, value);
        }

        private double _barkThickness;
        public double BarkThickness
        {
            get => _barkThickness;
            set => SetProperty(ref _barkThickness, value);
        }

        public string CreatedBy { get; set; }

        public int ErrorCount { get; set; }
    }
}