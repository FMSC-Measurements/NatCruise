# Fixes
list issues fixed

# Enhancments
list enhancments

# Changes
list changes

# Release Checklist
 - [ ] Run All Tests
 - [ ] Update _**Version**_ property in `/src/Directory.Build.props` file
 - [ ] Update _**android:versionName**_ and _**android:versionCode**_ attributes in `/src/FScruiser.Droid/Properties/AndroidManifest.xml`
 - [ ] Update _**AssemblyVersion**_ and _**AssemblyFileVersion**_ in `/src/FScruiser.Droid/Properties/AssemblyInfo.cs`
 - [ ] Update _**VERSION**_ value in `/src/NatCruise.Wpf/Setup.iss`
 - [ ] Run `/src/Build_Installer_PC.bat` and confirm output file exists `/Artifacts/[date code]/NatCruise_Setup_[version].exe`
 - [ ] Run `Build_ZipArtifact.cmd` and confirm output file exists `/Artifacts/[date code]/NatCruise.zip`
 - [ ] Run `Pack_FScruiser_APK.cmd` and confirm output file exits `/Artifacts/[date code]/com.FMSC.FScruiser-Signed.apk`
 - [ ] Run `Pack_FScruiser_AAB.cmd` and confirm output file exits `/Artifacts/[date code]/com.FMSC.FScruiser-Signed.aab`
