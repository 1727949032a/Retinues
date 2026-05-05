using System;

namespace Retinues.GUI.Editor
{
	// Token: 0x0200006C RID: 108
	public abstract class BaseListElementVM : BaseButtonVM
	{
		// Token: 0x0600020B RID: 523 RVA: 0x0000EB5D File Offset: 0x0000CD5D
		public void ApplyFilter(string filter)
		{
			if (string.IsNullOrWhiteSpace(filter))
			{
				base.IsVisible = true;
				return;
			}
			base.IsVisible = this.FilterMatch(filter);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000EB7C File Offset: 0x0000CD7C
		protected BaseListElementVM(bool autoRegister = true) : base(autoRegister)
		{
		}

		// Token: 0x0600020D RID: 525
		public abstract bool FilterMatch(string filter);
	}
}
