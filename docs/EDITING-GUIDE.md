# Editing guide

This guide is for changing the mod, whether you edit by hand or with an AI assistant. Every change below names the
file to edit, so you don't have to search the codebase. Read [ARCHITECTURE.md](ARCHITECTURE.md) for the big picture.

## Before you start

1. Install a recent **.NET SDK**. The build was checked with SDK 11.0.100. The project targets `net6.0`.
2. Install **SMAPI** and **Stardew Valley 1.6.15**. The build needs SMAPI's DLLs in the game folder.
3. Build from the repository root:

   ```bash
   cd src/PelicanTownPrivilege
   dotnet build
   ```

   This compiles the mod and copies it into your game's `Mods` folder. A Release build (`dotnet build -c Release`) also writes a zip to `bin/Release/net6.0/`. Use
   `-p:EnableModDeploy=false` if you don't want the build to touch your game folder.

   On Windows, the SDK install and game-folder setup are in [Build from source](../README.md#build-from-source).

4. Build without installing SMAPI into your game (compile check only): point `GamePath` at a folder that holds the
   game's DLLs and SMAPI's DLLs. For example:

   ```bash
   cd src/PelicanTownPrivilege
   dotnet build -p:GamePath=/path/to/reference-folder -p:EnableModDeploy=false
   ```

   The folder needs copies or symlinks of the game's DLLs, `StardewModdingAPI.dll` at the top level, and
   `smapi-internal/0Harmony.dll`, `smapi-internal/SMAPI.Toolkit.dll`, and `smapi-internal/SMAPI.Toolkit.CoreInterfaces.dll`.
   The v0.1 check used this layout. Its `StardewModdingAPI.dll` came from building SMAPI source at tag `4.5.2` with
   `-p:CopyToGameFolder=false`, so the game folder was not touched.

Changes to `config.json` are picked up the next time the game starts. Changes to `assets/catalog.json` also need a game restart. Edit the file in `src/PelicanTownPrivilege/assets/` and rebuild, or edit the copy in your game's `Mods/PelicanTownPrivilege/assets/` folder to try a change without rebuilding.

## I want to…

| Goal | Edit | Notes |
| --- | --- | --- |
| Change a shop price | `config.json`: `ShopPurchasePrice`, `JunimoHutPrice`, `BlueChickenPrice` | Restart the game. |
| Turn a section off | `config.json`: the matching `Enable…` switch | See [CONFIGURATION.md](CONFIGURATION.md). |
| Add an item to Pierre's or Clint's shop | `src/PelicanTownPrivilege/assets/catalog.json`, add an entry to `Entries` | See the recipe below. |
| Remove an item from a shop | `catalog.json`, delete its entry | Existing copies in saves are not affected. |
| Fix a wrong item ID | `catalog.json`, correct `ItemId` | Run `ptp_status` to confirm. |
| Change the area size | `config.json`: `ToolWidth` and `ToolDepth` (1 to 15) | The tile rules stay the same. |
| Change the seed-loading key | `config.json`: `LoadSeedsKey` | Use an SMAPI key name, such as `G` or `F5`. |
| Let the wide tools work in town or the mines | `config.json`: `AllowWideToolsOutsideFarmAndGreenhouse` | Off by default. |
| Change what counts as a valid tile | `Tools/ToolFootprint.cs` (`CanTill`, `TryGetPlantableSoil`) | Keep the protected-object rules. |
| Change how tilling behaves | `Services/WideHoeService.cs` | Energy formula is near the top of `Use`. |
| Change planting or seed loading | `Services/WidePlanterService.cs` | Load and unload are called from `HandleLoadSeedsKey`. |
| Change the area preview colours | `Services/FootprintPreview.cs` | Green is valid, red is skipped. |
| Change Junimo Hut rules | `Services/JunimoBuildingService.cs` | Only the `Junimo Hut` entry is touched. |
| Change Blue Chicken or Dinosaur rules | `Services/AnimalService.cs` | Only those two entries are touched. |
| Change overnight incubation | `Services/IncubationService.cs` | Coop and Ostrich paths are separate methods. |
| Change the gift warning or price line | `Services/ShopService.cs` (`GiftWarning`, `DrawCounterNotes`) | Text is plain English. |
| Change the trash-can upgrade | `Services/PremiumToolService.cs` (`IridiumTrashCanLevel`) | Level 4 is Iridium. |
| Change cleanup behaviour | `Services/CleanupService.cs` | Keep the dry run as the default. |
| Change the save backup | `Services/SaveMigrationService.cs` | Backups are written beside the Saves folder. |
| Add a true/false switch | See "Add a switch" below | |
| Add a feature | See "Add a feature" below | |

## Recipe: add an item to a shop

Open `src/PelicanTownPrivilege/assets/catalog.json` and add one object to `Entries`:

```json
{ "Shop": "SeedShop", "Section": "Processing", "Toggle": "EnableProcessingMachines", "Name": "Crystalarium" }
```

| Field | Meaning |
| --- | --- |
| `Shop` | `SeedShop` for Pierre, `Blacksmith` for Clint. Any other `Data/Shops` key also works. |
| `Section` | A label for the logs. |
| `Toggle` | The name of a true/false setting in `config.json`. The row is added only while it is `true`. |
| `Name` | The item's name as the game data lists it, for example `Keg` or `Galaxy Sword`. Used to check `ItemId`, or to find the item if no `ItemId` is given. |
| `ItemId` | A qualified ID such as `(O)107`, `(BC)645`, `(W)4`, or `(T)IridiumAxe`. Optional. |
| `DisplayName` | Optional. Shows a different name in the shop. |
| `Condition` | Optional. A game state query. The row shows only while it is true. |
| `ModData` | Optional. Tags put on the item when it is bought. Keys should start with `PelicanTownPrivilege/`, because cleanup only looks for that prefix. |

Then restart the game and run `ptp_status`. The row is either added, or it appears under "Skipped catalog rows" with a
reason.

## Recipe: add a true/false switch

1. In `Configuration/ModConfig.cs`, add a property:

   ```csharp
   public bool EnableCrystalariums { get; set; } = true;
   ```

2. Use it in `catalog.json` as `"Toggle": "EnableCrystalariums"`, or check it in code with `config.EnableCrystalariums`.
3. Add a row for it to the table in [CONFIGURATION.md](CONFIGURATION.md).

Property names are the switch names. `ptp_status` lists every switch and its current value, so you can check it.

## Recipe: add a feature

Follow the existing pattern. Each feature is one service class, plus a small wire-up in `ModEntry.cs`.

1. Create `Services/YourFeatureService.cs`. Pass `ModConfig` and `Diagnostics` in through the constructor.
2. For a **data change**, add a method that takes `IAssetData` and edit only the entries you need. Then add a branch in
   `ModEntry.OnAssetRequested`. Look at `AnimalService` for a short example.
3. For a **runtime change**, subscribe to a SMAPI event in `ModEntry.Entry`. Prefer the event that fits: `DayEnding`
   for day-end work, `ButtonPressed` for keys, `RenderedWorld` for drawing.
4. For a change to a **vanilla method**, add a Harmony patch in `RuntimeHooks/`. Check the mod's own tags first, so
   other items keep vanilla behaviour. Patches read services from `RuntimeHooks/HookServices.cs`, so set any new service
   there in `ModEntry.Entry`. Wrap the body in `try`/`catch` and log errors, so a bug cannot crash the game.
5. Update [ARCHITECTURE.md](ARCHITECTURE.md) and [CONFIGURATION.md](CONFIGURATION.md).
6. Build, then add a row to [TESTING.md](TESTING.md).

## Conventions

- **Never change `Data/Objects` prices.** Shop prices belong to the shop row, so sale values stay the same.
- **Track only mod-owned state.** Put tags in `modData` under the `PelicanTownPrivilege/` prefix. Use the helpers in
  `Tools/PrivilegeTools.cs`.
- **Don't trigger story or relationship effects.** The mod must not set mail flags, complete quests, or change
  friendship, and must not call anything that does.
- **Qualify `Object`.** Write `StardewValley.Object` in full. `Object` also names `System.Object`, and the compiler
  rejects the bare name when both namespaces are imported.
- **Keep the logs useful.** Use `Info` for things that happened, `Warn` for things the user should fix, and `Debug`
  for detail that is only needed when debugging.
- **Comment only where a reader needs help.** Explain why, not what.

## Prompts that work well with AI assistants

Copy one of these, paste it into your assistant, and fill in the blanks.

- *"In `src/PelicanTownPrivilege`, add a row for `<item name>` to Pierre's shop under the `<section>` section, using
  the `<toggle>` switch. Follow the pattern in `assets/catalog.json`, then explain how to verify the item ID."*
- *"Change the Junimo Hut so it costs `<n>` gold. Keep it in `JunimoBuildingService.cs`, and keep the config switch."*
- *"Add a new service that does `<what>`, following `docs/EDITING-GUIDE.md`. Use `DayEnding` for the timing. Keep any
  hook inside `RuntimeHooks/`, and update the docs."*
- *"Read `docs/SPEC.md` and tell me which acceptance tests in `docs/TESTING.md` the current code should already pass,
  and which it can't yet."*
