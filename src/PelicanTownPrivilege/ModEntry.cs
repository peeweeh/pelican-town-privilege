using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Content;
using PelicanTownPrivilege.RuntimeHooks;
using PelicanTownPrivilege.Services;

namespace PelicanTownPrivilege
{
    /// <summary>
    /// Entry point. Loads and validates config, reads the shop catalog, creates the services, installs the
    /// Harmony hooks, and subscribes to SMAPI events and console commands.
    /// </summary>
    public sealed class ModEntry : Mod
    {
        private ModConfig config;
        private Diagnostics diagnostics;
        private ShopService shops;
        private JunimoBuildingService junimoHuts;
        private PetBowlService petBowl;
        private AnimalService animals;
        private IncubationService incubation;
        private WidePlanterService planter;
        private FootprintPreview footprint;
        private CleanupService cleanup;
        private SButton loadSeedsButton;

        public override void Entry(IModHelper helper)
        {
            config = ConfigValidator.Load(helper, Monitor);
            diagnostics = new Diagnostics(Monitor, config, ModManifest.Version.ToString());

            CatalogFile catalog = LoadCatalog(helper);
            var resolver = new ItemResolver(helper.GameContent, diagnostics);
            var saveMigration = new SaveMigrationService(diagnostics);

            shops = new ShopService(config, catalog, resolver, diagnostics);
            junimoHuts = new JunimoBuildingService(config, diagnostics);
            petBowl = new PetBowlService(config, diagnostics);
            animals = new AnimalService(config, diagnostics);
            incubation = new IncubationService(config, diagnostics);
            planter = new WidePlanterService(config, diagnostics);
            footprint = new FootprintPreview(config);
            cleanup = new CleanupService(saveMigration, diagnostics);

            HookServices.Config = config;
            HookServices.Diagnostics = diagnostics;
            HookServices.WideHoe = new WideHoeService(config, diagnostics);
            HookServices.Planter = planter;
            HookServices.WideScythe = new WideScytheService(config);
            HookServices.WidePickaxe = new WidePickaxeService(config);
            HookServices.WideAxe = new WideAxeService(config);
            HookServices.PremiumTools = new PremiumToolService(diagnostics);
            new Harmony(ModManifest.UniqueID).PatchAll(typeof(ModEntry).Assembly);

            Enum.TryParse<SButton>(config.LoadSeedsKey.Trim(), true, out loadSeedsButton);

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayEnding += (sender, e) => incubation.OnDayEnding();
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.RenderedWorld += (sender, e) => footprint.Draw(e.SpriteBatch);
            helper.Events.Display.RenderedActiveMenu += (sender, e) => shops.DrawCounterNotes(e.SpriteBatch);

            helper.ConsoleCommands.Add("ptp_status",
                "Shows Pelican Town Privilege settings, versions, and any shop rows that were skipped.",
                (command, args) => diagnostics.PrintStatus());
            helper.ConsoleCommands.Add("ptp_cleanup",
                "Prepares a save to load without this mod. Dry run by default; use 'ptp_cleanup apply' to change the save.",
                (command, args) => cleanup.Run(args.Length > 0 && args[0].Equals("apply", StringComparison.OrdinalIgnoreCase)));
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
                e.Edit(asset => shops.EditShops(asset));
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
            {
                e.Edit(asset => junimoHuts.EditBuildings(asset));
                e.Edit(asset => petBowl.EditBuildings(asset));
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/FarmAnimals"))
                e.Edit(asset => animals.EditFarmAnimals(asset));
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMultiplayer)
                Monitor.Log("Multiplayer is outside the v0.1 test scope. Privilege features are untested with other players.", LogLevel.Warn);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!config.EnableWidePlanter || e.Button != loadSeedsButton) return;
            if (!Context.IsWorldReady || !Context.IsPlayerFree) return;

            planter.HandleLoadSeedsKey(Game1.player);
        }

        private CatalogFile LoadCatalog(IModHelper helper)
        {
            try
            {
                CatalogFile catalog = helper.Data.ReadJsonFile<CatalogFile>("assets/catalog.json");
                if (catalog != null) return catalog;

                diagnostics.Warn("assets/catalog.json was not found. No privilege shop rows will be added.");
            }
            catch (Exception ex)
            {
                diagnostics.Error("assets/catalog.json could not be read. No privilege shop rows will be added", ex);
            }

            return new CatalogFile();
        }
    }
}
