using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000019 RID: 25
[PrefabExtension("ClanScreen", "descendant::ClanScreenWidget/Children")]
internal sealed class ClanScreen_TopPanel_Insert : PrefabExtensionInsertPatch
{
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600003C RID: 60 RVA: 0x0000231E File Offset: 0x0000051E
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_TopPanel";
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600003D RID: 61 RVA: 0x00002325 File Offset: 0x00000525
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600003E RID: 62 RVA: 0x00002328 File Offset: 0x00000528
	public override int Index
	{
		get
		{
			return 3;
		}
	}
}
