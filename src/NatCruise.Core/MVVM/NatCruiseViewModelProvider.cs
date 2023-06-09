using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM
{
    public class NatCruiseViewModelProvider
    {
        private Dictionary<string, Type> _vmTypeMap = new Dictionary<string, Type>();

        public NatCruiseViewModelProvider()
        {
            Register("TreeCountEditView", typeof(TreeCountEditViewModel));
            Register("TreeEditView", typeof(TreeEditViewModel));
            Register("LogEditView", typeof(LogEditViewModel));
            Register("TreeAuditRuleListView", typeof(TreeAuditRuleListViewModel));
            Register("TreeAuditRuleEditView", typeof(TreeAuditRuleEditViewModel));
            Register("SubpopulationListView", typeof(SubpopulationListViewModel));
            Register("StratumTreeFieldSetupView", typeof(StratumTreeFieldSetupViewModel));
            Register("StratumLogFieldSetupView", typeof(StratumLogFieldSetupViewModel));
            Register("TreeErrorEditView", typeof(TreeErrorEditViewModel));
        }

        public Type GetViewModel(Type view)
        {
            var vmType = GetRegisteredViewModel(view);
            if (vmType != null) return vmType;

            return GetViewModelByConvention(view);
        }

        protected Type GetViewModelByConvention(Type view)
        {
            var viewName = view.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");

            string viewAssemblyName = null;
            if (viewName.StartsWith("NatCruise.Design"))
            {
                viewAssemblyName = "NatCruise.Design";
            }
            else if (viewName.StartsWith("FSCruiser.Design"))
            {
                viewAssemblyName = "NatCruise.Design";
            }
            else
            {
                viewAssemblyName = view.GetTypeInfo().Assembly.FullName;
            }

            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);

            var type = Type.GetType(viewModelName);
            return type;
        }

        public Type GetRegisteredViewModel(Type view)
        {
            var viewName = view.Name;

            if (_vmTypeMap.ContainsKey(viewName))
            {
                return _vmTypeMap[viewName];
            }
            return null;
        }

        public void Register(string viewTypeName, Type viewModelType)
        {
            _vmTypeMap[viewTypeName] = viewModelType;
        }
    }
}
