using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckCruise.Models;
using CruiseDAL;
using CruiseDAL.DataObjects;

namespace CheckCruise
{
    public class dataBaseCommands
    {
        #region
        public CruiseDAL.DAL DAL { get; }

        public dataBaseCommands()
        {
        }

        public dataBaseCommands(DAL dAL)
        {
            DAL = dAL ?? throw new ArgumentNullException(nameof(dAL));
        }

        
        #endregion


        public IReadOnlyList<UnitStratum> GetUnitStrata()
        {
            return DAL.Query<UnitStratum>("SELECT cu.Code as CuttingUnitCode, st.Code AS StratumCode, cu.CuttingUnit_CN, st.Stratum_CN, st.Method AS Method " +
                "FROM CuttingUnitStratum AS cust " +
                "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "JOIN Stratum AS st USING (Stratum_CN)").ToArray();
        }

        public bool doesTableExist(string tableName)
        {
            return DAL.CheckTableExists(tableName);
        }   //  end doesTableExist

        public void createNewTable(string tableName)
        {
            //  build query for new table
            string[] valuesList;
            string queryString = "";
            if (tableName == "Tolerances")
            {
                valuesList = createTolerancesList();
                queryString = createNewTableQuery(tableName, valuesList);
            }
            else if (tableName == "Results")
            {
                valuesList = createResultsList();
                queryString = createNewTableQuery(tableName, valuesList);
            }

            DAL.Execute(queryString);
        }   //  end createNewTable


        public string createNewTableQuery(string tableName, string[] valuesList)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(tableName);
            sb.Append(" (");

            //  add values to string
            foreach (string s in valuesList)
                sb.Append(s);
            //  add close parens and semi colon
            sb.Append(");");
            return sb.ToString();
        }   //  end createNewTableQuery


        public string[] createTolerancesList()
        {
            string[] tolerancesList = new string[23] {"T_Element TEXT,",
                                                      "T_Tolerance TEXT,",
                                                      "T_Units TEXT,",
                                                      "T_AddParam TEXT,",
                                                      "T_Weight REAL,",
                                                      "T_CUFTPGtolerance REAL,",
                                                      "T_CUFTPNtolerance REAL,",
                                                      "T_CUFTSGtolerance REAL,",
                                                      "T_CUFTSNtolerance REAL,",
                                                      "T_CUFTPSGtolerance REAL,",
                                                      "T_CUFTPSNtolerance REAL,",
                                                      "T_BDFTPGtolerance REAL,",
                                                      "T_BDFTPNtolerance REAL,",
                                                      "T_BDFTSGtolerance REAL,",
                                                      "T_BDFTSNtolerance REAL,",
                                                      "T_BDFTPSGtolerance REAL,",
                                                      "T_BDFTPSNtolerance REAL,",
                                                      "T_IncludeVolume INT,",
                                                      "T_BySpecies INT,",
                                                      "T_ByProduct INT,",
                                                      "T_ElementAccuracy REAL,",
                                                      "T_OverallAccuracy REAL,",
                                                      "T_DateStamp TEXT"};
            return tolerancesList;
        }   //  end createTolerancesList


