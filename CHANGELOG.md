# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.4.0] - 2023-04-23
### Changes
- Update core package dependency

## [1.3.0] - 2023-04-22
### Changes
- Removed some debugs

## [1.2.0] - 2023-04-22
### Changes
- Removed Unity Localization dependency
- Added selection of localization converter implementation
- Made `DropdownComponentConverter` extendable more easily
- Added extension class to set the sprite of an image with all its styles

## [1.1.4] - 2023-03-21
### Changes
- Added binding component on the fly even if specified component does not exist while binding. 
- Fixed text component "AutoHeight" correctly
- Added missing dropdown converter caption image binding

## [1.1.3] - 2023-01-06
### Changes
- Bug fixes to layouts

## [1.1.2] - 2022-12-20
### Changes
- Importing images referenced in a node without respecting its scale mode (fill, fit, tile and stretch) (only one image on a node can be imported right now)

## [1.1.1] - 2022-12-19
### Changes
- Added effect converter list in `FigmaImporter` to be able to customize effect converter for specified effect
- Adding multiple effects on the same node is possible now

## [1.1.0] - 2022-12-17
### Changes
- Meta files regenerated
- Better figma design inspector

## [1.0.0] - 2022-12-17
### Changes
- First release
