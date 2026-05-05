using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200001A RID: 26
[PrefabExtension("ClanScreen", "descendant::Widget[./Children/ClanMembers and ./Children/ClanParties and ./Children/ClanFiefs and ./Children/ClanIncome]/Children")]
internal class ClanScreen_TroopsPanel : PrefabExtensionInsertPatch
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000040 RID: 64 RVA: 0x00002333 File Offset: 0x00000533
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_TroopsPanel";
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000041 RID: 65 RVA: 0x0000233A File Offset: 0x0000053A
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000042 RID: 66 RVA: 0x0000233D File Offset: 0x0000053D
	public override int Index
	{
		get
		{
			return 4;
		}
	}
}
