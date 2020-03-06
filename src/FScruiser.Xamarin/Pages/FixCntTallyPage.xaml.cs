using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FixCntTallyPage : ContentPage
    {
        public FixCntTallyPage()
        {
            InitializeComponent();

            //Models.FixCntTallyPopulation MakeTallyPop(string fieldName)
            //{
            //    var tallyPop = new Models.FixCntTallyPopulation() { FieldName = fieldName };
            //    var buckets = new List<Models.FixCNTTallyBucket>()
            //    {
            //        new Models.FixCNTTallyBucket(tallyPop, 1.0, 0),
            //        new Models.FixCNTTallyBucket(tallyPop, 2.0, 0),
            //        new Models.FixCNTTallyBucket(tallyPop, 3.0, 0),
            //        new Models.FixCNTTallyBucket(tallyPop, 4.0, 0),
            //        new Models.FixCNTTallyBucket(tallyPop, 5.0, 0),
            //    };

            //    tallyPop.Buckets = buckets;
            //    return tallyPop;
            //}

            //var viewMode = new ViewModels.FixCNTViewModel()
            //{
            //    TallyPopulations = new Models.FixCntTallyPopulation[]
            //    {
            //        MakeTallyPop("DBH"),
            //    }
            //};

            //BindingContext = viewMode;
        }
    }
}