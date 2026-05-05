using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Troops;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Prevents the creation of clan and kingdom custom troop trees
    /// (RootBasic / RootElite) while keeping retinue troops
    /// (RetinueBasic / RetinueElite, i.e. House Guard / King's Champion) intact.
    /// </summary>
    [HarmonyPatch(typeof(TroopBuilder), "EnsureRegularTroops")]
    public static class DisableRegularTroopsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(WFaction faction, ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
