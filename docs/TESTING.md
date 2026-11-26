# Testing checklist

The v0.1 code compiles against Stardew Valley 1.6.15 and SMAPI 4.5.2. **Nothing has been run in game yet.** Use this
checklist when you test. Tick a box only when you have seen the result in game, and write what you saw in the log at
the bottom.

## Setup

1. Install SMAPI 4.5.2 or newer in your Stardew Valley 1.6.15 install.
2. Build the mod (`dotnet build` in `src/PelicanTownPrivilege`) or copy the release zip's `PelicanTownPrivilege`
   folder into `Mods`.
3. Start the game through SMAPI and watch the console. You should see `Config loaded.` and, when the shop data loads,
   `Privilege shop: added N rows`. There should be no red errors from this mod.
4. Run `ptp_status`. Every setting should be listed. Note any "Skipped catalog rows".

## Acceptance tests (from the spec, section 11)

Rows 1 to 10 follow the spec table in order. The spec's incubation row is split into the coop check (row 10) and the ostrich check (row 11), so rows 12 to 16 are spec rows 11 to 15.

| # | Test | How to check | Pass condition | Result |
| --- | --- | --- | --- | --- |
| 1 | New game | Start a new save. Visit Pierre's shop on the first day. | Privilege rows appear at the end of the list. | Not run |
| 2 | Existing save | Load a save from before the mod was installed. | It loads, and progress is unchanged. | Not run |
| 3 | Prices | Look at each privilege row's price. | Each costs exactly 1g. | Not run |
| 4 | Sale values | Sell a Galaxy Sword or Iridium Sprinkler you already own. | Sale value is the normal one. | Not run |
| 5 | Galaxy weapons | Buy a Galaxy Sword. | It is bought with no unlock, and no discovery mail or quest changes. | Not run |
| 6 | Gifts | Give a villager a gift they like. Check the friendship change. | Buying and giving gifts keep normal relationship behaviour. No heart event fires on its own. | Not run |
| 7 | Junimo Hut | Open Robin's menu on day 1. | Junimo Hut is listed in Robin's menu, costs 0g, needs no materials, and builds instantly. | Not run |
| 8 | Quest isolation | Buy a Junimo Hut and a Galaxy Sword. | No unrelated quest or Wizard step completes. | Not run |
| 9 | Blue Chicken | Visit Marnie on day 1. | Blue Chicken costs 1g, can be bought on day 1, and can be assigned to a coop. | Not run |
| 10 | Incubation | Put a Dinosaur Egg in a coop incubator, then sleep. | The egg hatches after one night, with no extra days needed. | Not run |
| 11 | Incubation (ostrich) | Put an Ostrich Egg in an Ostrich Incubator, then sleep. | The egg hatches after one night, with no extra days needed. This is separate from row 10, because the spec asks for the two systems to be tested apart. | Not run |
| 12 | Dinosaur production | Feed and pet a dinosaur for several days. | A fed and cared-for dinosaur produces once per day. | Not run |
| 13 | Wide hoe | Hold the Privilege Hoe on the farm and use it facing up, down, left, and right. | Each use tills up to 81 tiles (9 across and 9 deep) in front of you, starting one tile ahead. | Not run |
| 14 | Wide planter | Load 3 parsnip seeds, then use the planter on a row of tilled soil. | 3 tiles are planted, and exactly 3 seeds are used. | Not run |
| 15 | Protected objects | Use the wide hoe and planter next to crops, a tree, a chest, and a path. | None of those are removed or changed. | Not run |
| 16 | Removal | Run `ptp_cleanup` (dry run), then `ptp_cleanup apply` on a copy of the save. | Seeds come back, tags are cleared, and the report says "Ready to load without the mod: YES". | Not run |
| 17 | Wide hoe on grass | Stand on uncut grass on the farm, and use the Privilege Hoe facing it. | Grass is cleared and the same tiles are tilled in that use. Then the Privilege Planter can plant there. | Not run |
| 18 | Privilege Scythe | Hold the Privilege Scythe next to grass, weeds, a ripe scythe crop such as Wheat, and a tilled tile with no crop. Swing. | Grass and weeds in the area are cut, ripe scythe crops are harvested, the tilled tile stays, and combat still works. | Not run |
| 19 | Privilege Pickaxe | Stand near a few stones, an ore node, a boulder, a sprinkler, and a tilled tile. Use the Privilege Pickaxe facing each. | Stones break, the boulder breaks after four uses in the same spot, and the sprinkler, soil, and crops stay. | Not run |
| 20 | Pickaxe outside the farm | Use the Privilege Pickaxe in the mines on a stone. | It breaks only that stone, as a normal Iridium pickaxe. There is no area and no message. | Not run |
| 21 | Shop rows | Open Pierre's shop. | Privilege Scythe and Privilege Pickaxe are listed at 1g each. | Not run |
| 22 | Wide hoe on weeds | Stand on weedy ground on the farm, and use the Privilege Hoe facing it. | Weeds are removed and the same tiles are tilled in that use. | Not run |
| 23 | Twigs block tilling | Use the Privilege Hoe facing a twig. | The twig stays and its tile stays untilled. The Privilege Axe or a normal Axe clears twigs. | Not run |
| 24 | Planter in spring | Load Parsnip Seeds, then use the Privilege Planter facing tilled soil in spring. | Each empty tilled tile in the area gets a parsnip, and the seed count drops. | Not run |
| 25 | Privilege Axe | Stand near a tree, a stump or log, a twig, and a weed, then use the Privilege Axe facing each. | Trees fall after enough chops, stumps and logs break, and the twig and weed are removed. | Not run |
| 26 | Axe leaves fruit trees | Stand next to a fruit tree and use the Privilege Axe on it. | The fruit tree stays. | Not run |
| 27 | Axe outside the farm | Use the Privilege Axe on a tree in town. | It chops like a normal axe, with no area. | Not run |
| 28 | Privilege Iridium Sprinkler | Place a privilege Iridium Sprinkler on the farm and look at the area it covers. Then wait a night. | The area is 9×9 with the sprinkler in the middle, and the tilled tiles in it are watered overnight. | Not run |
| 29 | Pet Bowl | Open Robin's menu on day 1. | Pet Bowl is listed, costs 0g, needs no materials, and builds instantly. | Not run |
| 30 | Pet bowl watering | Place the Pet Bowl, water it with a watering can, and sleep. Repeat without watering. | A watered bowl gives +6 friendship each day. An unwatered bowl gives nothing, and a pet with no bowl loses 10 each day. | Not run |
| 31 | Planter debug | Set `EnableDebugLogging` to `true`, restart, and use the planter on tilled soil. | The SMAPI log has `Planter:` lines. Each says whether the crop data was found and why tiles were skipped. | Not run |

