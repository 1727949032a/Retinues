using HarmonyLib;
using Retinues.Features.Swaps;
using TaleWorlds.CampaignSystem.Party;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Disables the daily party swap that replaces vanilla troops in
    /// militia, caravan, and villager parties with custom faction troops.
    /// </summary>
    [HarmonyPatch(typeof(PartySwapBehavior), "OnDailyTickParty")]
    public static class DisablePartySwapPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(MobileParty party)
        {
            return false;
        }
    }
}
