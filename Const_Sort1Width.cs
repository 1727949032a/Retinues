using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

// Token: 0x02000008 RID: 8
[PrefabExtension("ClanScreen", "descendant::Constants")]
internal sealed class Const_Sort1Width : PrefabExtensionInsertPatch
{
	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600000E RID: 14 RVA: 0x000020CF File Offset: 0x000002CF
	[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
	public string FileName
	{
		get
		{
			return "ClanScreen_Const_Sort1Width";
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600000F RID: 15 RVA: 0x000020D6 File Offset: 0x000002D6
	public override InsertType Type
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000010 RID: 16 RVA: 0x000020D9 File Offset: 0x000002D9
	public override int Index
	{
		get
		{
			return 1001;
		}
	}
}
