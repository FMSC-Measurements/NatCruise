using NatCruise.Cruise.Models;
using NatCruise.Cruise.Services;
using NatCruise.Cruise.Util;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using NatCruise.Util;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using NatCruise.Data;
using Prism.Common;

namespace FScruiser.XF.ViewModels
{
    public class TreeListViewModel : XamarinViewModelBase
    {
        private ICommand _deleteTreeCommand;
        private Command<TreeStub> _editTreeCommand;
        private ICollection<TreeStub> _trees;
        private Command _addTreeCommand;
        private Command<TreeStub> _showLogsCommand;

        public ICollection<TreeStub> Trees
        {
            get { return _trees; }
            protected set
            {
                SetProperty(ref _trees, value);
            }
        }

        public string[] StratumCodes { get; set; }

        public ICommand AddTreeCommand => _addTreeCommand ?? (_addTreeCommand = new Command(AddTreeAsync));

        public ICommand DeleteTreeCommand => _deleteTreeCommand ?? (_deleteTreeCommand = new Command<TreeStub>(DeleteTree));

        public ICommand EditTreeCommand => _editTreeCommand ?? (_editTreeCommand = new Command<TreeStub>(async (x) => await ShowEditTreeAsync(x)));

        public ICommand ShowLogsCommand => _showLogsCommand ?? (_showLogsCommand = new Command<TreeStub>(async (x) => await ShowLogsAsync(x)));

        public string UnitCode { get; set; }

        public ICuttingUnitDatastore Datastore { get; set; }

        public ICruiseDialogService DialogService { get; set; }
        public ICruiseNavigationService NavigationService { get; protected set; }

        public event EventHandler TreeAdded;

        public TreeListViewModel(
            ICruiseDialogService dialogService,
            ICruiseNavigationService navigationService,
            IDataserviceProvider datastoreProvider)
        {
            if (datastoreProvider is null) { throw new ArgumentNullException(nameof(datastoreProvider)); }

            Datastore = datastoreProvider.GetDataservice<ICuttingUnitDatastore>();
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        protected override void Load(IParameters parameters)
        {
            var unitCode = UnitCode = parameters.GetValue<string>(NavParams.UNIT);

            Trees = Datastore.GetTreeStubsByUnitCode(unitCode).ToObservableCollection();

            StratumCodes = Datastore.GetStrataProxiesByUnitCode(UnitCode).Select(x => x.StratumCode).ToArray();
        }

        public async void AddTreeAsync()
        {
            var datastore = Datastore;

            var stratumCode = await DialogService.AskValueAsync("Select Stratum", StratumCodes);

            
            if (stratumCode != null)
            {
                var sampleGroups = datastore.GetSampleGroupCodes(stratumCode).OrEmpty()
                    .ToArray();

                var sampleGroupCode = await DialogService.AskValueAsync("Select Sample Group", sampleGroups);

                if (sampleGroupCode != null)
                {
                    var tree_guid = datastore.CreateMeasureTree(UnitCode, stratumCode, sampleGroupCode);
                    var newTree = datastore.GetTreeStub(tree_guid);
                    _trees.Add(newTree);
                    OnTreeAdded(null);
                }
            }
        }

        public void OnTreeAdded(EventArgs e)
        {
            TreeAdded?.Invoke(this, e);
        }

        public Task ShowEditTreeAsync(TreeStub tree)
        {
            return NavigationService.ShowTreeEdit(tree.TreeID);
        }

        private void DeleteTree(TreeStub tree)
        {
            if (tree == null) { return; }

            Datastore.DeleteTree(tree.TreeID);
        }

        public Task ShowLogsAsync(TreeStub tree)
        {
            return NavigationService.ShowLogsList(tree.TreeID);
        }
    }
}