using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using System;
using System.Linq;

namespace FScruiser.XF.ViewModels
{
    public class StratumDetailViewModel : ViewModelBase
    {
        public readonly string[] Fixed_Size_Plot_Methods = new string[] { CruiseMethods.FIX, CruiseMethods.FCM, CruiseMethods.F3P, CruiseMethods.FIXCNT };
        private Stratum _stratum;

        public StratumDetailViewModel(IStratumDataservice stratumDataservice)
        {
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
        }

        public IStratumDataservice StratumDataservice { get; }

        public Stratum Stratum
        {
            get => _stratum;
            private set
            {
                SetProperty(ref _stratum, value);
                NotifyCruiseMethodChanged();
            }
        }

        public bool IsPlot => CruiseMethods.PLOT_METHODS.Contains(Stratum?.Method) || Stratum?.Method == CruiseMethods.FIXCNT || Stratum?.Method == CruiseMethods.THREEPPNT;
        public bool IsVariableRariousePlot => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Stratum?.Method);
        public bool IsFixedSizePlot => Fixed_Size_Plot_Methods.Contains(Stratum?.Method);
        public bool Is3PPNT => Stratum?.Method == CruiseMethods.THREEPPNT;
        public bool IsFixCNT => Stratum?.Method == CruiseMethods.FIXCNT;

        protected void NotifyCruiseMethodChanged()
        {
            RaisePropertyChanged(nameof(IsPlot));
            RaisePropertyChanged(nameof(IsFixedSizePlot));
            RaisePropertyChanged(nameof(IsVariableRariousePlot));
            RaisePropertyChanged(nameof(Is3PPNT));
            RaisePropertyChanged(nameof(IsFixCNT));
        }

        public override void Load()
        {
            base.Load();

            var stratumCode = Parameters.GetValue<string>(NavParams.STRATUM);
            Stratum = StratumDataservice.GetStratum(stratumCode);
        }
    }
}