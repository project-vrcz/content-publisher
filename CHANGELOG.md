# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Software upgrade. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
  - Send in-app notification when app update available.
  - Show changes in app.
  - Skip specify version.
  - Update check run on start or each 30mins in background. (optional)
  - Download update in background. (optional)
- Toggle pinned or borderless window in tray menu. [`#291`](https://github.com/project-vrcz/content-publisher/pull/291)
- Add loading text to bootstrap screen.

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)

### Fixed

- Cannot exit app when dialog show. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
- Abnormal shutdown during web request may corrupted session storage in some case. [`#292`](https://github.com/project-vrcz/content-publisher/pull/292)
- Click tray icon only make window focused instead of bring window to front when window minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)
- Enable borderless window won't reset window state to normal from maximized or minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)

## [2.9.0-beta.2] - 2026-04-19

### Added

- Software upgrade. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
  - Send in-app notification when app update available.
  - Show changes in app.
  - Skip specify version.
  - Update check run on start or each 30mins in background. (optional)
  - Download update in background. (optional)

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)

### Fixed

- Cannot exit app when dialog show. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)

## [2.9.0-beta.1] - 2026-04-16

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)

## [2.8.2] - 2026-04-03

### Added

- Reuse login page for account repair. [`#266`](https://github.com/project-vrcz/content-publisher/pull/266)
  - i18n support for account repair.
  - Use cookies for account repair.

### Fixed

- Typo in login with cookies alert. [`#261`](https://github.com/project-vrcz/content-publisher/pull/261)
- Exit app with invalid or expired session took very long time. [`#267`](https://github.com/project-vrcz/content-publisher/pull/267)
- Bind to IPv4 localhost fail silently if bind to IPv6 succeed. [`#268`](https://github.com/project-vrcz/content-publisher/pull/268)

## [2.8.1] - 2026-04-03

### Fixed

- Task will always fail if retry after fail before record updated and after file uploaded. [`#259`](https://github.com/project-vrcz/content-publisher/pull/259)

## [2.8.0] - 2026-04-02

### Changed

- UI/UX Improvement.
  - Adjust spacing between toggle and label. [`#251`](https://github.com/project-vrcz/content-publisher/pull/251)
  - Adjust page header height and chinese text for login page. [`#252`](https://github.com/project-vrcz/content-publisher/pull/252)
- Allow all task action (Cancel, remove and debug) except of retry when session expired, instead of prevent all interactions. [`#257`](https://github.com/project-vrcz/content-publisher/pull/257)

### Added

- i18n support for tray menu. [`#250`](https://github.com/project-vrcz/content-publisher/pull/250)
- Login using cookies. [`#253`](https://github.com/project-vrcz/content-publisher/pull/253)
- Better session expired handle logic for tasks. [`#257`](https://github.com/project-vrcz/content-publisher/pull/257)
  - Cancel all task in publish state when session expired automatically.
  - Prevent task enter publish stage if session expired.

## [2.7.0] - 2026-03-29

### Changed

- Improved the text and term in the app. [`#242`](https://github.com/project-vrcz/content-publisher/pull/242)
- Extend the area trigger tooltip of tasks count in Tasks page. [`#242`](https://github.com/project-vrcz/content-publisher/pull/242)
- Slightly adjust the layout of some page or dialog. [`#242`](https://github.com/project-vrcz/content-publisher/pull/242)

### Added

- i18n support. [`#242`](https://github.com/project-vrcz/content-publisher/pull/242)
  - Support language: English, Mandarin Chinese (普通话)
  - Task progress text are not localize in this release.

### Fixed

- Add new account require restart to appear on Tasks page. [`#236`](https://github.com/project-vrcz/content-publisher/pull/236)
- Banner image in first start welcome page won't fit to window width. [`#238`](https://github.com/project-vrcz/content-publisher/pull/238)
- Show IP Flyout text foreground are always white even in light mode. [`#244`](https://github.com/project-vrcz/content-publisher/pull/244)
- Custom proxy URL field only show when re-enter settings page after select custom proxy. [`#245`](https://github.com/project-vrcz/content-publisher/pull/245)

## [2.6.1] - 2026-03-22

### Changed

- For world publish task which will create a new world, will create world record while create publish task. [`#229`](https://github.com/project-vrcz/content-publisher/pull/229)
- For Linux, the pin button in the home page now control topmost of the window. [`#230`](https://github.com/project-vrcz/content-publisher/pull/230)

### Added

- For Windows, you can choice use borderless window (current behavior) or normal window (with system window chrome). [`#230`](https://github.com/project-vrcz/content-publisher/pull/230)
- Send Test Notification. [`#232`](https://github.com/project-vrcz/content-publisher/pull/232)
- Notification Enabled Settings. [`#232`](https://github.com/project-vrcz/content-publisher/pull/232)
- Notify user when public IP changed. [`#231`](https://github.com/project-vrcz/content-publisher/pull/231)
  - Send desktop notification when public ip changed. (optional settings)
  - Show warning banner on Tasks page when public ip changed.
  - Will only print encrypted public ip to log. (decrypt in app debug settings)

### Fixed

- Fix issues with create new world. [`#229`](https://github.com/project-vrcz/content-publisher/pull/229)
  - Upload separate worlds accidentally when MPA upload or
  - Blueprint id was immediately clear after publish task create.

### Changes from 2.6.0

#### Fixed

- App crash when startup.
- Normal mode windows won't respond to Close or Resize correctly. [`#234`](https://github.com/project-vrcz/content-publisher/pull/234)

## [2.6.0] - 2026-03-22

### Changed

- For world publish task which will create a new world, will create world record while create publish task. [`#229`](https://github.com/project-vrcz/content-publisher/pull/229)
- For Linux, the pin button in the home page now control topmost of the window. [`#230`](https://github.com/project-vrcz/content-publisher/pull/230)

### Added

- For Windows, you can choice use borderless window (current behavior) or normal window (with system window chrome). [`#230`](https://github.com/project-vrcz/content-publisher/pull/230)
- Send Test Notification. [`#232`](https://github.com/project-vrcz/content-publisher/pull/232)
- Notification Enabled Settings. [`#232`](https://github.com/project-vrcz/content-publisher/pull/232)
- Notify user when public IP changed. [`#231`](https://github.com/project-vrcz/content-publisher/pull/231)
  - Send desktop notification when public ip changed. (optional settings)
  - Show warning banner on Tasks page when public ip changed.
  - Will only print encrypted public ip to log. (decrypt in app debug settings)

### Fixed

- Fix issues with create new world. [`#229`](https://github.com/project-vrcz/content-publisher/pull/229)
  - Upload separate worlds accidentally when MPA upload or
  - Blueprint id was immediately clear after publish task create.

## [2.5.0] - 2026-03-14

### Added

- Send notification when login failed during startup. [`#220`](https://github.com/project-vrcz/content-publisher/pull/220)
- Send notification when publish task failed. [`#220`](https://github.com/project-vrcz/content-publisher/pull/220)
- Custom RPC server port setting. [`#217`](https://github.com/project-vrcz/content-publisher/pull/217)
- App will fallback to next available localhost port if configured RPC port is unavailable on startup. [`#217`](https://github.com/project-vrcz/content-publisher/pull/217)
- Show startup warning dialog when configured RPC port is in use and fallback port is used. [`#217`](https://github.com/project-vrcz/content-publisher/pull/217)
- Allow selecting a default account in Account Settings for Tasks page. [`#218`](https://github.com/project-vrcz/content-publisher/pull/218)

## [2.4.2] - 2026-03-13

### Changed

- Will return to Home page if click "Repair" button in home page. [`#216`](https://github.com/project-vrcz/content-publisher/pull/216)

## [2.4.1] - 2026-03-13

### Fixed

- App crash when open settings page in some case. [`#214`](https://github.com/project-vrcz/content-publisher/pull/214)

## [2.4.0] - 2026-03-11

### Changed

- Move RGB cycling menu settings to Appearance setting. [`#212`](https://github.com/project-vrcz/content-publisher/pull/212)

### Added

- Sort how tasks sorted in Tasks page. [`#212`](https://github.com/project-vrcz/content-publisher/pull/212)
  - Latest first (Default), Oldest first.

### Fixed

- Tasks in Tasks page didn't sort correctly after re-enter Tasks page. [`#211`](https://github.com/project-vrcz/content-publisher/pull/211)

## [2.3.0] - 2026-02-24

### Added

- Show task created time. [`#207`](https://github.com/project-vrcz/content-publisher/pull/207)
- RGB Cycling animation menu bar. [`#208`](https://github.com/project-vrcz/content-publisher/pull/208)

### Fixed

- App won't exit process after quit in some cases. [`#206`](https://github.com/project-vrcz/content-publisher/pull/206)

## [2.2.2] - 2026-02-17

### Fixed

- App crashed when using the "remove tasks" action menu. [`#205`](https://github.com/project-vrcz/content-publisher/pull/205)

## [2.2.1] - 2026-02-16

### Fixed

- Bundle processing pipeline will always fail if app start with working directory which is not app folder. [`#204`](https://github.com/project-vrcz/content-publisher/pull/204)

## [2.2.0] - 2026-01-23

### Changed

- Menu items are more compact now. [`#179`](https://github.com/project-vrcz/content-publisher/pull/179)
- When avatar details are not found, it will be reported that the avatar may not exist or its owner account is not logged in. [`#192`](https://github.com/project-vrcz/content-publisher/pull/192)
- Reduce memory usage when upload. [`#195`](https://github.com/project-vrcz/content-publisher/pull/195) [`#197`](https://github.com/project-vrcz/content-publisher/pull/197)

### Added

- Show user display name and avatar when session invalid. [`#180`](https://github.com/project-vrcz/content-publisher/pull/180)
- Retry all failed or canceled tasks menu. [`#179`](https://github.com/project-vrcz/content-publisher/pull/179)
- New bundle process pipeline for mulit-target publish. [`#187`](https://github.com/project-vrcz/content-publisher/pull/187) [`#200`](https://github.com/project-vrcz/content-publisher/pull/200) [`#201`](https://github.com/project-vrcz/content-publisher/pull/201)
- New `FeatureFlags` field for rpc api metadata. [`#187`](https://github.com/project-vrcz/content-publisher/pull/187)
- Keep seleted tab after page switch for home and tasks page. [`#190`](https://github.com/project-vrcz/content-publisher/pull/190)
- Create publish task will faill if file id provide when create publish task is not exist. [`#194`](https://github.com/project-vrcz/content-publisher/pull/194)
- Will clean-up all temp files when remove task. [`#198`](https://github.com/project-vrcz/content-publisher/pull/198)

### Fixed

- App crash if session cookies storage is empty. [`#180`](https://github.com/project-vrcz/content-publisher/pull/180)
- Potential memory leak issues in UI [`#191`](https://github.com/project-vrcz/content-publisher/pull/191)
- Won't retry if download response body took too long. [`#193`](https://github.com/project-vrcz/content-publisher/pull/193)

## [2.1.0] - 2026-01-12

### Changed

- Show Build Datetime in local time zone. [`#129`](https://github.com/project-vrcz/content-publisher/pull/129)

### Added

- Windows Installer will reuse last install location. [`#172`](https://github.com/project-vrcz/content-publisher/pull/172)
- Windows Installer will uninstall previous version before install. [`#172`](https://github.com/project-vrcz/content-publisher/pull/172)
- Windows Installer / Uninstaller check is app running before start. [`#171`](https://github.com/project-vrcz/content-publisher/pull/171)
- Require confirm before exit app if have active publish tasks. [`#170`](https://github.com/project-vrcz/content-publisher/pull/170)
- Network Diagnostics. [`#169`](https://github.com/project-vrcz/content-publisher/pull/169)
  - Check out VRChat API Status.
  - Test Connection to VRChat API, AWS S3, Cloudflare and Cloudflare China.
  - Check out Cloudflare trace endpoint response.
- Include true app version instead of `snapshot` in rpc `ImplementationVersion` metadata. [`#165`](https://github.com/project-vrcz/content-publisher/pull/165)
- New Task Page UI [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
  - Show accounts in tabs.
  - Show warning if account doesn't permission to publish content.
  - Show placeholder if no tasks exist for selected account.
  - Allow repair account in Tasks page if session is expired or invalid. [`#144`](https://github.com/project-vrcz/content-publisher/pull/144)
  - Show tip and button to login page in Tasks page if no accounts login. [`#141`](https://github.com/project-vrcz/content-publisher/pull/141)
- Better struct logging support [`#146`](https://github.com/project-vrcz/content-publisher/pull/146)
  - Include `Application`, `ApplicationVersion`, `ApplicationBuildDate`, `ApplicationCommitHash` globally. [`#147`](https://github.com/project-vrcz/content-publisher/pull/147)
  - Include `ClientName`, `ClientId` in RPC client request related log message.
  - Include `RpcClientIp`, `RpcClientPort`, `RpcHttpMethod`, `RpcHttpPath`, `RpcHttpQuery`, `RequestId` in RPC HTTP client request related log message.
  - Include `TaskStage`, `TaskId`, `ContentType`, `ContentName`, `ContentId`, `ContentPlatform`, `RawBundleFileId`, `FinalBundleFileId` in content publish task related log message. [`#164`](https://github.com/project-vrcz/content-publisher/pull/164)
  - Include `HttpClientInstanceName` in http request logging message sent from VRChat Api HttpClient.
- App will mark session as expired or invalid if got http 401 when request VRChat api. [`#144`](https://github.com/project-vrcz/content-publisher/pull/144)
- Show app build info (version, git commit, build date) and task id in error report window. [`#140`](https://github.com/project-vrcz/content-publisher/pull/140) [`#161`](https://github.com/project-vrcz/content-publisher/pull/161)
- Check is account valid before enter account repair page. [`#138`](https://github.com/project-vrcz/content-publisher/pull/138)
  - If account is valid, the account will be mark as repaired. No further operation requested.
- Acknowledgement for early adopters and open source softwares in Settings Page. [`#129`](https://github.com/project-vrcz/content-publisher/pull/129) [`#163`](https://github.com/project-vrcz/content-publisher/pull/163)
  - Also the software license.
- Logging when create publish task failed. [`#128`](https://github.com/project-vrcz/content-publisher/pull/128)

### Fixed

- App crash when any error occurred during account repair process. [`#138`](https://github.com/project-vrcz/content-publisher/pull/138)
- Unable to scroll in Tasks page. (Fix by replace with new ui) [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
- App keep trying get current user in some case, which trigger api rate limit. [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
- Unable to publish new platform build for exist world. [`#157`](https://github.com/project-vrcz/content-publisher/pull/157)
- Remove account button show has tasks running when no tasks running. [`#162`](https://github.com/project-vrcz/content-publisher/pull/162)

### Changes from `2.1.0-rc.1`

#### Added

- Select first account tab when open Tasks page. [`#175`](https://github.com/project-vrcz/content-publisher/pull/175)

## [2.1.0-rc.1] - 2026-01-09

### Changed

- Show Build Datetime in local time zone. [`#129`](https://github.com/project-vrcz/content-publisher/pull/129)

### Added

- Windows Installer will reuse last install location. [`#172`](https://github.com/project-vrcz/content-publisher/pull/172)
- Windows Installer will uninstall previous version before install. [`#172`](https://github.com/project-vrcz/content-publisher/pull/172)
- Windows Installer / Uninstaller check is app running before start. [`#171`](https://github.com/project-vrcz/content-publisher/pull/171)
- Require confirm before exit app if have active publish tasks. [`#170`](https://github.com/project-vrcz/content-publisher/pull/170)
- Network Diagnostics. [`#169`](https://github.com/project-vrcz/content-publisher/pull/169)
  - Check out VRChat API Status.
  - Test Connection to VRChat API, AWS S3, Cloudflare and Cloudflare China.
  - Check out Cloudflare trace endpoint response.
- Include true app version instead of `snapshot` in rpc `ImplementationVersion` metadata. [`#165`](https://github.com/project-vrcz/content-publisher/pull/165)
- New Task Page UI [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
  - Show accounts in tabs.
  - Show warning if account doesn't permission to publish content.
  - Show placeholder if no tasks exist for selected account.
  - Allow repair account in Tasks page if session is expired or invalid. [`#144`](https://github.com/project-vrcz/content-publisher/pull/144)
  - Show tip and button to login page in Tasks page if no accounts login. [`#141`](https://github.com/project-vrcz/content-publisher/pull/141)
- Better struct logging support [`#146`](https://github.com/project-vrcz/content-publisher/pull/146)
  - Include `Application`, `ApplicationVersion`, `ApplicationBuildDate`, `ApplicationCommitHash` globally. [`#147`](https://github.com/project-vrcz/content-publisher/pull/147)
  - Include `ClientName`, `ClientId` in RPC client request related log message.
  - Include `RpcClientIp`, `RpcClientPort`, `RpcHttpMethod`, `RpcHttpPath`, `RpcHttpQuery`, `RequestId` in RPC HTTP client request related log message.
  - Include `TaskStage`, `TaskId`, `ContentType`, `ContentName`, `ContentId`, `ContentPlatform`, `RawBundleFileId`, `FinalBundleFileId` in content publish task related log message. [`#164`](https://github.com/project-vrcz/content-publisher/pull/164)
  - Include `HttpClientInstanceName` in http request logging message sent from VRChat Api HttpClient.
- App will mark session as expired or invalid if got http 401 when request VRChat api. [`#144`](https://github.com/project-vrcz/content-publisher/pull/144)
- Show app build info (version, git commit, build date) and task id in error report window. [`#140`](https://github.com/project-vrcz/content-publisher/pull/140) [`#161`](https://github.com/project-vrcz/content-publisher/pull/161)
- Check is account valid before enter account repair page. [`#138`](https://github.com/project-vrcz/content-publisher/pull/138)
  - If account is valid, the account will be mark as repaired. No further operation requested.
- Acknowledgement for early adopters and open source softwares in Settings Page. [`#129`](https://github.com/project-vrcz/content-publisher/pull/129) [`#163`](https://github.com/project-vrcz/content-publisher/pull/163)
  - Also the software license.
- Logging when create publish task failed. [`#128`](https://github.com/project-vrcz/content-publisher/pull/128)

### Fixed

- App crash when any error occurred during account repair process. [`#138`](https://github.com/project-vrcz/content-publisher/pull/138)
- Unable to scroll in Tasks page. (Fix by replace with new ui) [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
- App keep trying get current user in some case, which trigger api rate limit. [`#154`](https://github.com/project-vrcz/content-publisher/pull/154)
- Unable to publish new platform build for exist world. [`#157`](https://github.com/project-vrcz/content-publisher/pull/157)
- Remove account button show has tasks running when no tasks running. [`#162`](https://github.com/project-vrcz/content-publisher/pull/162)

## [2.0.2] - 2026-01-02

### Fixed

- Content Publish will always failed due to forget to remove test code. [`#127`](https://github.com/project-vrcz/content-publisher/pull/127)

## [2.0.1] - 2026-01-02

### Fixed

- Won't retry when connect timeout error occurred. [`#125`](https://github.com/project-vrcz/content-publisher/pull/125)

### Changed

- Will give more detail information when upload process found file version with same md5. [`#126`](https://github.com/project-vrcz/content-publisher/pull/126)

## [2.0.0] - 2025-12-29

### Added

- Custom Http Proxy. [`#113`](https://github.com/project-vrcz/content-publisher/pull/113)
- Report create publish task error to rpc client. [`#115`](https://github.com/project-vrcz/content-publisher/pull/115)
- New Error Report Window for debug publish task failed. [`#122`](https://github.com/project-vrcz/content-publisher/pull/122)
- Allow open logs folder in tray icon context menu. [`#122`](https://github.com/project-vrcz/content-publisher/pull/122)

### Fixed

- Unable to remove invalid user session in settings. [`#116`](https://github.com/project-vrcz/content-publisher/pull/116)
- VRChat Api HttpClient won't retry in some case. [`#118`](https://github.com/project-vrcz/content-publisher/pull/118)

### Changed

- HttpClient no longer follow `Retry-After` header. [`#118`](https://github.com/project-vrcz/content-publisher/pull/118)
- Rename to `VRChat Content Publisher`. [`#119`](https://github.com/project-vrcz/content-publisher/pull/119)
  - You must uninstall old version to install new version. (You can keep your user data)

## [1.3.0] - 2025-12-23

### Added

- Add Unity Setup Guide [`#110`](https://github.com/project-vrcz/content-publisher/pull/110)
  - Include install connect package, connect unity to app.
  - You can directly jump to home page if you connect unity to app during guide.

## [1.2.0] - 2025-12-18

### Added

- Support `ready-for-publish` health check endpoint for RPC. [`104`](https://github.com/project-vrcz/content-publisher/pull/105)
- Launch App by URL protocol `vrchat-content-manager://launch`. (Windows-only for now) [`#104`](https://github.com/project-vrcz/content-publisher/pull/104)
- Windows Installer (NSIS). [`#101`](https://github.com/project-vrcz/content-publisher/pull/101)
- Single Instance. [`#103`](https://github.com/project-vrcz/content-publisher/pull/103)
  - Prevent launch new instance when another intance already exist.
  - Bring up existing instance's main window.

## [1.1.0] - 2025-12-11

### Changed

- Insert new task to the beginning of the task list. [`#89`](https://github.com/project-vrcz/content-publisher/pull/89)
- Challenge Code will Always uppercase. [`#93`](https://github.com/project-vrcz/content-publisher/pull/93)
- Allow copy challenge code in request challenge dialog. [`#94`](https://github.com/project-vrcz/content-publisher/pull/94)

## [1.0.0] - 2025-12-08

### Added

- Show App version, commit hash and build date in App settings page [`#70`](https://github.com/project-vrcz/content-publisher/pull/70).
- Basic Linux Support [`#76`](https://github.com/project-vrcz/content-publisher/pull/76)

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
- Adjust http rqeuest pipeline [`#80`](https://github.com/project-vrcz/content-publisher/pull/80)
  - Use DecorrelatedJitterV2 as http request retry strategy
  - Increase retry delay
  - Increase MaxConnectionsPerServer to 256 from 10 for AWS S3 HttpClient

## [1.0.0-rc.1] - 2025-12-07

### Added

- Show App version, commit hash and build date in App settings page [`#70`](https://github.com/project-vrcz/content-publisher/pull/70).
- Basic Linux Support [`#76`](https://github.com/project-vrcz/content-publisher/pull/76)

### Changed

- Adjust http rqeuest pipeline [`#80`](https://github.com/project-vrcz/content-publisher/pull/80)
  - Use DecorrelatedJitterV2 as http request retry strategy
  - Increase retry delay
  - Increase MaxConnectionsPerServer to 256 from 10 for AWS S3 HttpClient

[unreleased]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0-beta.2...HEAD
[2.9.0-beta.2]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0-beta.1...v2.9.0-beta.2
[2.9.0-beta.1]: https://github.com/project-vrcz/content-publisher/compare/v2.8.2...v2.9.0-beta.1
[2.8.2]: https://github.com/project-vrcz/content-publisher/compare/v2.8.1...v2.8.2
[2.8.1]: https://github.com/project-vrcz/content-publisher/compare/v2.8.0...v2.8.1
[2.8.0]: https://github.com/project-vrcz/content-publisher/compare/v2.7.0...v2.8.0
[2.7.0]: https://github.com/project-vrcz/content-publisher/compare/v2.6.1...v2.7.0
[2.6.1]: https://github.com/project-vrcz/content-publisher/compare/v2.6.0...v2.6.1
[2.6.0]: https://github.com/project-vrcz/content-publisher/compare/v2.5.0...v2.6.0
[2.5.0]: https://github.com/project-vrcz/content-publisher/compare/v2.4.2...v2.5.0
[2.4.2]: https://github.com/project-vrcz/content-publisher/compare/v2.4.1...v2.4.2
[2.4.1]: https://github.com/project-vrcz/content-publisher/compare/v2.4.0...v2.4.1
[2.4.0]: https://github.com/project-vrcz/content-publisher/compare/v2.3.0...v2.4.0
[2.3.0]: https://github.com/project-vrcz/content-publisher/compare/v2.2.2...v2.3.0
[2.2.2]: https://github.com/project-vrcz/content-publisher/compare/v2.2.1...v2.2.2
[2.2.1]: https://github.com/project-vrcz/content-publisher/compare/v2.2.0...v2.2.1
[2.2.0]: https://github.com/project-vrcz/content-publisher/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/project-vrcz/content-publisher/compare/v2.1.0-rc.1...v2.1.0
[2.1.0-rc.1]: https://github.com/project-vrcz/content-publisher/compare/v2.0.2...v2.1.0-rc.1
[2.0.2]: https://github.com/project-vrcz/content-publisher/compare/v2.0.1...v2.0.2
[2.0.1]: https://github.com/project-vrcz/content-publisher/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/project-vrcz/content-publisher/compare/v1.3.0...v2.0.0
[1.3.0]: https://github.com/project-vrcz/content-publisher/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/project-vrcz/content-publisher/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/project-vrcz/content-publisher/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/project-vrcz/content-publisher/compare/v1.0.0-rc.1...v1.0.0
[1.0.0-rc.1]: https://github.com/project-vrcz/content-publisher/releases/tag/v1.0.0-rc.1
