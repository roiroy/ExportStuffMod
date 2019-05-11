ExportStuffMod
==============

This is a [Rise of Industry](https://www.riseofindustry.com/) mod to export in-game data, mostly recipes, in a computer readable format. The mod just prints in the debug log during game start and doesn't do anything after.

Based on https://github.com/S-anasol/ROI-CustomMod

Data
----
I've included recipe data exported by the "current" game in [`exports/exports.json`](exports/exports.json). Take a look at the `timestamp` there to judge how up-to-date that is. There's also [`exports/exports.csv`](exports/exports.csv) for the spreadsheet-inclined

Installation
------------

To install the compiled mod, go to the game installation folder (e.g. `C:\Program Files (x86)\Steam\steamapps\common\RiseOfIndustry`), create new folder `Mods` there if it doesn't exist already, then create new folder `ExportStuffMod` in `Mods`. Copy [`desc.json`](desc.json) and [`code`](code) from this repo to `ExportStuffMod`. 

Output
------

The mod outputs a large-ish JSON in the game's debug log. On Windows, that's located in `"%LOCALAPPDATA%Low\Dapper Penguin Studios\Rise of Industry\output_log.txt"`. 

Compilation
-----------

Visual Studio 2019 Community Edition solution and project files can be found in the [`src`](src) folder. Follow the [modding guideline](https://riseofindustry.gamepedia.com/Modding) on how to compile mods. The project is setup to output the dll in the [`code`](code) folder to conform with Rise of Industry's mod folder structure.