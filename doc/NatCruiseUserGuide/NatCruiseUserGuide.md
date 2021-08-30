<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image1.jpeg" style="width:7.65833in;height:9.81389in" />

> U.S. Forest Service
>
> Washington Office
>
> Forest Management Service Center
>
> Fort Collins, CO
>
> Updated July 2021
>
> This document is currently a work in progress and may be changed
> frequently.

The U.S. Department of Agriculture (USDA) prohibits discrimination in
all its programs and activities on the basis of race, color, national
origin, sex, religion, age, disability, political beliefs, sexual
orientation, or marital or family status. (Not all prohibited bases
apply to all programs.) Persons with disabilities who require
alternative means for communication of program information (Braille,
large print, audiotape, etc.) should contact USDA’s TARGET Center at
(202) 720-2600 (voice and TDD).

To file a complaint of discrimination, write USDA, Director, Office of
Civil Rights, Room 326- W, Whitten Building, 1400 Independence Avenue,
SW, Washington, DC 20250-9410 or call (202) 720-5964 (voice or TDD).
USDA is an equal opportunity provider and employer.

# Table of Contents

[About NatCruise Interim 1](#about-natcruise-interim)

[Installation 1](#installation)

[Using NatCruise Interim 2](#using-natcruise-interim)

[Saving changes 2](#saving-changes)

[Create New Cruise 3](#create-new-cruise)

[Sale Information 4](#sale-information)

[Cruise Information 5](#cruise-information)

[Units Setup 6](#units-setup)

[Strata Setup 7](#strata-setup)

[Stratum Details 7](#stratum-details)

[Fields 7](#fields)

[Cutting Units 8](#cutting-units)

[Sample Groups 8](#sample-groups)

[Sample Group Details 8](#sample-group-details)

[Subpopulations 9](#subpopulations)

[Species 10](#species)

[Audit Rules 11](#audit-rules)

[Tree Default Values 12](#tree-default-values)

[Design Templates 13](#design-templates)

[Stratum Template 13](#stratum-template)

[Tree Fields 13](#tree-fields)

[Template (.cut) Files 14](#template-.cut-files)

[APPENDIX A: CRUISE DESIGN FORMS 15](#appendix-a-cruise-design-forms)

[Sale Information 15](#sale-information-1)

[Cutting Units 15](#cutting-units-1)

[Stratum 16](#_Toc79588109)

[Tree Defaults 17](#tree-defaults)

[Tree Audit Rules 18](#tree-audit-rules)

# About NatCruise Interim

NatCruise Interim is part of the third generation of software for the
U.S. Forest Service’s [National Cruise
System](http://www.fs.fed.us/fmsc/measure/cruising/). It is used to
establish new sales, customize data entry forms, and modify cruise
designs. It is designed to work with the latest versions of Cruise
Processing and Cruise Design. NatCruise Interim serves as an
intermediate desktop software solution while a server-based cruise
management system is developed (anticipated release in fall 2023).

NatCruise Interim currently supports the cruise methods outlined in the
[Timber Cruising
Handbook](https://usdagcc.sharepoint.com/sites/fs-orms/orms-directives/Pages/Browse-Directives.aspx)
(FSH 2409.12), including 100 percent cruise, fixed plot, fixed
count-measure, point sampling, point count-measure, sample tree, 3P,
fixed plot-3P, point-3P, 3P-point, and fixed plot-count.

This document is a work in progress and may be changed frequently.

# Installation

Installation files for NatCruise Interim can be obtained from the Forest
Management Service Center personnel while in testing. Following
production release installation files will be available on the Software
Center for Forest Service users, or from the [Forest Management Service
Center
website](https://www.fs.fed.us/forestmanagement/products/measurement/)
for others. The default directory for all cruise files and template
files is C:/users/\<username>/Documents/CruiseFiles.

# Using NatCruise Interim

The opening screen for NatCruise Interim contains only a File menu with
four options: New Cruise, Open File, Open Recent, and Exit. Version
information is found at the top of the window (Figure 1).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image7.png" style="width:5.435in;height:3.2087in" />

Figure 1-NatCruise Interim Opening Screen

Once a cruise file is created or an existing cruise file is opened, the
best way to fill out information is to move from top to bottom down the
left hand menu and from left to right within nested menus (Figure 2).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image8.png" style="width:5.90064in;height:3.44348in" />

Figure 2 – nested menus within NatCruise Interim

## Saving changes

There is no Save option in the File menu because changes are
automatically saved whenever you check a box or click into a new
textbox. If the program crashes all changes prior to that point will be
retained.

#  Create New Cruise

The New Cruise button will launch a Create New Cruise window (Figure 3).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image9.png" style="width:4.32174in;height:3.20514in" />

Figure 3-Cruise File Creation Prompt

The first step in establishing a new cruise is to define the cruise
information. At the top of this form, you will be asked to select a
Template by using the Browse button. The default location for Template
files installed with V2 software is
C:/users/\<username>/Documents/CruiseFiles/Templates/. Template files
can also be downloaded [from the FMSC
website](https://www.fs.fed.us/forestmanagement/products/measurement/cruising/cruisemanager/index.php)
as a ZIP file. Template files can be edited in Cruise Manager (part of
the version 2 software suite). Although selecting a Template file is
optional, it contains establishment and design information you will have
to enter manually if you do not select one.

Once a template has been selected:

-   enter the Sale Name and Number (usually 5 digits)

-   Select Region and Forest from the pull down lists. District number
    (2 digits) will need to be typed.

-   Select Purpose from the pull-down list (usually Timber Sale or
    Recon).

-   UOM is the default Unit of Measure for the sale. This value will be
    used as the default whenever UOM needs to be entered. If you are
    using FIXCNT method with other cruising methods, you can change the
    UOM to 04 for those FIXCNT populations.

-   The checkbox labeled Number Plot Trees Sequentially Across Strata
    will determine how tree numbers auto populate within plots (1
    through n for each stratum vs 1 through n for the plot as a whole).

Note that all information entered in this screen can be edited later,
except for the Template file applied. Once all the information has been
entered, click on the Save As button, navigate to your desired save
location, edit the file name if desired, and press Save to move on to
subsequent setup steps. Unlike in V2, there is not currently an option
to create a sale folder; this will need to be done manually if desired.

> From this point onwards, the interface does not differ between new
> sale entry and editing an existing file.

# Sale Information

The Sale screen shows the Sale Name, Sale Number (usually 5 digits),
Region, Forest, and District (2 digits) in editable fields. As in
initial setup, Region and Forest are drop-down menus, while the other
fields are typed into.

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image10.png" style="width:5.94266in;height:2.3in" />

Figure 4-Sale level information

# Cruise Information

The Cruise screen shows the Cruise Number, Purpose, Unit of Measure, and
a Remarks field along with an option to number plot trees sequentially
across strata. If purpose is set to Timber Sale the cruise number
defaults to the Sale Number. If a different purpose is selected a code
indicating the purpose of the cruise will be appended to the end of the
sale number. If the box for numbering plot trees sequentially is
checked, all trees on a plot will be numbered sequentially in the order
they are entered into FSCruiser regardless of strata.

> This screen was added to NatCruise Interim in anticipation of a future
> release which will allow for multiple cruises (e.g. Recon, Timber
> Sale, and Check Cruise) to be stored within one Sale-level file.
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image11.png" style="width:5.92565in;height:2.3in" />

Figure 5 – Cruise level information

# Units Setup

The next step is to enter the Unit-level information. To begin, enter an
alphanumeric unit code into the box in the top left next to the green
plus sign. Press the green plus sign to add the unit to the file; to
delete a unit, highlight it in the menu on the left and press the red
minus button (Figure 6).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image12.png" style="width:5.34713in;height:3.00026in" />

Figure 6- Unit Information/add a unit

Each unit requires a Description and Area (in acres). In addition,
Logging Method, Payment Unit, Prescription (Rx), and Remarks can be
entered. These fields will all display in the cruise output, but only
Area, Logging Method, and Payment Unit will be imported into TIM.

# Strata Setup

In NatCruise Interim Strata, Sample Groups, and Sampling Frequency are
all set up using the submenus within the Strata page. Complete data can
be entered one stratum at a time (recommended), or each submenu can be
completed for all strata at once.

## Stratum Details

> To create a new stratum enter a code in the textbox on the left.
> Optionally, you can apply a Stratum Template by selecting one from the
> dropdown menu before pressing the green plus sign (Figure 7). If a
> template is not selected, the cruise method can be set manually. For
> plot-based cruise methods a field will appear for basal area factor
> (BAF) or fixed plot size (FPS). Fixed plot size is entered as the
> inverse of the plot size (e.g. for 1/20<sup>th</sup> acre plots FPS is
> 20). The selected template will set up a default list of fields, which
> can then be modified (see [Fields](#fields)).
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image13.png" style="width:6.34661in;height:2in" />

Figure 7 – Stratum Details and Templates

## Fields

> Fields can be selected manually, or you can start from a template (see
> [Design Templates](#design-templates)). A Default Value can be
> assigned to each field, and the field can be either locked or hidden.
> To rearrange the fields select one from the list and use the Move Up
> and Move Down buttons (Figure 8). Changes are applied to the Stratum
> highlighted on the left.
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image14.png" style="width:6.35in;height:2.5471in" />

Figure 8 – Fields Setup

## Cutting Units

> Select the desired Stratum from the list on the left and check the
> boxes for the corresponding Cutting Units. All units can be selected
> by using the Select All button, and all selections can be cleared by
> using the Clear All button (Figure 9).
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image15.png" style="width:6.34924in;height:2.17047in" />

Figure 9 – Stratum Cutting Units setup

## Sample Groups

> You will establish sample groups, frequencies (if applicable), and
> subpopulations (species lists) through the two submenus within Sample
> Groups.

### Sample Group Details

> Sample Group Details is where you will enter the sample group Code
> that will be displayed on the tally/plot page in FS Cruiser V3. Code
> and Primary Product are required fields. In addition, a Description,
> Secondary Product, Biomass Product, Cut/Leave designation, and
> Live/Dead Default can be set. If no choices are made, the program will
> default to Cut and Live (Figure 10). If the sample method selected
> involves frequency sampling, related field entry will be available
> (frequency, insurance frequency, sample selector type, big BAF, KZ,
> etc. depending on cruise method).
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image16.png" style="width:6.34953in;height:2.87826in" />

Figure 10 – Sample Group Details page

> There is a checkbox in this menu for Tally by Species. If checked,
> trees will be tallied at the Species Level. If left unchecked, trees
> will be tallied at the Sample Group level.

### Subpopulations

> Use this menu to assign species to the selected Sample Group. Species
> codes can be selected from the drop down, or a new code can be typed
> in. Press the green plus button to add the species to the list. Once a
> species code is added, the Live/Dead field can be changed. Adding a
> duplicate of an existing species code will create a copy with the
> opposite Live/Dead default (Figure 11). The program will not allow
> more than two replicates of the same species code.
>
> <img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image17.png" style="width:6.34939in;height:2.32174in" />

Figure 11 – Subpopulations setup

# Species

The Species menu shows species codes present in the Template File, as
well as any additional species codes entered into the cruise setup. Each
species code can be assigned a Contract Species and FIA code (Figure
12). The FIA code links the species to volume equations within Cruise
Processing, although this can also be done manually if the field is left
blank.

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image18.png" style="width:6.35in;height:5.06319in" />

Figure 12 – Species Setup

#  

# Audit Rules

Audit rules can be used to test entered tree data. If data violates the
Audit Rules a warning will be generated within FS Cruiser V3 as well as
within the Cruise Processing program. Errors triggered by audit rules
can be suppressed to enable processing, so audit rules like minimum
heights can be used even if they may be violated on rare occasions.

To create an audit rule, establish the rule on the left hand side of the
screen in the Add Audit Rule area. Choose a Tree Field, and minimum
and/or maximum allowed value. A description can also be added for
clarity (Figure13).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image19.png" style="width:6.35in;height:2.33372in" />

Figure 13 – Audit Rule Setup

To apply the audit rule to a Species, Product, and Live/Dead
combination, highlight the rule on the left and use the drop down menu
on the right in the Add Population To Audit Rule area. Note that ‘Any’
is an option for all three dropdowns. In the event that audit rules
conflict with one another, the most specific rule will take precedence
(for example, if a rule limits DBH for Sawtimber of Any species to 12.0
inches or greater but another rule limits DBH for Ponderosa Pine
Sawtimber to 10.0 – 99.9 inches, a value of 11.0 inches will not produce
an error).

To delete an Audit Rule or to delete a Population from an audit rule,
highlight the desired line and press the appropriate Delete button at
the bottom of the screen.

# Tree Default Values

The Tree Defaults table displays a list of species code/product
combinations present in the Template File, as well as any additional
species codes entered into the cruise setup. Many default values can be
set for each species. Primary Product Cull, Primary Product Hidden
Defect, and Tree Grade can be assigned different default values for Live
and Dead trees using the same species code (Figure 14). If you wish to
have multiple Hidden Primary Defect percent for the same Species,
Primary Product, and Live/Dead combination, you will have to create a
new record with a different Species code to distinguish it from the
original code. This list can be edited, new records can be added, and
existing records can be deleted.

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image20.png" style="width:6.35in;height:3.71042in" />

Figure 14 – Tree Default Values setup

# Design Templates

Templates for each cruise method can be edited or new templates can be
created using the Design Templates menu.

## Stratum Template

> A default Code, Yield Component, and Basal Area Factor or Fixed Plot
> Size can be set for each cruise method. To create a new template, type
> the code into the box on the left and hit the green “+” button (Figure
> 15).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image21.png" style="width:5.33415in;height:3.1in" />

Figure 15 – Stratum Template setup

## Tree Fields

> Default fields can be established for each Template, as well as
> assigning default values and either locking or hiding each one.
> Highlight the desired template on the left to edit the associated
> fields. Fields can be added and removed using the green “+” or the red
> “-“ buttons (Figure 16).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image22.png" style="width:5.35479in;height:3.1in" />

Figure 16 – Tree Fields setup

# Template (.cut) Files

In the current version of NatCruise Interim, Template Files from Version
2 of the National Cruise software suite are used in setup. These .cut
files contain information that will be used to fill in default values in
new cruises, including species information and field setup settings.

The information in Template Files can be edited directly using Cruise
Manager (available in the Forest Service Software Center or from the
[Forest Management Service Center
website](https://www.fs.fed.us/forestmanagement/products/measurement/cruising/cruisemanager/index.php)).
To edit a Template File, Click on the Open File button. In the Open File
dialog box, use the pull down list at the bottom for Files of type: and
select Cruise Template File (\*.cut). By default, Template Files will be
stored in My Documents/Cruise Files/Template Files. Select a Template
File and click Open. The Template Editor allows the user to setup and
edit Template data (Figure 17).

<img src="C:\Users\BenCamps\Documents\FMSC_GitHub\NatCruise\doc\NatCruiseUserGuide\/media/image23.png" style="width:6.35in;height:3.60825in" />

Figure 17-Template Editor in Cruise Manager

# APPENDIX A: CRUISE DESIGN FORMS

Fill out these forms for each new cruise being established, and keep an
updated copy in the presale folder.

Cruise Designer Name:

Date:

Path and Name of Template File Used:

## Sale Information

| Sale Number | Sale Name | Purpose | Default UOM | Region | Forest | District Code | Log Grading? |
|-------------|-----------|---------|-------------|--------|--------|---------------|--------------|
|             |           |         |             |        |        |               |              |

## Cutting Units

<table>
<colgroup>
<col style="width: 16%" />
<col style="width: 19%" />
<col style="width: 18%" />
<col style="width: 21%" />
<col style="width: 24%" />
</colgroup>
<thead>
<tr class="header">
<th><blockquote>
<p>Cutting Unit Code</p>
</blockquote></th>
<th><blockquote>
<p>Area</p>
</blockquote></th>
<th><blockquote>
<p>Description</p>
</blockquote></th>
<th><blockquote>
<p>Payment Unit</p>
</blockquote></th>
<th><blockquote>
<p>Logging Method</p>
</blockquote></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
</tbody>
</table>

## Stratum

> (Fill out a copy of this page for each stratum.)

Code: \_\_\_\_\_\_\_ Cruise Method: \_\_\_\_\_\_\_\_ Description:
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_

Month: \_\_\_\_\_\_\_ Year: \_\_\_\_\_\_\_\_ Yield Component:
\_\_\_\_\_\_ BAF: \_\_\_\_\_\_\_\_ FP Size: \_\_\_\_\_\_\_\_ 3PPNT KZ:
\_\_\_\_\_\_\_

List Cutting Units:

> **Sample Groups in Stratum**

| **SG Code** | **Prim Prod** | **Sec Prod** | **Default LD** | **Samp Freq** | **Ins Freq** | **Big BAF\*** | **KZ** | **Min KPI** | **Max KPI** | **Descrip** | **Tally by\*\*** | **System-atic?** | **Tree Defaults (Sp/PProd/LD)** |
|-------------|---------------|--------------|----------------|---------------|--------------|---------------|--------|-------------|-------------|-------------|------------------|------------------|---------------------------------|
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |
|             |               |              |                |               |              |               |        |             |             |             |                  |                  |                                 |

**Tree Field Setup Tally Setup –** Stratum Hot Key:

<table>
<colgroup>
<col style="width: 18%" />
<col style="width: 12%" />
<col style="width: 18%" />
<col style="width: 5%" />
<col style="width: 15%" />
<col style="width: 15%" />
<col style="width: 14%" />
</colgroup>
<thead>
<tr class="header">
<th><blockquote>
<p>Field Name</p>
</blockquote></th>
<th>Heading</th>
<th><blockquote>
<p>Width (0 = auto width)</p>
</blockquote></th>
<th></th>
<th><blockquote>
<p>SG or Sp Code</p>
</blockquote></th>
<th><blockquote>
<p>Description</p>
</blockquote></th>
<th><blockquote>
<p>Hot Key</p>
</blockquote></th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="even">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
<tr class="odd">
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>
</tbody>
</table>

\* or Small FP size for FCM

\*\* Sample Group, Species or Don’t Tally

## Tree Defaults 

| **Sp Code** | **Primary Prod** | **LD** | **FIA Code** | **Cull P** | **Hidden P** | **Cull S** | **Hidden S** | **% Rec** | **Grade** | **Form Class** | **Contr Sp** | **Merch Ht LL** | **Merch Ht Type** | **BTR** | **AvgZ** | **RefHtPer** |
|-------------|------------------|--------|--------------|------------|--------------|------------|--------------|-----------|-----------|----------------|--------------|-----------------|-------------------|---------|----------|--------------|
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |
|             |                  |        |              |            |              |            |              |           |           |                |              |                 |                   |         |          |              |

## Tree Audit Rules

| **Field** | **Min** | **Max** | **Tree Defaults (Sp/PProd/LD)** |
|-----------|---------|---------|---------------------------------|
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
|           |         |         |                                 |
