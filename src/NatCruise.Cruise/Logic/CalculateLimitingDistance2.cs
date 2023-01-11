using NatCruise.Util;
using System;

namespace NatCruise.Cruise.Logic
{
    public class CalculateLimitingDistance2 : ILimitingDistanceCalculator
    {
        public const int DECIMALS = 2;

        //public enum MeasureToType
        //{ Face, Center };

        //public enum PlotType
        //{ Fixed, Variable };

        //public enum LimitingDistanceResultType
        //{ Unknown = 0, In, Out };

        public const string TREE_STATUS_IN = "IN";
        public const string TREE_STATUS_OUT = "OUT";

        public decimal Calculate(decimal bafORfps, decimal dbh,
            int slopPct, bool isVariableRadius, bool isToFace)
        {
            if (dbh <= 0.0m
                || bafORfps <= 0) { return 0.0m; }

            if (isVariableRadius)
            {
                return CalculateVariableRadious(bafORfps, dbh, slopPct, isToFace);
            }
            else
            {
                return CalculateFixSize(bafORfps, dbh, slopPct, isToFace);
            }
        }

        public static decimal CalculateVariableRadious(decimal baf, decimal dbh, int slopePct, bool isToFace)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch
            if (dbh <= 0.0m
                || baf <= 0) { return 0.0m; }

            // Reference: FSH 2409.12 35.22a
            decimal plotRadiusFactor = CalculatePlotRadiusFactor(baf);
            decimal limitingDistance = Math.Round(dbh * plotRadiusFactor, 2);

            if (isToFace)
            {
                decimal toFaceCorrection = CalculateToFaceCorrection(dbh);
                limitingDistance = limitingDistance - toFaceCorrection;
            }

            decimal slopeCorrectionFactor = CalculateToSlopeCorrectionFactor(slopePct);
            limitingDistance = Math.Round(limitingDistance * slopeCorrectionFactor, 2, MidpointRounding.AwayFromZero);

            return limitingDistance;
        }

        public static decimal CalculatePlotRadiusFactor(decimal baf)
        {
            // round PRF to 3 digits.
            // Could not find any explicit rule on this but all examples and look up tables use 3 decimals
            return Math.Round((8.696m / DecimalMath.Sqrt(baf)), 3, MidpointRounding.AwayFromZero);
        }

        public static decimal CalculateToFaceCorrection(decimal dbh)
        {
            // to calculate to face correction: Half DBH in feet (divided by 12)
            return Math.Round(dbh / (12.0m * 2), 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalculateToSlopeCorrectionFactor(int slopePct)
        {
            if (slopePct < 10) { return 1.0m; } // slope correction factor should only be calculated for slopes 10% or more
            if (slopePct == 10) { return 1.01m; } // look up tables in the Forest Service Handbook list the SCF for a slope as 10 as 1.01
                                                  // this may be due to a rounding error, but to stay consistent with the handbook we return 1.01
                                                  
            decimal slope = slopePct / 100.0m; // convert slope percentage to decimal value
            // its not explicitly stated that slope correction factor should be rounded two decimals
            // but examples in the handbook (pg. 39) show two decimals.
            // also it is explicitly stated that slope correction factor should only be calculated for slopes 10% or more
            // which supports the idea of only using two decimals
            var slopeSqr = slope * slope;
            var scf = DecimalMath.Sqrt(1.0m + slopeSqr);
            return Math.Round(scf, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalculateFixSize(decimal fps, decimal dbh, int slopePct, bool isToFace)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch
            if (dbh <= 0.0m
                || fps <= 0) { return 0.0m; }

            // Reference: FSH 2409.12 34.22
            // Sect 34.22 shows plot radius in tenths
            decimal plotRad = Math.Round(DecimalMath.Sqrt((43560 / fps) / DecimalMath.Pi), 1, MidpointRounding.AwayFromZero);

            if (isToFace)
            {
                decimal toFaceCorrection = CalculateToFaceCorrection(dbh);
                plotRad = plotRad - toFaceCorrection;
            }

            decimal slopeCorrectionFactor = CalculateToSlopeCorrectionFactor(slopePct);
            plotRad = plotRad * slopeCorrectionFactor;

            return plotRad;
        }

        public bool DeterminTreeInOrOut(decimal slopeDistance, decimal limitingDistance)
        {
            return Math.Round(slopeDistance, DECIMALS, MidpointRounding.AwayFromZero) <= Math.Round(limitingDistance, DECIMALS, MidpointRounding.AwayFromZero);
        }

        public string GenerateReport(string treeStatus, decimal limitingDistance, decimal slopeDistance, int slopePCT, decimal azimuth,
            decimal bafORfps, decimal dbh, bool isVariableRadius, bool isToFace, string stratumCode)
        {
            dbh = Math.Round(dbh, 1, MidpointRounding.AwayFromZero); // dbh should be rounded to tenth of an inch

            var azimuthStr = (azimuth > 0) ? "Azimuth:" + azimuth.ToString() : "";
            var isToFaceStr = (isToFace) ? "Face" : "Center";
            var toFaceCorrection = (isToFace) ? $", TFC:{CalculateToFaceCorrection(dbh)}" : "";
            var isVariableRadiusStr = (isVariableRadius) ? "BAF" : "FPS";
            var slope = (slopePCT != 0 && slopePCT < 10) ? $"slope: {slopePCT}(<10%)"
                : $"slope: {slopePCT}%, SCF: {CalculateToSlopeCorrectionFactor(slopePCT)}";
            var prf = (isVariableRadius) ? $"PRF:{CalculatePlotRadiusFactor(bafORfps)}, " : "";

            return $"Tree was {treeStatus} (St:{stratumCode}, DBH:{dbh:F1}, {slope}, slope distance:{slopeDistance:F2}', limiting distance:{limitingDistance}' to {isToFaceStr} of tree {toFaceCorrection}, {prf}{isVariableRadiusStr}:{bafORfps}) {azimuthStr}\r\n";
        }
    }
}