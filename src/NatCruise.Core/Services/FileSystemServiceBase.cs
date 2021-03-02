using System.IO;

namespace NatCruise.Services
{
    public abstract class FileSystemServiceBase : IFileSystemService
    {
        public static string CRUISE_DATABASE_FILENAME = "CruiseDatabase.crz3";

        public abstract string AppDataDirectory { get; }

        public string DefaultCruiseDatabasePath => Path.Combine(AppDataDirectory, CRUISE_DATABASE_FILENAME);

        public abstract string ConvertTempDir { get; }

        public abstract string ImportTempDir { get; }

        public abstract string ExportTempDir { get; }

        public abstract string GetFileForConvert(string path);

        public abstract string GetFileForImport(string location);
    }
}