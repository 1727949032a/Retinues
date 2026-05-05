using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Features.Stocks;
using Retinues.Features.Unlocks;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.ObjectSystem;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000096 RID: 150
	[SafeClass]
	public class WItem : StringIdentifier
	{
		// Token: 0x06000634 RID: 1588 RVA: 0x0001F400 File Offset: 0x0001D600
		public WItem(ItemObject itemObject)
		{
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000635 RID: 1589 RVA: 0x0001F40F File Offset: 0x0001D60F
		public ItemObject Base
		{
			get
			{
				return this._itemObject;
			}
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0001F417 File Offset: 0x0001D617
		public WItem(string itemId) : this(MBObjectManager.Instance.GetObject<ItemObject>(itemId))
		{
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x0001F42A File Offset: 0x0001D62A
		public ItemImageIdentifierVM Image
		{
			get
			{
				return new ItemImageIdentifierVM(this.Base, "");
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x0001F43C File Offset: 0x0001D63C
		public WCulture Culture
		{
			get
			{
				if (this.Base.Culture == null)
				{
					return null;
				}
				return new WCulture(this._itemObject.Culture as CultureObject);
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x0001F462 File Offset: 0x0001D662
		public override string StringId
		{
			get
			{
				return this._itemObject.StringId;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x0001F46F File Offset: 0x0001D66F
		public string Name
		{
			get
			{
				return this._itemObject.Name.ToString();
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x0001F481 File Offset: 0x0001D681
		public bool IsCivilian
		{
			get
			{
				return this._itemObject.IsCivilian;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0001F48E File Offset: 0x0001D68E
		public int Value
		{
			get
			{
				return this._itemObject.Value;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x0001F49B File Offset: 0x0001D69B
		public ItemCategory Category
		{
			get
			{
				return this._itemObject.ItemCategory;
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0001F4A8 File Offset: 0x0001D6A8
		public ItemObject.ItemTypeEnum Type
		{
			get
			{
				return this._itemObject.ItemType;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x0001F4B5 File Offset: 0x0001D6B5
		public SkillObject RelevantSkill
		{
			get
			{
				return this._itemObject.RelevantSkill;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0001F4C2 File Offset: 0x0001D6C2
		public int Difficulty
		{
			get
			{
				return this._itemObject.Difficulty;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x0001F4D0 File Offset: 0x0001D6D0
		public List<EquipmentIndex> Slots
		{
			get
			{
				WItem.<>c__DisplayClass26_0 CS$<>8__locals1;
				CS$<>8__locals1.slots = new List<EquipmentIndex>();
				switch (this.Type)
				{
				case ItemObject.ItemTypeEnum.Horse:
					CS$<>8__locals1.slots.Add(EquipmentIndex.ArmorItemEndSlot);
					break;
				case ItemObject.ItemTypeEnum.OneHandedWeapon:
				case ItemObject.ItemTypeEnum.TwoHandedWeapon:
				case ItemObject.ItemTypeEnum.Polearm:
				case ItemObject.ItemTypeEnum.Arrows:
				case ItemObject.ItemTypeEnum.Bolts:
				case ItemObject.ItemTypeEnum.SlingStones:
				case ItemObject.ItemTypeEnum.Shield:
				case ItemObject.ItemTypeEnum.Bow:
				case ItemObject.ItemTypeEnum.Crossbow:
				case ItemObject.ItemTypeEnum.Sling:
				case ItemObject.ItemTypeEnum.Thrown:
				case ItemObject.ItemTypeEnum.Pistol:
				case ItemObject.ItemTypeEnum.Musket:
				case ItemObject.ItemTypeEnum.Bullets:
					WItem.<get_Slots>g__AddWeaponSlots|26_0(ref CS$<>8__locals1);
					break;
				case ItemObject.ItemTypeEnum.HeadArmor:
					CS$<>8__locals1.slots.Add(EquipmentIndex.NumAllWeaponSlots);
					break;
				case ItemObject.ItemTypeEnum.BodyArmor:
					CS$<>8__locals1.slots.Add(EquipmentIndex.Body);
					break;
				case ItemObject.ItemTypeEnum.LegArmor:
					CS$<>8__locals1.slots.Add(EquipmentIndex.Leg);
					break;
				case ItemObject.ItemTypeEnum.HandArmor:
					CS$<>8__locals1.slots.Add(EquipmentIndex.Gloves);
					break;
				case ItemObject.ItemTypeEnum.Cape:
					CS$<>8__locals1.slots.Add(EquipmentIndex.Cape);
					break;
				case ItemObject.ItemTypeEnum.HorseHarness:
					CS$<>8__locals1.slots.Add(EquipmentIndex.HorseHarness);
					break;
				}
				return CS$<>8__locals1.slots;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x0001F5D0 File Offset: 0x0001D7D0
		public ItemComponent ItemComponent
		{
			get
			{
				return this._itemObject.ItemComponent;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x0001F5DD File Offset: 0x0001D7DD
		public ArmorComponent ArmorComponent
		{
			get
			{
				return this._itemObject.ArmorComponent;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x0001F5EA File Offset: 0x0001D7EA
		public HorseComponent HorseComponent
		{
			get
			{
				return this._itemObject.HorseComponent;
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000645 RID: 1605 RVA: 0x0001F5F7 File Offset: 0x0001D7F7
		public WeaponComponent WeaponComponent
		{
			get
			{
				return this._itemObject.WeaponComponent;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x0001F604 File Offset: 0x0001D804
		public WeaponComponentData PrimaryWeapon
		{
			get
			{
				return this._itemObject.PrimaryWeapon;
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x0001F614 File Offset: 0x0001D814
		private static HashSet<string> VassalRewardItemIds
		{
			get
			{
				if (WItem._vassalRewardItemIdsCache == null)
				{
					WItem._vassalRewardItemIdsCache = new HashSet<string>();
					foreach (WCulture wculture in WCulture.All)
					{
						foreach (ItemObject itemObject in wculture.Base.VassalRewardItems)
						{
							WItem._vassalRewardItemIdsCache.Add(itemObject.StringId);
						}
					}
				}
				return WItem._vassalRewardItemIdsCache;
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x0001F6C0 File Offset: 0x0001D8C0
		public bool IsVassalRewardItem
		{
			get
			{
				return WItem.VassalRewardItemIds.Contains(this.StringId);
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000649 RID: 1609 RVA: 0x0001F6D2 File Offset: 0x0001D8D2
		public bool IsCrafted
		{
			get
			{
				return this._itemObject.IsCraftedByPlayer && this.Base.WeaponDesign != null;
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x0001F6F4 File Offset: 0x0001D8F4
		public string CraftedCode
		{
			get
			{
				if (this.IsCrafted)
				{
					WeaponDesign weaponDesign = this.Base.WeaponDesign;
					if (((weaponDesign != null) ? weaponDesign.UsedPieces : null) != null)
					{
						string arg = string.Join(":", this.Base.WeaponDesign.UsedPieces.Select(delegate(WeaponDesignElement p)
						{
							if (p == null)
							{
								return null;
							}
							CraftingPiece craftingPiece = p.CraftingPiece;
							if (craftingPiece == null)
							{
								return null;
							}
							return craftingPiece.StringId;
						}));
						return string.Format("{0}:{1}:{2}", this.Name, arg, this.Value);
					}
				}
				return null;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600064B RID: 1611 RVA: 0x0001F77F File Offset: 0x0001D97F
		public bool IsArmor
		{
			get
			{
				return this.ArmorComponent != null && ItemObject.ItemTypeEnum.HorseHarness != this.Type;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x0001F798 File Offset: 0x0001D998
		public bool IsHorse
		{
			get
			{
				return this.HorseComponent != null;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600064D RID: 1613 RVA: 0x0001F7A3 File Offset: 0x0001D9A3
		public bool IsWeapon
		{
			get
			{
				return this.WeaponComponent != null && this.PrimaryWeapon != null;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x0001F7B8 File Offset: 0x0001D9B8
		public bool IsShield
		{
			get
			{
				WeaponComponentData primaryWeapon = this.PrimaryWeapon;
				return primaryWeapon != null && primaryWeapon.IsShield;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x0600064F RID: 1615 RVA: 0x0001F7CB File Offset: 0x0001D9CB
		public bool IsRangedWeapon
		{
			get
			{
				WeaponComponentData primaryWeapon = this.PrimaryWeapon;
				return primaryWeapon != null && primaryWeapon.IsRangedWeapon;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x0001F7DE File Offset: 0x0001D9DE
		public bool IsMeleeWeapon
		{
			get
			{
				WeaponComponentData primaryWeapon = this.PrimaryWeapon;
				return primaryWeapon != null && primaryWeapon.IsMeleeWeapon;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000651 RID: 1617 RVA: 0x0001F7F1 File Offset: 0x0001D9F1
		public bool IsAmmo
		{
			get
			{
				WeaponComponentData primaryWeapon = this.PrimaryWeapon;
				return primaryWeapon != null && primaryWeapon.IsAmmo;
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x0001F804 File Offset: 0x0001DA04
		public int Tier
		{
			get
			{
				return Array.IndexOf(Enum.GetValues(this._itemObject.Tier.GetType()), this._itemObject.Tier) + 1;
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x0001F838 File Offset: 0x0001DA38
		public string Class
		{
			get
			{
				if (this.IsArmor)
				{
					return Format.CamelCaseToTitle(this.Type.ToString());
				}
				if (this.IsHorse)
				{
					return Format.CamelCaseToTitle(this.Category.ToString());
				}
				if (this.IsWeapon)
				{
					string text = Format.CamelCaseToTitle(this.PrimaryWeapon.WeaponClass.ToString());
					if (this.IsAmmo && !text.EndsWith("s"))
					{
						text += "s";
					}
					return text;
				}
				return Format.CamelCaseToTitle(this.Type.ToString());
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x0001F8E3 File Offset: 0x0001DAE3
		public bool IsUnlocked
		{
			get
			{
				return UnlocksBehavior.IsUnlocked(this.StringId);
			}
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0001F8F0 File Offset: 0x0001DAF0
		public void Unlock()
		{
			UnlocksBehavior.Unlock(this.Base);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0001F8FD File Offset: 0x0001DAFD
		public void Lock()
		{
			UnlocksBehavior.Lock(this.Base);
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000657 RID: 1623 RVA: 0x0001F90A File Offset: 0x0001DB0A
		public bool UnlockInProgress
		{
			get
			{
				return UnlocksBehavior.InProgress(this.StringId);
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000658 RID: 1624 RVA: 0x0001F917 File Offset: 0x0001DB17
		public int UnlockProgress
		{
			get
			{
				return UnlocksBehavior.GetProgress(this.StringId);
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000659 RID: 1625 RVA: 0x0001F924 File Offset: 0x0001DB24
		public bool IsStocked
		{
			get
			{
				return StocksBehavior.HasStock(this.StringId);
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0001F931 File Offset: 0x0001DB31
		public int GetStock()
		{
			return StocksBehavior.Get(this.StringId);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0001F93E File Offset: 0x0001DB3E
		public void Stock()
		{
			StocksBehavior.Add(this.StringId, 1);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001F94C File Offset: 0x0001DB4C
		public void Unstock()
		{
			StocksBehavior.Add(this.StringId, -1);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0001F95C File Offset: 0x0001DB5C
		private static string GetChevronCacheKey(WItem a, WItem b)
		{
			string str = ((a != null) ? a.StringId : null) ?? "__NULL_A__";
			string str2 = ((b != null) ? b.StringId : null) ?? "__NULL_B__";
			return str + "=>" + str2;
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0001F9A0 File Offset: 0x0001DBA0
		public void GetComparisonChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			positiveChevrons = 0;
			negativeChevrons = 0;
			if (other == null)
			{
				return;
			}
			if (other == this)
			{
				return;
			}
			string chevronCacheKey = WItem.GetChevronCacheKey(this, other);
			WItem.ChevronCacheEntry chevronCacheEntry;
			if (WItem._chevronCache.TryGetValue(chevronCacheKey, out chevronCacheEntry))
			{
				positiveChevrons = chevronCacheEntry.Positive;
				negativeChevrons = chevronCacheEntry.Negative;
				return;
			}
			this.ComputeComparisonChevronScore(other, out positiveChevrons, out negativeChevrons);
			WItem._chevronCache[chevronCacheKey] = new WItem.ChevronCacheEntry
			{
				Positive = positiveChevrons,
				Negative = negativeChevrons
			};
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0001FA20 File Offset: 0x0001DC20
		private void ComputeComparisonChevronScore(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			positiveChevrons = 0;
			negativeChevrons = 0;
			if (this.IsWeapon && other.IsWeapon)
			{
				WeaponComponentData primaryWeapon = this.PrimaryWeapon;
				WeaponComponentData primaryWeapon2 = other.PrimaryWeapon;
				if (primaryWeapon == null || primaryWeapon2 == null)
				{
					return;
				}
				if (primaryWeapon.WeaponClass != primaryWeapon2.WeaponClass)
				{
					return;
				}
				if (this.IsShield || other.IsShield)
				{
					if (!this.IsShield || !other.IsShield)
					{
						return;
					}
					this.CompareShieldChevrons(other, out positiveChevrons, out negativeChevrons);
					return;
				}
				else if (this.IsAmmo || other.IsAmmo)
				{
					if (!this.IsAmmo || !other.IsAmmo)
					{
						return;
					}
					this.CompareAmmoChevrons(other, out positiveChevrons, out negativeChevrons);
					return;
				}
				else
				{
					if (this.IsRangedWeapon && other.IsRangedWeapon)
					{
						this.CompareRangedWeaponChevrons(other, out positiveChevrons, out negativeChevrons);
						return;
					}
					if (this.IsMeleeWeapon && other.IsMeleeWeapon)
					{
						this.CompareMeleeWeaponChevrons(other, out positiveChevrons, out negativeChevrons);
						return;
					}
					return;
				}
			}
			else if (this.ArmorComponent != null && other.ArmorComponent != null && this.Type != ItemObject.ItemTypeEnum.HorseHarness && other.Type != ItemObject.ItemTypeEnum.HorseHarness)
			{
				if (this.Type != other.Type)
				{
					return;
				}
				this.CompareArmorChevrons(other, out positiveChevrons, out negativeChevrons);
				return;
			}
			else
			{
				if (this.Type == ItemObject.ItemTypeEnum.HorseHarness && other.Type == ItemObject.ItemTypeEnum.HorseHarness && this.ArmorComponent != null && other.ArmorComponent != null)
				{
					this.CompareHorseHarnessChevrons(other, out positiveChevrons, out negativeChevrons);
					return;
				}
				if (this.IsHorse && other.IsHorse && this.HorseComponent != null && other.HorseComponent != null)
				{
					this.CompareHorseChevrons(other, out positiveChevrons, out negativeChevrons);
					return;
				}
				return;
			}
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0001FB8D File Offset: 0x0001DD8D
		private static void AccumulateStatComparison(int thisValue, int otherValue, bool higherIsBetter, ref int better, ref int worse)
		{
			if (thisValue == otherValue)
			{
				return;
			}
			if (higherIsBetter ? (thisValue > otherValue) : (thisValue < otherValue))
			{
				better++;
				return;
			}
			worse++;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0001FBB2 File Offset: 0x0001DDB2
		private static void GetChevronsFromCounts(int better, int worse, out int positiveChevrons, out int negativeChevrons)
		{
			positiveChevrons = 0;
			negativeChevrons = 0;
			if (better + worse == 0)
			{
				return;
			}
			if (better == worse)
			{
				return;
			}
			if (worse == 0)
			{
				positiveChevrons = 3;
				negativeChevrons = 0;
				return;
			}
			if (better == 0)
			{
				positiveChevrons = 0;
				negativeChevrons = 3;
				return;
			}
			if (better > worse)
			{
				positiveChevrons = 2;
				negativeChevrons = 1;
				return;
			}
			positiveChevrons = 1;
			negativeChevrons = 2;
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0001FBEC File Offset: 0x0001DDEC
		private void CompareMeleeWeaponChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			WeaponComponentData primaryWeapon = this.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = other.PrimaryWeapon;
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(primaryWeapon.SwingDamage, primaryWeapon2.SwingDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.ThrustDamage, primaryWeapon2.ThrustDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.SwingSpeed, primaryWeapon2.SwingSpeed, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.ThrustSpeed, primaryWeapon2.ThrustSpeed, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.WeaponLength, primaryWeapon2.WeaponLength, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.Handling, primaryWeapon2.Handling, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0001FC98 File Offset: 0x0001DE98
		private void CompareRangedWeaponChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			WeaponComponentData primaryWeapon = this.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = other.PrimaryWeapon;
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(primaryWeapon.SwingDamage, primaryWeapon2.SwingDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.ThrustDamage, primaryWeapon2.ThrustDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.MissileSpeed, primaryWeapon2.MissileSpeed, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.Accuracy, primaryWeapon2.Accuracy, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0001FD18 File Offset: 0x0001DF18
		private void CompareAmmoChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			WeaponComponentData primaryWeapon = this.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = other.PrimaryWeapon;
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(primaryWeapon.SwingDamage, primaryWeapon2.SwingDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.ThrustDamage, primaryWeapon2.ThrustDamage, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.MissileSpeed, primaryWeapon2.MissileSpeed, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.Accuracy, primaryWeapon2.Accuracy, true, ref better, ref worse);
			int maxDataValue = (int)primaryWeapon.MaxDataValue;
			int maxDataValue2 = (int)primaryWeapon2.MaxDataValue;
			WItem.AccumulateStatComparison(maxDataValue, maxDataValue2, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0001FDB0 File Offset: 0x0001DFB0
		private void CompareShieldChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			WeaponComponentData primaryWeapon = this.PrimaryWeapon;
			WeaponComponentData primaryWeapon2 = other.PrimaryWeapon;
			int better = 0;
			int worse = 0;
			int maxDataValue = (int)primaryWeapon.MaxDataValue;
			int maxDataValue2 = (int)primaryWeapon2.MaxDataValue;
			WItem.AccumulateStatComparison(maxDataValue, maxDataValue2, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.BodyArmor, primaryWeapon2.BodyArmor, true, ref better, ref worse);
			WItem.AccumulateStatComparison(primaryWeapon.Handling, primaryWeapon2.Handling, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0001FE1C File Offset: 0x0001E01C
		private void CompareArmorChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			ArmorComponent armorComponent = this.ArmorComponent;
			ArmorComponent armorComponent2 = other.ArmorComponent;
			if (armorComponent == null || armorComponent2 == null)
			{
				positiveChevrons = 0;
				negativeChevrons = 0;
				return;
			}
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(armorComponent.HeadArmor, armorComponent2.HeadArmor, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.BodyArmor, armorComponent2.BodyArmor, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.ArmArmor, armorComponent2.ArmArmor, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.LegArmor, armorComponent2.LegArmor, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001FEAC File Offset: 0x0001E0AC
		private void CompareHorseHarnessChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			ArmorComponent armorComponent = this.ArmorComponent;
			ArmorComponent armorComponent2 = other.ArmorComponent;
			if (armorComponent == null || armorComponent2 == null)
			{
				positiveChevrons = 0;
				negativeChevrons = 0;
				return;
			}
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(armorComponent.BodyArmor, armorComponent2.BodyArmor, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.ManeuverBonus, armorComponent2.ManeuverBonus, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.SpeedBonus, armorComponent2.SpeedBonus, true, ref better, ref worse);
			WItem.AccumulateStatComparison(armorComponent.ChargeBonus, armorComponent2.ChargeBonus, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0001FF3C File Offset: 0x0001E13C
		private void CompareHorseChevrons(WItem other, out int positiveChevrons, out int negativeChevrons)
		{
			HorseComponent horseComponent = this.HorseComponent;
			HorseComponent horseComponent2 = other.HorseComponent;
			if (horseComponent == null || horseComponent2 == null)
			{
				positiveChevrons = 0;
				negativeChevrons = 0;
				return;
			}
			int better = 0;
			int worse = 0;
			WItem.AccumulateStatComparison(horseComponent.Speed, horseComponent2.Speed, true, ref better, ref worse);
			WItem.AccumulateStatComparison(horseComponent.Maneuver, horseComponent2.Maneuver, true, ref better, ref worse);
			WItem.AccumulateStatComparison(horseComponent.ChargeDamage, horseComponent2.ChargeDamage, true, ref better, ref worse);
			int thisValue = horseComponent.HitPoints + horseComponent.HitPointBonus;
			int otherValue = horseComponent2.HitPoints + horseComponent2.HitPointBonus;
			WItem.AccumulateStatComparison(thisValue, otherValue, true, ref better, ref worse);
			WItem.GetChevronsFromCounts(better, worse, out positiveChevrons, out negativeChevrons);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0001FFE7 File Offset: 0x0001E1E7
		[CompilerGenerated]
		internal static void <get_Slots>g__AddWeaponSlots|26_0(ref WItem.<>c__DisplayClass26_0 A_0)
		{
			A_0.slots.Add(EquipmentIndex.WeaponItemBeginSlot);
			A_0.slots.Add(EquipmentIndex.Weapon1);
			A_0.slots.Add(EquipmentIndex.Weapon2);
			A_0.slots.Add(EquipmentIndex.Weapon3);
		}

		// Token: 0x0400018C RID: 396
		private readonly ItemObject _itemObject = itemObject;

		// Token: 0x0400018D RID: 397
		private static HashSet<string> _vassalRewardItemIdsCache;

		// Token: 0x0400018E RID: 398
		private static readonly ConcurrentDictionary<string, WItem.ChevronCacheEntry> _chevronCache = new ConcurrentDictionary<string, WItem.ChevronCacheEntry>();

		// Token: 0x0200016B RID: 363
		private struct ChevronCacheEntry
		{
			// Token: 0x04000446 RID: 1094
			public int Positive;

			// Token: 0x04000447 RID: 1095
			public int Negative;
		}
	}
}
