using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200000D RID: 13
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_ExpandIndicatorHeight : PrefabExtensionInsertPatch
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000022 RID: 34 RVA: 0x0000214C File Offset: 0x0000034C
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_ExpandIndicatorHeight";
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000023 RID: 35 RVA: 0x00002153 File Offset: 0x00000353
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000024 RID: 36 RVA: 0x00002156 File Offset: 0x00000356
	public override int Index
	{
		get
		{
			return 1006;
		}
	}
}
