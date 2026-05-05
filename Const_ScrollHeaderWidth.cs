using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200000A RID: 10
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_ScrollHeaderWidth : PrefabExtensionInsertPatch
{
	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000016 RID: 22 RVA: 0x00002101 File Offset: 0x00000301
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_ScrollHeaderWidth";
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000017 RID: 23 RVA: 0x00002108 File Offset: 0x00000308
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000018 RID: 24 RVA: 0x0000210B File Offset: 0x0000030B
	public override int Index
	{
		get
		{
			return 1003;
		}
	}
}
