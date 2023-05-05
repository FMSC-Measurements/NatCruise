using CruiseDAL;
using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Data
{
    public class TreeAuditRuleDataservice : CruiseDataserviceBase, ITreeAuditRuleDataservice
    {
        public TreeAuditRuleDataservice(CruiseDatastore_V3 database, string cruiseID, string deviceID) : base(database, cruiseID, deviceID)
        {
        }

        public TreeAuditRuleDataservice(string path, string cruiseID, string deviceID) : base(path, cruiseID, deviceID)
        {
        }

        #region rule selector

        public void AddRuleSelector(TreeAuditRuleSelector tars)
        {
            Database.Execute2(
@"INSERT INTO TreeAuditRuleSelector (
    CruiseID,
    SpeciesCode,
    LiveDead,
    PrimaryProduct,
    TreeAuditRuleID
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @LiveDead,
    @PrimaryProduct,
    @TreeAuditRuleID
);"
            , new
            {
                CruiseID,
                tars.SpeciesCode,
                tars.LiveDead,
                tars.PrimaryProduct,
                tars.TreeAuditRuleID,
            });
        }

        public IEnumerable<TreeAuditRuleSelector> GetRuleSelectors()
        {
            return Database.From<TreeAuditRuleSelector>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public IEnumerable<TreeAuditRuleSelector> GetRuleSelectors(string tarID)
        {
            return Database.From<TreeAuditRuleSelector>()
                .Where("TreeAuditRuleID = @p1")
                .Query(tarID).ToArray();
        }

        public void DeleteRuleSelector(TreeAuditRuleSelector tars)
        {
            Database.Execute2(
@"DELETE FROM TreeAuditRuleSelector
WHERE TreeAuditRuleID = @TreeAuditRuleID
AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '')
AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '')
AND ifnull(LiveDead, '') = ifnull(@LiveDead, '');",
                new
                {
                    CruiseID,
                    tars.SpeciesCode,
                    tars.PrimaryProduct,
                    tars.LiveDead,
                    tars.TreeAuditRuleID,
                });
        }

        #endregion rule selector

        #region tree audit rule

        public void AddTreeAuditRule(TreeAuditRule tar)
        {
            if (string.IsNullOrEmpty(tar.TreeAuditRuleID)) { throw new ArgumentException("Value was null or empty", nameof(tar)); }

            Database.Execute2(
@"INSERT INTO TreeAuditRule (
    CruiseID,
    TreeAuditRuleID,
    Field,
    Min,
    Max,
    Description
) VALUES (
    @CruiseID,
    @TreeAuditRuleID,
    @Field,
    @Min,
    @Max,
    @Description
);",
    new
    {
        CruiseID,
        tar.TreeAuditRuleID,
        tar.Field,
        tar.Min,
        tar.Max,
        tar.Description,
    });
        }

        public IEnumerable<TreeAuditRule> GetTreeAuditRules()
        {
            return Database.From<TreeAuditRule>()
                .Where("CruiseID = @p1")
                .Query(CruiseID).ToArray();
        }

        public IEnumerable<TreeAuditRule> GetTreeAuditRules(string species, string prod, string livedead)
        {
            return Database.From<TreeAuditRule>()
                .Join("TreeAuditRuleSelector", "USING (TreeAuditRuleID)")
                .Where("CruiseID = @CruiseID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '')")
                .Query2(new
                {
                    CruiseID,
                    SpeciesCode = species,
                    PrimaryProduct = prod,
                    LiveDead = livedead,
                });
        }

        public TreeAuditRule GetTreeAuditRule(string tarID)
        {
            if (tarID is null)
            {
                throw new ArgumentNullException(nameof(tarID));
            }

            return Database.From<TreeAuditRule>()
                .Where("CruiseID = @p1 AND TreeAuditRuleID = @p2")
                .Query(CruiseID, tarID).SingleOrDefault();
        }

        public void DeleteTreeAuditRule(TreeAuditRule tar)
        {
            Database.Execute("DELETE FROM TreeAuditRule WHERE TreeAuditRuleID = @p1;", tar.TreeAuditRuleID);
        }

        public void UpsertTreeAuditRule(TreeAuditRule tar)
        {
            if (string.IsNullOrEmpty(tar.TreeAuditRuleID)) { throw new ArgumentException("Value was null or empty", nameof(tar)); }

            Database.Execute2(
@"INSERT INTO TreeAuditRule (
    CruiseID,
    TreeAuditRuleID,
    Field,
    Min,
    Max,
    Description
) VALUES (
    @CruiseID,
    @TreeAuditRuleID,
    @Field,
    @Min,
    @Max,
    @Description
)
ON CONFLICT (TreeAuditRuleID) DO
UPDATE SET
    Field = @Field,
    Min = @Min,
    Max = @Max,
    Description = @Description
WHERE TreeAuditRuleID = @TreeAuditRuleID;",
    new
    {
        CruiseID,
        tar.TreeAuditRuleID,
        tar.Field,
        tar.Min,
        tar.Max,
        tar.Description,
    });
        }

        #endregion tree audit rule
    }
}