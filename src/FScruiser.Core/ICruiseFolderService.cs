using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser
{
    public interface ICruiseFolderService
    {
        IEnumerable<string> CruiseFolders { get; }
    }
}