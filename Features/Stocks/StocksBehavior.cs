using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Retinues.Features.Stocks
{
	// Token: 0x020000BB RID: 187
	[SafeClass]
	public class StocksBehavior : CampaignBehaviorBase
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x00026A8D File Offset: 0x00024C8D
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x00026A94 File Offset: 0x00024C94
		public static StocksBehavior Instance { get; private set; }

		// Token: 0x06000785 RID: 1925 RVA: 0x00026A9C File Offset: 0x00024C9C
		public StocksBehavior()
		{
			StocksBehavior.Instance = this;
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x00026AAC File Offset: 0x00024CAC
		public Dictionary<string, int> StocksByItemId
		{
			get
			{
				Dictionary<string, int> result;
				if ((result = this._stocksByItemId) == null)
				{
					result = (this._stocksByItemId = new Dictionary<string, int>());
				}
				return result;
			}
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00026AD1 File Offset: 0x00024CD1
		public override void SyncData(IDataStore ds)
		{
			ds.SyncData<Dictionary<string, int>>("Retinues_Stocks", ref this._stocksByItemId);
			Log.Info(string.Format("{0} entries.", this.StocksByItemId.Count));
			Log.Dump(this._stocksByItemId, LogLevel.Debug);
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00026B10 File Offset: 0x00024D10
		public override void RegisterEvents()
		{
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00026B12 File Offset: 0x00024D12
		public static bool HasStock(string itemId)
		{
			return StocksBehavior.Get(itemId) > 0;
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x00026B20 File Offset: 0x00024D20
		public static int Get(string itemId)
		{
			if (StocksBehavior.Instance == null || itemId == null)
			{
				return 0;
			}
			int result;
			if (!StocksBehavior.Instance.StocksByItemId.TryGetValue(itemId, out result))
			{
				return 0;
			}
			return result;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x00026B50 File Offset: 0x00024D50
		public static void Set(string itemId, int count)
		{
			if (StocksBehavior.Instance == null || itemId == null)
			{
				return;
			}
			if (count <= 0)
			{
				StocksBehavior.Instance.StocksByItemId.Remove(itemId);
				return;
			}
			StocksBehavior.Instance.StocksByItemId[itemId] = count;
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x00026B84 File Offset: 0x00024D84
		public static void Add(string itemId, int delta)
		{
			if (StocksBehavior.Instance == null || itemId == null || delta == 0)
			{
				return;
			}
			int num;
			StocksBehavior.Instance.StocksByItemId.TryGetValue(itemId, out num);
			int num2 = num + delta;
			if (num2 <= 0)
			{
				StocksBehavior.Instance.StocksByItemId.Remove(itemId);
				return;
			}
			StocksBehavior.Instance.StocksByItemId[itemId] = num2;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x00026BE0 File Offset: 0x00024DE0
		[CommandLineFunctionality.CommandLineArgumentFunction("set_stock", "retinues")]
		public static string SetStock(List<string> args)
		{
			if (args.Count != 2)
			{
				return "Usage: retinues.set_stock [id] [count]";
			}
			string text = args[0];
			int num;
			if (!int.TryParse(args[1], out num))
			{
				return "Invalid count.";
			}
			StocksBehavior.Set(text, num);
			return string.Format("Set stock for {0} to {1}.", text, num);
		}

		// Token: 0x040001F2 RID: 498
		private Dictionary<string, int> _stocksByItemId;
	}
}
