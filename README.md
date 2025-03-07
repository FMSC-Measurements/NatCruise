# National Cruise System mobile and dektop applications
Welcome to the repository for the third generation of the National Cruise System mobile and desktop applications.

This repo consists of two applications: NatCruise and FScruiser, 
along with a shared project NatCruise.Core used by both application.

FScruiser.Droid and FScruiser.Xamarin are the Xamarin.Forms based applications for the FScruiser project.
in 2024 we will be transitioning to .NET MAUI for the FScruiser project and the FScruiser.Maui project will be the .NET MAUI based application.



For project specific information see the Readme.md files in the project directories.
 - [FScruiser.Maui](FScruiser/Readme.md)
 

Some properties are stored in the `Directory.Build.props` file.


## FScruiser.Droid

### Build
#### Prerequisites
Before building insure the **Xamarin** component is installed using Visual Studio Installer. In VS 2022 this is a
optional component under the **.NET Multi-platform App UI development** Workload.

 # Features
 ## NatCruise
 - Create and Design cruises
 - Setup data audit rules
 
 ## FScruiser
 - Tally Cruise Data using the following cruise methods: 
	- Plot Cruise Methods: FIX, PNT, FCM, PCM, F3P, P3P, 3PPNT, FixCNT
	- Tree Based Cruise Methods: 100%, STR, 3P, S3P
 - Calculate Limiting Distances


## MVVM 
For FScruiser Xamarin and NatCruise WPF [Prism](https://prismlibrary.com/) is used for MVVM. Although mostly just for ViewModel injection.
NatCruise WPF uses Prism's region feature. Moving to Maui we removed Prism and are using a more simple MVVM pattern using just IOC provided by `Microsoft.Extentions`.
Implementing all View classes with a constructor that takes a ViewModel as a parameter, rather than using a ViewModelLocator or other sort of naming convention based method.
MAUI shell is not used. 

### ViewModelBase
ViewModelBase is a simple base class for ViewModels. It implements INotifyPropertyChanged and provides a RaisePropertyChanged method.

ViewModelBase.OnInitialize is called when the ViewModel is first created and provides perameters to the ViewModel. Implementers should only override if view model 
has child view models that need to be initialized.

ViewModelBase.Load is called when the View OnNavigatedTo event is fired. This is where the ViewModel should load data from the model.



