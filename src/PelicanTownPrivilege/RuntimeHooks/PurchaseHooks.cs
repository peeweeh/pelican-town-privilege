using HarmonyLib;
using PelicanTownPrivilege.Tools;

namespace PelicanTownPrivilege.RuntimeHooks
{
    /// <summary>
    /// Shop purchase of the Iridium Trash Can upgrade. The shop calls actionWhenPurchased before handing over the
    /// item. Returning true there means the purchase was handled, so no unusable item is created.
    /// </summary>
    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.actionWhenPurchased))]
    public static class ObjectActionWhenPurchasedPatch
    {
        public static bool Prefix(StardewValley.Object __instance, string shopId, ref bool __result)
        {
            if (!PrivilegeTools.IsTrashCanSentinel(__instance)) return true;

            __result = HookServices.PremiumTools.HandleTrashCanPurchase(shopId);
            return false;
        }
    }
}
