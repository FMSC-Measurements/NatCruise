﻿using NatCruise.Data;
using NatCruise.Design.Data;
using NatCruise.Design.Validation;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using NatCruise.Services;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NatCruise.Design.ViewModels
{
    public class SampleGroupListViewModel : ViewModelBase
    {
        private DelegateCommand<string> _addSampleGroupCommand;
        private DelegateCommand<SampleGroup> _removeSampleGroupCommand;
        private ObservableCollection<SampleGroup> _sampleGroups;
        private Stratum _stratum;
        private SampleGroup _selectedSampleGroup;

        public SampleGroupListViewModel(IDataserviceProvider datastoreProvider, INatCruiseDialogService dialogService)
        {
            var sampleGroupDataservice = datastoreProvider.GetDataservice<ISampleGroupDataservice>();

            SampleGroupDataservice = sampleGroupDataservice ?? throw new ArgumentNullException(nameof(sampleGroupDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SampleGroupValidator = new SampleGroupValidator();
        }

        public event EventHandler SampleGroupAdded;

        protected ISampleGroupDataservice SampleGroupDataservice { get; }
        public INatCruiseDialogService DialogService { get; }
        public SampleGroupValidator SampleGroupValidator { get; }

        public ICommand AddSampleGroupCommand => _addSampleGroupCommand ??= new DelegateCommand<string>(AddSampleGroup);
        public ICommand RemoveSampleGroupCommand => _removeSampleGroupCommand ??= new DelegateCommand<SampleGroup>(RemoveSampleGroup);

        public Stratum Stratum
        {
            get => _stratum;
            set
            {
                SetProperty(ref _stratum, value);
                LoadSampleGroups(value);
            }
        }

        public ObservableCollection<SampleGroup> SampleGroups
        {
            get => _sampleGroups;
            protected set
            {
                if(_sampleGroups != null)
                {
                    foreach(var sg in _sampleGroups)
                    {
                        sg.PropertyChanged -= samplegroup_PropertyChanged;
                    }
                }
                SetProperty(ref _sampleGroups, value);
                if(value != null)
                {
                    foreach(var sg in value)
                    {
                        sg.PropertyChanged += samplegroup_PropertyChanged;
                    }
                }
            }
        }

        private void samplegroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SampleGroup.Errors)) { return; }
            if(sender is SampleGroup sg && sg != null)
            {
                ValidateSampeleGroup(sg);
            }
        }

        public SampleGroup SelectedSampleGroup
        {
            get => _selectedSampleGroup;
            set
            {
                SetProperty(ref _selectedSampleGroup, value);
                if(value != null)
                {
                    ValidateSampeleGroup(value);
                }
            }
        }

        public void AddSampleGroup(string code)
        {
            code = code.Trim();
            if (Regex.IsMatch(code, "^[a-zA-Z0-9]+$") is false) { return; }

            var newSampleGroup = new SampleGroup()
            {
                SampleGroupCode = code,
                StratumCode = Stratum.StratumCode,
                CruiseMethod = Stratum.Method,
                CutLeave = "C",
                DefaultLiveDead = "L",
            };

            try
            {
                SampleGroupDataservice.AddSampleGroup(newSampleGroup);
                newSampleGroup.PropertyChanged += samplegroup_PropertyChanged;
                SampleGroups.Add(newSampleGroup);
                SampleGroupAdded?.Invoke(this, EventArgs.Empty);
                SelectedSampleGroup = newSampleGroup;
            }
            catch (FMSC.ORM.UniqueConstraintException)
            {
                DialogService.ShowNotification("Sample Group Code Already Exists");
            }
        }

        public void RemoveSampleGroup(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }
            var sampleGroups = SampleGroups;

            SampleGroupDataservice.DeleteSampleGroup(sampleGroup);
            var index = sampleGroups.IndexOf(sampleGroup);
            sampleGroups.Remove(sampleGroup);
            sampleGroup.PropertyChanged -= samplegroup_PropertyChanged;

            if (index < 0) { return; }
            if (index <= sampleGroups.Count - 1)
            {
                var newSelectedSg = sampleGroups[index];
                SelectedSampleGroup = newSelectedSg;
            }
            else
            {
                SelectedSampleGroup = sampleGroups.LastOrDefault();
            }
        }

        protected void LoadSampleGroups(Stratum stratum)
        {
            if (stratum != null)
            {
                var sampleGroups = SampleGroupDataservice.GetSampleGroups(stratum.StratumCode).ToArray();

                foreach (var sampleGroup in sampleGroups)
                {
                    ValidateSampeleGroup(sampleGroup);
                }

                SampleGroups = new ObservableCollection<SampleGroup>(sampleGroups);

                
            }
        }

        public override void Load()
        {
            base.Load();

            LoadSampleGroups(Stratum);
        }

        public void ValidateSampeleGroup(SampleGroup sampleGroup)
        {
            if (sampleGroup is null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            var errors = SampleGroupValidator.Validate(sampleGroup).Errors
                .Where(x => x.Severity == FluentValidation.Severity.Error)
                .Select(x => x.ErrorMessage).ToArray();

            sampleGroup.Errors = errors;
        }
    }
}