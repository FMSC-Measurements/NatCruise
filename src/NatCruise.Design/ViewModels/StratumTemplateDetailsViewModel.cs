using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateDetailsViewModel : ViewModelBase
    {
        public readonly string[] YealdComponent_Options = new string[] { "CL", "CD", "NL", "ND", };

        private StratumDefault _stratumDefault;
        private IEnumerable<TreeField> _treefieldOptions;
        private IEnumerable<CruiseMethod> _methods;

        public StratumTemplateDetailsViewModel(ISetupInfoDataservice setupDataservice, ITemplateDataservice templateDataservice)
        {
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));
        }

        public ISetupInfoDataservice SetupDataservice { get; }
        public ITemplateDataservice TemplateDataservice { get; }

        public IEnumerable<string> YieldComponentOptions => YealdComponent_Options;

        public IEnumerable<TreeField> TreeFieldOptions
        {
            get => _treefieldOptions;
            protected set => SetProperty(ref _treefieldOptions, value);
        }

        public IEnumerable<CruiseMethod> Methods
        {
            get => _methods;
            set => SetProperty(ref _methods, value);
        }

        public StratumDefault StratumDefault
        {
            get => _stratumDefault;
            set
            {
                if(_stratumDefault != null)
                {
                    _stratumDefault.PropertyChanged -= StratumDefault_PropertyChanged;
                }
                SetProperty(ref _stratumDefault, value);
                if(value != null)
                {
                    value.PropertyChanged += StratumDefault_PropertyChanged;
                }
            }
        }

        public override void Load()
        {
            base.Load();

            Methods = SetupDataservice.GetCruiseMethods().ToArray();
        }

        private void StratumDefault_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var std = sender as StratumDefault;
            if (std != null)
            {
                TemplateDataservice.UpdateStratumDefault(std);
            }
        }
    }
}
