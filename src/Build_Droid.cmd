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

IF NOT DEFINED build_config SET build_config="Release"

:: msbuild xamarin doc https://docs.microsoft.com/en-us/xamarin/android/deploy-test/building-apps/

call %msbuild% -restore "%parent%FScruiser.Droid\FScruiser.Droid.csproj" ^
					-p:Configuration=%build_config% 

IF "%errorlevel%" EQU "0" (
ECHO Build compleated created successfully.
) ELSE (
ECHO Error while building project.
EXIT /B
)