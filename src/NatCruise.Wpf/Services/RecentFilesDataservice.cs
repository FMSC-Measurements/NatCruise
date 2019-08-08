using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class RecentFilesDataservice : IRecentFilesDataservice
    {
        public RecentFilesDataservice()
        {
            LoadRecentFiles();
        }

        protected void LoadRecentFiles()
        {

        }

        public IEnumerable<FileInfo> GetRecentFiles()
        {
            return new FileInfo[0];
        }

        public void AddRecentFile(string path)
        {

        }
    }
}
