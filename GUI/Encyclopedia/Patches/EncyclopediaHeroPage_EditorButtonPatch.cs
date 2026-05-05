using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Retinues.GUI.Encyclopedia.Patches
{
	// Token: 0x02000069 RID: 105
	[PrefabExtension("EncyclopediaHeroPage", "descendant::Widget[./Children/ButtonWidget[@Id='BookmarkButton']]/Children")]
	internal sealed class EncyclopediaHeroPage_EditorButtonPatch : PrefabExtensionInsertPatch
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000EB28 File Offset: 0x0000CD28
		[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
		public string FileName
		{
			get
			{
				return "Encyclopedia_EditorButton";
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000EB2F File Offset: 0x0000CD2F
		public override InsertType Type
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000EB32 File Offset: 0x0000CD32
		public override int Index
		{
			get
			{
				return 99;
			}
		}
	}
}
