using HarmonyLib;
using Retinues.Game.Wrappers;
using Retinues.Troops;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Prevents the creation of custom militia troops.
    /// </summary>
    [HarmonyPatch(typeof(TroopBuilder), "EnsureMilitiaTroops")]
    public static class DisableMilitiaTroopsPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(WFaction faction, ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
