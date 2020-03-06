using Backpack.XF.Converters;
using FScruiser.Models;
using FScruiser.XF.Converters;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FScruiser.XF.Util
{
    public class TreeEditControlFactory
    {
        static readonly NullableDoubleToStringConverter _nullableDoubleConverter = new NullableDoubleToStringConverter();
        static readonly NullableIntToStringConverter _nullableIntConverter = new NullableIntToStringConverter();

        public static View MakeEditView(TreeFieldValue tfv)
        {
            View editView = null;
            switch (tfv.DBType)
            {
                case "REAL":
                    {
                        editView = new Entry() { Keyboard = Keyboard.Numeric };
                        editView.SetBinding(Entry.TextProperty, "ValueReal", converter: _nullableDoubleConverter );
                        break;
                    }
                case "INT":
                case "INTEGER":
                    {
                        editView = new Entry() { Keyboard = Keyboard.Numeric };
                        editView.SetBinding(Entry.TextProperty, "ValueInt", converter: _nullableIntConverter);
                        break;
                    }
                case "TEXT":
                    {
                        editView = new Entry() { Keyboard = Keyboard.Text };
                        editView.SetBinding(Entry.TextProperty, "ValueText");
                        break;
                    }
                case "BOOL":
                case "BOOLEAN":
                    {
                        editView = new Switch();
                        editView.SetBinding(Switch.IsToggledProperty, "ValueBool");
                        break;
                    }
            }

            if(editView != null)
            {
                editView.BindingContext = tfv;
            }
            if (editView is Entry entry)
            {
                entry.Effects.Add(new Xamarin.Toolkit.Effects.EntrySelectAllText());
            }

            return editView;
        }

        //public static View MakeEditView(TreeFieldSetup field)
        //{
        //    View editView = null;
        //    switch (field.Field)
        //    {
        //        // unsupported fields 
        //        case "KPI":
        //        case "TreeCount":
        //        case "TreeNumber":
        //            {
        //                return null;
        //            }

        //        case "StratumCode":
        //        case "Stratum":
        //            {
        //                editView = MakeStratumPicker();
        //                break;
        //            }
        //        case "SampleGroupCode":
        //        case "Samplegroup":
        //            {
        //                editView = MakeSampleGroupPicker();
        //                break;
        //            }
        //        //case nameof(Tree.Species):
        //        //    {
        //        //        editView = MakeSpeciesPicker();
        //        //        break;
        //        //    }
        //        case nameof(Tree.CountOrMeasure):
        //            {
        //                editView = MakeCountMeasurePicker();
        //                break;
        //            }
        //        case nameof(TreeMeasurment.Aspect):
        //        case nameof(TreeMeasurment.CrownRatio):
        //        case nameof(TreeMeasurment.DBH):
        //        case nameof(TreeMeasurment.DBHDoubleBarkThickness):
        //        //case nameof(Tree.Diameter):
        //        case nameof(TreeMeasurment.DiameterAtDefect):
        //        case nameof(TreeMeasurment.DRC):
        //        case nameof(TreeMeasurment.FormClass):
        //        //case nameof(Tree.Height):
        //        case nameof(TreeMeasurment.HeightToFirstLiveLimb):
        //        case nameof(TreeMeasurment.MerchHeightPrimary):
        //        case nameof(TreeMeasurment.MerchHeightSecondary):
        //        case nameof(TreeMeasurment.PoleLength):
        //        case nameof(TreeMeasurment.RecoverablePrimary):
        //        case nameof(TreeMeasurment.SeenDefectPrimary):
        //        case nameof(TreeMeasurment.SeenDefectSecondary):
        //        case nameof(TreeMeasurment.Slope):
        //        case nameof(TreeMeasurment.TopDIBPrimary):
        //        case nameof(TreeMeasurment.TopDIBSecondary):
        //        case nameof(TreeMeasurment.TotalHeight):
        //        case nameof(TreeMeasurment.UpperStemDiameter):
        //        case nameof(TreeMeasurment.UpperStemHeight):
        //        case nameof(TreeMeasurment.VoidPercent):
        //            {
        //                editView = new Entry();
        //                ((InputView)editView).Keyboard = Keyboard.Numeric;
        //                ((Entry)editView).Behaviors.Add(new Xamarin.Toolkit.Behaviors.NumericValidationBehavior { TextColorInvalid = Color.Red });
        //                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
        //                editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
        //                break;
        //            }
        //        case nameof(TreeMeasurment.DefectCode):
        //            {
        //                editView = new Entry();
        //                ((InputView)editView).Keyboard = Keyboard.Default;
        //                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
        //                editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
        //                break;
        //            }
        //        default:
        //            {
        //                editView = new Entry();
        //                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Entry.SetImeOptions(editView, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ImeFlags.Next);
        //                editView.SetBinding(Entry.TextProperty, $"Tree.{field.Field}");
        //                break;
        //            }
        //    }

        //    if (editView is Entry entry)
        //    {
        //        entry.Effects.Add(new Xamarin.Toolkit.Effects.EntrySelectAllText());
        //    }

        //    return editView;
        //}

        public static View MakeStratumPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.StratumCodes));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.StratumCode));
            //view.ItemDisplayBinding = new Binding(nameof(Stratum.Code));

            return view;
        }

        public static Picker MakeCountMeasurePicker()
        {
            var picker = new Picker();
            picker.Items.Add(string.Empty);
            picker.Items.Add("C");
            picker.Items.Add("M");
            picker.SetBinding(Picker.SelectedItemProperty, $"Tree.{nameof(Tree.CountOrMeasure)}");

            return picker;
        }

        public static Picker MakeSampleGroupPicker()
        {
            var view = new Picker();
            view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.SampleGroupCodes));
            view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.SampleGroupCode));
            //view.ItemDisplayBinding = new Binding(nameof(SampleGroup.Code));

            return view;
        }

        //public static Picker MakeSpeciesPicker()
        //{
        //    var view = new Picker();
        //    view.SetBinding(Picker.ItemsSourceProperty, nameof(TreeEditViewModel.TreeDefaults));
        //    view.SetBinding(Picker.SelectedItemProperty, nameof(TreeEditViewModel.TreeDefault));
        //    //view.ItemDisplayBinding = new Binding(nameof(TreeDefaultValueDO.Species));

        //    return view;
        //}
    }
}
