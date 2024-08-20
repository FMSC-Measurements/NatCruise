#define MsBuildOutputDir ".\bin\Release\net472"
#define SolutionDir ".."
#define VERSION "3.1.14"

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
Name: deployV2Templates; Description: "V2 Templates"; GroupDescription: "Template Files"; Flags: checkablealone
;Name: deployV3Templates; Description: "V3 Templates"; GroupDescription: "Template Files"; Flags: checkablealone


[Files]
; need to update the paths below after the solution files and folders are updated.
Source: "{#MsBuildOutputDir}\*.exe"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.dll"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\*.exe.config"; DestDir: {app}; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x64\native\*.dll"; DestDir: {app}\runtimes\win-x64\native; Flags: ignoreversion;
Source: "{#MsBuildOutputDir}\runtimes\win-x86\native\*.dll"; DestDir: {app}\runtimes\win-x86\native; Flags: ignoreversion;
; copy template files to a temporary location in the apps deploy dir. we will delete these after install because 
; we will be copying them to the users documents dir. 
Source: "{#SolutionDir}\Template Files\V2\*.cut"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV2Templates
;Source: "{#SolutionDir}\Template Files\V3\*.crz3t"; DestDir: {app}\Templates; Flags: ignoreversion deleteafterinstall; Tasks: deployV3Templates


[Icons]
Name: {group}\{#APP}; Filename: {app}\{#EXEName}
Name: {autodesktop}\{#APP}; Filename: {app}\{#EXEName}; Tasks: desktopicon

[Run]
Filename: "{app}\{#EXEName}"; Description: "{cm:LaunchProgram,{#APP}}"; Flags: nowait postinstall skipifsilent

[Code]

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Log('ShouldSkipPage(' + IntToStr(PageID) + ') called');
  { Skip wpSelectDir page if admin install; show all others }
  case PageID of
    wpSelectDir:
      Result := IsAdminInstallMode();
  else
    Result := False;
  end;
end;

{ copies files matching pattern from srcDir to destDir }
procedure CopyFiles(srcDir: String; pattern: String; destDir: String; overwrite: Boolean);
var 
  FindRec: TFindRec;
begin
  if ForceDirectories(destDir) then
  begin {iterate files in srcDir and copy them to destDir }
    if FindFirst(srcDir + pattern, FindRec) then
    begin
      try
        repeat
          Log('Copy ' + srcDir + FindRec.Name + ' -> ' + destDir + FindRec.Name);
          if FileCopy(srcDir + FindRec.Name, destDir + FindRec.Name, not overwrite) then
          begin
            Log('File Copied ' + srcDir + FindRec.Name + ' -> ' + destDir + FindRec.Name);
          end else
          begin
            Log('File NOT Copied ' + srcDir + FindRec.Name + ' -> ' + destDir + FindRec.Name);
          end;
        until not FindNext(FindRec);
      finally
        FindClose(FindRec);
      end; { end try }
    end;   { end file copy loop }   
  end else { end if force dir }
  begin
    Log('Error Creating Dir ' + destDir);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  UsersPath: String;
  DocumentsPath: String;
  DesktopPath: String;
  UserTemplatesPath: String;
  CopyTemplateIfExists: Boolean;
  UserDirFindRec: TFindRec;
  TemplateSrcPath: String;
begin
  { Once the files are installed }
  if (CurStep = ssPostInstall) and (WizardIsTaskSelected('deployV2Templates') or WizardIsTaskSelected('deployV2Templates'))  then
  begin
    Log('Copying Templates');
    TemplateSrcPath := ExpandConstant('{app}') + '\Templates\';
    if IsAdminInstallMode() then
    begin
      UsersPath := ExpandConstant('{%HOMEDRIVE|C:}') + '\Users\';
      Log(Format('Users Path [%s]', [UsersPath]));

      { Iterate all users }
      if FindFirst(UsersPath + '*', UserDirFindRec) then
      begin
        try
          repeat  
            { Just directories and ignore Public, All Users, and Default User. All Users and Default User are symbolic links that we don't care about }

            if (UserDirFindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY <> 0) and (UserDirFindRec.Name <> 'Public') and (UserDirFindRec.Name <> 'All Users') and (UserDirFindRec.Name <> 'Default User') then
            begin
              DocumentsPath := UsersPath + UserDirFindRec.Name + '\Documents';
              Log('UserProfile ' + UsersPath + UserDirFindRec.Name);
              if DirExists(DocumentsPath) then
              begin
                UserTemplatesPath := DocumentsPath + '\CruiseFiles\Templates\';
                if WizardIsTaskSelected('deployV2Templates') then begin
                    CopyFiles(TemplateSrcPath, '*.cut', UserTemplatesPath, CopyTemplateIfExists);
                end;
                if WizardIsTaskSelected('deployV3Templates') then begin
                    CopyFiles(TemplateSrcPath, '*.crz3t', UserTemplatesPath, CopyTemplateIfExists);
                end;
              end;       { end if force dir }

              { delete any desktop icons left behind in the user's desktop folder }
              { note we aren't deleting from the Public\Desktop where the All Users desktop icon is located }

              DesktopPath := UsersPath + UserDirFindRec.Name + '\Desktop\';
              if DirExists(DesktopPath) then
              begin
                if DeleteFile(DesktopPath + 'Cruise Manager.lnk') then begin
                  Log('File Deleted ' + DesktopPath + 'Cruise Manager.lnk');
                end else
                  Log('File Not Deleted ' + DesktopPath + 'Cruise Manager.lnk');
              end;       {end if desktop exists }
            end;         { end check user dir exists }     
          until not FindNext(UserDirFindRec);
        finally
          FindClose(UserDirFindRec);
        end;
      end else
      begin
        Log(Format('Error listing User dirs [%s]', [UsersPath]));
      end;
    end else {end is admin mode}
    begin
      CopyFiles(TemplateSrcPath, '*.cut', ExpandConstant('{userdocs}') + '\CruiseFiles\Templates\', CopyTemplateIfExists); 
    end;
    
  end; {end if is templates component selected }
end;