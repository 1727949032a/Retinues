using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Troop.Panel
{
	// Token: 0x0200007B RID: 123
	[SafeClass]
	public sealed class TroopConversionRowVM : BaseVM
	{
		// Token: 0x060002F7 RID: 759 RVA: 0x00011B7F File Offset: 0x0000FD7F
		public TroopConversionRowVM(WCharacter source)
		{
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x00011B90 File Offset: 0x0000FD90
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Conversion] = new string[]
				{
					"PendingAmount",
					"GoldConversionCost",
					"InfluenceConversionCost",
					"SourceDisplay",
					"TargetDisplay",
					"CanRecruit",
					"CanRelease",
					"HasPendingConversions"
				};
				dictionary[UIEvent.Party] = new string[]
				{
					"SourceDisplay",
					"TargetDisplay",
					"CanRecruit",
					"CanRelease"
				};
				return dictionary;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x00011C1E File Offset: 0x0000FE1E
		private bool OtherRowsHavePendingConversions
		{
			get
			{
				return State.ConversionData.Any((KeyValuePair<WCharacter, int> kvp) => kvp.Key != this.Source && kvp.Value != 0);
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060002FA RID: 762 RVA: 0x00011C38 File Offset: 0x0000FE38
		private int Amount
		{
			get
			{
				int result;
				if (!State.ConversionData.TryGetValue(this.Source, out result))
				{
					return 0;
				}
				return result;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060002FB RID: 763 RVA: 0x00011C5C File Offset: 0x0000FE5C
		private int TotalAmount
		{
			get
			{
				return State.ConversionData.Values.Sum();
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002FC RID: 764 RVA: 0x00011C70 File Offset: 0x0000FE70
		private int TargetCount
		{
			get
			{
				int result;
				if (!State.PartyData.TryGetValue(State.Troop, out result))
				{
					return 0;
				}
				return result;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002FD RID: 765 RVA: 0x00011C94 File Offset: 0x0000FE94
		private int SourceCount
		{
			get
			{
				int result;
				if (!State.PartyData.TryGetValue(this.Source, out result))
				{
					return 0;
				}
				return result;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002FE RID: 766 RVA: 0x00011CB8 File Offset: 0x0000FEB8
		[DataSourceProperty]
		public int PendingAmount
		{
			get
			{
				return this.Amount;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002FF RID: 767 RVA: 0x00011CC0 File Offset: 0x0000FEC0
		[DataSourceProperty]
		public int GoldConversionCost
		{
			get
			{
				return RetinueManager.ConversionGoldCostPerUnit(State.Troop) * Math.Max(0, this.Amount);
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00011CD9 File Offset: 0x0000FED9
		[DataSourceProperty]
		public int InfluenceConversionCost
		{
			get
			{
				return RetinueManager.ConversionInfluenceCostPerUnit(State.Troop) * Math.Max(0, this.Amount);
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000301 RID: 769 RVA: 0x00011CF2 File Offset: 0x0000FEF2
		[DataSourceProperty]
		public string ButtonApplyConversionsText
		{
			get
			{
				return L.S("ret_apply_conversions_button_text", "Convert");
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000302 RID: 770 RVA: 0x00011D03 File Offset: 0x0000FF03
		[DataSourceProperty]
		public string ButtonClearConversionsText
		{
			get
			{
				return L.S("ret_clear_conversions_button_text", "Clear");
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000303 RID: 771 RVA: 0x00011D14 File Offset: 0x0000FF14
		[DataSourceProperty]
		public string SourceDisplay
		{
			get
			{
				return string.Format("{0} ({1})", Format.Crop(this.Source.Name, 40), this.SourceCount - this.Amount);
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000304 RID: 772 RVA: 0x00011D44 File Offset: 0x0000FF44
		[DataSourceProperty]
		public string TargetDisplay
		{
			get
			{
				return string.Format("{0} ({1}/{2})", Format.Crop(State.Troop.Name, 40), this.TargetCount + this.TotalAmount, RetinueManager.RetinueCapFor(State.Troop));
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000305 RID: 773 RVA: 0x00011D82 File Offset: 0x0000FF82
		[DataSourceProperty]
		public bool CanRecruit
		{
			get
			{
				return !this.OtherRowsHavePendingConversions && this.TargetCount + this.TotalAmount < RetinueManager.RetinueCapFor(State.Troop) && this.SourceCount - this.Amount > 0;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000306 RID: 774 RVA: 0x00011DB7 File Offset: 0x0000FFB7
		[DataSourceProperty]
		public bool CanRelease
		{
			get
			{
				return !this.OtherRowsHavePendingConversions && this.TargetCount + this.TotalAmount > 0;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000307 RID: 775 RVA: 0x00011DD3 File Offset: 0x0000FFD3
		[DataSourceProperty]
		public bool HasPendingConversions
		{
			get
			{
				return this.Amount != 0;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00011DDE File Offset: 0x0000FFDE
		[DataSourceProperty]
		public bool ShowGoldCost
		{
			get
			{
				return Config.GoldConversionCostPerTier > 0;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000309 RID: 777 RVA: 0x00011DED File Offset: 0x0000FFED
		[DataSourceProperty]
		public bool ShowInfluenceCost
		{
			get
			{
				return Config.InfluenceConversionCostPerTier > 0;
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00011DFC File Offset: 0x0000FFFC
		[DataSourceMethod]
		public void ExecuteRecruit()
		{
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_convert", "convert")))
			{
				return;
			}
			int num = 0;
			while (num < BaseVM.BatchInput(true) && this.CanRecruit)
			{
				Dictionary<WCharacter, int> conversionData = State.ConversionData;
				WCharacter source = this.Source;
				conversionData[source]++;
				num++;
			}
			State.UpdateConversionData(State.ConversionData);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00011E68 File Offset: 0x00010068
		[DataSourceMethod]
		public void ExecuteRelease()
		{
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_convert", "convert")))
			{
				return;
			}
			int num = 0;
			while (num < BaseVM.BatchInput(true) && this.CanRelease)
			{
				Dictionary<WCharacter, int> conversionData = State.ConversionData;
				WCharacter source = this.Source;
				conversionData[source]--;
				num++;
			}
			State.UpdateConversionData(State.ConversionData);
		}

		// Token: 0x040000E7 RID: 231
		private readonly WCharacter Source = source;
	}
}
