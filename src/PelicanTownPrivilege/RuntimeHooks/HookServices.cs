using PelicanTownPrivilege.Configuration;
using PelicanTownPrivilege.Services;

namespace PelicanTownPrivilege.RuntimeHooks
{
    /// <summary>Service references used by the Harmony hooks. ModEntry sets them once, before patching.</summary>
    public static class HookServices
    {
        public static ModConfig Config { get; set; }
        public static Diagnostics Diagnostics { get; set; }
        public static WideHoeService WideHoe { get; set; }
        public static WidePlanterService Planter { get; set; }
        public static WideScytheService WideScythe { get; set; }
        public static WidePickaxeService WidePickaxe { get; set; }
        public static PremiumToolService PremiumTools { get; set; }
    }
}
