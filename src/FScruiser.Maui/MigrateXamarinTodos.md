# Todo

 - [ ] add logging for view model load times, either to NavigationService or the view model base class
 - [ ] Change AutomationProperties to SemanticProperties
 - [ ] Switch NatCruise.Core TaskExtentions to use ILogger rather than ILoggingService

## Update Converters
Maui Community Toolkit has updated some of the converters used.

In the past there was a IsNotNullOrEmptyConverter that worked with all objects

now there are more type specific converters: IsStringNotNullOrEmpty, IsListNotNullOrEmpty

 - [ ] update isNotNullOrEmptyConverter to more type specific implementations
 - [ ] update listIsNotNullOrEmptyConver to isListNotNullOrEmptyConverter




# View Specific Todos

## Limiting Distance View
 - [ ] revisit radio buttons in Limiting Distance page. I think it is an issue with taps on the radio button content not bubbling down to radio button control.

## TallyView
 - [ ] look into getting rid of the ViewLifecycleEffect on the tally Feed. 
 - [ ] find/write something extension method for injecting TallySettingsDataService. Currently its just being instantiated in xaml (line 23)
 - [ ] find alternative to using relativeLayout
 - [ ] 
 
## LogListView
 - [ ] Rework templates for Logs