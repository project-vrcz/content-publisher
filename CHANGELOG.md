# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

## [2.12.2] - 2026-10-01

### Changed

- Cancel update no longer require confirm. [`#539`](https://github.com/project-vrcz/content-publisher/pull/539)

### Fixed

- Cancel update and retry download update will crash the app. [`#539`](https://github.com/project-vrcz/content-publisher/pull/539)
- Cancel update downloading will show as update download error. [`#539`](https://github.com/project-vrcz/content-publisher/pull/539)
- Unable to show any dialogs. [`#538`](https://github.com/project-vrcz/content-publisher/pull/538)

## [2.12.1] - 2026-09-29

### Fixed

- Exit a page won't execute it's unload logic, which result in: [`#536`](https://github.com/project-vrcz/content-publisher/pull/536)
  - As long as you've opened the paring page in onboarding, paring new client will also navigate to last page of onboarding page, until the app is restarted.
  - Increased memory usage.
  - Random hang up or low fps.
  - Random crash.

## [2.12.0] - 2026-09-29

### Changed

- Show detail error information returned by AWS S3 when file upload failed. [`#518`](https://github.com/project-vrcz/content-publisher/pull/518)

### Added

- Bring the main window of the running instance to the foreground when a desktop notification is clicked. [`#520`](https://github.com/project-vrcz/content-publisher/pull/520)
- Add a option to allow send task completed successfully notification. [`#517`](https://github.com/project-vrcz/content-publisher/pull/517)
- Allow manually refresh session state from accounts settings page and home page account tab. [`#521`](https://github.com/project-vrcz/content-publisher/issues/521) [`#522`](https://github.com/project-vrcz/content-publisher/pull/522)
- Add a Client Pairing Guide button in RPC server settings to open the client pairing onboarding, with back navigation to return to the settings page. [`#523`](https://github.com/project-vrcz/content-publisher/pull/523)
- Allow copying the RPC server port from the tray menu and from RPC server settings. [`#523`](https://github.com/project-vrcz/content-publisher/pull/523)
- Add a status bar at the bottom of the home page showing the aggregated task counts of all accounts and the RPC server port with a copy button. [`#523`](https://github.com/project-vrcz/content-publisher/pull/523)
- Show a reminder dialog when onboarding cannot launch the package manager with `vcc://` protocol. [`#526`](https://github.com/project-vrcz/content-publisher/pull/526)
- Add a final onboarding page telling users to publish from the VRChat SDK Build and Upload button. [`#528`](https://github.com/project-vrcz/content-publisher/pull/528)
- Show the proxy the app detected in the network diagnostics window. [`#530`](https://github.com/project-vrcz/content-publisher/pull/530)
  - Whether a proxy is used.
  - The app proxy mode.
  - The system proxy source.
  - The effective proxy for each destination.
- Add `GET /v1/user-sessions/validity` RPC API for clients to check whether a VRChat account session is still valid. [`#533`](https://github.com/project-vrcz/content-publisher/pull/533)

### Fixed

- App freezes for a moment when opening the settings page. [`#532`](https://github.com/project-vrcz/content-publisher/pull/532)
- Multipart upload won't abort when a single chunk upload failed. [`#518`](https://github.com/project-vrcz/content-publisher/pull/518)

## [2.11.0] - 2026-09-09

### Changed

- Change max retry attempts of `ConcurrentMultipartUploader` from 3 to 5. [`#497`](https://github.com/project-vrcz/content-publisher/pull/497)
- Use Avalonia API instead of Win32 API workaround to detect working area (or taskbar position and size) changed.
  - It should make no different.
  - Just to satisfy the maintainer's vanity: Maintainer fix the Avalonia API by submit patch to Avalonia: [AvaloniaUI/Avalonia/pull/21707](https://github.com/AvaloniaUI/Avalonia/pull/21707)

### Added

- Include exception details in logging. [`#495`](https://github.com/project-vrcz/content-publisher/pull/495)
- Include installation id in OpenTelemetry tracing. [`#495`](https://github.com/project-vrcz/content-publisher/pull/495)
- Save failed to send open telemetry trace data to disk for retry later. [`#498`](https://github.com/project-vrcz/content-publisher/pull/498)
- Async SQLite database operation. [`#510`](https://github.com/project-vrcz/content-publisher/pull/510)
  - It should improve the performance for batch operation. (like Remove all completed tasks)

## Fixed

- Click "Add to the package manager" button with `vcc://` protocol registered but protocol handler file not exist will crash the app. [`#499`](https://github.com/project-vrcz/content-publisher/pull/499)
  - Another line just to satisfy the maintainer's vanity: Maintainer fix this issue by submit patch to Avalonia: [AvaloniaUI/Avalonia/pull/21704](https://github.com/AvaloniaUI/Avalonia/pull/21704)
- In rare case, crash during save telemetry settings will result it be reset. [`#496`](https://github.com/project-vrcz/content-publisher/pull/496)

## [2.10.1] - 2026-06-26

### Fixed

- Unable to repair account using cookies. [`#477`](https://github.com/project-vrcz/content-publisher/pull/477)
- Upgrade from version older than `v2.10.0-beta.3` will create duplicated shortcut in start menu. [`#476`](https://github.com/project-vrcz/content-publisher/pull/476)
- Download update failed won't show actual error message. [`#478`](https://github.com/project-vrcz/content-publisher/pull/478)

## [2.10.0] - 2026-06-25

### Changed

- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.
- Reduced time required for looking for avatar owner when create avatar publish task. [`#406`](https://github.com/project-vrcz/content-publisher/pull/406)
  - You must upgrade connect package avatar pack to v0.5.2 in order to use this feature.
- Attempt id to identify each attempt of publishing. [`#420`](https://github.com/project-vrcz/content-publisher/pull/420)
  - Attempt id will show in error report and log (JSON).
- User session can be restored if user information cache available. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)
  - User information cache should always available unless you upgrade from version older than v2.2.0.
- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Remove ip changes detect logging encrypt. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
- Update available dialog are show fullscreen in App now. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)
- OpenTelemetry and Sentry support. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424) [`#471`](https://github.com/project-vrcz/content-publisher/pull/471)
  - For how telemetry works, see [PRIVACY POLICY](https://github.com/project-vrcz/content-publisher/blob/main/docs/privacy/PRIVACY.md).
  - You can also set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable if you want to see the tracing data.
- Support taskbar with position not at the bottom. [`#432`](https://github.com/project-vrcz/content-publisher/pull/432)
- Chinese support for crash handler. [`#397`](https://github.com/project-vrcz/content-publisher/pull/397)
- Use enter key to focus on password textbox and execute login command. [`#394`](https://github.com/project-vrcz/content-publisher/pull/394)
- Will show tooltip for task creation time. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- Add `ApplicationLifetimeSession` and `app.lifetime_session_id` to logging and tracing to identify each session of app.
- Allow choice app language in onboarding. [`#399`](https://github.com/project-vrcz/content-publisher/pull/399)
- Send desktop notification to tell user app hide to system tray when window hide at first time. [`#472`](https://github.com/project-vrcz/content-publisher/pull/472)
- Preview changelog rendered from markdown in update dialog. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)
- Show restart onboarding in-app notification when skip onboarding on first launch. [`#396`](https://github.com/project-vrcz/content-publisher/pull/396)
- Add request background public ip check run button in debug settings. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)

### Fixed

- If login fail during account repair, the repairing session will be remove. [`#454`](https://github.com/project-vrcz/content-publisher/pull/454)
  - You will lost all content publish tasks.
- Task will fail without retry if request multipart upload url fail. [`#470`](https://github.com/project-vrcz/content-publisher/pull/470)
- Apply a used port in RPC Server Settings will crash the App. [`#455`](https://github.com/project-vrcz/content-publisher/pull/455)
- Upload speed are unreliable in some case. [`#364`](https://github.com/project-vrcz/content-publisher/issues/364)
- Task will always fail due to retry non-idempotent requests. [`#411`](https://github.com/project-vrcz/content-publisher/pull/411)
  - (caused by timeout or connection abort during received response)
- In some case app crash will corrupt settings file.
  - It will result in app unable to start. [`#410`](https://github.com/project-vrcz/content-publisher/pull/410)
- Tasks may sort incorrectly when reload tasks page. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.
- In some case request handle error will result in uncompleted or invalid http response. [`#417`](https://github.com/project-vrcz/content-publisher/pull/417)
- Request with invalid jwt will get HTTP 500 response instead of HTTP 401. [`#413`](https://github.com/project-vrcz/content-publisher/pull/413)
- Public IP check require restart to turn on of turn off. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)
- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379) [`#433`](https://github.com/project-vrcz/content-publisher/pull/433)
- Incorrect tooltip for cannot remove account with existing tasks warning. [`#457`](https://github.com/project-vrcz/content-publisher/pull/457)
  - Previously the tooltip said you cannot remove account with uncompleted tasks, but actually you cannot remove account with existing tasks, even if all tasks are completed.
- Background service or host crash won't stop App. [`#462`](https://github.com/project-vrcz/content-publisher/issues/461) [`#465`](https://github.com/project-vrcz/content-publisher/pull/465)

### Changes from [2.10.0-beta.7]

#### Fixed

- Will only send tracing data when telemetry disabled. [`#453`](https://github.com/project-vrcz/content-publisher/pull/453)
  - No tracing data will send will telemetry enabled. Oops...
- App may crash when session invalid happened with public IP change check is enabled. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)
- `UserNameOrEmail` property won't be masked in telemetry logging. [`#460`](https://github.com/project-vrcz/content-publisher/pull/460)

### Changes from [2.10.0-beta.6]

#### Fixed

- Sentry telemetry won't sent any data due to misconfiguration.
- Misleading Text in Public IP Change Notification Settings. [`#442`](https://github.com/project-vrcz/content-publisher/pull/442)

### Changes from [2.10.0-beta.5]

#### Fixed

- Simple upload request won't retry after fail. [`#418`](https://github.com/project-vrcz/content-publisher/pull/418)
- Unable to start App from url protocol.

### Changes from [2.10.0-beta.4]

#### Added

- Chinese Simplified language support for windows installer. [`#393`](https://github.com/project-vrcz/content-publisher/pull/393)

#### Fixed

- Tasks can't be restored if user session expired or invalid during startup. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-rc.1] - 2026-06-20

### Changed

- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.
- Reduced time required for looking for avatar owner when create avatar publish task. [`#406`](https://github.com/project-vrcz/content-publisher/pull/406)
  - You must upgrade connect package avatar pack to v0.5.2 in order to use this feature.
- Attempt id to identify each attempt of publishing. [`#420`](https://github.com/project-vrcz/content-publisher/pull/420)
  - Attempt id will show in error report and log (JSON).
- User session can be restored if user information cache available. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)
  - User information cache should always available unless you upgrade from version older than v2.2.0.
- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Remove ip changes detect logging encrypt. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
- Update available dialog are show fullscreen in App now. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)
- OpenTelemetry and Sentry support. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
  - For how telemetry works, see [PRIVACY POLICY](https://github.com/project-vrcz/content-publisher/blob/main/docs/privacy/PRIVACY.md).
  - You can also set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable if you want to see the tracing data.
- Support taskbar with position not at the bottom. [`#432`](https://github.com/project-vrcz/content-publisher/pull/432)
- Chinese support for crash handler. [`#397`](https://github.com/project-vrcz/content-publisher/pull/397)
- Use enter key to focus on password textbox and execute login command. [`#394`](https://github.com/project-vrcz/content-publisher/pull/394)
- Will show tooltip for task creation time. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- Add `ApplicationLifetimeSession` and `app.lifetime_session_id` to logging and tracing to identify each session of app.
- Allow choice app language in onboarding. [`#399`](https://github.com/project-vrcz/content-publisher/pull/399)
- Preview changelog rendered from markdown in update dialog. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)
- Show restart onboarding in-app notification when skip onboarding on first launch. [`#396`](https://github.com/project-vrcz/content-publisher/pull/396)
- Add request background public ip check run button in debug settings. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)

### Fixed

- If login fail during account repair, the repairing session will be remove. [`#454`](https://github.com/project-vrcz/content-publisher/pull/454)
  - You will lost all content publish tasks.
- Apply a used port in RPC Server Settings will crash the App. [`#455`](https://github.com/project-vrcz/content-publisher/pull/455)
- Upload speed are unreliable in some case. [`#364`](https://github.com/project-vrcz/content-publisher/issues/364)
- Task will always fail due to retry non-idempotent requests. [`#411`](https://github.com/project-vrcz/content-publisher/pull/411)
  - (caused by timeout or connection abort during received response)
- In some case app crash will corrupt settings file.
  - It will result in app unable to start. [`#410`](https://github.com/project-vrcz/content-publisher/pull/410)
- Tasks may sort incorrectly when reload tasks page. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.
- In some case request handle error will result in uncompleted or invalid http response. [`#417`](https://github.com/project-vrcz/content-publisher/pull/417)
- Request with invalid jwt will get HTTP 500 response instead of HTTP 401. [`#413`](https://github.com/project-vrcz/content-publisher/pull/413)
- Public IP check require restart to turn on of turn off. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)
- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379) [`#433`](https://github.com/project-vrcz/content-publisher/pull/433)
- Incorrect tooltip for cannot remove account with existing tasks warning. [`#457`](https://github.com/project-vrcz/content-publisher/pull/457)
  - Previously the tooltip said you cannot remove account with uncompleted tasks, but actually you cannot remove account with existing tasks, even if all tasks are completed.
- Background service or host crash won't stop App. [`#462`](https://github.com/project-vrcz/content-publisher/issues/461)

### Changes from [2.10.0-beta.7]

#### Fixed

- Will only send tracing data when telemetry disabled. [`#453`](https://github.com/project-vrcz/content-publisher/pull/453)
  - No tracing data will send will telemetry enabled. Oops...
- App may crash when session invalid happened with public IP change check is enabled. [`#456`](https://github.com/project-vrcz/content-publisher/pull/456)
- `UserNameOrEmail` property won't be masked in telemetry logging. [`#460`](https://github.com/project-vrcz/content-publisher/pull/460)

### Changes from [2.10.0-beta.6]

#### Fixed

- Sentry telemetry won't sent any data due to misconfiguration.
- Misleading Text in Public IP Change Notification Settings. [`#442`](https://github.com/project-vrcz/content-publisher/pull/442)

### Changes from [2.10.0-beta.5]

#### Fixed

- Simple upload request won't retry after fail. [`#418`](https://github.com/project-vrcz/content-publisher/pull/418)
- Unable to start App from url protocol.

### Changes from [2.10.0-beta.4]

#### Added

- Chinese Simplified language support for windows installer. [`#393`](https://github.com/project-vrcz/content-publisher/pull/393)

#### Fixed

- Tasks can't be restored if user session expired or invalid during startup. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.7] - 2026-06-17

### Changed

- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.
- Reduced time required for looking for avatar owner when create avatar publish task. [`#406`](https://github.com/project-vrcz/content-publisher/pull/406)
  - You must upgrade connect package avatar pack to v0.5.2 in order to use this feature.
- Attempt id to identify each attempt of publishing. [`#420`](https://github.com/project-vrcz/content-publisher/pull/420)
  - Attempt id will show in error report and log (JSON).
- User session can be restored if user information cache available. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)
  - User information cache should always available unless you upgrade from version older than v2.2.0.
- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Remove ip changes detect logging encrypt. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
- Update available dialog are show fullscreen in App now. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)
- OpenTelemetry and Sentry support. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
  - For how telemetry works, see [PRIVACY POLICY](https://github.com/project-vrcz/content-publisher/blob/main/docs/privacy/PRIVACY.md).
  - You can also set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable if you want to see the tracing data.
- Support taskbar with position not at the bottom. [`#432`](https://github.com/project-vrcz/content-publisher/pull/432)
- Chinese support for crash handler. [`#397`](https://github.com/project-vrcz/content-publisher/pull/397)
- Use enter key to focus on password textbox and execute login command. [`#394`](https://github.com/project-vrcz/content-publisher/pull/394)
- Will show tooltip for task creation time. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- Allow choice app language in onboarding. [`#399`](https://github.com/project-vrcz/content-publisher/pull/399)
- Preview changelog rendered from markdown in update dialog. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)
- Show restart onboarding in-app notification when skip onboarding on first launch. [`#396`](https://github.com/project-vrcz/content-publisher/pull/396)

### Fixed

- Upload speed are unreliable in some case. [`#364`](https://github.com/project-vrcz/content-publisher/issues/364)
- Task will always fail due to retry non-idempotent requests. [`#411`](https://github.com/project-vrcz/content-publisher/pull/411)
  - (caused by timeout or connection abort during received response)
- In some case app crash will corrupt settings file.
  - It will result in app unable to start. [`#410`](https://github.com/project-vrcz/content-publisher/pull/410)
- Tasks may sort incorrectly when reload tasks page. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.
- In some case request handle error will result in uncompleted or invalid http response. [`#417`](https://github.com/project-vrcz/content-publisher/pull/417)
- Request with invalid jwt will get HTTP 500 response instead of HTTP 401. [`#413`](https://github.com/project-vrcz/content-publisher/pull/413)
- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379) [`#433`](https://github.com/project-vrcz/content-publisher/pull/433)

### Changes from [2.10.0-beta.6]

#### Fixed

- Sentry telemetry won't sent any data due to misconfiguration.
- Misleading Text in Public IP Change Notification Settings. [`#442`](https://github.com/project-vrcz/content-publisher/pull/442)

### Changes from [2.10.0-beta.5]

#### Fixed

- Simple upload request won't retry after fail. [`#418`](https://github.com/project-vrcz/content-publisher/pull/418)
- Unable to start App from url protocol.

### Changes from [2.10.0-beta.4]

#### Added

- Chinese Simplified language support for windows installer. [`#393`](https://github.com/project-vrcz/content-publisher/pull/393)

#### Fixed

- Tasks can't be restored if user session expired or invalid during startup. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.6] - 2026-06-17

### Changed

- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.
- Reduced time required for looking for avatar owner when create avatar publish task. [`#406`](https://github.com/project-vrcz/content-publisher/pull/406)
  - You must upgrade connect package avatar pack to v0.5.2 in order to use this feature.
- Attempt id to identify each attempt of publishing. [`#420`](https://github.com/project-vrcz/content-publisher/pull/420)
  - Attempt id will show in error report and log (JSON).
- User session can be restored if user information cache available. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)
  - User information cache should always available unless you upgrade from version older than v2.2.0.
- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Remove ip changes detect logging encrypt. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
- Update available dialog are show fullscreen in App now. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)
- OpenTelemetry and Sentry support. [`#424`](https://github.com/project-vrcz/content-publisher/pull/424)
  - For how telemetry works, see [PRIVACY POLICY](https://github.com/project-vrcz/content-publisher/blob/main/docs/privacy/PRIVACY.md).
  - You can also set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable if you want to see the tracing data.
- Support taskbar with position not at the bottom. [`#432`](https://github.com/project-vrcz/content-publisher/pull/432)
- Chinese support for crash handler. [`#397`](https://github.com/project-vrcz/content-publisher/pull/397)
- Use enter key to focus on password textbox and execute login command. [`#394`](https://github.com/project-vrcz/content-publisher/pull/394)
- Will show tooltip for task creation time. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- Allow choice app language in onboarding. [`#399`](https://github.com/project-vrcz/content-publisher/pull/399)
- Preview changelog rendered from markdown in update dialog. [`#435`](https://github.com/project-vrcz/content-publisher/pull/435)
- Show restart onboarding in-app notification when skip onboarding on first launch. [`#396`](https://github.com/project-vrcz/content-publisher/pull/396)

### Fixed

- Upload speed are unreliable in some case. [`#364`](https://github.com/project-vrcz/content-publisher/issues/364)
- Task will always fail due to retry non-idempotent requests. [`#411`](https://github.com/project-vrcz/content-publisher/pull/411)
  - (caused by timeout or connection abort during received response)
- In some case app crash will corrupt settings file.
  - It will result in app unable to start. [`#410`](https://github.com/project-vrcz/content-publisher/pull/410)
- Tasks may sort incorrectly when reload tasks page. [`#403`](https://github.com/project-vrcz/content-publisher/pull/403)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.
- In some case request handle error will result in uncompleted or invalid http response. [`#417`](https://github.com/project-vrcz/content-publisher/pull/417)
- Request with invalid jwt will get HTTP 500 response instead of HTTP 401. [`#413`](https://github.com/project-vrcz/content-publisher/pull/413)
- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379) [`#433`](https://github.com/project-vrcz/content-publisher/pull/433)

### Changes from [2.10.0-beta.5]

#### Fixed

- Simple upload request won't retry after fail. [`#418`](https://github.com/project-vrcz/content-publisher/pull/418)
- Unable to start App from url protocol.

### Changes from [2.10.0-beta.4]

#### Added

- Chinese Simplified language support for windows installer. [`#393`](https://github.com/project-vrcz/content-publisher/pull/393)

#### Fixed

- Tasks can't be restored if user session expired or invalid during startup. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.5] - 2026-05-23

### Changed

- User session can be restored if user information cache available. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)
  - User information cache should always available unless you upgrade from version older than v2.2.0.
- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.

### Added

- Chinese support for crash handler. [`#397`](https://github.com/project-vrcz/content-publisher/pull/397)
- Allow choice app language in onboarding. [`#399`](https://github.com/project-vrcz/content-publisher/pull/399)
- Use enter key to focus on password textbox and execute login command. [`#394`](https://github.com/project-vrcz/content-publisher/pull/394)
- Show restart onboarding in-app notification when skip onboarding on first launch. [`#396`](https://github.com/project-vrcz/content-publisher/pull/396)
- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)

### Fixed

- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.

### Changes from [2.10.0-beta.4]

#### Added

- Chinese Simplified language support for windows installer. [`#393`](https://github.com/project-vrcz/content-publisher/pull/393)

#### Fixed

- Tasks can't be restored if user session expired or invalid during startup. [`#398`](https://github.com/project-vrcz/content-publisher/pull/398)

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.4] - 2026-05-20

### Changed

- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)

### Fixed

- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.

### Changes from [2.10.0-beta.3]

#### Fixed

- Software upgrade feature was accidentally disabled for installer version. [`#383`](https://github.com/project-vrcz/content-publisher/pull/383)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.3] - 2026-05-20

### Changed

- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)
- Replace the Windows installer from NSIS with Inno Setup. [`#377`](https://github.com/project-vrcz/content-publisher/pull/377)
  - If you want to downgrade to any version older than `v2.10.0-beta.3`, you **MUST** uninstall newer version first.
  - No more PowerShell window during update.
  - High DPI and Dark mode support.

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)

### Fixed

- Log spam when app crash. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
- In some case app may partial start. [`#379`](https://github.com/project-vrcz/content-publisher/pull/379)
  - It can result in malfunctional or crash.

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.2] - 2026-05-18

### Changed

- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)
- Extend period of background update check to a hour. [`#371`](https://github.com/project-vrcz/content-publisher/pull/371)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.
  - Options to remove task from database after it completed. [`#373`](https://github.com/project-vrcz/content-publisher/pull/373)

### Changes from [2.10.0-beta.1]

#### Fixed

- App crash after click download update. [`#375`](https://github.com/project-vrcz/content-publisher/pull/375)

## [2.10.0-beta.1] - 2026-05-16

### Changed

- All file related to publish task (raw bundle, compressed bundle, etc) are storage in`%LOCALAPPDATA%\vrchat-content-manager-81b7bca3\rpc-files`. [`#365`](https://github.com/project-vrcz/content-publisher/pull/365)

### Added

- Tasks can be restore after crash or restarted. [`#367`](https://github.com/project-vrcz/content-publisher/pull/367)
  - Progress of all restored tasks will end with "Waiting for start".
  - You need to manually start restored tasks.
  - You can chose to retry or remove all restored (Pending) tasks.

## [2.9.4] - 2026-05-14

### Changed

- Onboarding will skip account login if any account exist. [`#352`](https://github.com/project-vrcz/content-publisher/pull/352)
- Rework About page. [`#355`](https://github.com/project-vrcz/content-publisher/pull/355)

### Added

- Show upload speed and percentage for file uploading. [`#347`](https://github.com/project-vrcz/content-publisher/pull/347)
- Restart onboarding from settings page. [`#352`](https://github.com/project-vrcz/content-publisher/pull/352)
- Open settings page from onboarding welcome page. [`#352`](https://github.com/project-vrcz/content-publisher/pull/352)

### Fixed

- Upload progress will become unreliable if request retry occurred. [`#347`](https://github.com/project-vrcz/content-publisher/pull/347)
- App crash randomly in some case. (Due to dispose Bitmap in wrong time). [`#349`](https://github.com/project-vrcz/content-publisher/pull/349)
- App icon in onboarding page use wrong color in light mode. [`#357`](https://github.com/project-vrcz/content-publisher/pull/357)

## [2.9.4-beta.1] - 2026-05-12

### Added

- Show upload speed and percentage for file uploading. [`#347`](https://github.com/project-vrcz/content-publisher/pull/347)

### Fixed

- Upload progress will become unreliable if request retry occurred. [`#347`](https://github.com/project-vrcz/content-publisher/pull/347)
- App crash randomly in some case. (Due to dispose Bitmap in wrong time). [`#349`](https://github.com/project-vrcz/content-publisher/pull/349)

## [2.9.3] - 2026-05-08

### Changed

- No notification will sent if first 64 bits prefix remain same after IPv6 address changed. [`#344`](https://github.com/project-vrcz/content-publisher/pull/344)
  - The change will still print to log.

### Added

- Toggle for public ip checker. [`#342`](https://github.com/project-vrcz/content-publisher/pull/342).
- Allow cancel update and delete downloaded file after update downloaded. [`#343`](https://github.com/project-vrcz/content-publisher/pull/343)

### Fixed

- App will try downgrade itself if remote app version lower than local app version. [`#339`](https://github.com/project-vrcz/content-publisher/pull/339)

## [2.9.2] - 2026-05-07

### Changed

- Disable download and install update feature for Linux and portable version. [`#334`](https://github.com/project-vrcz/content-publisher/pull/334)

### Fixed

- All file uploading task took over 30s will be forced abort. [`#332`](https://github.com/project-vrcz/content-publisher/pull/332)
- App crash on startup when download update background enabled or click Download Update on Linux. [`#334`](https://github.com/project-vrcz/content-publisher/pull/334)

## [2.9.1] - 2026-05-04

### Added

- Show alert for platform with no desktop notification support. [`#325`](https://github.com/project-vrcz/content-publisher/pull/325)

### Fixed

- Failure to initialize notification service will cause the app to crash. [`#325`](https://github.com/project-vrcz/content-publisher/pull/325)
- Multi-thread download accidentally use single connection when download update. [`#326`](https://github.com/project-vrcz/content-publisher/pull/326)

## [2.9.0] - 2026-04-29

### Added

- Software upgrade. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
  - Send in-app notification when app update available.
  - Show changes in app.
  - Skip specify version.
  - Update check run on start or each 30mins in background. (optional)
  - Download update in background. (optional)
- Toggle pinned or borderless window in tray menu. [`#291`](https://github.com/project-vrcz/content-publisher/pull/291)
- Add loading text to bootstrap screen.
- Send desktop notification on new pairing request [`#296`](https://github.com/project-vrcz/content-publisher/issues/298)
- Show crash report when app crashed. [`#305`](https://github.com/project-vrcz/content-publisher/pull/305)

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)
- Adjust margin between main window and screen bounds. [`#301`](https://github.com/project-vrcz/content-publisher/pull/301)
- Use LzmaCon Binaries from [project-vrcz/lzma](https://github.com/project-vrcz/lzma/releases/tag/v26.00-vrcz.2). [`#309`](https://github.com/project-vrcz/content-publisher/pull/309)
  - Added LZMA support for glibc 2.31 (Previous glibc 2.41).
  - You **STILL** need glibc 2.39 to run this app.
  - Slightly improve compression performance for linux build (by 4%~).
- Support glibc 2.39 (untested). [`#309`](https://github.com/project-vrcz/content-publisher/pull/309)

### Fixed

- Cannot exit app when dialog show. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
- Abnormal shutdown during web request may corrupted session storage in some case. [`#292`](https://github.com/project-vrcz/content-publisher/pull/292)
- Click tray icon only make window focused instead of bring window to front when window minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)
- Enable borderless window won't reset window state to normal from maximized or minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)
- Exception that cause app crash won't print to log in some case. [`#305`](https://github.com/project-vrcz/content-publisher/pull/305)

## [2.9.0-rc.1] - 2026-04-24

### Added

- Software upgrade. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
  - Send in-app notification when app update available.
  - Show changes in app.
  - Skip specify version.
  - Update check run on start or each 30mins in background. (optional)
  - Download update in background. (optional)
- Toggle pinned or borderless window in tray menu. [`#291`](https://github.com/project-vrcz/content-publisher/pull/291)
- Add loading text to bootstrap screen.
- Send desktop notification on new pairing request [`#296`](https://github.com/project-vrcz/content-publisher/issues/298)
- Show crash report when app crashed. [`#305`](https://github.com/project-vrcz/content-publisher/pull/305)

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)
- Adjust margin between main window and screen bounds. [`#301`](https://github.com/project-vrcz/content-publisher/pull/301)
- Use LzmaCon Binaries from [project-vrcz/lzma](https://github.com/project-vrcz/lzma/releases/tag/v26.00-vrcz.2). [`#309`](https://github.com/project-vrcz/content-publisher/pull/309)
  - Added LZMA support for glibc 2.31 (Previous glibc 2.41).
  - You **STILL** need glibc 2.39 to run this app.
  - Slightly improve compression performance for linux build (by 104%~).
- Support glibc 2.39 (untested). [`#309`](https://github.com/project-vrcz/content-publisher/pull/309)

### Fixed

- Cannot exit app when dialog show. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
- Abnormal shutdown during web request may corrupted session storage in some case. [`#292`](https://github.com/project-vrcz/content-publisher/pull/292)
- Click tray icon only make window focused instead of bring window to front when window minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)
- Enable borderless window won't reset window state to normal from maximized or minimized. [`#295`](https://github.com/project-vrcz/content-publisher/pull/295)
- Exception that cause app crash won't print to log in some case. [`#305`](https://github.com/project-vrcz/content-publisher/pull/305)

## [2.9.0-beta.3] - 2026-04-23

### Added

- Software upgrade. [`#287`](https://github.com/project-vrcz/content-publisher/pull/287)
  - Send in-app notification when app update available.
  - Show changes in app.
  - Skip specify version.
  - Update check run on start or each 30mins in background. (optional)
  - Download update in background. (optional)
- Toggle pinned or borderless window in tray menu. [`#291`](https://github.com/project-vrcz/content-publisher/pull/291)
- Add loading text to bootstrap screen.
- Send desktop notification on new pairing request [`#296`](https://github.com/project-vrcz/content-publisher/issues/298)

### Changed

- Rework Login with Cookies input fields. [`#275`](https://github.com/project-vrcz/content-publisher/pull/275)
- Update Use RGB Cycling background App Bar settings text for chinese. [`#276`](https://github.com/project-vrcz/content-publisher/pull/276)
- Adjust margin between main window and screen bounds. [`#301`](https://github.com/project-vrcz/content-publisher/pull/301)

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

[unreleased]: https://github.com/project-vrcz/content-publisher/compare/v2.12.2...HEAD
[2.12.2]: https://github.com/project-vrcz/content-publisher/compare/v2.12.1...v2.12.2
[2.12.1]: https://github.com/project-vrcz/content-publisher/compare/v2.12.0...v2.12.1
[2.12.0]: https://github.com/project-vrcz/content-publisher/compare/v2.11.0...v2.12.0
[2.11.0]: https://github.com/project-vrcz/content-publisher/compare/v2.10.1...v2.11.0
[2.10.1]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0...v2.10.1
[2.10.0]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-rc.1...v2.10.0
[2.10.0-rc.1]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.7...v2.10.0-rc.1
[2.10.0-beta.7]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.6...v2.10.0-beta.7
[2.10.0-beta.6]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.5...v2.10.0-beta.6
[2.10.0-beta.5]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.4...v2.10.0-beta.5
[2.10.0-beta.4]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.3...v2.10.0-beta.4
[2.10.0-beta.3]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.2...v2.10.0-beta.3
[2.10.0-beta.2]: https://github.com/project-vrcz/content-publisher/compare/v2.10.0-beta.1...v2.10.0-beta.2
[2.10.0-beta.1]: https://github.com/project-vrcz/content-publisher/compare/v2.9.4...v2.10.0-beta.1
[2.9.4]: https://github.com/project-vrcz/content-publisher/compare/v2.9.4-beta.1...v2.9.4
[2.9.4-beta.1]: https://github.com/project-vrcz/content-publisher/compare/v2.9.3...v2.9.4-beta.1
[2.9.3]: https://github.com/project-vrcz/content-publisher/compare/v2.9.2...v2.9.3
[2.9.2]: https://github.com/project-vrcz/content-publisher/compare/v2.9.1...v2.9.2
[2.9.1]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0...v2.9.1
[2.9.0]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0-rc.1...v2.9.0
[2.9.0-rc.1]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0-beta.3...v2.9.0-rc.1
[2.9.0-beta.3]: https://github.com/project-vrcz/content-publisher/compare/v2.9.0-beta.2...v2.9.0-beta.3
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
