# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.8.6] - 2023-12-09
### Changes
- Non-gradient rectangle textures are generated as small textures; this reduces the import time significantly (8min to 30 sec)!

## [1.8.5] - 2023-12-09
### Changes
- Minor `FigmaDesign` inspector changes

## [1.8.4] - 2023-10-18
- No changes

## [1.8.3] - 2023-10-03
### Changes
- Added `PDF` to export settings image format

## [1.8.2] - 2023-10-03
- No changes

## [1.8.1] - 2023-08-15
### Changes
- Branch can be specified while downloading the Figma file

## [1.8.0] - 2023-08-01
- No changes

## [1.7.1] - 2023-07-30
### Fixes
- Minor bug fix

## [1.7.0] - 2023-07-30
### Changes
- Added `minWidth`, `maxWidth`, `minHeight` and `maxHeight` to `INodeLayout`
- Added `TextTruncation` to `TypeStyle`
- `TextAutoResize.Truncate` is deprecated 

### Fixes
- Fixed `layoutGrow` is nor marked as `[DataMember]` on `GroupNode`

## [1.6.5] - 2023-07-23
- No changes

## [1.6.4] - 2023-07-21
- No changes

## [1.6.3] - 2023-07-20
- ### Fixes
- Fixed possible bug when deserializing `AffineTransform` to use current serializer

## [1.6.2] - 2023-07-20
- No changes

## [1.6.1] - 2023-07-20
### Fixes
- Fixed Json settings conflict with `Newtonsoft.Json-for-Unity.Converters`

## [1.6.0] - 2023-07-20
### Changes
- Removed `JsonSubTypes` dependency completely
- Conversion `Figma.Color` to `UnityEngine.Color` made explicit

## [1.5.9] - 2023-07-20
### Changes
- Added `PaintJsonConverter` and `EffectJsonConverter` instead of using `JsonSubTypes`

## [1.5.8] - 2023-07-19
### Changes
- Added `expandEdges` sprite generation option to importer
- Solid paint imported as solid instead of gradient

## [1.5.7] - 2023-07-18
- No changes

## [1.5.6] - 2023-07-16
- No changes

### Changes
- Added `LayoutPositioning` enum to `SceneNode` 
- Added `itemReverseZIndex` to `FrameNode`
- Enums are serialized with its string value instead of integer value.

## [1.5.5] - 2023-07-10
- No changes

## [1.5.4] - 2023-07-09
- No changes

## [1.5.3] - 2023-07-06
### Changes
-  Added `FIGMA_PRINT_SVG_STRING` definition for printing svg strings to the console while importing (you should add this definition to `PlayerSettings` for enable printing)

### Fixes
- Fixed a bug that svg string is generated respecting to the operating system's region format

## [1.5.2] - 2023-06-25
- No changes

## [1.5.1] - 2023-06-20
- No changes
 
## [1.4.16] - 2023-06-18
- No changes

## [1.4.15] - 2023-06-16
### Fixes
- Removed SVG debug logs

## [1.4.14] - 2023-06-16
### Fixes
- Fixed `stroke` importing for `FrameNode` and `RectangleNode`

## [1.4.13] - 2023-06-16
### Fixes
- Fixed multiple `fill` attribute XML error while importing `RectangleNode`

## [1.4.12] - 2023-06-14
### Fixes
- Fixed svg generation if a node has more than one geometry
- Fixed svg stroke generation

## [1.4.11] - 2023-06-13
### Fixes
- Disable importing solid color as gradient if fill-rule is "evenodd" because of Vector Graphics Package bug

## [1.4.9] - 2023-05-29
### Fixes
- Fixed `Sprite` importing in Unity Cloud Build

## [1.4.7] - 2023-05-07
### Changes
- Added error handling if `Sprite` is not being generated

## [1.4.5] - 2023-05-04
### Fixes
- Fixed a bug that created `Sprite` and `Texture` assets were marked with `HideFlags.DontSave` which causes build errors

## [1.4.3] - 2023-05-02
### Fixes
- Fixed a bug that initial `FigmaDesign` asset import fails 
- The scale factor is taken into account while calculating the `Sprite` borders
 
## [1.4.2] - 2023-04-25
### Changes
- Added override node option for generating of `Sprite`
- Added scale factor and min-max texture size parameters to `SpriteGenerateOptions`

### Fixes
- Fixed copying Figma design content from the Inspector

## [1.4.0] - 2023-04-23
### Changes
- Added file version option to `FigmaDownloaderAsset`
- `FigmaDownloaderAsset` now accepts single file entry

### Fixes
- Fixed a bug that images always are downloaded ignoring the "Download Images" option from the `FigmaDownloaderAsset`

## [1.3.0] - 2023-04-22
### Changes
- Downloaded Figma file is compressed to save space.

### Fixes
- Fixed sprite size calculation while generating it.

## [1.2.0] - 2023-04-22
### Changes
- Removed Unity Localization dependency

## [1.1.3] - 2023-01-06
### Changes
- Added "Truncate" resizing option to text nodes

## [1.1.2] - 2022-12-20
### Changes
- Added downloading images referenced in a file

## [1.1.1] - 2022-12-19
### Fixes
- Fixed a bug that thin long sprites could not be generated

## [1.1.0] - 2022-12-17
### Changes
- Meta files regenerated

## [1.0.0] - 2022-12-17
### Changes
- First release
