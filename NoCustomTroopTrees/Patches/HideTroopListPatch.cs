using HarmonyLib;
using Retinues.GUI.Editor.VM.Troop.List;

namespace NoCustomTroopTrees.Patches
{
    /// <summary>
    /// Hides the Elite, Regular (Basic), and Militia troop categories
    /// from the Retinues editor UI, keeping only the Retinue list visible.
    /// </summary>
    [HarmonyPatch(typeof(TroopListVM), "get_ShowEliteList")]
    public static class HideEliteListPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(TroopListVM), "get_ShowBasicList")]
    public static class HideBasicListPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(TroopListVM), "get_ShowMilitiaList")]
    public static class HideMilitiaListPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
