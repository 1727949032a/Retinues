using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.SaveSystem;

namespace Retinues.Troops.Save
{
	// Token: 0x0200003B RID: 59
	public class FactionSaveData
	{
		// Token: 0x06000131 RID: 305 RVA: 0x0000861B File Offset: 0x0000681B
		public FactionSaveData()
		{
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00008624 File Offset: 0x00006824
		public FactionSaveData(BaseFaction faction)
		{
			if (faction == null)
			{
				return;
			}
			this.RetinueElite = FactionSaveData.CreateIfNeeded(faction.RetinueElite);
			this.RetinueBasic = FactionSaveData.CreateIfNeeded(faction.RetinueBasic);
			this.RootElite = FactionSaveData.CreateIfNeeded(faction.RootElite);
			this.RootBasic = FactionSaveData.CreateIfNeeded(faction.RootBasic);
			this.MilitiaMelee = FactionSaveData.CreateIfNeeded(faction.MilitiaMelee);
			this.MilitiaMeleeElite = FactionSaveData.CreateIfNeeded(faction.MilitiaMeleeElite);
			this.MilitiaRanged = FactionSaveData.CreateIfNeeded(faction.MilitiaRanged);
			this.MilitiaRangedElite = FactionSaveData.CreateIfNeeded(faction.MilitiaRangedElite);
			this.CaravanGuard = FactionSaveData.CreateIfNeeded(faction.CaravanGuard);
			this.CaravanMaster = FactionSaveData.CreateIfNeeded(faction.CaravanMaster);
			this.Villager = FactionSaveData.CreateIfNeeded(faction.Villager);
			List<WCharacter> mercenaryTroops = faction.MercenaryTroops;
			List<TroopSaveData> list;
			if (mercenaryTroops == null)
			{
				list = null;
			}
			else
			{
				Func<WCharacter, TroopSaveData> selector;
				if ((selector = FactionSaveData.<>O.<0>__CreateIfNeeded) == null)
				{
					selector = (FactionSaveData.<>O.<0>__CreateIfNeeded = new Func<WCharacter, TroopSaveData>(FactionSaveData.CreateIfNeeded));
				}
				list = (from d in mercenaryTroops.Select(selector)
				where d != null
				select d).ToList<TroopSaveData>();
			}
			List<TroopSaveData> list2 = list;
			this.Mercenaries = ((list2 != null && list2.Count > 0) ? list2 : null);
			List<WCharacter> banditTroops = faction.BanditTroops;
			List<TroopSaveData> list3;
			if (banditTroops == null)
			{
				list3 = null;
			}
			else
			{
				Func<WCharacter, TroopSaveData> selector2;
				if ((selector2 = FactionSaveData.<>O.<0>__CreateIfNeeded) == null)
				{
					selector2 = (FactionSaveData.<>O.<0>__CreateIfNeeded = new Func<WCharacter, TroopSaveData>(FactionSaveData.CreateIfNeeded));
				}
				list3 = (from d in banditTroops.Select(selector2)
				where d != null
				select d).ToList<TroopSaveData>();
			}
			List<TroopSaveData> list4 = list3;
			this.Bandits = ((list4 != null && list4.Count > 0) ? list4 : null);
			List<WCharacter> civilianTroops = faction.CivilianTroops;
			List<TroopSaveData> list5;
			if (civilianTroops == null)
			{
				list5 = null;
			}
			else
			{
				Func<WCharacter, TroopSaveData> selector3;
				if ((selector3 = FactionSaveData.<>O.<0>__CreateIfNeeded) == null)
				{
					selector3 = (FactionSaveData.<>O.<0>__CreateIfNeeded = new Func<WCharacter, TroopSaveData>(FactionSaveData.CreateIfNeeded));
				}
				list5 = (from d in civilianTroops.Select(selector3)
				where d != null
				select d).ToList<TroopSaveData>();
			}
			List<TroopSaveData> list6 = list5;
			this.Civilians = ((list6 != null && list6.Count > 0) ? list6 : null);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00008840 File Offset: 0x00006A40
		public void Apply(WFaction faction = null)
		{
			if (faction == null)
			{
				TroopSaveData retinueElite = this.RetinueElite;
				if (retinueElite != null)
				{
					retinueElite.Deserialize();
				}
				TroopSaveData retinueBasic = this.RetinueBasic;
				if (retinueBasic != null)
				{
					retinueBasic.Deserialize();
				}
				TroopSaveData rootElite = this.RootElite;
				if (rootElite != null)
				{
					rootElite.Deserialize();
				}
				TroopSaveData rootBasic = this.RootBasic;
				if (rootBasic != null)
				{
					rootBasic.Deserialize();
				}
				TroopSaveData militiaMelee = this.MilitiaMelee;
				if (militiaMelee != null)
				{
					militiaMelee.Deserialize();
				}
				TroopSaveData militiaMeleeElite = this.MilitiaMeleeElite;
				if (militiaMeleeElite != null)
				{
					militiaMeleeElite.Deserialize();
				}
				TroopSaveData militiaRanged = this.MilitiaRanged;
				if (militiaRanged != null)
				{
					militiaRanged.Deserialize();
				}
				TroopSaveData militiaRangedElite = this.MilitiaRangedElite;
				if (militiaRangedElite != null)
				{
					militiaRangedElite.Deserialize();
				}
				TroopSaveData caravanGuard = this.CaravanGuard;
				if (caravanGuard != null)
				{
					caravanGuard.Deserialize();
				}
				TroopSaveData caravanMaster = this.CaravanMaster;
				if (caravanMaster != null)
				{
					caravanMaster.Deserialize();
				}
				TroopSaveData villager = this.Villager;
				if (villager != null)
				{
					villager.Deserialize();
				}
				if (this.Mercenaries != null)
				{
					foreach (TroopSaveData troopSaveData in this.Mercenaries)
					{
						troopSaveData.Deserialize();
					}
				}
				if (this.Bandits != null)
				{
					foreach (TroopSaveData troopSaveData2 in this.Bandits)
					{
						troopSaveData2.Deserialize();
					}
				}
				if (this.Civilians == null)
				{
					return;
				}
				using (List<TroopSaveData>.Enumerator enumerator = this.Civilians.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TroopSaveData troopSaveData3 = enumerator.Current;
						troopSaveData3.Deserialize();
					}
					return;
				}
			}
			Log.Info("Deserializing troop data for faction: " + faction.Name);
			TroopSaveData retinueElite2 = this.RetinueElite;
			if (retinueElite2 != null)
			{
				retinueElite2.Deserialize(faction, RootCategory.RetinueElite);
			}
			TroopSaveData retinueBasic2 = this.RetinueBasic;
			if (retinueBasic2 != null)
			{
				retinueBasic2.Deserialize(faction, RootCategory.RetinueBasic);
			}
			TroopSaveData rootElite2 = this.RootElite;
			if (rootElite2 != null)
			{
				rootElite2.Deserialize(faction, RootCategory.RootElite);
			}
			TroopSaveData rootBasic2 = this.RootBasic;
			if (rootBasic2 != null)
			{
				rootBasic2.Deserialize(faction, RootCategory.RootBasic);
			}
			TroopSaveData militiaMelee2 = this.MilitiaMelee;
			if (militiaMelee2 != null)
			{
				militiaMelee2.Deserialize(faction, RootCategory.MilitiaMelee);
			}
			TroopSaveData militiaMeleeElite2 = this.MilitiaMeleeElite;
			if (militiaMeleeElite2 != null)
			{
				militiaMeleeElite2.Deserialize(faction, RootCategory.MilitiaMeleeElite);
			}
			TroopSaveData militiaRanged2 = this.MilitiaRanged;
			if (militiaRanged2 != null)
			{
				militiaRanged2.Deserialize(faction, RootCategory.MilitiaRanged);
			}
			TroopSaveData militiaRangedElite2 = this.MilitiaRangedElite;
			if (militiaRangedElite2 != null)
			{
				militiaRangedElite2.Deserialize(faction, RootCategory.MilitiaRangedElite);
			}
			TroopSaveData caravanGuard2 = this.CaravanGuard;
			if (caravanGuard2 != null)
			{
				caravanGuard2.Deserialize(faction, RootCategory.CaravanGuard);
			}
			TroopSaveData caravanMaster2 = this.CaravanMaster;
			if (caravanMaster2 != null)
			{
				caravanMaster2.Deserialize(faction, RootCategory.CaravanMaster);
			}
			TroopSaveData villager2 = this.Villager;
			if (villager2 == null)
			{
				return;
			}
			villager2.Deserialize(faction, RootCategory.Villager);
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00008AF4 File Offset: 0x00006CF4
		private static TroopSaveData CreateIfNeeded(WCharacter troop)
		{
			if (troop == null || !troop.NeedsPersistence)
			{
				return null;
			}
			return new TroopSaveData(troop);
		}

		// Token: 0x0400005B RID: 91
		[SaveableField(1)]
		public TroopSaveData RetinueElite;

		// Token: 0x0400005C RID: 92
		[SaveableField(2)]
		public TroopSaveData RetinueBasic;

		// Token: 0x0400005D RID: 93
		[SaveableField(3)]
		public TroopSaveData RootElite;

		// Token: 0x0400005E RID: 94
		[SaveableField(4)]
		public TroopSaveData RootBasic;

		// Token: 0x0400005F RID: 95
		[SaveableField(5)]
		public TroopSaveData MilitiaMelee;

		// Token: 0x04000060 RID: 96
		[SaveableField(6)]
		public TroopSaveData MilitiaMeleeElite;

		// Token: 0x04000061 RID: 97
		[SaveableField(7)]
		public TroopSaveData MilitiaRanged;

		// Token: 0x04000062 RID: 98
		[SaveableField(8)]
		public TroopSaveData MilitiaRangedElite;

		// Token: 0x04000063 RID: 99
		[SaveableField(9)]
		public TroopSaveData CaravanGuard;

		// Token: 0x04000064 RID: 100
		[SaveableField(10)]
		public TroopSaveData CaravanMaster;

		// Token: 0x04000065 RID: 101
		[SaveableField(11)]
		public TroopSaveData Villager;

		// Token: 0x04000066 RID: 102
		[SaveableField(12)]
		public TroopSaveData PrisonGuard;

		// Token: 0x04000067 RID: 103
		[SaveableField(16)]
		public List<TroopSaveData> Mercenaries;

		// Token: 0x04000068 RID: 104
		[SaveableField(14)]
		public List<TroopSaveData> Bandits;

		// Token: 0x04000069 RID: 105
		[SaveableField(13)]
		public List<TroopSaveData> Civilians;

		// Token: 0x0400006A RID: 106
		[SaveableField(15)]
		public List<TroopSaveData> Heroes;

		// Token: 0x02000120 RID: 288
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000340 RID: 832
			public static Func<WCharacter, TroopSaveData> <0>__CreateIfNeeded;
		}
	}
}
