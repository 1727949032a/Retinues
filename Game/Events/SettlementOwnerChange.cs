using System;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.Actions;

namespace Retinues.Game.Events
{
	// Token: 0x020000AB RID: 171
	[SafeClass]
	public class SettlementOwnerChange
	{
		// Token: 0x06000743 RID: 1859 RVA: 0x00024E0F File Offset: 0x0002300F
		public SettlementOwnerChange(WSettlement settlement, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail, WHero oldOwner, WHero newOwner)
		{
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x00024E3E File Offset: 0x0002303E
		public bool WasCaptured
		{
			get
			{
				return this._detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000745 RID: 1861 RVA: 0x00024E49 File Offset: 0x00023049
		public bool WasBartered
		{
			get
			{
				return this._detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x00024E54 File Offset: 0x00023054
		public bool WasGranted
		{
			get
			{
				return this._detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByKingDecision;
			}
		}

		// Token: 0x040001DA RID: 474
		private readonly ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail _detail = detail;

		// Token: 0x040001DB RID: 475
		public WSettlement Settlement = settlement;

		// Token: 0x040001DC RID: 476
		public WHero OldOwner = oldOwner ?? null;

		// Token: 0x040001DD RID: 477
		public WHero NewOwner = newOwner ?? null;
	}
}
