using CruiseDAL.Schema;
using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupDetailPageViewModel : ViewModelBase
    {
        private SampleGroup _sampleGroup;
        private string _method;

        public SampleGroupDetailPageViewModel(IDataserviceProvider dataserviceProvider)
        {
            var sampleGroupDataservice = dataserviceProvider.GetDataservice<ISampleGroupDataservice>();
            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            ProductOptions = sampleGroupDataservice.GetProducts();
        }

        public ISampleGroupDataservice SampleGroupDataservice { get; }

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

            Method = SampleGroupDataservice.GetMethod(sampleGroup.StratumCode);
        }
    }
}