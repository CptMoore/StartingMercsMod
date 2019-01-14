# StartingMercsMod
BattleTech mod (using ModTek and DynModLib) that allows to add random or ronin mech warriors.

## Installation and Requirements

* install [DynModLib](https://github.com/CptMoore/DynModLib/releases) using [instructions here](https://github.com/CptMoore/DynModLib)
* install StartingMercsMod by putting it into the \BATTLETECH\Mods\ folder

## Features

- add random mercs to starting roster.
- add ronin mercs to starting roster.

Setting | Type | Default | Description
--- | --- | --- | ---
addRandomMercsCount | int | 3 | amount of random mercs to add to roster
randomMercQuality | int | 1 | merc quality is based on difficulty, choose a value between 1 and 5, corresponds to a planets difficulty level
roninChance | float | 0.08f | chance that a random ronin is part of the starting roster
addRoninMercs | string[] | [] | a list of ronin pilot to always add, all backers and ronins under StreamingAssets\data\pilot can be chosen. e.g. `["pilot_ronin_Kraken", "pilot_backer_Chang"]`
addRoninsOnSavegameLoad | bool | false | if set to true, the ronins mentioned under addRoninMercs are added to the barracks when loading a save game, great for adding pilots to an existing save game
allowMultipleRoninCopies | bool | false | allow multiple copies of the same ronin when adding them through this mod, useful to revive dead ronins or get back fired ronins

Note:

The lists `StartingMechWarriors` and `StartingMechWarriorPortraits` in `SimGameConstants.json` needs to be empty in order for this to work. ModTek in-memory patching should do that for you automatically. The game uses the first mercs in the roster to add as pilots for the starting mission. In any case mercs added through this mod will appear in the roster after the mission.

## Download

Downloads can be found on [github](https://github.com/CptMoore/StartingMercs/).

## Install

After installing BTML, ModTek and DynModLib, put the mod into the \BATTLETECH\Mods\ folder and launch the game.

## Development

* Use git
* Use Visual Studio or DynModLib to compile the project
