namespace NatCruise.Services
{
    public interface IFileSystemService
    {
        string AppDataDirectory { get; }

        string DefaultCruiseDatabasePath { get; }

        string ConvertTempDir { get; }

        string ImportTempDir { get; }

        string ExportTempDir { get; }

        string GetFileForImport(string location);

        string GetFileForConvert(string path);

        void CopyTo(string from, string to);
    }
}