using HarmonyLib;
using Retinues.Game.Wrappers;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Disables the volunteer swap system that replaces vanilla volunteers
    /// in settlements with custom faction troops.
    /// This is the core method called by VolunteerSwapOnUpdate and
    /// VolunteerSwapForPlayer, preventing town recruitment from being
    /// overridden with custom troops.
    /// Retinue troops are unaffected (managed by AutoJoinBehavior).
    /// </summary>
    [HarmonyPatch(typeof(WSettlement), "SwapVolunteers")]
    public static class DisableSettlementVolunteerSwapPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}
