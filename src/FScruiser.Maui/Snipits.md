# UI Snippits

example of standard xmlns used
```xml
<ctrls:BasePage 
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"

             

             xmlns:tk="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             xmlns:bp_ctrls="clr-namespace:Backpack.Maui.Controls;assembly=Backpack.Maui"
             xmlns:const="clr-namespace:FScruiser.Maui.Constants"

             xmlns:local="clr-namespace:FScruiser.Maui"
             xmlns:bhvrs="clr-namespace:FScruiser.Maui.Behaviors" 
             xmlns:ctrls="clr-namespace:FScruiser.Maui.Controls"


             x:Class="FScruiser.Maui.Views.SomeView"
             x:Name="_page"
             SemanticProperties.Description="Some Page">
    <ContentPage.Content>
       <!-- ... -->


        <Entry Text={Binding ...}>
             <Entry.Behaviors>
                <mct:SelectAllTextBehavior />
                <bhvrs:SelectNextOnCompleatedBehavior />
            </Entry.Behaviors>

            <Entry.Behaviors>
                <tk:SelectAllTextBehavior />
            </Entry.Behaviors>
</Entry>
    </ContentPage.Content>
</ctrls:BasePage>
```

