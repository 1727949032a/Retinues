using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Equipments;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Equipment.List
{
	// Token: 0x02000085 RID: 133
	[SafeClass]
	public sealed class EquipmentRowVM : BaseListElementVM
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x0001894C File Offset: 0x00016B4C
		public EquipmentRowVM(WItem rowItem, bool isAvailable, bool isUnlocked, int progress) : base(false)
		{
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x00018972 File Offset: 0x00016B72
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00018979 File Offset: 0x00016B79
		public void OnSlotChanged()
		{
			this.UpdateComparisonChevrons();
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00018981 File Offset: 0x00016B81
		public void OnSlotChangedSelective()
		{
			base.OnPropertyChanged("IsSelected");
			base.OnPropertyChanged("ShowIsEquipped");
			base.OnPropertyChanged("ShowInStockText");
			base.OnPropertyChanged("ShowCost");
			base.OnPropertyChanged("AvailableFromAnotherSet");
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000189BA File Offset: 0x00016BBA
		public void OnEquipChanged()
		{
			this.UpdateComparisonChevrons();
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x000189C4 File Offset: 0x00016BC4
		public void OnEquipChangedSelective()
		{
			base.OnPropertyChanged("IsSelected");
			base.OnPropertyChanged("Stock");
			base.OnPropertyChanged("InStockText");
			base.OnPropertyChanged("ShowInStockText");
			base.OnPropertyChanged("ShowCost");
			base.OnPropertyChanged("Cost");
			base.OnPropertyChanged("ShowIsEquipped");
			base.OnPropertyChanged("IsDisabledText");
			base.OnPropertyChanged("AvailableFromAnotherSet");
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00018A34 File Offset: 0x00016C34
		public void OnEquipmentChanged()
		{
			this.UpdateComparisonChevrons();
			base.OnPropertyChanged("ShowComparisonIcon");
			base.OnPropertyChanged("PositiveComparisonSprite");
			base.OnPropertyChanged("NegativeComparisonSprite");
			base.OnPropertyChanged("NegativeComparisonSpriteOffset");
			base.OnPropertyChanged("IsSelected");
			base.OnPropertyChanged("IsEnabled");
			base.OnPropertyChanged("ShowIsEquipped");
			base.OnPropertyChanged("IsDisabledText");
			base.OnPropertyChanged("ShowInStockText");
			base.OnPropertyChanged("ShowCost");
			base.OnPropertyChanged("AvailableFromAnotherSet");
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00018AC0 File Offset: 0x00016CC0
		private WItem Item
		{
			get
			{
				return this.StagedItem ?? this.EquippedItem;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00018AD2 File Offset: 0x00016CD2
		private WItem EquippedItem
		{
			get
			{
				WEquipment equipment = State.Equipment;
				if (equipment == null)
				{
					return null;
				}
				return equipment.Get(State.Slot);
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00018AEC File Offset: 0x00016CEC
		private WItem StagedItem
		{
			get
			{
				Dictionary<EquipmentIndex, EquipData> equipData = State.EquipData;
				EquipData equipData2;
				if (equipData == null || !equipData.TryGetValue(State.Slot, out equipData2))
				{
					return null;
				}
				if (equipData2.Equip == null)
				{
					return null;
				}
				return new WItem(equipData2.Equip.ItemId);
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x00018B2F File Offset: 0x00016D2F
		private bool IsEquipped
		{
			get
			{
				return !this.IsEmptyRow && this.EquippedItem == this.RowItem;
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00018B4C File Offset: 0x00016D4C
		private bool IsEmptyRow
		{
			get
			{
				return this.RowItem == null;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00018B5A File Offset: 0x00016D5A
		private bool IsRequirementBlocked
		{
			get
			{
				return !this.IsEmptyRow && !EquipmentManager.MeetsItemSkillRequirements(State.Troop, this.RowItem);
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00018B7C File Offset: 0x00016D7C
		private bool IsTierBlocked
		{
			get
			{
				if (!this.IsEmptyRow && ClanScreen.EditorMode != EditorMode.Heroes && !DoctrineAPI.IsDoctrineUnlocked<Ironclad>())
				{
					int tier = this.RowItem.Tier;
					WCharacter troop = State.Troop;
					return tier - ((troop != null) ? troop.Tier : 0) > Config.AllowedTierDifference;
				}
				return false;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000490 RID: 1168 RVA: 0x00018BCB File Offset: 0x00016DCB
		private bool IsEquipmentTypeBlocked
		{
			get
			{
				if (!this.IsEmptyRow)
				{
					WEquipment equipment = State.Equipment;
					if (equipment != null && equipment.IsCivilian)
					{
						WItem rowItem = this.RowItem;
						return rowItem != null && !rowItem.IsCivilian;
					}
				}
				return false;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00018BFE File Offset: 0x00016DFE
		[DataSourceProperty]
		public int Cost
		{
			get
			{
				if (!(this.RowItem == null))
				{
					return EquipmentManager.GetItemCost(this.RowItem);
				}
				return 0;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000492 RID: 1170 RVA: 0x00018C1B File Offset: 0x00016E1B
		[DataSourceProperty]
		public string CostFontColor
		{
			get
			{
				if (EquipmentRebateBehavior.HasRebate(this.RowItem))
				{
					return "#c5eb89ff";
				}
				return "#F4E1C4FF";
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x00018C35 File Offset: 0x00016E35
		[DataSourceProperty]
		public int Stock
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return 0;
				}
				return rowItem.GetStock();
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x00018C48 File Offset: 0x00016E48
		[DataSourceProperty]
		public string Name
		{
			get
			{
				WItem rowItem = this.RowItem;
				return ((rowItem != null) ? rowItem.Name : null) ?? L.S("empty_item", "Empty");
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x00018C6F File Offset: 0x00016E6F
		[DataSourceProperty]
		public string InStockText
		{
			get
			{
				return L.T("in_stock", "In Stock ({STOCK})").SetTextVariable("STOCK", this.Stock).ToString();
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x00018C98 File Offset: 0x00016E98
		[DataSourceProperty]
		public string IsDisabledText
		{
			get
			{
				if (!this.IsUnlocked)
				{
					return L.T("unlock_progress_text", "Unlocking ({PROGRESS}%)").SetTextVariable("PROGRESS", (int)((float)this.Progress / (float)Config.RequiredKillsPerItem * 100f)).ToString();
				}
				if (!this.IsAvailable)
				{
					if (!(Player.CurrentSettlement == null))
					{
						TextObject textObject = L.T("item_unavailable_text", "Not sold in {SETTLEMENT}");
						string tag = "SETTLEMENT";
						WSettlement currentSettlement = Player.CurrentSettlement;
						return textObject.SetTextVariable(tag, (currentSettlement != null) ? currentSettlement.Name : null).ToString();
					}
					return L.S("item_unavailable_no_settlement", "Not in town");
				}
				else
				{
					if (this.IsRequirementBlocked)
					{
						TextObject textObject2 = L.T("skill_requirement_text", "{SKILL}: {LEVEL}");
						string tag2 = "SKILL";
						WItem rowItem = this.RowItem;
						TextObject variable;
						if (rowItem == null)
						{
							variable = null;
						}
						else
						{
							SkillObject relevantSkill = rowItem.RelevantSkill;
							variable = ((relevantSkill != null) ? relevantSkill.Name : null);
						}
						TextObject textObject3 = textObject2.SetTextVariable(tag2, variable);
						string tag3 = "LEVEL";
						WItem rowItem2 = this.RowItem;
						return textObject3.SetTextVariable(tag3, (rowItem2 != null) ? rowItem2.Difficulty : 0).ToString();
					}
					if (this.IsTierBlocked)
					{
						return L.S("item_tier_blocked_text", "Tier too high");
					}
					if (this.IsEquipmentTypeBlocked)
					{
						return L.S("item_equipment_type_blocked_text", "Not civilian");
					}
					return string.Empty;
				}
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00018DD1 File Offset: 0x00016FD1
		[DataSourceProperty]
		public bool ShowIsEquipped
		{
			get
			{
				return (this.StagedItem != null && this.IsEquipped) || (this.AvailableFromAnotherSet && !this.IsEquipmentTypeBlocked);
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x00018E00 File Offset: 0x00017000
		[DataSourceProperty]
		public bool ShowInStockText
		{
			get
			{
				if (!ClanScreen.IsStudioMode && !PreviewOverlay.IsEnabled && Config.EquippingTroopsCostsGold && this.IsEnabled && !this.IsSelected && !this.IsEquipped && !this.AvailableFromAnotherSet)
				{
					WItem rowItem = this.RowItem;
					return rowItem != null && rowItem.IsStocked;
				}
				return false;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x00018E5C File Offset: 0x0001705C
		[DataSourceProperty]
		public bool ShowCost
		{
			get
			{
				return !ClanScreen.IsStudioMode && !PreviewOverlay.IsEnabled && Config.EquippingTroopsCostsGold && this.IsEnabled && !this.IsSelected && !this.IsEquipped && !this.AvailableFromAnotherSet && !this.ShowInStockText && this.Cost > 0;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x00018EB6 File Offset: 0x000170B6
		[DataSourceProperty]
		public override bool IsSelected
		{
			get
			{
				return this.RowItem == this.Item;
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00018EC9 File Offset: 0x000170C9
		[DataSourceProperty]
		public override bool IsEnabled
		{
			get
			{
				return this.IsEmptyRow || (this.IsUnlocked && this.IsAvailable && !this.IsRequirementBlocked && !this.IsTierBlocked && !this.IsEquipmentTypeBlocked);
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x00018F00 File Offset: 0x00017100
		[DataSourceProperty]
		public bool AvailableFromAnotherSet
		{
			get
			{
				if (this.RowItem == null || State.Troop == null || State.Equipment == null)
				{
					return false;
				}
				HashSet<string> availableFromAnotherSetCache = State.AvailableFromAnotherSetCache;
				return availableFromAnotherSetCache != null && availableFromAnotherSetCache.Count != 0 && availableFromAnotherSetCache.Contains(this.RowItem.StringId);
			}
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00018F58 File Offset: 0x00017158
		private void UpdateComparisonChevrons()
		{
			this.PositiveChevrons = 0;
			this.NegativeChevrons = 0;
			if (!Config.EnableItemComparisonIcons)
			{
				return;
			}
			if (!this.IsEnabled)
			{
				return;
			}
			try
			{
				WItem rowItem = this.RowItem;
				if (rowItem != null)
				{
					rowItem.GetComparisonChevrons(this.Item, out this.PositiveChevrons, out this.NegativeChevrons);
				}
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("EquipmentRowVM.GetChevronCounts failed for RowItem={0}, Current={1}: {2}", this.RowItem, this.Item, arg));
			}
			base.OnPropertyChanged("ShowComparisonIcon");
			base.OnPropertyChanged("PositiveComparisonSprite");
			base.OnPropertyChanged("NegativeComparisonSprite");
			base.OnPropertyChanged("NegativeComparisonSpriteOffset");
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001900C File Offset: 0x0001720C
		[DataSourceProperty]
		public bool ShowComparisonIcon
		{
			get
			{
				return this.PositiveChevrons > 0 || this.NegativeChevrons > 0;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x00019022 File Offset: 0x00017222
		[DataSourceProperty]
		public string PositiveComparisonSprite
		{
			get
			{
				if (this.PositiveChevrons <= 0)
				{
					return string.Empty;
				}
				if (this.PositiveChevrons > 3)
				{
					this.PositiveChevrons = 3;
				}
				return string.Format("General\\TroopTierIcons\\icon_tier_{0}_big", this.PositiveChevrons);
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00019058 File Offset: 0x00017258
		[DataSourceProperty]
		public string NegativeComparisonSprite
		{
			get
			{
				if (this.NegativeChevrons <= 0)
				{
					return string.Empty;
				}
				if (this.NegativeChevrons > 3)
				{
					this.NegativeChevrons = 3;
				}
				return string.Format("General\\TroopTierIcons\\icon_tier_{0}_big", this.NegativeChevrons);
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001908E File Offset: 0x0001728E
		[DataSourceProperty]
		public int NegativeComparisonSpriteOffset
		{
			get
			{
				if (this.NegativeChevrons > 0 && this.PositiveChevrons > 0)
				{
					return 5;
				}
				return 0;
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x000190A5 File Offset: 0x000172A5
		[DataSourceProperty]
		public string ImageId
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = rowItem.Image;
				if (image == null)
				{
					return null;
				}
				return image.Id;
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x000190C3 File Offset: 0x000172C3
		[DataSourceProperty]
		public string BannerId
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				WCulture culture = rowItem.Culture;
				if (culture == null)
				{
					return null;
				}
				BannerImageIdentifier image = culture.Image;
				if (image == null)
				{
					return null;
				}
				return image.Id;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060004A4 RID: 1188 RVA: 0x000190EC File Offset: 0x000172EC
		[DataSourceProperty]
		public string ImageAdditionalArgs
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = rowItem.Image;
				if (image == null)
				{
					return null;
				}
				return image.AdditionalArgs;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0001910A File Offset: 0x0001730A
		[DataSourceProperty]
		public string BannerAdditionalArgs
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				WCulture culture = rowItem.Culture;
				if (culture == null)
				{
					return null;
				}
				BannerImageIdentifier image = culture.Image;
				if (image == null)
				{
					return null;
				}
				return image.AdditionalArgs;
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00019133 File Offset: 0x00017333
		[DataSourceProperty]
		public string ImageTextureProviderName
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = rowItem.Image;
				if (image == null)
				{
					return null;
				}
				return image.TextureProviderName;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00019151 File Offset: 0x00017351
		[DataSourceProperty]
		public string BannerTextureProviderName
		{
			get
			{
				WItem rowItem = this.RowItem;
				if (rowItem == null)
				{
					return null;
				}
				WCulture culture = rowItem.Culture;
				if (culture == null)
				{
					return null;
				}
				BannerImageIdentifier image = culture.Image;
				if (image == null)
				{
					return null;
				}
				return image.TextureProviderName;
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0001917A File Offset: 0x0001737A
		[DataSourceProperty]
		public CharacterEquipmentItemVM Hint
		{
			get
			{
				if (this.RowItem == null)
				{
					return null;
				}
				return new CharacterEquipmentItemVM(this.RowItem.Base);
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001919C File Offset: 0x0001739C
		[DataSourceMethod]
		public void ExecuteSelect()
		{
			if (State.Troop == null || State.Equipment == null)
			{
				Log.Error("ExecuteSelect: Aborting - Troop or Equipment is null");
				return;
			}
			string format = "ExecuteSelect: Start RowItem={0}, Slot={1}, SetIndex={2}";
			WItem rowItem = this.RowItem;
			object obj;
			if (rowItem == null)
			{
				obj = null;
			}
			else
			{
				string name = rowItem.Name;
				obj = ((name != null) ? name.ToString() : null);
			}
			object arg = obj ?? "null";
			object arg2 = State.Slot;
			WEquipment equipment = State.Equipment;
			Log.Debug(string.Format(format, arg, arg2, ((equipment != null) ? equipment.Index.ToString() : null) ?? "null"));
			if (this.RowItem != null && !this.RowItem.Slots.Contains(State.Slot))
			{
				Log.Debug("ExecuteSelect: Aborting - RowItem does not fit slot");
				return;
			}
			if (!Config.EquippingTroopsTakesTime && !ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_modify", "modify")))
			{
				Log.Debug("ExecuteSelect: Aborting - Context not allowed for modification");
				return;
			}
			WCharacter troop = State.Troop;
			int setIndex = State.Equipment.Index;
			EquipmentIndex slot = State.Slot;
			if (PreviewOverlay.IsEnabled)
			{
				string format2 = "ExecuteSelect: Preview mode - Troop={0}, SetIndex={1}, Slot={2}, RowItem={3}";
				object[] array = new object[4];
				int num = 0;
				WCharacter troop3 = troop;
				array[num] = (((troop3 != null) ? troop3.ToString() : null) ?? "null");
				array[1] = setIndex;
				array[2] = slot;
				int num2 = 3;
				WItem rowItem2 = this.RowItem;
				object obj2;
				if (rowItem2 == null)
				{
					obj2 = null;
				}
				else
				{
					string name2 = rowItem2.Name;
					obj2 = ((name2 != null) ? name2.ToString() : null);
				}
				array[num2] = (obj2 ?? "null");
				Log.Debug(string.Format(format2, array));
				if (this.RowItem == null)
				{
					PreviewOverlay.SetPreview(troop, setIndex, slot, null);
					return;
				}
				PreviewOverlay.SetPreview(troop, setIndex, slot, this.RowItem);
				return;
			}
			else
			{
				WItem witem = State.Equipment.Get(slot);
				bool flag = this.RowItem == null;
				bool flag2 = this.RowItem != null && this.RowItem == witem;
				WItem witem2 = null;
				PendingEquipData pendingEquipData = EquipStagingBehavior.Get(troop, slot, setIndex);
				if (pendingEquipData != null)
				{
					witem2 = new WItem(pendingEquipData.ItemId);
				}
				bool flag3 = pendingEquipData != null;
				string format3 = "ExecuteSelect: Computed state - Troop={0}, EquippedItem={1}, selectionIsNull={2}, selectionIsEquipped={3}";
				object[] array2 = new object[4];
				int num3 = 0;
				WCharacter troop2 = troop;
				array2[num3] = (((troop2 != null) ? troop2.ToString() : null) ?? "null");
				int num4 = 1;
				object obj3;
				if (witem == null)
				{
					obj3 = null;
				}
				else
				{
					string name3 = witem.Name;
					obj3 = ((name3 != null) ? name3.ToString() : null);
				}
				array2[num4] = (obj3 ?? "null");
				array2[2] = flag;
				array2[3] = flag2;
				Log.Debug(string.Format(format3, array2));
				if (ClanScreen.IsStudioMode)
				{
					Log.Debug("ExecuteSelect: Studio mode - calling EquipmentManager.TryEquip (allowPurchase:false)");
					EquipmentManager.EquipResult equipResult = EquipmentManager.TryEquip(troop, setIndex, slot, this.RowItem, false);
					Log.Debug(string.Format("ExecuteSelect: TryEquip (studio) result - Ok={0}, Reason={1}, Staged={2}, RefundedCopies={3}, AddedCopies={4}, GoldDelta={5}", new object[]
					{
						equipResult.Ok,
						equipResult.Reason,
						equipResult.Staged,
						equipResult.RefundedCopies,
						equipResult.AddedCopies,
						equipResult.GoldDelta
					}));
					State.UpdateEquipData(null, true);
					return;
				}
				if (flag)
				{
					if (flag3 && witem2 != null)
					{
						Log.Debug("ExecuteSelect: Empty row clicked with pending change - rolling back staged equip and unstaging.");
						EquipmentManager.RollbackStagedEquip(troop, setIndex, slot, witem2);
					}
					if (!ClanScreen.IsStudioMode && Config.EquippingTroopsTakesTime && witem != null)
					{
						WLoadout loadout = troop.Loadout;
						int num5 = loadout.MaxCountPerSet(witem);
						int num6 = loadout.RequiredAfterForItem(witem, setIndex, slot, null);
						bool flag4 = num6 < num5;
						Log.Debug(string.Format("ExecuteSelect: Unequip path - revertWouldStage={0}, beforeOld={1}, afterOld={2}", flag4, num5, num6));
						if (flag4)
						{
							Log.Debug("ExecuteSelect: Showing unequip warning inquiry");
							InformationManager.ShowInquiry(new InquiryData(L.S("unequip_warn_title", "Unequip Item"), L.T("unequip_warning_text", "Unequipping is instant, but equipping a different item later may take time.\n\nConfirm?").ToString(), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
							{
								Log.Debug("ExecuteSelect: Unequip warning confirmed - calling EquipmentManager.TryUnequip");
								EquipmentManager.EquipResult equipResult4 = EquipmentManager.TryUnequip(troop, setIndex, slot);
								Log.Debug(string.Format("ExecuteSelect: TryUnequip result - Ok={0}, Reason={1}, Staged={2}, RefundedCopies={3}, AddedCopies={4}, GoldDelta={5}", new object[]
								{
									equipResult4.Ok,
									equipResult4.Reason,
									equipResult4.Staged,
									equipResult4.RefundedCopies,
									equipResult4.AddedCopies,
									equipResult4.GoldDelta
								}));
								State.UpdateEquipData(null, true);
							}, delegate()
							{
								Log.Debug("ExecuteSelect: Unequip warning cancelled by user");
							}, "", 0f, null, null, null), false, false);
							return;
						}
					}
					Log.Debug("ExecuteSelect: Unequip without warning - calling EquipmentManager.TryUnequip");
					EquipmentManager.EquipResult equipResult2 = EquipmentManager.TryUnequip(troop, setIndex, slot);
					Log.Debug(string.Format("ExecuteSelect: TryUnequip result - Ok={0}, Reason={1}, Staged={2}, RefundedCopies={3}, AddedCopies={4}, GoldDelta={5}", new object[]
					{
						equipResult2.Ok,
						equipResult2.Reason,
						equipResult2.Staged,
						equipResult2.RefundedCopies,
						equipResult2.AddedCopies,
						equipResult2.GoldDelta
					}));
					State.UpdateEquipData(null, true);
					return;
				}
				if (flag2)
				{
					if (flag3 && witem2 != null)
					{
						Log.Debug("ExecuteSelect: Equipped item clicked with pending change - rolling back staged equip and unstaging.");
						EquipmentManager.RollbackStagedEquip(troop, setIndex, slot, witem2);
						State.UpdateEquipData(null, true);
						return;
					}
					Log.Debug("ExecuteSelect: Selection is already equipped - refreshing state");
					State.UpdateEquipData(null, true);
					return;
				}
				else
				{
					if (flag3 && witem2 != null)
					{
						Log.Debug("ExecuteSelect: New item clicked while a staged change exists - rolling back previous staged equip and unstaging.");
						EquipmentManager.RollbackStagedEquip(troop, setIndex, slot, witem2);
					}
					EquipmentManager.EquipQuote equipQuote = EquipmentManager.QuoteEquip(troop, setIndex, slot, this.RowItem);
					Log.Debug(string.Format("ExecuteSelect: Quote - IsChange={0}, CopiesToBuy={1}, GoldCost={2}", equipQuote.IsChange, equipQuote.CopiesToBuy, equipQuote.GoldCost));
					if (!equipQuote.IsChange)
					{
						Log.Debug("ExecuteSelect: Quote indicates no change - refreshing state");
						State.UpdateEquipData(null, true);
						return;
					}
					bool flag5 = equipQuote.CopiesToBuy > 0 && equipQuote.GoldCost > 0;
					Log.Debug(string.Format("ExecuteSelect: needsPurchase={0}", flag5));
					if (flag5)
					{
						Log.Debug("ExecuteSelect: Showing purchase confirmation inquiry");
						InformationManager.ShowInquiry(new InquiryData(L.S("buy_item", "Buy Item"), L.T("buy_item_text", "Are you sure you want to buy {NAME} for {COST} gold?").SetTextVariable("NAME", this.RowItem.Name).SetTextVariable("COST", equipQuote.GoldCost).ToString(), true, true, L.S("yes", "Yes"), L.S("no", "No"), delegate()
						{
							Log.Debug("ExecuteSelect: Purchase confirmed - calling EquipmentManager.TryEquip (allowPurchase:true)");
							EquipmentManager.EquipResult equipResult4 = EquipmentManager.TryEquip(troop, setIndex, slot, this.RowItem, true);
							Log.Debug(string.Format("ExecuteSelect: TryEquip (purchase) result - Ok={0}, Reason={1}, Staged={2}, RefundedCopies={3}, AddedCopies={4}, GoldDelta={5}, Reason={6}", new object[]
							{
								equipResult4.Ok,
								equipResult4.Reason,
								equipResult4.Staged,
								equipResult4.RefundedCopies,
								equipResult4.AddedCopies,
								equipResult4.GoldDelta,
								equipResult4.Ok ? "Ok" : equipResult4.Reason.ToString()
							}));
							if (!equipResult4.Ok && equipResult4.Reason == EquipmentManager.EquipFailReason.NotEnoughGold)
							{
								Notifications.Popup(L.T("not_enough_gold_title", "Not Enough Gold"), L.T("not_enough_gold_text", "You do not have enough gold to purchase this item."), null, true);
								Log.Debug("ExecuteSelect: TryEquip failed - NotEnoughGold");
							}
							State.UpdateEquipData(null, true);
						}, delegate()
						{
							Log.Debug("ExecuteSelect: Purchase cancelled by user");
						}, "", 0f, null, null, null), false, false);
						return;
					}
					Log.Debug("ExecuteSelect: Direct equip path - calling EquipmentManager.TryEquip (allowPurchase:true)");
					EquipmentManager.EquipResult equipResult3 = EquipmentManager.TryEquip(troop, setIndex, slot, this.RowItem, true);
					Log.Debug(string.Format("ExecuteSelect: TryEquip (direct) result - Ok={0}, Reason={1}, Staged={2}, RefundedCopies={3}, AddedCopies={4}, GoldDelta={5}", new object[]
					{
						equipResult3.Ok,
						equipResult3.Reason,
						equipResult3.Staged,
						equipResult3.RefundedCopies,
						equipResult3.AddedCopies,
						equipResult3.GoldDelta
					}));
					State.UpdateEquipData(null, true);
					return;
				}
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00019978 File Offset: 0x00017B78
		public override bool FilterMatch(string filter)
		{
			if (this.RowItem == null)
			{
				return true;
			}
			string value = filter.Trim().ToLowerInvariant();
			string text = this.RowItem.Name.ToString().ToLowerInvariant();
			ItemCategory category = this.RowItem.Category;
			string text2 = (category != null) ? category.ToString().ToLowerInvariant() : null;
			string text3 = this.RowItem.Type.ToString().ToLowerInvariant();
			WCulture culture = this.RowItem.Culture;
			string text4;
			if (culture == null)
			{
				text4 = null;
			}
			else
			{
				string name = culture.Name;
				text4 = ((name != null) ? name.ToString().ToLowerInvariant() : null);
			}
			string text5 = text4 ?? string.Empty;
			return text.Contains(value) || text2.Contains(value) || text3.Contains(value) || text5.Contains(value);
		}

		// Token: 0x04000129 RID: 297
		public readonly WItem RowItem = rowItem;

		// Token: 0x0400012A RID: 298
		public readonly bool IsAvailable = isAvailable;

		// Token: 0x0400012B RID: 299
		public readonly bool IsUnlocked = isUnlocked;

		// Token: 0x0400012C RID: 300
		public readonly int Progress = progress;

		// Token: 0x0400012D RID: 301
		private int PositiveChevrons;

		// Token: 0x0400012E RID: 302
		private int NegativeChevrons;
	}
}
