using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Doctrines
{
	// Token: 0x02000088 RID: 136
	[SafeClass]
	public sealed class DoctrineDescriptionLineVM : BaseVM
	{
		// Token: 0x060004C5 RID: 1221 RVA: 0x0001A4F4 File Offset: 0x000186F4
		public DoctrineDescriptionLineVM(string text)
		{
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001A503 File Offset: 0x00018703
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x0001A50A File Offset: 0x0001870A
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
		}

		// Token: 0x04000136 RID: 310
		private string _text = text;
	}
}
