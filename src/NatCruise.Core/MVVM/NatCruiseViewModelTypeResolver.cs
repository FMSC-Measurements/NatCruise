﻿using NatCruise.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NatCruise.MVVM
{
    public class NatCruiseViewModelTypeResolver : IViewModelTypeResolver
    {
        private Dictionary<string, Type> _vmTypeMap = new Dictionary<string, Type>();

        public NatCruiseViewModelTypeResolver()
        {
            Register("TreeCountEditView", typeof(TreeCountEditViewModel));
            Register("TreeEditView", typeof(TreeEditViewModel));
            Register("LogEditView", typeof(LogEditViewModel));
            Register("TreeAuditRuleListView", typeof(TreeAuditRuleListViewModel));
            Register("TreeAuditRuleEditView", typeof(TreeAuditRuleEditViewModel));
            Register("SubpopulationListView", typeof(SubpopulationListViewModel));
            Register("StratumInfoView", typeof(StratumInfoViewModel));
            Register("StratumTreeFieldSetupView", typeof(StratumTreeFieldSetupViewModel));
            Register("StratumLogFieldSetupView", typeof(StratumLogFieldSetupViewModel));
            Register("TreeErrorEditView", typeof(TreeErrorEditViewModel));
            Register("TallyPopulationDetailsView", typeof(TallyPopulationDetailsViewModel));
            Register("AboutView", typeof(AboutViewModel));
        }

        public Type GetViewModelType(Type view)
        {
            var vmType = GetRegisteredViewModel(view);
            if (vmType != null) return vmType;

            return GetViewModelByConvention(view);
        }

        protected Type GetViewModelByConvention(Type view)
        {
            var viewName = view.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");

            string viewAssemblyName = view.GetTypeInfo().Assembly.FullName;

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