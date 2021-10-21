using CruiseDAL.Schema;

namespace NatCruise
{
    public static class CruiseMethodTypeExtentions
    {
        public static CruiseMethodType StringToCruiseMethodType(string method)
        {
            return method switch
            {
                CruiseMethods.H_PCT => CruiseMethodType.H_PCT,
                CruiseMethods.STR => CruiseMethodType.STR,
                CruiseMethods.S3P => CruiseMethodType.S3P,
                CruiseMethods.THREEP => CruiseMethodType.ThreeP,
                CruiseMethods.FIX => CruiseMethodType.FIX,
                CruiseMethods.F3P => CruiseMethodType.F3P,
                CruiseMethods.FCM => CruiseMethodType.FCM,
                CruiseMethods.PCM => CruiseMethodType.PCM,
                CruiseMethods.PNT => CruiseMethodType.PNT,
                CruiseMethods.P3P => CruiseMethodType.P3P,
                CruiseMethods.THREEPPNT => CruiseMethodType.ThreePPNT,
                CruiseMethods.FIXCNT => CruiseMethodType.FIXCNT,
                _ => CruiseMethodType.None,
            };
        }

    }
}