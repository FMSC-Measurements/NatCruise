using NatCruise.Data;
using NatCruise.Models;
using NatCruise.MVVM;
using NatCruise.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NatCruise.Wpf.FieldData.ViewModels
{
    public class TreeListViewModel : ViewModelBase
    {
        private IEnumerable<TreeEx> _trees;
        private string _cuttingUnitCode;
        private string _stratumCode;
        private string _sampleGroupCode;
        private IEnumerable<TreeField> _fields;
        private IEnumerable<int> _plotOptions;
        private int? _plotNumber;
        private TreeEx _selectedTree;
        private readonly IEnumerable<TreeField> COMMON_TREEFIELDS = new[]
        {
            new TreeField{DbType = "TEXT", Heading = "Cutting Unit", Field = nameof(Tree.CuttingUnitCode)},
            new TreeField{DbType = "TEXT", Heading = "Plot Number", Field = nameof(Tree.PlotNumber)},
            new TreeField{DbType = "TEXT", Heading = "Tree Number", Field = nameof(Tree.TreeNumber)},
            new TreeField{DbType = "TEXT", Heading = "Stratum", Field = nameof(Tree.StratumCode)},
            new TreeField{DbType = "TEXT", Heading = "Sample Group", Field = nameof(Tree.SampleGroupCode)},
            new TreeField{DbType = "TEXT", Heading = "Species", Field = nameof(Tree.SpeciesCode)},
            new TreeField{DbType = "TEXT", Heading = "Live/Dead", Field = nameof(Tree.LiveDead)},
            new TreeField{DbType = "TEXT", Heading = "Count/Measure", Field = nameof(Tree.CountOrMeasure)},
        };

        public static readonly IEnumerable<string> LOCKED_FIELDS = new[]
        {
            nameof(Tree.TreeID),
            nameof(Tree.CuttingUnitCode),
            nameof(Tree.PlotNumber),
        };

        public TreeListViewModel(ITreeDataservice treeDataservice,
                                 ITreeFieldDataservice treeFieldDataservice,
                                 INatCruiseDialogService natCruiseDialogService,
                                TreeEditViewModel treeEditViewModel)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
            NatCruiseDialogService = natCruiseDialogService ?? throw new ArgumentNullException(nameof(natCruiseDialogService));
            TreeEditViewModel = treeEditViewModel ?? throw new ArgumentNullException(nameof(treeEditViewModel));
        }

        public ITreeDataservice TreeDataservice { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }
        public INatCruiseDialogService NatCruiseDialogService { get; }
        public TreeEditViewModel TreeEditViewModel { get; }

        public string CuttingUnitCode
        {
            get => _cuttingUnitCode;
            set
            {
                SetProperty(ref _cuttingUnitCode, value);
                Load();
            }
        }

        public string StratumCode
        {
            get => _stratumCode;
            set
            {
                SetProperty(ref _stratumCode, value);
                Load();
            }
        }

        public string SampleGroupCode
        {
            get => _sampleGroupCode;
            set
            {
                SetProperty(ref _sampleGroupCode, value);
                Load();
            }
        }



        public int? PlotNumber
        {
            get => _plotNumber;
            set
            {
                SetProperty(ref _plotNumber, value);
                Load();
            }
        }

        public IEnumerable<TreeEx> Trees
        {
            get => _trees;
            set
            {
                if (_trees != null)
                {
                    foreach (var tree in _trees)
                    { tree.PropertyChanged -= Tree_PropertyChanged; }
                }
                SetProperty(ref _trees, value);
                if (value != null)
                {
                    foreach (var tree in value)
                    { tree.PropertyChanged += Tree_PropertyChanged; }
                }
            }
        }

        

        private void Tree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //var tree = sender as TreeEx;
            //if (tree == null) { return; }
            //var propertyName = e.PropertyName;

            //if (LOCKED_FIELDS.Contains(propertyName)) { return; }

            //if (propertyName == nameof(Tree.TreeNumber))
            //{
            //    var newTreeNumber = tree.TreeNumber;
            //    var unit = tree.CuttingUnitCode;
            //    var plotNumber = tree.PlotNumber;
            //    if (TreeDataservice.IsTreeNumberAvalible(unit, newTreeNumber, plotNumber: plotNumber))
            //    {
            //        TreeDataservice.UpdateTree(tree);
            //    }
            //    else
            //    {
            //        var treeNumber = TreeDataservice.GetTreeNumber(tree.TreeID);
            //        tree.TreeNumber = treeNumber.Value;
            //        NatCruiseDialogService.ShowMessageAsync("Tree Number Already Exists");
            //    }
            //}
            //else
            //{
            //    TreeDataservice.UpdateTree(tree);
            //}
        }

        public TreeEx SelectedTree
        {
            get => _selectedTree;
            set
            {
                SetProperty(ref _selectedTree, value);
                TreeEditViewModel.Tree = value;
            }
        }

        public IEnumerable<TreeField> Fields
        {
            get => _fields;
            protected set
            {
                SetProperty(ref _fields, value);
            }
        }

        public override void Load()
        {
            base.Load();

            var trees = TreeDataservice.GetTrees(cuttingUnitCode: CuttingUnitCode,
                stratumCode: StratumCode,
                sampleGroupCode: SampleGroupCode,
                plotNumber:PlotNumber);
            Trees = trees;

            Fields = COMMON_TREEFIELDS.Concat(TreeFieldDataservice.GetTreeFieldsUsedInCruise()).ToArray();
        }
    }
}