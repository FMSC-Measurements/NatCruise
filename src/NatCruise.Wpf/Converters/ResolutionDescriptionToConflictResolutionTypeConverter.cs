using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CruiseDAL.V3.Sync;

namespace NatCruise.Wpf.Converters
{
    public class ResolutionDescriptionToConflictResolutionTypeConverter : IValueConverter
    {
        public const string CHOSEDEST = "Keep Output File Record";
        public const string CHOSESOURCE = "Keep Crew File Record";
        public const string CHOSELATEST = "Chose Latest";
        public const string CHOSEDESTMERGEDATA = "Keep Output File Record And Merge Downstream Records";
        public const string CHOSESOURCEMERGEDATA = "Keep Crew File Record And Merge Downstream Records";
        public const string MODIFYDEST = "Modify Output File Record";
        public const string MODIFYSOURCE = "Modify Crew File Record";

        public static readonly string[] RESOLUTION_OPTIONS = new string[] {
            CHOSEDEST,
            CHOSESOURCE,
            //CHOSELATEST,
            CHOSEDESTMERGEDATA,
            CHOSESOURCEMERGEDATA,
            MODIFYDEST,
            MODIFYSOURCE,
        };

        public static readonly string[] RESOLUTION_OPTIONS_NO_MERGE = new string[] {
            CHOSEDEST,
            CHOSESOURCE,
            //CHOSELATEST,
            MODIFYDEST,
            MODIFYSOURCE,
        };

        public static readonly string[] RESOLUTION_OPTIONS_NO_CHOSE = new string[] {
            CHOSEDESTMERGEDATA,
            CHOSESOURCEMERGEDATA,
            MODIFYDEST,
            MODIFYSOURCE,
        };

        public static readonly string[] RESOLUTION_OPTIONS_CHOSE = new string[] {
            CHOSEDEST,
            CHOSESOURCE,
            //CHOSELATEST,
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConflictResolutionType eValue)
            {
                switch (eValue)
                {
                    case ConflictResolutionType.ChoseDest: { return CHOSEDEST; }
                    case ConflictResolutionType.ChoseSource: { return CHOSESOURCE; }
                    case ConflictResolutionType.ChoseLatest: { return CHOSELATEST; }
                    case ConflictResolutionType.ChoseDestMergeData: { return CHOSEDESTMERGEDATA; }
                    case ConflictResolutionType.ChoseSourceMergeData: { return CHOSESOURCEMERGEDATA; }
                    case ConflictResolutionType.ModifyDest: { return MODIFYDEST; }
                    case ConflictResolutionType.ModifySource: { return MODIFYSOURCE; }
                    default: { return null; }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
            {
                if (sValue.Equals(CHOSEDEST, StringComparison.OrdinalIgnoreCase))
                { return ConflictResolutionType.ChoseDest; }

                if (sValue.Equals(CHOSESOURCE, StringComparison.OrdinalIgnoreCase))
                { return ConflictResolutionType.ChoseSource; }

                if (sValue.Equals(CHOSELATEST, StringComparison.OrdinalIgnoreCase))
                { return ConflictResolutionType.ChoseLatest; }

                if (sValue.Equals(CHOSEDESTMERGEDATA))
                { return ConflictResolutionType.ChoseDestMergeData; }

                if (sValue.Equals(CHOSESOURCEMERGEDATA))
                { return ConflictResolutionType.ChoseDestMergeData; }

                if (sValue.Equals(MODIFYDEST, StringComparison.OrdinalIgnoreCase))
                { return ConflictResolutionType.ModifyDest; }

                if (sValue.Equals(MODIFYSOURCE, StringComparison.OrdinalIgnoreCase))
                { return ConflictResolutionType.ModifySource; }
            }
            return ConflictResolutionType.NotSet;
        }
    }
}
