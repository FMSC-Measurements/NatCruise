using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class LogListViewModel : ViewModelBase
    {
        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<Log> _logs;

        public LogListViewModel(ILogDataservice logDataservice)
        {
            LogDataservice = logDataservice ?? throw new ArgumentNullException(nameof(logDataservice));
        }

        public ILogDataservice LogDataservice { get; }

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

        public override void Load()
        {
            base.Load();

            var unitCode = CuttingUnitCode;
            var stCode = StratumCode;
            var sgCode = SampleGroupCode;

            var logs = LogDataservice.GetLogs(unitCode, stCode, sgCode);
            Logs = logs;
        }
    }
}
