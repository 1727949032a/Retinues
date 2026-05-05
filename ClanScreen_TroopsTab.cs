using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200001B RID: 27
[PrefabExtension("ClanScreen", "descendant::ListPanel[.//TextWidget[@Text='@MembersText'] and .//TextWidget[@Text='@PartiesText']]/Children")]
internal class ClanScreen_TroopsTab : PrefabExtensionInsertPatch
{
	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000044 RID: 68 RVA: 0x00002348 File Offset: 0x00000548
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_TroopsTab";
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000045 RID: 69 RVA: 0x0000234F File Offset: 0x0000054F
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000046 RID: 70 RVA: 0x00002352 File Offset: 0x00000552
	public override int Index
	{
		get
		{
			return 2;
		}
	}
}
