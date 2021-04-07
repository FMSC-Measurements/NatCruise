using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupDetailViewModel : ViewModelBase
    {
        private SampleGroup _sampleGroup;
        private string _method;
        private IEnumerable<string> _sampleSelectorTypeOptions;

        public SampleGroupDetailViewModel(IDataserviceProvider dataserviceProvider, ISetupInfoDataservice setupInfo)
        {
            var sampleGroupDataservice = dataserviceProvider.GetDataservice<ISampleGroupDataservice>();
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            SetupDataservice = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
        }

        public ISampleGroupDataservice SampleGroupDataservice { get; }
        public ISetupInfoDataservice SetupDataservice { get; }

        public IEnumerable<string> DefaultLiveDeadOptions { get; } = new[] { "L", "D" };

        public IEnumerable<Product> ProductOptions { get; protected set; }

        public SampleGroup SampleGroup
        {
            get => _sampleGroup;
            set
            {
                OnSampleGroupChanging(_sampleGroup);
                SetProperty(ref _sampleGroup, value);
                OnSampleGroupChanged(value);
            }
        }

        private void OnSampleGroupChanging(SampleGroup oldSampleGroup)
        {
            if (oldSampleGroup != null) { oldSampleGroup.PropertyChanged -= SampleGroup_PropertyChanged; }
        }

        private void OnSampleGroupChanged(SampleGroup sampleGroup)
        {
            if (sampleGroup == null) { return; }

            Method = SampleGroupDataservice.GetMethod(sampleGroup.StratumCode);
            sampleGroup.PropertyChanged += SampleGroup_PropertyChanged;
        }

        private void SampleGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var sampleGroup = (SampleGroup)sender;
            SampleGroupDataservice.UpdateSampleGroup(sampleGroup);
        }

        public string Method
        {
            get => _method;
            protected set
            {
                SetProperty(ref _method, value);
                RaisePropertyChanged(nameof(IsSTR));
                RaisePropertyChanged(nameof(Is3P));
                RaisePropertyChanged(nameof(IsVariableRadious));
            }
        }

        public bool IsSTR => Method == CruiseMethods.STR;
        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);
        public IEnumerable<string> SampleSelectorTypeOptions
        {
            get => _sampleSelectorTypeOptions;
            set => SetProperty(ref _sampleSelectorTypeOptions, value);
        }

        public bool IsVariableRadious => CruiseMethods.VARIABLE_RADIUS_METHODS.Contains(Method);

        //protected override void Refresh(CruiseManagerNavigationParamiters navParams)
        //{
        //    var sampleGroupCode = navParams.SampleGroupCode;
        //    var stratum = navParams.StratumCode;

        //    SampleGroup = SampleGroupDataservice.GetSampleGroup(stratum, sampleGroupCode);

        //    Method = SampleGroupDataservice.GetMethod(stratum);
        //    RaisePropertyChanged(nameof(Is3P));
        //    RaisePropertyChanged(nameof(IsSTR));
        //    RaisePropertyChanged(nameof(IsVariableRadious));
        //}

        public override void Load()
        {
            var sampleGroup = SampleGroup;
            if (sampleGroup == null) { return; }

            ProductOptions = SetupDataservice.GetProducts();
            var method = SampleGroupDataservice.GetMethod(sampleGroup.StratumCode);
            Method = method;

            switch(method)
            {
                case CruiseMethods.STR:
                case CruiseMethods.PCM:
                case CruiseMethods.FCM:
                    {
                        SampleSelectorTypeOptions = new[]
                          {
                            SampleGroupTableDefinition.SAMPLESELECTORTYPE_BLOCKSELECTER,
                            SampleGroupTableDefinition.SAMPLESELECTORTYPE_SYSTEMATICSELECTER,
                            SampleGroupTableDefinition.SAMPLESELECTORTYPE_CLICKERSELECTER
                        };
                        break;
                    }
            }
        }
    }
}