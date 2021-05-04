using Android.App;
using Android.Content;
using NatCruise.Services;
using System.IO;

namespace FScruiser.Droid.Services
{
    public class AndroidFileSystemService : FileSystemServiceBase
    {
        public AndroidFileSystemService(Context context)
        {
            Context = context;

            var importTempDir = ImportTempDir;
            var convertTempDir = ConvertTempDir;

            if (Directory.Exists(importTempDir) == false)
            { Directory.CreateDirectory(importTempDir); }

            if (Directory.Exists(convertTempDir) == false)
            { Directory.CreateDirectory(convertTempDir); }

            var exportTempDir = ExportTempDir;
            if (Directory.Exists(exportTempDir) == false)
            { Directory.CreateDirectory(exportTempDir); }
        }

        public Context Context { get; }
        public Activity ParentActivity { get; }

        public override string AppDataDirectory => Context.FilesDir.AbsolutePath;
        public string CacheDir => Context.CacheDir.AbsolutePath;

        public override string ImportTempDir => Path.Combine(CacheDir, "ImportTemp");
        public override string ConvertTempDir => Path.Combine(CacheDir, "ConvertTemp");
        public override string ExportTempDir => Path.Combine(CacheDir, "ExportTemp");

        public override string GetFileForImport(string location)
        {
            var fileName = Path.GetFileName(location);
            var destinationPath = Path.Combine(ImportTempDir, fileName);
            if (IsInternalPath(location))
            {
                File.Copy(location, destinationPath, true);
            }
            else
            {
                File.Copy(location, destinationPath, true);
            }

            return destinationPath;
        }

        public override string GetFileForConvert(string path)
        {
            var fileName = Path.GetFileName(path);
            var destinationPath = Path.Combine(ConvertTempDir, fileName);

            File.Copy(path, destinationPath, overwrite: true);
            return destinationPath;
        }

        public override void CopyTo(string from, string to)
        {
            var resolver = Context.ContentResolver;

            var aTo = Android.Net.Uri.Parse(to);
            using var stream = resolver.OpenOutputStream(aTo);
            using var fromStream = File.OpenRead(from);
            fromStream.CopyTo(stream);
        }

        private bool IsInternalPath(string path)
        {
            // TODO detect if file is in convert temp dir
            return false;
        }
    }
}