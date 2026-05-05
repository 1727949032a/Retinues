using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Retinues.Configuration;

// Token: 0x02000018 RID: 24
[PrefabExtension("EscapeMenu", "descendant::NavigatableListPanel[@Id='ButtonsContainer']/ItemTemplate/Widget")]
internal class EscapeMenu_ItemTemplate_Margins : PrefabExtensionSetAttributePatch
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600003A RID: 58 RVA: 0x000022D4 File Offset: 0x000004D4
	public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
	{
		get
		{
			Option<bool> enableGlobalEditor = Config.EnableGlobalEditor;
			if (enableGlobalEditor == null || !enableGlobalEditor)
			{
				return new List<PrefabExtensionSetAttributePatch.Attribute>();
			}
			return new List<PrefabExtensionSetAttributePatch.Attribute>(1)
			{
				new PrefabExtensionSetAttributePatch.Attribute("MarginBottom", "22")
			};
		}
	}
}
