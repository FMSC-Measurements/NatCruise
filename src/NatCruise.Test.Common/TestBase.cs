using Bogus;
using CruiseDAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace NatCruise.Test
{
    public class TestBase
    {
        protected ITestOutputHelper Output { get; }
        protected DbProviderFactory DbProvider { get; private set; }
        protected Randomizer Rand { get; } = new Randomizer(123456);
        protected Stopwatch _stopwatch;
        private string _testTempPath;

        List<string> FilesToBeDeleted { get; } = new List<string>();

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Output.WriteLine($"CodeBase: {System.Reflection.Assembly.GetExecutingAssembly().CodeBase}");
            var testTempPath = TestTempPath;
            if (!Directory.Exists(testTempPath))
            {
                Directory.CreateDirectory(testTempPath);
            }
        }

        ~TestBase()
        {
            foreach (var file in FilesToBeDeleted)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // do nothing
                }
            }
        }

        public string TestExecutionDirectory
        {
            get
            {
                var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                return Path.GetDirectoryName(codeBase);
            }
        }

        public string TestTempPath => _testTempPath ??= Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName);

        public string TestFilesDirectory => Path.Combine(TestExecutionDirectory, "TestFiles");

        public void StartTimer()
        {
            _stopwatch = new Stopwatch();
            Output.WriteLine("Stopwatch Started");
            _stopwatch.Start();
        }

        public void EndTimer()
        {
            _stopwatch.Stop();
            Output.WriteLine("Stopwatch Ended:" + _stopwatch.ElapsedMilliseconds.ToString() + "ms");
        }

        public void DumpDatabaseInfo(CruiseDatastore_V3 ds, params string[] tables)
        {
            Output.WriteLine($"DAL Version: {ds.DatabaseVersion}");

            foreach (var table in tables)
            {
                var tableSql = ds.GetTableSQL(table);
                Output.WriteLine(tableSql);
            }
        }

        public string GetTempFilePath(string extention, string fileName = null)
        {
            return Path.Combine(TestTempPath, (fileName ?? Guid.NewGuid().ToString()) + extention);
        }

        public string GetTestFile(string fileName) => InitializeTestFile(fileName);

        protected string InitializeTestFile(string fileName)
        {
            var sourcePath = Path.Combine(TestFilesDirectory, fileName);
            if (File.Exists(sourcePath) == false) { throw new FileNotFoundException(sourcePath); }

            var targetPath = Path.Combine(TestTempPath, fileName);

            RegesterFileForCleanUp(targetPath);
            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }

        public void RegesterFileForCleanUp(string path)
        {
            FilesToBeDeleted.Add(path);
        }
    }
}