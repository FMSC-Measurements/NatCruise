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

The appIcon consists of two layered components: the primary component which in our case is just a a solid color svg, 
and the forground component which is the actual logo. This makes use of Android's adaptive icon feature.


# MVVM 
## ViewModelLocatorExtension
This markup extension helps with automaticly providing ViewModels for views while they are loading.
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