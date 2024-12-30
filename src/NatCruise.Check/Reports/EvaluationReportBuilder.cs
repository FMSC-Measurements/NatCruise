using CruiseDAL.DataObjects;
using NatCruise.Check.Data;
using NatCruise.Check.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Reports
{


    public partial class EvaluationReportBuilder
    {
        public static readonly string[] TREE_BASED_CRUISE_METHODS = new[] { "100", "STR", "S3P", "3P" };

        public dataBaseCommands CheckCruiseDataService { get; }

        public ReportLevel ReportLevel { get; }
        public string CruiserName { get; }
        public string CruiserInitials { get; }

        public string SaleName { get; }
        public string Region { get; }
        public List<ResultsList> CheckResults { get; }
        public List<TolerancesList> CruiseTolerances { get; }
        public List<TreeDO> CheckTrees { get; }
        public List<LogStockDO> CheckLogs { get; }

        public string CheckCruiserInitials { get; }

        public EvaluationReportBuilder(dataBaseCommands checkCruiseDataService, ReportLevel reportLevel, string cruiserName, string cruiserInitials)
        {
            CheckCruiseDataService = checkCruiseDataService;

            ReportLevel = reportLevel;

            SaleName = CheckCruiseDataService.getSaleName();
            Region = CheckCruiseDataService.getRegion();
            CheckLogs = CheckCruiseDataService.getLogStock();
            CruiseTolerances = CheckCruiseDataService.getTolerances();
            CheckCruiserInitials = CheckCruiseDataService.getCheckCruiserInitials();

            if (reportLevel == ReportLevel.BySale)
            {
                CheckResults = CheckCruiseDataService.getResultsTable("", "");
                CheckTrees = CheckCruiseDataService.getTrees();
            }
            else if (reportLevel == ReportLevel.ByCruiser)
            {
                if (cruiserInitials == null) throw new ArgumentNullException(nameof(cruiserInitials));
                if (cruiserName == null) throw new ArgumentNullException(nameof(cruiserName));

                CheckResults = CheckCruiseDataService.getResultsTable("", cruiserInitials);

                CheckTrees = new List<TreeDO>();
                var rawTrees = CheckCruiseDataService.getTrees();
                foreach (var cr in CheckResults)
                {
                    var tree = rawTrees.Find(x => x.Stratum.Code == cr.R_Stratum && x.CuttingUnit.Code == cr.R_CuttingUnit
                                    && x.TreeNumber.ToString() == cr.R_TreeNumber);
                    if (tree != null)
                    { CheckTrees.Add(tree); }
                }
            }
        }

        public void CreateReport(TextWriter writer)
        {
            //  first, check if the entire cruise has tree-based methods
            //  the reason being, in/out trees are not checked and need to be
            //  suppressed from the evaluation report and the pass/fail
            //  calculation --  July 2015
            var hasAllTreeBasedStrata = CheckHasAllTreeBasedStrata(CheckCruiseDataService);

            //  create RTF objer
            EvaluationReportRTF evalRTF = new EvaluationReportRTF();

            float totalOverallErr = 0;

            int overallTotalPossible = 0;
            var passFail = 0; // 1 is fail 0 is pass

            //  update main header and print
            evalRTF.MainHeader[13] = evalRTF.MainHeader[13].Replace("XXXXX", SaleName);
            evalRTF.MainHeader[16] = evalRTF.MainHeader[16].Replace("XX", Region);
            printLines(writer, evalRTF.MainHeader, 18);

            if (ReportLevel == ReportLevel.ByCruiser)
            {
                //  update and print cruiser ID and name
                evalRTF.CruiserID[7] = evalRTF.CruiserID[7].Replace("XXXX", CruiserName);
                printLines(writer, evalRTF.CruiserID, 9);
                evalRTF.CruiserName[7] = evalRTF.CruiserName[7].Replace("XXXX", CruiserInitials);
                printLines(writer, evalRTF.CruiserName, 9);
            }

            WriteTreeMeasurmentsTable(writer, hasAllTreeBasedStrata, evalRTF, ref totalOverallErr, ref overallTotalPossible, ref passFail);

            //  printing volume goes here
            //  June 2015 -- volume section has been revised
            //  check for include volume is moved to volume output
            int nResult = 0;
            if (ReportLevel == ReportLevel.BySale)
            {
                nResult = DisplayVolume(writer, CruiseTolerances[0], "");
            }
            else if (ReportLevel == ReportLevel.ByCruiser)
            {
                nResult = DisplayVolume(writer, CruiseTolerances[0], CruiserInitials);
            }

            //  printing rest of page goes here
            float calcError = (float)((1.0 - totalOverallErr / overallTotalPossible)) * 100;
            if (passFail == 1 || calcError < CruiseTolerances[0].T_OverallAccuracy || nResult == 1)
                evalRTF.EvaluationLines[1] = evalRTF.EvaluationLines[1].Replace("XXXX", "FAIL");
            else evalRTF.EvaluationLines[1] = evalRTF.EvaluationLines[1].Replace("XXXX", "PASS");
            printLines(writer, evalRTF.EvaluationLines, 2);

            printLines(writer, evalRTF.FirstText, 6);
            printLines(writer, evalRTF.SecondText, 5);

            evalRTF.ThirdText[2] = evalRTF.ThirdText[2].Replace("XXX", Utilities.FormatField(CruiseTolerances[0].T_ElementAccuracy, "{0,3:F0}").ToString());
            evalRTF.ThirdText[2] = evalRTF.ThirdText[2].Replace("ZZZ", Utilities.FormatField(CruiseTolerances[0].T_OverallAccuracy, "{0,3:F0}").ToString());
            printLines(writer, evalRTF.ThirdText, 5);

            printLines(writer, evalRTF.CommentSection, 27);
            printLines(writer, evalRTF.SignatureBlock, 22);

            writer.WriteLine("}");
        }

        private void WriteTreeMeasurmentsTable(TextWriter writer, bool hasAllTreeBasedStrata, EvaluationReportRTF evalRTF, ref float totalOverallErr, ref int overallTotalPossible, ref int passFail)
        {
            int numElements = 0;
            float totalAccuracyScore = 0;

            //  need a small array for element line so replacement doesn't mess up original;
            string hold15 = evalRTF.ElementLine[15];
            string hold16 = evalRTF.ElementLine[16];
            string hold17 = evalRTF.ElementLine[17];
            string hold18 = evalRTF.ElementLine[18];
            string hold19 = evalRTF.ElementLine[19];
            string hold20 = evalRTF.ElementLine[20];
            string hold21 = evalRTF.ElementLine[21];

            //  Print TreeMeasurments Table
            printLines(writer, evalRTF.FirstHeader, 25); // Print Table Title and first row of headers
            printLines(writer, evalRTF.SecondHeader, 23); // Second row of headers
            printLines(writer, evalRTF.ThirdHeader, 23); // Third row of headers

            // output tolerances list
            foreach (TolerancesList tl in CruiseTolerances)
            {
                //  if the cruise is all tree-based strata and element is in/out trees
                //  skip printing of this element
                if (!hasAllTreeBasedStrata || tl.T_Element != "In/Out Trees")
                {
                    StringBuilder tempString = new StringBuilder();
                    //  insert element
                    if (tl.T_Element != "" || tl.T_Element != null)
                        evalRTF.ElementLine[15] = evalRTF.ElementLine[15].Replace("XXXXX", tl.T_Element);

                    //  insert tolerance, units and additional parameter
                    if (tl.T_Tolerance == "None")
                    {
                        evalRTF.ElementLine[16] = evalRTF.ElementLine[16].Replace("TTTT", tl.T_Tolerance);
                    }
                    else if (tl.T_Tolerance != "None")
                    {
                        tempString.Remove(0, tempString.Length);
                        tempString.Append("+/- ");
                        tempString.Append(tl.T_Tolerance);
                        tempString.Append(" ");
                        tempString.Append(tl.T_Units);
                        if (tl.T_AddParam != "None")
                        {
                            tempString.Append(" or ");
                            tempString.Append(tl.T_AddParam);
                        }   //  endif
                        evalRTF.ElementLine[16] = evalRTF.ElementLine[16].Replace("TTTT", tempString.ToString());
                    }   //  endif

                    //  put weight in correct position
                    tempString.Remove(0, tempString.Length);
                    tempString.Append(Utilities.FormatField(tl.T_Weight, "{0,3:F0}").ToString());
                    evalRTF.ElementLine[19] = evalRTF.ElementLine[19].Replace("AAAA", tempString.ToString());

                    //  Calculate values for each element except in/out trees
                    passFail = CalculateValues(tl, out float totalPossible, out int numberIncorrect, out float totalError, out float accuracyScore,
                                            CruiseTolerances[0].T_ElementAccuracy) ? passFail : 1; // passFail value of 1 is fail

                    //  load element line with values
                    evalRTF.ElementLine[17] = evalRTF.ElementLine[17].Replace("YYYY", Utilities.FormatField(totalPossible, "{0,4:F0}").ToString());
                    evalRTF.ElementLine[18] = evalRTF.ElementLine[18].Replace("ZZZZ", Utilities.FormatField(numberIncorrect, "{0,4:F0}").ToString());
                    evalRTF.ElementLine[20] = evalRTF.ElementLine[20].Replace("BBBB", Utilities.FormatField(totalError, "{0,4:F0}").ToString());
                    evalRTF.ElementLine[21] = evalRTF.ElementLine[21].Replace("DDDD", Utilities.FormatField(accuracyScore, "{0,4:F0}").ToString());
                    //  When an area-based stratum is not checked, the in/out element shows zero for everything
                    //  The accuracy of zero causes the check to fail so need to suppress  printing that element
                    //  If there are multiple strata that are area-based, even one checked would prevent the
                    //  element from being suppressed since total possible will be greater than zero
                    //  February 2016
                    if (totalPossible > 0)
                        printLines(writer, evalRTF.ElementLine, 23);

                    //  add values to overall totals
                    overallTotalPossible += (int)totalPossible;
                    totalOverallErr += totalError;
                    totalAccuracyScore += accuracyScore;
                    numElements++;

                    //  replace lines in the element line to get originals back for next element store
                    evalRTF.ElementLine[15] = hold15;
                    evalRTF.ElementLine[16] = hold16;
                    evalRTF.ElementLine[17] = hold17;
                    evalRTF.ElementLine[18] = hold18;
                    evalRTF.ElementLine[19] = hold19;
                    evalRTF.ElementLine[20] = hold20;
                    evalRTF.ElementLine[21] = hold21;
                }   //  endif all tree-based strata
            }   //  end foreach

            //  print total line
            evalRTF.TotalLine[17] = evalRTF.TotalLine[17].Replace("ZZZZ", Utilities.FormatField(overallTotalPossible, "{0,4:F0}").ToString());
            evalRTF.TotalLine[20] = evalRTF.TotalLine[20].Replace("BBBB", Utilities.FormatField(totalOverallErr, "{0,3:F0}").ToString());
            if (totalAccuracyScore > 0)
            {
                float calcValue = (float)totalAccuracyScore / numElements;
                evalRTF.TotalLine[21] = evalRTF.TotalLine[21].Replace("DDDD",
                    Utilities.FormatField(calcValue, "{0,3:F0}").ToString());
            }   //  endif
            printLines(writer, evalRTF.TotalLine, 23);
        }

        private bool CheckHasAllTreeBasedStrata(dataBaseCommands checkCruiseDataService)
        {
            //  check each stratum in checkResults for all tree-based methods
            List<StratumDO> sList = checkCruiseDataService.getStrata();

            return sList.All(x => TREE_BASED_CRUISE_METHODS.Contains(x.Method));

            //int numTreeBased = 0;
            //foreach (StratumDO s in sList)
            //{
            //    if (s.Method == "100" || s.Method == "STR" ||
            //        s.Method == "S3P" || s.Method == "3P")
            //        numTreeBased++;
            //}   //  end foreach
            //if (numTreeBased == sList.Count)
            //{
            //    //  can't remove in/out element since that line stores other
            //    //  flags for volume
            //    //  so set a flag to indicate all strata were tree-based
            //    return numTreeBased;
            //}   //  endif
            //return 0;
        }   //  end CheckStrata

        private bool CalculateValues(TolerancesList currTL, out float totalPossible, out int numberIncorrect,
                            out float totalError, out float accuracyScore, float elementAccuracy)
        {
            //  this has to change slightly for log elements
            //  total possible is the total number of logs in the selected units.  even though only three trees may have been checked
            if (currTL.T_Element == "Log Grade" || currTL.T_Element == "Log Defect %")
            {
                List<ResultsList> justLogs = CheckResults.FindAll(
                    delegate (ResultsList r)
                    {
                        return r.R_CountMeasure == "" && r.R_LogGrade_R >= 0;
                    });
                totalPossible = justLogs.Count();
            }
            else
            {
                //  find total possible
                totalPossible = TrueNumberOfTrees(currTL.T_Element, CheckCruiserInitials);
            }   //  endif

            //  number incorrect
            List<ResultsList> justIncorrect = findNumberIncorrect(currTL.T_Element, CheckResults);
            numberIncorrect = justIncorrect.Count();

            //  calculate total error
            totalError = numberIncorrect * currTL.T_Weight;
            if ((Region == "10" && currTL.T_Element == "Log Defect %") ||
                (Region == "10" && currTL.T_Element == "Log Grade"))
                totalError = (float)(justIncorrect.Count * 0.85);

            //  Calculate accuracy score
            accuracyScore = (totalPossible > 0.0) ? (float)(1.00 - (totalError / totalPossible)) * 100 : 0.0f;

            //  if score is less than individual accuracy score, cruiser fails on in/out trees
            return !(accuracyScore < elementAccuracy && totalPossible > 0);
        }   //  end CalculateValues

        private float TrueNumberOfTrees(string currElement, string CCinitials)
        {
            float returnTrees = 0;
            //  pull stratum list to get two stage methods separated from other methods
            List<StratumDO> sList = CheckCruiseDataService.getStrata();
            if (currElement == "In/Out Trees")
            {
                foreach (StratumDO sl in sList)
                {
                    switch (sl.Method)
                    {
                        case "FIX":
                        case "PNT":
                        case "FIXCNT":
                            //  pull strata from checkResults
                            List<ResultsList> justStrata = CheckResults.FindAll(
                                delegate (ResultsList rl)
                                {
                                    return rl.R_Stratum == sl.Code &&
                                        (rl.R_LogNumber == "" || rl.R_LogNumber == "0");
                                });
                            returnTrees += justStrata.Count();
                            break;

                        case "P3P":
                        case "F3P":
                        case "PCM":
                        case "FCM":
                        case "3PPNT":
                            //  pull strata from check cruiser trees
                            List<ResultsList> justCounts = CheckResults.FindAll(
                                delegate (ResultsList r)
                                {
                                    return r.R_Stratum == sl.Code &&
                                        r.R_CountMeasure == "C";
                                });
                            returnTrees += justCounts.Count();
                            List<ResultsList> justMeasured = CheckResults.FindAll(
                                delegate (ResultsList r)
                                {
                                    return r.R_Stratum == sl.Code &&
                                        r.R_CountMeasure == "M";
                                });
                            returnTrees += justMeasured.Count();
                            break;
                    }   //  end switch on method
                }   // end foreach loop
                //  pull any out trees from check cruiser results and delete from returnTrees
                List<ResultsList> justInOut = CheckResults.FindAll(
                    delegate (ResultsList r)
                    {
                        return r.R_OutResult == 1;
                    });
                returnTrees -= justInOut.Count;
            }
            else
            {
                List<ResultsList> justStratum = new List<ResultsList>();
                foreach (StratumDO sl in sList)
                {
                    switch (sl.Method)
                    {
                        case "PCM":
                        case "FCM":
                        case "3PPNT":
                        case "F3P":
                        case "P3P":
                        case "S3P":
                            //  pull check cruiser measured trees for two-stage methods
                            List<ResultsList> justMeasured = CheckResults.FindAll(
                                delegate (ResultsList r)
                                {
                                    return r.R_Stratum == sl.Code &&
                                        r.R_CountMeasure == "M";
                                });
                            returnTrees += justMeasured.Count();

                            //  now add counts for species element
                            if (currElement == "Species")
                            {
                                List<ResultsList> justCounts = CheckResults.FindAll(
                                    delegate (ResultsList r)
                                    {
                                        return r.R_Stratum == sl.Code &&
                                            r.R_CountMeasure == "C";
                                    });
                                returnTrees += justCounts.Count();
                                //  any in/out errors to be deducted?
                                List<ResultsList> justInOut = CheckResults.FindAll(
                                    delegate (ResultsList js)
                                    {
                                        return js.R_Stratum == sl.Code && js.R_OutResult == 1;
                                    });
                                returnTrees -= justInOut.Count();
                            }   //  endif
                            break;

                        default:
                            //  find all check records for current stratum
                            justStratum = CheckResults.FindAll(
                                delegate (ResultsList cr)
                                {
                                    return cr.R_Stratum == sl.Code && cr.R_CountMeasure == "M";
                                });
                            returnTrees += justStratum.Count();

                            break;
                    }   //  end switch
                }   //  end foreach loop
            }   //  endif on element

            return returnTrees;
        }   //  end TrueNumberOfTrees

        private static List<ResultsList> findNumberIncorrect(string currElement, List<ResultsList> checkResults)
        {
            List<ResultsList> justIncorrect = new List<ResultsList>();
            switch (currElement)
            {
                case "Species":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_Species_R == 1;
                        });
                    break;

                case "Product":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_Product_R == 1;
                        });
                    break;

                case "LiveDead":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_LiveDead_R == 1;
                        });
                    break;

                case "DBH":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_DBHOB_R == 1;
                        });
                    break;

                case "Total Height":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_TotalHeight_R == 1;
                        });
                    break;

                case "Total Height <= 100 feet":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_TotHeightUnder_R == 1;
                        });
                    break;

                case "Total Height > 100 feet":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_TotHeightOver_R == 1;
                        });
                    break;

                case "Merch Height Primary":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_MerchHgtPP_R == 1;
                        });
                    break;

                case "Merch Height Secondary":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_MerchHgtSP_R == 1;
                        });
                    break;

                case "Height to First Live Limb":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_HgtFLL_R == 1;
                        });
                    break;

                case "Top DIB Primary":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_TopDIBPP_R == 1;
                        });
                    break;

                case "Seen Defect % Primary":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_SeenDefPP_R == 1;
                        });
                    break;

                case "Seen Defect % Secondary":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_SeenDefSP_R == 1;
                        });
                    break;

                case "Recoverable %":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_RecDef_R == 1;
                        });
                    break;

                case "Form class":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_FormClass_R == 1;
                        });
                    break;

                case "Clear Face":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_Clear_R == 1;
                        });
                    break;

                case "Tree Grade":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_TreeGrade_R == 1;
                        });
                    break;

                case "Log Grade":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_LogGrade_R == 1;
                        });
                    break;

                case "Log Defect %":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_LogSeenDef_R == 1;
                        });
                    break;

                case "In/Out Trees":
                    justIncorrect = checkResults.FindAll(
                        delegate (ResultsList cr)
                        {
                            return cr.R_InResult == 1 || cr.R_OutResult == 1;
                        });
                    break;
            }   //  end switch
            return justIncorrect;
        }   // findNumberIncorrect

        // TODO remove nRows since we can just get length from arrayToPrint
        public static void printLines(TextWriter writer, string[] arrayToPrint, int nRows)
        {
            for (int k = 0; k < nRows; k++)
            {
                writer.WriteLine(arrayToPrint[k]);
            }
        }
    }
}