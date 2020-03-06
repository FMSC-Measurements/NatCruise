using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class CalculateLimitingDistance
    {
        public static int DECIMALS = 2;


        public enum MeasureToType { Face, Center };
        public enum PlotType { Fixed, Variable };
        public enum LimitingDistanceResultType { Unknown = 0, In, Out};

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
            double bafORfps, double dbh, int slopPct, bool isVariableRadius, bool isToFace, string stratumCode)
        {
            var azimuthStr = (azimuth > 0) ? "Azimuth:" + azimuth.ToString() : String.Empty;

            return String.Format("Tree was {0} (St:{9}, DBH:{1}, slope:{2}%, slope distance:{3:F2}', limiting distance:{4:F2}' to {5} of tree, {6}:{7}) {8}\r\n",
                    treeStatus,
                    dbh,
                    slopePCT,
                    slopeDistance,
                    limitingDistance,
                    (isToFace) ? "Face" : "Center",
                    (isVariableRadius) ? "BAF" : "FPS",
                    bafORfps,
                    azimuthStr,
                    stratumCode);
        }
    }
}
