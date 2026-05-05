using HarmonyLib;
using Retinues.Game;
using Retinues.Troops;
using TaleWorlds.CampaignSystem;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// After the game loads saved troop data, clears the clan and kingdom
    /// Root/Militia troop slots so that existing saves don't keep using
    /// previously created custom troop trees.
    /// Retinue slots (RetinueBasic / RetinueElite) are preserved.
    /// </summary>
    [HarmonyPatch(typeof(FactionBehavior), "OnGameLoaded")]
    public static class ClearSavedTroopsPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ClearFaction(Player.Clan);
            ClearFaction(Player.Kingdom);
        }

        private static void ClearFaction(BaseFaction faction)
        {
            if (faction == null)
                return;

            faction.RootBasic = null;
            faction.RootElite = null;
            faction.MilitiaMelee = null;
            faction.MilitiaMeleeElite = null;
            faction.MilitiaRanged = null;
            faction.MilitiaRangedElite = null;
        }
    }
}
