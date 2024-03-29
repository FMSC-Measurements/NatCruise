﻿using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using Prism.Commands;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NatCruise.MVVM.ViewModels
{
    public class TreeErrorEditViewModel : ViewModelBase
    {
        private int _treeNumber;
        private TreeError _treeError;
        private string _treeAuditRuleID;
        private ICommand _saveCommand;

        public TreeErrorEditViewModel(ITreeDataservice treeDataservice,
            ITreeErrorDataservice treeErrorDataservice,
            INatCruiseDialogService dialogService,
            INatCruiseNavigationService navigationService)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TreeErrorDataservice = treeErrorDataservice ?? throw new ArgumentNullException(nameof(treeErrorDataservice));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public EventHandler Saved;


        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand(Save);

        protected ITreeDataservice TreeDataservice { get; }
        public ITreeErrorDataservice TreeErrorDataservice { get; }
        protected INatCruiseDialogService DialogService { get; }
        public INatCruiseNavigationService NavigationService { get; }
        public int TreeNumber { get => _treeNumber; set => SetProperty(ref _treeNumber, value); }

        protected TreeError TreeError
        {
            get => _treeError;
            set
            {
                SetProperty(ref _treeError, value);
                OnPropertyChanged(nameof(IsResolved));
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(Level));
                OnPropertyChanged(nameof(Resolution));
                OnPropertyChanged(nameof(Initials));
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
                    OnPropertyChanged(nameof(IsResolved));
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

        protected override void Load(IDictionary<string, object> parameters)
        {
            if (parameters is null) { throw new System.ArgumentNullException(nameof(parameters)); }

            var treeID = parameters.GetValue<string>(NavParams.TreeID);
            var treeAuditRuleID = parameters.GetValue<string>(NavParams.TreeAuditRuleID);

            Load(treeID, treeAuditRuleID);
        }

        public void Load(string treeID, string treeAuditRuleID)
        {
            var treeNumber = TreeDataservice.GetTreeNumber(treeID);
            var treeError = TreeErrorDataservice.GetTreeError(treeID, treeAuditRuleID);

            TreeError = treeError;
            TreeNumber = treeNumber.GetValueOrDefault();
        }

        public void Save()
        {
            var isResolved = IsResolved;

            if (isResolved == false)
            {
                SuppressError();
            }
            else
            {
                UnSupressError();
            }
            Saved?.Invoke(this, EventArgs.Empty);
        }

        public void SuppressError()
        {
            var treeError = TreeError;
            if (treeError == null) { return; }
            var treeID = treeError.TreeID;
            var treeAuditRuleID = treeError.TreeAuditRuleID;
            var remarks = Resolution;
            var sig = Initials;

            if (sig == null)
            {
                DialogService.ShowMessageAsync("Initials required");
                return;
            }
            TreeErrorDataservice.SetTreeAuditResolution(treeID, treeAuditRuleID, remarks, sig);

            Saved?.Invoke(this, EventArgs.Empty);
        }

        public void UnSupressError()
        {
            var treeError = TreeError;
            if (treeError == null) { return; }
            var treeID = treeError.TreeID;
            var treeAuditRuleID = treeError.TreeAuditRuleID;

            TreeErrorDataservice.ClearTreeAuditResolution(treeID, treeAuditRuleID);

            Saved?.Invoke(this, EventArgs.Empty);
        }
    }
}