# Pelican Town Privilege: specification v0.1

This is the build specification the mod is written against. It was transcribed from the v0.1 specification PDF
(*Pelican Town Privilege, Standalone mod specification, v0.1*). If the spec changes, change this file first, then
the code and docs that depend on it.

> A personal Stardew Valley mod that puts premium equipment, useful automation, rare animals, and excellent gifts
> within reach from the start.
>
> The principle: keep the farming, collecting, relationships, and exploration. Remove the procurement grind.

## 1. Scope and dependencies

One custom mod containing our own shop changes, tool behaviour, animal settings, and building-access changes.

Dependency boundary: "no other mods" means no Content Patcher, Automate, Better Junimos, or other gameplay or
framework mods. This specification assumes SMAPI is acceptable as the sole mod loader. It does not propose modifying
the game executable.

| Component | Decision |
| --- | --- |
| Implementation | C# SMAPI mod |
| Required dependencies | SMAPI only |
| Third-party mod code or assets | None |
| Original game assets | Reuse where appropriate |
| Existing saves | Support alongside new games |
| Base progression | Preserve quests and story; bypass only specified purchase restrictions |
| Changes | Configurable and, where practical, reversible |
| Multiplayer | Outside v1 acceptance scope; must not silently alter other players' progression |

This is a build specification, not a completed or tested mod.

## 2. Core design rules

1. Shop overrides affect what the player pays, not an item's underlying sale value.
2. Purchases remain optional; nothing is automatically dumped into the player's inventory.
3. Existing shop inventories remain available.
4. The player chooses building placement.
5. No automatic relationship gains, quest completions, or story flags.
6. No invented dinosaur-helper mechanics.
7. No custom animal artwork or replacement species.
8. No rewriting another mod's features wholesale when the requested result can be implemented narrowly.

## 3. Shops and premium access

### General store: Pierre's Privilege Counter

Add a clearly separated section to Pierre's existing shop.

All listed items cost **1g each**, have unlimited stock, and ignore normal season or progression restrictions for
these added entries.

| Category | Stock |
| --- | --- |
| Watering | Iridium Sprinkler |
| Animal collection | Auto-Grabber |
| Animal care | Auto-Petter |
| Processing | Keg, Preserves Jar, Mayonnaise Machine, Cheese Press |
| Storage | Chest, Big Chest, Junimo Chest, Big Stone Chest (added in v0.2) |
| Rare eggs | Dinosaur Egg, Void Egg, Golden Egg, Ostrich Egg |
| Incubation | Ostrich Incubator |
| Universal gift selection | Golden Pumpkin, Pearl, Prismatic Shard, Rabbit's Foot, Magic Rock Candy |
| Friendship shortcut | Stardrop Tea |
| Crop seeds (added in v0.2) | The crop seeds Pierre normally sells, in any season |

Implementation must validate each item identifier against the installed game version. An invalid entry is skipped with
a clear log message rather than crashing the shop.

### Weapons: Galaxy equipment

To match your "first shop" requirement, place these in Pierre's Privilege Counter too.

| Item | Price | Unlock requirement |
| --- | --- | --- |
| Galaxy Sword | 1g | None |
| Galaxy Dagger | 1g | None |
| Galaxy Hammer | 1g | None |

Buying these must not mark their normal discovery events or related quests complete.

Infinity weapons are outside the default v1 inventory. They can be considered later rather than silently turning
"premium purple weapons" into every endgame weapon.

### Blacksmith: instant premium tools

Add direct purchases at Clint's:

- Iridium Axe
- Iridium Pickaxe
- Iridium Hoe
- Iridium Watering Can
- Iridium Trash Can upgrade

Price: 1g each. No bars, prerequisite tool tier, or waiting period.

Tool purchases must preserve existing tools rather than silently deleting or replacing them. The trash-can purchase
must apply the appropriate player upgrade instead of creating an unusable inventory object.

## 4. Wide-area farming tools

Your original request was "nine-wide tiller and planter." v0.1 built that as a **9×1 strip**. Version 0.2 changes it to a **10×10 area** at your request. Its size is set by `ToolWidth` and `ToolDepth`.

