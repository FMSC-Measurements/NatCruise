using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using NatCruise.Cruise.Data;
using NatCruise.Cruise.Models;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotTreeListViewModel : XamarinViewModelBase
    {
        public const string ALL_PLOTS_FILTEROPTION = "ALL";
        private IEnumerable<Tree_Ex> _allTrees;
        private IEnumerable<string> _plotFilterOptions;
        private bool _onlyShowTreesWithErrorsOrWarnings;
        private string _plotFilter;
        private Command<Tree_Ex> _deleteTreeCommand;
        private Command<Tree_Ex> _editTreeCommand;
        private Command<Tree_Ex> _showLogsCommand;
        private IEnumerable<TreeField> _treeFields;
        private CuttingUnit _cuttingUnit;

        public ITreeDataservice TreeDataservice { get; }
        public IPlotDataservice PlotDataservice { get; }
        public ICuttingUnitDataservice CuttingUnitDeataservice { get; }
        public ICruiseNavigationService NavigationService { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }

        public ICommand DeleteTreeCommand => _deleteTreeCommand ??= new Command<Tree_Ex>(DeleteTree);

        public ICommand EditTreeCommand => _editTreeCommand ??= new Command<Tree_Ex>((tree) =>
                 {
                     if (tree != null) NavigationService.ShowTreeEdit(tree.TreeID);
                 });

        public ICommand ShowLogsCommand => _showLogsCommand ??= new Command<Tree_Ex>((tree) =>
                 {
                     if (tree != null) NavigationService.ShowLogsList(tree.TreeID);
                 });

        public string Title => $"Unit {CuttingUnit?.CuttingUnitCode} - {CuttingUnit?.Description} Plot Trees";

        public CuttingUnit CuttingUnit
        {
            get => _cuttingUnit;
            protected set
            {
                SetProperty(ref _cuttingUnit, value);
                RaisePropertyChanged(nameof(Title));
            }
        }

        public IEnumerable<Tree_Ex> AllTrees
        {
            get => _allTrees;
            protected set
            {
                SetProperty(ref _allTrees, value);
                //RaisePropertyChanged(nameof(Trees));
            }
        }

        public IEnumerable<Tree_Ex> Trees => AllTrees?.Where(x =>
                         (PlotFilter == ALL_PLOTS_FILTEROPTION || x.PlotNumber.ToString() == PlotFilter) &&
                         (!OnlyShowTreesWithErrorsOrWarnings || x.ErrorCount > 0 || x.WarningCount > 0));

        public IEnumerable<string> PlotFilterOptions
        {
            get => _plotFilterOptions;
            protected set => SetProperty(ref _plotFilterOptions, value);
        }

        public string PlotFilter
        {
            get => _plotFilter;
            set
            {
                SetProperty(ref _plotFilter, value);
                RaisePropertyChanged(nameof(Trees));
            }
        }

        public bool OnlyShowTreesWithErrorsOrWarnings
        {
            get => _onlyShowTreesWithErrorsOrWarnings;
            set
            {
                SetProperty(ref _onlyShowTreesWithErrorsOrWarnings, value);
                RaisePropertyChanged(nameof(Trees));
            }
        }

        public IEnumerable<TreeField> TreeFields
        {
            get => _treeFields;
            protected set => SetProperty(ref _treeFields, value);
        }

        public PlotTreeListViewModel(ITreeDataservice treeDataservice,
            IPlotDataservice plotDataservice,
            ICuttingUnitDataservice cuttingUnitDataservice,
            ICruiseNavigationService cruiseNavigationService,
            ITreeFieldDataservice treeFieldDataservice)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            PlotDataservice = plotDataservice ?? throw new ArgumentNullException(nameof(plotDataservice));
            CuttingUnitDeataservice = cuttingUnitDataservice ?? throw new ArgumentNullException(nameof(cuttingUnitDataservice));
            NavigationService = cruiseNavigationService ?? throw new ArgumentNullException(nameof(cruiseNavigationService));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
        }

        protected override void Load(IParameters parameters)
        {
            base.Load(parameters);

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var cuttingUnit = CuttingUnit = CuttingUnitDeataservice.GetUnit(unitCode);

            var plotNumbers = PlotDataservice.GetPlotsByUnitCode(unitCode).Select(x => x.PlotNumber.ToString());
            PlotFilterOptions = new[] { ALL_PLOTS_FILTEROPTION }.Concat(plotNumbers).ToArray();

            if (IsLoaded is false)
            {
                TreeFields = TreeFieldDataservice.GetPlotTreeFields(unitCode);
            }

            AllTrees = TreeDataservice.GetPlotTreesByUnitCode(unitCode).ToArray();

            // initialize plot filter because setting plotFilterOptions causes binding to set
            // plot filter to null, so we have to initialize it here.
            PlotFilter = ALL_PLOTS_FILTEROPTION;
        }

        private void DeleteTree(Tree_Ex obj)
        {
            throw new NotImplementedException();
        }
    }
}