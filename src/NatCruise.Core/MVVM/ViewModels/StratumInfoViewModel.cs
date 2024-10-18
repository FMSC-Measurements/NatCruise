using CommunityToolkit.Mvvm.ComponentModel;
using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.Navigation;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM.ViewModels
{
    public partial class StratumInfoViewModel : ViewModelBase
    {
        public readonly string[] Fixed_Size_Plot_Methods = new string[] { CruiseMethods.FIX, CruiseMethods.FCM, CruiseMethods.F3P, CruiseMethods.FIXCNT };
        private Stratum _stratum;
        private IEnumerable<string> _cuttingUnits;

        public StratumInfoViewModel(IStratumDataservice stratumDataservice, ICuttingUnitDataservice cuttingUnitDataservice)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
            CuttingUnitDataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
        }

        public IStratumDataservice StratumDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDataservice { get; }

        public Stratum Stratum
        {
            get => _stratum;
            private set
            {
                SetProperty(ref _stratum, value);
                OnStratumChanged(value);
            }
        }

        private void OnStratumChanged(Stratum newStratum)
        {
            CuttingUnits = (newStratum != null) ? CuttingUnitDataservice.GetCuttingUnitCodesByStratum(newStratum.StratumCode)
                : Enumerable.Empty<string>();


            if (newStratum is not null)
            {
                var method = newStratum.Method;
                IsPlot = CruiseMethods.PLOT_METHODS.Contains(method) || method == CruiseMethods.FIXCNT || method == CruiseMethods.THREEPPNT;
                IsVariableRadiousPlot = CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(method);
                IsFixedSizePlot = Fixed_Size_Plot_Methods.Contains(method);
                Is3PPNT = method == CruiseMethods.THREEPPNT;
                IsFixCNT = method == CruiseMethods.FIXCNT;
            }

        }

        [ObservableProperty]
        private bool _isPlot;
        [ObservableProperty]
        private bool _isVariableRadiousPlot;
        [ObservableProperty]
        private bool _isFixedSizePlot;
        [ObservableProperty]
        private bool _is3PPNT;
        [ObservableProperty]
        private bool _isFixCNT;

        public IEnumerable<string> CuttingUnits
        {
            get => _cuttingUnits;
            protected set => SetProperty(ref _cuttingUnits, value);
        }

        protected override void OnInitialize(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new ArgumentNullException(nameof(parameters)); }

            base.OnInitialize(parameters);

            var stratumCode = Parameters.GetValue<string>(NavParams.STRATUM);
            Load(stratumCode);
        }

        public void Load(string stratumCode)
        {
            Stratum = StratumDataservice.GetStratum(stratumCode);
            CuttingUnits = CuttingUnitDataservice.GetCuttingUnitCodesByStratum(stratumCode);
        }
    }
}
