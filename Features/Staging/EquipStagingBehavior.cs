using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Game.Menu;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Features.Staging
{
	// Token: 0x020000C1 RID: 193
	[SafeClass]
	public class EquipStagingBehavior : BaseStagingBehavior<PendingEquipData>
	{
		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x00027B17 File Offset: 0x00025D17
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x00027B1F File Offset: 0x00025D1F
		protected override string SaveFieldName { get; set; } = "Retinues_Equip_Pending";

		// Token: 0x060007D9 RID: 2009 RVA: 0x00027B28 File Offset: 0x00025D28
		protected override PendingEquipData GetStagedChange(WCharacter troop, string objectKey)
		{
			if (troop == null || string.IsNullOrEmpty(objectKey))
			{
				return null;
			}
			return base.GetPending(troop.StringId, objectKey);
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00027B4A File Offset: 0x00025D4A
		protected override List<PendingEquipData> GetStagedChanges(WCharacter troop)
		{
			if (troop == null)
			{
				return new List<PendingEquipData>();
			}
			return base.GetPending(troop.StringId);
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x00027B68 File Offset: 0x00025D68
		protected override void StageChange(WCharacter troop, object payload)
		{
			if (troop == null)
			{
				return;
			}
			if (payload is EquipStagingBehavior.EquipChange)
			{
				EquipStagingBehavior.EquipChange equipChange = (EquipStagingBehavior.EquipChange)payload;
				WItem item = equipChange.Item;
				int remaining = EquipStagingBehavior.HoursFromGold((item == null) ? 100 : EquipmentManager.GetItemCost(item));
				base.SetPending(troop.StringId, EquipStagingBehavior.ComposeKey((int)equipChange.Slot, equipChange.EquipmentIndex), new PendingEquipData
				{
					TroopId = troop.StringId,
					Remaining = remaining,
					ItemId = ((item != null) ? item.StringId : null),
					Slot = equipChange.Slot,
					Carry = 0f,
					EquipmentIndex = equipChange.EquipmentIndex
				});
				string text;
				if (BaseStagingBehavior<PendingEquipData>.IsInManagedMenu(out text))
				{
					BaseStagingBehavior<PendingEquipData>.RefreshManagedMenuOrDefault();
				}
				return;
			}
			Log.Warn("TroopEquipBehavior.StageChange called with invalid payload.");
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x00027C33 File Offset: 0x00025E33
		protected override void UnstageChange(WCharacter troop, string objectKey)
		{
			if (troop == null || string.IsNullOrEmpty(objectKey))
			{
				return;
			}
			base.RemovePending(troop.StringId, objectKey);
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x00027C54 File Offset: 0x00025E54
		protected override void UnstageChanges(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			foreach (WEquipment wequipment in troop.Loadout.Equipments)
			{
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					base.RemovePending(troop.StringId, EquipStagingBehavior.ComposeKey((int)slot, wequipment.Index));
				}
			}
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x00027D04 File Offset: 0x00025F04
		public static PendingEquipData Get(WCharacter troop, EquipmentIndex slot, int equipmentIndex = 0)
		{
			return ((EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance).GetStagedChange(troop, EquipStagingBehavior.ComposeKey((int)slot, equipmentIndex));
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00027D1D File Offset: 0x00025F1D
		public static List<PendingEquipData> Get(WCharacter troop)
		{
			return ((EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance).GetStagedChanges(troop);
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x00027D2F File Offset: 0x00025F2F
		public static void Stage(WCharacter troop, EquipmentIndex slot, WItem item, int equipmentIndex = 0)
		{
			((EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance).StageChange(troop, new EquipStagingBehavior.EquipChange(slot, equipmentIndex, item));
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00027D4E File Offset: 0x00025F4E
		public static void Unstage(WCharacter troop, EquipmentIndex slot, int equipmentIndex = 0)
		{
			((EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance).UnstageChange(troop, EquipStagingBehavior.ComposeKey((int)slot, equipmentIndex));
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00027D68 File Offset: 0x00025F68
		public static void Unstage(WCharacter troop, int equipmentIndex = 0)
		{
			EquipStagingBehavior equipStagingBehavior = (EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance;
			foreach (EquipmentIndex slot in WEquipment.Slots)
			{
				equipStagingBehavior.UnstageChange(troop, EquipStagingBehavior.ComposeKey((int)slot, equipmentIndex));
			}
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00027DCC File Offset: 0x00025FCC
		public static void Unstage(WCharacter troop)
		{
			((EquipStagingBehavior)BaseStagingBehavior<PendingEquipData>.Instance).UnstageChanges(troop);
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x00027DDE File Offset: 0x00025FDE
		protected override string OptionId
		{
			get
			{
				return "ret_equip_pending";
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x00027DE5 File Offset: 0x00025FE5
		protected override string OptionText
		{
			get
			{
				return L.S("upgrade_equip_pending_btn", "Equip troops");
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x00027DF6 File Offset: 0x00025FF6
		protected override string InquiryTitle
		{
			get
			{
				return L.S("upgrade_equip_select_troop", "Select troops to equip");
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00027E07 File Offset: 0x00026007
		protected override string InquiryDescription
		{
			get
			{
				return L.S("upgrade_equip_choose_troop", "Choose one or more troops to start equipping now.");
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x00027E18 File Offset: 0x00026018
		protected override string InquiryAffirmative
		{
			get
			{
				return L.S("upgrade_equip_begin", "Begin upgrading equipment");
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00027E29 File Offset: 0x00026029
		protected override string InquiryNegative
		{
			get
			{
				return L.S("cancel", "Cancel");
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x00027E3A File Offset: 0x0002603A
		protected override string ActionString
		{
			get
			{
				return L.S("action_equip", "equip");
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x00027E4B File Offset: 0x0002604B
		protected override GameMenuOption.LeaveType LeaveType
		{
			get
			{
				return GameMenuOption.LeaveType.Craft;
			}
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00027E50 File Offset: 0x00026050
		protected override string BuildElementTitle(WCharacter troop, PendingEquipData data)
		{
			WItem witem = (data.ItemId != null) ? new WItem(data.ItemId) : null;
			string arg = (witem != null) ? witem.Name : L.S("upgrade_equip_unequip", "Unequip");
			return string.Format("{0}\n{1} ({2}h)", troop.Name, arg, data.Remaining);
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00027EB4 File Offset: 0x000260B4
		protected override void FinalModalSummary()
		{
			if (this._batchedActions.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<WCharacter, List<WItem>> keyValuePair in this._batchedActions)
			{
				WCharacter key = keyValuePair.Key;
				List<WItem> value = keyValuePair.Value;
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (WItem witem in value)
				{
					string text = (witem != null) ? witem.Name.ToString() : L.S("upgrade_equip_unequip", "Unequip").ToString();
					if (!dictionary.ContainsKey(text))
					{
						dictionary[text] = 0;
					}
					Dictionary<string, int> dictionary2 = dictionary;
					string key2 = text;
					dictionary2[key2]++;
				}
				List<string> list2 = new List<string>();
				foreach (KeyValuePair<string, int> keyValuePair2 in dictionary)
				{
					if (keyValuePair2.Value > 1)
					{
						list2.Add(string.Format("{0} x {1}", keyValuePair2.Value, keyValuePair2.Key));
					}
					else
					{
						list2.Add(keyValuePair2.Key);
					}
				}
				string item = key.Name + ": " + string.Join(", ", list2);
				list.Add(item);
			}
			TextObject description = L.T("equip_complete_summary", "The following troops have equipped new items:\n\n{SUMMARY}").SetTextVariable("SUMMARY", string.Join("\n", list));
			Notifications.Popup(L.T("equip_complete", "Equipment Updated"), description, null, true);
			this._batchedActions.Clear();
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x000280D8 File Offset: 0x000262D8
		protected override void StartWait(CampaignGameStarter starter, string troopId, string objId, PendingEquipData data, Action onAfterCompleted = null)
		{
			WCharacter wcharacter = new WCharacter(troopId);
			if (data.ItemId != null)
			{
				new WItem(data.ItemId);
			}
			TimedWaitMenu.Start(starter, "equip_" + objId, L.T("upgrade_equip_progress", "Equipping {NAME}...").SetTextVariable("NAME", wcharacter.Name).ToString(), (float)data.Remaining, delegate
			{
				WCharacter wcharacter2 = new WCharacter(troopId);
				WItem witem = (data.ItemId != null) ? new WItem(data.ItemId) : null;
				EquipmentManager.ApplyImmediate(wcharacter2, data.EquipmentIndex, data.Slot, witem);
				this.RemovePending(troopId, objId);
				TextObject textObject = L.T("equip_complete_text", "{TROOP} has equipped {ITEM}.").SetTextVariable("TROOP", wcharacter2.Name).SetTextVariable("ITEM", (witem != null) ? witem.Name : L.S("upgrade_equip_unequip", "Unequip"));
				if (!this._batchActive)
				{
					Notifications.Popup(L.T("equip_complete", "Equipment Updated"), textObject, null, true);
				}
				else
				{
					if (!this._batchedActions.ContainsKey(wcharacter2))
					{
						this._batchedActions[wcharacter2] = new List<WItem>();
					}
					this._batchedActions[wcharacter2].Add(witem);
					Log.Message(textObject.ToString());
				}
				Action onAfterCompleted2 = onAfterCompleted;
				if (onAfterCompleted2 == null)
				{
					return;
				}
				onAfterCompleted2();
			}, delegate
			{
				Action onAfterCompleted2 = onAfterCompleted;
				if (onAfterCompleted2 == null)
				{
					return;
				}
				onAfterCompleted2();
			}, GameMenu.MenuOverlayType.SettlementWithBoth, delegate(float _)
			{
				if (data.Remaining > 0)
				{
					data.Remaining--;
				}
			});
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x000281A5 File Offset: 0x000263A5
		private static string ComposeKey(int slot, int equipmentIndex)
		{
			return string.Format("{0}:{1}", slot, equipmentIndex);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x000281C0 File Offset: 0x000263C0
		private static int HoursFromGold(int gold)
		{
			double num = (double)Math.Max(1, gold);
			double num2 = Math.Log10(num);
			double num3 = (num <= 1000.0) ? (10.0 * num2 + -18.0) : ((num <= 5000.0) ? (17.16811869688072 * num2 + -39.504356090642155) : (48.0 * num2 + -153.5505602081289));
			num3 *= (double)Config.EquipmentTimeMultiplier;
			num3 /= 5.0;
			return Math.Max(1, (int)Math.Ceiling(num3));
		}

		// Token: 0x0400020E RID: 526
		private readonly Dictionary<WCharacter, List<WItem>> _batchedActions = new Dictionary<WCharacter, List<WItem>>();

		// Token: 0x02000190 RID: 400
		public readonly struct EquipChange
		{
			// Token: 0x06000C4A RID: 3146 RVA: 0x00035361 File Offset: 0x00033561
			public EquipChange(EquipmentIndex slot, int equipmentIndex, WItem item)
			{
				this.Slot = slot;
				this.EquipmentIndex = equipmentIndex;
				this.Item = item;
			}

			// Token: 0x040004C5 RID: 1221
			public readonly EquipmentIndex Slot;

			// Token: 0x040004C6 RID: 1222
			public readonly int EquipmentIndex;

			// Token: 0x040004C7 RID: 1223
			public readonly WItem Item;
		}
	}
}
