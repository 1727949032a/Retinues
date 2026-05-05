using System;
using HarmonyLib;
using TaleWorlds.InputSystem;

// Token: 0x02000011 RID: 17
[HarmonyPatch(typeof(Input))]
internal static class Input_LKey_Blockers
{
	// Token: 0x0600002B RID: 43 RVA: 0x000021C1 File Offset: 0x000003C1
	[HarmonyPostfix]
	[HarmonyPatch("IsKeyDown", new Type[]
	{
		typeof(InputKey)
	})]
	private static void IsKeyDown_Postfix(InputKey key, ref bool __result)
	{
		if (__result && ClanHotkeyGate.Matches(key))
		{
			__result = false;
		}
	}

	// Token: 0x0600002C RID: 44 RVA: 0x000021D2 File Offset: 0x000003D2
	[HarmonyPostfix]
	[HarmonyPatch("IsKeyPressed", new Type[]
	{
		typeof(InputKey)
	})]
	private static void IsKeyPressed_Postfix(InputKey key, ref bool __result)
	{
		if (__result && ClanHotkeyGate.Matches(key))
		{
			__result = false;
		}
	}

	// Token: 0x0600002D RID: 45 RVA: 0x000021E3 File Offset: 0x000003E3
	[HarmonyPostfix]
	[HarmonyPatch("IsKeyReleased", new Type[]
	{
		typeof(InputKey)
	})]
	private static void IsKeyReleased_Postfix(InputKey key, ref bool __result)
	{
		if (__result && ClanHotkeyGate.Matches(key))
		{
			__result = false;
		}
	}
}
