using HarmonyLib;
using Retinues.Game.Wrappers;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Disables WRoster.SwapTroops and SwapTroopsPreservingHeroes which
    /// replace vanilla troops in party rosters with custom faction troops.
    /// Called by MilitiaSwap, CaravanSwap, VillagerSwap initialization
    /// patches and PartySwapBehavior daily tick.
    /// </summary>
    [HarmonyPatch(typeof(WRoster), "SwapTroops")]
    public static class DisableRosterSwapTroopsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(WRoster), "SwapTroopsPreservingHeroes")]
    public static class DisableRosterSwapTroopsPreservingHeroesPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}
