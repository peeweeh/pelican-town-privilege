using StardewValley;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.Services
{
    /// <summary>
    /// Premium tool purchases. The Iridium Trash Can upgrade is applied directly when bought, so no
    /// unusable inventory item is created and no upgrade-queue wait applies.
    /// </summary>
    public sealed class PremiumToolService
    {
        public const int IridiumTrashCanLevel = 4;

        private readonly Diagnostics diagnostics;

        public PremiumToolService(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        /// <summary>
        /// Applies the trash-can upgrade. Returns true so the shop does not give the player an item.
        /// </summary>
        public bool HandleTrashCanPurchase(string shopId)
        {
            Farmer player = Game1.player;
            if (player.trashCanLevel >= IridiumTrashCanLevel)
            {
                Feedback.Show("Your trash can is already Iridium.");
                return true;
            }

            player.trashCanLevel = IridiumTrashCanLevel;
            Game1.playSound("coin");
            Feedback.Show("Iridium trash can installed.");
            diagnostics.Info($"Trash can upgraded to Iridium (shop {shopId}).");
            return true;
        }
    }
}
