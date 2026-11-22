# Configuration

Pelican Town Privilege reads its settings from `config.json`. SMAPI creates the file the first time the mod runs.

**Where it lives:** the mod's own folder, `Stardew Valley/Mods/PelicanTownPrivilege/config.json`
(on macOS Steam installs this is inside `Contents/MacOS/Mods/`).

**When changes apply:** settings are read when the game starts. **Restart the game after every change.**

**Invalid values:** the mod corrects them to the default, writes the corrected file back, and logs each change as
`Config corrected: ...` in the SMAPI console.

## All settings

### Pierre's Privilege Counter (Data/Shops: `SeedShop`)

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnablePrivilegeShop` | `true` | true/false | Master switch. When `false`, no privilege rows are added to Pierre's or Clint's shops. |
| `ShopPurchasePrice` | `1` | whole number, 0 or more | Price of every privilege row in Pierre's and Clint's shops. Negative values reset to `1`. |
| `UnlimitedPrivilegeStock` | `true` | true/false | `true` means unlimited stock. `false` applies `PrivilegeStockLimit` per player. |
| `PrivilegeStockLimit` | `10` | whole number, 1 or more | Stock per player when `UnlimitedPrivilegeStock` is `false`. *Added beyond the spec.* |
| `EnableWateringStock` | `true` | true/false | Iridium Sprinkler. *Added beyond the spec, so each section can be switched off.* |
| `EnableAnimalEquipment` | `true` | true/false | Auto-Grabber and Auto-Petter. |
| `EnableProcessingMachines` | `true` | true/false | Keg, Preserves Jar, Mayonnaise Machine, Cheese Press. |
| `EnableStorageStock` | `true` | true/false | Chest, Big Chest, Junimo Chest. *Added beyond the spec.* |
| `EnableRareEggStock` | `true` | true/false | Dinosaur, Void, Golden, and Ostrich eggs, plus the Ostrich Incubator. |
| `EnableGiftStock` | `true` | true/false | Universal gift items and Stardrop Tea. |
| `EnableGalaxyWeapons` | `true` | true/false | Galaxy Sword, Dagger, and Hammer. |

### Clint's Blacksmith (Data/Shops: `Blacksmith`)

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnablePremiumTools` | `true` | true/false | Iridium Axe, Pickaxe, Hoe, Watering Can, and the Iridium Trash Can upgrade. |

### Wide-area farming tools

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnableWideHoe` | `true` | true/false | Privilege Hoe: its shop row and its 9×1 tilling behaviour. |
| `EnableWidePlanter` | `true` | true/false | Privilege Planter: its shop row, its planting behaviour, and the Load key. |
| `ToolWidth` | `9` | odd whole number, 1 to 15 | Length of the strip. Must be odd so it can be centred on the player. Other values reset to `9`. |
| `AllowWideToolsOutsideFarmAndGreenhouse` | `false` | true/false | `false` means the wide tools do nothing outside the farm and greenhouse. `true` allows any location. |
| `LoadSeedsKey` | `"F"` | a key name from SMAPI's list (for example `F`, `G`, `K`, `F5`) | Key for loading and unloading the planter. Unknown names reset to `F`. *Added beyond the spec.* |

### Robin's Junimo Huts (Data/Buildings: `Junimo Hut`)

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnableEarlyJunimoHuts` | `true` | true/false | Removes the Wizard requirement for the Junimo Hut and applies the settings below. `false` leaves the vanilla entry unchanged. |
| `JunimoHutPrice` | `1` | whole number, 0 or more | Cost in gold. Negative values reset to `1`. |
| `JunimoHutRequiresMaterials` | `false` | true/false | `false` empties the material list. |
| `InstantJunimoConstruction` | `true` | true/false | `true` sets build days to `0`, which means instant construction. |

### Marnie's animals and incubation

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnableEarlyBlueChickens` | `true` | true/false | Blue Chicken is available immediately. Its unlock condition is set to `TRUE`, so Shane's friendship events are not needed. |
| `BlueChickenPrice` | `1` | whole number, 0 or more | Price of a Blue Chicken. Negative values reset to `1`. |
| `EnableOvernightIncubation` | `true` | true/false | Eggs already in a coop incubator or an Ostrich Incubator finish when the day ends. |
| `EnableDailyDinosaurEggs` | `true` | true/false | Dinosaurs produce every day (`DaysToProduce = 1`) when they are fed and cared for. |

### Diagnostics

| Setting | Default | Type | What it does |
| --- | --- | --- | --- |
| `EnableDebugLogging` | `false` | true/false | Writes extra `Debug` lines to the SMAPI console. |

## Example

```json
{
  "EnablePrivilegeShop": true,
  "ShopPurchasePrice": 1,
  "UnlimitedPrivilegeStock": true,
  "PrivilegeStockLimit": 10,
  "EnableWateringStock": true,
  "EnableAnimalEquipment": true,
  "EnableProcessingMachines": true,
  "EnableStorageStock": true,
  "EnableRareEggStock": true,
  "EnableGiftStock": true,
  "EnableGalaxyWeapons": true,
  "EnablePremiumTools": true,
  "EnableWideHoe": true,
  "EnableWidePlanter": true,
  "ToolWidth": 9,
  "AllowWideToolsOutsideFarmAndGreenhouse": false,
  "LoadSeedsKey": "F",
  "EnableEarlyJunimoHuts": true,
  "JunimoHutPrice": 1,
  "JunimoHutRequiresMaterials": false,
  "InstantJunimoConstruction": true,
  "EnableEarlyBlueChickens": true,
  "BlueChickenPrice": 1,
  "EnableOvernightIncubation": true,
  "EnableDailyDinosaurEggs": true,
  "EnableDebugLogging": false
}
```

## Turning a section off

Set its switch to `false` and restart. For example, to keep the Privilege Hoe but not the planter, set
`"EnableWidePlanter": false`. Items that were already bought stay in the save and keep their mod tags. They then
behave as described in [CLEANUP.md](CLEANUP.md).