        public string[] createResultsList()
        {
            string[] resultsList = new string[75] {"R_Stratum TEXT,",
                                                   "R_CuttingUnit TEXT,",
                                                   "R_Plot TEXT,",
                                                   "R_SampleGroup TEXT,",
                                                   "R_CountMeasure TEXT,",
                                                   "R_TreeNumber TEXT,",
                                                   "R_LogNumber TEXT,",
                                                   "R_CC_Species TEXT,",
                                                   "R_Species_R INT,",
                                                   "R_CC_LiveDead TEXT,",
                                                   "R_LiveDead_R INT,",
                                                   "R_CC_Product TEXT,",
                                                   "R_Product_R INT,",
                                                   "R_CC_DBHOB REAL,",
                                                   "R_DBHOB_R INT,",
                                                   "R_CC_TotalHeight REAL,",
                                                   "R_TotalHeight_R INT,",
                                                   "R_CC_TotHeightUnder REAL,",
                                                   "R_TotHeightUnder_R INT,",
                                                   "R_CC_TotHeightOver REAL,",
                                                   "R_TotHeightOver_R INT,",
                                                   "R_CC_MerchHgtPP REAL,",
                                                   "R_MerchHgtPP_R INT,",
                                                   "R_CC_MerchHgtSP REAL,",
                                                   "R_MerchHgtSP_R INT,",
                                                   "R_CC_HgtFLL REAL,",
                                                   "R_HgtFLL_R INT,",
                                                   "R_CC_SeenDefPP REAL,",
                                                   "R_SeenDefPP_R INT,",
                                                   "R_CC_SeenDefSP REAL,",
                                                   "R_SeenDefSP_R INT,",
                                                   "R_CC_RecDef REAL,",
                                                   "R_RecDef_R INT,",
                                                   "R_CC_TopDIBPP INT,",
                                                   "R_TopDIBPP_R INT,",
                                                   "R_CC_FormClass INT,",
                                                   "R_FormClass_R INT,",
                                                   "R_CC_Clear TEXT,",
                                                   "R_Clear_R INT,",
                                                   "R_CC_TreeGrade TEXT,",
                                                   "R_TreeGrade_R INT,",
                                                   "R_CC_LogGrade TEXT,",
                                                   "R_LogGrade_R INT,",
                                                   "R_CC_LogSeenDef REAL,",
                                                   "R_LogSeenDef_R INT,",
                                                   "R_IncludeVol INT,",
                                                   "R_TreeSpecies TEXT,",
                                                   "R_TreeProduct TEXT,",
                                                   "R_MarkerInitials TEXT,",
                                                   "R_CC_GrossCUFTPP REAL,",
                                                   "R_GrossCUFTPP REAL,",
                                                   "R_CC_NetCUFTPP REAL,",
                                                   "R_NetCUFTPP REAL,",
                                                   "R_CC_GrossCUFTSP REAL,",
                                                   "R_GrossCUFTSP REAL,",
                                                   "R_CC_NetCUFTSP REAL,",
                                                   "R_NetCUFTSP REAL,",
                                                   "R_CC_GrossCUFTPSP REAL,",
                                                   "R_GrossCUFTPSP REAL,",
                                                   "R_CC_NetCUFTPSP REAL,",
                                                   "R_NetCUFTPSP REAL,",
                                                   "R_CC_GrossBDFTPP REAL,",
                                                   "R_GrossBDFTPP REAL,",
                                                   "R_CC_NetBDFTPP REAL,",
                                                   "R_NetBDFTPP REAL,",
                                                   "R_CC_GrossBDFTSP REAL,",
                                                   "R_GrossBDFTSP REAL,",
                                                   "R_CC_NetBDFTSP REAL,",
                                                   "R_NetBDFTSP REAL,",
                                                   "R_CC_GrossBDFTPSP REAL,",
                                                   "R_GrossBDFTPSP REAL,",
                                                   "R_CC_NetBDFTPSP REAL,",
                                                   "R_NetBDFTPSP REAL,",
                                                   "R_InResult INT,",
                                                   "R_OutResult INT"};
            return resultsList;
        }   //  end createResultsList


        public SaleDO getSale()
        {
            return DAL.From<SaleDO>().Read().FirstOrDefault();
        }   //  end getSale

        public List<StratumDO> getStrata()
        {
            return DAL.From<StratumDO>().OrderBy("Code").Read().ToList();
        }   //  end getStrata

        public List<CuttingUnitDO> getUnits()
        {
            return DAL.From<CuttingUnitDO>().Read().ToList();
        }   //  end getUnits

        public List<LogStockDO> getLogStock()
        {
            return DAL.From<LogStockDO>().Read().ToList();
        }   //  end getLogs

        public List<LogDO> getLogs()
        {
            return DAL.From<LogDO>().Read().ToList();
        }   //  end getLogs

        public List<TreeDO> getTrees()
        {
            return DAL.From<TreeDO>().Read().ToList();
        }   //  end getTrees


        public List<TreeCalculatedValuesDO> getCalculatedTrees()
        {
            return DAL.From<TreeCalculatedValuesDO>().Read().ToList();
        }   //  end getCalculatedTrees



        public List<TreeCalculatedValuesDO> getCalculatedTrees(string cruiseInit)
        {
            return DAL.From<TreeCalculatedValuesDO>()
                .Join("Tree", "USING (Tree_CN)")
                .Where("Initials = @p1")
                .Read(cruiseInit).ToList();
        }   //  end getCalcTrees


