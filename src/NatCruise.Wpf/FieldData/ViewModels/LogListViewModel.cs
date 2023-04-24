using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.MVVM.ViewModels;
using NatCruise.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class LogListViewModel : ViewModelBase
    {
        private readonly IEnumerable<LogFieldSetup> COMMON_LOGFIELDS = new[]
        {
            new LogFieldSetup{DbType = "TEXT", Heading = "Cutting Unit", Field = nameof(Log.CuttingUnitCode)},
            new LogFieldSetup{DbType = "TEXT", Heading = "Plot Number", Field = nameof(Log.PlotNumber)},
            new LogFieldSetup{DbType = "TEXT", Heading = "Tree Number", Field = nameof(Log.TreeNumber)},
            //new LogFieldSetup{DbType = "TEXT", Heading = "Stratum", Field = nameof(Log.StratumCode)},
            //new LogFieldSetup{DbType = "TEXT", Heading = "Sample Group", Field = nameof(Log.SampleGroupCode)},
            //new LogFieldSetup{DbType = "TEXT", Heading = "Species", Field = nameof(Log.SpeciesCode)},
            //new LogFieldSetup{DbType = "TEXT", Heading = "Live/Dead", Field = nameof(Log.LiveDead)},
            new LogFieldSetup{DbType = "TEXT", Heading = "Log Number", Field = nameof(Log.LogNumber)},
        };

        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<Log> _logs;
        private IEnumerable<LogFieldSetup> _fields;
        private Log _selectedLog;
        private string _treeID;

        public LogListViewModel(ILogDataservice logDataservice, ILogFieldDataservice logFieldDataservice, IFieldSetupDataservice fieldSetupDataservice, LogEditViewModel logEditViewModel)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
            LogFieldDataservice = logFieldDataservice ?? throw new ArgumentNullException(nameof(logFieldDataservice));
            LogEditViewModel = logEditViewModel ?? throw new ArgumentNullException(nameof(logEditViewModel));
            FieldSetupDataservice = fieldSetupDataservice ?? throw new ArgumentNullException(nameof(fieldSetupDataservice));
        }

        public ILogDataservice LogDataservice { get; }
        public ILogFieldDataservice LogFieldDataservice { get; }
        public LogEditViewModel LogEditViewModel { get; }
        public IFieldSetupDataservice FieldSetupDataservice { get; }

        public string TreeID
        {
            get => _treeID;
            set
            {
                SetProperty(ref _treeID, value);
                Load();
            }
        }

        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set
            {
                SetProperty(ref _cuttingUnitCode, value);
                Load();
            }
        }

        public string StratumCode
        {
            get => _stratumCode;
            set
            {
                SetProperty(ref _stratumCode, value);
                Load();
            }
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set
            {
                SetProperty(ref _sampleGroupCode, value);
                Load();
            }
        }

        public IEnumerable<Log> Logs
        {
            get => _logs;
            set
            {
                SetProperty(ref _logs, value);
            }
        }

        public Log SelectedLog
        {
            get => _selectedLog;
            set
            {
                SetProperty(ref _selectedLog, value);
                LogEditViewModel.Log = value;
            }
        }

        public IEnumerable<LogFieldSetup> Fields
        {
            get => _fields;
            set => SetProperty(ref _fields, value);
        }

        public override void Load()
        {
            base.Load();

            var treeID = TreeID;
            if (treeID != null)
            {
                Fields = FieldSetupDataservice.GetLogFieldSetupsByTreeID(treeID);

                Logs = LogDataservice.GetLogs(treeID).ToObservableCollection()
                    ?? new ObservableCollection<Log>();
            }
            else
            {


                var unitCode = CuttingUnitCode;
                var stCode = StratumCode;
                var sgCode = SampleGroupCode;

                var logs = LogDataservice.GetLogs(unitCode, stCode, sgCode);
                Logs = logs;

                Fields = COMMON_LOGFIELDS.Concat(FieldSetupDataservice.GetLogFieldSetupsByCruise());
            }
        }
    }
}
