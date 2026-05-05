using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000009 RID: 9
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_Sort1Height : PrefabExtensionInsertPatch
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000012 RID: 18 RVA: 0x000020E8 File Offset: 0x000002E8
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_Sort1Height";
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000013 RID: 19 RVA: 0x000020EF File Offset: 0x000002EF
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000014 RID: 20 RVA: 0x000020F2 File Offset: 0x000002F2
	public override int Index
	{
		get
		{
			return 1002;
		}
	}
}
