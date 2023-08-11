using Microsoft.Win32;
using NatCruise.Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NatCruise.Wpf.Services
{
    public class FileAssociationService
    {
        public const string REGKEY_APPPATH = "Software\\Microsoft\\Windows\\CurrentVersion\\App Paths";
        public const string REGKEY_CLASSES = "Software\\Classes";
        public const string REGKEY_APPLICATIONS = "Software\\Classes\\Applications";
        public const string SUBKEY_APPLICATIONS = "Applications";
        public const string SUBKEY_OPENWITHPROGIDS = "OpenWithProgids";
        private string _openCommand;

        public string ApplicationFullPath { get; }
        public string ProgramID { get; }

        public IEnumerable<AssociatedFileTypeInfo> FileTypes { get; }

        public string ExecutalbleName { get; }

        public string OpenCommand => _openCommand ??= $"{ExecutalbleName} %1";

        public bool IsAppRegistered { get; }

        public FileAssociationService(string progID, IEnumerable<AssociatedFileTypeInfo> fileTypes)
        {
            ApplicationFullPath = Assembly.GetEntryAssembly().Location;
            ExecutalbleName = Path.GetFileName(ApplicationFullPath);
            IsAppRegistered = CheckIsAppRegistered();

            ProgramID = progID ?? throw new ArgumentNullException(nameof(progID));
            FileTypes = fileTypes ?? throw new ArgumentNullException(nameof(fileTypes));
        }

        public bool CheckIsAppRegistered()
        {
            var exeName = ExecutalbleName;

            using var hlcu_appPaths = Registry.CurrentUser.OpenSubKey(REGKEY_APPPATH + "\\" + exeName, false);
            if (hlcu_appPaths != null
                && hlcu_appPaths.GetValue(null).Equals(ApplicationFullPath))
            { return true; }

            using var hkcr_applications = Registry.ClassesRoot.OpenSubKey(SUBKEY_APPLICATIONS + "\\" + exeName, false);
            if (hkcr_applications != null) { return true; }

            using var hklm_appPaths = Registry.LocalMachine.OpenSubKey(REGKEY_APPPATH + "\\" + exeName, false);
            if (hklm_appPaths != null
                && hklm_appPaths.GetValue(null).Equals(ApplicationFullPath))
            { return true; }

            return false;
        }

        public void RegisterApp()
        {
            RegisterApp(ApplicationFullPath);
        }

        public Task RegisterAppAsync()
        {
            return Task.Factory.StartNew(() => RegisterApp(ApplicationFullPath));
        }

        public static void RegisterApp(string fullAppExePath)
        {
            var exeName = Path.GetFileName(fullAppExePath);
            if (!exeName.EndsWith(".exe")) throw new ArgumentException("Expected fullExePath to point to file with extension .exe");

            using var hkcu_appPaths = Registry.CurrentUser.OpenSubKey(REGKEY_APPPATH, true);

            using var appKey = hkcu_appPaths.CreateSubKey(exeName, true);
            appKey.SetValue((string)null, fullAppExePath);
        }

        public bool CheckIsFileTypeRegistered(AssociatedFileTypeInfo filetype)
        {
            if(!CheckIsExtentionRegistered(filetype)) return false;

            return CheckIsFileTypeLabelRegistered(filetype);
        }

        internal bool CheckIsExtentionRegistered(AssociatedFileTypeInfo filetype)
        {
            var extention = filetype.Extension;
            if (!extention.StartsWith(".")) throw new InvalidOperationException("expected AssociatedFileTypeInfo.Extension to start with '.'");


            using var hkcr_exe = Registry.ClassesRoot.OpenSubKey(extention, false);
            if (hkcr_exe is null) return false;

            using var openwithProgIDs = hkcr_exe.OpenSubKey("OpenWithProgieds");
            if (openwithProgIDs is null) return false;

            return openwithProgIDs.GetValue(filetype.Label) != null;
        }

        internal bool CheckIsFileTypeLabelRegistered(AssociatedFileTypeInfo filetype)
        {
            var classFileTypeLabel = Registry.ClassesRoot.OpenSubKey(filetype.Label, false);
            if (classFileTypeLabel is null) return false;

            using var command = classFileTypeLabel.OpenSubKey("shell\\open\\command");
            if(command is null) return false;
            if (command.GetValue(null) is null) return false;
            return command.GetValue(null).Equals(OpenCommand);
        }

        public void Register()
        {
            RegisterApp(ApplicationFullPath);

            foreach(var filetype in FileTypes)
            {
                RegisterFileType(filetype);
            }
        }

        public IEnumerable<AssociatedFileTypeInfo> CheckUnRegisteredFileTypes()
        {
            foreach (var filetype in FileTypes)
            {
                if (!CheckIsExtentionRegistered(filetype)
                    || !CheckIsFileTypeLabelRegistered(filetype))
                {
                    yield return filetype;
                }
            }
        }



        public void RegisterFileType(AssociatedFileTypeInfo filetype)
        {
            RegisterFileTypeLabel(filetype);

            RegisterExtension(filetype);
        }

        internal void RegisterFileTypeLabel(AssociatedFileTypeInfo filetype)
        {
            var fileTypeClassPath = $"{REGKEY_CLASSES}\\{filetype.Label}";
            using var classFileTypeLabel = Registry.CurrentUser.CreateSubKey(fileTypeClassPath, true);

            using var command = classFileTypeLabel.CreateSubKey("shell\\open\\command", true);
            command.SetValue(null, OpenCommand);
        }

        internal void RegisterExtension(AssociatedFileTypeInfo filetype)
        {
            var extention = filetype.Extension;
            if (!extention.StartsWith(".")) throw new InvalidOperationException("expected AssociatedFileTypeInfo.Extension to start with '.'");

            using var hkcr_exe = Registry.CurrentUser.CreateSubKey($"{REGKEY_CLASSES}\\{extention}", true);

            using var openwithProgIDs = hkcr_exe.CreateSubKey("OpenWithProgids");
            openwithProgIDs.SetValue(filetype.Label, "");
        }
    }
}
