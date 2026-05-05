using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000012 RID: 18
[PrefabExtension("ClanScreen", "descendant::ButtonWidget[@CommandParameter.Click='0']")]
internal class ClanScreen_MembersWidth : PrefabExtensionSetAttributePatch
{
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600002E RID: 46 RVA: 0x000021F4 File Offset: 0x000003F4
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
