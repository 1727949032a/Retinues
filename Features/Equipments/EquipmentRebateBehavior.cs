using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Features.Equipments
{
	// Token: 0x020000C8 RID: 200
	[SafeClass]
	public sealed class EquipmentRebateBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x00029724 File Offset: 0x00027924
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x0002972B File Offset: 0x0002792B
		public static EquipmentRebateBehavior Instance { get; private set; }

		// Token: 0x06000834 RID: 2100 RVA: 0x00029733 File Offset: 0x00027933
		public EquipmentRebateBehavior()
		{
			EquipmentRebateBehavior.Instance = this;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0002974C File Offset: 0x0002794C
		public override void RegisterEvents()
		{
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0002974E File Offset: 0x0002794E
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, int>>("Retinues_EquipmentRebates", ref this._itemPurchaseCounts);
			if (dataStore.IsLoading && this._itemPurchaseCounts == null)
			{
				this._itemPurchaseCounts = new Dictionary<string, int>();
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00029780 File Offset: 0x00027980
		public static float GetRebateMultiplier(WItem item)
		{
			if (EquipmentRebateBehavior.Instance == null || item == null)
			{
				return 1f;
			}
			float num = Config.EquipmentCostReductionPerPurchase;
			if (num <= 0f)
			{
				return 1f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			int num2;
			if (!EquipmentRebateBehavior.Instance._itemPurchaseCounts.TryGetValue(item.StringId, out num2) || num2 <= 0)
			{
				return 1f;
			}
			float num3 = 1f - num;
			if (num3 <= 0f)
			{
				return 0f;
			}
			double num4 = Math.Pow((double)num3, (double)num2);
			if (num4 < 0.0)
			{
				num4 = 0.0;
			}
			return (float)num4;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00029825 File Offset: 0x00027A25
		public static bool HasRebate(WItem item)
		{
			return Config.EquipmentCostReductionPerPurchase > 0f && EquipmentRebateBehavior.Instance != null && !(item == null) && EquipmentRebateBehavior.Instance._itemPurchaseCounts.ContainsKey(item.StringId);
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00029864 File Offset: 0x00027A64
		public static void RegisterPurchase(WItem item, int copies)
		{
			if (EquipmentRebateBehavior.Instance == null || item == null)
			{
				return;
			}
			if (copies <= 0)
			{
				return;
			}
			if (Config.EquipmentCostReductionPerPurchase <= 0f)
			{
				return;
			}
			int num;
			if (!EquipmentRebateBehavior.Instance._itemPurchaseCounts.TryGetValue(item.StringId, out num))
			{
				num = 0;
			}
			long num2 = (long)num + (long)copies;
			if (num2 > 2147483647L)
			{
				num2 = 2147483647L;
			}
			EquipmentRebateBehavior.Instance._itemPurchaseCounts[item.StringId] = (int)num2;
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x000298E1 File Offset: 0x00027AE1
		public static void ClearItem(WItem item)
		{
			if (EquipmentRebateBehavior.Instance == null || item == null)
			{
				return;
			}
			EquipmentRebateBehavior.Instance._itemPurchaseCounts.Remove(item.StringId);
		}

		// Token: 0x04000223 RID: 547
		private Dictionary<string, int> _itemPurchaseCounts = new Dictionary<string, int>();
	}
}
