using System.Collections.Generic;

namespace NatCruise.Data
{
    public interface IRecentFilesDataservice
    {
        IEnumerable<string> GetRecentFiles();

        void AddRecentFile(string filePath);
    }
}