## Additional checks

| # | Check | Pass condition | Result |
| --- | --- | --- | --- |
| A1 | Footprint preview | Hold a privilege tool. Area tiles show green or red, and they rotate as you turn. | Not run |
| A2 | Load seeds | Hold seeds and press the Load key. Message shows the seed and total. Seeds leave the inventory. | Not run |
| A3 | Unload seeds | Hold the planter and press the Load key. All seeds return to the inventory. | Not run |
| A4 | Planter tooltip | Hover over the planter in the inventory. | The description shows the seed and count. | Not run |
| A5 | Planter cannot be trashed with seeds | Try to drop the planter in the trash can while it holds seeds. | It is refused. | Not run |
| A6 | Iridium Trash Can | Buy the upgrade at Clint's. | Trash can level is Iridium, no item appears in the inventory, and the row disappears. | Not run |
| A7 | Tools kept | Buy an Iridium Axe while owning a Copper Axe. | Both axes are in the inventory. | Not run |
| A8 | Outside the farm | Use a privilege hoe in town. | Nothing changes, and a message says where it works. No energy is used. | Not run |
| A9 | Energy | Compare one wide-hoe use with one normal hoe use. | Stamina drops by the same amount. | Not run |
| A10 | Gift warning | Open Pierre's shop. | The warning line is visible above the shop. | Not run |
| A11 | Invalid config | Set `ToolWidth` to `16` and `ShopPurchasePrice` to `-5`, then start the game. | Both values are corrected, and each correction is logged. | Not run |
| A12 | Bad item ID | Change one `ItemId` in `catalog.json` to `(O)999999`. | The row is found by name if it exists. Otherwise it is skipped and logged, and the game keeps running. | Not run |
| A13 | Multiplayer | Load a multiplayer save. | A warning appears in the log. Nothing else is tested. | Not run |
| A14 | Selling a loaded planter | Known gap: selling is not blocked in v0.1. Sell a loaded planter to a shop and record what happens. | Recorded in the log below. A failure here is expected until the gap is fixed. | Not run |
| A15 | Crop seeds | Buy Parsnip Seeds at Pierre's in winter, then try to plant them with the Privilege Planter. Each seed costs 1g, and nothing is planted, because parsnip cannot grow in winter. | Not run |
| A16 | Storage | Open Pierre's shop. Big Chest and Big Stone Chest are listed at 1g each, and each holds almost twice a regular chest. | Not run |
| A17 | Scythe toggle | Set `EnableWideScythe` to `false`, restart, and swing a Privilege Scythe. | Only the normal swing happens. | Not run |

## Log for this test run

Record the game version, SMAPI version, operating system, and any errors:

```
Date:
Game version:
SMAPI version:
Operating system:
Notes:
```
