using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200000B RID: 11
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_ScrollHeaderHeight : PrefabExtensionInsertPatch
{
	// Token: 0x17000010 RID: 16
	// (get) Token: 0x0600001A RID: 26 RVA: 0x0000211A File Offset: 0x0000031A
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_ScrollHeaderHeight";
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x0600001B RID: 27 RVA: 0x00002121 File Offset: 0x00000321
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x0600001C RID: 28 RVA: 0x00002124 File Offset: 0x00000324
	public override int Index
	{
		get
		{
			return 1004;
		}
	}
}
