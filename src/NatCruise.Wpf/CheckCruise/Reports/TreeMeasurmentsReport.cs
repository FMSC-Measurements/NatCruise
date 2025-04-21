using CruiseDAL.DataObjects;
using NatCruise.Wpf.CheckCruise.Data;
using NatCruise.Wpf.CheckCruise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.CheckCruise.Reports
{
    public class TreeMeasurmentsReportBuilder
    {
        public static void Generate(dataBaseCommands cruiseDataservice, IEnumerable<UnitStratum> cc_Units, TextWriter writer)
        {
            string[] textHeader = new string[17] { "ST", "CU", "PL", "SG", "TN", "SP", "PP", "LD", "DBH", "TH", "MHP", "MHS", "HFLL", "SD", "LN", "GR", " LSD" };

            var tList = cruiseDataservice.getTrees();
            var logList = cruiseDataservice.getLogs();

            WriteHeader(writer, textHeader);

            //  loop through units selected list to retrieve proper tree data
            foreach (var ccu in cc_Units)
            {
                StringBuilder sb = new StringBuilder();

                List<TreeDO> justUnitData = tList.FindAll(
                    delegate (TreeDO t)
                    {
                        return t.Stratum.Code == ccu.StratumCode && t.CuttingUnit.Code == ccu.CuttingUnitCode;
                    });
                foreach (TreeDO jud in justUnitData)
                {
                    //  build print line
                    sb.Append(jud.Stratum.Code);
                    sb.Append(",");
                    sb.Append(jud.CuttingUnit.Code);
                    sb.Append(",");
                    if (jud.Plot == null)
                        sb.Append("    ");
                    else sb.Append(jud.Plot.PlotNumber);
                    sb.Append(",");
                    sb.Append(jud.SampleGroup.Code);
                    sb.Append(",");
                    sb.Append(jud.TreeNumber);
                    sb.Append(",");
                    sb.Append(jud.Species);
                    sb.Append(",");
                    sb.Append(jud.SampleGroup.PrimaryProduct);
                    sb.Append(",");
                    sb.Append(jud.LiveDead);
                    sb.Append(",");
                    sb.Append(jud.DBH.ToString());
                    sb.Append(",");
                    sb.Append(jud.TotalHeight.ToString());
                    sb.Append(",");
                    sb.Append(jud.MerchHeightPrimary.ToString());
                    sb.Append(",");
                    sb.Append(jud.MerchHeightSecondary.ToString());
                    sb.Append(",");
                    sb.Append(jud.HeightToFirstLiveLimb.ToString());
                    sb.Append(",");
                    sb.Append(jud.SeenDefectPrimary.ToString());
                    //  find any logs associated with current tree
                    var justLogs = logList.Where(ld => jud.Tree_CN == ld.Tree_CN);
                    if (justLogs.Any())
                    {
                        sb.Append(",");
                        foreach (LogDO jl in justLogs)
                        {
                            writer.Write(sb.ToString());
                            writer.Write(jl.LogNumber);
                            writer.Write(",");
                            writer.Write(jl.Grade);
                            writer.Write(",");
                            writer.WriteLine(jl.SeenDefect.ToString());
                        }   //  end foreach loop
                    }
                    else
                    {
                        //  write just tree record
                        writer.WriteLine(sb.ToString());
                    }   //  endif
                }   //  end foreach loop of just trees in unit
            }   //  end foreach loop of selected units
        }

        private static void WriteHeader(TextWriter writer, string[] textHeader)
        {
            StringBuilder sb = new StringBuilder();
            //  write header line to file
            for (int k = 0; k < 16; k++)
            {
                sb.Append(textHeader[k] + ",");
            }   //  end for k loop
            sb.Append(textHeader[16]);
            writer.WriteLine(sb);
        }
    }
}
