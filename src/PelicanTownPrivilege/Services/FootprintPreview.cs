using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Draws the 9x1 strip over the world while a privilege tool is held.
    /// Green means the tile will be used. Red means it will be skipped.
    /// </summary>
    public sealed class FootprintPreview
    {
        private readonly ModConfig config;
        private Texture2D pixel;

        public FootprintPreview(ModConfig config)
        {
            this.config = config;
        }

        public void Draw(SpriteBatch batch)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree) return;

            Farmer who = Game1.player;
            Item held = who.CurrentItem;
            bool wide = config.EnableWideHoe && PrivilegeTools.IsWideHoe(held);
            bool planter = config.EnableWidePlanter && PrivilegeTools.IsPlanter(held);
            if (!wide && !planter) return;

            GameLocation location = Game1.currentLocation;
            bool allowed = ToolFootprint.IsAllowedLocation(location, config);
            string seedId = planter ? PrivilegeTools.GetSeedId(held) : null;
            int size = (int)(ToolFootprint.TileSize * Game1.options.zoomLevel);

            pixel ??= CreatePixel();
            foreach (Vector2 tile in ToolFootprint.Compute(who, config.ToolWidth))
            {
                bool valid = allowed && (wide
                    ? ToolFootprint.CanTill(location, tile)
                    : ToolFootprint.TryGetPlantableSoil(location, tile, seedId, out _));

                Color color = valid ? Color.LimeGreen * 0.45f : Color.Red * 0.35f;
                Vector2 screen = Game1.GlobalToLocal(new Vector2(tile.X * ToolFootprint.TileSize, tile.Y * ToolFootprint.TileSize));
                batch.Draw(pixel, new Rectangle((int)screen.X, (int)screen.Y, size, size), color);
            }
        }

        private static Texture2D CreatePixel()
        {
            var texture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
}
