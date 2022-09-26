using NatCruise.Util;
using System;

namespace NatCruise.Cruise.Logic
{
    public class CalculateLimitingDistance : ILimitingDistanceCalculator
    {
        public const int DECIMALS = 2;

        public const string TREE_STATUS_IN = "IN";
        public const string TREE_STATUS_OUT = "OUT";

        public decimal Calculate(decimal bafORfps, decimal dbh,
            int slopPct, bool isVariableRadius, bool isToFace)
        {
            if (dbh <= 0.0m
                || bafORfps <= 0) { return 0.0m; }

            decimal toFaceCorrection = (isToFace) ?
                (dbh / 12.0m) * 0.5m
                : 0.0m;

            decimal slope = slopPct / 100.0m;

            if (isVariableRadius)
            {
                // Reference: FSH 2409.12 35.22a
                decimal plotRadiusFactor = (8.696m / DecimalMath.Sqrt(bafORfps));
                decimal limitingDistance = dbh * plotRadiusFactor;
                decimal correctedPRF = (limitingDistance - toFaceCorrection) / dbh;

                decimal slopeCorrectionFactor = DecimalMath.Sqrt(1.0m + (slope * slope));
                decimal correctedLimitingDistance = dbh * correctedPRF * slopeCorrectionFactor;
                return correctedLimitingDistance;
            }
            else
            {
                // Reference: FSH 2409.12 34.22
                decimal plotRad = DecimalMath.Sqrt((43560m / bafORfps) / new Decimal(Math.PI));
                decimal slopeCorrectionFactor = DecimalMath.Sqrt(1.0m + (slope * slope));
                decimal limitingDistance = (plotRad - toFaceCorrection) * slopeCorrectionFactor;
                return limitingDistance;
            }
        }

        public bool DeterminTreeInOrOut(decimal slopeDistance, decimal limitingDistance)
        {
            return Math.Round(slopeDistance, DECIMALS, MidpointRounding.AwayFromZero) <= Math.Round(limitingDistance, DECIMALS, MidpointRounding.AwayFromZero);
        }

        public string GenerateReport(string treeStatus, decimal limitingDistance, decimal slopeDistance, int slopePCT, decimal azimuth,
            decimal bafORfps, decimal dbh, bool isVariableRadius, bool isToFace, string stratumCode)
        {
            var azimuthStr = (azimuth > 0) ? "Azimuth:" + azimuth.ToString() : String.Empty;
            var isToFaceStr = (isToFace) ? "Face" : "Center";
            var isVariableRadiusStr = (isVariableRadius) ? "BAF" : "FPS";

            return $"Tree was {treeStatus} (St:{stratumCode}, DBH:{dbh}, slope:{slopePCT}%, slope distance:{slopeDistance:F2}', limiting distance:{limitingDistance:F2}' to {isToFaceStr} of tree, {isVariableRadiusStr}:{bafORfps}) {azimuthStr}\r\n";
        }
    }
}