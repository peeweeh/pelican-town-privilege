# Architecture

This page explains how the mod is put together, so you can find the right file before changing anything.
For step-by-step editing recipes, see [EDITING-GUIDE.md](EDITING-GUIDE.md).

## Folder map

```
src/PelicanTownPrivilege/
├── ModEntry.cs                 SMAPI entry point: wiring, events, console commands
├── manifest.json               SMAPI manifest (version is filled in from the .csproj at build)
├── assets/catalog.json         The shop rows: what Pierre and Clint sell (edit this to change stock)
├── Configuration/
│   ├── ModConfig.cs            Every setting in config.json, with defaults
│   └── ConfigValidator.cs      Loads config.json, corrects bad values, logs each correction
├── Content/
│   ├── CatalogModels.cs        Shape of catalog.json
│   └── ItemResolver.cs         Checks item IDs against the game's data; falls back to names
├── Services/
│   ├── ShopService.cs          Adds catalog rows to Data/Shops; draws Pierre's price line and gift warning
│   ├── PremiumToolService.cs   Applies the Iridium Trash Can upgrade at purchase
│   ├── WideHoeService.cs       Privilege Hoe: 9x9 tilling, clears grass and weeds first
│   ├── WidePlanterService.cs   Privilege Planter: loading, unloading, and area planting
│   ├── WideScytheService.cs    Privilege Scythe: area cutting and harvesting after each swing
│   ├── WidePickaxeService.cs   Privilege Pickaxe: area breaking of stones, ore, and boulders
│   ├── WideAxeService.cs       Privilege Axe: area chopping of trees, stumps, logs, twigs, and weeds
│   ├── FootprintPreview.cs     Draws the area over the world while a privilege tool is held
│   ├── JunimoBuildingService.cs Edits the Junimo Hut entry in Data/Buildings
│   ├── AnimalService.cs        Edits Blue Chicken and Dinosaur entries in Data/FarmAnimals
│   ├── IncubationService.cs    Overnight incubation (coop and Ostrich Incubator, separate paths)
│   ├── SaveMigrationService.cs Makes a save backup before any change to a save
│   ├── CleanupService.cs       ptp_cleanup: returns seeds, clears mod tags, reports readiness
│   └── Diagnostics.cs          Logging helpers and the ptp_status report
├── Tools/
│   ├── PrivilegeTools.cs       Mod tags on items (modData keys) and seed storage helpers
│   ├── ToolFootprint.cs        The wide-area geometry and tile rules
│   └── LocationScan.cs         Lists every location, including building interiors; on-screen feedback
└── RuntimeHooks/               The only Harmony patches. Each one checks mod tags first.
    ├── HookServices.cs         Service references the hooks use (set once in ModEntry)
    ├── ToolHooks.cs            Hoe.DoFunction, Tool.DisplayName, Tool.getDescription, Item.canBeTrashed
    └── PurchaseHooks.cs        Object.actionWhenPurchased (Iridium Trash Can)
```

## How it starts

1. `ConfigValidator.Load` reads `config.json`, corrects invalid values, and logs each correction.
2. `assets/catalog.json` is read with `helper.Data.ReadJsonFile`.
3. Services are created with the validated config.
4. `HookServices` is filled in and Harmony's `PatchAll` installs the hooks in `RuntimeHooks/`.
5. SMAPI events and the console commands `ptp_status` and `ptp_cleanup` are registered.

## Data edits (no runtime code)

These are applied through SMAPI's `AssetRequested` event. They change the game's own data tables and nothing else.

| Asset | Entries touched | Code | Controlled by |
| --- | --- | --- | --- |
| `Data/Shops` | Rows appended to `SeedShop` (Pierre) and `Blacksmith` (Clint) | `ShopService.EditShops` | `catalog.json` plus the toggles in `config.json` |
| `Data/Buildings` | `Junimo Hut` only | `JunimoBuildingService.EditBuildings` | `EnableEarlyJunimoHuts`, `JunimoHutPrice`, `JunimoHutRequiresMaterials`, `InstantJunimoConstruction` |
| `Data/FarmAnimals` | `Blue Chicken` and `Dinosaur` only | `AnimalService.EditFarmAnimals` | `EnableEarlyBlueChickens`, `BlueChickenPrice`, `EnableDailyDinosaurEggs` |

Shop rows are only appended. Existing rows, and the sale values in `Data/Objects`, are never changed.

## Runtime behaviour

