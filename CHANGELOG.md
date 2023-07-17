# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.5.7] - 2023-07-18
### Changes
- Added lazy loading for `FigmaAsset`

## [1.5.6] - 2023-07-16
### Changes
- Added `LayoutPositioning` for children of a `FrameNode`

## [1.5.5] - 2023-07-10
### Changes
- Fixed a bug that `FigmaAssetAttribute` works only for public members 

## [1.5.4] - 2023-07-09
### Changes
- Added `FigmaAssetAttribute` to bind external assets manually from figma importer inspector

## [1.5.3] - 2023-07-06
- No changes

## [1.5.2] - 2023-06-25
### Changes
- Selectable selector state is updated on `OnEnable`

## [1.5.1] - 2023-06-20
### Changes
- Added `ProgressBar` component

## [1.5.0] - 2023-06-19
### Changes
- Changed default value of `isRequired` to `true` for  `FigmaNodeAttribute`, `FigmaResourceAttribute` and `FigmaLocalizeAttribute`

## [1.4.16] - 2023-06-18
### Changes
- Added `FigmaLocalizeAttribute` for binding localization without referencing localized asset.
- 
## [1.4.15] - 2023-06-16
### Fixes
- Removed SVG debug logs

## [1.4.14] - 2023-06-16
### Changes
- Added `FigmaPage` converter

## [1.4.13] - 2023-06-16
### Fixes
- Fixed multiple `fill` attribute XML error while importing `RectangleNode`

## [1.4.12] - 2023-06-14
### Fixes
- Fixed svg generation if a node has more than one geometry
- Fixed svg stroke generation

## [1.4.10] - 2023-06-11
### Fixes
- Added `Query` extension to `FigmaNode` method searching for component type without binding key 

## [1.4.9] - 2023-05-29
### Fixes
- Fixed `Sprite` importing in Unity Cloud Build

## [1.4.8] - 2023-05-09
### Changes
- Added `FigmaResourceAttribute` for binding assets from the Resource folder

## [1.4.7] - 2023-05-07
### Changes
- Added error handling if `Sprite` is not being generated

## [1.4.6] - 2023-05-05
### Changes
- Added asset importer option to make the imported asset not dependant any external asset

## [1.4.5] - 2023-05-04
### Fixes
- Fixed a bug that created `Sprite` and `Texture` assets were marked with `HideFlags.DontSave` which causes build errors

## [1.4.4] - 2023-05-03
### Fixes
- Fixes binding of logs to `FigmaDocument`

## [1.4.3] - 2023-05-02
### Changes
- Added an option to specify `FigmaNode` game objects' layer

### Fixes
- An exception thrown is the same binding key is used for different type of classes while binding nodes with `FigmaNodeBehaviourConverter` 
- An exception thrown is the same type id is used for different type of classes while binding components with `FigmaComponentBehaviourConverter`

## [1.4.2] - 2023-04-25
### Changes
- Added importing the override colors of the instance swap
- Added scale factor and min-max texture size parameters for sprite generation

## [1.4.1] - 2023-04-23
### Fixes
- Fixed a bug that text localization is ignored if `TextNode` is not inside a `ComponentSetNode`

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
