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

::IF NOT DEFINED build_config SET build_config="Release"
dotnet restore %parent%NatCruiseDesktop.slnf 
dotnet build %parent%NatCruiseDesktop.slnf && dotnet-coverage collect "dotnet test %parent%NatCruiseDesktop.slnf" -f xml -o "coverage.xml


IF /I "%ERRORLEVEL%" NEQ "0" (
ECHO build failed
IF "%interactive%"=="0" PAUSE
EXIT /B 1
)


::End Boilderplate
::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0