        public List<TreeDO> getTrees(string cruiseInit)
        {
            return DAL.From<TreeDO>()
                .Where("Initials = @p1")
                .Read().ToList();
        }   //  end getTrees by cruiser





        public void saveTrees(List<TreeDO> tList)
        {
            foreach (TreeDO tdo in tList)
            {
                if (tdo.DAL == null)
                    tdo.DAL = DAL;
                tdo.Save();
            }   //  end foreach loop
        }   //  end saveTrees

        
        public List<StratumDO> getCurrentStratum(string stratumCode)
        {
            return DAL.From<StratumDO>().Where("Code = @p1").Read(stratumCode).ToList();
        }   //  end getCurrentStratum




        public void deleteUnit(string unitToDelete, string stratumToDelete)
        {
            DAL.Execute("DELETE FROM Log WHERE Tree_CN IN " +
                "(SELECT Tree_CN FROM Tree AS t " +
                "   JOIN CuttingUnit AS cu USING (CuttingUnit_CN)" +
                "   JOIN Stratum AS st USING (Stratum_CN) " +
                "   WHERE cu.Code = @p1 " +
                "       AND st.Code = @p2);", unitToDelete, stratumToDelete);

            DAL.Execute("DELETE FROM Tree AS T WHERE Tree_CN IN " +
                "(SELECT Tree_CN FROM Tree AS t " +
                "   JOIN CuttingUnit AS cu USING (CuttingUnit_CN)" +
                "   JOIN Stratum AS st USING (Stratum_CN) " +
                "   WHERE cu.Code = @p1 " +
                "       AND st.Code = @p2);", unitToDelete, stratumToDelete);

        }   //  end deleteUnit


        public void deleteTreeCalculatedValues()
        {
            //  the following code is what Ben Campbell suggested but it runs REALLY SLOW
            //  Switched back to my method which runs faster
            //List<TreeCalculatedValuesDO> tcvList = getTreeCalculatedValues();
            //foreach (TreeCalculatedValuesDO tcvdo in tcvList)
            //    tcvdo.Delete();

            DAL.Execute("DELETE FROM TreeCalculatedValues WHERE TreeCalcValues_CN>0");

        }   //  end deleteTreeCalculatedValues


        public void deleteLogStock()
        {
            //  see note above concerning this code

            DAL.Execute("DELETE FROM LogStock WHERE LogStock_CN>0");
        }   //  end deleteLogStock


        public string getRegion()
        {
            var sale = getSale();
            return sale.Region;
        }   //  end getRegion


        public string getSaleName()
        {
            var sale = getSale();
            return sale.Name;
        }   //  end getSaleName


        public void clearLogTable()
        {
            List<LogDO> logList = getLogs();
            if (logList.Count > 0)
            {
                foreach (LogDO l in logList)
                    l.Delete();
            }   //    endif
        }   //  end clearLogTable

        public ArrayList getCruiserInitials()
        {
            //  pull cruiser initials from results table
            var initials = DAL.QueryScalar<string>("SELECT DISTINCT R_MarkerInitials FROM Results")
                .Where(x => string.IsNullOrEmpty(x) == false)
                .ToArray();

            return new ArrayList(initials);
        }   //  end getCruiserInitials

        public string getCheckCruiserInitials()
        {
            return DAL.ReadGlobalValue("CheckCruise", "Check Cruiser Initials");
        }

        public void SaveCheckCruiserInitials(string checkCruiseInititals)
        {
            DAL.WriteGlobalValue("CheckCruise", "Check Cruiser Initials", checkCruiseInititals);
        } 

        // TODO what does this do?... and test
        public void updateTreeMeasurements()
        {
            //  This saves the updated tree list
            DAL.Execute(
@"INSERT OR REPLACE INTO Tree (
    Tree_CN,
    CuttingUnit_CN,
    Stratum_CN,
    Plot_CN,
    TreeDefaultValue_CN,
    SampleGroup_CN,
    TreeNumber,
    Species,
    CountOrMeasure,
    CreatedBy,
    CreatedDate
)
SELECT
    Tree_CN,
    CuttingUnit_CN,
    Stratum_CN,
    Plot_CN,
    TreeDefaultValue_CN,
    SampleGroup_CN,
    TreeNumber,
    Species,
    CountOrMeasure,
    CreatedBy,
    CreatedDate
FROM Tree;");
            return;
        }   //  end saveUpdatedTrees


