using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x0200000C RID: 12
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_ExpandIndicatorWidth : PrefabExtensionInsertPatch
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x0600001E RID: 30 RVA: 0x00002133 File Offset: 0x00000333
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_ExpandIndicatorWidth";
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600001F RID: 31 RVA: 0x0000213A File Offset: 0x0000033A
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000020 RID: 32 RVA: 0x0000213D File Offset: 0x0000033D
	public override int Index
	{
		get
		{
			return 1005;
		}
	}
}
