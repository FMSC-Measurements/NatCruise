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

IF NOT DEFINED fscruiser_ks_password SET /p fscruiser_ks_password="Enter FScruiser Key Store Password "

:: msbuild xamarin doc https://docs.microsoft.com/en-us/xamarin/android/deploy-test/building-apps/

call %msbuild% -restore "%parent%FScruiser.Droid\FScruiser.Droid.csproj" ^
					-t:SignAndroidPackage ^
					-p:Configuration=%build_config% ^
					-p:AndroidKeyStore=True ^
					-p:AndroidSigningKeyStore="%parent%FScruiser.Droid\FMSC.keystore" ^
					-p:AndroidSigningStorePass=env:fscruiser_ks_password ^
					-p:AndroidSigningKeyAlias=FMSC ^
					-p:AndroidSigningKeyPass=env:fscruiser_ks_password

SET sign_output=%parent%FScruiser.Droid\bin\Release\com.FMSC.FScruiser-Signed.apk

IF NOT DEFINED dateCode (SET dateCode=%date:~10,4%_%date:~4,2%_%date:~7,2%)
IF NOT DEFINED artifactsDir (SET artifactsDir=../Artifacts/%dateCode%/)

SET apk_output_path=%artifactsDir%com.FMSC.FScruiser-Signed.apk
					
copy "%sign_output%" "%apk_output_path%"
					

::End Boilderplate
::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0