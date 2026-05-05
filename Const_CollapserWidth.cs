using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000006 RID: 6
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_CollapserWidth : PrefabExtensionInsertPatch
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000006 RID: 6 RVA: 0x0000209D File Offset: 0x0000029D
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_CollapserWidth";
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000007 RID: 7 RVA: 0x000020A4 File Offset: 0x000002A4
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000008 RID: 8 RVA: 0x000020A7 File Offset: 0x000002A7
	public override int Index
	{
		get
		{
			return 999;
		}
	}
}
