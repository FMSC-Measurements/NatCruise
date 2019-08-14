using NatCruise.Wpf.Properties;
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
            Settings = Settings.Default;
        }

        Settings Settings { get; }

        public void AddRecentFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath);

            var recentFiles = GetRecentFiles().Select(x => Path.GetFullPath(x));
            recentFiles = recentFiles.Union(new[] { filePath });
            Settings.RecentFiles = String.Join(",", recentFiles.ToArray());
            Settings.Save();
        }

        public IEnumerable<string> GetRecentFiles()
        {
            return Settings.RecentFiles
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?? Enumerable.Empty<string>();
        }
    }
}
