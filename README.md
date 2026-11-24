# Pelican Town Privilege

A personal [SMAPI](https://smapi.io) mod for Stardew Valley. It puts premium tools, useful machines, rare animals, and
Junimo Huts within reach from the start.

The principle: keep the farming, collecting, relationships, and exploration. Remove the procurement grind.

> **Status: v0.1.0. It compiles, but it has not been run in game yet.** Work through
> [docs/TESTING.md](docs/TESTING.md) before you rely on it.

## What it adds

| Where | What | Price |
| --- | --- | --- |
| Pierre's General Store | Iridium Sprinkler, Auto-Grabber, Auto-Petter, Keg, Preserves Jar, Mayonnaise Machine, Cheese Press, chests, rare eggs, Ostrich Incubator, universal gifts, Stardrop Tea, Galaxy Sword, Dagger, and Hammer, crop seeds | 1g each |
| Pierre's General Store | Privilege Hoe and Privilege Planter (wide-area farming tools) | 1g each |
| Clint's Blacksmith | Iridium Axe, Pickaxe, Hoe, Watering Can, and the Iridium Trash Can upgrade | 1g each |
| Robin's construction menu | Junimo Hut, with no Wizard requirement, no materials, and instant construction. Junimo helpers come out on their own (up to 3 per hut) when crops within 8 tiles are ripe. | 1g |
| Marnie's animal shop | Blue Chicken, available from the start | 1g |

Also:

- **Wide tools.** The Privilege Hoe tills, and the Privilege Planter plants, a 10×10 area in front of you. The area
  rotates with your facing, and a preview shows which tiles will be used.
- **Overnight incubation.** Eggs in coop incubators and Ostrich Incubators finish when the day ends.
- **Daily Dinosaur eggs.** A fed, cared-for dinosaur produces once a day.
- **Save cleanup.** `ptp_cleanup` returns planter seeds and clears the mod's tags, so a save can load without the mod.

Everything can be switched off in `config.json`. See [docs/CONFIGURATION.md](docs/CONFIGURATION.md).

Not included: new quests, heart events, gift changes, automatic planting or shipping, and infinity weapons. The full
scope is in [docs/SPEC.md](docs/SPEC.md).

## Install

You need:

- Stardew Valley 1.6.15
- SMAPI 4.5.2 or newer

Copy the `PelicanTownPrivilege` folder into your game's `Mods` folder. You can build it yourself (see below), or use a
release zip if one is published. Then start the game through SMAPI.

Once the game is running, type `ptp_status` in the SMAPI console. It lists every setting and any shop rows that were
skipped.

## Build from source

You need the .NET SDK, and SMAPI installed in your game folder. The build reads SMAPI's DLLs from there.

### macOS and Linux

```bash
cd src/PelicanTownPrivilege
dotnet build
```

### Windows

Install the .NET SDK once (approve any UAC prompt), then open a new PowerShell window so that `dotnet` is on your PATH. .NET 10 is the long-term-support release:

```powershell
winget install Microsoft.DotNet.SDK.10
dotnet --version
```

Get the code and build it. If you don't have Git, use **Code > Download ZIP** on the GitHub page and unzip it instead:

```powershell
git clone https://github.com/peeweeh/pelican-town-privilege.git
cd pelican-town-privilege\src\PelicanTownPrivilege
dotnet build
```

### Game folder

The build finds a Steam install in your main Steam library automatically. If your game is somewhere else (for example, a second Steam drive), create a file named `stardewvalley.targets` in your home folder (`C:\Users\<you>` on Windows, `/Users/<you>` on macOS) with this content, changing the path:

```xml
<Project>
  <PropertyGroup>
    <GamePath>D:\SteamLibrary\steamapps\common\Stardew Valley</GamePath>
  </PropertyGroup>
</Project>
```

This file lives outside the repo, so Git never sees it. It is also easier than `-p:GamePath=...` on the command line, where quoting paths with spaces can be fiddly in PowerShell.

### Options and problems

- `dotnet build -c Release` also writes a zip to `bin/Release/net6.0/` (`bin\Release\net6.0\` on Windows).
- `-p:EnableModDeploy=false` skips copying the mod into your game's `Mods` folder.
- If the build says it can't find your game folder, create `stardewvalley.targets` as described above.
- If the build says the game folder doesn't contain SMAPI, install SMAPI into that folder, then build again.
- If copying into `Mods` fails with an access error on Windows, run PowerShell as administrator, or use `-p:EnableModDeploy=false` and copy the `PelicanTownPrivilege` folder yourself.

## Configuration

Settings live in `config.json` in the mod's folder. Restart the game after every change. Invalid values are reset to
their defaults, and each correction is logged. The full list is in [docs/CONFIGURATION.md](docs/CONFIGURATION.md).

## Changing the mod (vibe coding welcome)

Most changes fall into one of these:

- **Change a price, turn a section off, or change a key:** edit `config.json`.
- **Add or remove a shop item:** edit `src/PelicanTownPrivilege/assets/catalog.json`. Each row names a shop, a switch,
  and an item. Item IDs are checked against the game, and rows that cannot be found are skipped and listed by
  `ptp_status`.
- **Change how a tool or building works:** edit the matching file in `src/PelicanTownPrivilege/Services/`.

[docs/EDITING-GUIDE.md](docs/EDITING-GUIDE.md) has a recipe for each kind of change, plus copy-and-paste prompts for AI
assistants. [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) explains which file does what.

## Documentation

| Document | What it covers |
| --- | --- |
| [docs/SPEC.md](docs/SPEC.md) | The v0.1 specification this mod follows |
| [docs/CONFIGURATION.md](docs/CONFIGURATION.md) | Every setting, its default, and how invalid values are handled |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | How the code is organised, and the decisions behind it |
| [docs/EDITING-GUIDE.md](docs/EDITING-GUIDE.md) | Recipes for common changes, and prompts for AI assistants |
| [docs/TESTING.md](docs/TESTING.md) | The in-game test checklist, with a place to record results |
| [docs/CLEANUP.md](docs/CLEANUP.md) | Removing the mod from a save |
| [CHANGELOG.md](CHANGELOG.md) | Version history |

## Known gaps (v0.1)

- Not tested in game yet.
- Selling a loaded planter is not blocked. Only trashing is.
- Catalog item IDs are best-known. Anything the game cannot find is skipped and reported.
- Multiplayer is outside the v0.1 scope. A warning is logged when a multiplayer save loads.

## License

No license has been chosen yet, so the code is not licensed for reuse. Add a `LICENSE` file if you want others to use it.

## Disclaimer

This is an unofficial fan mod. Stardew Valley is made by ConcernedApe, and this project is not affiliated with or
endorsed by them.
