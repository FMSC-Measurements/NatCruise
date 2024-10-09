::Boilderplate 
::@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

::detect if invoked via Window Explorer
SET interactive=1
ECHO %CMDCMDLINE% | FIND /I "/c" >NUL 2>&1
IF %ERRORLEVEL% == 0 SET interactive=0

::name of this script
SET me=%~n0
::directory of script
SET parent=%~dp0
::::::::::::::::::

SET msbuild="%parent%tools\msbuild.cmd"

IF NOT DEFINED build_config SET build_config=Release
IF NOT DEFINED framework_target SET framework_target=net8.0-android

::IF NOT DEFINED fscruiser_uploadks_password SET /p fscruiser_uploadks_password="Enter FScruiser Upload Key Store Password "

:: dotnet pack maui documentation https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-8.0

dotnet publish "%parent%FScruiser.Maui\FScruiser.Maui.csproj" --framework %framework_target% --configuration %build_config% 


IF %errorlevel% NEQ 0 GOTO ERROR_EXIT

::SET signed_aab_output=%parent%FScruiser.Droid\bin\Release\net8.0-android\publish\gov.usda.fs.fscruiser-Signed.aab
SET unsigned_aab_output=%parent%FScruiser.Maui\bin\Release\net8.0-android\publish\gov.usda.fs.fscruiser.aab

IF NOT DEFINED dateCode (SET dateCode=%date:~10,4%_%date:~4,2%_%date:~7,2%)
IF NOT DEFINED artifactsDir (SET artifactsDir=%parent%..\Artifacts\%dateCode%\)

IF NOT EXIST "%artifactsDir%" (
	MKDIR "%artifactsDir%"
	IF "!errorlevel!" EQU "0" (
		ECHO Folder created successfully.
	) ELSE (
		ECHO Error while creating folder.
	)
) ELSE (
	ECHO Folder already exists.
)

::SET signed_aab_artifact_path=%artifactsDir%gov.usda.fs.fscruiser-Signed.aab
SET unsigned_aab_artifact_path=%artifactsDir%gov.usda.fs.fscruiser.aab
					
::copy "%signed_aab_output%" "%signed_aab_artifact_path%"
copy "%unsigned_aab_output%" "%unsigned_aab_artifact_path%"

::End Boilderplate
::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0

:ERROR_EXIT
ECHO ERROR: %errorlevel%
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B %errorlevel% 