        public List<TreeDO> getStrataUnit()
        {
            return DAL.From<TreeDO>()
                .GroupBy("Stratum_CN,CuttingUnit_CN")
                .Read().ToList();
        }   //  end getStrataUnit


        public List<TolerancesList> getTolerances()
        {
            return DAL.Query<TolerancesList>("SELECT * FROM Tolerances").ToList();
        }   //  end getTolerances


        public void saveTolerances(IEnumerable<TolerancesList> currentList)
        {
            DAL.Execute("DELETE FROM Tolerances;");
            foreach (var tl in currentList)
            {
                DAL.Execute2(
    @"INSERT OR REPLACE INTO Tolerances (
    T_Element,
    T_Tolerance,
    T_Units,
    T_AddParam,
    T_Weight,
    T_CUFTPGtolerance,
    T_CUFTPNtolerance,
    T_CUFTSGtolerance,
    T_CUFTSNtolerance,
    T_CUFTPSGtolerance,
    T_CUFTPSNtolerance,
    T_BDFTPGtolerance,
    T_BDFTPNtolerance,
    T_BDFTSGtolerance,
    T_BDFTSNtolerance,
    T_BDFTPSGtolerance,
    T_BDFTPSNtolerance,
    T_IncludeVolume,
    T_BySpecies,
    T_ByProduct,
    T_ElementAccuracy,
    T_OverallAccuracy,
    T_DateStamp
) VALUES (
    @T_Element,
    @T_Tolerance,
    @T_Units,
    @T_AddParam,
    @T_Weight,
    @T_CUFTPGtolerance,
    @T_CUFTPNtolerance,
    @T_CUFTSGtolerance,
    @T_CUFTSNtolerance,
    @T_CUFTPSGtolerance,
    @T_CUFTPSNtolerance,
    @T_BDFTPGtolerance,
    @T_BDFTPNtolerance,
    @T_BDFTSGtolerance,
    @T_BDFTSNtolerance,
    @T_BDFTPSGtolerance,
    @T_BDFTPSNtolerance,
    @T_IncludeVolume,
    @T_BySpecies,
    @T_ByProduct,
    @T_ElementAccuracy,
    @T_OverallAccuracy,
    @T_DateStamp
);", tl);
            }
        }   //  end saveTolerances


        public List<ResultsList> getResultsTable(string groupToOrder, string whereBy)
        {
            var query = new StringBuilder();
            query.Append("SELECT * FROM Results");
            if (!string.IsNullOrEmpty(groupToOrder))
            {
                query.Append(" GROUP BY ");
                query.Append(groupToOrder);
            }
            else if (!string.IsNullOrEmpty(whereBy))
            {
                query.Append(" WHERE R_MarkerInitials = '");
                query.Append(whereBy);
                query.Append("';");
            }   //  endif

            return DAL.Query<ResultsList>(query.ToString()).ToList();
        }   //  end getResults



        public void clearResults()
        {
            DAL.Execute("DELETE FROM Results;");
        }   // end clearResults
        