| Feature | Trigger | Code |
| --- | --- | --- |
| Privilege Hoe tilling | Hoe is used (Harmony prefix on `Hoe.DoFunction`) | `WideHoeService.Use` |
| Privilege Planter planting | Planter is used (same prefix) | `WidePlanterService.Plant` |
| Privilege Scythe area | Normal scythe swing finishes (Harmony postfix on `MeleeWeapon.DoDamage`) | `WideScytheService.Use` |
| Privilege Pickaxe breaking | Pickaxe is used (Harmony prefix on `Pickaxe.DoFunction`) | `WidePickaxeService.Use` |
| Privilege Axe chopping | Axe is used (Harmony prefix on `Axe.DoFunction`) | `WideAxeService.Use` |
| Privilege Iridium Sprinkler area | Sprinkler radius is read (Harmony postfix on `Object.GetModifiedRadiusForSprinkler`) | `SprinklerRadiusPatch` in `ToolHooks.cs` |
| Load and unload seeds | Load key pressed (`ButtonPressed`) | `WidePlanterService.HandleLoadSeedsKey` |
| Area preview | Every frame, only while a privilege tool is held (`RenderedWorld`) | `FootprintPreview.Draw` |
| Names and seed tooltip | Item name and description are read (Harmony postfixes) | `ToolHooks.cs` |
| Planter cannot be trashed while it holds seeds | Trash check (Harmony postfix on `Item.canBeTrashed`) | `ToolHooks.cs` |
| Iridium Trash Can upgrade | Purchase (Harmony prefix on `Object.actionWhenPurchased`) | `PremiumToolService.HandleTrashCanPurchase` |
| Overnight incubation | Day ending (`DayEnding`) | `IncubationService.OnDayEnding` |
| Pierre's price line and gift warning | Shop open (`RenderedActiveMenu`) | `ShopService.DrawCounterNotes` |
| Save cleanup | `ptp_cleanup` command | `CleanupService.Run` |
| Status report | `ptp_status` command | `Diagnostics.PrintStatus` |

## Mod-owned state

The mod writes no save-file data of its own. Its only persistent state is:

- **Item tags** in `Item.modData`, all with the key prefix `PelicanTownPrivilege/`:
  - `PelicanTownPrivilege/Tool` is `WideHoe`, `WidePlanter`, `WideScythe`, `WidePickaxe`, `WideAxe`, or `WideSprinkler`. Set by the shop row.
  - `PelicanTownPrivilege/SeedId` and `PelicanTownPrivilege/SeedCount` hold the planter's stored seeds.
  - `PelicanTownPrivilege/Action` is `UpgradeTrashCan` on the sentinel shop item. It is never put in a player's inventory.
- **Shop row IDs** of the form `PelicanTownPrivilege.<Name>`. These exist only in the shop table, which the game
  rebuilds from data.

The game ignores unknown `modData` keys, so a save that is loaded without the mod still loads. Privilege tools then
behave as ordinary hoes. See [CLEANUP.md](CLEANUP.md).

## Item identifiers and validation

Item IDs use the game's qualified format:

| Prefix | Kind | Example |
| --- | --- | --- |
| `(O)` | Object | `(O)107` Dinosaur Egg |
| `(BC)` | Big craftable (machines, chests, sprinklers) | `(BC)645` Iridium Sprinkler |
| `(W)` | Weapon | Not needed for Galaxy weapons, which are matched by name |
| `(T)` | Tool | `(T)IridiumAxe` |

`ItemResolver` checks every catalog row against the installed game's `Data/Objects`, `Data/BigCraftables`,
`Data/Weapons`, and `Data/Tools` tables:

1. The `ItemId` exists and its name matches `Name` (or no `Name` is given): the row is added.
2. The `ItemId` exists but is a different item: the mod looks the item up by `Name`. If that fails, it keeps the
   `ItemId` and logs a warning.
3. The `ItemId` is missing or does not exist: the mod looks the item up by `Name`.
4. Nothing matches: the row is skipped and logged. `ptp_status` lists every skipped row.

## Decisions made for v0.1

These were judgement calls made while building v0.1. Change them in code or config if you disagree.

- **Tools are base items with tags, not custom classes.** The Privilege Hoe and Planter are ordinary `Hoe` items, the
  Privilege Scythe is an ordinary scythe, and the Privilege Pickaxe is an ordinary Iridium pickaxe, and the Privilege Axe is an ordinary Iridium axe. Each has `modData`
  tags. Without the mod they act as their normal selves, so nothing is lost.
