using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Features.Agents;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000098 RID: 152
	[SafeClass]
	public class WLoadout
	{
		// Token: 0x0600066B RID: 1643 RVA: 0x00020019 File Offset: 0x0001E219
		public WLoadout(WCharacter troop)
		{
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x00020028 File Offset: 0x0001E228
		public WCharacter Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x00020030 File Offset: 0x0001E230
		private bool IsHero
		{
			get
			{
				return this._troop.IsHero;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x0002003D File Offset: 0x0001E23D
		private Hero Hero
		{
			get
			{
				return this._troop.Base.HeroObject;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600066F RID: 1647 RVA: 0x00020050 File Offset: 0x0001E250
		// (set) Token: 0x06000670 RID: 1648 RVA: 0x000200E4 File Offset: 0x0001E2E4
		public List<Equipment> BaseEquipments
		{
			get
			{
				if (!this.IsHero)
				{
					MBEquipmentRoster fieldValue = Reflector.GetFieldValue<MBEquipmentRoster>(this._troop.Base, "_equipmentRoster");
					return ((fieldValue != null) ? fieldValue.AllEquipments : null) ?? new MBReadOnlyList<Equipment>();
				}
				if (this.Hero == null)
				{
					return new List<Equipment>();
				}
				List<Equipment> list = new List<Equipment>();
				if (this.Hero.BattleEquipment != null)
				{
					list.Add(this.Hero.BattleEquipment);
				}
				if (this.Hero.CivilianEquipment != null)
				{
					list.Add(this.Hero.CivilianEquipment);
				}
				return list;
			}
			set
			{
				if (this.IsHero)
				{
					return;
				}
				MBEquipmentRoster mbequipmentRoster = new MBEquipmentRoster();
				Reflector.SetFieldValue(mbequipmentRoster, "_equipments", new MBList<Equipment>(value ?? new List<Equipment>()));
				Reflector.SetFieldValue(this._troop.Base, "_equipmentRoster", mbequipmentRoster);
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000671 RID: 1649 RVA: 0x00020130 File Offset: 0x0001E330
		public List<WEquipment> Equipments
		{
			get
			{
				return (from e in this.BaseEquipments
				select new WEquipment(e, this, this.BaseEquipments.IndexOf(e))).ToList<WEquipment>();
			}
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00020150 File Offset: 0x0001E350
		public void SetEquipments(List<WEquipment> value)
		{
			if (this.IsHero)
			{
				this.ApplyHeroEquipments(value);
				this.Troop.NeedsPersistence = true;
				return;
			}
			this.BaseEquipments = (from we in value
			select we.Base).ToList<Equipment>();
			this.Troop.NeedsPersistence = true;
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x000201B5 File Offset: 0x0001E3B5
		public WEquipment Battle
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Equipments.FirstOrDefault((WEquipment eq) => !eq.IsCivilian);
				}
				return this.Equipments.FirstOrDefault<WEquipment>();
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000674 RID: 1652 RVA: 0x000201F8 File Offset: 0x0001E3F8
		public WEquipment Civilian
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Equipments.FirstOrDefault((WEquipment eq) => eq.IsCivilian);
				}
				if (this.Equipments.Count <= 1)
				{
					return null;
				}
				return this.Equipments[1];
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x00020254 File Offset: 0x0001E454
		public List<WEquipment> BattleSets
		{
			get
			{
				if (!this.IsHero)
				{
					return (from eq in this.Equipments
					where !eq.IsCivilian
					select eq).ToList<WEquipment>();
				}
				if (this.Battle == null)
				{
					return new List<WEquipment>();
				}
				return new List<WEquipment>(1)
				{
					this.Battle
				};
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x000202BC File Offset: 0x0001E4BC
		public List<WEquipment> CivilianSets
		{
			get
			{
				if (!this.IsHero)
				{
					return (from eq in this.Equipments
					where eq.IsCivilian
					select eq).ToList<WEquipment>();
				}
				if (this.Civilian == null)
				{
					return new List<WEquipment>();
				}
				return new List<WEquipment>(1)
				{
					this.Civilian
				};
			}
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00020324 File Offset: 0x0001E524
		public WEquipment Get(int index)
		{
			List<WEquipment> equipments = this.Equipments;
			if (index < 0 || index >= equipments.Count)
			{
				return null;
			}
			return equipments[index];
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00020350 File Offset: 0x0001E550
		public EquipmentCategory GetCategory(int index)
		{
			if (this.IsHero)
			{
				if (index == 0)
				{
					return EquipmentCategory.Invalid;
				}
				if (index == 1)
				{
					return EquipmentCategory.Civilian;
				}
				return EquipmentCategory.Invalid;
			}
			else
			{
				WEquipment wequipment = this.Get(index);
				if (wequipment == null)
				{
					return EquipmentCategory.Invalid;
				}
				if (!wequipment.IsCivilian)
				{
					return EquipmentCategory.Invalid;
				}
				return EquipmentCategory.Civilian;
			}
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0002038A File Offset: 0x0001E58A
		public WEquipment CreateBattleSet()
		{
			return this.CreateSet(false);
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00020393 File Offset: 0x0001E593
		public WEquipment CreateCivilianSet()
		{
			return this.CreateSet(true);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0002039C File Offset: 0x0001E59C
		public WEquipment CreateSet(bool civilian)
		{
			if (this.IsHero)
			{
				WEquipment result;
				if (!civilian)
				{
					if ((result = this.Battle) == null)
					{
						return this.Civilian;
					}
				}
				else
				{
					result = (this.Civilian ?? this.Battle);
				}
				return result;
			}
			WEquipment wequipment = WEquipment.FromCode(null, this, this.Equipments.Count, new bool?(civilian));
			List<WEquipment> equipments = this.Equipments;
			equipments.Add(wequipment);
			this.SetEquipments(equipments);
			this.Normalize();
			this.Troop.NeedsPersistence = true;
			return wequipment;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00020418 File Offset: 0x0001E618
		public void ToggleCivilian(WEquipment eq, bool makeCivilian)
		{
			if (eq == null)
			{
				return;
			}
			if (this.IsHero)
			{
				return;
			}
			if (makeCivilian && !eq.IsCivilian && this.BattleSets.Count <= 1)
			{
				return;
			}
			if (!makeCivilian && eq.IsCivilian && this.CivilianSets.Count <= 1)
			{
				return;
			}
			eq.SetCivilian(makeCivilian);
			this.Normalize();
			this.Troop.NeedsPersistence = true;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00020480 File Offset: 0x0001E680
		public void Remove(WEquipment equipment)
		{
			if (equipment == null)
			{
				return;
			}
			if (this.IsHero)
			{
				return;
			}
			List<WEquipment> equipments = this.Equipments;
			int index = equipment.Index;
			if (index < 0 || index >= equipments.Count)
			{
				return;
			}
			bool isCivilian = equipment.IsCivilian;
			int count = this.CivilianSets.Count;
			int count2 = this.BattleSets.Count;
			if (isCivilian && count <= 1)
			{
				return;
			}
			if (!isCivilian && count2 <= 1)
			{
				return;
			}
			equipments.RemoveAt(index);
			this.SetEquipments(equipments);
			CombatAgentBehavior.OnRemoved(this.Troop, index);
			this.Normalize();
			this.Troop.NeedsPersistence = true;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00020513 File Offset: 0x0001E713
		public void Clear()
		{
			this.SetEquipments(new List<WEquipment>(2)
			{
				WEquipment.FromCode(null, this, 0, new bool?(false)),
				WEquipment.FromCode(null, this, 1, new bool?(true))
			});
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0002054C File Offset: 0x0001E74C
		public void FillFrom(WLoadout loadout, bool copyAll = false)
		{
			if (copyAll)
			{
				this.SetEquipments(loadout.Equipments.Select((WEquipment eq, int i) => WEquipment.FromCode(eq.Code, this, i, new bool?(eq.IsCivilian))).ToList<WEquipment>());
				this.Normalize();
				return;
			}
			WEquipment wequipment = null;
			WEquipment wequipment2 = null;
			foreach (WEquipment wequipment3 in loadout.Equipments)
			{
				if (wequipment3.IsCivilian && wequipment2 == null)
				{
					wequipment2 = WEquipment.FromCode(wequipment3.Code, this, this.Equipments.Count, new bool?(true));
				}
				else if (!wequipment3.IsCivilian && wequipment == null)
				{
					wequipment = WEquipment.FromCode(wequipment3.Code, this, this.Equipments.Count, new bool?(false));
				}
				if (wequipment != null && wequipment2 != null)
				{
					break;
				}
			}
			this.SetEquipments(new List<WEquipment>(2)
			{
				wequipment ?? WEquipment.FromCode(null, this, 0, new bool?(false)),
				wequipment2 ?? WEquipment.FromCode(null, this, 1, new bool?(true))
			});
			this.Normalize();
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00020668 File Offset: 0x0001E868
		public void Normalize()
		{
			this.Troop.UpgradeItemRequirement = this.ComputeUpgradeItemRequirement();
			if (this.Troop.IsCustom)
			{
				if (!this.BattleSets.Any<WEquipment>())
				{
					this.CreateBattleSet();
				}
				if (!this.CivilianSets.Any<WEquipment>())
				{
					this.CreateCivilianSet();
				}
				List<WEquipment> equipments = this.Equipments;
				int num = equipments.FindIndex((WEquipment e) => !e.IsCivilian);
				if (num > 0)
				{
					WEquipment item = equipments[num];
					equipments.RemoveAt(num);
					equipments.Insert(0, item);
					this.SetEquipments(equipments);
				}
			}
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0002070C File Offset: 0x0001E90C
		public void Apply(int setIndex, EquipmentIndex slot, WItem newItem)
		{
			WEquipment wequipment = this.Get(setIndex);
			if (wequipment == null)
			{
				return;
			}
			if (newItem == null)
			{
				wequipment.UnsetItem(slot);
			}
			else
			{
				wequipment.SetItem(slot, newItem);
			}
			if (this.GetCategory(setIndex) == EquipmentCategory.Invalid)
			{
				this.Troop.FormationClass = wequipment.ComputeFormationClass();
			}
			this.Troop.UpgradeItemRequirement = this.ComputeUpgradeItemRequirement();
			foreach (WCharacter wcharacter in this.Troop.UpgradeTargets)
			{
				wcharacter.UpgradeItemRequirement = wcharacter.Loadout.ComputeUpgradeItemRequirement();
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x00020798 File Offset: 0x0001E998
		public List<WItem> Items
		{
			get
			{
				return (from i in this.Equipments.SelectMany((WEquipment eq) => eq.Items)
				where i != null
				select i).ToList<WItem>();
			}
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x000207F8 File Offset: 0x0001E9F8
		public Dictionary<WItem, int> ItemsInSet(int setIndex)
		{
			Dictionary<WItem, int> dictionary = new Dictionary<WItem, int>();
			WEquipment wequipment = this.Get(setIndex);
			if (wequipment == null)
			{
				return dictionary;
			}
			foreach (EquipmentIndex slot in WEquipment.Slots)
			{
				WItem witem = wequipment.Get(slot);
				if (!(witem == null))
				{
					int num;
					dictionary.TryGetValue(witem, out num);
					dictionary[witem] = num + 1;
				}
			}
			return dictionary;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00020884 File Offset: 0x0001EA84
		public int CountEquipped(WItem item)
		{
			if (item == null)
			{
				return 0;
			}
			int num = 0;
			foreach (WEquipment wequipment in this.Equipments)
			{
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					if (wequipment.Get(slot) == item)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x00020930 File Offset: 0x0001EB30
		public bool IsEquippedElsewhere(WItem item, int excludingSetIndex = -1, EquipmentIndex? excludingSlot = null)
		{
			if (item == null)
			{
				return false;
			}
			this.Get(excludingSetIndex);
			for (int i = 0; i < this.Equipments.Count; i++)
			{
				WEquipment wequipment = this.Equipments[i];
				foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
				{
					if ((i != excludingSetIndex || excludingSlot == null || equipmentIndex != excludingSlot.Value) && wequipment.Get(equipmentIndex) == item)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x000209E0 File Offset: 0x0001EBE0
		public int CountInSet(WItem item, int setIndex)
		{
			if (item == null)
			{
				return 0;
			}
			WEquipment wequipment = this.Get(setIndex);
			if (wequipment == null)
			{
				return 0;
			}
			int num = 0;
			foreach (EquipmentIndex slot in WEquipment.Slots)
			{
				if (wequipment.Get(slot) == item)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x00020A5C File Offset: 0x0001EC5C
		public int MaxCountPerSet(WItem item)
		{
			if (item == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.Equipments.Count; i++)
			{
				int num2 = this.CountInSet(item, i);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00020A9C File Offset: 0x0001EC9C
		public Dictionary<WItem, int> RequiredCopies()
		{
			Dictionary<WItem, int> dictionary = new Dictionary<WItem, int>();
			for (int i = 0; i < this.Equipments.Count; i++)
			{
				foreach (KeyValuePair<WItem, int> keyValuePair in this.ItemsInSet(i))
				{
					WItem key = keyValuePair.Key;
					int value = keyValuePair.Value;
					int num;
					if (!dictionary.TryGetValue(key, out num) || value > num)
					{
						dictionary[key] = value;
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00020B34 File Offset: 0x0001ED34
		public int MaxOverOtherSets(WItem item, int excludingSet)
		{
			if (item == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.Equipments.Count; i++)
			{
				if (i != excludingSet)
				{
					int num2 = this.CountInSet(item, i);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00020B78 File Offset: 0x0001ED78
		public int RequiredAfterForItem(WItem item, int setIndex, EquipmentIndex slot, WItem newItem)
		{
			if (item == null)
			{
				return 0;
			}
			WEquipment wequipment = this.Get(setIndex);
			StringIdentifier left = (wequipment != null) ? wequipment.Get(slot) : null;
			int num = this.CountInSet(item, setIndex);
			if (left == item)
			{
				num--;
			}
			if (newItem == item)
			{
				num++;
			}
			int num2 = this.MaxOverOtherSets(item, setIndex);
			if (num <= num2)
			{
				return num2;
			}
			return num;
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00020BD8 File Offset: 0x0001EDD8
		[return: TupleElementNames(new string[]
		{
			"before",
			"after",
			"deltaRemove"
		})]
		public Dictionary<WItem, ValueTuple<int, int, int>> PreviewDeleteSet(int setIndex)
		{
			Dictionary<WItem, ValueTuple<int, int, int>> dictionary = new Dictionary<WItem, ValueTuple<int, int, int>>();
			foreach (WItem witem in new HashSet<WItem>(this.Items))
			{
				int num = this.MaxCountPerSet(witem);
				int num2 = this.MaxOverOtherSets(witem, setIndex);
				int item = (num > num2) ? (num - num2) : 0;
				dictionary[witem] = new ValueTuple<int, int, int>(num, num2, item);
			}
			return dictionary;
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x00020C64 File Offset: 0x0001EE64
		public int ComputeSkillRequirement(SkillObject skill)
		{
			int num = 0;
			foreach (WEquipment wequipment in this.Equipments)
			{
				int num2 = wequipment.ComputeSkillRequirement(skill);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x00020CC0 File Offset: 0x0001EEC0
		public ItemCategory ComputeUpgradeItemRequirement()
		{
			if (!this.Troop.IsCustom && Config.VanillaUpgradeRequirements)
			{
				return this.Troop.UpgradeItemRequirement;
			}
			ItemCategory itemCategory = this.FindBestHorseCategory();
			WCharacter parent = this.Troop.Parent;
			ItemCategory itemCategory2 = (parent != null) ? parent.Loadout.FindBestHorseCategory() : null;
			if (Config.NoNobleHorseUpgradeRequirements)
			{
				if (itemCategory == DefaultItemCategories.NobleHorse)
				{
					itemCategory = DefaultItemCategories.WarHorse;
				}
				if (itemCategory2 == DefaultItemCategories.NobleHorse)
				{
					itemCategory2 = DefaultItemCategories.WarHorse;
				}
			}
			if (!this.IsBetterHorseCategory(itemCategory, itemCategory2))
			{
				return null;
			}
			return itemCategory;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x00020D4B File Offset: 0x0001EF4B
		private bool IsBetterHorseCategory(ItemCategory a, ItemCategory b)
		{
			if (a == DefaultItemCategories.NobleHorse)
			{
				return b != DefaultItemCategories.NobleHorse;
			}
			if (a == DefaultItemCategories.WarHorse)
			{
				return b != DefaultItemCategories.WarHorse;
			}
			return a != null && b == null;
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00020D80 File Offset: 0x0001EF80
		private ItemCategory FindBestHorseCategory()
		{
			ItemCategory itemCategory = null;
			foreach (WEquipment wequipment in this.Equipments)
			{
				if (!wequipment.IsCivilian || !Config.NoCivilianSetUpgradeRequirements)
				{
					WItem witem = wequipment.Get(EquipmentIndex.ArmorItemEndSlot);
					if (witem != null)
					{
						ItemCategory category = witem.Category;
						if (itemCategory == null || this.IsBetterHorseCategory(category, itemCategory))
						{
							itemCategory = category;
						}
					}
				}
			}
			return itemCategory;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00020E10 File Offset: 0x0001F010
		private void ApplyHeroEquipments(List<WEquipment> sets)
		{
			if (!this.IsHero)
			{
				return;
			}
			Hero hero = this.Hero;
			if (hero == null || sets == null || sets.Count == 0)
			{
				return;
			}
			WEquipment wequipment = sets.FirstOrDefault((WEquipment eq) => !eq.IsCivilian) ?? sets.FirstOrDefault<WEquipment>();
			WEquipment wequipment2 = sets.FirstOrDefault((WEquipment eq) => eq.IsCivilian);
			if (wequipment != null)
			{
				WLoadout.CopyEquipment(wequipment.Base, hero.BattleEquipment);
			}
			if (wequipment2 != null)
			{
				WLoadout.CopyEquipment(wequipment2.Base, hero.CivilianEquipment);
			}
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x00020EBC File Offset: 0x0001F0BC
		private static void CopyEquipment(Equipment src, Equipment dst)
		{
			if (src == null || dst == null)
			{
				return;
			}
			for (int i = 0; i < 12; i++)
			{
				dst[i] = src[i];
			}
		}

		// Token: 0x04000193 RID: 403
		private readonly WCharacter _troop = troop;
	}
}
