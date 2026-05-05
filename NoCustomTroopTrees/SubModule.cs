using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace NoCustomTroopTrees
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                _harmony = new Harmony("NoCustomTroopTrees");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print(
                    "[NoCustomTroopTrees] Harmony patch failed: " + ex.Message,
                    0,
                    TaleWorlds.Library.Debug.DebugColor.Red);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("NoCustomTroopTrees");
        }
    }
}
