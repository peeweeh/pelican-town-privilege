using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Menus;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Content;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Adds the privilege rows from assets/catalog.json to Pierre's shop (SeedShop) and Clint's shop (Blacksmith).
    /// Existing rows are never changed or removed; privilege rows are appended after them.
    /// </summary>
    public sealed class ShopService
    {
        public const string PierreShopId = "SeedShop";
        public const string GiftWarning = "Cheap does not mean universally appreciated. Normal gift preferences still apply.";

        private readonly ModConfig config;
        private readonly CatalogFile catalog;
        private readonly ItemResolver resolver;
        private readonly Diagnostics diagnostics;

        public ShopService(ModConfig config, CatalogFile catalog, ItemResolver resolver, Diagnostics diagnostics)
        {
            this.config = config;
            this.catalog = catalog;
            this.resolver = resolver;
            this.diagnostics = diagnostics;
        }

        public void EditShops(IAssetData asset)
        {
            if (!config.EnablePrivilegeShop)
            {
                diagnostics.Info("EnablePrivilegeShop is false: no privilege rows added to shops.");
                return;
            }

            IDictionary<string, ShopData> shops = asset.AsDictionary<string, ShopData>().Data;
            ResolutionSession session = resolver.StartSession();
            int added = 0;

            foreach (CatalogEntry entry in catalog.Entries ?? new List<CatalogEntry>())
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Shop)) continue;

                string label = $"{entry.Section ?? "privilege"} / {entry.DisplayName ?? entry.Name ?? entry.ItemId}";
                if (!ModConfig.IsToggle(entry.Toggle))
                {
                    diagnostics.Skip(label, $"unknown Toggle '{entry.Toggle}' (must match a true/false setting in config.json)");
                    continue;
                }
                if (!config.IsEnabled(entry.Toggle)) continue;

                if (!shops.TryGetValue(entry.Shop, out ShopData shop) || shop == null)
                {
                    diagnostics.Skip(label, $"shop '{entry.Shop}' was not found");
                    continue;
                }

                ResolvedItem item = session.Resolve(entry);
                if (item == null) continue;

                shop.Items ??= new List<ShopItemData>();
                shop.Items.Add(CreateRow(entry, item));
                added++;
            }

            diagnostics.Info($"Privilege shop: added {added} rows (Pierre's and Clint's).");
        }

        /// <summary>Draws the price line and gift warning while Pierre's shop is open.</summary>
        public void DrawCounterNotes(SpriteBatch batch)
        {
            if (!config.EnablePrivilegeShop) return;
            if (Game1.activeClickableMenu is not ShopMenu shop || shop.ShopId != PierreShopId) return;

            Vector2 top = new Vector2(shop.xPositionOnScreen, System.Math.Max(0, shop.yPositionOnScreen - 60));
            batch.DrawString(Game1.smallFont, $"Privilege Counter: {config.ShopPurchasePrice}g each, listed at the end of the shop.", top, Color.White);
            batch.DrawString(Game1.smallFont, GiftWarning, top + new Vector2(0, 26), Color.LightYellow);
        }

        private ShopItemData CreateRow(CatalogEntry entry, ResolvedItem item)
        {
            var row = new ShopItemData
            {
                Id = "PelicanTownPrivilege." + Slug(entry.DisplayName ?? entry.Name ?? item.QualifiedId),
                ItemId = item.QualifiedId,
                Price = config.ShopPurchasePrice,
                IgnoreShopPriceModifiers = true,
                ApplyProfitMargins = false,
                Condition = string.IsNullOrWhiteSpace(entry.Condition) ? null : entry.Condition
            };

            if (!string.IsNullOrWhiteSpace(entry.DisplayName))
                row.ObjectDisplayName = entry.DisplayName;

            if (entry.ModData != null && entry.ModData.Count > 0)
                row.ModData = new Dictionary<string, string>(entry.ModData);

            if (!config.UnlimitedPrivilegeStock)
            {
                row.AvailableStock = config.PrivilegeStockLimit;
                row.AvailableStockLimit = LimitedStockMode.Player;
            }

            return row;
        }

        private static string Slug(string text)
        {
            return new string((text ?? string.Empty).Where(char.IsLetterOrDigit).ToArray());
        }
    }
}
