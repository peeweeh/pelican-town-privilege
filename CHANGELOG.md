# Changelog

All notable changes to this project are recorded here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Crop seeds at Pierre's privilege counter: the crop seeds Pierre normally sells, at 1g in any season. Turn them off with `EnableCropSeeds`.
- Big Stone Chest at Pierre's privilege counter, 1g, in the storage section. Turn it off with `EnableStorageStock`.
- Privilege Scythe at Pierre's privilege counter, 1g. After each swing, it cuts grass, clears weeds, and harvests ripe scythe crops across the 10×10 area. Soil is never removed, and the normal swing and combat are unchanged. Turn it off with `EnableWideScythe`.
- Privilege Pickaxe at Pierre's privilege counter, 1g. It is built on the Iridium Pickaxe, so it can break boulders. One use breaks every stone, ore node, and boulder in the 10×10 area. Soil, crops, trees, and crafted objects are never touched. Outside the farm and greenhouse it works as a normal pickaxe. Turn it off with `EnableWidePickaxe`.

### Changed

- The Privilege Hoe and Planter cover a 10×10 area (`ToolWidth` × `ToolDepth`, both default 10) instead of a 9×1 strip.
- `ToolWidth` no longer has to be odd. Even widths reach one tile further right (or down) than left (or up).
- The Big Chest row now uses its correct item ID, `(BC)BigChest`. Before, it used the Stone Chest ID and relied on the mod's name lookup.
- The Junimo Hut costs 0g (was 1g). `JunimoHutPrice` now defaults to `0`.
- `ToolWidth` and `ToolDepth` now apply to all four wide-area tools: hoe, planter, scythe, and pickaxe.
- The planter's failure message says the soil cannot grow the seed right now. Before, it said no tilled soil accepted the seed, which hid season and location limits.

### Fixed

- The Junimo Hut was missing from Robin's construction menu. The game lists each building only under its builder, and the hut's builder was not Robin. The mod now sets it to Robin.
- The Privilege Hoe never tilled grass. The game's grass ignores hoes, so planting had no tilled soil to use. The hoe now clears a grass tuft and tills it in the same use. Grass cleared by the hoe drops nothing.
- The Privilege Hoe skipped weedy tiles. Weeds in the area are now removed and tilled in the same use, like grass. Stones, boulders, and twigs still block tilling.

## [0.1.0] - 2026-11-23

First build, written from the v0.1 specification. It compiles against Stardew Valley 1.6.15 and SMAPI 4.5.2. It has
**not been run in game yet**.

### Added

- Pierre's Privilege Counter: Iridium Sprinkler, Auto-Grabber, Auto-Petter, machines, storage, rare eggs, Ostrich
  Incubator, universal gifts, Stardrop Tea, and the Galaxy Sword, Dagger, and Hammer. All cost 1g.
- Clint's Blacksmith: Iridium Axe, Pickaxe, Hoe, Watering Can, and the Iridium Trash Can upgrade. All cost 1g.
- Privilege Hoe: tills a 9×1 strip in front of the player, rotating with facing. It costs the same energy as one normal
  hoe use.
- Privilege Planter: loads a seed stack, then plants one seed per valid tile in the strip.
- Early Junimo Huts from Robin: 1g, no materials, instant construction, and no Wizard requirement.
- Early Blue Chickens from Marnie (1g), and Dinosaurs that produce once a day.
- Overnight incubation for coop incubators and Ostrich Incubators.
- `config.json` settings for every section, with validation and logged corrections.
- Console commands: `ptp_status` shows the settings and skipped rows. `ptp_cleanup` is a dry run by default, and
  `ptp_cleanup apply` makes a save backup before it changes anything.
- Documentation: README, specification, configuration, architecture, editing guide, testing checklist, and cleanup
  guide.

### Known issues

- Not yet tested in game. See [docs/TESTING.md](docs/TESTING.md).
- Selling a loaded planter is not blocked. Trashing is blocked.
- Catalog item IDs are best-known. Rows the game cannot resolve are skipped and listed by `ptp_status`.
- Multiplayer is outside the v0.1 scope.
