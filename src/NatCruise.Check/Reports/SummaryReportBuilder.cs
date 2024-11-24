using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckCruise.Reports
{
    public class SummaryReportBuilder
    {
        public static readonly string[] TREE_BASED_CRUISE_METHODS = new[] { "100", "STR", "S3P", "3P" };
        private const string COLUMN_SEPARATOR = "     |";

        public class TextReportHeaders
        {
            public string[] TextHeader = new string[3] {"                                                  NATIONAL CHECK CRUISE PROGRAM",
                                                    "                                                       VERSION: XXXX",
                                                    "                                              RUN DATE & TIME: XXXX"};

            public string[] singleSaleHeader = new string[7] {"SUMMARY REPORT BY CRUISER",
                                                           "SINGLE SALE",
                                                           "SALENAME:  XXXX",
                                                           "CRUISE FILE:  XXXX",
                                                           "CHECK CRUISE FILE:  XXXX",
                                                           "NUMBER OF ELEMENTS CHECKED:  XX",
                                                           "TOLERANCES DATED:  XXXX"};

            public string[] multipleSaleHeader = new string[5] {"SUMMARY REPORT BY CRUISER",
                                                            "MULTIPLE SALES",
                                                            "SALENAMES                  CRUISE FILES                                                  CHECK CRUISE FILES",
                                                            "NUMBER OF ELEMENTS CHECKED:  XX",
                                                            "TOLERANCES DATED:  XXXX"};

            public string[] saleSummaryHeader = new string[6] {"SUMMARY REPORT BY SALE",
                                                           "SALENAME:  XXXX",
                                                           "CRUISE FILE:  XXXX",
                                                           "CHECK CRUISE FILE:  XXXX",
                                                           "NUMBER OF ELEMENTS CHECKED:  XX",
                                                           "TOLERANCES DATED:  XXXX"};

            public string[] columnHeadings = new string[3] {"ELEMENT:  XXX",
                                                        "     CUTTING                                      CRUISER    *********** DIFFERENCE **********    OUT OF",
                                                        "     UNIT     TREE   C/M   ORIGINAL    CHECK      INITIALS    ACTUAL       # DIFF      % DIFF    TOLERANCE"};

            public string[] summaryColumnHeaders = new string[3] {"ELEMENT: XXX ",
                                                              "           CUTTING                                                CRUISER     ********* DIFFERENCE *********     OUT OF",
                                                              " STRATUM   UNIT     PLOT    TREE   C/M     ORIGINAL    CHECK      INITIALS    ACTUAL      # DIFF      % DIFF     TOLERANCE"};
            public string[] summaryLogElementHeaders = new string[3] {"ELEMENT: XXX ",
                                                              "               CUTTING                                                   CRUISER    ********* DIFFERENCE *********     OUT OF",
                                                              "     STRATUM   UNIT     PLOT    TREE   C/M  LOG   ORIGINAL    CHECK      INITIALS   ACTUAL      # DIFF      % DIFF     TOLERANCE"};
        }   //  end TextReportHeaders


        public dataBaseCommands CheckCruiseDataService { get; }
        public dataBaseCommands CruiseDataService { get; }

        public string SaleName { get; }
        public string Region { get; }

        public List<ResultsList> CheckResults { get; }
        public List<TolerancesList> CruiseTolerances { get; }
        public List<LogStockDO> CheckLogs { get; }

        public List<TreeDO> CruiserTrees { get; }
        private List<TreeCalculatedValuesDO> CalculatedTrees { get; }
        public string MarkersInitials { get; }
        private int ReportToOutput { get; }
        private TextReportHeaders TRH = new TextReportHeaders();
        private int[] DefaultFieldLengths { get; }
        //private List<string> prtArray = new List<string>();

        public SummaryReportBuilder(dataBaseCommands checkCruiseDataService, dataBaseCommands cruiseDataService, string markersInitials, int reportToOutput, string applicationVersion)
        {
            CheckCruiseDataService = checkCruiseDataService;
            CruiseDataService = cruiseDataService;
            ReportToOutput = reportToOutput;
            MarkersInitials = markersInitials;

            TRH.TextHeader[1] = TRH.TextHeader[1].Replace("XXXX", applicationVersion);
            TRH.TextHeader[2] = TRH.TextHeader[2].Replace("XXXX", DateTime.Now.ToString());

            var checkCruiseFile = checkCruiseDataService.DAL.Path;
            var cruiserFile = cruiseDataService.DAL.Path;

            CheckResults = checkCruiseDataService.getResultsTable("", markersInitials);
            CruiseTolerances = checkCruiseDataService.getTolerances();

            CheckLogs = cruiseDataService.getLogStock();
            CruiserTrees = cruiseDataService.getTrees();
            CalculatedTrees = cruiseDataService.getCalculatedTrees();
            SaleName = cruiseDataService.getSaleName();
            Region = cruiseDataService.getRegion();

            switch (reportToOutput)
            {
                case 1:
                    {

                        TRH.saleSummaryHeader[1] = TRH.saleSummaryHeader[1].Replace("XXXX", SaleName);
                        TRH.saleSummaryHeader[2] = TRH.saleSummaryHeader[2].Replace("XXXX", cruiserFile);
                        TRH.saleSummaryHeader[3] = TRH.saleSummaryHeader[3].Replace("XXXX", checkCruiseFile);
                        TRH.saleSummaryHeader[4] = TRH.saleSummaryHeader[4].Replace("XX", CruiseTolerances.Count.ToString());
                        TRH.saleSummaryHeader[5] = TRH.saleSummaryHeader[5].Replace("XXXX", CruiseTolerances[0].T_DateStamp);

                        //  fill fieldLengths
                        DefaultFieldLengths = new int[17] { 1, 4, 9, 9, 8, 6, 14, 12, 9, 5, 6, 3, 6, 3, 6, 3, 6 };
                        break;
                    }
                case 2:
                    {
                        TRH.singleSaleHeader[2] = TRH.singleSaleHeader[2].Replace("XXXX", SaleName);
                        TRH.singleSaleHeader[3] = TRH.singleSaleHeader[3].Replace("XXXX", cruiserFile);
                        TRH.singleSaleHeader[4] = TRH.singleSaleHeader[4].Replace("XXXX", checkCruiseFile);
                        TRH.singleSaleHeader[5] = TRH.singleSaleHeader[5].Replace("XX", CruiseTolerances.Count.ToString());
                        TRH.singleSaleHeader[6] = TRH.singleSaleHeader[6].Replace("XXXX", CruiseTolerances[0].T_DateStamp);

                        //  fill fieldLengths
                        DefaultFieldLengths = new int[15] { 1, 8, 8, 6, 11, 10, 9, 8, 6, 3, 6, 3, 6, 3, 6 };
                        break;
                    }
                    //case 3:
                    //    //  output filename
                    //    outputFileName = checkCruiseFile.Replace("_CC.cruise", "_CruiserMultipleSummary.txt");

                    //    TRH.multipleSaleHeader[3] = TRH.multipleSaleHeader[3].Replace("XX", cruiseTolerances.Count.ToString());
                    //    TRH.multipleSaleHeader[4] = TRH.multipleSaleHeader[4].Replace("XXXX", cruiseTolerances[0].T_DateStamp);

                    //    //  fill fieldLengths
                    //    DefaultFieldLengths = new int[14] { 1, 8, 10, 10, 11, 9, 5, 6, 3, 6, 3, 6, 3, 6 };
                    //    break;
            }   //  end switch on reportToOutput


        }

        public void GenerateSummaryReport(string outputFileName)
        {
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                BuildSummaryReport(writer);
            }
        }




        public void BuildSummaryReport(TextWriter writer)
        {
            var reportToOutput = ReportToOutput;
            var markInits = MarkersInitials;

            //checkResults = CheckCruiseDataService.getResultsTable("", markInits);
            //cruiseTolerances = CheckCruiseDataService.getTolerances();
            var hasAllTreeBasedStrata = CheckHasAllTreeBasedStrata(CheckCruiseDataService);


            //currSaleName = CruiseDataService.getSaleName();
            //currRegion = CruiseDataService.getRegion();
            //cruiserTrees = CruiseDataService.getTrees();
            //calculatedTrees = CruiseDataService.getCalculatedTrees();

            int numOlines = 0;

            foreach (TolerancesList tl in CruiseTolerances)
            {
                if (!hasAllTreeBasedStrata || tl.T_Element != "In/Out Trees")
                {
                    //  write report heading and column headers for each new element
                    WriteReportHeading(writer, ref numOlines);
                    //  then write column headigns
                    WriteColumnHeaders(writer, tl.T_Element, ref numOlines);
                    //  print records for current element
                    PrintElement(writer, tl, ref numOlines);
                    numOlines = 0;
                }   //  endif
            }   //  end foreach

            //  print volume whether or not include volume was selected
            foreach (TolerancesList ct in CruiseTolerances)
            {
                if (ct.T_CUFTPGtolerance > 0 || ct.T_CUFTPGtolerance == -1)
                {
                    //  create output for both gross and net primary product volume
                    //  gross cuft
                    PrintVolume(1, writer, CheckResults, ct.T_CUFTPGtolerance, markInits, reportToOutput);
                    //  net cuft
                    PrintVolume(2, writer, CheckResults, ct.T_CUFTPNtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_CUFTSGtolerance > 0 || ct.T_CUFTSGtolerance == -1)
                {
                    //  create output for both gross and net secondary product volume
                    //  gross cuft
                    PrintVolume(3, writer, CheckResults, ct.T_CUFTSGtolerance, markInits, reportToOutput);
                    //  net cuft
                    PrintVolume(4, writer, CheckResults, ct.T_CUFTSNtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_BDFTPGtolerance > 0 || ct.T_BDFTPGtolerance == -1)
                {
                    //  create output for both gross and net primary product volume
                    //  gross BDFT
                    PrintVolume(5, writer, CheckResults, ct.T_BDFTPGtolerance, markInits, reportToOutput);
                    //  net BDFT
                    PrintVolume(6, writer, CheckResults, ct.T_BDFTPNtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_BDFTSGtolerance > 0 || ct.T_BDFTSGtolerance == -1)
                {
                    //  create output for both gross and net secondary product volume
                    //  gross BDFT
                    PrintVolume(7, writer, CheckResults, ct.T_BDFTSGtolerance, markInits, reportToOutput);
                    //  net BDFT
                    PrintVolume(8, writer, CheckResults, ct.T_BDFTSNtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_CUFTPSGtolerance > 0 || ct.T_CUFTPSGtolerance == -1)
                {
                    //  create output for just gross CUFT which includes primary and secondary
                    PrintVolume(9, writer, CheckResults, ct.T_CUFTPSGtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_CUFTPSNtolerance > 0 || ct.T_CUFTPSNtolerance == -1)
                {
                    //  and net CUFT including primary and secondary
                    PrintVolume(10, writer, CheckResults, ct.T_CUFTPSNtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_BDFTPSGtolerance > 0 || ct.T_BDFTPSGtolerance == -1)
                {
                    //  gross BDFT including primary and secondary
                    PrintVolume(11, writer, CheckResults, ct.T_BDFTPSGtolerance, markInits, reportToOutput);
                }   //  endif
                if (ct.T_BDFTPSNtolerance > 0 || ct.T_BDFTPSNtolerance == -1)
                {
                    // net BDFT including primary and secondary
                    PrintVolume(12, writer, CheckResults, ct.T_BDFTPSNtolerance, markInits, reportToOutput);
                }   //  endif
            }   //  end for each loop
        }


        private void WriteReportHeading(TextWriter strTextOut, ref int numOlines)
        {
            if (numOlines == 0 || numOlines >= 50)
            {
                //  page break
                strTextOut.WriteLine("\f");
                numOlines = 0;

                //  output main header
                for (int k = 0; k < 3; k++)
                {
                    strTextOut.WriteLine(TRH.TextHeader[k]);
                    numOlines++;
                }   //  end for k loop

                switch (ReportToOutput)
                {
                    case 1:
                        for (int k = 0; k < TRH.saleSummaryHeader.Count(); k++)
                        {
                            strTextOut.WriteLine(TRH.saleSummaryHeader[k]);
                            numOlines++;
                        }   //  end foreach
                        break;
                    case 2:
                        for (int k = 0; k < TRH.singleSaleHeader.Count(); k++)
                        {
                            strTextOut.WriteLine(TRH.singleSaleHeader[k]);
                            numOlines++;
                        }   //  end foreach
                        break;
                    case 3:
                        for (int k = 0; k < TRH.multipleSaleHeader.Count(); k++)
                        {
                            strTextOut.WriteLine(TRH.multipleSaleHeader[k]);
                            numOlines++;
                        }   //  end foreach
                        break;
                }   //  end switch
                    //  write two blank lines before headers
                strTextOut.WriteLine("");
                numOlines++;
                strTextOut.WriteLine("");
                numOlines++;
            }   //  endif
            return;
        }   //  end WriteReportHeading

        private void WriteColumnHeaders(TextWriter strTextOut, string currElement, ref int numOlines)
        {
            string holdHeading;
            //  log element headings
            if (currElement == "Log Grade" || currElement == "Log Defect %")
            {
                holdHeading = TRH.summaryLogElementHeaders[0];
                TRH.summaryLogElementHeaders[0] = TRH.summaryLogElementHeaders[0].Replace("XXX", currElement);
                for (int k = 0; k < TRH.summaryLogElementHeaders.Count(); k++)
                {
                    strTextOut.WriteLine(TRH.summaryLogElementHeaders[k]);
                    numOlines++;
                }   //  end for k loop
            }
            else
            {
                switch (ReportToOutput)
                {
                    case 1:
                        holdHeading = TRH.summaryColumnHeaders[0];
                        TRH.summaryColumnHeaders[0] = TRH.summaryColumnHeaders[0].Replace("XXX", currElement);
                        for (int k = 0; k < TRH.summaryColumnHeaders.Count(); k++)
                        {
                            strTextOut.WriteLine(TRH.summaryColumnHeaders[k]);
                            numOlines++;
                        }   //  end for k loop
                            //  replace first line of heading for next page
                        TRH.summaryColumnHeaders[0] = holdHeading;
                        break;
                    case 2:
                    case 3:
                        holdHeading = TRH.columnHeadings[0];
                        TRH.columnHeadings[0] = TRH.columnHeadings[0].Replace("XXX", currElement);
                        for (int k = 0; k < TRH.columnHeadings.Count(); k++)
                        {
                            strTextOut.WriteLine(TRH.columnHeadings[k]);
                            numOlines++;
                        }   //  end for k loop
                            //  replace first line of heading for next page
                        TRH.columnHeadings[0] = holdHeading;
                        break;
                }   // end switch
            }   //  endif
                //  write long line or medium  line whatever you call it
            strTextOut.WriteLine("___________________________________________________________________________________________________________________________________");
            numOlines++;

            return;
        }   //  end WriteColumnHeaders

        private void printLine(TextWriter strTextOut, string currentElement, IEnumerable<string> prtArray, int[] fieldLengths, ref int numOlines)
        {
            //  new page needed?
            if (numOlines >= 50 || numOlines == 0)
            {
                WriteReportHeading(strTextOut, ref numOlines);
                WriteColumnHeaders(strTextOut, currentElement, ref numOlines);
            }   //  endif
            StringBuilder printLine = new StringBuilder();
            int k = 0;
            foreach (string str in prtArray)
            {
                printLine.Append(str.PadLeft(fieldLengths[k]));
                k++;
            }   //  end foreach loop
            strTextOut.WriteLine(printLine.ToString());
            numOlines++;
        }   //  end printLine


        private void PrintVolume(int whichVolume, TextWriter strTextOut, List<ResultsList> checkResults, double currTolerance, string markInits, int reportToOutput)
        {
            //  reset field lengths
            var fieldLengths = new int[17] { 1, 4, 9, 9, 8, 6, 14, 12, 9, 5, 6, 3, 6, 3, 6, 3, 6 };
            string currElement = "";
            var numOlines = 0;
            //  load values from checkResults for measured trees only
            List<ResultsList> currentResults = checkResults.FindAll(
                delegate (ResultsList rl)
                {
                    return rl.R_CountMeasure == "M";
                });

            foreach (ResultsList rl in currentResults)
            {
                //  clear print array
                var prtArray = new List<string>();
                prtArray.Add("");
                //  identifying data
                switch (reportToOutput)
                {
                    case 1:
                        prtArray.Add(rl.R_Stratum.PadLeft(4, ' '));
                        prtArray.Add(rl.R_CuttingUnit.PadLeft(9, ' '));
                        prtArray.Add(rl.R_Plot.PadLeft(9, ' '));
                        prtArray.Add(rl.R_TreeNumber.PadLeft(8, ' '));
                        prtArray.Add(rl.R_CountMeasure.PadLeft(6, ' '));
                        break;
                    case 2:
                    case 3:
                        prtArray.Add(rl.R_CuttingUnit.PadLeft(8, ' '));
                        prtArray.Add(rl.R_TreeNumber.PadLeft(10, ' '));
                        prtArray.Add(rl.R_CountMeasure.PadLeft(6, ' '));
                        break;
                }   //  end switch

                //  need cruiser initials for current tree
                if (rl.R_MarkerInitials != null)
                    markInits = rl.R_MarkerInitials;
                else markInits = " ";

                //  key to whichVolume
                //  1 = Gross CUFT Primary
                //  2 = Net CUFT Primary
                //  3 = Gross CUFT Secondary
                //  4 = Net CUFT Secondary
                //  5 = Gross BDFT Primary
                //  6 = Net BDFT Primary
                //  7 = Gross BDFT Secondary
                //  8 = Net BDFT Secondary
                //  9 = Gross CUFT including primary and secondary
                // 10 = Net CUFT including primary and secondary
                // 11 = Gross BDFT including primary and secondary
                // 12 = Net BDFT including primary and secondary

                ElementValues ev = null;

                switch (whichVolume)
                {
                    case 1:
                        //  Primary Gross CUFT
                        ev = LoadVolumeVariables(rl.R_GrossCUFTPP, rl.R_CC_GrossCUFTPP, currTolerance);
                        currElement = "Gross CUFT Primary";
                        break;
                    case 2:
                        //  Primary net CUFT
                        ev = LoadVolumeVariables(rl.R_NetCUFTPP, rl.R_CC_NetCUFTPP, currTolerance);
                        currElement = "Net CUFT Primary";
                        break;
                    case 3:
                        //  Secondary gross CUFT
                        ev = LoadVolumeVariables(rl.R_GrossCUFTSP, rl.R_CC_GrossCUFTSP, currTolerance);
                        currElement = "Gross CUFT Secondary";
                        break;
                    case 4:
                        //  Secondary net CUFT
                        ev = LoadVolumeVariables(rl.R_NetCUFTSP, rl.R_CC_NetCUFTSP, currTolerance);
                        currElement = "Net CUFT Secondary";
                        break;
                    case 5:
                        //  Primary Gross BDFT
                        ev = LoadVolumeVariables(rl.R_GrossBDFTPP, rl.R_CC_GrossBDFTPP, currTolerance);
                        currElement = "Gross BDFT Primary";
                        break;
                    case 6:
                        //  Primary net BDFT
                        ev = LoadVolumeVariables(rl.R_NetBDFTPP, rl.R_CC_NetBDFTPP, currTolerance);
                        currElement = "Net BDFT Primary";
                        break;
                    case 7:
                        //  Secondary gross BDFT
                        ev = LoadVolumeVariables(rl.R_GrossBDFTSP, rl.R_CC_GrossBDFTSP, currTolerance);
                        currElement = "Gross BDFT Secondary";
                        break;
                    case 8:
                        // Secondary net BDFT
                        ev = LoadVolumeVariables(rl.R_NetBDFTSP, rl.R_CC_NetBDFTSP, currTolerance);
                        currElement = "Net BDFT Secondary";
                        break;
                    case 9:
                        //  Gross CUFT which includes primary and secondary
                        ev = LoadVolumeVariables(rl.R_GrossCUFTPSP, rl.R_CC_GrossCUFTPSP, currTolerance);
                        currElement = "Gross CUFT Primary + Secondary";
                        break;
                    case 10:
                        //  Net CUFT including primary and secondary
                        ev = LoadVolumeVariables(rl.R_NetCUFTPSP, rl.R_CC_NetCUFTPSP, currTolerance);
                        currElement = "Net CUFT Primary + Secondary";
                        break;
                    case 11:
                        //  Gross BDFT including primary and secondary
                        ev = LoadVolumeVariables(rl.R_GrossBDFTPSP, rl.R_CC_GrossBDFTPSP, currTolerance);
                        currElement = "Gross BDFT Primary + Secondary";
                        break;
                    case 12:
                        //  Net BDFT including primary and secondary
                        ev = LoadVolumeVariables(rl.R_NetBDFTPSP, rl.R_CC_NetBDFTPSP, currTolerance);
                        currElement = "Net BDFT Primary + Secondary";
                        break;
                }   //  end switch

                //  load print array
                LoadPrintArray(prtArray, ev, markInits);
                printLine(strTextOut, currElement, prtArray, fieldLengths, ref numOlines);
            }   //  end foreach loop
            return;
        }   //  end PrintVolume

        private static ElementValues LoadVolumeVariables(double cruiserVolume, double checkCruiserVolume, double currTolerance)
        {
            var outOfTolerance = "na";
            string diff1 = null;
            //  calculate tolerance range
            if (currTolerance > 0)
            {
                var tolerPlus = checkCruiserVolume + (checkCruiserVolume * (currTolerance / 100));
                var tolerMinus = checkCruiserVolume - (checkCruiserVolume * (currTolerance / 100));
                var actualDiff = checkCruiserVolume - cruiserVolume;
                diff1 = Utilities.FormatField(actualDiff, "{0,3:F0}").ToString().PadLeft(3, ' ');
                if (Math.Abs(actualDiff) < tolerMinus && Math.Abs(actualDiff) > tolerPlus)
                    outOfTolerance = "1 **";
                else outOfTolerance = "0";
            }

            return new ElementValues
            {
                cruiserString = Utilities.FormatField(cruiserVolume, "{0,6:F0}").ToString().PadLeft(6, ' '),
                checkCruiserString = Utilities.FormatField(checkCruiserVolume, "{0,6:F0}").ToString().PadLeft(6, ' '),
                diff1 = diff1,
                // these two differences will always be blank
                diff2 = " ",
                diff3 = " ",
                outOfTolerance = outOfTolerance,
            };
        }   //  end LoadVolumeVariables

        private void PrintElement(TextWriter strTextOut, TolerancesList currElement, ref int numOlines)
        {
            int[] fieldLengths = DefaultFieldLengths;
            string markInits = MarkersInitials;

            //  Change to checkResults list added all record to the table
            //  So this is fine for In/out Trees and species but the rest of the elements
            //  need to be measured trees only
            //  need to pull measured trees for other elements
            //  and just log records for log elements
            List<ResultsList> currentResults = new List<ResultsList>();
            if (currElement.T_Element != "In/Out Trees" && currElement.T_Element != "Species" &&
                currElement.T_Element != "Log Grade" && currElement.T_Element != "Log Defect %")
                currentResults = CheckResults.FindAll(
                    delegate (ResultsList rl)
                    {
                        return rl.R_CountMeasure == "M";
                    });
            else if (currElement.T_Element == "Log Grade" || currElement.T_Element == "Log Defect %")
            {
                currentResults = CheckResults.FindAll(
                    delegate (ResultsList rl)
                    {
                        return rl.R_LogNumber != "0";
                    });
                //  reset fieldLengths                            
                fieldLengths = new int[18] { 1, 8, 9, 9, 8, 6, 5, 12, 11, 9, 5, 6, 3, 6, 3, 6, 3, 6 };
            }
            else if (currElement.T_Element == "In/Out Trees" || currElement.T_Element == "Species")
                currentResults = CheckResults.FindAll(
                    delegate (ResultsList rl)
                    {
                        return rl.R_CountMeasure == "M" || rl.R_CountMeasure == "C";
                    });

            //  print each element
            string oot = "";
            switch (currElement.T_Element)
            {
                case "Species":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_Species_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].Species.PadLeft(6, ' '), cr.R_CC_Species.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "LiveDead":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_LiveDead_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].LiveDead.PadLeft(6, ' '), cr.R_CC_LiveDead.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Product":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_Product_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].SampleGroup.PrimaryProduct.PadLeft(6, ' '), cr.R_CC_Product.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Clear Face":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_Clear_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].ClearFace.PadLeft(6, ' '), cr.R_CC_Clear.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Tree Grade":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_TreeGrade_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].Grade.PadLeft(6, ' '), cr.R_CC_TreeGrade.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "DBH":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_DBHOB_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].DBH.ToString().PadLeft(6, ' '), cr.R_CC_DBHOB.ToString().PadLeft(6, ' '),
                                        Utilities.FormatField(cr.R_CC_DBHOB - CruiserTrees[nthRow].DBH, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Total Height":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_TotalHeight_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].TotalHeight.ToString().PadLeft(6, ' '), cr.R_CC_TotalHeight.ToString().PadLeft(6, ' '),
                                         Utilities.FormatField(cr.R_CC_TotalHeight - CruiserTrees[nthRow].TotalHeight, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Total Height <= 100 feet":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_TotHeightUnder_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].TotalHeight.ToString().PadLeft(6, ' '), cr.R_CC_TotHeightUnder.ToString().PadLeft(6, ' '),
                                            Utilities.FormatField(cr.R_CC_TotHeightUnder - CruiserTrees[nthRow].TotalHeight, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Total Height > 100 feet":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_TotHeightOver_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].TotalHeight.ToString().PadLeft(6, ' '), cr.R_CC_TotHeightOver.ToString().PadLeft(6, ' '),
                                            Utilities.FormatField(cr.R_CC_TotHeightOver - CruiserTrees[nthRow].TotalHeight, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Merch Height Primary":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_MerchHgtPP_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].MerchHeightPrimary.ToString().PadLeft(6, ' '), cr.R_CC_MerchHgtPP.ToString().PadLeft(6, ' '),
                                           Utilities.FormatField(cr.R_CC_MerchHgtPP - CruiserTrees[nthRow].MerchHeightPrimary, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Merch Height Secondary":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_MerchHgtSP_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].MerchHeightSecondary.ToString().PadLeft(6, ' '), cr.R_CC_MerchHgtSP.ToString().PadLeft(6, ' '),
                                           Utilities.FormatField(cr.R_CC_MerchHgtSP - CruiserTrees[nthRow].MerchHeightSecondary, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Height To First Live Limb":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_HgtFLL_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].HeightToFirstLiveLimb.ToString().PadLeft(6, ' '), cr.R_CC_HgtFLL.ToString().PadLeft(6, ' '),
                                           Utilities.FormatField(cr.R_CC_HgtFLL - CruiserTrees[nthRow].HeightToFirstLiveLimb, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Top DIB Primary":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_TopDIBPP_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].TopDIBPrimary.ToString().PadLeft(6, ' '), cr.R_CC_TopDIBPP.ToString().PadLeft(6, ' '),
                                           Utilities.FormatField(cr.R_CC_TopDIBPP - CruiserTrees[nthRow].TopDIBPrimary, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Recoverable %":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_RecDef_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            var ev = LoadElementVariables(CruiserTrees[nthRow].RecoverablePrimary.ToString().PadLeft(6, ' '), cr.R_CC_RecDef.ToString().PadLeft(6, ' '),
                                            Utilities.FormatField(cr.R_CC_RecDef - CruiserTrees[nthRow].RecoverablePrimary, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Seen Defect % Primary":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_SeenDefPP_R == 1)
                                oot = "1 **";
                            else oot = "0";
                            //  for region 5, need to calculate seen defect before printing
                            float seenDefect = (Region == "05" || Region == "5") ? getCruiserDefect((long)CruiserTrees[nthRow].Tree_CN)
                                : CruiserTrees[nthRow].SeenDefectPrimary;

                            var ev = LoadElementVariables(Utilities.FormatField(seenDefect, "{0,6:F1}").ToString().PadLeft(6, ' '),
                                                Utilities.FormatField(cr.R_CC_SeenDefPP, "{0,6:F1}").ToString().PadLeft(6, ' '),
                                                Utilities.FormatField(cr.R_CC_SeenDefPP - seenDefect, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);

                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Seen Defect % Secondary":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_SeenDefSP_R == 1)
                                oot = "1 **";
                            else oot = "0";

                            var ev = LoadElementVariables(Utilities.FormatField(CruiserTrees[nthRow].SeenDefectSecondary, "{0,6:F1}").ToString().PadLeft(6, ' '),
                                                Utilities.FormatField(cr.R_CC_SeenDefSP, "{0,6:F1}").ToString().PadLeft(6, ' '),
                                        Utilities.FormatField(cr.R_CC_SeenDefSP - CruiserTrees[nthRow].SeenDefectSecondary, "{0,6:F1}").ToString().PadLeft(6, ' '), " ", " ", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Log Grade":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Log", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserLog(CheckLogs, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CheckLogs[nthRow].Tree.Initials != null)
                                markInits = CheckLogs[nthRow].Tree.Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_LogGrade_R == 1)
                                oot = "1 **";
                            else oot = "0";

                            var ev = LoadElementVariables(CheckLogs[nthRow].Grade.PadLeft(6, ' '), cr.R_CC_LogGrade.PadLeft(6, ' '), "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "Log Defect %":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Log", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserLog(CheckLogs, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CheckLogs[nthRow].Tree.Initials != null)
                                markInits = CheckLogs[nthRow].Tree.Initials;
                            else markInits = " ";

                            string cruiserString = null;
                            if (nthRow >= 0)
                                cruiserString = Utilities.FormatField(CheckLogs[nthRow].SeenDefect, "{0,6:F0}").ToString().PadLeft(6, ' ');
                            else cruiserString = "**";
                            var checkCruiserString = Utilities.FormatField(cr.R_CC_LogSeenDef, "{0,6:F0}").ToString().PadLeft(6, ' ');
                            var diff1 = Utilities.FormatField(cr.R_CC_LogSeenDef - CheckLogs[nthRow].SeenDefect, "{0,6:F0}").ToString().PadLeft(6, ' ');
                            var diff2 = " ";
                            var diff3 = " ";
                            //  finish print array and print line
                            if (cr.R_LogSeenDef_R == 1)
                                oot = "1 **";
                            else oot = "0";

                            var ev = LoadElementVariables(cruiserString, checkCruiserString, diff1, diff2, diff3, oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
                case "In/Out Trees":
                    foreach (ResultsList cr in currentResults)
                    {
                        //  clear print array
                        var prtArray = new List<string>();
                        prtArray.Add("");
                        //  load Identifying information
                        LoadIdentifyingInfo(prtArray, cr, "Tree", ReportToOutput);
                        //  find cruiser tree
                        int nthRow = findCruiserTree(CruiserTrees, cr);
                        if (nthRow >= 0)
                        {
                            markInits = "";
                            //  new page needed?
                            if (numOlines >= 50 || numOlines == 0)
                            {
                                WriteReportHeading(strTextOut, ref numOlines);
                                WriteColumnHeaders(strTextOut, currElement.T_Element, ref numOlines);
                            }   //  endif
                            if (CruiserTrees[nthRow].Initials != null)
                                markInits = CruiserTrees[nthRow].Initials;
                            else markInits = " ";
                            //  finish print array and print line
                            if (cr.R_OutResult == 1)
                                oot = "1 **";
                            else oot = "0";

                            var ev = LoadElementVariables("      ", "      ", "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }
                        else if (nthRow < 0)
                        {
                            //  probably an out tree called in by check cruiser
                            if (cr.R_InResult == 1)
                                oot = "1 **";
                            else oot = "0";

                            var ev = LoadElementVariables("      ", "      ", "na", "na", "na", oot);
                            LoadPrintArray(prtArray, ev, markInits);
                            printLine(strTextOut, currElement.T_Element, prtArray, fieldLengths, ref numOlines);
                        }   //  endif on nthRow
                    }   //  end foreach loop
                    break;
            }   //  end switch on element

            return;
        }   //  end PrintElements

        private static void LoadIdentifyingInfo(IList<string> prtArray, ResultsList cr, string logElement, int reportToOutput)
        {
            //  loads identifying information for specified report
            switch (reportToOutput)
            {
                case 1:
                    prtArray.Add(cr.R_Stratum.PadLeft(4, ' '));
                    prtArray.Add(cr.R_CuttingUnit.PadLeft(9, ' '));
                    prtArray.Add(cr.R_Plot.PadLeft(9, ' '));
                    prtArray.Add(cr.R_TreeNumber.PadLeft(8, ' '));
                    prtArray.Add(cr.R_CountMeasure.PadLeft(6, ' '));
                    if (logElement == "Log") prtArray.Add(cr.R_LogNumber.PadLeft(5, ' '));
                    break;
                case 2:
                case 3:
                    prtArray.Add(cr.R_CuttingUnit.PadLeft(8, ' '));
                    prtArray.Add(cr.R_TreeNumber.PadLeft(8, ' '));
                    prtArray.Add(cr.R_CountMeasure.PadLeft(6, ' '));
                    break;
            }   //  end switcu
            return;
        }   //  end LoadIdentifyingInto

        private ElementValues LoadElementVariables(string Value1, string Value2, string Value3, string Value4, string Value5, string Value6)
        {
            //cruiserString = Value1;
            //checkCruiserString = Value2;
            //diff1 = Value3;
            //diff2 = Value4;
            //diff3 = Value5;
            //outOfTolerance = Value6;

            return new ElementValues
            {
                cruiserString = Value1,
                checkCruiserString = Value2,
                diff1 = Value3,
                diff2 = Value4,
                diff3 = Value5,
                outOfTolerance = Value6
            };
        }   //  end LoadElementVariables

        //private static void LoadPrintArray(IList<string> prtArray, string cruiserString, string checkCruiserString, string outOfTolerance,
        //                    string diff1, string diff2, string diff3, string markInits)
        //{
        //    prtArray.Add(cruiserString);
        //    prtArray.Add(checkCruiserString);
        //    prtArray.Add(markInits);
        //    prtArray.Add(COLUMN_SEPARATOR);
        //    prtArray.Add(diff1);
        //    prtArray.Add(COLUMN_SEPARATOR);
        //    prtArray.Add(diff2);
        //    prtArray.Add(COLUMN_SEPARATOR);
        //    prtArray.Add(diff3);
        //    prtArray.Add(COLUMN_SEPARATOR);
        //    prtArray.Add(outOfTolerance);
        //}   //  end LoadPrintArray

        private static void LoadPrintArray(IList<string> prtArray, ElementValues data, string markInits)
        {
            prtArray.Add(data.cruiserString);
            prtArray.Add(data.checkCruiserString);
            prtArray.Add(markInits);
            prtArray.Add(COLUMN_SEPARATOR);
            prtArray.Add(data.diff1);
            prtArray.Add(COLUMN_SEPARATOR);
            prtArray.Add(data.diff2);
            prtArray.Add(COLUMN_SEPARATOR);
            prtArray.Add(data.diff3);
            prtArray.Add(COLUMN_SEPARATOR);
            prtArray.Add(data.outOfTolerance);
        }   //  end LoadPrintArray

        private static int findCruiserTree(List<TreeDO> cruiserTrees, ResultsList cr)
        {
            int nthRow = cruiserTrees.FindIndex(
                delegate (TreeDO t)
                {
                    string tempPlot;
                    if (t.Plot == null)
                        tempPlot = "";
                    else tempPlot = t.Plot.PlotNumber.ToString();
                    return t.Stratum.Code == cr.R_Stratum && t.CuttingUnit.Code == cr.R_CuttingUnit &&
                                tempPlot == cr.R_Plot && t.TreeNumber.ToString() == cr.R_TreeNumber;
                });

            return nthRow;
        }   //  end findCruiserTree


        private static int findCruiserLog(List<LogStockDO> checkLogs, ResultsList cr)
        {
            //  pull log for this check cruiser result
            int nthRow = checkLogs.FindIndex(
                delegate (LogStockDO lsd)
                {
                    string tempPlot = "";
                    if (lsd.Tree.Plot.PlotNumber.ToString() == null)
                        tempPlot = "";
                    else tempPlot = lsd.Tree.Plot.PlotNumber.ToString();

                    return lsd.Tree.Stratum.Code == cr.R_Stratum && lsd.Tree.CuttingUnit.Code == cr.R_CuttingUnit &&
                                               tempPlot == cr.R_Plot && lsd.LogNumber == cr.R_LogNumber;
                });
            return nthRow;
        }   //  end findCruiserLog

        private float getCruiserDefect(long currTree_CN)
        {
            int nthRow = CalculatedTrees.FindIndex(
                delegate (TreeCalculatedValuesDO tcv)
                {
                    return tcv.Tree_CN == currTree_CN;
                });
            if (nthRow >= 0)
            {
                if (CalculatedTrees[nthRow].GrossCUFTPP > 0)
                    return ((CalculatedTrees[nthRow].GrossCUFTPP - CalculatedTrees[nthRow].NetCUFTPP) / CalculatedTrees[nthRow].GrossCUFTPP) * 100;
            }   // endif
            return 0;
        }   //  end getCruiserDefect


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

        class ElementValues
        {
            public string cruiserString { get; set; }
            public string checkCruiserString { get; set; }
            public string diff1 { get; set; }
            public string diff2 { get; set; }
            public string diff3 { get; set; }
            public string outOfTolerance { get; set; }
        }
    }
}
