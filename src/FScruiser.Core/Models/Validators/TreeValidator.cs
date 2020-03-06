using FScruiser.Validation;

namespace FScruiser.Models.Validators
{
    public class TreeValidator : Validator_Base<Tree_Ex>
    {
        public TreeValidator()
        {
            Rules.Add(new NotNullOrWhitespaceRule<Tree_Ex>(nameof(Tree_Ex.Species), ValidationLevel.Error, "Species is required"));
            Rules.Add(new NotNullOrWhitespaceRule<Tree_Ex>(nameof(Tree_Ex.SampleGroupCode), ValidationLevel.Error, "Sample Group is required"));
            Rules.Add(new NotNullOrWhitespaceRule<Tree_Ex>(nameof(Tree_Ex.StratumCode), ValidationLevel.Error, "Stratum is required"));
            Rules.Add(new NotNullOrWhitespaceRule<Tree_Ex>(nameof(Tree_Ex.CountOrMeasure), ValidationLevel.Error, "Count or Measure value invalid"));
            Rules.Add(new NotNullOrWhitespaceRule<Tree_Ex>(nameof(Tree.LiveDead), ValidationLevel.Error, "Live Dead can not be blank"));

            Rules.Add(new PredicateRule<Tree_Ex>(nameof(Tree_Ex.MerchHeightSecondary), "Merch Height Secondary must be greater than Merch Height Primary"
                , ValidationLevel.Error
                , x => x.MerchHeightSecondary > x.MerchHeightPrimary || TreeIsNotMeassure(x)));

            Rules.Add(new PredicateRule<Tree_Ex>(nameof(Tree_Ex.UpperStemHeight), "Upper Stem Height must be greater or equal to Merch Height Primary"
                , ValidationLevel.Error
                , x => x.UpperStemHeight >= x.MerchHeightPrimary || TreeIsNotMeassure(x)));

            Rules.Add(new PredicateRule<Tree_Ex>(nameof(Tree_Ex.UpperStemDiameter), "Upper Stem Diameter must be smaller than DBH"
                , ValidationLevel.Error
                , x => x.UpperStemDiameter < x.DBH || TreeIsNotMeassure(x)));

            Rules.Add(new PredicateRule<Tree_Ex>(nameof(Tree_Ex.TopDIBSecondary), "Top DIB Secondary must be less Top DIB Primary"
                , ValidationLevel.Error
                , x => x.TopDIBSecondary <= x.TopDIBPrimary || TreeIsNotMeassure(x)));//TODO this validation doesn't match with the error message but this is how it was before

            Rules.Add(new PredicateRule<Tree_Ex>(nameof(Tree_Ex.SeenDefectPrimary), "Seen Defect Primary must be greater than Recoverable Primary"
                , ValidationLevel.Error
                , x => x.SeenDefectPrimary >= x.RecoverablePrimary || TreeIsNotMeassure(x)));//TODO this validation doesn't match with the error message but this is how it was before

            Rules.Add(new PredicateRule<Tree_Ex>("Heights", "HT, MerchHtP, MerchHtS, or UStemHT must be greater than 0"
                , ValidationLevel.Error
                , x => TreeIsNotMeassure(x) || x.TotalHeight > 0 || x.MerchHeightPrimary > 0 || x.MerchHeightSecondary > 0 || x.UpperStemHeight > 0));

            Rules.Add(new PredicateRule<Tree_Ex>("Diameters", "DBH or DRC must be greater than 0"
                , ValidationLevel.Error
                , x => TreeIsNotMeassure(x) || x.DBH > 0 || x.DRC > 0));
        }

        private static bool TreeIsNotMeassure(Tree_Ex tree)
        {
            var countMeasure = tree.CountOrMeasure;
            return countMeasure != "M";
        }
    }
}