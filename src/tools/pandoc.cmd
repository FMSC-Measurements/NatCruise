::Boilderplate
::@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

::name of this script
SET me=%~n0
::directory of script
SET parent=%~dp0

ECHO %me%

set pandocPath="%localappdata%\Pandoc\pandoc.exe"

if not exist %pandocPath% (
	set pandocPath="%userprofile%\bin\Pandoc\pandoc.exe"
)
if not exist %pandocPath% (
	set pandocPath="%parent%\pandoc.exe"
)

if exist %pandocPath% (
  %pandocPath% %*
)