        public void saveResults(IEnumerable<ResultsList> checkResults)
        {
            foreach(var cr in checkResults)
            {
                DAL.Execute2(
@"
INSERT OR REPLACE INTO Results(
    R_Stratum,
    R_CuttingUnit,
    R_Plot,
    R_SampleGroup,
    R_CountMeasure,
    R_TreeNumber,
    R_LogNumber, 
    R_CC_Species,
    R_Species_R,
    R_CC_LiveDead,
    R_LiveDead_R,
    R_CC_Product,
    R_Product_R,
    R_CC_DBHOB,
    R_DBHOB_R,
    R_CC_TotalHeight,
    R_TotalHeight_R,
    R_CC_TotHeightUnder,
    R_TotHeightUnder_R,
    R_CC_TotHeightOver,
    R_TotHeightOver_R,
    R_CC_MerchHgtPP,
    R_MerchHgtPP_R,
    R_CC_MerchHgtSP,
    R_MerchHgtSP_R,
    R_CC_HgtFLL,
    R_HgtFLL_R,
    R_CC_SeenDefPP,
    R_SeenDefPP_R,
    R_CC_SeenDefSP,
    R_SeenDefSP_R,
    R_CC_RecDef,
    R_RecDef_R,
    R_CC_TopDIBPP,
    R_TopDIBPP_R,
    R_CC_FormClass,
    R_FormClass_R,
    R_CC_Clear,
    R_Clear_R,
    R_CC_TreeGrade,
    R_TreeGrade_R,
    R_CC_LogGrade,
    R_LogGrade_R,
    R_CC_LogSeenDef,
    R_LogSeenDef_R,
    R_IncludeVol,
    R_TreeSpecies,
    R_TreeProduct,
    R_MarkerInitials,
    R_CC_GrossCUFTPP,
    R_GrossCUFTPP,
    R_CC_GrossBDFTPP,
    R_GrossBDFTPP,
    R_CC_NetCUFTPP,
    R_NetCUFTPP,
    R_CC_NetBDFTPP,
    R_NetBDFTPP,
    R_CC_GrossCUFTSP,
    R_GrossCUFTSP,
    R_CC_GrossBDFTSP,
    R_GrossBDFTSP,
    R_CC_NetCUFTSP,
    R_NetCUFTSP,
    R_CC_NetBDFTSP,
    R_NetBDFTSP,
    R_CC_GrossCUFTPSP,
    R_GrossCUFTPSP,
    R_CC_NetCUFTPSP,
    R_NetCUFTPSP,
    R_CC_GrossBDFTPSP,
    R_GrossBDFTPSP,
    R_CC_NetBDFTPSP,
    R_NetBDFTPSP,
    R_InResult,
    R_OutResult
) VALUES (
    @R_Stratum,
    @R_CuttingUnit,
    @R_Plot,
    @R_SampleGroup,
    @R_CountMeasure,
    @R_TreeNumber,
    @R_LogNumber, 
    @R_CC_Species,
    @R_Species_R,
    @R_CC_LiveDead,
    @R_LiveDead_R,
    @R_CC_Product,
    @R_Product_R,
    @R_CC_DBHOB,
    @R_DBHOB_R,
    @R_CC_TotalHeight,
    @R_TotalHeight_R,
    @R_CC_TotHeightUnder,
    @R_TotHeightUnder_R,
    @R_CC_TotHeightOver,
    @R_TotHeightOver_R,
    @R_CC_MerchHgtPP,
    @R_MerchHgtPP_R,
    @R_CC_MerchHgtSP,
    @R_MerchHgtSP_R,
    @R_CC_HgtFLL,
    @R_HgtFLL_R,
    @R_CC_SeenDefPP,
    @R_SeenDefPP_R,
    @R_CC_SeenDefSP,
    @R_SeenDefSP_R,
    @R_CC_RecDef,
    @R_RecDef_R,
    @R_CC_TopDIBPP,
    @R_TopDIBPP_R,
    @R_CC_FormClass,
    @R_FormClass_R,
    @R_CC_Clear,
    @R_Clear_R,
    @R_CC_TreeGrade,
    @R_TreeGrade_R,
    @R_CC_LogGrade,
    @R_LogGrade_R,
    @R_CC_LogSeenDef,
    @R_LogSeenDef_R,
    @R_IncludeVol,
    @R_TreeSpecies,
    @R_TreeProduct,
    @R_MarkerInitials,
    @R_CC_GrossCUFTPP,
    @R_GrossCUFTPP,
    @R_CC_GrossBDFTPP,
    @R_GrossBDFTPP,
    @R_CC_NetCUFTPP,
    @R_NetCUFTPP,
    @R_CC_NetBDFTPP,
    @R_NetBDFTPP,
    @R_CC_GrossCUFTSP,
    @R_GrossCUFTSP,
    @R_CC_GrossBDFTSP,
    @R_GrossBDFTSP,
    @R_CC_NetCUFTSP,
    @R_NetCUFTSP,
    @R_CC_NetBDFTSP,
    @R_NetBDFTSP,
    @R_CC_GrossCUFTPSP,
    @R_GrossCUFTPSP,
    @R_CC_NetCUFTPSP,
    @R_NetCUFTPSP,
    @R_CC_GrossBDFTPSP,
    @R_GrossBDFTPSP,
    @R_CC_NetBDFTPSP,
    @R_NetBDFTPSP,
    @R_InResult,
    @R_OutResult
);", cr);
            }
        }   //  end saveResults

    }
}
