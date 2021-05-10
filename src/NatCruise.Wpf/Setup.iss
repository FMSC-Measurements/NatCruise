#define APP "National Cruise System"
#define EXEName "NatCruise.Wpf.exe"
#define MsBuildOutputDir ".\bin\Release\net472"
#define VERSION "0.28"
#define BASEURL "https://www.fs.fed.us/forestmanagement/products/measurement"
#define ORGANIZATION "U.S. Forest Service, Forest Management Service Center"

[Setup]
;value displayed throught the setup. 
;used as default value for AppId, VersionInfoDescription and VersionInfoProductName 
AppName={#APP}
;used to prevent user from installing/uninstalling while app is running
;requires app code to create a mutex while program is running
AppMutex=CruiseManager
;not displayed in ui. used for uninstall registry key and where install detects previouse install settings
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
;this is less effecent if some files don't need to be installed 
SolidCompression=yes
;notifys windows that file associations have changed when installer exits
ChangesAssociations=yes

;dont allow program to be installed on network drives
AllowUNCPath=no
AllowNetworkDrive=no

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

OutputBaseFilename=NatCruise_Setup

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons};
;Name: deployV2Templates; Description: "V2 Templates"; GroupDescription: "Template Files"; Flags: checkablealone
;Name: deployV2Templates/overwriteV2Templates; Description: "Recopy existing template files"; GroupDescription: "Template Files"; Flags: unchecked dontinheritcheck;
;Name: associateV3CruiseFileTypes; Description: "Associate V3 Cruise (.crz3) Files with NatCruise"; GroupDescription: "Associate File Types";  
;Name: associateV2CruiseFileTypes; Description: "Associate V2 Cruise (.cruise) Files with NatCruise"; GroupDescription: "Associate File Types"; Flags: unchecked
;Name: associateCutFileTypes; Description: "Associate V2 Template (.cut) Files with Cruise Manager"; GroupDescription: "Associate File Types"; Flags: unchecked


[Files]
; need to update the paths below after the solution files and folders are updated.
Source: "{#MsBuildOutputDir}\*.exe"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.exe.config"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.dll"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x64\native\*.dll"; DestDir: {app}\runtimes\win-x64\native; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x86\native\*.dll"; DestDir: {app}\runtimes\win-x86\native; Flags: ignoreversion;
;Source: "..\..\Template Files\V2\*.cut"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV2Templates
;Source: "..\..\Template Files\V3\*.cut"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV3Templates

[Icons]
Name: {group}\{#APP}; Filename: {app}\{#EXEName}
Name: {autodesktop}\{#APP}; Filename: {app}\{#EXEName}; Tasks: desktopicon

[Run]
Filename: "{app}\{#EXEName}"; Description: "{cm:LaunchProgram,{#APP}}"; Flags: nowait postinstall skipifsilent