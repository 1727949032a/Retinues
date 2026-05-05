using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000015 RID: 21
[PrefabExtension("ClanScreen", "descendant::ButtonWidget[@CommandParameter.Click='3']")]
internal class ClanScreen_IncomeWidth : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000034 RID: 52 RVA: 0x00002263 File Offset: 0x00000463
	public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
	{
		get
		{
			return new List<PrefabExtensionSetAttributePatch.Attribute>(1)
			{
				new PrefabExtensionSetAttributePatch.Attribute("SuggestedWidth", "240")
			};
		}
	}
}
