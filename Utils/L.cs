using System;
using TaleWorlds.Localization;

namespace Retinues.Utils
{
	// Token: 0x02000033 RID: 51
	public static class L
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000645B File Offset: 0x0000465B
		public static TextObject T(string id, string fallback)
		{
			return new TextObject("{=ret_" + id + "}" + fallback, null);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00006474 File Offset: 0x00004674
		public static string S(string id, string fallback)
		{
			return L.T(id, fallback).ToString();
		}
	}
}
