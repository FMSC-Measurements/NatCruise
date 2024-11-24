using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CheckCruise
{
    public static class Utilities
    {
        public static StringBuilder FormatField(float fieldToFormat, string formatOfField)
        {
            StringBuilder SB = new StringBuilder();
            SB.Remove(0, SB.Length);
            SB.AppendFormat(formatOfField, fieldToFormat);
            return SB;
        }

        public static StringBuilder FormatField(double fieldToFormat,string formatOfField)
        {
            StringBuilder SB = new StringBuilder();
            SB.Remove(0,SB.Length);
            SB.AppendFormat(formatOfField, fieldToFormat);
            return SB;
        }

        public static StringBuilder FormatField(long fieldToFormat, string formatOfField)
        {
            StringBuilder SB = new StringBuilder();
            SB.Remove(0, SB.Length);
            SB.AppendFormat(formatOfField, fieldToFormat);
            return SB;
        }

        public static StringBuilder FormatField(int fieldToFormat, string formatOfField)
        {
            StringBuilder SB = new StringBuilder();
            SB.Remove(0, SB.Length);
            SB.AppendFormat(formatOfField, fieldToFormat);
            return SB;
        }

        public static  int IsFileOpen(string outputFileName)
        {
            try
            {
                FileStream fs = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                fs.Close();
            }
            catch
            {
                return 1;
            }   //  end try catch

            return 0;
        }   //  end IsFileOpen

    }   //  end class Utilities
}
