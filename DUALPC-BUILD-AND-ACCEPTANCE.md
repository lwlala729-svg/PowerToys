# DUAL-PC MWB — build and acceptance handoff (2026-11-13)

BASE: Microsoft PowerToys 0.102.3101, commit `e84973c4f3724eeedd34d836559dac32734c3e22`
SOURCE_BRANCH: `mwb-dualpc-stability-20261113`
PRESERVE_ORIGINAL: `mwb-remove-100mb-limit-v0.102.3101`; never merge this older base into current `main`.
TARGETS: older Windows x64 PC and private Windows x64 PC.
NETWORK: older host is behind a secondary NAT; it initiates TCP 15101 toward the private main host. Exact names/IPs remain in the private project blackboard only. No router/network changes.

## Actual source changes
- Existing fork: 100 MiB sender gate removed; `SendFileEx` count `long`, padding arithmetic retains existing protocol.
- New: avoid overlapping client attempts while socket Resolving/Connecting/Handshaking/Connected. Same-IP guard includes Handshaking, and per-IP duplicate check plus socket registration are atomic under TcpSocketsLock.
- New: existing CloseAnUnusedSocket 60 second cleanup now runs on UpdateTCPClients before blocking retry.
- New: backport Name2IP mapping list fix from newer PowerToys. User hosts file backup stays intact.
- Keyboard hook: unchanged from Microsoft's v0.102.3101 source. Preserve the user's reported prior working keyboard behavior; no speculative hook changes.
- Added `MouseWithoutBorders.UnitTests/Core/DualPcConnectionRegressionTests.cs` for active statuses and map parsing.

## Build dependency blocker — official upstream evidence
Required vcpkg CMake asset: `cmake-4.4.3-windows-x86_64.zip`
Size (GitHub Kitware v4.4.3 release metadata): `54408599` bytes.
SHA-256 (Kitware release metadata): `4d52ebab7193a698651639ed80d8d04fd903358843572cf44c7fd234cb7c26ab`
Official URL: https://github.com/Kitware/CMake/releases/download/v4.4.3/cmake-4.4.3-windows-x86_64.zip

Earlier file was only about 11.1 MB and cannot be a complete copy. Do not treat it as valid. Verify full length and SHA256 with Get-Item/Get-FileHash, then compare SHA512 with the *local checked-out vcpkg portfile*. Never edit expected hashes. Reuse already present worktree/toolchain/dependency cache. Stop at the first new actual error, not a broad network scan. If release-assets.githubusercontent.com is inaccessible, obtain the identical official asset through another authorized host or trusted handoff, verify both hashes and place it in the existing vcpkg cache location.

From *existing* Windows build worktree after fetching this branch, use the repository's own script:
`./tools/build/build-installer.ps1 -Platform x64 -Configuration Release -PerUser true`
Verify any existing local toolchain/restore setup before invoking. Do not use `-Clean`. The source script documents that it generates a local signing certificate for MSIX; target machines must trust the user's own generated certificate through approved certificate management, not by disabling Windows security. This build is **not signed by Microsoft**.

Existing release target root:
`<existing private TASK-009 release directory>`
Record installer full path, its SHA256, package/signature state, and source commit. No generated installer or binary has been produced by this GitHub-only work.

## Keyboard behavior preservation
The user reports that keyboard sharing between the two PCs had previously worked. The recent Doubao test reporting keys staying local is one failed test state, not proof keyboard forwarding never worked. The speculative InputHook.RealData reset was removed from this candidate; InputHook.cs is identical to the official base. Do not change keyboard hooks unless a reproducible keyboard-specific defect is observed after the connection is genuinely MWB Connected. Test keyboard forwarding separately in Notepad on both directions, including letters, numbers, Enter, Backspace, modifier keys and recovery of local input.

## Tests / acceptance
Source-level checks performed: static source and behavior-preservation assertions; new branch readback and source diff were checked. Keyboard hook is byte-for-byte unchanged from the official base. Windows runtime verification is still required. NOT a compiler, unit test, or Windows test.
1. Build targeted MWB unit tests including DualPcConnectionRegressionTests; report actual executable exit code and passed test count. Fail -> stop.
2. On older PC, confirm source-pinned and user-approved install path, run MWB, initiate to private host over TCP 15101; verify only ONE active client handshake attempt per machine; do not assume TCP ESTABLISHED == trusted MWB Connected.
3. After explicit human installation approval, install the *same version/custom build* on private host, preserve current network/IP/firewall/access restrictions, existing inbox, unrelated apps and disk ACLs. Verify normal local user access unchanged.
4. Verify mouse X99->private host and reverse with actual monitor cursor; separately verify keyboard letters/numbers/backspace/Enter in Notepad on both; do not use remote desktop/simulator full-screen. Confirm keyboard follows active remote screen, not merely local text.
5. Three clean restarts/reconnects, 5 minutes stable connection, no duplicate RST and no ghost client sockets.
6. File-copy tests >100 MiB and >2 GiB are STILL NOT DONE. The official MWB receiver writes to desktop/MouseWithoutBorders on an interactive session; enabling it on private host would violate the frozen <private host's scoped inbox on E:>-only receiver boundary. Keep `TransferFile=False` and `ShareClipboard=False` until a scoped E-only receive path and the separate TCP 15100/firewall requirements are explicitly verified. Do not silently enable it to claim success.
7. Do not widen 445/SMB, RDP, SSH, 15100, or NTFS permissions; no router changes. Only 15101 was already authorized. No private data/credential logging.

## Status at handoff
SOURCE_PATCH=COMMITTED
SOURCE_ASSERTIONS=PASS (static only)
C_SHARP_COMPILE=NOT_RUN
WINDOWS_TESTS=NOT_RUN
INSTALLER=NOT_BUILT
DUAL_HOST_ACCEPTANCE=NOT_RUN
LARGE_FILE_TRANSFER=NOT_TESTED

## User-visible UI and language freeze
No edits to Microsoft PowerToys UI, window layout, resource, localization, icon, installer-source, version, font, keyboard hook, or unrelated module files are intended. Compare the final commit's changed filenames to the exact v0.102.3101 base before building. Reject any change outside the approved MWB backend/test and this documentation.
A locally built package is not guaranteed to carry Microsoft's signature. Do not misrepresent its publisher, bypass SmartScreen, or install an untrusted unsigned package on the private host. Back up existing settings, check Chinese glyphs/font rendering and all major MWB settings against the same original version, and require human visual acceptance before replacing the working install.
Official default MWB file receiving destination is not E-inbox-only. Do not enable built-in transfer on the private host before a dedicated scoped receiver exists and is verified.
