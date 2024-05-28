# Version 3.1.13

## NatCruise
### Fixes
 - Fix Sale Number allowing values longer than 5 characters

## FScruiser
### Fixes
 - Fix suppressing warning on trees
 - Fix initials field not showing values to pick from when editing initials in tally page
 - Fix issue where audit rules on production cruise created in Cruise Design can fail to import sometimes
 - Fix issue where user unable to export cruise sometimes
 - Fix several UI issues on setting page


# Version 3.1.12 - March 31 2024

## NatCruise
### Fixes
 - Fix filters resetting when changing tabs in Field Data page
 - Fix deleting Sample Group in superuser mode
 - Fix endless loop situation when combing file fails 

### Enhancements
 - Add number of errors column to plot in Field Data page
 - V2 Templates included on installation

## FScruiser
### Fixes
 - Fix issue when editing Min/Max values on Tree Audit Rules

# Version 3.1.11 - Dec 18 2024

## NatCruise 
### Fixes
 - Fix when removing unit from stratum, stratum continues to show on exiting plots causing unresolvable errors if no trees exist in plot and not marked as null.
 - Fix deleting species doesn't remove deleted species from list
 - Fix updating fields on stratum templates 
 - Fix deleting stratum templates

### Enhancements
 - Add a multiple selection, select all feature to tree and plot field data pages to allow deleting multiple records at a time. 

### Changes
 - Changed behavior for locking design elements. Now design elements are only locked when tree records exist belonging to it.


# Version 3.1.10 - Oct 3
# FScruiser
## Changes
 - Update Tally Population Info page - adding Stratum and Sample Group tabs 

# NatCruise
## Fixes 
 - Fixed issue when editing Stratum or Sample Group on tree
 - Fixed issue when adding Subpopulation that errorniously says subpopulation has already been added
 - Fixed Tree doesn't display KPI or STM 
## Enhancments
 - Add ability to Add/Delete field data

# Version 3.1.9 - Aug 29
# FScruiser
## Fixes
 - Fix Species Code being unset when switching between trees in Tally Pages

## Changes
- When changing Stratum on tree prompt user if SG code not valid for new stratum
- When changing SG prompt user if Species code not valid for new SG

# NatCruise
## Fixes
- Fix decimal being truncated while typing in values on trees

## Enhancements
- Add ability to associate file types with NatCruise

## Changes
- When changing Stratum on tree prompt user if SG code not valid for new stratum
- When changing SG prompt user if Species code not valid for new SG

# Version 3.1.8 - July 28
# FScruiser and NatCruise 
## Fixes
 - Fix issue when multiple audit rules are applyed to the same field on a given tree
 - Fix potential issue when updating older cruise file from 3.5.4

# NatCruise
## Enhancments
 - Add ability to export Field Data to CSV
 - Add option to create sale folder when creating new cruise

# Version 3.1.7 - June 23
# FScruiser
## Fixes
 - Fix prev/next buttons not working after returning to plot tally page

## Enhancments
 - Add tally population page for plot tally populations
 - Add plot level tree counts indicator for plot tally populations
 - Add ability to adjust size of tally button panels
 - Add about page 

## Changes
 - Change colors use to indicate measure tree, insurance tree, tree counts, and manually added tree in the tally feeds

# NatCruise
## Fixes
 - Fix issue with false positives on plot tree conflict check when combining files
 - Fix issue with Sale Info not loading sometimes

## Enhancements
 - Add dialog for suppressing warnings on tree records

# Version 3.1.6 - June 1, 2023
# NatCruise

## Fixes
- Fixed Edit Tree Counts accessible for plot tally populations. (For plot cruise methods all counts must be associated with a tree, as required by Cruise Processing)
- Fix unrecognized FIA codes not displaying in species page
- Fix some species field changes not saving

## Enhancements
 - Add ability to delete species 
 - Add ability to search FIA codes in species page

## Changes
 - Update look of the Tree Audit Rule page

# FScruiser
## Fixes


## Enhancements
 - Add Stratum, Sample Group, and Subpopulation Pages
 - Allow adding Subpopulations
 - Add Tree and Log Field pages
 - Allow editing Tree and Log field setup
 - Add Tree Audit Rule pages and allow editing audit rules
 - Add Count Measure field in Plot Tally Page for two stage plot cruise methods

