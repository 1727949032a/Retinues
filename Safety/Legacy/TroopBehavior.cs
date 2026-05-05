using System;
using System.Collections.Generic;
using Retinues.Game;
using Retinues.Troops.Save;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000055 RID: 85
	public class TroopBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000B604 File Offset: 0x00009804
		public List<LegacyTroopSaveData> TroopData
		{
			get
			{
				List<LegacyTroopSaveData> result;
				if ((result = this._troopData) == null)
				{
					result = (this._troopData = new List<LegacyTroopSaveData>());
				}
				return result;
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000B62C File Offset: 0x0000982C
		public override void SyncData(IDataStore ds)
		{
			if (ds.IsSaving)
			{
				return;
			}
			ds.SyncData<List<LegacyTroopSaveData>>("Retinues_Troops_Data", ref this._troopData);
			if (this.TroopData.Count == 0)
			{
				Log.Debug("No legacy custom roots found in save.");
				return;
			}
			ValueTuple<FactionSaveData, FactionSaveData> valueTuple = LegacyTroopSaveConverter.ConvertLegacyFactionData(this.TroopData);
			FactionSaveData item = valueTuple.Item1;
			FactionSaveData item2 = valueTuple.Item2;
			Log.Info("Applying migrated troop data to factions...");
			item.Apply(Player.Clan);
			item2.Apply(Player.Kingdom);
			Log.Debug(string.Format("Migrated {0} legacy root troops.", this.TroopData.Count));
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000B6C1 File Offset: 0x000098C1
		public override void RegisterEvents()
		{
		}

		// Token: 0x040000A4 RID: 164
		private List<LegacyTroopSaveData> _troopData;
	}
}
