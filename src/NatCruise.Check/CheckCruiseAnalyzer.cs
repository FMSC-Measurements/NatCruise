using CruiseDAL.DataObjects;
using Microsoft.Extensions.Logging;
using NatCruise.Check.Data;
using NatCruise.Check.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace NatCruise.Check
{
    public class CheckCruiseAnalyzer
    {
        private ILogger? Log { get; }
        public dataBaseCommands CheckCruiseDataService { get; }
        public dataBaseCommands CruiseDataService { get; }

        public CheckCruiseAnalyzer(dataBaseCommands checkCruiseDataService,
            dataBaseCommands cruiseDataService,
            ILogger<CheckCruiseAnalyzer> log)
        {
            CheckCruiseDataService = checkCruiseDataService;
            CruiseDataService = cruiseDataService;
            Log = log;
        }


        public Task AnalyzeAsync(string? checkCruiserInitials, ToleranceConfig toleranceConfig, IProgress<string>? progress = null)
        {
            return Task.Run(() => Analyze(checkCruiserInitials, toleranceConfig, progress));
        }

        public void Analyze(string? checkCruiserInitials, ToleranceConfig toleranceConfig, IProgress<string>? progress = null)
        {
            Log?.LogInformation("Starting Analize Check Cruise");
            var warnings = new List<string>();

            List<TreeDO> checkTrees = CheckCruiseDataService.getTrees();
            List<TreeCalculatedValuesDO> checkCalculatedTrees = CheckCruiseDataService.getCalculatedTrees();
            //List<TolerancesList> cruiseTolerances = CheckCruiseDataService.getTolerances();
            List<LogStockDO> checkLogs = CheckCruiseDataService.getLogStock();
            var justStrataUnits = CheckCruiseDataService.getStrataUnit();
            string currRegion = CheckCruiseDataService.getRegion();
            checkCruiserInitials ??= CheckCruiseDataService.getCheckCruiserInitials();

            if (String.IsNullOrEmpty(checkCruiserInitials))
            {
                Log?.LogWarning("No Value For Check Cruiser Initials");
                throw new ArgumentException("No Value For Check Cruiser Initials");
            }

            if (!toleranceConfig.Tolerances.Any())
            {
                Log?.LogWarning("No Tolerances Configured");
                throw new InvalidOperationException("No Tolerances Configured");
            }

            bool areCheckingVolumes = toleranceConfig.VolumeConfig?.IncludeVolumes ?? false;

            if (checkCalculatedTrees.Sum(c => c.GrossCUFTPP) == 0.0
                && checkCalculatedTrees.Sum(c => c.GrossBDFTPP) == 0.0)
            {
                var message = "No calculated volume detected in check cruise file!\nPlease process the check cruise file in CruiseProcessing\nand retry Check Cruise.";
                Log?.LogWarning(message);
                throw new InvalidOperationException(message);
            }


            // load data from original cruise and check it
            List<TreeDO> originalTrees = CruiseDataService.getTrees();
            List<TreeCalculatedValuesDO> originalCalculatedTrees = CruiseDataService.getCalculatedTrees();
            List<LogStockDO> originalLogs = CruiseDataService.getLogStock();

            if (originalCalculatedTrees.Sum(o => o.GrossCUFTPP) == 0.0
                && originalCalculatedTrees.Sum(o => o.GrossBDFTPP) == 0.0)
            {
                throw new InvalidOperationException("No calculated volume detected in cruise file!\nPlease process the cruise file in CruiseProcessing\nand retry Check Cruise.");
            }

            var longestInitialValue = originalTrees.Select(i => i.Initials.Length).Max();
            if (longestInitialValue > 3)
            {
                warnings.Add("Tree with longer than three character initials found");
            }

            //var hasMultipleInitials = originalTrees.GroupBy(x => x.Initials).Count() > 1;
            //if(hasMultipleInitials)
            //{

            //}

            if (!checkTrees.Any(t => t.Initials == checkCruiserInitials))
            {
                throw new InvalidOperationException("NO TREES IN THE CHECK CRUISE FILE HAVE CHECK CRUISER INITIALS!\nCannot continue with anaylsis.\n Please record check cruiser initials on checked trees.");
            }

            var initialiedTrees = checkTrees.Where(t => t.Initials == checkCruiserInitials).ToArray();
            var checkMeasureTrees = initialiedTrees.Where(t => t.CountOrMeasure == "M").ToArray();


            //  fill results table -- clear table first
            //  July 2015 -- per conversation with C.Bodenhausen, looks like all count and measure
            //  trees need to go in the results table.  Mainly because a count record could be
            //  called in by the check cruiser and needs to be noted in the results table
            CheckCruiseDataService.clearResults();
            var checkResults = fillResultsTable(initialiedTrees);
            CheckCruiseDataService.saveResults(checkResults);
            // TODO this is redundant can we get rid of this?
            checkResults = CheckCruiseDataService.getResultsTable("", "");

            //  loop through tolerance elements to compare
            foreach (TolerancesList ct in toleranceConfig.Tolerances)
            {
                //  if current element is In/Out trees need to do analysis differently
                //  need original trees and check trees complete for comparison
                if (ct.T_Element == "In/Out Trees")
                    CheckInOutTrees(ct, checkTrees, originalTrees, checkCruiserInitials, checkResults, justStrataUnits);
                else if (ct.T_Element == "Second Height Accuracy")
                {
                    //  element is used for Region 8 only and based on upper stem height
                    foreach (TreeDO jc in checkMeasureTrees)
                    {
                        //  find cruiser tree for current check tree
                        TreeDO cruiserTree = originalTrees.Find(
                            delegate (TreeDO t)
                            {
                                return t.Tree_CN == jc.Tree_CN;
                            });
                        var nthResult = plusOrMinus(cruiserTree.UpperStemHeight, jc.UpperStemHeight, ct.T_Tolerance, ct.T_Units);

                    }

                }
                else if (ct.T_Element == "Second Height Bias")
                {
                    //  element is used for Region 8 only and is based on an average height for upper tem HT
                    //  find average height for upper stem height in checked trees
                    int numChecked = checkTrees.Count;
                    float sumCheckedUSH = checkTrees.Sum(c => c.UpperStemHeight);
                    int numCruiser = originalTrees.Count;
                    float sumCruiserUSH = originalTrees.Sum(o => o.UpperStemHeight);
                    float checkedAverage = sumCheckedUSH / numChecked;
                    float cruiserAverage = sumCruiserUSH / numCruiser;
                    var nthResult = plusOrMinus((float)cruiserAverage, (float)checkedAverage, ct.T_Tolerance, ct.T_Units);
                }
                else
                {
                    foreach (TreeDO td in checkMeasureTrees)
                    {
                        //  find cruiser tree for current check tree
                        TreeDO cruiserTree = originalTrees.Find(
                            delegate (TreeDO t)
                            {
                                return t.Tree_CN == td.Tree_CN;
                            });
                        if (cruiserTree != null)
                        {
                            PassFail nthResult = 0; // pass = 0/ fail = 1
                            switch (ct.T_Element)
                            {
                                case "Species":
                                    nthResult = straightComparison(cruiserTree.Species, td.Species);
                                    break;
                                case "Live/Dead":
                                    nthResult = straightComparison(cruiserTree.LiveDead, td.LiveDead);
                                    break;
                                case "Product":
                                    nthResult = straightComparison(cruiserTree.SampleGroup.PrimaryProduct, td.SampleGroup.PrimaryProduct);
                                    break;
                                case "DBH":
                                    nthResult = DBHanalysis(td, cruiserTree, ct);
                                    break;
                                case "Total Height <= 100 feet":
                                case "Total Height > 100 feet":
                                case "Total Height":
                                    nthResult = totalHeightAnalysis(td, cruiserTree, ct);
                                    break;
                                case "Merch Height Primary":
                                    nthResult = plusOrMinus(cruiserTree.MerchHeightPrimary, td.MerchHeightPrimary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Merch Height Secondary":
                                    nthResult = plusOrMinus(cruiserTree.MerchHeightSecondary, td.MerchHeightSecondary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Height to First Live Limb":
                                    nthResult = plusOrMinus(cruiserTree.HeightToFirstLiveLimb, td.HeightToFirstLiveLimb, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Top DIB Primary":
                                    nthResult = plusOrMinus(cruiserTree.TopDIBPrimary, td.TopDIBPrimary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Seen Defect % Primary":
                                    if (currRegion == "5" || currRegion == "05")
                                    {
                                        //  find current tree in calculated values
                                        TreeCalculatedValuesDO oct = originalCalculatedTrees.Find(
                                            delegate (TreeCalculatedValuesDO tcv)
                                            {
                                                return tcv.Tree_CN == td.Tree_CN;
                                            });
                                        TreeCalculatedValuesDO cct = checkCalculatedTrees.Find(
                                            delegate (TreeCalculatedValuesDO tcv)
                                            {
                                                return tcv.Tree_CN == td.Tree_CN;
                                            });
                                        nthResult = plusOrMinus(ct.T_Tolerance, oct, cct);
                                    }
                                    else nthResult = plusOrMinus(cruiserTree.SeenDefectPrimary, td.SeenDefectPrimary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Seen Defect % Secondary":
                                    nthResult = plusOrMinus(cruiserTree.SeenDefectSecondary, td.SeenDefectSecondary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Recoverable %":
                                    nthResult = plusOrMinus(cruiserTree.RecoverablePrimary, td.RecoverablePrimary, ct.T_Tolerance, ct.T_Units);
                                    break;
                                case "Form class":
                                    nthResult = straightComparison(cruiserTree.FormClass, td.FormClass);
                                    break;
                                case "Clear Face":
                                    nthResult = straightComparison(cruiserTree.ClearFace, td.ClearFace);
                                    break;
                                case "Tree Grade":
                                    nthResult = straightComparison(cruiserTree.Grade, td.Grade);
                                    break;
                                case "Log Grade":
                                    {
                                        //  find logs for current tree
                                        var checkTreeLogs = checkLogs.FindAll(
                                            delegate (LogStockDO l)
                                            {
                                                return l.Tree_CN == td.Tree_CN;
                                            });
                                        var cruiserTreeLogs = originalLogs.FindAll(
                                            delegate (LogStockDO ls)
                                            {
                                                return ls.Tree_CN == td.Tree_CN;
                                            });
                                        if (currRegion == "10")
                                            checkR10logs(checkTreeLogs, cruiserTreeLogs, td, ct, cruiserTree.Initials, checkResults);
                                        else
                                        {
                                            foreach (LogStockDO ctl in checkTreeLogs)
                                            {
                                                int nthRow = cruiserTreeLogs.FindIndex(
                                                    delegate (LogStockDO ls)
                                                    {
                                                        return ls.LogNumber == ctl.LogNumber;
                                                    });
                                                if (nthRow >= 0)
                                                {
                                                    nthResult = straightComparison(cruiserTreeLogs[nthRow].Grade, ctl.Grade);
                                                    updateLogChecks(td, ctl, nthResult, 1, cruiserTree.Initials, checkResults);
                                                }   //  endif
                                            }   //  end foreach loop
                                        }   //  endif                                    
                                        break;
                                    }
                                case "Log Defect %":
                                    {
                                        //  find logs for current tree
                                        var checkTreeLogs = checkLogs.FindAll(
                                            delegate (LogStockDO l)
                                            {
                                                return l.Tree_CN == td.Tree_CN;
                                            });
                                        var cruiserTreeLogs = originalLogs.FindAll(
                                            delegate (LogStockDO ls)
                                            {
                                                return ls.Tree_CN == td.Tree_CN;
                                            });
                                        if (currRegion == "10")
                                            checkR10logs(checkTreeLogs, cruiserTreeLogs, td, ct, cruiserTree.Initials, checkResults);
                                        else if (currRegion == "6" || currRegion == "06")
                                            checkR6logs(checkTreeLogs, cruiserTreeLogs, td, ct, cruiserTree.Initials, checkResults);
                                        else
                                        {
                                            foreach (LogStockDO ctl in checkTreeLogs)
                                            {
                                                int nthRow = cruiserTreeLogs.FindIndex(
                                                    delegate (LogStockDO ls)
                                                    {
                                                        return ls.LogNumber == ctl.LogNumber;
                                                    });
                                                if (nthRow >= 0)
                                                {
                                                    nthResult = straightComparison(cruiserTreeLogs[nthRow].Grade, ctl.Grade);
                                                    updateLogChecks(td, ctl, nthResult, 2, cruiserTree.Initials, checkResults);
                                                }   //  endif
                                            }   //  end foreach loop
                                        }   //  endif

                                        break;
                                    }
                            }   //  end switch

                            //  update results except for log grade and log defect
                            //  already done in the seperate calling routine
                            if (ct.T_Element != "Log Grade" && ct.T_Element != "Log Defect %")
                                updateResults(td, ct.T_Element, nthResult, checkResults);
                        }   // endif on cruiserTree   
                    }   //  end foreach loop on check trees
                }
            }   //  end for each loop

            progress?.Report("Capturing volumes");

            LoadVolume(checkMeasureTrees, checkCalculatedTrees, originalCalculatedTrees, checkResults);

            CheckCruiseDataService.saveResults(checkResults);
            progress?.Report("Analysis complete");

        }

        private void LoadVolume(IReadOnlyCollection<TreeDO> checkTrees, List<TreeCalculatedValuesDO> checkCalculatedTrees,
                                List<TreeCalculatedValuesDO> originalCalculatedTrees, IReadOnlyList<ResultsList> checkResults)
        {
            //  load cubic and board volume into check results
            //  comparison is done when reports are generated
            foreach (TreeDO ct in checkTrees)
            {
                //  find tree_CN in check trees -- should be the same as in check calculated trees
                TreeCalculatedValuesDO checkTCV = checkCalculatedTrees.Find(x => x.Tree_CN == ct.Tree_CN);

                // if no checkTCV then that means cruise might not be processed, skip all trees
                if (checkTCV == null) break;

                //  then find row in checkResults to update
                //  correct plot if needed
                string plotNumber = (ct.Plot == null) ? "" : ct.Plot.PlotNumber.ToString();

                var checkResult = checkResults.FirstOrDefault(r => r.R_Stratum == ct.Stratum.Code
                && r.R_CuttingUnit == ct.CuttingUnit.Code
                && r.R_Plot == plotNumber
                && r.R_SampleGroup == ct.SampleGroup.Code
                && r.R_TreeNumber == ct.TreeNumber.ToString());

                //  total volume is gross plus net primary and secondary (separately) for the species product recorded in check results
                //  primary and secondary product are stored separately for each volume type
                if (checkResult == null) { continue; }

                checkResult.R_TreeSpecies = ct.Species;
                checkResult.R_TreeProduct = ct.SampleGroup.PrimaryProduct;

                checkResult.R_CC_GrossCUFTPP = checkTCV.GrossCUFTPP;
                checkResult.R_CC_NetCUFTPP = checkTCV.NetCUFTPP;
                checkResult.R_CC_GrossBDFTPP = checkTCV.GrossBDFTPP;
                checkResult.R_CC_NetBDFTPP = checkTCV.NetBDFTPP;

                checkResult.R_CC_GrossCUFTSP = checkTCV.GrossCUFTSP;
                checkResult.R_CC_NetCUFTSP = checkTCV.NetCUFTSP;
                checkResult.R_CC_GrossBDFTSP = checkTCV.GrossBDFTSP;
                checkResult.R_CC_NetBDFTSP = checkTCV.NetBDFTSP;

                //  add primary and secondary together for total volume fields
                checkResult.R_CC_GrossCUFTPSP = checkTCV.GrossCUFTPP + checkTCV.GrossCUFTSP;
                checkResult.R_CC_NetCUFTPSP = checkTCV.NetCUFTPP + checkTCV.NetCUFTSP;
                checkResult.R_CC_GrossBDFTPSP = checkTCV.GrossBDFTPP + checkTCV.GrossBDFTSP;
                checkResult.R_CC_NetBDFTPSP = checkTCV.NetBDFTPP + checkTCV.NetBDFTSP;


                //  now need original tree data
                TreeCalculatedValuesDO origTCV = originalCalculatedTrees.Find(x => x.Tree_CN == ct.Tree_CN);
                if (origTCV != null)
                {
                    checkResult.R_GrossCUFTPP = origTCV.GrossCUFTPP;
                    checkResult.R_NetCUFTPP = origTCV.NetCUFTPP;
                    checkResult.R_GrossBDFTPP = origTCV.GrossBDFTPP;
                    checkResult.R_NetBDFTPP = origTCV.NetBDFTPP;

                    checkResult.R_GrossCUFTSP = origTCV.GrossCUFTSP;
                    checkResult.R_NetCUFTSP = origTCV.NetCUFTSP;
                    checkResult.R_GrossBDFTSP = origTCV.GrossBDFTSP;
                    checkResult.R_NetBDFTSP = origTCV.NetBDFTSP;

                    checkResult.R_GrossCUFTPSP = origTCV.GrossCUFTPP + origTCV.GrossCUFTSP;
                    checkResult.R_NetCUFTPSP = origTCV.NetCUFTPP + origTCV.NetCUFTSP;
                    checkResult.R_GrossBDFTPSP = origTCV.GrossBDFTPP + origTCV.GrossBDFTSP;
                    checkResult.R_NetBDFTPSP = origTCV.NetBDFTPP + origTCV.NetBDFTSP;

                    //  add check cruiser inititals to results table
                    checkResult.R_MarkerInitials = origTCV.Tree.Initials;
                }

                //  and the include volume flag so volume differences are calculated
                //checkResult.R_IncludeVol = (includeVolumes) ? 1 : 0;

            }
        }

        private void CheckInOutTrees(TolerancesList currTolerance, List<TreeDO> checkTrees, List<TreeDO> cruiserTrees, string checkCruiserInitials, IList<ResultsList> checkResults, IEnumerable<TreeDO> justStrataUnits)
        {
            //  NOTE -- tree-based methods are not evaluated for in/out trees since those trees usually fall into
            //  plot-based methods such as those checked for below.  the new system registers a tree count of 1 for each tree
            //  so the check on tree count is removed and simplifies this whole process.
            int nthRow;
            foreach (TreeDO td in checkTrees)
            {
                switch (td.Stratum.Method)
                {

                    case "FIX":
                    case "PNT":
                    case "3PPNT":
                    case "FIXCNT":
                    case "P3P":
                    case "F3P":
                    case "PCM":
                    case "FCM":
                        nthRow = cruiserTrees.FindIndex(
                            delegate (TreeDO t)
                            {
                                return t.Tree_CN == td.Tree_CN;
                            });
                        if (nthRow < 0)
                            updateResults(td, "in", PassFail.Fail, checkResults);
                        break;
                }   //  end switch on method
            }   //  end foreach loop

            //  then check original data against check cruise data for offsetting numbering using cutting unit and plot
            //  need distinct stratum and cutting unit otherwise get a lot of out trees for units not checked
            //  according to Craig B. need to include count records in the check
            string tempPlot;
            foreach (TreeDO jsu in justStrataUnits)
            {
                List<TreeDO> justUnits = cruiserTrees.FindAll(
                    delegate (TreeDO t)
                    {
                        return t.Stratum_CN == jsu.Stratum_CN && t.CuttingUnit_CN == jsu.CuttingUnit_CN;
                    });
                foreach (TreeDO ju in justUnits)
                {
                    switch (ju.Stratum.Method)
                    {
                        case "FIX":
                        case "PNT":
                        case "3PPNT":
                        case "FIXCNT":
                        case "P3P":
                        case "F3P":
                        case "PCM":
                        case "FCM":
                            nthRow = checkTrees.FindIndex(
                                delegate (TreeDO t)
                                {
                                    if (t.Plot == null)
                                        tempPlot = "";
                                    else tempPlot = t.Plot.PlotNumber.ToString();
                                    //  seems like the check cruiser initials aren't needed for this check but comment out the code
                                    //  in case this comes up later
                                    return t.Stratum.Code == ju.Stratum.Code && t.CuttingUnit.Code == ju.CuttingUnit.Code &&
                                           tempPlot == ju.Plot.PlotNumber.ToString() && t.Tree_CN == ju.Tree_CN;
                                });
                            if (nthRow < 0)
                                updateResults(ju, "out", PassFail.Fail, checkResults);
                            break;
                    }   //  end switch
                }   //  end foreach loop
            }   //  end for unique  unit
            return;
        }

        private static void checkR10logs(List<LogStockDO> checkTreeLogs, List<LogStockDO> cruiserTreeLogs,
                        TreeDO td, TolerancesList ct, string cruiserInitials, IList<ResultsList> checkResults)
        {
            if (ct.T_Element == "Log Grade")
            {
                //  check first log for exact match
                if (checkTreeLogs[0].Grade != cruiserTreeLogs[0].Grade)
                    updateLogChecks(td, checkTreeLogs[0], PassFail.Fail, 1, cruiserInitials, checkResults);

                //  continue checking rest of logs
                for (int k = 1; k < cruiserTreeLogs.Count; k++)
                {
                    //  convert to a number for comparison
                    int OC_gradeNumber = Convert.ToInt16(cruiserTreeLogs[k].Grade);
                    //  find log in check cruiser logs
                    int nthRow = checkTreeLogs.FindIndex(
                        delegate (LogStockDO ls)
                        {
                            return ls.LogNumber == cruiserTreeLogs[k].LogNumber;
                        });
                    if (nthRow >= 0)
                    {
                        //  convert to a number
                        int CC_gradeNumber = Convert.ToInt16(checkTreeLogs[nthRow].Grade);
                        if (CC_gradeNumber - OC_gradeNumber > 1)
                            updateLogChecks(td, checkTreeLogs[nthRow], PassFail.Fail, 1, cruiserInitials, checkResults);
                        else
                            updateLogChecks(td, checkTreeLogs[nthRow], PassFail.Pass, 1, cruiserInitials, checkResults);
                    }   //  endif
                }   //  end for k loop
            }
            else if (ct.T_Element == "Log Defect %")
            {
                //  check all logs for defect difference
                foreach (LogStockDO ctl in checkTreeLogs)
                {
                    float itemTolerance = (float)Convert.ToDouble(ct.T_Tolerance);
                    itemTolerance = ctl.SeenDefect * itemTolerance / 100;

                    //  find corresponding log in cruiser logs
                    int nthRow = cruiserTreeLogs.FindIndex(
                        delegate (LogStockDO ls)
                        {
                            return ls.LogNumber == ctl.LogNumber;
                        });
                    int itemDifference = Convert.ToInt16(ctl.SeenDefect - cruiserTreeLogs[nthRow].SeenDefect);
                    if (itemDifference >= itemTolerance)
                    { updateLogChecks(td, ctl, PassFail.Fail, 2, cruiserInitials, checkResults); }
                    else
                    { updateLogChecks(td, ctl, PassFail.Pass, 2, cruiserInitials, checkResults); }
                }   //  end foreach loop

            }   //  endif
            return;
        }   //  end checkR10logs


        private static void checkR6logs(List<LogStockDO> checkTreeLogs, List<LogStockDO> cruiserTreeLogs,
                                TreeDO td, TolerancesList ct, string cruiserInitials, IList<ResultsList> checkResults)
        {
            PassFail nthResult = 0;
            foreach (LogStockDO ctl in checkTreeLogs)
            {
                //  find corresponding log in cruiser logs
                int nthRow = cruiserTreeLogs.FindIndex(
                    delegate (LogStockDO ls)
                    {
                        return ls.LogNumber == ctl.LogNumber;
                    });

                if (nthRow >= 0)
                {
                    if (cruiserTreeLogs[nthRow].SeenDefect > 0 && ctl.SeenDefect > 0)
                    {
                        //  tolerance changes for this condition
                        nthResult = plusOrMinus(cruiserTreeLogs[nthRow].SeenDefect, ctl.SeenDefect, "15.0", ct.T_Units);
                    }
                    else nthResult = plusOrMinus(cruiserTreeLogs[nthRow].SeenDefect, ctl.SeenDefect, ct.T_Tolerance, ct.T_Units);

                    updateLogChecks(td, ctl, nthResult, 2, cruiserInitials, checkResults);
                }   //  endif nthRow
            }   //  end foreach loop
            return;
        }   //  end checkR6logs


        private static void updateLogChecks(TreeDO td, LogStockDO currLog, PassFail nthResult, int whichType, string cruiserInitials, IList<ResultsList> checkResults)
        {
            //  find tree in results table
            //  if it doesn't exist, add it
            string currPlot;
            if (td.Plot == null)
                currPlot = "";
            else currPlot = td.Plot.PlotNumber.ToString();

            var checkResult = checkResults.FirstOrDefault(rr => rr.R_Stratum == td.Stratum.Code && rr.R_CuttingUnit == td.CuttingUnit.Code &&
                            rr.R_Plot == currPlot && rr.R_SampleGroup == td.SampleGroup.Code &&
                            rr.R_TreeNumber == td.TreeNumber.ToString() &&
                            rr.R_LogNumber == currLog.LogNumber);

            if (checkResult != null)
            {
                checkResult.R_LogNumber = currLog.LogNumber;
                if (whichType == 1)
                {
                    //  update log grade
                    checkResult.R_CC_LogGrade = currLog.Grade;
                    checkResult.R_LogGrade_R = (int)nthResult;
                    //  need cruiser initials for reports
                    checkResult.R_MarkerInitials = cruiserInitials;
                }
                else if (whichType == 2)
                {
                    //  update log defect
                    checkResult.R_CC_LogSeenDef = currLog.SeenDefect;
                    checkResult.R_LogSeenDef_R = (int)nthResult;
                    //  ditto above on initials
                    checkResult.R_MarkerInitials = cruiserInitials;
                }   //  endif
            }
            else
            {
                ResultsList rl = new ResultsList();
                rl.R_Stratum = td.Stratum.Code;
                rl.R_CuttingUnit = td.CuttingUnit.Code.ToString();
                if (td.Plot != null)
                    rl.R_Plot = td.Plot.PlotNumber.ToString();
                else rl.R_Plot = "";
                rl.R_SampleGroup = td.SampleGroup.Code;
                rl.R_TreeNumber = td.TreeNumber.ToString();
                rl.R_LogNumber = currLog.LogNumber;
                rl.R_MarkerInitials = cruiserInitials;
                if (whichType == 1)
                {
                    rl.R_CC_LogGrade = currLog.Grade;
                    rl.R_LogGrade_R = (int)nthResult;
                }
                else if (whichType == 2)
                {
                    rl.R_CC_LogSeenDef = currLog.SeenDefect;
                    rl.R_LogSeenDef_R = (int)nthResult;
                }   //  endif
                checkResults.Add(rl);
            }   //  endif
            return;
        }

        private static PassFail plusOrMinus(float originalItem, float checkItem, string itemTolerance, string itemUnits)
        {
            //  should work for everything except seen defect % primary for Region 5
            //  Merch Height Primary, Merch Height Secondary
            //  Height to First Live Limb, Top FIB Primary, Recoverable %
            //  and Seen Defect % Secondary
            //  Log defect %
            float tolerancePlus = 0;
            float toleranceMinus = 0;

            //  convert tolerance to int
            int convertedTolerance = (int)Convert.ToDouble(itemTolerance);
            if (itemUnits == TolerancesList.UNIT_PERCENT && checkItem > 0.0)
            {
                tolerancePlus = checkItem + (checkItem * (convertedTolerance / 100));
                toleranceMinus = checkItem - (checkItem * (convertedTolerance / 100));
            }
            else if (checkItem == 0.0)
            {
                tolerancePlus = convertedTolerance;
                toleranceMinus = 0;
            }   //  endif

            float itemDifference = checkItem - originalItem;
            if (itemUnits == TolerancesList.UNIT_PERCENT)
            {
                if (originalItem > tolerancePlus || originalItem < toleranceMinus)
                    return PassFail.Fail;
            }
            else if (Math.Abs(itemDifference) > convertedTolerance)
                return PassFail.Fail;

            return PassFail.Pass;
        }   //  end plusOrMinus


        //  overloaded plus or minus for Region 5 seen defect comparison
        private static PassFail plusOrMinus(string itemTolerance, TreeCalculatedValuesDO cruiserTree, TreeCalculatedValuesDO checkTree)
        {
            //  calculates seen defect for Region 5 for comparison
            float checkItem = 0;
            float originalItem = 0;

            //  check item
            if (checkTree.GrossCUFTPP > 0)
                checkItem = ((checkTree.GrossCUFTPP - checkTree.NetCUFTPP) / checkTree.GrossCUFTPP) * 100;
            if (cruiserTree.GrossCUFTPP > 0)
                originalItem = ((cruiserTree.GrossCUFTPP - cruiserTree.NetCUFTPP) / cruiserTree.GrossCUFTPP) * 100;

            // TODO not really sure what is going on here is this value getting saved or retrieved elsewhere?
            // 
            //  need to store the item result for the summary report
            //  but original will need to be calculated when the summary report is created
            checkTree.Tree.SeenDefectPrimary = checkItem;

            int convertedTolerance = (int)Convert.ToDecimal(itemTolerance);
            float tolerPlus = checkItem + convertedTolerance;
            float tolerMinus = checkItem - convertedTolerance;
            if (Math.Abs(originalItem) > tolerPlus || Math.Abs(originalItem) < tolerMinus)
                return PassFail.Fail;

            return PassFail.Pass;
        }

        private static PassFail DBHanalysis(TreeDO checkTree, TreeDO cruiserTree, TolerancesList currentTolerance)
        {
            //  DBH analysis
            float modifiedTolerance = 0;
            float tolerancePlus = 0;
            float toleranceMinus = 0;

            //  convert item tolerance from string to float
            float itemTolerance = (float)Convert.ToDouble(currentTolerance.T_Tolerance);

            float itemDifference = checkTree.DBH - cruiserTree.DBH;
            //  need to drop itemDifference to exactly one decimal place
            itemDifference = (float)Math.Round(itemDifference, 1);
            switch (currentTolerance.T_AddParam)
            {
                case "7% whichever >":
                    modifiedTolerance = (float)0.07 * checkTree.DBH;
                    if (modifiedTolerance > itemTolerance)
                        itemTolerance = modifiedTolerance;
                    //  itemTolerance = checkTree.DBH + modifiedTolerance;
                    break;
                case "3% whichever >":
                    modifiedTolerance = (float)0.03 * checkTree.DBH;
                    if (modifiedTolerance > itemTolerance)
                        itemTolerance = modifiedTolerance;
                    //  itemTolerance = checkTree.DBH + modifiedTolerance;  
                    break;
            }   //  end switch

            switch (currentTolerance.T_Units)
            {
                case TolerancesList.UNIT_INCHES:
                    if (Math.Abs(itemDifference) > itemTolerance)
                        return PassFail.Fail;
                    break;
                case TolerancesList.UNIT_PERCENT:
                    tolerancePlus = checkTree.DBH + (checkTree.DBH * (itemTolerance / 100));
                    toleranceMinus = checkTree.DBH - (checkTree.DBH * (itemTolerance / 100));
                    if (cruiserTree.DBH > tolerancePlus || cruiserTree.DBH < toleranceMinus)
                        return PassFail.Fail;
                    break;
            }   //  end switch

            return PassFail.Pass;
        }


        private static PassFail totalHeightAnalysis(TreeDO checkTree, TreeDO cruiserTree, TolerancesList currentTolerance)
        {
            //  total height conditions checked
            float itemDifference = 0;

            //  convert item tolerance from string to float
            float itemTolerance = (float)Convert.ToDouble(currentTolerance.T_Tolerance);


            switch (currentTolerance.T_Element)
            {
                case "Total Height <= 100 feet":
                    if (checkTree.TotalHeight <= 100)
                    {
                        if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_NONE)
                        {
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > itemTolerance)
                                return PassFail.Fail;
                        }
                        else if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_7PCT)
                        {
                            //  modify tolerance accordingly
                            double modifiedTolerance = 0.07 * checkTree.TotalHeight;
                            if (modifiedTolerance > itemTolerance) itemTolerance = (float)modifiedTolerance;
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > itemTolerance)
                                return PassFail.Fail;
                        }   //  endif
                    }   //  endif
                    break;
                case "Total Height > 100 feet":
                    if (checkTree.TotalHeight > 100)
                    {
                        if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_NONE)
                        {
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > itemTolerance)
                                return PassFail.Fail;
                        }
                        else if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_PLUS1PER25FTOVER100)
                        {
                            float modifiedTolerance = ((checkTree.TotalHeight - 100) / 25) * 1 + itemTolerance;
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > modifiedTolerance)
                                return PassFail.Fail;
                        }   //  endif
                    }   //  endif
                    break;
                case "Total Height":
                    itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                    if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_NONE)
                    {
                        if (currentTolerance.T_Units == TolerancesList.UNIT_FEET)
                        {
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > itemTolerance)
                                return PassFail.Fail;
                        }
                        else if (currentTolerance.T_Units == TolerancesList.UNIT_PERCENT)
                        {
                            itemTolerance = itemTolerance / 100 * checkTree.TotalHeight;
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > itemTolerance)
                                return PassFail.Fail;
                        }   //  endif
                    }
                    else if (currentTolerance.T_AddParam == TolerancesList.ADD_PARAM_PLUS1PER25FTOVER100)
                    {
                        if (checkTree.TotalHeight > 100)
                        {
                            float modifiedTolerance = ((checkTree.TotalHeight - 100) / 25) * 1 + itemTolerance;
                            itemDifference = checkTree.TotalHeight - cruiserTree.TotalHeight;
                            if (Math.Abs(itemDifference) > modifiedTolerance)
                                return PassFail.Fail;
                        }   // endif tree height over 100
                    }   //  endif
                    break;
            }   //  end switch
            return PassFail.Pass;
        }

        private static PassFail straightComparison(string originalItem, string checkItem)
        {
            //  this comparison will work for species, product, live/dead
            //  tree grade, log grade (except Region 10),  clear face, 
            return (originalItem != checkItem) ?
                 PassFail.Fail : PassFail.Pass;
        }

        private PassFail straightComparison(float originalItem, float checkItem)
        {
            //  compares two floats
            //  used for form class
            return (originalItem != checkItem) ?
                 PassFail.Fail : PassFail.Pass;
        }   //  end straightComparison

        private void updateResults(TreeDO checkTree, string elementChecked, PassFail iResult, IList<ResultsList> checkResults)
        {
            //  should work for every element except volume
            //  first find tree in results table
            //  check for null plot and change to empty
            string plotNumber = (checkTree.Plot == null) ? "" : checkTree.Plot.PlotNumber.ToString();

            var checkResult = checkResults.FirstOrDefault(r =>
                r.R_Stratum == checkTree.Stratum.Code
                && r.R_CuttingUnit == checkTree.CuttingUnit.Code
                && r.R_Plot == plotNumber
                && r.R_SampleGroup == checkTree.SampleGroup.Code
                && r.R_TreeNumber == checkTree.TreeNumber.ToString());

            if (checkResult != null)
            {
                //  update element checked
                switch (elementChecked)
                {
                    case "Species":
                        checkResult.R_CC_Species = checkTree.Species;
                        checkResult.R_Species_R = (int)iResult;
                        break;
                    case "Live/Dead":
                        checkResult.R_CC_LiveDead = checkTree.LiveDead;
                        checkResult.R_LiveDead_R = (int)iResult;
                        break;
                    case "Product":
                        checkResult.R_CC_Product = checkTree.SampleGroup.PrimaryProduct;
                        checkResult.R_Product_R = (int)iResult;
                        break;
                    case "DBH":
                        checkResult.R_CC_DBHOB = checkTree.DBH;
                        checkResult.R_DBHOB_R = (int)iResult;
                        break;
                    case "Total Height <= 100 feet":
                        checkResult.R_CC_TotHeightUnder = checkTree.TotalHeight;
                        checkResult.R_TotHeightUnder_R = (int)iResult;
                        break;
                    case "Total Height > 100 feet":
                        checkResult.R_CC_TotHeightOver = checkTree.TotalHeight;
                        checkResult.R_TotHeightOver_R = (int)iResult;
                        break;
                    case "Total Height":
                        checkResult.R_CC_TotalHeight = checkTree.TotalHeight;
                        checkResult.R_TotalHeight_R = (int)iResult;
                        break;
                    case "Merch Height Primary":
                        checkResult.R_CC_MerchHgtPP = checkTree.MerchHeightPrimary;
                        checkResult.R_MerchHgtPP_R = (int)iResult;
                        break;
                    case "Merch Height Secondary":
                        checkResult.R_CC_MerchHgtSP = checkTree.MerchHeightSecondary;
                        checkResult.R_MerchHgtSP_R = (int)iResult;
                        break;
                    case "Height to First Live Limb":
                        checkResult.R_CC_HgtFLL = checkTree.HeightToFirstLiveLimb;
                        checkResult.R_HgtFLL_R = (int)iResult;
                        break;
                    case "Top DIB Primary":
                        checkResult.R_CC_TopDIBPP = checkTree.TopDIBPrimary;
                        checkResult.R_TopDIBPP_R = (int)iResult;
                        break;
                    case "Seen Defect % Primary":
                        checkResult.R_CC_SeenDefPP = checkTree.SeenDefectPrimary;
                        checkResult.R_SeenDefPP_R = (int)iResult;
                        break;
                    case "Seen Defect % Secondary":
                        checkResult.R_CC_SeenDefSP = checkTree.SeenDefectSecondary;
                        checkResult.R_SeenDefSP_R = (int)iResult;
                        break;
                    case "Recoverable %":
                        checkResult.R_CC_RecDef = checkTree.RecoverablePrimary;
                        checkResult.R_RecDef_R = (int)iResult;
                        break;
                    case "Form class":
                        checkResult.R_CC_FormClass = (int)checkTree.FormClass;
                        checkResult.R_FormClass_R = (int)iResult;
                        break;
                    case "Clear Face":
                        checkResult.R_CC_Clear = checkTree.ClearFace;
                        checkResult.R_Clear_R = (int)iResult;
                        break;
                    case "Tree Grade":
                        checkResult.R_CC_TreeGrade = checkTree.Grade;
                        checkResult.R_TreeGrade_R = (int)iResult;
                        break;
                    case "in":
                        checkResult.R_InResult = (int)iResult;
                        checkResult.R_TreeSpecies = checkTree.Species;
                        checkResult.R_TreeProduct = checkTree.SampleGroup.PrimaryProduct;
                        break;
                    case "out":
                        checkResult.R_OutResult = (int)iResult;
                        checkResult.R_CountMeasure = checkTree.CountOrMeasure;
                        checkResult.R_TreeSpecies = checkTree.Species;
                        checkResult.R_TreeProduct = checkTree.SampleGroup.PrimaryProduct;
                        break;
                }   //  end switch on element checked
            }
            else
            {
                //  this probably only works for in/out trees
                //  when cruiser calls a tree in but check cruiser calls it out
                //  means the tree is not in the results table
                ResultsList rl = new ResultsList();
                rl.R_Stratum = checkTree.Stratum.Code;
                rl.R_CuttingUnit = checkTree.CuttingUnit.Code;
                rl.R_Plot = plotNumber;
                rl.R_SampleGroup = checkTree.SampleGroup.Code;
                rl.R_TreeNumber = checkTree.TreeNumber.ToString();
                rl.R_CountMeasure = checkTree.CountOrMeasure;
                rl.R_TreeSpecies = checkTree.Species;
                rl.R_TreeProduct = checkTree.SampleGroup.PrimaryProduct;
                rl.R_LogNumber = "0";
                switch (elementChecked)
                {
                    case "in":
                        rl.R_InResult = (int)iResult;
                        break;
                    case "out":
                        rl.R_OutResult = (int)iResult;
                        break;
                }   //  end switch
                checkResults.Add(rl);
            }   //  endif on nthRow
            return;
        }

        private List<ResultsList> fillResultsTable(IEnumerable<TreeDO> checkTrees)
        {
            var checkResults = new List<ResultsList>();
            foreach (TreeDO ct in checkTrees)
            {
                ResultsList rl = new ResultsList();
                rl.R_Stratum = ct.Stratum.Code;
                rl.R_CuttingUnit = ct.CuttingUnit.Code;
                if (ct.Plot != null)
                    rl.R_Plot = ct.Plot.PlotNumber.ToString();
                else rl.R_Plot = "";
                rl.R_SampleGroup = ct.SampleGroup.Code;
                rl.R_CountMeasure = ct.CountOrMeasure;
                rl.R_TreeNumber = ct.TreeNumber.ToString();
                rl.R_LogNumber = "";
                rl.R_CC_Species = "";
                rl.R_CC_LiveDead = "";
                rl.R_CC_Product = "";
                rl.R_CC_Clear = "";
                rl.R_CC_TreeGrade = "";
                rl.R_CC_LogGrade = "";
                rl.R_TreeSpecies = "";
                rl.R_TreeProduct = "";
                rl.R_MarkerInitials = "";

                checkResults.Add(rl);
            }   //  end foreach loop
            return checkResults;
        }   //  end fillResultsTable
    }
}