### Privilege Hoe

| Setting | Behaviour |
| --- | --- |
| Purchase location | Pierre |
| Price | 1g |
| Footprint | 10 tiles across and 10 deep, starting one tile in front of the player |
| Orientation | Rotates with player facing |
| Charge requirement | None |
| Energy | One normal hoe-use cost per activation |
| Valid locations | Farm and greenhouse by default |
| Invalid tiles | Skip without cancelling the whole action |

Protected targets:

- Existing crops.
- Planted trees.
- Buildings, machines, paths, and fences.
- Occupied or otherwise invalid tiles.

The footprint should be visible before use.

### Privilege Planter

| Setting | Behaviour |
| --- | --- |
| Purchase location | Pierre |
| Price | 1g |
| Footprint | Same 10×10 area |
| Seed selection | Explicit "Load seeds" interaction |
| Seed cost | One seed per successfully planted tile |
| Valid target | Empty, tilled soil that accepts the selected seed |
| Invalid target | Skip; do not consume a seed |
| Insufficient seeds | Plant as many valid tiles as remaining seeds allow |
| Seasonal restrictions | Respect normal planting rules |
| Fertilizer | Not automatically applied in v1 |

Seed loading must show the selected seed and remaining quantity. Unloading or removing the tool must not destroy its
stored seeds.

No seed duplication, free planting, or accidental replacement of growing crops.

## 5. Junimo Huts from the beginning

### Robin's construction menu

Add Junimo Hut access with these settings:

| Setting | Value |
| --- | --- |
| Availability | From the beginning |
| Wizard quest requirement | Removed for this purchase |
| Cost | 1g |
| Materials | None |
| Construction time | Instant |
| Placement | Normal building-placement interaction |
| Purchase limit | None beyond valid farm space |

This bypass must not complete the Wizard's quests or unlock unrelated magical buildings.

### Junimo behaviour

Use the game's normal harvesting behaviour for v1.

Do not add:

- Automatic planting.
- Automatic fertilizing.
- Automatic watering.
- Forestry or debris clearing.
- New working-weather rules.
- New harvesting ranges.
- Automatic processing or shipping.

Your cheap sprinklers handle watering. Junimos handle harvesting. The wide planter handles replanting.

That is the agreed convenience package without quietly building a second automation overhaul.

## 6. Animals and incubation

### Marnie's animal shop

| Animal | Price | Availability |
| --- | --- | --- |
| Blue Chicken | 1g | Available immediately |
| Other existing livestock | Existing prices initially | Existing behaviour |

Blue chickens still require a valid home. Buying one must not complete Shane's friendship events.

Do not invent a blue-chicken egg item.

### Rare animals

Dinosaur, void-chicken, golden-chicken, and ostrich access begins with the eggs sold at Pierre's. Hatching must still
require an appropriate building or incubator.

### Proposed convenience defaults

| Setting | Default |
| --- | --- |
| Faster incubation | Enabled |
| Incubation target | Complete after one overnight transition |
| Dinosaur egg production | Once per day when normal care requirements are met |
| Animal friendship | Normal |
| Feeding requirements | Normal |
| Petting requirements | Normal; Auto-Petter available to buy |
| Birth/pregnancy mechanics | Unchanged |
| Meat production | Not included |

The one-night incubation rule must be tested separately for coop and ostrich incubation. Do not assume the two systems
share an implementation.

Animal treats and controlled breeding are deferred: they were candidates, not essential to your convenience objective.

## 7. Gifts and friendship

The mod changes access and purchase prices, not gift tastes or friendship rules.

Requirements:

- Keep each villager's normal likes, loves, and dislikes.
- Keep normal gift limits and item-specific exceptions.
- Keep birthday and quality effects.
- Do not automatically trigger heart events.
- Do not change romance or marriage requirements.
- Do not auto-gift.

The gift counter should include a short warning:

> Cheap does not mean universally appreciated. Normal gift preferences still apply.

No bespoke gifting UI in v1.

## 8. Configuration

Ship with the agreed convenience features enabled, but allow individual sections to be disabled.

