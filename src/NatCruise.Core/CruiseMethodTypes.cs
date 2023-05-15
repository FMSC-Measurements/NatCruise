using System;

namespace NatCruise
{
    [Flags]
    public enum CruiseMethodType
    {
        None = 0,
        H_PCT = 1, STR = 2, S3P = 4, ThreeP = 8, FIX = 16, F3P = 32, FCM = 64, PCM = 128, PNT = 256, P3P = 512, ThreePPNT = 1024, FIXCNT = 2048,
        PlotMethods = FIX | F3P | FCM | PCM | P3P | PNT | ThreePPNT | FIXCNT,
        ThreePMethods = ThreeP | F3P | S3P | P3P | ThreePPNT,
        FrequencySampleMethods = STR | FCM | PCM | S3P,
        VariableRadiousPlotMethods = PCM | PNT | P3P,
        FixedSizePlotMethods = FIX | F3P | FCM,
    }
}