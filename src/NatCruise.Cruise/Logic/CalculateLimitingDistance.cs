using NatCruise.Util;
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

        public static decimal Calculate2(decimal bafORfps, decimal dbh,
            int slopPct, bool isVariableRadius, bool isToFace)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch
            if (dbh <= 0.0m
                || bafORfps <= 0) { return 0.0m; }

            decimal toFaceCorrection = (isToFace) ?
                (dbh / (12.0m * 2)) // to calculate to face correction: Half DBH in feet (divided by 12) 
                : 0.0m;

            decimal slope = slopPct / 100.0m; // convert slope percentage to decimal value

            if (isVariableRadius)
            {
                // Reference: FSH 2409.12 35.22a
                decimal plotRadiusFactor = (8.696m / DecimalMath.Sqrt(bafORfps));
                decimal limitingDistance = dbh * plotRadiusFactor;
                decimal correctedPRF = (limitingDistance - toFaceCorrection) / dbh;

                

                decimal slopeCorrectionFactor = 0m;
                if(slopPct >= 10)
                {
                    slopeCorrectionFactor = DecimalMath.Sqrt(1.0m + (slope * slope));
                }
                decimal correctedLimitingDistance = dbh * correctedPRF * slopeCorrectionFactor;
                return correctedLimitingDistance;
            }
            else
            {
                // Reference: FSH 2409.12 34.22
                decimal plotRad = DecimalMath.Sqrt((43560 / bafORfps) / DecimalMath.Pi);
                decimal slopeCorrectionFactor = 1 / DecimalMath.Cos(DecimalMath.ATan(slope));
                decimal limitingDistance = (plotRad - toFaceCorrection) * slopeCorrectionFactor;
                return limitingDistance;
            }
        }

        public static decimal CalculateVariableRadious(decimal baf, decimal dbh, int slopePct, bool isToFace)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch
            if (dbh <= 0.0m
                || baf <= 0) { return 0.0m; }


            // Reference: FSH 2409.12 35.22a
            decimal plotRadiusFactor = (8.696m / DecimalMath.Sqrt(baf));
            decimal limitingDistance = dbh * plotRadiusFactor;

            if(isToFace)
            {
                decimal toFaceCorrection = CalculateToFaceCorrection(dbh);
                limitingDistance = limitingDistance - toFaceCorrection;
            }

            if (slopePct >= 10) // slope correction factor should only be calculated for slopes 10% or more
            {
                decimal slope = slopePct / 100.0m; // convert slope percentage to decimal value
                decimal slopeCorrectionFactor = DecimalMath.Sqrt(1.0m + (slope * slope)); // round to two decimals?

                limitingDistance = limitingDistance * slopeCorrectionFactor;
            }

            return limitingDistance;
        }

        public static decimal CalculateToFaceCorrection(decimal dbh)
        {
            return (dbh / (12.0m * 2)); // to calculate to face correction: Half DBH in feet (divided by 12) 
        }

        public static decimal CalculateToSlopeCorrectionFactor(int slopePct)
        {
            decimal slope = slopePct / 100.0m; // convert slope percentage to decimal value
            return DecimalMath.Sqrt(1.0m + (slope * slope)); // round to two decimals?
        }

        public static decimal CalculateFixSize(decimal fps, decimal dbh, int slopePct, bool isToFace)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch
            if (dbh <= 0.0m
                || baf <= 0) { return 0.0m; }
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