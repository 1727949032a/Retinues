using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Equipment.List
{
	// Token: 0x02000084 RID: 132
	[SafeClass]
	public sealed class EquipmentListVM : BaseListVM
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0001767C File Offset: 0x0001587C
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x000176BE File Offset: 0x000158BE
		public bool ShowCrafted
		{
			get
			{
				return (DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>() || ClanScreen.IsStudioMode) && this._showCrafted && EquipmentListVM.WeaponSlots.Contains(State.Slot.ToString());
			}
			set
			{
				if (this._showCrafted != value)
				{
					this._showCrafted = value;
					this._needsRebuild = true;
					this._currentPageIndex = 0;
					if (base.IsVisible)
					{
						this.Build();
					}
				}
			}
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x000176EC File Offset: 0x000158EC
		private static EquipmentIndex GetSnapshotKey(EquipmentIndex slot)
		{
			if (slot == EquipmentIndex.WeaponItemBeginSlot || slot == EquipmentIndex.Weapon1 || slot == EquipmentIndex.Weapon2 || slot == EquipmentIndex.Weapon3)
			{
				return EquipmentIndex.WeaponItemBeginSlot;
			}
			return slot;
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x00017700 File Offset: 0x00015900
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00017707 File Offset: 0x00015907
		protected override void OnFactionChange()
		{
			this._needsRebuild = true;
			this._currentPageIndex = 0;
			if (base.IsVisible)
			{
				this.Build();
			}
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00017728 File Offset: 0x00015928
		protected override void OnEquipChange()
		{
			if (!base.IsVisible)
			{
				return;
			}
			EquipChangeDelta? lastEquipChange = State.LastEquipChange;
			if (lastEquipChange == null)
			{
				return;
			}
			this._equipChangeIds.Clear();
			EquipChangeDelta value = lastEquipChange.Value;
			this.<OnEquipChange>g__Handle|26_0(value.OldEquippedId);
			this.<OnEquipChange>g__Handle|26_0(value.NewEquippedId);
			this.<OnEquipChange>g__Handle|26_0(value.OldStagedId);
			this.<OnEquipChange>g__Handle|26_0(value.NewStagedId);
			foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
			{
				equipmentRowVM.OnEquipChanged();
			}
			State.LastEquipChange = null;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x000177DC File Offset: 0x000159DC
		protected override void OnTroopChange()
		{
			this._needsRebuild = true;
			this._currentPageIndex = 0;
			if (base.IsVisible)
			{
				this.Build();
			}
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x000177FC File Offset: 0x000159FC
		protected override void OnSlotChange()
		{
			this._needsRebuild = true;
			this._currentPageIndex = 0;
			if (!base.IsVisible)
			{
				return;
			}
			BaseFaction faction = State.Faction;
			string b = (faction != null) ? faction.StringId : null;
			if (this._lastFactionId == b)
			{
				string text = State.Slot.ToString();
				if (this._lastSlotId == text)
				{
					this._needsRebuild = false;
					return;
				}
				if (EquipmentListVM.WeaponSlots.Contains(text) && EquipmentListVM.WeaponSlots.Contains(this._lastSlotId))
				{
					this._needsRebuild = false;
					string lastSlotId = this._lastSlotId;
					this._lastSlotId = text;
					EquipmentIndex oldSlot;
					EquipmentIndex newSlot;
					if (EquipmentListVM.TryParseSlot(lastSlotId, out oldSlot) && EquipmentListVM.TryParseSlot(text, out newSlot))
					{
						this.NotifySlotSelectionChanged(oldSlot, newSlot);
						this.NotifyEquippedRowChanged(oldSlot, newSlot);
					}
					foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
					{
						equipmentRowVM.OnSlotChanged();
					}
					return;
				}
			}
			base.FilterText = string.Empty;
			this.Build();
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001791C File Offset: 0x00015B1C
		private static bool TryParseSlot(string slotId, out EquipmentIndex slot)
		{
			return Enum.TryParse<EquipmentIndex>(slotId, out slot);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00017928 File Offset: 0x00015B28
		private WItem GetCurrentItemForSlot(EquipmentIndex slot)
		{
			EquipData equipData;
			if (State.EquipData != null && State.EquipData.TryGetValue(slot, out equipData) && equipData.Equip != null)
			{
				return new WItem(equipData.Equip.ItemId);
			}
			WEquipment equipment = State.Equipment;
			if (equipment == null)
			{
				return null;
			}
			return equipment.Get(slot);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00017978 File Offset: 0x00015B78
		private void NotifyEquippedRowChanged(EquipmentIndex oldSlot, EquipmentIndex newSlot)
		{
			WItem currentItemForSlot = this.GetCurrentItemForSlot(oldSlot);
			WItem currentItemForSlot2 = this.GetCurrentItemForSlot(newSlot);
			EquipmentRowVM equipmentRowVM = null;
			if (currentItemForSlot != null && !string.IsNullOrEmpty(currentItemForSlot.StringId) && this._rowsByItemId.TryGetValue(currentItemForSlot.StringId, out equipmentRowVM) && equipmentRowVM != null)
			{
				equipmentRowVM.OnSlotChangedSelective();
			}
			EquipmentRowVM equipmentRowVM2;
			if (currentItemForSlot2 != null && !string.IsNullOrEmpty(currentItemForSlot2.StringId) && this._rowsByItemId.TryGetValue(currentItemForSlot2.StringId, out equipmentRowVM2) && equipmentRowVM2 != null && equipmentRowVM2 != equipmentRowVM)
			{
				equipmentRowVM2.OnSlotChangedSelective();
			}
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00017A04 File Offset: 0x00015C04
		private void NotifySlotSelectionChanged(EquipmentIndex oldSlot, EquipmentIndex newSlot)
		{
			WItem currentItemForSlot = this.GetCurrentItemForSlot(oldSlot);
			WItem currentItemForSlot2 = this.GetCurrentItemForSlot(newSlot);
			if (currentItemForSlot == null)
			{
				EquipmentRowVM emptyRow = this._emptyRow;
				if (emptyRow != null)
				{
					emptyRow.OnSlotChangedSelective();
				}
			}
			else
			{
				string stringId = currentItemForSlot.StringId;
				EquipmentRowVM equipmentRowVM;
				if (!string.IsNullOrEmpty(stringId) && this._rowsByItemId.TryGetValue(stringId, out equipmentRowVM) && equipmentRowVM != null)
				{
					equipmentRowVM.OnSlotChangedSelective();
				}
			}
			if (!(currentItemForSlot2 == null))
			{
				string stringId2 = currentItemForSlot2.StringId;
				EquipmentRowVM equipmentRowVM2;
				if (!string.IsNullOrEmpty(stringId2) && this._rowsByItemId.TryGetValue(stringId2, out equipmentRowVM2) && equipmentRowVM2 != null)
				{
					equipmentRowVM2.OnSlotChangedSelective();
				}
				return;
			}
			EquipmentRowVM emptyRow2 = this._emptyRow;
			if (emptyRow2 == null)
			{
				return;
			}
			emptyRow2.OnSlotChangedSelective();
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00017AAC File Offset: 0x00015CAC
		protected override void OnEquipmentChange()
		{
			if (base.IsVisible)
			{
				foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
				{
					equipmentRowVM.OnEquipmentChanged();
				}
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00017B00 File Offset: 0x00015D00
		public void Build()
		{
			if (!this._needsRebuild)
			{
				return;
			}
			BaseFaction faction = State.Faction;
			string text = (faction != null) ? faction.StringId : null;
			string text2 = State.Slot.ToString();
			EquipmentIndex slot = State.Slot;
			string text3 = string.Format("{0}|{1}|{2}", text ?? string.Empty, text2, (this.ShowCrafted > false) ? 1 : 0);
			string a;
			List<EquipmentListVM.ItemTuple> list;
			if (this._fullTupleKeys.TryGetValue(slot, out a) && a == text3 && this._fullTuples.TryGetValue(slot, out list) && list != null)
			{
				Log.Info(string.Format("Reusing cached equipment list for slot {0}", State.Slot));
				this._needsRebuild = false;
				this._lastFactionId = text;
				this._lastSlotId = text2;
				this.RebuildVisibleFromSnapshot();
				foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
				{
					equipmentRowVM.OnSlotChanged();
				}
				return;
			}
			Log.Info(string.Format("Rebuilding equipment list for slot {0}", State.Slot));
			this._needsRebuild = false;
			this._lastFactionId = text;
			this._lastSlotId = text2;
			if (!this._cache.ContainsKey(text))
			{
				this._cache[text] = new Dictionary<string, List<ValueTuple<WItem, bool, int>>>();
			}
			if (!this._cache[text].ContainsKey(text2))
			{
				this._cache[text][text2] = null;
			}
			List<ValueTuple<WItem, bool, int>> list2 = this._cache[text][text2];
			bool flag = list2 == null;
			if (flag && this.ShowCrafted)
			{
				flag = false;
			}
			if (flag)
			{
				this._cache[text][text2] = new List<ValueTuple<WItem, bool, int>>();
			}
			List<ValueTuple<WItem, bool, bool, int>> list3 = EquipmentManager.CollectAvailableItems(State.Faction, State.Slot, list2, this.ShowCrafted);
			if (flag)
			{
				this._cache[text][text2].Clear();
				foreach (ValueTuple<WItem, bool, bool, int> valueTuple in list3)
				{
					WItem item = valueTuple.Item1;
					bool item2 = valueTuple.Item3;
					int item3 = valueTuple.Item4;
					this._cache[text][text2].Add(new ValueTuple<WItem, bool, int>(item, item2, item3));
				}
			}
			this._rowsByItemId.Clear();
			EquipmentIndex snapshotKey = EquipmentListVM.GetSnapshotKey(State.Slot);
			List<EquipmentListVM.ItemTuple> list4 = new List<EquipmentListVM.ItemTuple>(list3.Count);
			this._fullTuples[snapshotKey] = list4;
			foreach (ValueTuple<WItem, bool, bool, int> valueTuple2 in list3)
			{
				WItem item4 = valueTuple2.Item1;
				bool item5 = valueTuple2.Item2;
				bool item6 = valueTuple2.Item3;
				int item7 = valueTuple2.Item4;
				if (!(item4 == null))
				{
					string key = item4.StringId ?? item4.GetHashCode().ToString();
					if (!this._rowsByItemId.ContainsKey(key))
					{
						this._rowsByItemId[key] = null;
						list4.Add(new EquipmentListVM.ItemTuple
						{
							Item = item4,
							IsAvailable = item5,
							IsUnlocked = item6,
							Progress = item7,
							Name = (item4.Name ?? string.Empty),
							Category = (item4.Class ?? string.Empty),
							Tier = item4.Tier,
							Value = item4.Value,
							Cost = EquipmentManager.GetItemCost(item4),
							EnabledRank = ((item6 && item5) ? 0 : 1)
						});
					}
				}
			}
			this._fullTupleKeys[slot] = text3;
			this.RebuildVisibleFromSnapshot();
			foreach (EquipmentRowVM equipmentRowVM2 in this.EquipmentRows)
			{
				equipmentRowVM2.OnSlotChanged();
			}
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00017F58 File Offset: 0x00016158
		private void EnsureSnapshotForCurrentSlot()
		{
			if (this._needsRebuild)
			{
				this.Build();
			}
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00017F68 File Offset: 0x00016168
		private void RebuildVisibleFromSnapshot()
		{
			EquipmentIndex snapshotKey = EquipmentListVM.GetSnapshotKey(State.Slot);
			List<EquipmentListVM.ItemTuple> list;
			if (!this._fullTuples.TryGetValue(snapshotKey, out list) || list == null)
			{
				list = new List<EquipmentListVM.ItemTuple>();
			}
			list.Sort(Comparer<EquipmentListVM.ItemTuple>.Create(new Comparison<EquipmentListVM.ItemTuple>(this.<RebuildVisibleFromSnapshot>g__Compare|36_1)));
			List<EquipmentListVM.ItemTuple> list2 = new List<EquipmentListVM.ItemTuple>(list.Count);
			string text = base.FilterText ?? string.Empty;
			if (string.IsNullOrWhiteSpace(text))
			{
				list2.AddRange(list);
			}
			else
			{
				EquipmentListVM.<>c__DisplayClass36_0 CS$<>8__locals1;
				CS$<>8__locals1.search = text.Trim().ToLowerInvariant();
				foreach (EquipmentListVM.ItemTuple itemTuple in list)
				{
					if (EquipmentListVM.<RebuildVisibleFromSnapshot>g__Matches|36_2(itemTuple, ref CS$<>8__locals1))
					{
						list2.Add(itemTuple);
					}
				}
			}
			this._filteredCount = list2.Count;
			if (this._filteredCount == 0)
			{
				this._totalPages = 1;
				this._currentPageIndex = 0;
			}
			else
			{
				this._totalPages = (this._filteredCount + this.MaxRows - 1) / this.MaxRows;
				if (this._currentPageIndex < 0)
				{
					this._currentPageIndex = 0;
				}
				if (this._currentPageIndex >= this._totalPages)
				{
					this._currentPageIndex = this._totalPages - 1;
				}
			}
			int num = this._currentPageIndex * this.MaxRows;
			int num2 = (this._filteredCount == 0) ? 0 : Math.Min(this.MaxRows, this._filteredCount - num);
			List<EquipmentListVM.ItemTuple> list3 = (num2 > 0) ? list2.GetRange(num, num2) : new List<EquipmentListVM.ItemTuple>();
			this.EquipmentRows.Clear();
			this._emptyRow = new EquipmentRowVM(null, true, true, 0)
			{
				IsVisible = base.IsVisible
			};
			this.EquipmentRows.Add(this._emptyRow);
			foreach (EquipmentListVM.ItemTuple itemTuple2 in list3)
			{
				EquipmentRowVM equipmentRowVM = new EquipmentRowVM(itemTuple2.Item, itemTuple2.IsAvailable, itemTuple2.IsUnlocked, itemTuple2.Progress)
				{
					IsVisible = base.IsVisible
				};
				this.EquipmentRows.Add(equipmentRowVM);
				WItem item = itemTuple2.Item;
				string text2;
				if ((text2 = ((item != null) ? item.StringId : null)) == null)
				{
					WItem item2 = itemTuple2.Item;
					text2 = ((item2 != null) ? item2.GetHashCode().ToString() : null);
				}
				string text3 = text2;
				if (!string.IsNullOrEmpty(text3))
				{
					this._rowsByItemId[text3] = equipmentRowVM;
				}
			}
			foreach (EquipmentRowVM equipmentRowVM2 in this.EquipmentRows)
			{
				equipmentRowVM2.OnSlotChanged();
			}
			base.OnPropertyChanged("SortByNameSelected");
			base.OnPropertyChanged("SortByNameState");
			base.OnPropertyChanged("SortByCategorySelected");
			base.OnPropertyChanged("SortByCategoryState");
			base.OnPropertyChanged("SortByTierSelected");
			base.OnPropertyChanged("SortByTierState");
			base.OnPropertyChanged("SortByValueSelected");
			base.OnPropertyChanged("SortByValueState");
			base.OnPropertyChanged("CurrentPage");
			base.OnPropertyChanged("TotalPages");
			base.OnPropertyChanged("CanGoPrevPage");
			base.OnPropertyChanged("CanGoNextPage");
			base.OnPropertyChanged("PageText");
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x000182C0 File Offset: 0x000164C0
		// (set) Token: 0x0600045F RID: 1119 RVA: 0x000182C8 File Offset: 0x000164C8
		[DataSourceProperty]
		public MBBindingList<EquipmentRowVM> EquipmentRows { get; set; } = new MBBindingList<EquipmentRowVM>();

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x000182D1 File Offset: 0x000164D1
		[DataSourceProperty]
		public bool SortByNameSelected
		{
			get
			{
				return this._sort == EquipmentListVM.SortMode.Name;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x000182DC File Offset: 0x000164DC
		[DataSourceProperty]
		public bool SortByCategorySelected
		{
			get
			{
				return this._sort == EquipmentListVM.SortMode.Category;
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x000182E7 File Offset: 0x000164E7
		[DataSourceProperty]
		public bool SortByTierSelected
		{
			get
			{
				return this._sort == EquipmentListVM.SortMode.Tier;
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x000182F2 File Offset: 0x000164F2
		[DataSourceProperty]
		public bool SortByValueSelected
		{
			get
			{
				return this._sort == EquipmentListVM.SortMode.Value;
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x000182FD File Offset: 0x000164FD
		[DataSourceProperty]
		public CampaignUIHelper.SortState SortByNameState
		{
			get
			{
				if (this._sort != EquipmentListVM.SortMode.Name)
				{
					return CampaignUIHelper.SortState.Default;
				}
				if (!this._descending)
				{
					return CampaignUIHelper.SortState.Ascending;
				}
				return CampaignUIHelper.SortState.Descending;
			}
		}

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x00018315 File Offset: 0x00016515
		[DataSourceProperty]
		public CampaignUIHelper.SortState SortByCategoryState
		{
			get
			{
				if (this._sort != EquipmentListVM.SortMode.Category)
				{
					return CampaignUIHelper.SortState.Default;
				}
				if (!this._descending)
				{
					return CampaignUIHelper.SortState.Ascending;
				}
				return CampaignUIHelper.SortState.Descending;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x0001832C File Offset: 0x0001652C
		[DataSourceProperty]
		public CampaignUIHelper.SortState SortByTierState
		{
			get
			{
				if (this._sort != EquipmentListVM.SortMode.Tier)
				{
					return CampaignUIHelper.SortState.Default;
				}
				if (!this._descending)
				{
					return CampaignUIHelper.SortState.Ascending;
				}
				return CampaignUIHelper.SortState.Descending;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00018344 File Offset: 0x00016544
		[DataSourceProperty]
		public CampaignUIHelper.SortState SortByValueState
		{
			get
			{
				if (this._sort != EquipmentListVM.SortMode.Value)
				{
					return CampaignUIHelper.SortState.Default;
				}
				if (!this._descending)
				{
					return CampaignUIHelper.SortState.Ascending;
				}
				return CampaignUIHelper.SortState.Descending;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x0001835C File Offset: 0x0001655C
		[DataSourceProperty]
		public string SortByNameText
		{
			get
			{
				return L.S("sort_name", "Name");
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x0001836D File Offset: 0x0001656D
		[DataSourceProperty]
		public string SortByCategoryText
		{
			get
			{
				return L.S("sort_category", "Category");
			}
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x0001837E File Offset: 0x0001657E
		[DataSourceProperty]
		public string SortByTierText
		{
			get
			{
				return L.S("sort_tier", "Tier");
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x0001838F File Offset: 0x0001658F
		[DataSourceProperty]
		public string SortByValueText
		{
			get
			{
				return L.S("sort_value", "Value");
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x000183A0 File Offset: 0x000165A0
		[DataSourceProperty]
		public int CurrentPage
		{
			get
			{
				if (this._totalPages != 0)
				{
					return this._currentPageIndex + 1;
				}
				return 0;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x000183B4 File Offset: 0x000165B4
		[DataSourceProperty]
		public int TotalPages
		{
			get
			{
				return this._totalPages;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x000183BC File Offset: 0x000165BC
		[DataSourceProperty]
		public bool CanGoPrevPage
		{
			get
			{
				return this.CurrentPage > 1;
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x000183C7 File Offset: 0x000165C7
		[DataSourceProperty]
		public bool CanGoNextPage
		{
			get
			{
				return this.CurrentPage < this.TotalPages;
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x000183D7 File Offset: 0x000165D7
		[DataSourceProperty]
		public string PageText
		{
			get
			{
				return string.Format("{0}/{1}", this.CurrentPage, this.TotalPages);
			}
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x000183F9 File Offset: 0x000165F9
		[DataSourceMethod]
		public void ExecuteSortByName()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (this._sort == EquipmentListVM.SortMode.Name)
			{
				this._descending = !this._descending;
			}
			else
			{
				this._sort = EquipmentListVM.SortMode.Name;
				this._descending = false;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001842F File Offset: 0x0001662F
		[DataSourceMethod]
		public void ExecuteSortByCategory()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (this._sort == EquipmentListVM.SortMode.Category)
			{
				this._descending = !this._descending;
			}
			else
			{
				this._sort = EquipmentListVM.SortMode.Category;
				this._descending = false;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00018464 File Offset: 0x00016664
		[DataSourceMethod]
		public void ExecuteSortByTier()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (this._sort == EquipmentListVM.SortMode.Tier)
			{
				this._descending = !this._descending;
			}
			else
			{
				this._sort = EquipmentListVM.SortMode.Tier;
				this._descending = false;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001849A File Offset: 0x0001669A
		[DataSourceMethod]
		public void ExecuteSortByValue()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (this._sort == EquipmentListVM.SortMode.Value)
			{
				this._descending = !this._descending;
			}
			else
			{
				this._sort = EquipmentListVM.SortMode.Value;
				this._descending = false;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000184D0 File Offset: 0x000166D0
		[DataSourceMethod]
		public void ExecutePrevPage()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (!this.CanGoPrevPage)
			{
				return;
			}
			this._currentPageIndex--;
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x000184F5 File Offset: 0x000166F5
		[DataSourceMethod]
		public void ExecuteNextPage()
		{
			this.EnsureSnapshotForCurrentSlot();
			if (!this.CanGoNextPage)
			{
				return;
			}
			this._currentPageIndex++;
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x0001851A File Offset: 0x0001671A
		public override List<BaseListElementVM> Rows
		{
			get
			{
				return this.EquipmentRows.ToList<BaseListElementVM>();
			}
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00018527 File Offset: 0x00016727
		protected override void OnFilterTextChanged()
		{
			this._currentPageIndex = 0;
			if (!base.IsVisible)
			{
				return;
			}
			if (this._needsRebuild)
			{
				return;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00018548 File Offset: 0x00016748
		public override void RefreshFilter()
		{
			if (!base.IsVisible)
			{
				return;
			}
			if (this._needsRebuild)
			{
				this.Build();
				return;
			}
			this.RebuildVisibleFromSnapshot();
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00018568 File Offset: 0x00016768
		public override void Show()
		{
			base.Show();
			foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
			{
				equipmentRowVM.Show();
			}
			if (this._needsRebuild)
			{
				this.Build();
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x000185C8 File Offset: 0x000167C8
		public override void Hide()
		{
			foreach (EquipmentRowVM equipmentRowVM in this.EquipmentRows)
			{
				equipmentRowVM.Hide();
			}
			base.Hide();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00018704 File Offset: 0x00016904
		[CompilerGenerated]
		private void <OnEquipChange>g__Handle|26_0(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				EquipmentRowVM emptyRow = this._emptyRow;
				if (emptyRow == null)
				{
					return;
				}
				emptyRow.OnEquipChangedSelective();
				return;
			}
			else
			{
				if (!this._equipChangeIds.Add(id))
				{
					return;
				}
				EquipmentRowVM equipmentRowVM;
				if (this._rowsByItemId.TryGetValue(id, out equipmentRowVM) && equipmentRowVM != null)
				{
					equipmentRowVM.OnEquipChangedSelective();
				}
				return;
			}
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00018752 File Offset: 0x00016952
		[CompilerGenerated]
		private int <RebuildVisibleFromSnapshot>g__Primary|36_0(int v)
		{
			if (!this._descending)
			{
				return v;
			}
			return -v;
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00018760 File Offset: 0x00016960
		[CompilerGenerated]
		private int <RebuildVisibleFromSnapshot>g__Compare|36_1(EquipmentListVM.ItemTuple a, EquipmentListVM.ItemTuple b)
		{
			if (a.EnabledRank != b.EnabledRank)
			{
				return a.EnabledRank - b.EnabledRank;
			}
			switch (this._sort)
			{
			case EquipmentListVM.SortMode.Category:
			{
				int num = this.<RebuildVisibleFromSnapshot>g__Primary|36_0(string.Compare(a.Category, b.Category, StringComparison.Ordinal));
				if (num != 0)
				{
					return num;
				}
				return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
			}
			case EquipmentListVM.SortMode.Name:
				return this.<RebuildVisibleFromSnapshot>g__Primary|36_0(string.Compare(a.Name, b.Name, StringComparison.Ordinal));
			case EquipmentListVM.SortMode.Tier:
			{
				int num2 = this.<RebuildVisibleFromSnapshot>g__Primary|36_0(a.Tier.CompareTo(b.Tier));
				if (num2 != 0)
				{
					return num2;
				}
				num2 = string.Compare(a.Category, b.Category, StringComparison.Ordinal);
				if (num2 != 0)
				{
					return num2;
				}
				return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
			}
			case EquipmentListVM.SortMode.Value:
			{
				int num3 = this.<RebuildVisibleFromSnapshot>g__Primary|36_0(a.Value.CompareTo(b.Value));
				if (num3 != 0)
				{
					return num3;
				}
				return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
			}
			default:
				return 0;
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00018870 File Offset: 0x00016A70
		[CompilerGenerated]
		internal static bool <RebuildVisibleFromSnapshot>g__Matches|36_2(EquipmentListVM.ItemTuple t, ref EquipmentListVM.<>c__DisplayClass36_0 A_1)
		{
			if (t.Item == null)
			{
				return true;
			}
			string name = t.Name;
			string text = ((name != null) ? name.ToLowerInvariant() : null) ?? string.Empty;
			string category = t.Category;
			string text2 = ((category != null) ? category.ToLowerInvariant() : null) ?? string.Empty;
			string text3 = t.Item.Type.ToString().ToLowerInvariant();
			WCulture culture = t.Item.Culture;
			string text4;
			if (culture == null)
			{
				text4 = null;
			}
			else
			{
				string name2 = culture.Name;
				text4 = ((name2 != null) ? name2.ToString().ToLowerInvariant() : null);
			}
			string text5 = text4 ?? string.Empty;
			return text.Contains(A_1.search) || text2.Contains(A_1.search) || text3.Contains(A_1.search) || text5.Contains(A_1.search);
		}

		// Token: 0x04000117 RID: 279
		private bool _needsRebuild = true;

		// Token: 0x04000118 RID: 280
		private EquipmentListVM.SortMode _sort;

		// Token: 0x04000119 RID: 281
		private bool _descending;

		// Token: 0x0400011A RID: 282
		public int MaxRows = Config.MaxEquipmentRowsPerPage;

		// Token: 0x0400011B RID: 283
		private int _filteredCount;

		// Token: 0x0400011C RID: 284
		private int _currentPageIndex;

		// Token: 0x0400011D RID: 285
		private int _totalPages = 1;

		// Token: 0x0400011E RID: 286
		private readonly Dictionary<EquipmentIndex, List<EquipmentListVM.ItemTuple>> _fullTuples = new Dictionary<EquipmentIndex, List<EquipmentListVM.ItemTuple>>();

		// Token: 0x0400011F RID: 287
		private readonly Dictionary<EquipmentIndex, string> _fullTupleKeys = new Dictionary<EquipmentIndex, string>();

		// Token: 0x04000120 RID: 288
		private bool _showCrafted;

		// Token: 0x04000121 RID: 289
		[TupleElementNames(new string[]
		{
			"item",
			"unlocked",
			"progress"
		})]
		private readonly Dictionary<string, Dictionary<string, List<ValueTuple<WItem, bool, int>>>> _cache = new Dictionary<string, Dictionary<string, List<ValueTuple<WItem, bool, int>>>>();

		// Token: 0x04000122 RID: 290
		private string _lastFactionId;

		// Token: 0x04000123 RID: 291
		private string _lastSlotId;

		// Token: 0x04000124 RID: 292
		private readonly Dictionary<string, EquipmentRowVM> _rowsByItemId = new Dictionary<string, EquipmentRowVM>(StringComparer.Ordinal);

		// Token: 0x04000125 RID: 293
		private readonly HashSet<string> _equipChangeIds = new HashSet<string>(StringComparer.Ordinal);

		// Token: 0x04000126 RID: 294
		private EquipmentRowVM _emptyRow;

		// Token: 0x04000127 RID: 295
		public static readonly List<string> WeaponSlots = new List<string>(4)
		{
			EquipmentIndex.WeaponItemBeginSlot.ToString(),
			EquipmentIndex.Weapon1.ToString(),
			EquipmentIndex.Weapon2.ToString(),
			EquipmentIndex.Weapon3.ToString()
		};

		// Token: 0x02000157 RID: 343
		private enum SortMode
		{
			// Token: 0x040003F2 RID: 1010
			Category,
			// Token: 0x040003F3 RID: 1011
			Name,
			// Token: 0x040003F4 RID: 1012
			Tier,
			// Token: 0x040003F5 RID: 1013
			Value
		}

		// Token: 0x02000158 RID: 344
		private sealed class ItemTuple
		{
			// Token: 0x040003F6 RID: 1014
			public WItem Item;

			// Token: 0x040003F7 RID: 1015
			public bool IsAvailable;

			// Token: 0x040003F8 RID: 1016
			public bool IsUnlocked;

			// Token: 0x040003F9 RID: 1017
			public int Progress;

			// Token: 0x040003FA RID: 1018
			public string Name;

			// Token: 0x040003FB RID: 1019
			public string Category;

			// Token: 0x040003FC RID: 1020
			public int Tier;

			// Token: 0x040003FD RID: 1021
			public int Value;

			// Token: 0x040003FE RID: 1022
			public int Cost;

			// Token: 0x040003FF RID: 1023
			public int EnabledRank;
		}
	}
}
