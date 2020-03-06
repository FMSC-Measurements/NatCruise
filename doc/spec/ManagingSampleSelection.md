# Managing Sample Selection State
Sample selection state is used in sampling methods that are used to guarantee a certain number of samples over a given number of trees.

Version 3 cruise files store separate sampling state for each device. Separate devices are identified using the device model plus a unique identifier. In situations where multiple devices are used in parallel, it is not possible for each device to coordinate with each other to ensure the desired number of samples are achieved. However, we do seek to ensure that each device will produce close to the correct number of samples over the course of the cruise. 

Special consideration has been made for when cruise files need to be updated and/or transferred between devices. When opening a cruise file that has been partially cruised on another device, FScruiser will ask the user if they want to continue sampling from where the previous device left off or to start with a fresh state.

If at some point during the cruise the cruise design needs to be updated, then each copy of the cruise file can be combined, the combined file updated and then the updated file given back to the crews. Because the sample state for each device is retained during this process, each device will be able to pick up right where it left off with the combined and update file.  