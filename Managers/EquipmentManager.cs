using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Equipments;
using Retinues.Features.Staging;
using Retinues.Features.Unlocks;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Retinues.Managers
{
	// Token: 0x0200005D RID: 93
	[SafeClass]
	public static class EquipmentManager
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x0000C3B8 File Offset: 0x0000A5B8
		[return: TupleElementNames(new string[]
		{
			"item",
			"isAvailable",
			"isUnlocked",
			"progress"
		})]
		public static List<ValueTuple<WItem, bool, bool, int>> CollectAvailableItems(BaseFaction faction, EquipmentIndex slot, [TupleElementNames(new string[]
		{
			"item",
			"unlocked",
			"progress"
		})] List<ValueTuple<WItem, bool, int>> cache = null, bool craftedOnly = false)
		{
			if (cache == null)
			{
				Log.Info(string.Format("Building equipment eligibility cache for slot {0}", slot));
			}
			else
			{
				Log.Info(string.Format("Using provided equipment eligibility cache for slot {0}", slot));
			}
			if (craftedOnly)
			{
				if (!DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>() && !ClanScreen.IsStudioMode)
				{
					return new List<ValueTuple<WItem, bool, bool, int>>();
				}
				cache = null;
			}
			object obj = cache ?? EquipmentManager.BuildEligibilityList(faction, slot, craftedOnly);
			HashSet<string> hashSet = null;
			if (!craftedOnly && !ClanScreen.IsStudioMode && Config.RestrictItemsToTownInventory)
			{
				hashSet = EquipmentManager.BuildCurrentTownAvailabilitySet();
			}
			object obj2 = obj;
			List<ValueTuple<WItem, bool, bool, int>> list = new List<ValueTuple<WItem, bool, bool, int>>(obj2.Count);
			foreach (ValueTuple<WItem, bool, int> valueTuple in obj2)
			{
				WItem item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				int item3 = valueTuple.Item3;
				bool item4 = hashSet == null || hashSet.Contains(item.StringId);
				list.Add(new ValueTuple<WItem, bool, bool, int>(item, item4, item2, item3));
			}
			return list;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000C4BC File Offset: 0x0000A6BC
		[return: TupleElementNames(new string[]
		{
			"item",
			"unlocked",
			"progress"
		})]
		private static List<ValueTuple<WItem, bool, int>> BuildEligibilityList(BaseFaction faction, EquipmentIndex slot, bool craftedOnly)
		{
			bool flag = DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>() || ClanScreen.IsStudioMode;
			bool flag2 = DoctrineAPI.IsDoctrineUnlocked<AncestralHeritage>();
			string text;
			if (faction == null)
			{
				text = null;
			}
			else
			{
				WCulture culture = faction.Culture;
				text = ((culture != null) ? culture.StringId : null);
			}
			string b = text;
			WFaction clan = Player.Clan;
			string text2;
			if (clan == null)
			{
				text2 = null;
			}
			else
			{
				WCulture culture2 = clan.Culture;
				text2 = ((culture2 != null) ? culture2.StringId : null);
			}
			string b2 = text2;
			WFaction kingdom = Player.Kingdom;
			string text3;
			if (kingdom == null)
			{
				text3 = null;
			}
			else
			{
				WCulture culture3 = kingdom.Culture;
				text3 = ((culture3 != null) ? culture3.StringId : null);
			}
			string b3 = text3;
			List<ItemObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<ItemObject>();
			List<ValueTuple<WItem, bool, int>> list = new List<ValueTuple<WItem, bool, int>>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ItemObject itemObject in objectTypeList)
			{
				WItem witem = new WItem(itemObject);
				try
				{
					if (craftedOnly)
					{
						if (witem.IsCrafted)
						{
							if (flag)
							{
								if (witem.Slots.Contains(slot))
								{
									if (witem.CraftedCode != null && !hashSet.Contains(witem.CraftedCode))
									{
										list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
										hashSet.Add(witem.CraftedCode);
									}
								}
							}
						}
					}
					else if (!witem.IsCrafted)
					{
						if (Config.AllEquipmentUnlocked || ClanScreen.IsStudioMode)
						{
							if (witem.Slots.Contains(slot))
							{
								list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
							}
						}
						else if (witem.IsUnlocked)
						{
							if (witem.Slots.Contains(slot))
							{
								list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
							}
						}
						else
						{
							WCulture culture4 = witem.Culture;
							string a = (culture4 != null) ? culture4.StringId : null;
							int num;
							if (Config.AllCultureEquipmentUnlocked && a == b)
							{
								if (witem.Slots.Contains(slot))
								{
									list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
								}
							}
							else if (flag2 && (a == b2 || a == b3))
							{
								if (witem.Slots.Contains(slot))
								{
									list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
								}
							}
							else if (Config.UnlockItemsFromKills && UnlocksBehavior.Instance.ProgressByItemId.TryGetValue(witem.StringId, out num))
							{
								if (num >= Config.RequiredKillsPerItem)
								{
									witem.Unlock();
									if (witem.Slots.Contains(slot))
									{
										list.Add(new ValueTuple<WItem, bool, int>(witem, true, 0));
									}
								}
								else if (witem.Slots.Contains(slot))
								{
									list.Add(new ValueTuple<WItem, bool, int>(witem, false, num));
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}
			return list;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000C7A8 File Offset: 0x0000A9A8
		private static HashSet<string> BuildCurrentTownAvailabilitySet()
		{
			if (Player.CurrentSettlement == null)
			{
				return null;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			foreach (ValueTuple<WItem, int> valueTuple in Player.CurrentSettlement.ItemCounts())
			{
				WItem item = valueTuple.Item1;
				if (valueTuple.Item2 > 0)
				{
					hashSet.Add(item.StringId);
				}
			}
			return hashSet;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000C830 File Offset: 0x0000AA30
		public static bool IsUnlockedForPaste(WCharacter troop, EquipmentIndex slot, WItem item)
		{
			if (item == null)
			{
				return true;
			}
			if (!item.Slots.Contains(slot))
			{
				return false;
			}
			if (Config.AllEquipmentUnlocked || ClanScreen.IsStudioMode)
			{
				return true;
			}
			bool flag = DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>() || ClanScreen.IsStudioMode;
			bool flag2 = DoctrineAPI.IsDoctrineUnlocked<AncestralHeritage>();
			string text;
			if (troop == null)
			{
				text = null;
			}
			else
			{
				BaseFaction faction = troop.Faction;
				if (faction == null)
				{
					text = null;
				}
				else
				{
					WCulture culture = faction.Culture;
					text = ((culture != null) ? culture.StringId : null);
				}
			}
			string b = text;
			WFaction clan = Player.Clan;
			string text2;
			if (clan == null)
			{
				text2 = null;
			}
			else
			{
				WCulture culture2 = clan.Culture;
				text2 = ((culture2 != null) ? culture2.StringId : null);
			}
			string b2 = text2;
			WFaction kingdom = Player.Kingdom;
			string text3;
			if (kingdom == null)
			{
				text3 = null;
			}
			else
			{
				WCulture culture3 = kingdom.Culture;
				text3 = ((culture3 != null) ? culture3.StringId : null);
			}
			string b3 = text3;
			bool result;
			try
			{
				if (item.IsCrafted)
				{
					if (!flag)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else if (item.IsUnlocked)
				{
					result = true;
				}
				else
				{
					WCulture culture4 = item.Culture;
					string a = (culture4 != null) ? culture4.StringId : null;
					int num;
					if (Config.AllCultureEquipmentUnlocked && a == b)
					{
						result = true;
					}
					else if (flag2 && (a == b2 || a == b3))
					{
						result = true;
					}
					else if (Config.UnlockItemsFromKills && UnlocksBehavior.Instance.ProgressByItemId.TryGetValue(item.StringId, out num))
					{
						if (num >= Config.RequiredKillsPerItem)
						{
							item.Unlock();
							result = true;
						}
						else
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = false;
			}
			return result;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000C9C8 File Offset: 0x0000ABC8
		public static bool CanEquip(WCharacter troop, WItem item)
		{
			EquipmentManager.EquipLimitReason equipLimitReason;
			return EquipmentManager.CanEquip(troop, item, out equipLimitReason);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000C9E0 File Offset: 0x0000ABE0
		public static bool CanEquip(WCharacter troop, WItem item, out EquipmentManager.EquipLimitReason reasons)
		{
			reasons = EquipmentManager.EquipLimitReason.None;
			if (troop == null)
			{
				return false;
			}
			if (item != null && item.RelevantSkill != null && !EquipmentManager.MeetsItemSkillRequirements(troop, item))
			{
				reasons |= EquipmentManager.EquipLimitReason.Skill;
			}
			if (Config.DisallowMountsForT1Troops && !troop.IsHero && troop.Tier <= 1 && item != null && item.IsHorse)
			{
				reasons |= EquipmentManager.EquipLimitReason.MountT1;
			}
			if (((item != null) ? item.Tier : 0) - troop.Tier > Config.AllowedTierDifference && !troop.IsHero && !DoctrineAPI.IsDoctrineUnlocked<Ironclad>())
			{
				reasons |= EquipmentManager.EquipLimitReason.TierDifference;
			}
			return reasons == EquipmentManager.EquipLimitReason.None;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000CA88 File Offset: 0x0000AC88
		public static bool MeetsItemSkillRequirements(WCharacter troop, WItem item)
		{
			return item == null || item.RelevantSkill == null || item.Difficulty <= troop.GetSkill(item.RelevantSkill);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000CAB8 File Offset: 0x0000ACB8
		public static EquipmentManager.EquipQuote QuoteEquip(WCharacter troop, int setIndex, EquipmentIndex slot, WItem newItem)
		{
			EquipmentManager.<>c__DisplayClass13_0 CS$<>8__locals1;
			CS$<>8__locals1.setIndex = setIndex;
			CS$<>8__locals1.slot = slot;
			CS$<>8__locals1.newItem = newItem;
			EquipmentManager.EquipQuote equipQuote = new EquipmentManager.EquipQuote();
			CS$<>8__locals1.loadout = troop.Loadout;
			WEquipment wequipment = CS$<>8__locals1.loadout.Get(CS$<>8__locals1.setIndex);
			WItem witem = (wequipment != null) ? wequipment.Get(CS$<>8__locals1.slot) : null;
			equipQuote.IsChange = (witem != CS$<>8__locals1.newItem);
			if (!equipQuote.IsChange)
			{
				equipQuote.DeltaAdd = 0;
				equipQuote.DeltaRemove = 0;
				equipQuote.CopiesFromStock = 0;
				equipQuote.CopiesToBuy = 0;
				equipQuote.GoldCost = 0;
				equipQuote.WouldStage = false;
				return equipQuote;
			}
			CS$<>8__locals1.counterpart = null;
			if (troop != null)
			{
				if (troop.IsCaptain && troop.BaseTroop != null)
				{
					CS$<>8__locals1.counterpart = troop.BaseTroop;
				}
				else if (!troop.IsCaptain && troop.Captain != null)
				{
					CS$<>8__locals1.counterpart = troop.Captain;
				}
			}
			int num = EquipmentManager.<QuoteEquip>g__GlobalMaxCountPerSet|13_0(witem, ref CS$<>8__locals1);
			int num2 = (witem != null) ? EquipmentManager.<QuoteEquip>g__GlobalRequiredAfterForItem|13_1(witem, ref CS$<>8__locals1) : 0;
			int num3 = EquipmentManager.<QuoteEquip>g__GlobalMaxCountPerSet|13_0(CS$<>8__locals1.newItem, ref CS$<>8__locals1);
			int num4 = (CS$<>8__locals1.newItem != null) ? EquipmentManager.<QuoteEquip>g__GlobalRequiredAfterForItem|13_1(CS$<>8__locals1.newItem, ref CS$<>8__locals1) : 0;
			equipQuote.DeltaRemove = Math.Max(0, num - num2);
			equipQuote.DeltaAdd = Math.Max(0, num4 - num3);
			int val = (CS$<>8__locals1.newItem != null) ? CS$<>8__locals1.newItem.GetStock() : 0;
			equipQuote.CopiesFromStock = Math.Min(val, equipQuote.DeltaAdd);
			equipQuote.CopiesToBuy = Math.Max(0, equipQuote.DeltaAdd - equipQuote.CopiesFromStock);
			int itemCost = EquipmentManager.GetItemCost(CS$<>8__locals1.newItem);
			equipQuote.GoldCost = itemCost * equipQuote.CopiesToBuy;
			bool flag = Config.EquippingTroopsTakesTime && !ClanScreen.IsStudioMode;
			equipQuote.WouldStage = (flag && equipQuote.DeltaAdd > 0);
			return equipQuote;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000CCB8 File Offset: 0x0000AEB8
		private static EquipmentManager.EquipFailReason CheckAffordability(in EquipmentManager.EquipQuote q, bool allowPurchase)
		{
			if (q.DeltaAdd <= 0)
			{
				return EquipmentManager.EquipFailReason.None;
			}
			if (!Config.EquippingTroopsCostsGold)
			{
				return EquipmentManager.EquipFailReason.None;
			}
			if (q.CopiesFromStock < q.DeltaAdd && !allowPurchase)
			{
				return EquipmentManager.EquipFailReason.NotEnoughStock;
			}
			if (q.CopiesToBuy > 0 && Player.Gold < q.GoldCost)
			{
				return EquipmentManager.EquipFailReason.NotEnoughGold;
			}
			return EquipmentManager.EquipFailReason.None;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000CD10 File Offset: 0x0000AF10
		public static EquipmentManager.EquipResult TryEquip(WCharacter troop, int setIndex, EquipmentIndex slot, WItem newItem, bool allowPurchase = true)
		{
			EquipmentManager.EquipResult equipResult = new EquipmentManager.EquipResult
			{
				Ok = false,
				Staged = false,
				Reason = EquipmentManager.EquipFailReason.None
			};
			if (troop.Loadout.Get(setIndex).IsCivilian && newItem != null && !newItem.IsCivilian)
			{
				equipResult.Reason = EquipmentManager.EquipFailReason.NotCivilian;
				return equipResult;
			}
			if (!EquipmentManager.CanEquip(troop, newItem))
			{
				equipResult.Reason = EquipmentManager.EquipFailReason.NotAllowed;
				return equipResult;
			}
			if (ClanScreen.IsStudioMode)
			{
				return EquipmentManager.TryEquip_Studio(troop, setIndex, slot, newItem, equipResult);
			}
			return EquipmentManager.TryEquip_Custom(troop, setIndex, slot, newItem, equipResult, allowPurchase);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000CD90 File Offset: 0x0000AF90
		public static EquipmentManager.EquipResult TryEquip_Studio(WCharacter troop, int setIndex, EquipmentIndex slot, WItem newItem, EquipmentManager.EquipResult res)
		{
			WEquipment wequipment = troop.Loadout.Get(setIndex);
			if (((wequipment != null) ? wequipment.Get(slot) : null) == newItem)
			{
				res.Ok = true;
				return res;
			}
			EquipmentManager.ApplyStructureWithHorseRule(troop, setIndex, slot, newItem);
			res.Ok = true;
			res.Staged = false;
			res.GoldDelta = 0;
			res.AddedCopies = 0;
			res.RefundedCopies = 0;
			return res;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000CDFC File Offset: 0x0000AFFC
		public static EquipmentManager.EquipResult TryEquip_Custom(WCharacter troop, int setIndex, EquipmentIndex slot, WItem newItem, EquipmentManager.EquipResult res, bool allowPurchase = true)
		{
			WEquipment wequipment = troop.Loadout.Get(setIndex);
			WItem witem = (wequipment != null) ? wequipment.Get(slot) : null;
			EquipmentManager.EquipQuote equipQuote = EquipmentManager.QuoteEquip(troop, setIndex, slot, newItem);
			if (!equipQuote.IsChange)
			{
				res.Ok = true;
				return res;
			}
			if (equipQuote.DeltaAdd > 0)
			{
				EquipmentManager.EquipFailReason equipFailReason = EquipmentManager.CheckAffordability(equipQuote, allowPurchase);
				if (equipFailReason != EquipmentManager.EquipFailReason.None)
				{
					res.Reason = equipFailReason;
					return res;
				}
			}
			if (equipQuote.DeltaAdd > 0 && newItem != null && Config.EquippingTroopsCostsGold)
			{
				for (int i = 0; i < equipQuote.CopiesFromStock; i++)
				{
					newItem.Unstock();
				}
				if (equipQuote.CopiesToBuy > 0)
				{
					int itemCost = EquipmentManager.GetItemCost(newItem);
					if (itemCost > 0)
					{
						Player.ChangeGold(-itemCost * equipQuote.CopiesToBuy);
					}
					for (int j = 0; j < equipQuote.CopiesToBuy; j++)
					{
						newItem.Stock();
						newItem.Unstock();
					}
					EquipmentRebateBehavior.RegisterPurchase(newItem, equipQuote.CopiesToBuy);
					res.GoldDelta = -itemCost * equipQuote.CopiesToBuy;
				}
				res.AddedCopies = equipQuote.DeltaAdd;
			}
			if (equipQuote.DeltaRemove > 0 && witem != null && Config.EquippingTroopsCostsGold)
			{
				for (int k = 0; k < equipQuote.DeltaRemove; k++)
				{
					witem.Stock();
				}
				res.RefundedCopies = equipQuote.DeltaRemove;
			}
			if (equipQuote.WouldStage)
			{
				EquipStagingBehavior.Stage(troop, slot, newItem, setIndex);
				res.Ok = true;
				res.Staged = true;
				return res;
			}
			EquipmentManager.ApplyStructureWithHorseRule(troop, setIndex, slot, newItem);
			res.Ok = true;
			res.Staged = false;
			return res;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000CF94 File Offset: 0x0000B194
		public static EquipmentManager.EquipResult TryUnequip(WCharacter troop, int setIndex, EquipmentIndex slot)
		{
			EquipmentManager.<>c__DisplayClass18_0 CS$<>8__locals1;
			CS$<>8__locals1.setIndex = setIndex;
			CS$<>8__locals1.slot = slot;
			EquipmentManager.EquipResult equipResult = new EquipmentManager.EquipResult
			{
				Ok = false,
				Staged = false,
				Reason = EquipmentManager.EquipFailReason.None
			};
			CS$<>8__locals1.loadout = troop.Loadout;
			WEquipment wequipment = CS$<>8__locals1.loadout.Get(CS$<>8__locals1.setIndex);
			WItem witem = (wequipment != null) ? wequipment.Get(CS$<>8__locals1.slot) : null;
			if (witem == null)
			{
				equipResult.Ok = true;
				return equipResult;
			}
			CS$<>8__locals1.counterpart = null;
			if (troop != null)
			{
				if (troop.IsCaptain && troop.BaseTroop != null)
				{
					CS$<>8__locals1.counterpart = troop.BaseTroop;
				}
				else if (!troop.IsCaptain && troop.Captain != null)
				{
					CS$<>8__locals1.counterpart = troop.Captain;
				}
			}
			int num = EquipmentManager.<TryUnequip>g__GlobalMaxCountPerSet|18_0(witem, ref CS$<>8__locals1);
			int num2 = EquipmentManager.<TryUnequip>g__GlobalRequiredAfterForItem|18_1(witem, ref CS$<>8__locals1);
			int num3 = Math.Max(0, num - num2);
			if (CS$<>8__locals1.slot == EquipmentIndex.ArmorItemEndSlot)
			{
				WItem witem2 = wequipment.Get(EquipmentIndex.HorseHarness);
				if (witem2 != null)
				{
					int num4 = CS$<>8__locals1.loadout.MaxCountPerSet(witem2);
					int num5 = CS$<>8__locals1.loadout.RequiredAfterForItem(witem2, CS$<>8__locals1.setIndex, EquipmentIndex.HorseHarness, null);
					int num6 = Math.Max(0, num4 - num5);
					CS$<>8__locals1.loadout.Apply(CS$<>8__locals1.setIndex, EquipmentIndex.HorseHarness, null);
					for (int i = 0; i < num6; i++)
					{
						witem2.Stock();
					}
					equipResult.RefundedCopies += num6;
				}
			}
			CS$<>8__locals1.loadout.Apply(CS$<>8__locals1.setIndex, CS$<>8__locals1.slot, null);
			for (int j = 0; j < num3; j++)
			{
				witem.Stock();
			}
			equipResult.RefundedCopies += num3;
			equipResult.Ok = true;
			return equipResult;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000D160 File Offset: 0x0000B360
		public static EquipmentManager.DeleteSetQuote QuoteDeleteSet(WCharacter troop, int setIndex)
		{
			EquipmentManager.DeleteSetQuote deleteSetQuote = new EquipmentManager.DeleteSetQuote
			{
				Refunds = new Dictionary<WItem, int>()
			};
			foreach (KeyValuePair<WItem, ValueTuple<int, int, int>> keyValuePair in troop.Loadout.PreviewDeleteSet(setIndex))
			{
				if (keyValuePair.Value.Item3 > 0)
				{
					deleteSetQuote.Refunds[keyValuePair.Key] = keyValuePair.Value.Item3;
				}
			}
			return deleteSetQuote;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000D1F4 File Offset: 0x0000B3F4
		public static EquipmentManager.DeleteSetResult TryDeleteSet(WCharacter troop, int setIndex)
		{
			EquipmentManager.DeleteSetResult deleteSetResult = new EquipmentManager.DeleteSetResult
			{
				Ok = false,
				Refunded = new Dictionary<WItem, int>()
			};
			foreach (KeyValuePair<WItem, ValueTuple<int, int, int>> keyValuePair in troop.Loadout.PreviewDeleteSet(setIndex))
			{
				WItem key = keyValuePair.Key;
				int item = keyValuePair.Value.Item3;
				if (item > 0)
				{
					for (int i = 0; i < item; i++)
					{
						key.Stock();
					}
					deleteSetResult.Refunded[key] = item;
				}
			}
			WEquipment equipment = troop.Loadout.Get(setIndex);
			troop.Loadout.Remove(equipment);
			deleteSetResult.Ok = true;
			return deleteSetResult;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000D2C4 File Offset: 0x0000B4C4
		public static void ApplyImmediate(WCharacter troop, int setIndex, EquipmentIndex slot, WItem newItem)
		{
			EquipmentManager.ApplyStructureWithHorseRule(troop, setIndex, slot, newItem);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
		private static void ApplyStructureWithHorseRule(WCharacter t, int setIndex, EquipmentIndex slot, WItem item)
		{
			WLoadout loadout = t.Loadout;
			if (slot == EquipmentIndex.ArmorItemEndSlot && item == null)
			{
				loadout.Apply(setIndex, EquipmentIndex.HorseHarness, null);
			}
			loadout.Apply(setIndex, slot, item);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000D308 File Offset: 0x0000B508
		public static int GetItemCost(WItem item)
		{
			if (item == null)
			{
				return 0;
			}
			if (!Config.EquippingTroopsCostsGold)
			{
				return 0;
			}
			if (ClanScreen.IsStudioMode)
			{
				return 0;
			}
			int num = item.Value;
			if (DoctrineAPI.IsDoctrineUnlocked<CulturalPride>() && Player.Clan.Culture == item.Culture)
			{
				num = (int)((float)num * 0.8f);
			}
			if (DoctrineAPI.IsDoctrineUnlocked<RoyalPatronage>())
			{
				num = (int)((float)num * 0.8f);
			}
			float num2 = (float)num * Config.EquipmentCostMultiplier;
			if (EquipmentRebateBehavior.Instance != null)
			{
				float rebateMultiplier = EquipmentRebateBehavior.GetRebateMultiplier(item);
				num2 *= rebateMultiplier;
			}
			if (num2 <= 0f)
			{
				return 0;
			}
			return (int)num2;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000D3A4 File Offset: 0x0000B5A4
		public static void RollbackStagedEquip(WCharacter troop, int setIndex, EquipmentIndex slot, WItem stagedItem)
		{
			if (troop == null)
			{
				return;
			}
			if (stagedItem == null)
			{
				return;
			}
			WEquipment wequipment = troop.Loadout.Get(setIndex);
			WItem witem = (wequipment != null) ? wequipment.Get(slot) : null;
			EquipmentManager.EquipQuote equipQuote = EquipmentManager.QuoteEquip(troop, setIndex, slot, stagedItem);
			if (!equipQuote.IsChange)
			{
				return;
			}
			if (equipQuote.DeltaAdd > 0)
			{
				for (int i = 0; i < equipQuote.DeltaAdd; i++)
				{
					stagedItem.Stock();
				}
			}
			if (equipQuote.DeltaRemove > 0 && witem != null)
			{
				for (int j = 0; j < equipQuote.DeltaRemove; j++)
				{
					witem.Unstock();
				}
			}
			EquipStagingBehavior.Unstage(troop, slot, setIndex);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000D444 File Offset: 0x0000B644
		public static EquipmentManager.PasteResult TryPasteEquipment(WEquipment source, WEquipment target, HashSet<EquipmentIndex> allowedSlots = null)
		{
			EquipmentManager.PasteResult pasteResult = new EquipmentManager.PasteResult
			{
				Ok = false,
				Reason = EquipmentManager.EquipFailReason.None,
				Details = EquipmentManager.EquipLimitReason.None
			};
			if (source == null || target == null)
			{
				return pasteResult;
			}
			WCharacter troop = target.Loadout.Troop;
			if (troop == null)
			{
				return pasteResult;
			}
			bool isStudioMode = ClanScreen.IsStudioMode;
			foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
			{
				if (allowedSlots == null || allowedSlots.Contains(equipmentIndex))
				{
					WItem witem = source.Get(equipmentIndex);
					if (!(witem == null))
					{
						if (target.IsCivilian && !witem.IsCivilian)
						{
							pasteResult.Reason = EquipmentManager.EquipFailReason.NotCivilian;
							return pasteResult;
						}
						EquipmentManager.EquipLimitReason equipLimitReason;
						if (!EquipmentManager.CanEquip(troop, witem, out equipLimitReason))
						{
							pasteResult.Reason = EquipmentManager.EquipFailReason.NotAllowed;
							pasteResult.Details |= equipLimitReason;
						}
					}
				}
			}
			if (pasteResult.Reason != EquipmentManager.EquipFailReason.None)
			{
				return pasteResult;
			}
			int num = EquipmentManager.QuotePasteGoldCost(source, target, allowedSlots);
			if (!isStudioMode && Config.EquippingTroopsCostsGold && num > 0 && Player.Gold < num)
			{
				pasteResult.Reason = EquipmentManager.EquipFailReason.NotEnoughGold;
				return pasteResult;
			}
			if (!isStudioMode && Config.EquippingTroopsCostsGold && num > 0)
			{
				Player.ChangeGold(-num);
			}
			foreach (EquipmentIndex equipmentIndex2 in WEquipment.Slots)
			{
				if (allowedSlots == null || allowedSlots.Contains(equipmentIndex2))
				{
					WItem witem2 = source.Get(equipmentIndex2);
					WItem witem3 = target.Get(equipmentIndex2);
					if (!(witem2 == witem3))
					{
						EquipmentManager.EquipQuote equipQuote = EquipmentManager.QuoteEquip(troop, target.Index, equipmentIndex2, witem2);
						if (equipQuote.IsChange)
						{
							if (!isStudioMode && Config.EquippingTroopsCostsGold)
							{
								for (int i = 0; i < equipQuote.CopiesFromStock; i++)
								{
									witem2.Unstock();
								}
								if (equipQuote.CopiesToBuy > 0)
								{
									for (int j = 0; j < equipQuote.CopiesToBuy; j++)
									{
										witem2.Stock();
										witem2.Unstock();
									}
									EquipmentRebateBehavior.RegisterPurchase(witem2, equipQuote.CopiesToBuy);
								}
							}
							if (equipQuote.DeltaRemove > 0 && witem3 != null && !isStudioMode && Config.EquippingTroopsCostsGold)
							{
								for (int k = 0; k < equipQuote.DeltaRemove; k++)
								{
									witem3.Stock();
								}
							}
							if (equipQuote.WouldStage && !isStudioMode)
							{
								EquipStagingBehavior.Stage(troop, equipmentIndex2, witem2, target.Index);
							}
							else
							{
								EquipmentManager.ApplyStructureWithHorseRule(troop, target.Index, equipmentIndex2, witem2);
							}
						}
					}
				}
			}
			pasteResult.Ok = true;
			return pasteResult;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000D710 File Offset: 0x0000B910
		public static int QuotePasteGoldCost(WEquipment source, WEquipment target, HashSet<EquipmentIndex> allowedSlots = null)
		{
			if (source == null || target == null)
			{
				return 0;
			}
			WCharacter troop = target.Loadout.Troop;
			if (troop == null)
			{
				return 0;
			}
			if (ClanScreen.IsStudioMode || !Config.EquippingTroopsCostsGold)
			{
				return 0;
			}
			int num = 0;
			foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
			{
				if (allowedSlots == null || allowedSlots.Contains(equipmentIndex))
				{
					WItem witem = source.Get(equipmentIndex);
					WItem right = target.Get(equipmentIndex);
					if (!(witem == right) && EquipmentManager.CanEquip(troop, witem))
					{
						EquipmentManager.EquipQuote equipQuote = EquipmentManager.QuoteEquip(troop, target.Index, equipmentIndex, witem);
						if (equipQuote.IsChange)
						{
							num += equipQuote.GoldCost;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000D7EC File Offset: 0x0000B9EC
		[CompilerGenerated]
		internal static int <QuoteEquip>g__GlobalMaxCountPerSet|13_0(WItem item, ref EquipmentManager.<>c__DisplayClass13_0 A_1)
		{
			if (item == null)
			{
				return 0;
			}
			int num = A_1.loadout.MaxCountPerSet(item);
			if (A_1.counterpart != null)
			{
				int num2 = A_1.counterpart.Loadout.MaxCountPerSet(item);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000D838 File Offset: 0x0000BA38
		[CompilerGenerated]
		internal static int <QuoteEquip>g__GlobalRequiredAfterForItem|13_1(WItem item, ref EquipmentManager.<>c__DisplayClass13_0 A_1)
		{
			if (item == null)
			{
				return 0;
			}
			int num = A_1.loadout.RequiredAfterForItem(item, A_1.setIndex, A_1.slot, A_1.newItem);
			if (A_1.counterpart == null)
			{
				return num;
			}
			int num2 = A_1.counterpart.Loadout.MaxCountPerSet(item);
			if (num <= num2)
			{
				return num2;
			}
			return num;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000D898 File Offset: 0x0000BA98
		[CompilerGenerated]
		internal static int <TryUnequip>g__GlobalMaxCountPerSet|18_0(WItem item, ref EquipmentManager.<>c__DisplayClass18_0 A_1)
		{
			if (item == null)
			{
				return 0;
			}
			int num = A_1.loadout.MaxCountPerSet(item);
			if (A_1.counterpart != null)
			{
				int num2 = A_1.counterpart.Loadout.MaxCountPerSet(item);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000D8E4 File Offset: 0x0000BAE4
		[CompilerGenerated]
		internal static int <TryUnequip>g__GlobalRequiredAfterForItem|18_1(WItem item, ref EquipmentManager.<>c__DisplayClass18_0 A_1)
		{
			if (item == null)
			{
				return 0;
			}
			int num = A_1.loadout.RequiredAfterForItem(item, A_1.setIndex, A_1.slot, null);
			if (A_1.counterpart == null)
			{
				return num;
			}
			int num2 = A_1.counterpart.Loadout.MaxCountPerSet(item);
			if (num <= num2)
			{
				return num2;
			}
			return num;
		}

		// Token: 0x0200012C RID: 300
		public enum EquipFailReason
		{
			// Token: 0x04000361 RID: 865
			None,
			// Token: 0x04000362 RID: 866
			NotAllowed,
			// Token: 0x04000363 RID: 867
			NotEnoughStock,
			// Token: 0x04000364 RID: 868
			NotEnoughGold,
			// Token: 0x04000365 RID: 869
			NotCivilian
		}

		// Token: 0x0200012D RID: 301
		[Flags]
		public enum EquipLimitReason
		{
			// Token: 0x04000367 RID: 871
			None = 0,
			// Token: 0x04000368 RID: 872
			Skill = 1,
			// Token: 0x04000369 RID: 873
			MountT1 = 2,
			// Token: 0x0400036A RID: 874
			TierDifference = 4
		}

		// Token: 0x0200012E RID: 302
		public sealed class EquipQuote
		{
			// Token: 0x0400036B RID: 875
			public bool IsChange;

			// Token: 0x0400036C RID: 876
			public int DeltaAdd;

			// Token: 0x0400036D RID: 877
			public int DeltaRemove;

			// Token: 0x0400036E RID: 878
			public int CopiesFromStock;

			// Token: 0x0400036F RID: 879
			public int CopiesToBuy;

			// Token: 0x04000370 RID: 880
			public int GoldCost;

			// Token: 0x04000371 RID: 881
			public bool WouldStage;
		}

		// Token: 0x0200012F RID: 303
		public sealed class EquipResult
		{
			// Token: 0x04000372 RID: 882
			public bool Ok;

			// Token: 0x04000373 RID: 883
			public bool Staged;

			// Token: 0x04000374 RID: 884
			public EquipmentManager.EquipFailReason Reason;

			// Token: 0x04000375 RID: 885
			public int GoldDelta;

			// Token: 0x04000376 RID: 886
			public int AddedCopies;

			// Token: 0x04000377 RID: 887
			public int RefundedCopies;
		}

		// Token: 0x02000130 RID: 304
		public sealed class DeleteSetQuote
		{
			// Token: 0x04000378 RID: 888
			public Dictionary<WItem, int> Refunds;
		}

		// Token: 0x02000131 RID: 305
		public sealed class DeleteSetResult
		{
			// Token: 0x04000379 RID: 889
			public bool Ok;

			// Token: 0x0400037A RID: 890
			public Dictionary<WItem, int> Refunded;
		}

		// Token: 0x02000132 RID: 306
		public sealed class PasteResult
		{
			// Token: 0x0400037B RID: 891
			public bool Ok;

			// Token: 0x0400037C RID: 892
			public EquipmentManager.EquipFailReason Reason;

			// Token: 0x0400037D RID: 893
			public EquipmentManager.EquipLimitReason Details;
		}
	}
}
