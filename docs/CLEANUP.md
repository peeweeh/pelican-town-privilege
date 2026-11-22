# Cleanup and removal

Custom tools need a plan for removal. The Privilege Hoe and Planter store their state on the item, so a save that
still holds them has to be cleaned before it can load without the mod. `ptp_cleanup` does that.

> **Status: untested.** Uninstall safety is not guaranteed until the removal test in [TESTING.md](TESTING.md) passes.
> Make a backup of your save folder yourself before you try this.

## What is and is not affected by removal

| Item | Without the mod | Notes |
| --- | --- | --- |
| Shop rows you bought (Iridium tools, sprinklers, machines, eggs, weapons) | Ordinary items. Nothing to clean. | Future purchases stop being discounted. Existing items are not taken back. |
| Junimo Huts, Blue Chickens, Ostrich Incubators, and other buildings or animals | Ordinary buildings and animals. Nothing to clean. | The Wizard requirement and purchase price only applied when buying. |
| Privilege Hoe | Behaves as a normal hoe. | Its tags are removed by `ptp_cleanup apply`. |
| Privilege Planter | Behaves as a normal hoe, so it would till instead of plant. | Its stored seeds are returned to you by `ptp_cleanup apply`. Its tags are then removed. |
| Trash-can upgrade | Stays at Iridium. | The upgrade is the game's own state. Nothing to reverse. |

## Running the cleanup

Load the save you want to clean, then run:

```
ptp_cleanup
```

With no argument this is a **dry run**. It lists every privilege tool and every seed it found, and it changes nothing.
It checks every player's inventory, and every chest in the game's main locations and inside buildings.

To clean up for real:

```
ptp_cleanup apply
```

Apply does the following, in order:

1. Makes a backup of the save folder in `PelicanTownPrivilege-backups`, beside your `Saves` folder. If the backup
   fails, nothing changes.
2. Returns each planter's seeds to your inventory. Seeds that do not fit stay in the planter, and the tool keeps its
   tags.
3. Removes the mod's tags from each privilege tool. Each one becomes an ordinary hoe.
4. Scans again and prints `Ready to load without the mod: YES` or `NO`.

Save the game after a successful cleanup.

## When cleanup cannot finish

The report lists anything that could not be cleaned, with the reason. The usual cause is a full inventory: make room
and run `ptp_cleanup apply` again. The other cause is a seed the game does not know, for example one from a mod you
removed. Those seeds cannot be returned, so the planter keeps them and its tags. Each run makes its own backup.

The scan covers player inventories and chests only. Machine outputs, mail, and anything held outside the game's main
locations and buildings are not scanned. Keep privilege tools out of those places before you remove the mod.

## Removing the mod

1. Run `ptp_cleanup` (dry run) and check the list.
2. Run `ptp_cleanup apply` and confirm `Ready to load without the mod: YES`.
3. Save the game.
4. Close the game and remove the `PelicanTownPrivilege` folder from `Mods`.

If you skip step 2, the save still loads, but privilege tools behave as ordinary hoes and their seeds stay in them.
