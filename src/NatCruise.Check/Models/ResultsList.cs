using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckCruise
{
    [Table("Results")]
    public class ResultsList
    {
        public string R_Stratum { get; set; }
        public string R_CuttingUnit { get; set; }
        public string R_Plot { get; set; }
        public string R_SampleGroup { get; set; }
        public string R_CountMeasure { get; set; }
        public string R_TreeNumber { get; set; }
        public string R_LogNumber { get; set; }
        public string R_CC_Species { get; set; }
        public int R_Species_R { get; set; }
        public string R_CC_LiveDead { get; set; }
        public int R_LiveDead_R { get; set; }
        public string R_CC_Product { get; set; }
        public int R_Product_R { get; set; }
        public float R_CC_DBHOB { get; set; }
        public int R_DBHOB_R { get; set; }
        public float R_CC_TotalHeight { get; set; }
        public int R_TotalHeight_R { get; set; }
        public float R_CC_TotHeightUnder { get; set; }
        public int R_TotHeightUnder_R { get; set; }
        public float R_CC_TotHeightOver { get; set; }
        public int R_TotHeightOver_R { get; set; }
        public float R_CC_MerchHgtPP { get; set; }
        public int R_MerchHgtPP_R { get; set; }
        public float R_CC_MerchHgtSP { get; set; }
        public int R_MerchHgtSP_R { get; set; }
        public float R_CC_HgtFLL { get; set; }
        public int R_HgtFLL_R { get; set; }
        public float R_CC_SeenDefPP { get; set; }
        public int R_SeenDefPP_R { get; set; }
        public float R_CC_SeenDefSP { get; set; }
        public int R_SeenDefSP_R { get; set; }
        public float R_CC_RecDef { get; set; }
        public int R_RecDef_R { get; set; }
        public float R_CC_TopDIBPP { get; set; }
        public int R_TopDIBPP_R { get; set; }
        public int R_CC_FormClass { get; set; }
        public int R_FormClass_R { get; set; }
        public string R_CC_Clear { get; set; }
        public int R_Clear_R { get; set; }
        public string R_CC_TreeGrade { get; set; }
        public int R_TreeGrade_R { get; set; }
        public string R_CC_LogGrade { get; set; }
        public int R_LogGrade_R { get; set; }
        public float R_CC_LogSeenDef { get; set; }
        public int R_LogSeenDef_R { get; set; }
        public int R_IncludeVol { get; set; }
        public string R_TreeSpecies { get; set; }
        public string R_TreeProduct { get; set; }
        public string R_MarkerInitials { get; set; }
        public double R_CC_GrossCUFTPP { get; set; }
        public double R_GrossCUFTPP { get; set; }
        public double R_CC_NetCUFTPP { get; set; }
        public double R_NetCUFTPP { get; set; }
        public double R_CC_GrossCUFTSP { get; set; }
        public double R_GrossCUFTSP { get; set; }
        public double R_CC_NetCUFTSP { get; set; }
        public double R_NetCUFTSP { get; set; }
        public double R_CC_GrossCUFTPSP { get; set; }
        public double R_GrossCUFTPSP { get; set; }
        public double R_CC_NetCUFTPSP { get; set; }
        public double R_NetCUFTPSP { get; set; }
        public double R_CC_GrossBDFTPP { get; set; }
        public double R_GrossBDFTPP { get; set; }
        public double R_CC_NetBDFTPP { get; set; }
        public double R_NetBDFTPP { get; set; }
        public double R_CC_GrossBDFTSP { get; set; }
        public double R_GrossBDFTSP { get; set; }
        public double R_CC_NetBDFTSP { get; set; }
        public double R_NetBDFTSP { get; set; }
        public double R_CC_GrossBDFTPSP { get; set; }
        public double R_GrossBDFTPSP { get; set; }
        public double R_CC_NetBDFTPSP { get; set; }
        public double R_NetBDFTPSP { get; set; }
        public int R_InResult { get; set; }
        public int R_OutResult { get; set; }
    }
}
