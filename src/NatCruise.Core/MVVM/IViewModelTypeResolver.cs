using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.MVVM
{
    public interface IViewModelTypeResolver
    {
        Type GetViewModelType(Type view);

        void Register(string viewTypeName, Type viewModelType);
    }
}
