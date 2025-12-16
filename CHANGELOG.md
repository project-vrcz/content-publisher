# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Windows Installer (NSIS). [`#101`](https://github.com/project-vrcz/content-manager/pull/101)

## [1.1.0] - 2025-12-11

### Changed

- Insert new task to the beginning of the task list. [`#89`](https://github.com/project-vrcz/content-manager/pull/89)
- Challenge Code will Always uppercase. [`#93`](https://github.com/project-vrcz/content-manager/pull/93)
- Allow copy challenge code in request challenge dialog. [`#94`](https://github.com/project-vrcz/content-manager/pull/94)

## [1.0.0] - 2025-12-08

### Added

- Show App version, commit hash and build date in App settings page [`#70`](https://github.com/project-vrcz/content-manager/pull/70).
- Basic Linux Support [`#76`](https://github.com/project-vrcz/content-manager/pull/76)

### Changed

- Use `Path.Combine(Path.GetTempPath(), "vrchat-content-manager-81b7bca3")` as temp path:
  - Windows:
    - If App running as SYSTEM, it will use `C:\Windows\SystemTemp\vrchat-content-manager-81b7bca3` (DON'T DO TAHT)
    - If not, App will check environment variables in the following order and uses the first path found:
      - The path specified by the `TMP` environment variable. (usually `C:\Users\{UserName}\AppData\Local\Temp\vrchat-content-manager-81b7bca3`)
      - The path specified by the `TEMP` environment variable. (usually `C:\Users\{UserName}\AppData\Local\Temp\vrchat-content-manager-81b7bca3`)
      - The path specified by the `USERPROFILE` environment variable. (usually `C:\Users\{UserName}\vrchat-content-manager-81b7bca3`)
      - The Windows directory. (MAYBE `C:\Windows\Temp\vrchat-content-manager-81b7bca3`, and you will run into trouble as App MAY don't have premission to access this folder) 
  - Linux:
    - Use environment variable `TMPDIR` if exist.
    - If not, use `/tmp/vrchat-content-manager-81b7bca3`
  - see [Path.GetTempPath()](https://learn.microsoft.com/en-us/dotnet/api/System.IO.Path.GetTempPath?view=net-10.0) for more information.
- Adjust http rqeuest pipeline [`#80`](https://github.com/project-vrcz/content-manager/pull/80)
  - Use DecorrelatedJitterV2 as http request retry strategy
  - Increase retry delay
  - Increase MaxConnectionsPerServer to 256 from 10 for AWS S3 HttpClient

## [1.0.0-rc.1] - 2025-12-07

### Added

- Show App version, commit hash and build date in App settings page [`#70`](https://github.com/project-vrcz/content-manager/pull/70).
- Basic Linux Support [`#76`](https://github.com/project-vrcz/content-manager/pull/76)

### Changed

- Adjust http rqeuest pipeline [`#80`](https://github.com/project-vrcz/content-manager/pull/80)
  - Use DecorrelatedJitterV2 as http request retry strategy
  - Increase retry delay
  - Increase MaxConnectionsPerServer to 256 from 10 for AWS S3 HttpClient

[unreleased]: https://github.com/project-vrcz/content-manager/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/project-vrcz/content-manager/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/project-vrcz/content-manager/compare/v1.0.0-rc.1...v1.0.0
[1.0.0-rc.1]: https://github.com/project-vrcz/content-manager/releases/tag/v1.0.0-rc.1