Illustrative configuration:

```json
{
  "EnablePrivilegeShop": true,
  "ShopPurchasePrice": 1,
  "UnlimitedPrivilegeStock": true,

  "EnableGalaxyWeapons": true,
  "EnablePremiumTools": true,
  "EnableGiftStock": true,
  "EnableAnimalEquipment": true,
  "EnableProcessingMachines": true,
  "EnableRareEggStock": true,

  "EnableWideHoe": true,
  "EnableWidePlanter": true,
  "ToolWidth": 10,
  "ToolDepth": 10,
  "AllowWideToolsOutsideFarmAndGreenhouse": false,

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

Configuration requirements:

- Validate values at load.
- Reject invalid widths or negative prices.
- Log corrections clearly.
- State when a restart is required.
- Do not require a third-party configuration menu.

## 9. Technical structure

A proposed division of responsibilities:

```
PelicanTownPrivilege
├── ModEntry
├── Configuration
├── ShopService
├── PremiumToolService
├── WideHoeService
├── WidePlanterService
├── JunimoBuildingService
├── AnimalService
├── IncubationService
├── SaveMigrationService
└── Diagnostics
```

Implementation policy:

- Prefer supported SMAPI events and editable game data.
- Verify the target game and SMAPI versions before selecting APIs.
- Keep unavoidable runtime hooks isolated.
- Never copy implementation code from the previously discussed mods.
- Track only mod-owned state.
- Avoid polling every game tick when an interaction, day transition, or data edit will do.

## 10. Save safety and removal

### Ordinary purchased items and buildings

Design around normal game objects wherever possible. Disable future discounts when the mod is removed; do not attempt
to repossess purchases or reverse completed actions.

### Custom tools

Custom tools require an explicit removal strategy.

Before release, provide a cleanup command that:

1. Returns stored planter seeds.
2. Converts or removes mod-specific tool state safely.
3. Identifies any locations or inventories that could not be cleaned.
4. Reports whether the save is ready to load without the mod.

Do not advertise uninstall safety until this has been tested.

### Backup policy

Before the first persistent migration, create or require a recoverable save backup. Never silently rewrite an entire
save to apply shop discounts.

## 11. Acceptance tests

| Test | Pass condition |
| --- | --- |
| New game | Privilege shop entries appear on the first normal shop visit |
| Existing save | Mod loads without resetting progress |
| Prices | Added entries cost exactly 1g |
| Sale values | Original item sale values remain unchanged |
| Galaxy weapons | Purchasable without completing their normal unlock |
| Gifts | Buying or giving gifts retains normal relationship behaviour |
| Junimo Hut | Purchasable from Robin early, with no materials, and placed instantly |
| Quest isolation | Buying a hut or weapon does not complete unrelated quests |
| Blue Chicken | Purchasable early and assigned to a valid coop |
| Incubation | Eligible eggs hatch after one overnight transition |
| Dinosaur production | Eligible fed dinosaur produces at the configured daily interval |
| Wide hoe | Affects only valid tiles within the 10×10 area |
| Wide planter | Consumes exactly one seed per successful planting |
| Protected objects | No crops, structures, or machines are destroyed |
| Removal | Stored seeds recovered and custom tool state cleaned successfully |

## 12. Build order

**Milestone 1: The privilege economy**

- SMAPI project and configuration.
- Pierre's inventory.
- Galaxy weapons.
- Gifts and machines.
- Premium tool purchasing.

**Milestone 2: Buildings and animals**

- Early 1g Junimo Huts.
- Early blue chickens.
- Rare egg inventory.
- Overnight incubation.
- Daily dinosaur eggs.

**Milestone 3: Wide tools**

- Nine-wide hoe.
- Seed-loading planter.
- Footprint preview.
- Safe unloading and removal.

**Milestone 4: Release hardening**

- Existing-save testing.
- Invalid-tile and inventory edge cases.
- Removal testing.
- Installation instructions and release ZIP.

> The next action is to implement Milestone 1 against your installed game and SMAPI versions. Get the shop working
> first; the empire should have a functioning cash register before we commission agricultural machinery.
