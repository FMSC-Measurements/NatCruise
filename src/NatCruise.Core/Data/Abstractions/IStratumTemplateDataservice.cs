using NatCruise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Data
{
    public interface IStratumTemplateDataservice : IDataservice
    {
        #region StratumTemplate

        IEnumerable<StratumTemplate> GetStratumTemplates();

        void UpsertStratumTemplate(StratumTemplate st);

        void DeleteStratumTemplate(StratumTemplate st);

        #endregion StratumTemplate

        #region StratumTemplateTreeFieldSetup

        IEnumerable<StratumTemplateTreeFieldSetup> GetStratumTemplateTreeFieldSetups(string stratumTemplateName);

        void UpsertStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs);

        void DeleteStratumTemplateTreeFieldSetup(StratumTemplateTreeFieldSetup stfs);

        #endregion StratumTemplateTreeFieldSetup

        #region StratumTemplateLogFieldSetup

        IEnumerable<StratumTemplateLogFieldSetup> GetStratumTemplateLogFieldSetups(string stratumTemplateName);

        void UpsertStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs);

        void DeleteStratumTemplateLogFieldSetup(StratumTemplateLogFieldSetup stlfs);

        #endregion StratumTemplateLogFieldSetup
    }
}
