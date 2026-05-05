using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Features.Staging;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000093 RID: 147
	[SafeClass]
	public class WEquipment
	{
		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001E2B9 File Offset: 0x0001C4B9
		public Equipment Base
		{
			get
			{
				return this._equipment;
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0001E2C1 File Offset: 0x0001C4C1
		public WEquipment(Equipment equipment, WLoadout loadout, int index)
		{
			this._equipment = equipment;
			this.Loadout = loadout;
			this.Index = index;
			WEquipment.SanitizeArmor(this._equipment);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0001E2E9 File Offset: 0x0001C4E9
		public WEquipment(Equipment equipment, WCharacter troop, int index) : this(equipment, troop.Loadout, index)
		{
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0001E2FC File Offset: 0x0001C4FC
		public static void SanitizeArmor(Equipment eq)
		{
			if (eq == null)
			{
				return;
			}
			EquipmentIndex[] array = new EquipmentIndex[4];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.F3E0813C89D0991F07FBB5027A68C0CB809E1EFFC0C1E8974664BD2E57B50024).FieldHandle);
			foreach (EquipmentIndex index in array)
			{
				ItemObject item = eq[index].Item;
				if (item != null && item.ArmorComponent == null)
				{
					eq[index] = default(EquipmentElement);
				}
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001E360 File Offset: 0x0001C560
		// (set) Token: 0x060005D9 RID: 1497 RVA: 0x0001E368 File Offset: 0x0001C568
		public WLoadout Loadout { get; private set; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x0001E371 File Offset: 0x0001C571
		// (set) Token: 0x060005DB RID: 1499 RVA: 0x0001E379 File Offset: 0x0001C579
		public int Index { get; private set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x0001E382 File Offset: 0x0001C582
		public EquipmentCategory Category
		{
			get
			{
				return this.Loadout.GetCategory(this.Index);
			}
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0001E398 File Offset: 0x0001C598
		public void SetCivilian(bool makeCivilian)
		{
			try
			{
				Equipment.EquipmentType equipmentType = makeCivilian ? Equipment.EquipmentType.Civilian : Equipment.EquipmentType.Battle;
				Reflector.SetFieldValue(this._equipment, "_equipmentType", equipmentType);
			}
			catch
			{
			}
			this.Loadout.Troop.NeedsPersistence = true;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x0001E3EC File Offset: 0x0001C5EC
		public static WEquipment FromCode(string code, WLoadout loadout, int index, bool? forceCivilian = null)
		{
			Equipment equipment;
			if (code == null)
			{
				equipment = new Equipment(Equipment.EquipmentType.Battle);
			}
			else
			{
				equipment = Equipment.CreateFromEquipmentCode(code);
			}
			WEquipment wequipment = new WEquipment(equipment, loadout, index);
			if (forceCivilian != null)
			{
				wequipment.SetCivilian(forceCivilian.Value);
			}
			return wequipment;
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x060005DF RID: 1503 RVA: 0x0001E42C File Offset: 0x0001C62C
		public string Code
		{
			get
			{
				return this.Base.CalculateEquipmentCode();
			}
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0001E439 File Offset: 0x0001C639
		public static bool IsArmorSlot(EquipmentIndex slot)
		{
			return slot == EquipmentIndex.NumAllWeaponSlots || slot == EquipmentIndex.Cape || slot == EquipmentIndex.Body || slot == EquipmentIndex.Gloves || slot == EquipmentIndex.Leg;
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x0001E454 File Offset: 0x0001C654
		public List<WItem> Items
		{
			get
			{
				List<WItem> list = new List<WItem>();
				foreach (EquipmentIndex index in WEquipment.Slots)
				{
					if (this._equipment[index].Item != null)
					{
						list.Add(new WItem(this._equipment[index].Item));
					}
				}
				return list;
			}
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0001E4DC File Offset: 0x0001C6DC
		public WItem Get(EquipmentIndex slot)
		{
			ItemObject item = this._equipment[slot].Item;
			if (item != null)
			{
				return new WItem(item);
			}
			return null;
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0001E50C File Offset: 0x0001C70C
		public void SetItem(EquipmentIndex slot, WItem item)
		{
			if (item == null)
			{
				this._equipment[slot] = new EquipmentElement(null, null, null, false);
				return;
			}
			if (WEquipment.IsArmorSlot(slot))
			{
				ItemObject @base = item.Base;
				if (((@base != null) ? @base.ArmorComponent : null) == null)
				{
					string format = "Attempted to place non-armor item '{0}' into {1}; clearing slot instead.";
					ItemObject base2 = item.Base;
					Log.Warn(string.Format(format, (base2 != null) ? base2.Name : null, slot));
					this._equipment[slot] = new EquipmentElement(null, null, null, false);
					return;
				}
			}
			this._equipment[slot] = new EquipmentElement(item.Base, null, null, false);
			this.Loadout.Troop.NeedsPersistence = true;
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x0001E5BC File Offset: 0x0001C7BC
		public void UnsetItem(EquipmentIndex slot)
		{
			this._equipment[slot] = new EquipmentElement(null, null, null, false);
			this.Loadout.Troop.NeedsPersistence = true;
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x0001E5E4 File Offset: 0x0001C7E4
		public void UnsetAll()
		{
			foreach (EquipmentIndex slot in WEquipment.Slots)
			{
				this.UnsetItem(slot);
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x0001E638 File Offset: 0x0001C838
		public Dictionary<SkillObject, int> SkillRequirements
		{
			get
			{
				Dictionary<SkillObject, int> dictionary = new Dictionary<SkillObject, int>();
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					WItem witem = this.Get(slot);
					if (witem != null && witem.RelevantSkill != null)
					{
						if (!dictionary.ContainsKey(witem.RelevantSkill))
						{
							dictionary[witem.RelevantSkill] = 0;
						}
						if (witem.Difficulty > dictionary[witem.RelevantSkill])
						{
							dictionary[witem.RelevantSkill] = witem.Difficulty;
						}
					}
				}
				return dictionary;
			}
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0001E6E4 File Offset: 0x0001C8E4
		public int ComputeSkillRequirement(SkillObject skill)
		{
			int result;
			if (!this.SkillRequirements.TryGetValue(skill, out result))
			{
				return 0;
			}
			return result;
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0001E704 File Offset: 0x0001C904
		public FormationClass ComputeFormationClass()
		{
			bool hasNonThrowableRangedWeapons = this.HasNonThrowableRangedWeapons;
			bool hasMount = this.HasMount;
			FormationClass result;
			if (hasNonThrowableRangedWeapons)
			{
				if (!hasMount)
				{
					result = FormationClass.Ranged;
				}
				else
				{
					result = FormationClass.HorseArcher;
				}
			}
			else if (!hasMount)
			{
				result = FormationClass.Infantry;
			}
			else
			{
				result = FormationClass.Cavalry;
			}
			return result;
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x0001E739 File Offset: 0x0001C939
		public bool IsCivilian
		{
			get
			{
				return this._equipment.IsCivilian;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x0001E748 File Offset: 0x0001C948
		public bool HasRangedWeapons
		{
			get
			{
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					WItem witem = this.Get(slot);
					if (witem != null && witem.IsRangedWeapon)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x0001E7B4 File Offset: 0x0001C9B4
		public bool HasNonThrowableRangedWeapons
		{
			get
			{
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					WItem witem = this.Get(slot);
					if (witem != null && witem.IsRangedWeapon && witem.Type != ItemObject.ItemTypeEnum.Thrown)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x0001E82C File Offset: 0x0001CA2C
		public bool HasMount
		{
			get
			{
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					WItem witem = this.Get(slot);
					if (witem != null && witem.IsHorse)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0001E898 File Offset: 0x0001CA98
		public Equipment StagingPreview()
		{
			Equipment equipment = new Equipment(this.Base);
			List<PendingEquipData> list = EquipStagingBehavior.Get(this.Loadout.Troop);
			if (list == null)
			{
				return equipment;
			}
			foreach (PendingEquipData pendingEquipData in list)
			{
				if (pendingEquipData.EquipmentIndex == this.Index)
				{
					ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(pendingEquipData.ItemId);
					if (@object != null)
					{
						equipment[pendingEquipData.Slot] = new EquipmentElement(@object, null, null, false);
					}
				}
			}
			return equipment;
		}

		// Token: 0x0400017B RID: 379
		private readonly Equipment _equipment;

		// Token: 0x0400017E RID: 382
		public static readonly List<EquipmentIndex> Slots = new List<EquipmentIndex>(11)
		{
			EquipmentIndex.NumAllWeaponSlots,
			EquipmentIndex.Cape,
			EquipmentIndex.Body,
			EquipmentIndex.Gloves,
			EquipmentIndex.Leg,
			EquipmentIndex.WeaponItemBeginSlot,
			EquipmentIndex.Weapon1,
			EquipmentIndex.Weapon2,
			EquipmentIndex.Weapon3,
			EquipmentIndex.ArmorItemEndSlot,
			EquipmentIndex.HorseHarness
		};
	}
}
