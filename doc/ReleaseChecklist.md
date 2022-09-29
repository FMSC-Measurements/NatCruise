
# NatCruise
 - update version in ./src/Directory.Build.props
 - update version in ./src/NatCruise.WPF/Setup.iss
 - Run Build_Installer_PC.bat
 - Run Build_ZipArtifact.cmd
 - rename NatCruise_Setup.exe to indicate version e.x. NatCruise_Setup_v3.0.0.exe
 - Test installer using Windows Sandbox enviroment
 - Copy output from ./Artifacts dir
 - Post release to GitHub
 - Submit release to EO Release Management

# FScruiser
 - update version in ./src/Directory.Build.props
 - update android:versionName and android:versionCode - see note on adroid:versionCode
 - Run Pack_FScruiser_AAB.cmd - this will build the AAB file for upload to play store
 - Run Pack_FScruiser_APK.cmd - this will build the APK file. This is useful for archiving app versions to be side loaded or as a fall back for users without access to play store. 

 ## android:versionCode
 this is an integer value used by the play store to identify each release. 
 newer releases should always have a higher value from previous releases. 
 as of writing the values in versionCode corespond to [major version:1 digit][minor version:2 digits][patch:2 digits]
