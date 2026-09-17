# Stationeers Render Distance

Client-only [BepInEx](https://github.com/BepInEx/BepInEx) + [StationeersLaunchPad](https://github.com/StationeersLaunchPad/StationeersLaunchPad) plugin that keeps **buildings** and **fire / particle effects** visible farther away.

It does not change simulation, saves, or networking. Dedicated servers must not load it.

- **Source:** https://github.com/REPLACE_GITHUB_OWNER/stationeers-render-distance
- **Steam Workshop:** not published yet. After the first in-game Publish, this will be `https://steamcommunity.com/sharedfiles/filedetails/?id=<id>`

## Requirements

- Stationeers player client (`rocketstation.exe`)
- [BepInEx 5.4.x](https://github.com/BepInEx/BepInEx/releases)
- [StationeersLaunchPad](https://github.com/StationeersLaunchPad/StationeersLaunchPad)

## Install (Workshop)

1. Install BepInEx and StationeersLaunchPad on the **client**.
2. Subscribe to this item on Steam Workshop (link above, once published).
3. Enable it in LaunchPad. Press **F10** in-world for sliders.

Do **not** enable this on Stationeers Dedicated Server.

## Install (local / from source)

Release build copies the Workshop package to:

`%USERPROFILE%\Documents\My Games\Stationeers\mods\RenderDistance\`

Or copy `StationeersRenderDistance.dll` to `Steam\steamapps\common\Stationeers\BepInEx\plugins\` for a quick debug load.

Config is created at `Stationeers\BepInEx\config\se.admin.RenderDistance.cfg`.

## In-game menu

While the world is running or paused, press **F10** (configurable) for:

- Distance sliders: buildings, small things, particles/fire (0–2000 m; 0 = vanilla)
- Toggles: buildings, effects, all particles (off = fire-like names only)

Values save to the BepInEx config immediately.

## Config file

```
[Buildings]
Enabled = true
StructureRenderDistance = 500
SmallThingRenderDistance = 0

[Effects]
Enabled = true
ParticleRenderDistance = 300
AffectAllParticles = true

[General]
LogPatches = false
MenuHotkey = F10
```

Distances are meters. The plugin uses `max(vanilla, config)` so values never draw closer than stock.

- **StructureRenderDistance** — frames, walls, devices, cladding, and other structures. Default 500 m.
- **SmallThingRenderDistance** — items, crates, and draggables. `0` keeps vanilla.
- **ParticleRenderDistance** — world fire and burning-thing flames. Vanilla fire is 50 m. Default 300 m.
- **AffectAllParticles** — also relaxes Unity particle culling for smoke, steam, and other particle systems. Turn this off if you only want fire.

Many distant burning grids is expensive. Keep particle range lower than buildings if the frame rate drops.

## Publish to Steam Workshop

1. `dotnet build -c Release` (copies into Documents mods).
2. Launch **Stationeers** (`rocketstation.exe`).
3. Main menu → **Workshop** → **Workshop Mods**.
4. Select **Render Distance** (no `[Workshop]` prefix) → **Publish**. Wait until a `[Workshop]` copy appears.
5. Disable the **local** copy so the mod is not loaded twice.
6. Set the Workshop item to Public if you want it listed.

Updates: bump `<Version>` in `About/About.xml`, rebuild, Publish again. After the first Publish, the game writes a Workshop ID into `About.xml` — leave it there.

## Build

Needs a local Stationeers install. Default Steam path is detected. If yours is elsewhere:

```powershell
$env:STATIONEERS_DIR = "D:\Steam\steamapps\common\Stationeers"
dotnet build -c Release
```

Or copy `Local.Developer.props.example` to `Local.Developer.props` and set `StationeersDllDir` to `...\rocketstation_Data\Managed`.

Output: `bin\Release\StationeersRenderDistance.dll`

## Compatibility

- Harmony postfix / transpiler only. It does not replace `Thing.SetOcclusion`.
- If a game update renames a method, that patch is skipped and the rest still load. Enable `LogPatches` to see what applied.
- Terrain LOD is unchanged (vanilla still fades around ~650 m).
- Listen-server hosts still load the mod (they render). Headless / batch-mode dedicated processes log `Skipped: client-only` and apply no patches.
- Default menu hotkey is F10. Change `MenuHotkey` if another mod already uses F10.
