using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Retinues.GUI.Encyclopedia.Patches
{
	// Token: 0x0200006A RID: 106
	[PrefabExtension("EncyclopediaUnitPage", "descendant::Widget[./Children/ButtonWidget[@Id='BookmarkButton']]/Children")]
	internal sealed class EncyclopediaUnitPage_EditorButtonPatch : PrefabExtensionInsertPatch
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000EB3E File Offset: 0x0000CD3E
		[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
		public string FileName
		{
			get
			{
				return "Encyclopedia_EditorButton";
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000205 RID: 517 RVA: 0x0000EB45 File Offset: 0x0000CD45
		public override InsertType Type
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000EB48 File Offset: 0x0000CD48
		public override int Index
		{
			get
			{
				return 99;
			}
		}
	}
}
