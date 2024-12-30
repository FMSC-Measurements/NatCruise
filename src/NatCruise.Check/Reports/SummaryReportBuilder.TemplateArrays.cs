using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Check.Reports
{
    public partial class SummaryReportBuilder
    {
        protected class TextReportHeaders
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
        }
    }
}
