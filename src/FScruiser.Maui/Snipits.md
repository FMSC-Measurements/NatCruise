# UI Snippits

example of standard xmlns used
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"

             x:Class="FScruiser.Maui.Views.SomeView"
             x:Name="_page"

            -- community toolkit --
             xmlns:mct="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             xmlns:tc="clr-namespace:CommunityToolkit.Maui;assembly=CommunityToolkit.Maui"

             -- frequently used namespaces --
             xmlns:local="clr-namespace:FScruiser.Maui"
             xmlns:bhvrs="clr-namespace:FScruiser.Maui.Behaviors" 
             xmlns:ctrls="clr-namespace:FScruiser.Maui.Controls"

             -- accessibility --
             SemanticProperties.Description="Settings Page"

             -- for ViewModelLocator --
             xmlns:mvvm="clr-namespace:FScruiser.Maui.MVVM"
             xmlns:vms="clr-namespace:FScruiser.Maui.ViewModels"

             BindingContext="{mvvm:ViewModelLocater Type={Type vms:SomeViewModel}}">
    <ContentPage.Content>
       <!-- ... -->


        <Entry Text={Binding ...}>
             <Entry.Behaviors>
                <mct:SelectAllTextBehavior />
                <bhvrs:SelectNextOnCompleatedBehavior />
            </Entry.Behaviors>

            <Entry.Behaviors>
                <mct:SelectAllTextBehavior />
            </Entry.Behaviors>
</Entry>
    </ContentPage.Content>
</ContentPage>
```

