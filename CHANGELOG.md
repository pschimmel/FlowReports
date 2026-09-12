# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 0.9.0 / 2026-06-07
### Added
- Added header and footer bands.
- Added automatic height of bands.
- Added unit tests for ViewModel classes.
- Added `FilterExpression` for `ReportBand` — filter rendered items using string expressions (e.g., `Age > 30 && Status == 'Active'`).
- Ordering of data in bands.
- Editor UI: band details now include a filter expression textbox for easy editing.
- Rendering integrates filter evaluation so non-matching items are skipped.
- Unit tests covering filter expression parsing and evaluation.
- Allow text formatting.

### Changed
- Updated third party libraries.
- Update to .NET 10.0.

### Fixed
- Fixed wrong logic on page breaks.

## 0.8.8 / 2ß25-11-10
### Fixed
- Fixed missing naming of top level data source.

## 0.8.7 / 2025-10-07
### Changed
- Removed make stuff
- Dropped support for .NET Framework 4.8
- Updated third party libraries
- Fixed resource references 

## 0.8.2 / 2023-10-29
### Fixed
- Prevent infinite loop when analyzing recursive references.

## 0.8.1 / 2023-10-05
### Fixed
- Fixed build script for CI.

## 0.8.0 / 2023-09-05
### Added
- Added CHANGELOG
- Initial Release of the package