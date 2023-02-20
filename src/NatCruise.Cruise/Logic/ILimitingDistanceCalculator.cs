using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Cruise.Logic
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
