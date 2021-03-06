﻿using FScruiser.XF.Constants;
using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Data;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class TreeErrorEditViewModel : XamarinViewModelBase
    {
        private int _treeNumber;
        private TreeError _treeError;
        private string _treeAuditRuleID;
        private ICommand _saveCommand;

        public TreeErrorEditViewModel(IDataserviceProvider datastoreProvider, ICruiseDialogService dialogService)
        {
            Datastore = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public ICommand SaveCommand => _saveCommand ??= new Command(Save);

        private ICuttingUnitDatastore Datastore { get; set; }
        public ICruiseDialogService DialogService { get; }
        public int TreeNumber { get => _treeNumber; set => SetProperty(ref _treeNumber, value); }

        protected TreeError TreeError
        {
            get => _treeError;
            set
            {
                SetProperty(ref _treeError, value);
                RaisePropertyChanged(nameof(IsResolved));
                RaisePropertyChanged(nameof(Message));
                RaisePropertyChanged(nameof(Level));
                RaisePropertyChanged(nameof(Resolution));
                RaisePropertyChanged(nameof(Initials));
            }
        }

        public string Message => TreeError?.Message;

        public string Level => TreeError?.Level;

        /// <summary>
        /// Gets or sets the <see cref="IsResolved" /> property. This is a bindable property.
        /// </summary>
        public bool IsResolved
        {
            get => TreeError?.IsResolved ?? default(bool);
            set
            {
                var treeError = TreeError;
                if (treeError != null)
                {
                    treeError.IsResolved = value;
                    RaisePropertyChanged(nameof(IsResolved));
                }
            }
        }

        public string Resolution
        {
            get => TreeError?.Resolution;
            set
            {
                var treeError = TreeError;
                if (treeError != null)
                {
                    treeError.Resolution = value;
                }
            }
        }

        public string Initials
        {
            get => TreeError?.ResolutionInitials;
            set
            {
                var treeError = TreeError;
                if (treeError != null)
                {
                    treeError.ResolutionInitials = value;
                }
            }
        }

        public string TreeAuditRuleID { get => _treeAuditRuleID; set => SetProperty(ref _treeAuditRuleID, value); }

        //private bool ValidateForm()
        //{
        //    if(IsSuppressed)
        //    {
        //        return !(string.IsNullOrWhiteSpace(Remarks) && string.IsNullOrWhiteSpace(Initials));
        //    }
        //    return true;
        //}

        protected override void Load(IParameters parameters)
        {
            if (parameters is null) { throw new System.ArgumentNullException(nameof(parameters)); }

            var treeID = parameters.GetValue<string>(NavParams.TreeID);
            var treeAuditRuleID = parameters.GetValue<string>(NavParams.TreeAuditRuleID);

            var datastore = Datastore;
            var treeNumber = datastore.GetTreeNumber(treeID);
            var treeError = datastore.GetTreeError(treeID, treeAuditRuleID);

            TreeError = treeError;
            TreeNumber = treeNumber.GetValueOrDefault();
        }

        public void Save()
        {
            var isResolved = IsResolved;
            var treeError = TreeError;
            if (treeError == null) { return; }
            var treeID = treeError.TreeID;
            var treeAuditRuleID = treeError.TreeAuditRuleID;
            var remarks = Resolution;
            var sig = Initials;

            if (isResolved == true)
            {
                if (remarks == null)
                {
                    DialogService.ShowMessageAsync("Remarks required");
                    return;
                }
                if (sig == null)
                {
                    DialogService.ShowMessageAsync("Initials required");
                    return;
                }
                Datastore.SetTreeAuditResolution(treeID, treeAuditRuleID, remarks, sig);
            }
            else
            {
                Datastore.ClearTreeAuditResolution(treeID, treeAuditRuleID);
            }
        }
    }
}