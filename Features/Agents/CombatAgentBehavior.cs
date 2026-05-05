using System;
using System.Collections.Generic;
using Retinues.Game.Wrappers;
using Retinues.Mods;
using TaleWorlds.CampaignSystem;

namespace Retinues.Features.Agents
{
	// Token: 0x020000CC RID: 204
	public sealed class CombatAgentBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600084A RID: 2122 RVA: 0x00029EB0 File Offset: 0x000280B0
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, Dictionary<int, EquipmentPolicy>>>("Retinues_EquipmentUsePolicy", ref this._byTroop);
			if (dataStore.IsLoading && ModCompatibility.HasNavalDLC)
			{
				foreach (Dictionary<int, EquipmentPolicy> dictionary in this._byTroop.Values)
				{
					foreach (EquipmentPolicy equipmentPolicy in dictionary.Values)
					{
						if (equipmentPolicy.SiegeAssault || equipmentPolicy.SiegeDefense)
						{
							equipmentPolicy.SiegeAssault = true;
							equipmentPolicy.SiegeDefense = true;
						}
					}
				}
			}
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00029F7C File Offset: 0x0002817C
		public override void RegisterEvents()
		{
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00029F80 File Offset: 0x00028180
		public EquipmentPolicy GetPolicy(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return EquipmentPolicy.None;
			}
			Dictionary<int, EquipmentPolicy> dictionary;
			EquipmentPolicy equipmentPolicy;
			if (this._byTroop.TryGetValue(troop.StringId, out dictionary) && dictionary.TryGetValue(index, out equipmentPolicy) && equipmentPolicy != null)
			{
				return equipmentPolicy;
			}
			return EquipmentPolicy.All;
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x00029FC6 File Offset: 0x000281C6
		public bool IsEnabled_FieldBattle(WCharacter troop, int index)
		{
			return this.GetPolicy(troop, index).FieldBattle;
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x00029FD5 File Offset: 0x000281D5
		public bool IsEnabled_NavalBattle(WCharacter troop, int index)
		{
			return this.GetPolicy(troop, index).NavalBattle;
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x00029FE4 File Offset: 0x000281E4
		public bool IsEnabled_SiegeDefense(WCharacter troop, int index)
		{
			return this.GetPolicy(troop, index).SiegeDefense;
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x00029FF3 File Offset: 0x000281F3
		public bool IsEnabled_SiegeAssault(WCharacter troop, int index)
		{
			return this.GetPolicy(troop, index).SiegeAssault;
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0002A002 File Offset: 0x00028202
		public bool IsEnabled_GenderOverride(WCharacter troop, int index)
		{
			return this.GetPolicy(troop, index).GenderOverride;
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0002A011 File Offset: 0x00028211
		public void Toggle_FieldBattle(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return;
			}
			if (!this.CanDisable(troop, index, PolicyToggleType.FieldBattle))
			{
				return;
			}
			this.Set(troop, index, delegate(EquipmentPolicy p)
			{
				p.FieldBattle = !p.FieldBattle;
			});
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0002A050 File Offset: 0x00028250
		public void Toggle_NavalBattle(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return;
			}
			if (!this.CanDisable(troop, index, PolicyToggleType.NavalBattle))
			{
				return;
			}
			this.Set(troop, index, delegate(EquipmentPolicy p)
			{
				p.NavalBattle = !p.NavalBattle;
			});
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0002A08F File Offset: 0x0002828F
		public void Toggle_SiegeDefense(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return;
			}
			if (!this.CanDisable(troop, index, PolicyToggleType.SiegeDefense))
			{
				return;
			}
			this.Set(troop, index, delegate(EquipmentPolicy p)
			{
				p.SiegeDefense = !p.SiegeDefense;
			});
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0002A0CE File Offset: 0x000282CE
		public void Toggle_SiegeAssault(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return;
			}
			if (!this.CanDisable(troop, index, PolicyToggleType.SiegeAssault))
			{
				return;
			}
			this.Set(troop, index, delegate(EquipmentPolicy p)
			{
				p.SiegeAssault = !p.SiegeAssault;
			});
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0002A10D File Offset: 0x0002830D
		public void Toggle_GenderOverride(WCharacter troop, int index)
		{
			if (troop == null)
			{
				return;
			}
			this.Set(troop, index, delegate(EquipmentPolicy p)
			{
				p.GenderOverride = !p.GenderOverride;
			});
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x0002A140 File Offset: 0x00028340
		public void OnRemoveAlt(WCharacter troop, int removedIndex)
		{
			if (troop == null || removedIndex < 2)
			{
				return;
			}
			Dictionary<int, EquipmentPolicy> dictionary;
			if (!this._byTroop.TryGetValue(troop.StringId, out dictionary))
			{
				return;
			}
			Dictionary<int, EquipmentPolicy> dictionary2 = new Dictionary<int, EquipmentPolicy>();
			foreach (KeyValuePair<int, EquipmentPolicy> keyValuePair in dictionary)
			{
				if (keyValuePair.Key < removedIndex)
				{
					dictionary2[keyValuePair.Key] = keyValuePair.Value;
				}
				else if (keyValuePair.Key > removedIndex)
				{
					dictionary2[keyValuePair.Key - 1] = keyValuePair.Value;
				}
			}
			if (dictionary2.Count == 0)
			{
				this._byTroop.Remove(troop.StringId);
				return;
			}
			this._byTroop[troop.StringId] = dictionary2;
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0002A220 File Offset: 0x00028420
		private void Set(WCharacter troop, int index, Action<EquipmentPolicy> mut)
		{
			if (troop == null)
			{
				return;
			}
			Dictionary<int, EquipmentPolicy> dictionary;
			if (!this._byTroop.TryGetValue(troop.StringId, out dictionary))
			{
				dictionary = (this._byTroop[troop.StringId] = new Dictionary<int, EquipmentPolicy>());
			}
			EquipmentPolicy equipmentPolicy;
			if (!dictionary.TryGetValue(index, out equipmentPolicy) || equipmentPolicy == null)
			{
				Dictionary<int, EquipmentPolicy> dictionary2 = dictionary;
				EquipmentPolicy equipmentPolicy2 = new EquipmentPolicy();
				equipmentPolicy2.FieldBattle = true;
				equipmentPolicy2.NavalBattle = true;
				equipmentPolicy2.SiegeDefense = true;
				equipmentPolicy2.SiegeAssault = true;
				equipmentPolicy2.GenderOverride = false;
				equipmentPolicy = equipmentPolicy2;
				dictionary2[index] = equipmentPolicy2;
			}
			mut(equipmentPolicy);
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x0002A2AA File Offset: 0x000284AA
		private static CombatAgentBehavior Inst
		{
			get
			{
				Campaign campaign = Campaign.Current;
				if (campaign == null)
				{
					return null;
				}
				return campaign.GetCampaignBehavior<CombatAgentBehavior>();
			}
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0002A2BC File Offset: 0x000284BC
		public static bool IsEnabled(WCharacter troop, int altIndex, PolicyToggleType t)
		{
			bool result;
			switch (t)
			{
			case PolicyToggleType.FieldBattle:
			{
				CombatAgentBehavior inst = CombatAgentBehavior.Inst;
				result = (inst == null || inst.IsEnabled_FieldBattle(troop, altIndex));
				break;
			}
			case PolicyToggleType.NavalBattle:
			{
				CombatAgentBehavior inst2 = CombatAgentBehavior.Inst;
				result = (inst2 == null || inst2.IsEnabled_NavalBattle(troop, altIndex));
				break;
			}
			case PolicyToggleType.SiegeDefense:
			{
				CombatAgentBehavior inst3 = CombatAgentBehavior.Inst;
				result = (inst3 == null || inst3.IsEnabled_SiegeDefense(troop, altIndex));
				break;
			}
			case PolicyToggleType.SiegeAssault:
			{
				CombatAgentBehavior inst4 = CombatAgentBehavior.Inst;
				result = (inst4 == null || inst4.IsEnabled_SiegeAssault(troop, altIndex));
				break;
			}
			case PolicyToggleType.GenderOverride:
			{
				CombatAgentBehavior inst5 = CombatAgentBehavior.Inst;
				result = (inst5 == null || inst5.IsEnabled_GenderOverride(troop, altIndex));
				break;
			}
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0002A358 File Offset: 0x00028558
		public static void Toggle(WCharacter troop, int altIndex, PolicyToggleType t)
		{
			switch (t)
			{
			case PolicyToggleType.FieldBattle:
			{
				CombatAgentBehavior inst = CombatAgentBehavior.Inst;
				if (inst == null)
				{
					return;
				}
				inst.Toggle_FieldBattle(troop, altIndex);
				return;
			}
			case PolicyToggleType.NavalBattle:
			{
				CombatAgentBehavior inst2 = CombatAgentBehavior.Inst;
				if (inst2 == null)
				{
					return;
				}
				inst2.Toggle_NavalBattle(troop, altIndex);
				return;
			}
			case PolicyToggleType.SiegeDefense:
			{
				CombatAgentBehavior inst3 = CombatAgentBehavior.Inst;
				if (inst3 == null)
				{
					return;
				}
				inst3.Toggle_SiegeDefense(troop, altIndex);
				return;
			}
			case PolicyToggleType.SiegeAssault:
			{
				CombatAgentBehavior inst4 = CombatAgentBehavior.Inst;
				if (inst4 == null)
				{
					return;
				}
				inst4.Toggle_SiegeAssault(troop, altIndex);
				return;
			}
			case PolicyToggleType.GenderOverride:
			{
				CombatAgentBehavior inst5 = CombatAgentBehavior.Inst;
				if (inst5 == null)
				{
					return;
				}
				inst5.Toggle_GenderOverride(troop, altIndex);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x0002A3D9 File Offset: 0x000285D9
		public static void DisableAll(WCharacter troop, int altIndex)
		{
			if (troop == null)
			{
				return;
			}
			CombatAgentBehavior inst = CombatAgentBehavior.Inst;
			if (inst == null)
			{
				return;
			}
			inst.Set(troop, altIndex, delegate(EquipmentPolicy p)
			{
				p.FieldBattle = false;
				p.NavalBattle = false;
				p.SiegeDefense = false;
				p.SiegeAssault = false;
				p.GenderOverride = false;
			});
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0002A415 File Offset: 0x00028615
		public static void OnRemoved(WCharacter troop, int removedIndex)
		{
			CombatAgentBehavior inst = CombatAgentBehavior.Inst;
			if (inst == null)
			{
				return;
			}
			inst.OnRemoveAlt(troop, removedIndex);
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0002A428 File Offset: 0x00028628
		private int CountEnabled(WCharacter troop, PolicyToggleType t)
		{
			if (troop == null)
			{
				return 0;
			}
			int num = 0;
			List<WEquipment> equipments = troop.Loadout.Equipments;
			for (int i = 0; i < equipments.Count; i++)
			{
				if (!equipments[i].IsCivilian && CombatAgentBehavior.IsEnabled(troop, i, t))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0002A47C File Offset: 0x0002867C
		private bool CanDisable(WCharacter troop, int index, PolicyToggleType t)
		{
			return !CombatAgentBehavior.IsEnabled(troop, index, t) || t == PolicyToggleType.GenderOverride || this.CountEnabled(troop, t) > 1;
		}

		// Token: 0x04000236 RID: 566
		private Dictionary<string, Dictionary<int, EquipmentPolicy>> _byTroop = new Dictionary<string, Dictionary<int, EquipmentPolicy>>();
	}
}
