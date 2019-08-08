using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public interface IRecentFilesDataservice
    {
        IEnumerable<FileInfo> GetRecentFiles();

        void AddRecentFile(string path);
    }
}
