using System;

namespace NatCruise.Cruise.Logic
{
    public static class CalculateLimitingDistance
    {
        public const int DECIMALS = 2;

        public enum MeasureToType { Face, Center };

        public enum PlotType { Fixed, Variable };

        public enum LimitingDistanceResultType { Unknown = 0, In, Out };

        public const string TREE_STATUS_IN = "IN";
        public const string TREE_STATUS_OUT = "OUT";

        public static double Calculate(double bafORfps, double dbh,
            int slopPct, bool isVariableRadius, bool isToFace)
        {
            if (dbh <= 0.0
                || bafORfps <= 0) { return 0.0; }

            double toFaceCorrection = (isToFace) ?
                (dbh / 12.0) * 0.5
                : 0.0;

            double slope = slopPct / 100.0d;

            if (isVariableRadius)
            {
                // Reference: FSH 2409.12 35.22a
                double plotRadiusFactor = (8.696 / Math.Sqrt(bafORfps));
                double limitingDistance = dbh * plotRadiusFactor;
                double correctedPRF = (limitingDistance - toFaceCorrection) / dbh;

                double slopeCorrectionFactor = Math.Sqrt(1.0d + (slope * slope));
                double correctedLimitingDistance = dbh * correctedPRF * slopeCorrectionFactor;
                return correctedLimitingDistance;
            }
            else
            {
                // Reference: FSH 2409.12 34.22
                double plotRad = Math.Sqrt((43560 / bafORfps) / Math.PI);
                double slopeCorrectionFactor = 1 / Math.Cos(Math.Atan(slope));
                double limitingDistance = (plotRad - toFaceCorrection) * slopeCorrectionFactor;
                return limitingDistance;
            }
        }

        public static bool DeterminTreeInOrOut(double slopeDistance, double limitingDistance)
        {
            return Math.Round(slopeDistance, DECIMALS) <= Math.Round(limitingDistance, DECIMALS);
        }

        public static string GenerateReport(string treeStatus, double limitingDistance, double slopeDistance, double slopePCT, double azimuth,
            double bafORfps, double dbh, bool isVariableRadius, bool isToFace, string stratumCode)
        {
            var azimuthStr = (azimuth > 0) ? "Azimuth:" + azimuth.ToString() : String.Empty;
            var isToFaceStr = (isToFace) ? "Face" : "Center";
            var isVariableRadiusStr = (isVariableRadius) ? "BAF" : "FPS";

            return $"Tree was {treeStatus} (St:{stratumCode}, DBH:{dbh}, slope:{slopePCT}%, slope distance:{slopeDistance:F2}', limiting distance:{limitingDistance:F2}' to {isToFaceStr} of tree, {isVariableRadiusStr}:{bafORfps}) {azimuthStr}\r\n";
        }
    }
}