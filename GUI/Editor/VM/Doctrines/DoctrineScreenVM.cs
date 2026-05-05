using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Doctrines
{
	// Token: 0x02000089 RID: 137
	[SafeClass]
	public sealed class DoctrineScreenVM : BaseVM
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0001A51A File Offset: 0x0001871A
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060004CA RID: 1226 RVA: 0x0001A521 File Offset: 0x00018721
		[DataSourceProperty]
		public MBBindingList<DoctrineColumnVM> Columns
		{
			get
			{
				if (this._columns == null)
				{
					this._columns = DoctrineColumnVM.CreateColumns();
				}
				return this._columns;
			}
		}

		// Token: 0x04000137 RID: 311
		private MBBindingList<DoctrineColumnVM> _columns;
	}
}
