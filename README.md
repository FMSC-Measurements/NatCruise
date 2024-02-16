# National Cruise System mobile and dektop applications
Welcome to the repository for the third generation of the National Cruise System mobile and desktop applications.

This repo consists of two applications: NatCruise and FScruiser, 
along with a shared project NatCruise.Core used by both application.
 
# Build

## FScruiser.Maui
### Prerequisites
Before building a dotnet Maui app you need to install the required workflows. 
This can be done using the Visual Studio installer and selecting the **.NET Multi-platform App UI development** 
item in the Workloads tab.

Another option is to run the command
`dotnet workload restore` from the FScruise.Maui project directory

## FScruiser.Droid
### Prerequisites
Befor building insure the **Xamarin** component is installed using Visual Studio Installer. In VS 2022 this is a
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



