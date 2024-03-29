﻿using NatCruise.Data;
using NatCruise.Wpf.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NatCruise.Wpf.Services
{
    public class RecentFilesDataservice : IRecentFilesDataservice
    {
        public RecentFilesDataservice()
        {
            Settings = Settings.Default;
        }

        private Settings Settings { get; }

        public void AddRecentFile(string filePath)
        {
            filePath = Path.GetFullPath(filePath);

            var recentFiles = GetRecentFiles().Select(x => Path.GetFullPath(x)).ToList();
            recentFiles.RemoveAll(x => string.Equals(x, filePath, StringComparison.OrdinalIgnoreCase));
            recentFiles.Add(filePath);

            Settings.RecentFiles = String.Join(",", recentFiles.ToArray());
            Settings.Save();
        }

        public IEnumerable<string> GetRecentFiles()
        {
            return Settings.RecentFiles
                ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => System.IO.File.Exists(x))
                ?? Enumerable.Empty<string>();
        }
    }
}