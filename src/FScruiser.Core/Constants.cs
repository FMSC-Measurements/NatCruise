using FScruiser.Models;

namespace FScruiser
{
    public class Constants
    {
        public enum TallyLedger_EntryType { unknown = 0, Utility, TreeCountEdit, Tally };

        public static readonly TreeFieldSetup[] DEFAULT_TREE_FIELDS = new TreeFieldSetup[]{
            new TreeFieldSetup(){
                Field = nameof(Tree_Ex.TreeNumber), Heading = "Tree", FieldOrder = 1 },
            new TreeFieldSetup() {
                Field = "Stratum", Heading = "St", FieldOrder = 2 },
            new TreeFieldSetup() {
                Field = "SampleGroup", Heading = "SG", FieldOrder = 3 },
            new TreeFieldSetup() {
                Field = nameof(Tree_Ex.Species), Heading = "Sp", FieldOrder = 4 },
            new TreeFieldSetup() {
                Field = nameof(Tree_Ex.DBH), Heading = "DBH", FieldOrder = 5 },
            new TreeFieldSetup() {
                Field = nameof(Tree_Ex.TotalHeight), Heading = "THT", FieldOrder = 6 },
            new TreeFieldSetup() {
                Field = nameof(Tree_Ex.SeenDefectPrimary), Heading = "Def", FieldOrder = 7 }
        };

        public static readonly string[] HEIGHT_FIELDS = new string[]
        {
            nameof(Tree_Ex.TotalHeight),
            nameof(Tree_Ex.HeightToFirstLiveLimb),
            nameof(Tree_Ex.MerchHeightPrimary),
            nameof(Tree_Ex.MerchHeightSecondary),
            nameof(Tree_Ex.UpperStemHeight)
        };

        public static readonly string[] DIAMATER_FIELDS = new string[]
        {
            nameof(Tree_Ex.DBH),
            nameof(Tree_Ex.DBHDoubleBarkThickness),
            nameof(Tree_Ex.UpperStemDiameter)
        };

        public static readonly string[] LESS_IMPORTANT_TREE_FIELDS = new string[]
        {
            nameof(Tree.TreeNumber),
            "Stratum",
            "SampleGroup",
            nameof(Tree.CountOrMeasure),
            "TreeCount",
            "KPI",
            "STM",
            nameof(Tree_Ex.Initials),
            nameof(Tree_Ex.LiveDead),
            nameof(Tree_Ex.Grade),
            nameof(Tree_Ex.HiddenPrimary)
        };

        public static readonly LogFieldSetup[] DEFAULT_LOG_FIELDS = new LogFieldSetup[]{
            new LogFieldSetup(){
                Field = nameof(Log.LogNumber), Heading = "LogNum"},
            new LogFieldSetup(){
                Field = nameof(Log.Grade), Heading = "Grade"},
            new LogFieldSetup() {
                Field = nameof(Log.SeenDefect), Heading = "PctSeenDef"}
        };
    }
}