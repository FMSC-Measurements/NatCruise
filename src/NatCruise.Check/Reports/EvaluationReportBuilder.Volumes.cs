using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckCruise.Reports
{
    public partial class EvaluationReportBuilder
    {
        public int DisplayVolume(TextWriter writer, TolerancesList currTolerance, string cruiserInitials)
        {
            EvaluationReportRTF eRTF = new EvaluationReportRTF();
            int includeFlag = 0;
            int compareResult = 0;

            //  displays either volume with no comparison or comparison volume
            includeFlag = CheckResults.Sum(c => c.R_IncludeVol);

            //  get results data based on group selected
            List<ResultsList> groupedTrees = new List<ResultsList>();
            int byWhich = 0;
            if (currTolerance.T_BySpecies == 1)
            {
                groupedTrees = CheckCruiseDataService.getResultsTable("R_TreeSpecies", "");
                byWhich = 1;
            }
            else if (currTolerance.T_ByProduct == 1)
            {
                groupedTrees = CheckCruiseDataService.getResultsTable("R_TreeProduct", "");
                byWhich = 2;
            }   //  endif

            //  complete and print header for volume portion
            int tableToUse = CompleteAndPrintHeader(currTolerance, writer, eRTF, includeFlag);

            //  capture volumeLine for future changes
            string[] lineHold = new string[13];
            for (int k = 0; k < 13; k++)
                lineHold[k] = eRTF.VolumeLine[k + 27];
            List<ResultsList> currentSpeciesVolume = new List<ResultsList>();
            foreach (ResultsList gt in groupedTrees)
            {
                if (gt.R_TreeSpecies != "")
                {
                    switch (byWhich)
                    {
                        case 1:
                            eRTF.VolumeLine[27] = eRTF.VolumeLine[27].Replace("xxxxxx", gt.R_TreeSpecies).PadRight(6, ' ');
                            currentSpeciesVolume = CheckResults.FindAll(
                                delegate (ResultsList r)
                                {
                                    return r.R_TreeSpecies == gt.R_TreeSpecies && r.R_CountMeasure == "M";
                                });
                            break;
                        case 2:
                            eRTF.VolumeLine[27] = eRTF.VolumeLine[27].Replace("xxxxxx", gt.R_TreeProduct).PadRight(6, ' ');
                            currentSpeciesVolume = CheckResults.FindAll(
                                delegate (ResultsList r)
                                {
                                    return r.R_TreeProduct == gt.R_TreeProduct && r.R_CountMeasure == "M";
                                });
                            break;
                    }   //  endif no species code
                }   //  end switch 

                if (currentSpeciesVolume.Count > 0)
                {
                    //  fields to sum volumes
                    double CUFTgross = 0;
                    double CUFTnet = 0;
                    double BDFTgross = 0;
                    double BDFTnet = 0;
                    double CUFTgrossCC = 0;
                    double CUFTnetCC = 0;
                    double BDFTgrossCC = 0;
                    double BDFTnetCC = 0;
                    switch (tableToUse)
                    {
                        case 1:
                            CUFTgross = currentSpeciesVolume.Sum(c => c.R_GrossCUFTPP);
                            CUFTnet = currentSpeciesVolume.Sum(c => c.R_NetCUFTPP);
                            CUFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossCUFTPP);
                            foreach (ResultsList ccv in currentSpeciesVolume)
                            {
                                double dtemp = ccv.R_CC_NetCUFTPP;
                            }
                            CUFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetCUFTPP);
                            BDFTgross = currentSpeciesVolume.Sum(c => c.R_GrossBDFTPP);
                            BDFTnet = currentSpeciesVolume.Sum(c => c.R_NetBDFTPP);
                            BDFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossBDFTPP);
                            BDFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetBDFTPP);
                            break;
                        case 2:
                            BDFTgross = currentSpeciesVolume.Sum(c => c.R_GrossBDFTSP);
                            BDFTnet = currentSpeciesVolume.Sum(c => c.R_NetBDFTSP);
                            BDFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossBDFTSP);
                            BDFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetBDFTSP);
                            CUFTgross = currentSpeciesVolume.Sum(c => c.R_GrossCUFTSP);
                            CUFTnet = currentSpeciesVolume.Sum(c => c.R_NetCUFTSP);
                            CUFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossCUFTSP);
                            CUFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetCUFTSP);
                            break;
                        case 3:
                            //  Since primary and secondary are added together, these variables are slightly
                            //  different for the report.  
                            CUFTgross = currentSpeciesVolume.Sum(c => c.R_GrossCUFTPSP);
                            CUFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossCUFTPSP);

                            CUFTnet = currentSpeciesVolume.Sum(c => c.R_NetCUFTPSP);
                            CUFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetCUFTPSP);

                            BDFTgross = currentSpeciesVolume.Sum(c => c.R_GrossBDFTPSP);
                            BDFTgrossCC = currentSpeciesVolume.Sum(cc => cc.R_CC_GrossBDFTPSP);

                            BDFTnet = currentSpeciesVolume.Sum(c => c.R_NetBDFTPSP);
                            BDFTnetCC = currentSpeciesVolume.Sum(cc => cc.R_CC_NetBDFTPSP);
                            break;
                    }   //  end switch

                    //  put volume in the volume line
                    eRTF.VolumeLine[28] = eRTF.VolumeLine[28].Replace("xxxxxx", Utilities.FormatField(CUFTgross, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[29] = eRTF.VolumeLine[29].Replace("xxxxxx", Utilities.FormatField(CUFTgrossCC, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[31] = eRTF.VolumeLine[31].Replace("xxxxxx", Utilities.FormatField(CUFTnet, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[32] = eRTF.VolumeLine[32].Replace("xxxxxx", Utilities.FormatField(CUFTnetCC, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[34] = eRTF.VolumeLine[34].Replace("xxxxxx", Utilities.FormatField(BDFTgross, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[35] = eRTF.VolumeLine[35].Replace("xxxxxx", Utilities.FormatField(BDFTgrossCC, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[37] = eRTF.VolumeLine[37].Replace("xxxxxx", Utilities.FormatField(BDFTnet, "{0,4:F0}").ToString());
                    eRTF.VolumeLine[38] = eRTF.VolumeLine[38].Replace("xxxxxx", Utilities.FormatField(BDFTnetCC, "{0,4:F0}").ToString());

                    //  is comparison needed
                    double PC_CUFTgross = 0;
                    double PC_CUFTnet = 0;
                    double PC_BDFTgross = 0;
                    double PC_BDFTnet = 0;
                    if (includeFlag > 0)
                    {
                        if (CUFTgross > 0)
                            PC_CUFTgross = ((CUFTgross - CUFTgrossCC) / CUFTgross) * 100;
                        else PC_CUFTgross = 0;
                        if (CUFTgross == 0.0 && CUFTgrossCC == 0.0 && PC_CUFTgross == 100.0)
                            eRTF.VolumeLine[30] = eRTF.VolumeLine[30].Replace("xx", "na");
                        else if (currTolerance.T_CUFTPGtolerance == 0.0 && currTolerance.T_CUFTSGtolerance == 0.0 &&
                                    currTolerance.T_CUFTPSGtolerance == 0.0)
                            eRTF.VolumeLine[30] = eRTF.VolumeLine[30].Replace("xx", "na");
                        else eRTF.VolumeLine[30] = eRTF.VolumeLine[30].Replace("xx", Utilities.FormatField(Math.Abs(PC_CUFTgross), "{0,2:F0}").ToString());
                        //  is lercent difference greater than tolerance?
                        if (currTolerance.T_CUFTPGtolerance != 0.0 && Math.Abs(PC_CUFTgross) > currTolerance.T_CUFTPGtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_CUFTSGtolerance != 0.0 && Math.Abs(PC_CUFTgross) > currTolerance.T_CUFTSGtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_CUFTPSGtolerance != 0.0 && Math.Abs(PC_CUFTgross) > currTolerance.T_CUFTPSGtolerance)
                            compareResult = 1;

                        if (CUFTnet > 0)
                            PC_CUFTnet = ((CUFTnet - CUFTnetCC) / CUFTnet) * 100;
                        else PC_CUFTnet = 0;
                        if (CUFTnet == 0.0 && CUFTnetCC == 0.0 && PC_CUFTnet == 100.0)
                            eRTF.VolumeLine[33] = eRTF.VolumeLine[33].Replace("xx", "na");
                        else if (currTolerance.T_CUFTPNtolerance == 0.0 && currTolerance.T_CUFTSNtolerance == 0.0 &&
                                    currTolerance.T_CUFTPSNtolerance == 0.0)
                            eRTF.VolumeLine[33] = eRTF.VolumeLine[33].Replace("xx", "na");
                        else eRTF.VolumeLine[33] = eRTF.VolumeLine[33].Replace("xx", Utilities.FormatField(Math.Abs(PC_CUFTnet), "{0,2:F0}").ToString());
                        //  is lercent difference greater than tolerance?
                        if (currTolerance.T_CUFTPNtolerance != 09.0 && Math.Abs(PC_CUFTnet) > currTolerance.T_CUFTPNtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_CUFTSNtolerance != 0.0 && Math.Abs(PC_CUFTnet) > currTolerance.T_CUFTSNtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_CUFTPSNtolerance != 0.0 && Math.Abs(PC_CUFTnet) > currTolerance.T_CUFTPSNtolerance)
                            compareResult = 1;

                        if (BDFTgross > 0)
                            PC_BDFTgross = ((BDFTgross - BDFTgrossCC) / BDFTgross) * 100;
                        else PC_BDFTgross = 0;
                        if (BDFTgross == 0.0 && BDFTgrossCC == 0.0 && PC_BDFTgross == 100.0)
                            eRTF.VolumeLine[36] = eRTF.VolumeLine[36].Replace("xx", "na");
                        else if (currTolerance.T_BDFTPGtolerance == 0.0 && currTolerance.T_BDFTSGtolerance == 0.0 &&
                                    currTolerance.T_BDFTPSGtolerance == 0.0)
                            eRTF.VolumeLine[36] = eRTF.VolumeLine[36].Replace("xx", "na");
                        else eRTF.VolumeLine[36] = eRTF.VolumeLine[36].Replace("xx", Utilities.FormatField(Math.Abs(PC_BDFTgross), "{0,2:F0}").ToString());
                        //  is lercent difference greater than tolerance?
                        if (currTolerance.T_BDFTPGtolerance != 0.0 && Math.Abs(PC_BDFTgross) > currTolerance.T_BDFTPGtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_BDFTSGtolerance != 0.0 && Math.Abs(PC_BDFTgross) > currTolerance.T_BDFTSGtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_BDFTPSGtolerance != 0.0 && Math.Abs(PC_BDFTgross) > currTolerance.T_BDFTPSGtolerance)
                            compareResult = 1;

                        if (BDFTnet > 0)
                            PC_BDFTnet = ((BDFTnet - BDFTnetCC) / BDFTnet) * 100;
                        else PC_BDFTnet = 0;
                        if (BDFTnet == 0.0 && BDFTnetCC == 0.0 && PC_BDFTnet == 100.0)
                            eRTF.VolumeLine[39] = eRTF.VolumeLine[39].Replace("xx", "na");
                        else if (currTolerance.T_BDFTPNtolerance == 0.0 && currTolerance.T_BDFTSNtolerance == 0.0 &&
                                    currTolerance.T_BDFTPSNtolerance == 0.0)
                            eRTF.VolumeLine[39] = eRTF.VolumeLine[39].Replace("xx", "na");
                        else eRTF.VolumeLine[39] = eRTF.VolumeLine[39].Replace("xx", Utilities.FormatField(Math.Abs(PC_BDFTnet), "{0,2:F0}").ToString());
                        //  is lercent difference greater than tolerance?
                        if (currTolerance.T_BDFTPNtolerance != 0.0 && Math.Abs(PC_BDFTnet) > currTolerance.T_BDFTPNtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_BDFTSNtolerance != 0.0 && Math.Abs(PC_BDFTnet) > currTolerance.T_BDFTSNtolerance)
                            compareResult = 1;
                        else if (currTolerance.T_BDFTPSNtolerance != 0.0 && Math.Abs(PC_BDFTnet) > currTolerance.T_BDFTPSNtolerance)
                            compareResult = 1;

                    }
                    else
                    {
                        eRTF.VolumeLine[30] = eRTF.VolumeLine[30].Replace("xx", "na");
                        eRTF.VolumeLine[33] = eRTF.VolumeLine[33].Replace("xx", "na");
                        eRTF.VolumeLine[36] = eRTF.VolumeLine[36].Replace("xx", "na");
                        eRTF.VolumeLine[39] = eRTF.VolumeLine[39].Replace("xx", "na");
                    }   //  endif

                    //  print volume lines
                    printLines(writer, eRTF.VolumeLine, 41);

                }   //  endif volume to print
                //  restore volume line with line holds
                for (int k = 0; k < 13; k++)
                    eRTF.VolumeLine[k + 27] = lineHold[k];
            }   //  end foreach loop


            return compareResult;

        }   //  end DisplayVolume


        private int CompleteAndPrintHeader(TolerancesList currTolerance, TextWriter writer, EvaluationReportRTF eRTF, int includeFlag)
        {
            int tableType = 0;
            //  show percent tolerance as needed
            if (includeFlag == 0)
            {
                if (currTolerance.T_CUFTPGtolerance == 1 && currTolerance.T_BDFTPGtolerance == 1)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderPrimary[22] = eRTF.VolumeHeaderPrimary[22].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderPrimary[22] = eRTF.VolumeHeaderPrimary[22].Replace("xxxx", "Product");

                    eRTF.VolumeHeaderPrimary[8] = eRTF.VolumeHeaderPrimary[8].Replace("zzz%", "None");
                    printLines(writer, eRTF.VolumeHeaderPrimary, 28);
                    tableType = 1;
                }
                else if (currTolerance.T_CUFTSGtolerance == 1 && currTolerance.T_BDFTSGtolerance == 1)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderSecondary[22] = eRTF.VolumeHeaderSecondary[22].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderSecondary[22] = eRTF.VolumeHeaderSecondary[22].Replace("xxxx", "Product");

                    eRTF.VolumeHeaderSecondary[8] = eRTF.VolumeHeaderSecondary[8].Replace("zzz%", "None");
                    printLines(writer, eRTF.VolumeHeaderSecondary, 28);
                    tableType = 2;
                }
                else if (currTolerance.T_CUFTPSGtolerance == 1 && currTolerance.T_BDFTPSGtolerance == 1)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderBoth[13] = eRTF.VolumeHeaderBoth[13].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderBoth[13] = eRTF.VolumeHeaderBoth[13].Replace("xxxx", "Product");

                    eRTF.VolumeHeaderBoth[14] = eRTF.VolumeHeaderBoth[14].Replace("zzz%", "None");
                    eRTF.VolumeHeaderBoth[15] = eRTF.VolumeHeaderBoth[15].Replace("zzz%", "None");
                    eRTF.VolumeHeaderBoth[16] = eRTF.VolumeHeaderBoth[16].Replace("zzz%", "None");
                    eRTF.VolumeHeaderBoth[17] = eRTF.VolumeHeaderBoth[17].Replace("zzz%", "None");
                    tableType = 3;
                    printLines(writer, eRTF.VolumeHeaderBoth, 19);
                }   //  endif
            }
            else if (includeFlag > 0)
            {
                if (currTolerance.T_CUFTPGtolerance > 0 || currTolerance.T_BDFTPGtolerance > 0)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderPrimary[22] = eRTF.VolumeHeaderPrimary[22].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderPrimary[22] = eRTF.VolumeHeaderPrimary[22].Replace("xxxx", "Product");

                    eRTF.VolumeHeaderPrimary[8] = eRTF.VolumeHeaderPrimary[8].Replace("zzz", Utilities.FormatField(currTolerance.T_CUFTPGtolerance, "{0,3:F0}").ToString());

                    printLines(writer, eRTF.VolumeHeaderPrimary, 28);
                    tableType = 1;
                }
                else if (currTolerance.T_CUFTSGtolerance > 0 || currTolerance.T_BDFTPGtolerance > 0)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderSecondary[22] = eRTF.VolumeHeaderSecondary[22].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderSecondary[22] = eRTF.VolumeHeaderSecondary[22].Replace("xxxx", "Product");

                    eRTF.VolumeHeaderSecondary[8] = eRTF.VolumeHeaderSecondary[8].Replace("zzz", Utilities.FormatField(currTolerance.T_CUFTSGtolerance, "{0,3:F0}").ToString());

                    printLines(writer, eRTF.VolumeHeaderSecondary, 28);
                    tableType = 2;
                }
                else if (currTolerance.T_CUFTPSGtolerance > 0 || currTolerance.T_BDFTPSGtolerance > 0)
                {
                    if (currTolerance.T_BySpecies == 1)
                        eRTF.VolumeHeaderBoth[13] = eRTF.VolumeHeaderBoth[13].Replace("xxxx", "Species");
                    else if (currTolerance.T_ByProduct == 1)
                        eRTF.VolumeHeaderBoth[13] = eRTF.VolumeHeaderBoth[13].Replace("xxxx", "Product");
                    eRTF.VolumeHeaderBoth[14] = eRTF.VolumeHeaderBoth[14].Replace("zzz", Utilities.FormatField(currTolerance.T_CUFTPSGtolerance, "{0,3:F0}").ToString());
                    eRTF.VolumeHeaderBoth[15] = eRTF.VolumeHeaderBoth[15].Replace("zzz", Utilities.FormatField(currTolerance.T_CUFTPSNtolerance, "{0,3:F0}").ToString());
                    eRTF.VolumeHeaderBoth[16] = eRTF.VolumeHeaderBoth[16].Replace("zzz", Utilities.FormatField(currTolerance.T_BDFTPSGtolerance, "{0,3:F0}").ToString());
                    eRTF.VolumeHeaderBoth[17] = eRTF.VolumeHeaderBoth[17].Replace("zzz", Utilities.FormatField(currTolerance.T_BDFTPSNtolerance, "{0,3:F0}").ToString());

                    tableType = 3;
                    printLines(writer, eRTF.VolumeHeaderBoth, 19);
                }   //  endif

            }   //  endif on includeFlag

            printLines(writer, eRTF.CruiserCheckHeader, 42);
            return tableType;
        }   //  end CompleteAndPrintHeader
    }
}
