using CruiseDAL.Schema;
using NatCruise.Wpf.Data;
using NatCruise.Wpf.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.ViewModels
{
    public class StratumDetailPageViewModel : BindableBase
    {
        private Stratum _stratum;
        private IEnumerable<Method> _methods;
        private IEnumerable<string> _hotKeyOptions;

        public StratumDetailPageViewModel(IDataserviceProvider dataserviceProvider)
        {
            var stratumDataservice = dataserviceProvider.GetDataservice<IStratumDataservice>();
            StratumDataservice = stratumDataservice ?? throw new ArgumentNullException(nameof(stratumDataservice));
        }

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                if(_stratum != null) { _stratum.PropertyChanged -= Stratum_PropertyChanged; }
                _stratum = value;
                OnStratumChanged(value);
                RaisePropertyChanged();
            }
        }

        private void OnStratumChanged(Stratum stratum)
        {
            if(stratum == null) { return; }


            Methods = StratumDataservice.GetMethods();

            HotKeyOptions = new string[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H",
                "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "X", "Y", "Z",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
            };


            var stratumCode = Stratum.StratumCode;
            CuttingUnits = StratumDataservice.GetCuttingUnitCodesByStratum(stratumCode);
            stratum.PropertyChanged += Stratum_PropertyChanged;
        }

        public IEnumerable<string> CuttingUnits { get; set; }

        public IEnumerable<Method> Methods
        {
            get => _methods;
            protected set => SetProperty(ref _methods, value);
        }

        public IEnumerable<string> HotKeyOptions
        {
            get => _hotKeyOptions;
            protected set => SetProperty(ref _hotKeyOptions, value);
        }

        protected IStratumDataservice StratumDataservice { get; }

        public bool IsPlot => CruiseMethods.PLOT_METHODS.Contains(Stratum?.Method);

        private void Stratum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var stratum = Stratum;

            if(stratum == null) { return; }
            StratumDataservice.UpdateStratum(stratum);

            if(e.PropertyName == nameof(Stratum.Method))
            { RaisePropertyChanged(nameof(IsPlot)); }
        }
    }
}
