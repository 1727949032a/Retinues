using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;

// Token: 0x0200000F RID: 15
[HarmonyPatch(typeof(ClanManagementVM))]
internal static class ClanManagementVM_HotkeyGuards
{
	// Token: 0x06000027 RID: 39 RVA: 0x0000216F File Offset: 0x0000036F
	[HarmonyPrefix]
	[HarmonyPatch("SelectPreviousCategory")]
	private static bool Prev_Prefix()
	{
		return !HotkeyBlocker.BlockHotkeys;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x0000217B File Offset: 0x0000037B
	[HarmonyPrefix]
	[HarmonyPatch("SelectNextCategory")]
	private static bool Next_Prefix()
	{
		return !HotkeyBlocker.BlockHotkeys;
	}
}
