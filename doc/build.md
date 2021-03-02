# AppCenter 


# environment variables
The application build process makes use of environment variables on the developers machine in inject constants into the source code. Using [Scripty](./Dependencies.md/#Scripty.MsBuild), the script file FScruiser.Xamarin/Secrets.local.csx will run prior to the build generating the Secrets.local.cs file. Secrets.local.csx copies the environment variables 
The following environment variables are used
 - `natcruise_appcenterr_key_windows`
 - `fscruiser_appcenter_key_droid`
 - `fscruiser_appcenter_key_uwp`
 - `fscruiser_appcenter_key_ios`

