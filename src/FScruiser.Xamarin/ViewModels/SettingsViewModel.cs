﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CruiseDAL;
using FScruiser.XF.Data;
using FScruiser.XF.Services;
using NatCruise.Services;
using Prism.Navigation;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public IApplicationSettings AppSettings { get; }
        public IDialogService DialogService { get; }
        public IFileSystemService FileSystemService { get; }

        public ICommand ResetDatabaseCommand => new Command(() => ResetDatabase());

        public SettingsViewModel(IDialogService dialogService, IFileSystemService fileSystemService)
        {
            AppSettings = new ApplicationSettings();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            FileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
        }

        public async void ResetDatabase()
        {
            if (await DialogService.AskYesNoAsync("This will delete all cruise data do you want to continue", "Warning", defaultNo: true))
            {
                var databasePath = FileSystemService.DefaultCruiseDatabasePath;
                var newDatabase = new CruiseDatastore_V3(databasePath, true);
            }
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            AppSettings.Save();
        }
    }
}
