using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Util
{
    public static class IContainerExtensionExtensions
    {
        public static T ResolveOrDefault<T>(this IContainerExtension @this)
        {
            try
            {
                return @this.Resolve<T>();
            }
            catch
            {
                return default(T);
            }
        }
    }
}
