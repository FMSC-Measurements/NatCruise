namespace NatCruise.Logic
{
    public interface ILimitingDistanceCalculator
    {
        decimal Calculate(decimal baf, decimal fps, decimal dbh,
            int slopPct, bool isVariableRadius, bool isToFace);

        bool DeterminTreeInOrOut(decimal slopeDistance, decimal limitingDistance);

        string GenerateReport(string treeStatus, decimal limitingDistance, decimal slopeDistance, int slopePCT, decimal azimuth,
            decimal baf, decimal fps, decimal dbh, bool isVariableRadius, bool isToFace, string stratumCode, string treeNumber = "");
    }
}