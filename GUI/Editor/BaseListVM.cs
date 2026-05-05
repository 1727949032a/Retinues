using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor
{
	// Token: 0x0200006D RID: 109
	public abstract class BaseListVM : BaseVM
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000EB85 File Offset: 0x0000CD85
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000EB8D File Offset: 0x0000CD8D
		[DataSourceProperty]
		public string FilterText
		{
			get
			{
				return this._filterText;
			}
			set
			{
				if (this._filterText == value)
				{
					return;
				}
				if (value == null)
				{
					value = string.Empty;
				}
				this._filterText = value;
				this.OnFilterTextChanged();
				base.OnPropertyChanged("FilterText");
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000EBC0 File Offset: 0x0000CDC0
		[DataSourceProperty]
		public string FilterLabel
		{
			get
			{
				return L.S("item_search_label", "Filter:");
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000211 RID: 529
		public abstract List<BaseListElementVM> Rows { get; }

		// Token: 0x06000212 RID: 530 RVA: 0x0000EBD4 File Offset: 0x0000CDD4
		protected virtual void OnFilterTextChanged()
		{
			foreach (BaseListElementVM baseListElementVM in this.Rows)
			{
				baseListElementVM.ApplyFilter(this._filterText);
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000EC2C File Offset: 0x0000CE2C
		public virtual void RefreshFilter()
		{
			this.OnFilterTextChanged();
		}

		// Token: 0x040000AD RID: 173
		private string _filterText = string.Empty;
	}
}
