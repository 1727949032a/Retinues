using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000014 RID: 20
[PrefabExtension("ClanScreen", "descendant::ButtonWidget[@CommandParameter.Click='2']")]
internal class ClanScreen_FiefsWidth : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000032 RID: 50 RVA: 0x0000223E File Offset: 0x0000043E
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
