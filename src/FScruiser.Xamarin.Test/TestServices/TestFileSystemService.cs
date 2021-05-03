using NatCruise.Services;
using System;
using System.IO;

namespace FScruiser.XF.TestServices
{
    public class TestFileSystemService : IFileSystemService
    {
        private string _testTempPath;

        public string AppDataDirectory => "";

        public string DefaultCruiseDatabasePath => Path.Combine(TestTempPath, "CruiseDatabase.crz3");

        public string ConvertTempDir => "";

        public string TestTempPath
        {
            get
            {
                return _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
            }
        }

        public string ImportTempDir => "";

        public string ExportTempDir => "";

        public string GetFileForImport(string location)
        {
            return "";
        }

        public string GetFileForConvert(string path)
        {
            return "";
        }

        public void CopyTo(string from, string to)
        {
            throw new NotImplementedException();
        }
    }
}