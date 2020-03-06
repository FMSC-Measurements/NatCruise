using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public interface IFileDialogService
    {
        string SelectCruiseFile();

        string SelectCruiseFileDestination(string defaultDir = null, string defaultFileName = null);
    }
}
