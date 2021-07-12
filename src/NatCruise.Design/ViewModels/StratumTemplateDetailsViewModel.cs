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

        private StratumTemplate _stratumTemplate;
        private IEnumerable<TreeField> _treefieldOptions;
        private IEnumerable<CruiseMethod> _methods;

        public StratumTemplateDetailsViewModel(ISetupInfoDataservice setupDataservice, ITemplateDataservice templateDataservice)
        {
            SetupDataservice = setupDataservice ?? throw new ArgumentNullException(nameof(setupDataservice));
            TemplateDataservice = templateDataservice ?? throw new ArgumentNullException(nameof(templateDataservice));

            Methods = SetupDataservice.GetCruiseMethods().ToArray();
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

        public StratumTemplate StratumTemplate
        {
            get => _stratumTemplate;
            set
            {
                if(_stratumTemplate != null)
                {
                    _stratumTemplate.PropertyChanged -= StratumTemplate_PropertyChanged;
                }
                SetProperty(ref _stratumTemplate, value);
                if(value != null)
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
                TemplateDataservice.UpsertStratumTemplate(st);
            }
        }
    }
}
