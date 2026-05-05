using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000016 RID: 22
[PrefabExtension("ClanScreen", "descendant::Widget[@Id='TopPanel']")]
internal class ClanScreen_TopPanel_Visible : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000036 RID: 54 RVA: 0x00002288 File Offset: 0x00000488
	public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
	{
		get
		{
			return new List<PrefabExtensionSetAttributePatch.Attribute>(1)
			{
				new PrefabExtensionSetAttributePatch.Attribute("IsVisible", "@IsTopPanelVisible")
			};
		}
	}
}
