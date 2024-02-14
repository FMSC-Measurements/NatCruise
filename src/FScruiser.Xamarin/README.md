# FScruiser.Xamarin Redme

## Notes

### Hacks
 - when using Xamarin.CommunitToolkit.TabView - there is an issue with the binding source 
 propigating to child TabViewItems. As a workaround content of a TabViewItem
needs to be wrapped in a ContentView. Then you can set the BindingContext of the 
child view. TabViews are used in the TallyPopulationDetailsViews

