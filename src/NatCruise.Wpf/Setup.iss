#define MsBuildOutputDir ".\bin\Release\net472"
#define SolutionDir ".."
#define VERSION "3.1.12"

#define APP "National Cruise System"
#define EXEName "NatCruise.Wpf.exe"
#define BASEURL "https://www.fs.fed.us/forestmanagement/products/measurement"
#define ORGANIZATION "U.S. Forest Service, Forest Management Service Center"

[Setup]
;value displayed throught the setup. 
;used as default value for AppId, VersionInfoDescription and VersionInfoProductName 
AppName={#APP}
;used to prevent user from installing/uninstalling while app is running
;requires app code to create a mutex while program is running
AppMutex=CruiseManager
;not displayed in ui. used for uninstall registry key and where install detects previous install settings
;defaults to AppName
AppID={#APP}
;used as default value for AppVerName. displayed in version field of app's add/remove entry
;used to provide major/minor version values in registry entry
AppVersion={#VERSION}
;specifies the file version on the setup exe
VersionInfoVersion={#VERSION}

AppPublisher={#ORGANIZATION}
AppPublisherURL={#BASEURL}
AppSupportURL={#BASEURL}/support.shtml
AppUpdatesURL={#BASEURL}/cruising/index.shtml
LicenseFile=..\..\LICENSE.md

DefaultDirName={autopf}\FMSC\{#APP}
DefaultGroupName=FMSC\{#APP}

Compression=lzma
;causes all files to be compressed together
;this is less efficient if some files don't need to be installed 
SolidCompression=yes
;notifies windows that file associations have changed when installer exits
ChangesAssociations=yes

;dont allow program to be installed on network drives
AllowUNCPath=no
AllowNetworkDrive=no

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

OutputBaseFilename=NatCruise_Setup_{#VERSION}

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons};
;Name: associateV3CruiseFileTypes; Description: "Associate V3 Cruise (.crz3) Files with NatCruise"; GroupDescription: "Associate File Types";  
;Name: deployV2Templates; Description: "V2 Templates"; GroupDescription: "Template Files"; Flags: checkablealone
;Name: deployV2Templates/overwriteV2Templates; Description: "Recopy existing template files"; GroupDescription: "Template Files"; Flags: unchecked dontinheritcheck;

;Name: associateV2CruiseFileTypes; Description: "Associate V2 Cruise (.cruise) Files with NatCruise"; GroupDescription: "Associate File Types"; Flags: unchecked
;Name: associateCutFileTypes; Description: "Associate V2 Template (.cut) Files with Cruise Manager"; GroupDescription: "Associate File Types"; Flags: unchecked


[Files]
; need to update the paths below after the solution files and folders are updated.
Source: "{#MsBuildOutputDir}\*.exe"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.dll"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.exe.config"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x64\native\*.dll"; DestDir: {app}\runtimes\win-x64\native; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x86\native\*.dll"; DestDir: {app}\runtimes\win-x86\native; Flags: ignoreversion;
;Source: "..\..\Template Files\V2\*.cut"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV2Templates
;Source: "..\..\Template Files\V3\*.cut"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV3Templates

[InstallDelete]
;clean any files from the net 6.0 release
Type: files; Name: "{app}\*.exe"
Type: files; Name: "{app}\*.dll"
Type: files; Name: "{app}\NatCruise.Wpf.dll.config"


[Icons]
Name: {group}\{#APP}; Filename: {app}\{#EXEName}
Name: {autodesktop}\{#APP}; Filename: {app}\{#EXEName}; Tasks: desktopicon

[Run]
Filename: "{app}\{#EXEName}"; Description: "{cm:LaunchProgram,{#APP}}"; Flags: nowait postinstall skipifsilent

[Registry]

;Root: HKLM; Subkey: SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{#EXEName}; ValueType: none; Flags: deletekey noerror; Tasks: associateV3CruiseFileTypes;

;Root: HKA; Subkey: "Software\Classes\Applications\{#EXEName}\SupportedTypes"; ValueType: string; ValueName: ".crz3"; ValueData: ""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\Applications\{#EXEName}\SupportedTypes"; ValueType: string; ValueName: ".crz3t"; ValueData: ""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;

;; NCS.CruiseFileV3 is the internal unique name for the file type assocation for the .crz3 extention
;Root: HKA; Subkey: "Software\Classes\.crz3"; ValueType: string; ValueName: ""; ValueData: "NCS.CruiseFileV3"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\.crz3\OpenWithProgids"; ValueType: string; ValueName: "NCS.CruiseFileV3"; ValueData: ""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\NCS.CruiseFileV3"; ValueType: string; ValueName: ""; ValueData: "Cruise File V3"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;; the ',0' in ValueData tells Explorer to use the first icon from NatCruise.Wpf.exe
;Root: HKA; Subkey: "Software\Classes\NCS.CruiseFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#EXEName},0"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\NCS.CruiseFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#EXEName}"" ""%1"""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;

;; register cruise template files
;Root: HKA; Subkey: "Software\Classes\.crz3t"; ValueType: string; ValueName: ""; ValueData: "NCS.TemplateFileV3"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\.crz3t\OpenWithProgids"; ValueType: string; ValueName: "NCS.TemplateFileV3"; ValueData: ""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\NCS.TemplateFile"; ValueType: string; ValueName: ""; ValueData: "Cruise Template File V3"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\NCS.TemplateFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#EXEName},0"; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;
;Root: HKA; Subkey: "Software\Classes\NCS.TemplateFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#EXEName}"" ""%1"""; Flags: uninsdeletevalue; Tasks: associateV3CruiseFileTypes;