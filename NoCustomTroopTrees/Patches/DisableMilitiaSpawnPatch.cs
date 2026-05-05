using HarmonyLib;
using Retinues.Features.Swaps.Patches;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Disables PlayerMilitiaSpawnPatch which replaces vanilla militia
    /// spawning in player settlements with custom militia troops.
    /// By skipping this Prefix, the game falls back to vanilla militia.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMilitiaSpawnPatch), "Prefix")]
    public static class DisableMilitiaSpawnPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = true; // tell Harmony to run the original Settlement.AddMilitiasToParty
            return false;
        }
    }
}
