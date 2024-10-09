# FScruiser.Maui

## Application Icon and Splash Screen
`FScruiser.Maui.csproj` contains the following properties to set the application icon and splash screen.
```xml
  <PropertyGroup>
    <MauiSplashScreenImage>Resources\SplashScreen.png</MauiSplashScreenImage>
    <MauiIconImage>Resources\icon.png</MauiIconImage>
  </PropertyGroup>
```
These automatically set values in the Android resources and default themes that define the icon and splash screen for the application.

The appIcon consists of two layered components: the primary component which in our case is just a solid color svg, 
and the forground component which is the actual logo. This makes use of Android's adaptive icon feature.
The name of the ressource referenced in `AndroidManifest.xml` takes its name from the primary component of the appIcon.
For best results setting up the app icon I recommend using [https://icon.kitchen/](https://icon.kitchen/)

## Build
### Prerequisites
Before building a dotnet Maui app you need to install the required workflows. 
This can be done using the Visual Studio installer and selecting the **.NET Multi-platform App UI development** 
item in the Workloads tab.

Another option is to run the command
`dotnet workload restore` from the FScruise.Maui project directory




# MVVM 
 ## ViewModelBase



## ViewModelLocatorExtension - no longer used
This markup extension helps with automatically providing ViewModels for views while they are loading.
To use set the binding context of the page and provide the type of the view model you wish to use.

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"

             xmlns:mvvm="clr-namespace:FScruiser.Maui.MVVM"
             xmlns:vms="clr-namespace:FScruiser.Maui.ViewModels"

             BindingContext="{mvvm:ViewModelLocater Type={Type vms:SomeViewModel}}">
 <!-- ... -->
</ContentPage>
```

If the view model is based on ViewModelBase. ViewModelLocatorExtention will wire up the 