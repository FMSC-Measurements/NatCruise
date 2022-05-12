using NatCruise.Data;
using NatCruise.Models;
using System;
using System.Collections.Generic;
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

        private readonly IEnumerable<TreeField> COMMON_TREEFIELDS = new[]
        {
            new TreeField{DbType = "TEXT", Heading = "Cutting Unit", Field = nameof(Tree.CuttingUnitCode)},
            new TreeField{DbType = "TEXT", Heading = "Plot Number", Field = nameof(Tree.PlotNumber)},
            new TreeField{DbType = "TEXT", Heading = "Tree Number", Field = nameof(Tree.TreeNumber)},
            new TreeField{DbType = "TEXT", Heading = "Stratum", Field = nameof(Tree.StratumCode)},
            new TreeField{DbType = "TEXT", Heading = "Sample Group", Field = nameof(Tree.SampleGroupCode)},
            new TreeField{DbType = "TEXT", Heading = "Species", Field = nameof(Tree.SpeciesCode)},
            new TreeField{DbType = "TEXT", Heading = "Live/Dead", Field = nameof(Tree.LiveDead)},
        };

        public TreeListViewModel(ITreeDataservice treeDataservice, ITreeFieldDataservice treeFieldDataservice)
        {
            TreeDataservice = treeDataservice ?? throw new ArgumentNullException(nameof(treeDataservice));
            TreeFieldDataservice = treeFieldDataservice ?? throw new ArgumentNullException(nameof(treeFieldDataservice));
        }

        public ITreeDataservice TreeDataservice { get; }
        public ITreeFieldDataservice TreeFieldDataservice { get; }

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

        public IEnumerable<TreeEx> Trees
        {
            get => _trees;
            set => SetProperty(ref _trees, value);
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

            var trees = TreeDataservice.GetTrees(CuttingUnitCode, StratumCode, SampleGroupCode);
            Trees = trees;

            Fields = COMMON_TREEFIELDS.Concat(TreeFieldDataservice.GetTreeFieldsUsedInCruise()).ToArray();
        }
    }
}