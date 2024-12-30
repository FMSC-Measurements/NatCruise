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

        public string CruiserInitials { get; }

        public string SaleName { get; protected set; }
        public string Region { get; protected set; }
        public List<ResultsList> CheckResults { get; protected set; }
        public List<TreeDO> CheckTrees { get; protected set; }
        public List<LogStockDO> CheckLogs { get; protected set; }

        public string CheckCruiserInitials { get; protected set; }

        public EvaluationReportBuilder(dataBaseCommands checkCruiseDataService)
        {
            CheckCruiseDataService = checkCruiseDataService;
        }

        public void Initialize()
        {
            SaleName = CheckCruiseDataService.getSaleName();
            Region = CheckCruiseDataService.getRegion();
            CheckLogs = CheckCruiseDataService.getLogStock();
            //CruiseTolerances = CheckCruiseDataService.getTolerances();
            CheckCruiserInitials = CheckCruiseDataService.getCheckCruiserInitials();
        }

        public void BuildReport(TextWriter writer, ToleranceConfig toleranceConfig, ReportLevel reportLevel, string cruiserName, string cruiserInitials)
        {
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

            //  create RTF objer
            EvaluationReportRTF evalRTF = new EvaluationReportRTF();

            double totalOverallErr = 0;

            int overallTotalPossible = 0;

            //  update main header and print
            evalRTF.MainHeader[13] = evalRTF.MainHeader[13].Replace("XXXXX", SaleName);
            evalRTF.MainHeader[16] = evalRTF.MainHeader[16].Replace("XX", Region);
            printLines(writer, evalRTF.MainHeader, 18);

            if (reportLevel == ReportLevel.ByCruiser)
            {
                //  update and print cruiser ID and name
                evalRTF.CruiserID[7] = evalRTF.CruiserID[7].Replace("XXXX", cruiserName);
                printLines(writer, evalRTF.CruiserID, 9);
                evalRTF.CruiserName[7] = evalRTF.CruiserName[7].Replace("XXXX", cruiserInitials);
                printLines(writer, evalRTF.CruiserName, 9);
            }

            var passFail = WriteTreeMeasurmentsTable(writer,
                toleranceConfig,
                evalRTF,
                ref totalOverallErr,
                ref overallTotalPossible);

            //  printing volume goes here
            //  June 2015 -- volume section has been revised
            //  check for include volume is moved to volume output
            PassFail volumeComparisonResult = PassFail.Pass;
            if (reportLevel == ReportLevel.BySale)
            {
                volumeComparisonResult = DisplayVolume(writer, toleranceConfig.VolumeConfig, "");
            }
            else if (reportLevel == ReportLevel.ByCruiser)
            {
                volumeComparisonResult = DisplayVolume(writer, toleranceConfig.VolumeConfig, cruiserInitials);
            }

            //  printing rest of page goes here
            double calcError = (1.0 - totalOverallErr / overallTotalPossible) * 100.0;
            if (passFail == PassFail.Fail
                || calcError < toleranceConfig.MinOverallAccuracy
                || volumeComparisonResult == PassFail.Fail)
             {
                evalRTF.EvaluationLines[1] = evalRTF.EvaluationLines[1].Replace("XXXX", "FAIL");
            }
            else
            {
                evalRTF.EvaluationLines[1] = evalRTF.EvaluationLines[1].Replace("XXXX", "PASS");
            }
            printLines(writer, evalRTF.EvaluationLines, 2);

            printLines(writer, evalRTF.FirstText, 6);
            printLines(writer, evalRTF.SecondText, 5);

            evalRTF.ThirdText[2] = evalRTF.ThirdText[2].Replace("XXX", Utilities.FormatField(toleranceConfig.MinElementAccuracy, "{0,3:F0}").ToString());
            evalRTF.ThirdText[2] = evalRTF.ThirdText[2].Replace("ZZZ", Utilities.FormatField(toleranceConfig.MinOverallAccuracy, "{0,3:F0}").ToString());
            printLines(writer, evalRTF.ThirdText, 5);

            printLines(writer, evalRTF.CommentSection, 27);
            printLines(writer, evalRTF.SignatureBlock, 22);

            writer.WriteLine("}");
        }

        private PassFail WriteTreeMeasurmentsTable(TextWriter writer,
            ToleranceConfig toleranceConfig,
            EvaluationReportRTF evalRTF,
            ref double totalOverallErr,
            ref int overallTotalPossible)
        {
            //  first, check if the entire cruise has tree-based methods
            //  the reason being, in/out trees are not checked and need to be
            //  suppressed from the evaluation report and the pass/fail
            //  calculation --  July 2015
            var hasAllTreeBasedStrata = CheckHasAllTreeBasedStrata(CheckCruiseDataService);

            PassFail passFail = PassFail.Pass;
            int numElements = 0;
            double totalAccuracyScore = 0;

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
            foreach (TolerancesList tl in toleranceConfig.Tolerances)
            {
                //  if the cruise is all tree-based strata and element is in/out trees
                //  skip printing of this element
                if (hasAllTreeBasedStrata && tl.T_Element == "In/Out Trees")
                { continue; }

                //  insert element
                if (tl.T_Element != "" || tl.T_Element != null)
                    evalRTF.ElementLine[15] = evalRTF.ElementLine[15].Replace("XXXXX", tl.T_Element);

                //  insert tolerance, units and additional parameter
                if (tl.T_Tolerance == "None")
                {
                    evalRTF.ElementLine[16] = evalRTF.ElementLine[16].Replace("TTTT", tl.T_Tolerance);
                }
                else
                {
                    var str = "+/- " + tl.T_Tolerance + " " + tl.T_Units;
                    if (tl.T_AddParam != "None")
                    {
                        str = str + " or " + tl.T_AddParam;
                    }
                    evalRTF.ElementLine[16] = evalRTF.ElementLine[16].Replace("TTTT", str);
                }   //  endif

                //  put weight in correct position
                evalRTF.ElementLine[19] = evalRTF.ElementLine[19]
                    .Replace("AAAA", Utilities.FormatField(tl.T_Weight, "{0,3:F0}").ToString());

                //  Calculate values for each element except in/out trees
                if (!CalculateValues(tl,
                    out int totalPossible,
                    out int numberIncorrect,
                    out double totalError,
                    out double accuracyScore,
                    toleranceConfig.MinElementAccuracy))
                { passFail = PassFail.Fail; }

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
                overallTotalPossible += totalPossible;
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

            return passFail;
        }

        private bool CheckHasAllTreeBasedStrata(dataBaseCommands checkCruiseDataService)
        {
            //  check each stratum in checkResults for all tree-based methods
            List<StratumDO> sList = checkCruiseDataService.getStrata();

            // TODO it would probably be more effective to check for any plot based methods
            // than to check that all strata are tree based, but that was what the original implimentation did
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

        private bool CalculateValues(TolerancesList currTL, out int totalPossible, out int numberIncorrect,
                            out double totalError, out double accuracyScore, double elementAccuracy)
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
            totalError = numberIncorrect * currTL.T_Weight; // TODO is weighting the error at this point a bug, shouldn't wight be applied later
            if ((Region == "10" && currTL.T_Element == "Log Defect %") ||
                (Region == "10" && currTL.T_Element == "Log Grade"))
            { totalError = numberIncorrect * 0.85; } // TODO remove magic element weight for R10

            //  Calculate accuracy score
            accuracyScore = (totalPossible > 0.0) ? (1.00 - (totalError / totalPossible)) * 100.0 : 0.0;

            //  if score is less than individual accuracy score, cruiser fails on in/out trees
            return !(accuracyScore < elementAccuracy && totalPossible > 0);
        }   //  end CalculateValues

        private int TrueNumberOfTrees(string currElement, string CCinitials)
        {
            int returnTrees = 0;
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