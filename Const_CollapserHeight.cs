using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000007 RID: 7
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_CollapserHeight : PrefabExtensionInsertPatch
{
	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600000A RID: 10 RVA: 0x000020B6 File Offset: 0x000002B6
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_CollapserHeight";
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600000B RID: 11 RVA: 0x000020BD File Offset: 0x000002BD
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600000C RID: 12 RVA: 0x000020C0 File Offset: 0x000002C0
	public override int Index
	{
		get
		{
			return 1000;
		}
	}
}