## Changes
 - Change default behavior of Prev/Next tree buttons in Plot Tally Page. Buttons now go between all tree, old behavior only went between measure trees. Added option in settings page to use old behavior

# Version 3.1.5 - April 24, 2023

## NatCruise
### Fixes
- Fix plots page showing some duplicate columns
- Strata templates now only shows templates with valid cruise method for cruise

### Enhancements
- Add ability to setup multiple contract species per species code based on product
- After adding a new item to a collection (Strata, Sample Groups, Strata Template) display switches back to edit view for that record
- Show list of log associated with a given tree when tree is selected in Field Data trees tab. 
- In Combing Files, show device name that created record when comparing two conflicting records

### Changes
 - Add additional design checks
 - Allow FixCNT cruise method for recon cruises

## FScruiser & NatCruise
### Fixes
- When combining or importing files with `number plot trees sequentially across strata` option disabled. Plot tree conflict checking will allow multiple trees with the same tree number per plot.

# Version 3.1.4 - Mar 13, 2023

## FScruiser
### Fixes
- Fix Tree and Plot Tree page crashing when field contains empty text

### Enhancments
- Improve error messages when importing cruise files
- Show which fields have warnings in the tree quick edit panel

### Changes
- Update Limiting Distance page

## NatCruise
### Fixes
- Fix all species assigned same FIA code when using V2 template
- Fix design mismatch errors not being shown when combining files

# Version 3.1.3 - Jan 27, 2023
## FScruiser
### Fixes
 - Fixed tree count edits not appearing tally feed after being added
 - Fixed can't add 3PPNT plots
 - Fix new calculate limiting distance, when slope % is 10%

## NatCruise
### Fixes
 - Fixed FIA codes not being read with using V2 (.cut) template files

### Enhancements
 - Added option to create V3 template file when opening V2 (.cut) template files
 
# Version 3.1.2 - Jan 4, 2023
## FScruiser
### Fixes
 - Fixed numeric tree fields in plot tally page don't show numeric keyboard
 - Logs page in Field Data now shows tree number, unit and plot info
 - Fix in Tree Edit Page clicking done on last tree data field doesn't move cursor to Remarks field
 - Fix user can still click edit on tree after untallying it, causing app to crash. 
 - Fix various issues with editing initials on tree

### Enhancments
 - Select all contents when entering tree data text boxes

### Changes
 - Option to add/remove stratum hidden when unit contains single plot stratum 

## NatCruise
## Features
 - Add Combine Files feature (located in the Tools menu)

### Fixes
- Logs page in Field Data now shows tree number, unit and plot info
- Fix scrolling issue in Cutting Units page
- Fix crash when editing species 

### Changes
 - Option to add/remove stratum hidden when unit contains single plot stratum 
 - Change default UOM on FixCNT sample groups to 04 
 
# Version 3.1.1 - Oct 20, 2022
## NatCruise
### Fixes 
 - Fixes creating cruise using V2 template file. 

## FScruiser and NatCruise
### Fixes
 - reduce possibility of issue with updating cruise file
 
# Version 3.1.0 - Sep 29, 2022

## FScruiser
### Enhancements 
 - Various Improvements to the plot list page
    - Number of plot errors show 
    - Summery of tree data shown including tree count and tree errors
    - Null strata shown 
 - Added Previous and Next buttons to the quick tree edit panel to make jumping between measure trees quicker
 - Increased size of tree remarks field in the quick tree edit panel to support editing and viewing longer remarks
 - Added button to limiting distance calculator to copy report to the clipboard

### Changes
 - Added option in settings to use a alternant logic for calculating limiting distance. This option is to allow the new calculation logic to be tested. The new calculation logic uses rounding of intermediate values in a way that tries to be more consistent with hand calculated limiting distance. Precisions used:  Plot Radius Factor - 3 decimals; DBH - tenth of an inch;  Slope Correction Factor - 2 decimals;  Plot Radius for fixed size plot methods - tenth of a foot. To-Face Correction - hundredth of a foot; and for the final Limiting Distance - hundredth of a foot. 
There is one deviation from the handbook in the calculation when correcting for measuring to the face in variable radius plots. The handbook instructs you to calculate the corrected distance to the face of the tree and then use that to calculate a corrected PRF which you then use to recalculate the corrected limiting distance. This redundant step was omitted because it makes the accuracy of the to-face correction dependent on the LD, as well  adds  inconsistency in the accuracy of the corrected LD for Fixed vs Variable plots. The limiting distance report reflects the intermediate values used for Plot Radius Factor (PRF), Slope Correction Factor (SCF), and To Face Correction (TFC). When the option to use the new calculation logic is selected, there is a message shown at the top of the limiting distance page indicating the option is enabled.

