# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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
