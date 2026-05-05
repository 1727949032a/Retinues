using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Doctrines;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Doctrines
{
	// Token: 0x02000087 RID: 135
	[SafeClass]
	public sealed class DoctrineColumnVM : BaseVM
	{
		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0001A22C File Offset: 0x0001842C
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001A234 File Offset: 0x00018434
		public DoctrineColumnVM(string name, IEnumerable<DoctrineVM> doctrines)
		{
			this._name = name;
			this.Doctrines = new MBBindingList<DoctrineVM>();
			if (doctrines != null)
			{
				foreach (DoctrineVM doctrineVM in doctrines)
				{
					doctrineVM.Column = this;
					this.Doctrines.Add(doctrineVM);
				}
			}
			this.Refresh();
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001A2AC File Offset: 0x000184AC
		public static MBBindingList<DoctrineColumnVM> CreateColumns()
		{
			Campaign campaign = Campaign.Current;
			DoctrineServiceBehavior doctrineServiceBehavior = (campaign != null) ? campaign.GetCampaignBehavior<DoctrineServiceBehavior>() : null;
			MBBindingList<DoctrineColumnVM> mbbindingList = new MBBindingList<DoctrineColumnVM>();
			List<string> list = new List<string>(5)
			{
				L.S("doctrines_col_0", "Spoils"),
				L.S("doctrines_col_1", "Armory"),
				L.S("doctrines_col_2", "Troops"),
				L.S("doctrines_col_3", "Training"),
				L.S("doctrines_col_4", "Retinues")
			};
			if (doctrineServiceBehavior != null)
			{
				foreach (IGrouping<int, DoctrineDefinition> grouping in from d in doctrineServiceBehavior.AllDoctrines()
				group d by d.Column into g
				orderby g.Key
				select g)
				{
					string name = (grouping.Key >= 0 && grouping.Key < list.Count) ? list[grouping.Key] : string.Empty;
					List<DoctrineVM> doctrines = grouping.OrderBy((DoctrineDefinition d) => d.Row).Select((DoctrineDefinition d) => new DoctrineVM(d.Key)).ToList<DoctrineVM>();
					mbbindingList.Add(new DoctrineColumnVM(name, doctrines));
				}
			}
			return mbbindingList;
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0001A460 File Offset: 0x00018660
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x0001A468 File Offset: 0x00018668
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name == value)
				{
					return;
				}
				this._name = value;
				base.OnPropertyChanged("Name");
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x0001A48B File Offset: 0x0001868B
		[DataSourceProperty]
		public MBBindingList<DoctrineVM> Doctrines { get; }

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001A494 File Offset: 0x00018694
		public void Refresh()
		{
			foreach (DoctrineVM doctrineVM in this.Doctrines)
			{
				doctrineVM.Refresh();
			}
			base.OnPropertyChanged("Name");
			base.OnPropertyChanged("Doctrines");
		}

		// Token: 0x04000134 RID: 308
		private string _name;
	}
}
