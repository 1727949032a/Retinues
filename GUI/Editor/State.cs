using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Features.Agents;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Mods;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Retinues.GUI.Editor
{
	// Token: 0x02000077 RID: 119
	[SafeClass]
	public static class State
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000F9FF File Offset: 0x0000DBFF
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000FA06 File Offset: 0x0000DC06
		public static HashSet<string> AvailableFromAnotherSetCache { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000FA0E File Offset: 0x0000DC0E
		// (set) Token: 0x06000259 RID: 601 RVA: 0x0000FA15 File Offset: 0x0000DC15
		public static BaseFaction PendingFaction { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000FA1D File Offset: 0x0000DC1D
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000FA24 File Offset: 0x0000DC24
		public static WCharacter PendingTroop { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000FA2C File Offset: 0x0000DC2C
		// (set) Token: 0x0600025D RID: 605 RVA: 0x0000FA33 File Offset: 0x0000DC33
		public static BaseFaction Faction { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000FA3B File Offset: 0x0000DC3B
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000FA42 File Offset: 0x0000DC42
		public static WCharacter Troop { get; private set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000FA4A File Offset: 0x0000DC4A
		// (set) Token: 0x06000261 RID: 609 RVA: 0x0000FA51 File Offset: 0x0000DC51
		public static WEquipment Equipment { get; private set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0000FA59 File Offset: 0x0000DC59
		// (set) Token: 0x06000263 RID: 611 RVA: 0x0000FA60 File Offset: 0x0000DC60
		public static EquipmentIndex Slot { get; private set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000FA68 File Offset: 0x0000DC68
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000FA6F File Offset: 0x0000DC6F
		public static Dictionary<EquipmentIndex, EquipData> EquipData { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000FA77 File Offset: 0x0000DC77
		// (set) Token: 0x06000267 RID: 615 RVA: 0x0000FA7E File Offset: 0x0000DC7E
		public static Dictionary<SkillObject, SkillData> SkillData { get; private set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0000FA86 File Offset: 0x0000DC86
		// (set) Token: 0x06000269 RID: 617 RVA: 0x0000FA8D File Offset: 0x0000DC8D
		public static Dictionary<WCharacter, int> ConversionData { get; private set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000FA95 File Offset: 0x0000DC95
		// (set) Token: 0x0600026B RID: 619 RVA: 0x0000FA9C File Offset: 0x0000DC9C
		public static Dictionary<WCharacter, int> PartyData { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000FAA4 File Offset: 0x0000DCA4
		public static WCulture Culture
		{
			get
			{
				BaseFaction faction = State.Faction;
				if (faction == null)
				{
					return null;
				}
				return faction.Culture;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000FAB8 File Offset: 0x0000DCB8
		public static WClan Clan
		{
			get
			{
				WClan wclan = State.Faction as WClan;
				if (wclan != null)
				{
					return wclan;
				}
				if (State.Faction is WFaction)
				{
					return new WClan(Hero.MainHero.Clan);
				}
				BaseFaction faction = State.Faction;
				WCulture culture = faction as WCulture;
				if (culture != null)
				{
					return WClan.All.FirstOrDefault(delegate(WClan c)
					{
						string a;
						WCulture culture;
						if (c == null)
						{
							a = null;
						}
						else
						{
							culture = c.Culture;
							a = ((culture != null) ? culture.StringId : null);
						}
						return a == culture.StringId;
					});
				}
				return null;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		public static void ResetAll()
		{
			if (!ClanScreen.IsStudioMode)
			{
				foreach (WFaction wfaction in new WFaction[]
				{
					Player.Clan,
					Player.Kingdom
				})
				{
					if (wfaction != null)
					{
						TroopBuilder.EnsureTroopsExist(wfaction);
					}
				}
			}
			EventManager.FireBatch(delegate
			{
				if (State.PendingFaction != null)
				{
					State.UpdateFaction(State.PendingFaction);
					State.PendingFaction = null;
				}
				else
				{
					State.UpdateFaction(null);
				}
				if (State.PendingTroop != null)
				{
					State.UpdateTroop(State.PendingTroop);
					State.PendingTroop = null;
				}
				State.UpdatePartyData(null);
			});
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000FB9C File Offset: 0x0000DD9C
		public static void UpdateFaction(BaseFaction faction = null)
		{
			if (faction == null)
			{
				faction = ((!ClanScreen.IsStudioMode) ? Player.Clan : Player.Culture);
			}
			BaseFaction faction2 = faction;
			if (faction2 != null)
			{
				faction2.InvalidateCategoryCache();
			}
			EventManager.FireBatch(delegate
			{
				State.Faction = faction;
				State.UpdateTroop(null);
				EventManager.Fire(UIEvent.Faction);
			});
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000FBFC File Offset: 0x0000DDFC
		public static void UpdateTroop(WCharacter troop = null)
		{
			if (troop == null)
			{
				troop = State.Faction.Troops.FirstOrDefault<WCharacter>();
			}
			EventManager.FireBatch(delegate
			{
				State.Troop = troop;
				if (State.Troop != null)
				{
					State.FixIntegrity(State.Troop);
				}
				State.UpdateEquipment(null);
				State.UpdateSkillData(null);
				State.UpdateSlot(EquipmentIndex.WeaponItemBeginSlot);
				EventManager.Fire(UIEvent.Troop);
			});
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000FC44 File Offset: 0x0000DE44
		public static void UpdateEquipment(WEquipment equipment = null)
		{
			EventManager.FireBatch(delegate
			{
				if (equipment == null)
				{
					equipment = (State.Troop.IsCivilian ? State.Troop.Loadout.Civilian : State.Troop.Loadout.Battle);
				}
				State.Equipment = equipment;
				State.UpdateEquipData(null, false);
				EventManager.Fire(UIEvent.Equipment);
			});
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000FC62 File Offset: 0x0000DE62
		public static void UpdateSlot(EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot)
		{
			if (State.Slot == slot)
			{
				return;
			}
			State.Slot = slot;
			EventManager.Fire(UIEvent.Slot);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public static void UpdateEquipData(Dictionary<EquipmentIndex, EquipData> equipData = null, bool singleUpdate = true)
		{
			if (equipData == null)
			{
				equipData = State.ComputeEquipData();
			}
			WCharacter troop = State.Troop;
			if (troop != null && troop.IsRetinue)
			{
				State.UpdateConversionData(null);
			}
			EquipChangeDelta? lastEquipChange = singleUpdate ? State.CaptureEquipChange(equipData, State.Slot) : null;
			State.EquipData = equipData;
			State.RebuildAvailableFromAnotherSetCache();
			if (singleUpdate)
			{
				State.LastEquipChange = lastEquipChange;
				EventManager.Fire(UIEvent.Equip);
			}
			State.UpdateAppearance();
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000FCE5 File Offset: 0x0000DEE5
		public static void UpdateSkillData(Dictionary<SkillObject, SkillData> skillData = null)
		{
			if (skillData == null)
			{
				skillData = State.ComputeSkillData();
			}
			State.SkillData = skillData;
			EventManager.Fire(UIEvent.Train);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000FCFD File Offset: 0x0000DEFD
		public static void UpdateConversionData(Dictionary<WCharacter, int> conversionData = null)
		{
			if (conversionData == null)
			{
				conversionData = State.ComputeConversionData();
			}
			State.ConversionData = conversionData;
			EventManager.Fire(UIEvent.Conversion);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000FD18 File Offset: 0x0000DF18
		public static void ClearPendingConversions()
		{
			if (State.ConversionData == null)
			{
				State.UpdateConversionData(null);
				return;
			}
			bool flag = false;
			foreach (WCharacter key in State.ConversionData.Keys.ToList<WCharacter>())
			{
				if (State.ConversionData[key] != 0)
				{
					State.ConversionData[key] = 0;
					flag = true;
				}
			}
			if (flag)
			{
				EventManager.Fire(UIEvent.Conversion);
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000FDA4 File Offset: 0x0000DFA4
		public static void UpdatePartyData(Dictionary<WCharacter, int> partyData = null)
		{
			if (partyData == null)
			{
				partyData = State.ComputePartyData();
			}
			State.PartyData = partyData;
			EventManager.Fire(UIEvent.Party);
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000FDBC File Offset: 0x0000DFBC
		public static void UpdateAppearance()
		{
			EventManager.Fire(UIEvent.Appearance);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000FDC4 File Offset: 0x0000DFC4
		private static Dictionary<EquipmentIndex, EquipData> ComputeEquipData()
		{
			Dictionary<EquipmentIndex, EquipData> dictionary = new Dictionary<EquipmentIndex, EquipData>();
			foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
			{
				dictionary[equipmentIndex] = new EquipData
				{
					Item = State.Equipment.Get(equipmentIndex),
					Equip = EquipStagingBehavior.Get(State.Troop, equipmentIndex, State.Equipment.Index)
				};
			}
			return dictionary;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000FE54 File Offset: 0x0000E054
		private static Dictionary<SkillObject, SkillData> ComputeSkillData()
		{
			Dictionary<SkillObject, SkillData> dictionary = new Dictionary<SkillObject, SkillData>();
			foreach (KeyValuePair<SkillObject, int> keyValuePair in State.Troop.Skills)
			{
				dictionary[keyValuePair.Key] = new SkillData
				{
					Value = keyValuePair.Value,
					Train = TrainStagingBehavior.Get(State.Troop, keyValuePair.Key)
				};
			}
			return dictionary;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
		private static Dictionary<WCharacter, int> ComputeConversionData()
		{
			Dictionary<WCharacter, int> dictionary = new Dictionary<WCharacter, int>();
			WCharacter troop = State.Troop;
			if (troop == null || !troop.IsRetinue)
			{
				return dictionary;
			}
			foreach (WCharacter wcharacter in RetinueManager.GetRetinueSourceTroops(State.Troop))
			{
				if (wcharacter != null && wcharacter.IsValid)
				{
					dictionary[wcharacter] = 0;
				}
			}
			return dictionary;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000FF6C File Offset: 0x0000E16C
		private static Dictionary<WCharacter, int> ComputePartyData()
		{
			Dictionary<WCharacter, int> dictionary = new Dictionary<WCharacter, int>();
			foreach (WRosterElement wrosterElement in Player.Party.MemberRoster.Elements)
			{
				dictionary[wrosterElement.Troop] = wrosterElement.Number;
			}
			return dictionary;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000FFD4 File Offset: 0x0000E1D4
		private static EquipChangeDelta? CaptureEquipChange(Dictionary<EquipmentIndex, EquipData> next, EquipmentIndex slot)
		{
			EquipData equipData;
			if (!next.TryGetValue(slot, out equipData))
			{
				return null;
			}
			string text = null;
			string text2 = null;
			EquipData equipData2;
			if (State.EquipData != null && State.EquipData.TryGetValue(slot, out equipData2))
			{
				WItem item = equipData2.Item;
				text = ((item != null) ? item.StringId : null);
				PendingEquipData equip = equipData2.Equip;
				text2 = ((equip != null) ? equip.ItemId : null);
			}
			WItem item2 = equipData.Item;
			string text3 = (item2 != null) ? item2.StringId : null;
			PendingEquipData equip2 = equipData.Equip;
			string text4 = (equip2 != null) ? equip2.ItemId : null;
			if (text == text3 && text2 == text4)
			{
				return null;
			}
			return new EquipChangeDelta?(new EquipChangeDelta(text, text3, text2, text4));
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00010090 File Offset: 0x0000E290
		public static void FixIntegrity(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			try
			{
				troop.Loadout.Normalize();
				State.FixCombatPolicies(troop);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000100D8 File Offset: 0x0000E2D8
		public static void FixCombatPolicies(WCharacter troop)
		{
			State.<>c__DisplayClass66_0 CS$<>8__locals1;
			CS$<>8__locals1.troop = troop;
			if (CS$<>8__locals1.troop == null)
			{
				return;
			}
			State.<FixCombatPolicies>g__EnsureOne|66_0(PolicyToggleType.FieldBattle, ref CS$<>8__locals1);
			State.<FixCombatPolicies>g__EnsureOne|66_0(PolicyToggleType.SiegeDefense, ref CS$<>8__locals1);
			State.<FixCombatPolicies>g__EnsureOne|66_0(PolicyToggleType.SiegeAssault, ref CS$<>8__locals1);
			if (ModCompatibility.HasNavalDLC)
			{
				State.<FixCombatPolicies>g__EnsureOne|66_0(PolicyToggleType.NavalBattle, ref CS$<>8__locals1);
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00010124 File Offset: 0x0000E324
		private static void RebuildAvailableFromAnotherSetCache()
		{
			Retinues.Utils.Timer.Begin("BuildAvailableFromAnotherSetCache");
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			try
			{
				WCharacter troop = State.Troop;
				WEquipment equipment = State.Equipment;
				if (troop == null || equipment == null)
				{
					State.AvailableFromAnotherSetCache = hashSet;
				}
				else
				{
					List<WEquipment> equipments = troop.Loadout.Equipments;
					int index = equipment.Index;
					if (index < 0 || index >= equipments.Count)
					{
						State.AvailableFromAnotherSetCache = hashSet;
					}
					else
					{
						List<HashSet<string>> list = new List<HashSet<string>>(equipments.Count);
						for (int i = 0; i < equipments.Count; i++)
						{
							HashSet<string> hashSet2 = new HashSet<string>(StringComparer.Ordinal);
							Equipment @base = equipments[i].Base;
							foreach (EquipmentIndex index2 in WEquipment.Slots)
							{
								ItemObject item = @base[index2].Item;
								if (item != null)
								{
									hashSet2.Add(item.StringId);
								}
							}
							list.Add(hashSet2);
						}
						HashSet<string> hashSet3 = list[index];
						HashSet<string> hashSet4 = null;
						WCharacter wcharacter = null;
						if (troop.IsCaptain && troop.BaseTroop != null)
						{
							wcharacter = troop.BaseTroop;
						}
						else if (!troop.IsCaptain && troop.Captain != null)
						{
							wcharacter = troop.Captain;
						}
						if (wcharacter != null && wcharacter.IsValid)
						{
							hashSet4 = new HashSet<string>(StringComparer.Ordinal);
							foreach (WEquipment wequipment in wcharacter.Loadout.Equipments)
							{
								Equipment base2 = wequipment.Base;
								foreach (EquipmentIndex index3 in WEquipment.Slots)
								{
									ItemObject item2 = base2[index3].Item;
									if (item2 != null)
									{
										hashSet4.Add(item2.StringId);
									}
								}
							}
						}
						HashSet<string> hashSet5 = new HashSet<string>(StringComparer.Ordinal);
						foreach (HashSet<string> other in list)
						{
							hashSet5.UnionWith(other);
						}
						if (hashSet4 != null)
						{
							hashSet5.UnionWith(hashSet4);
						}
						foreach (string item3 in hashSet5)
						{
							if (!hashSet3.Contains(item3))
							{
								bool flag = false;
								for (int j = 0; j < list.Count; j++)
								{
									if (j != index && list[j].Contains(item3))
									{
										flag = true;
										break;
									}
								}
								bool flag2 = hashSet4 != null && hashSet4.Contains(item3);
								if (flag || flag2)
								{
									hashSet.Add(item3);
								}
							}
						}
						State.AvailableFromAnotherSetCache = hashSet;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				State.AvailableFromAnotherSetCache = hashSet;
			}
			finally
			{
				Retinues.Utils.Timer.End("BuildAvailableFromAnotherSetCache");
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000104EC File Offset: 0x0000E6EC
		[CompilerGenerated]
		internal static void <FixCombatPolicies>g__EnsureOne|66_0(PolicyToggleType t, ref State.<>c__DisplayClass66_0 A_1)
		{
			int num = 0;
			List<WEquipment> equipments = A_1.troop.Loadout.Equipments;
			for (int i = 0; i < equipments.Count; i++)
			{
				if (!equipments[i].IsCivilian && CombatAgentBehavior.IsEnabled(A_1.troop, i, t))
				{
					num++;
				}
			}
			if (num == 0 && !CombatAgentBehavior.IsEnabled(A_1.troop, 0, t))
			{
				CombatAgentBehavior.Toggle(A_1.troop, 0, t);
			}
		}

		// Token: 0x040000DB RID: 219
		public static EquipChangeDelta? LastEquipChange;
	}
}
