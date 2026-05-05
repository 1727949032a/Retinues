using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000017 RID: 23
[PrefabExtension("ClanScreen", "descendant::Widget[@Id='FinancePanelWidget']")]
internal class ClanScreen_FinancePanel_Visible : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000038 RID: 56 RVA: 0x000022AD File Offset: 0x000004AD
	public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
	{
		get
		{
			return new List<PrefabExtensionSetAttributePatch.Attribute>(1)
			{
				new PrefabExtensionSetAttributePatch.Attribute("IsVisible", "@IsFinancePanelVisible")
			};
		}
	}
}
