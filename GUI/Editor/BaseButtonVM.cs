using System;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor
{
	// Token: 0x0200006B RID: 107
	public abstract class BaseButtonVM : BaseVM
	{
		// Token: 0x06000208 RID: 520 RVA: 0x0000EB54 File Offset: 0x0000CD54
		protected BaseButtonVM(bool autoRegister = true) : base(autoRegister)
		{
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000209 RID: 521
		[DataSourceProperty]
		public abstract bool IsEnabled { get; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600020A RID: 522
		[DataSourceProperty]
		public abstract bool IsSelected { get; }
	}
}