- **Outside the farm and greenhouse, the hoe and planter do nothing** and show a message. No energy is used. The scythe
  and pickaxe act as their normal selves there.
- **Energy:** the Privilege Hoe and Pickaxe use the same formula as `Hoe.DoFunction` and `Pickaxe.DoFunction`, once per
  activation. The planter uses no energy, because the spec gives it none. The scythe adds none: the normal swing's cost applies.
- **Area contents:** the hoe removes grass and weeds itself and tills in the same use. Grass is cleared directly, because the game's grass ignores hoes. Neither drops anything. The scythe cuts grass, clears weeds, and harvests scythe crops. The pickaxe breaks stones, ore, and boulders. The axe chops trees, stumps, and logs, and clears twigs and weeds. It never touches fruit trees. Neither the hoe nor the pickaxe removes soil, and neither touches crops, trees, paths, buildings, crafted objects, or characters.
- **Seed IDs:** the planter stores the qualified seed ID (for example `(O)472`), but the game's planting code expects the plain ID (`472`). `PrivilegeTools.PlantingId` converts it before every planting call.
- **Sprinkler area:** the privilege Iridium Sprinkler's radius is set to 4 by a postfix on the game's radius method, so its area is 9×9 with the sprinkler in the middle. The watering code itself is unchanged.
- **Seed loading:** hold a seed stack and press the Load key to put the whole stack in your planter. Hold the planter
  and press the key to take everything back out. A planter holds one seed type at a time.
- **Planter seeds can't be trashed** while the planter holds any. Use the unload key first. Selling is not blocked yet (see Known unknowns).
- **Iridium Trash Can** sets the trash-can level to Iridium straight away. The shop row is hidden once the player is
  already at Iridium, using the `PLAYER_HAS_TRASH_CAN_LEVEL` game state query.
- **Area size:** `ToolWidth` and `ToolDepth` are whole numbers from 1 to 15 (default 9, so the middle tile is under the player). An even width can't be centred exactly, so it reaches one tile further right (or down) than left (or up).
- **Overnight incubation** sets an egg's remaining time to zero when the day ends. Only eggs already in an incubator
  are affected. The coop path and the Ostrich Incubator path are separate code.
- **Junimo Hut:** its builder is set to Robin, so Robin's menu lists it (the game lists each building only under its builder). Its Wizard build condition is cleared, its cost comes from config (`JunimoHutPrice`, default `0`), and its material list is emptied
  unless `JunimoHutRequiresMaterials` is `true`, and its build days are `0` (instant) unless `InstantJunimoConstruction` is `false`. No other building changes.
- **Blue Chicken:** its unlock condition is set to `TRUE`, so Shane's friendship events are not needed. A coop is still required.
- **Dinosaur:** `DaysToProduce` is set to `1`.
- **Gift warning:** drawn as text above Pierre's shop while it is open. It is not part of the shop data.
- **Settings added beyond the spec:** `EnableWateringStock`, `EnableStorageStock`, `PrivilegeStockLimit`, and
  `LoadSeedsKey`. Each one gives a feature its own switch or lets you tune it.
- **Ostrich Incubator** is listed under the "Incubation" section and is controlled by `EnableRareEggStock`.
- **Infinity weapons** are left out of v1. The spec leaves them for later.
- **Multiplayer** is outside v0.1 scope. A warning is logged when a multiplayer save loads.
- **Cleanup backups** are copied to `PelicanTownPrivilege-backups`, beside the `Saves` folder and never inside it.

## Known unknowns (not yet tested in game)

The build compiles against Stardew Valley 1.6.15 and SMAPI 4.5.2, but nothing has been run in game yet. These are the
parts most likely to need adjusting:

- The item IDs in `catalog.json` are the best-known values. Only some have been checked against the game's data. The
  resolver covers the rest, and `ptp_status` shows what was skipped.
- The data keys `Blue Chicken` and `Dinosaur` in `Data/FarmAnimals` are assumed. The mod logs a warning if either is
  missing.
- The area preview's screen position, and the planting call with qualified seed IDs, are untested.
- The overnight timing for both incubator paths is untested.
- The trash-can purchase depends on the shop row's `modData` reaching `actionWhenPurchased`. The code path has been
  read, but not run. The item used is `(O)168`, the game's own Trash object. If the tag does not arrive, the player
  would receive a Trash item instead of the upgrade.
- Selling a loaded planter to a shop is not blocked. The seed protection only covers trashing (`Item.canBeTrashed`), and
  the shop code does not check it.

[TESTING.md](TESTING.md) lists the checks to run.
