using NatCruise.Data;
using NatCruise.Design.Models;
using System.Collections.Generic;

namespace NatCruise.Wpf.Data
{
    public interface ITemplateDataservice : IDataservice
    {

        #region TreeDefaultValues

        IEnumerable<TreeDefaultValue> GetTreeDefaultValues();

        void AddTreeDefaultValue(TreeDefaultValue tdv);

        void UpsertTreeDefaultValue(TreeDefaultValue tdv);

        void DeleteTreeDefaultValue(TreeDefaultValue tdv);

        #endregion TreeDefaultValues

        #region Reports

        IEnumerable<Reports> GetReports();

        void AddReport(Reports report);

        void UpsertReport(Reports report);

        #endregion Reports

        #region VolumeEquation

        IEnumerable<VolumeEquation> GetVolumeEquations();

        void AddVolumeEquation(VolumeEquation ve);

        void UpsertVolumeEquation(VolumeEquation ve);

        #endregion VolumeEquation
    }
}