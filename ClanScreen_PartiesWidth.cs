using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000013 RID: 19
[PrefabExtension("ClanScreen", "descendant::ButtonWidget[@CommandParameter.Click='1']")]
internal class ClanScreen_PartiesWidth : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000030 RID: 48 RVA: 0x00002219 File Offset: 0x00000419
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
