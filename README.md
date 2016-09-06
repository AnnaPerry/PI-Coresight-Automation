# PI-Coresight-Automation
Libraries for programmatically creating PI Coresight displays.

## Overview
* #### Core library
  * `CoresightAutomation`
  
    Core functionality for connecting to PI Coresight and creating displays.
    Currently focused on building "default" displays for a given PI AF Element and its Element Template.

* #### Data access libraries
  * `CoresightAutomation.PIWebAPI`
  * `CoresightAutomation.AFSDK`

  Libraries for reading PI AF Element and Element Template metadata from the PI System and producing the universal `AFElementTemplateSlim` object required by the core library.
  
  Pick one.

* #### Demo applications
   * `CoresightAutomation.Demo.PIWebAPI`
   * `CoresightAutomation.Demo.AFSDK`

   Simplistic console applications which show the creation of a default display.

* #### Scripts
   * `CoresightAutomation.NuGetPublish`

   Contains utility script~~s~~ for packing and publishing the packages to NuGet.

## Prerequisites
`CoresightAutomation` (base library, so applies to all scenarios)
* PI Coresight 2016

`CoresightAutomation.PIWebAPI` (using PI Web API to load Asset data)
* ≥ PI Web API 2015

`CoresightAutomation.AFSDK` (using PI AF SDK to load Asset data)
* ≥ PI AF SDK 2.7

## Packages
On NuGet as pre-release:
* [CoresightAutomation](https://www.nuget.org/packages/CoresightAutomation)
* [CoresightAutomation.PIWebAPI](https://www.nuget.org/packages/CoresightAutomation.PIWebAPI)
* [CoresightAutomation.AFSDK](https://www.nuget.org/packages/CoresightAutomation.AFSDK)

## Creating a default display

An example AF Element from everyone's favorite example database:

   ![Target AF Element](https://github.com/jamesbperry/PI-Coresight-Automation/raw/master/docs/demo/compressor-element.jpg)

New Compressor Template display created by one of the `.Demo.` apps:

   ![Coresight Displays](https://github.com/jamesbperry/PI-Coresight-Automation/raw/master/docs/demo/coresight-display-list.jpg)

   ![Coresight Display](https://github.com/jamesbperry/PI-Coresight-Automation/raw/master/docs/demo/compressor-default-display.jpg)

## Known issues
* Display naming collisions are not validated
* Authentication/authorization issues are not handled gracefully
* The display-building objects in `CoresightAutomation` deserve major refactoring
* There are no tests of any kind
* `CoresightAutomation` is a Janus hell of .xproj/project.json mirrored by .csproj/packages.config.
   * This is due to current netcore tooling limitations
   * ... and will be resolved with the death of project.json in VS 15 RTM, slated for Q4 2016
   * Until then, any changes to NuGet packages in the core project should be done manually in `project.json` and `packages.config`