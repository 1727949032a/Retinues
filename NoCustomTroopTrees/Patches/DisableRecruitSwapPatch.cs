using HarmonyLib;
using Retinues.Features.Volunteers.Patches;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Disables the AI recruit swap that replaces vanilla troops with
    /// custom troops when AI lords recruit from settlements.
    /// </summary>
    [HarmonyPatch(typeof(VolunteerSwapForAI), "Postfix")]
    public static class DisableRecruitSwapPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}
