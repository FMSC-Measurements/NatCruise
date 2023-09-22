using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class StratumTemplateDetailsViewModel : ViewModelBase
    {
        public readonly string[] YieldComponent_Options = new string[] { "CL", "CD", "NL", "ND", };

        private StratumTemplate _stratumTemplate;
        private IEnumerable<TreeField> _treefieldOptions;
        private IEnumerable<CruiseMethod> _methods;

        public StratumTemplateDetailsViewModel(ISetupInfoDataservice setupDataservice, IStratumTemplateDataservice stratumTemplateDataservice)
        {
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            StratumTemplateDataservice = stratumTemplateDataservice ?? throw new ArgumentNullException(nameof(stratumTemplateDataservice));

            Methods = SetupDataservice.GetCruiseMethods().ToArray();
        }

        public ISetupInfoDataservice SetupDataservice { get; }
        public IStratumTemplateDataservice StratumTemplateDataservice { get; }

        public IEnumerable<string> YieldComponentOptions => YieldComponent_Options;

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

        public StratumTemplate StratumTemplate
        {
            get => _stratumTemplate;
            set
            {
                if (_stratumTemplate != null)
                {
                    _stratumTemplate.PropertyChanged -= StratumTemplate_PropertyChanged;
                }
                SetProperty(ref _stratumTemplate, value);
                if (value != null)
                {
                    value.PropertyChanged += StratumTemplate_PropertyChanged;
                }
            }
        }

        private void StratumTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var st = sender as StratumTemplate;
            if (st != null)
            {
                StratumTemplateDataservice.UpsertStratumTemplate(st);
            }
        }
    }
}