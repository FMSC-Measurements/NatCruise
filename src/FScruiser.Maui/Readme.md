# Markdown File

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