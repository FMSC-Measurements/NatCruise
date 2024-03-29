@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

::Boilderplate 
::detect if invoked via Window Explorer
SET interactive=1
ECHO %CMDCMDLINE% | FIND /I "/c" >NUL 2>&1
IF %ERRORLEVEL% == 0 SET interactive=0

::name of this script
SET me=%~n0
::directory of script
SET parent=%~dp0

SET pandoc="%parent%pandoc.cmd"

call %pandoc% "%parent%NatCruiseInterimUserGuide_WorkInProgress.docx" -t gfm -o "%parent%NatCruiseUserGuide.md" --extract-media "."

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0