## CRZ3 File Format Changes
 - Added additional value checks to fields the cruise file structure
     - Enforce valid values for Log and Tree Grade ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9')
     - Enforce ranges on percentage fields is between 0 and 100
     - Enforce positive values on various numeric fields
     - Enforce value between 0 and 360 on Azimuth 
     - Enforce value between 0 and 200 on slope 
	 
## NatCruise
### Enhancements 
 - Users can now press enter key to add new Unit/Stratum/Sample Group/Subpopulation 
 - Text Boxes reject inputting non numeric values into numeric fields
### Changes 
 - Allow Min/Max KPI to be blank
 - Allow Sampling Frequency of 0 when BigBAF > 0
 - Revert Change where tree default value fields start off with 0 for all numeric fields. Numeric tree fields will receive a default value of 0 if the tree has no value and the default has no value. This was done to be able to differentiate when a default is intentionally set to 0 vs when a field has no value and no default. 

### Fixes
 - Fixed Design Checks not catching missing Tree Defaults sometimes
	 
# Version 3.0.4 - Jun 27, 2022
## NatCruise
### New Features
 - Added Field Data Page. Allows Editing Trees, Plots, Logs, and Counts

## Fixes
 - Fixed field order not saving correctly when setting up fields
 - Fixed various spelling mistakes

## Changes 
 - Allow Max KPI to be 0
 - Contents of text boxs when beginning editing 
 - Allow editing cruise and sale number
 - Applicable Tree Default Values now initialized to 0. This change is primarily for aesthetic purposes. Existing Tree Defaults with no value displayed will default to 0 as well. 
 - Sample Frequency of 0 allowed when Big BAF > 0
 - Add ClickerSelecter as option when selecting sample selector type. This will allow for cruisers that are using clickers for tallying. This feature is not fully implemented in FScruiser. 
 
# Version 3.0.3 - Jun 2, 2022
## FScruiser
### Fixes 
 - Fixed sometimes cruises wouldn't export if more than one cruise existed in a sale

### Enhancements
  - Display Insurance Frequency and KZ in the Tree Count Edit page
  
# Version 3.0.2 - May 25, 2022
## NatCruise 
### Features
 - Added Field Data page
 - Added Combine Files page

### Fixes
 - Fixed field order not saving correctly when setting up fields
 - Fixed various spelling mistakes

### Changes 
 - Allow Max KPI to be 0
 - Contents of text boxs when beginning editing 
 - Allow editing cruise and sale number
 - Applicable Tree Default Values now initialized to 0. This change is primarily for aesthetic purposes. Existing Tree Defaults with no value displayed will default to 0 as well. 
 - Sample Frequency of 0 allowed when Big BAF > 0
 - Add ClickerSelecter as option when selecting sample selector type. This will allow for cruisers that are using clickers for tallying. This feature is not fully implemented in FScruiser. 

# Version 3.0.1 - May 6, 2022

## FScruiser
### Fixes
 - Fixed issue where recon measurement data in production cruises wasn't being imported into FScruiser
 - Fixed displayed tree counts could in some situations include tree counts from other cruses with similar populations
 - Fixed height and diameter tree errors not showing on plot trees
 - Fixed displayed KPI total not updating on tally buttons
 - Fixed issue with reimporting cruise after being deleted
 - Fixed various spelling mistakes
 
# Version 3.0.0 - Mar 1, 2022
## FScruiser
### Enhancements
 - Added Tree error indicator to the Plot Tally page
 - Improved Tree error indicators updating on the Tally page
 - Added sound for tally button presses in FixCNT tally page
 - KPI and STM shown on entries in Tally and Plot Tally page

### Changes
 - Change tree count on STM trees to 0 
 - Disabled ability to open `.cruise` files (just for FScruiser)

### Fixes
 - Fixed various issues with configuring and entering cruisers initials
 - Fixed crash when untalling tally by sample group entries (since v0.41)

## NatCruise

### Fixes
 - Fix various spelling errors and UI issues
 - Fixed crash when selecting or opening template, when last template location is no longer available e.g. removable storage or deleted folder
 - Fixed options for 3PPNT not shown in Stratum Details page