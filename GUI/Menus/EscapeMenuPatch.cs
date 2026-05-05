using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Utils;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;

namespace Retinues.GUI.Menus
{
	// Token: 0x02000062 RID: 98
	[HarmonyPatch(typeof(EscapeMenuVM))]
	internal static class EscapeMenuPatch
	{
		// Token: 0x060001ED RID: 493 RVA: 0x0000E44C File Offset: 0x0000C64C
		[HarmonyPostfix]
		[HarmonyPatch(MethodType.Constructor, new Type[]
		{
			typeof(IEnumerable<EscapeMenuItemVM>),
			typeof(TextObject)
		})]
		private static void Postfix(EscapeMenuVM __instance)
		{
			EscapeMenuPatch.<>c__DisplayClass0_0 CS$<>8__locals1 = new EscapeMenuPatch.<>c__DisplayClass0_0();
			CS$<>8__locals1.__instance = __instance;
			try
			{
				if (Config.EnableGlobalEditor)
				{
					CS$<>8__locals1.title = L.T("troop_editor_button", "Troop Editor");
					TextObject title = CS$<>8__locals1.title;
					Action<object> onExecute = new Action<object>(CS$<>8__locals1.<Postfix>g__OnExecute|1);
					object identifier = "ret_troop_editor";
					Func<Tuple<bool, TextObject>> getIsDisabledAndReason;
					if ((getIsDisabledAndReason = EscapeMenuPatch.<>O.<0>__notDisabled) == null)
					{
						getIsDisabledAndReason = (EscapeMenuPatch.<>O.<0>__notDisabled = new Func<Tuple<bool, TextObject>>(EscapeMenuPatch.<Postfix>g__notDisabled|0_0));
					}
					EscapeMenuItemVM item = new EscapeMenuItemVM(title, onExecute, identifier, getIsDisabledAndReason, false);
					int index = Math.Min(3, CS$<>8__locals1.__instance.MenuItems.Count);
					CS$<>8__locals1.__instance.MenuItems.Insert(index, item);
				}
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("Failed to inject Troop Editor into EscapeMenuVM: {0}", arg));
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000E514 File Offset: 0x0000C714
		[CompilerGenerated]
		internal static Tuple<bool, TextObject> <Postfix>g__notDisabled|0_0()
		{
			return new Tuple<bool, TextObject>(false, new TextObject(string.Empty, null));
		}

		// Token: 0x02000137 RID: 311
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400038B RID: 907
			public static Func<Tuple<bool, TextObject>> <0>__notDisabled;
		}
	}
}
