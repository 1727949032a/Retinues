using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Troops.Save;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace Retinues.Troops
{
	// Token: 0x02000037 RID: 55
	[SafeClass]
	public class FactionBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00006864 File Offset: 0x00004A64
		public override void SyncData(IDataStore ds)
		{
			ds.SyncData<FactionSaveData>("Retinues_ClanTroops", ref this._clanTroops);
			ds.SyncData<FactionSaveData>("Retinues_KingdomTroops", ref this._kingdomTroops);
			ds.SyncData<List<FactionSaveData>>("Retinues_CultureTroops", ref this._cultureTroops);
			ds.SyncData<List<FactionSaveData>>("Retinues_MinorClanTroops", ref this._minorClanTroops);
			if (ds.IsSaving)
			{
				TroopImportExport.MakeBackup();
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000068C8 File Offset: 0x00004AC8
		public override void RegisterEvents()
		{
			CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(this.OnBeforeSave));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.ClanUnlock));
			CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomUnlock));
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006934 File Offset: 0x00004B34
		private void OnBeforeSave()
		{
			if (Player.Clan != null)
			{
				this._clanTroops = new FactionSaveData(Player.Clan);
			}
			if (Player.Kingdom != null)
			{
				this._kingdomTroops = new FactionSaveData(Player.Kingdom);
			}
			if (FactionBehavior.ResetCultureTroops)
			{
				FactionBehavior.ResetCultureTroops = false;
				this._cultureTroops = null;
				this._minorClanTroops = null;
				return;
			}
			if (Config.EnableGlobalEditor)
			{
				MBReadOnlyList<CultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<CultureObject>();
				List<CultureObject> list = ((objectTypeList != null) ? objectTypeList.ToList<CultureObject>() : null) ?? new List<CultureObject>();
				this._cultureTroops = new List<FactionSaveData>();
				foreach (CultureObject culture in list)
				{
					this._cultureTroops.Add(new FactionSaveData(new WCulture(culture)));
				}
				this._minorClanTroops = new List<FactionSaveData>();
				foreach (Clan clan in Clan.All)
				{
					if (clan.IsMinorFaction)
					{
						this._minorClanTroops.Add(new FactionSaveData(new WClan(clan)));
					}
				}
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006A78 File Offset: 0x00004C78
		private void OnGameLoaded(CampaignGameStarter _)
		{
			FactionBehavior.ResetCultureTroops = false;
			FactionSaveData clanTroops = this._clanTroops;
			if (clanTroops != null)
			{
				clanTroops.Apply(Player.Clan);
			}
			FactionSaveData kingdomTroops = this._kingdomTroops;
			if (kingdomTroops != null)
			{
				kingdomTroops.Apply(Player.Kingdom);
			}
			if (Config.EnableGlobalEditor)
			{
				if (this._cultureTroops != null)
				{
					foreach (FactionSaveData factionSaveData in this._cultureTroops)
					{
						factionSaveData.Apply(null);
					}
				}
				if (this._minorClanTroops != null)
				{
					foreach (FactionSaveData factionSaveData2 in this._minorClanTroops)
					{
						factionSaveData2.Apply(null);
					}
				}
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00006B58 File Offset: 0x00004D58
		private void ClanUnlock(Settlement s, bool _, Hero n, Hero o, Hero __, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail d)
		{
			if (!new WFaction((n != null) ? n.Clan : null).IsPlayerClan)
			{
				return;
			}
			Log.Debug(string.Format("Fief acquired: {0}, ensuring troops exist.", s.Name));
			TroopBuilder.EnsureTroopsExist(Player.Clan);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00006B92 File Offset: 0x00004D92
		private void KingdomUnlock(Kingdom k)
		{
			if (!new WFaction(k).IsPlayerKingdom)
			{
				return;
			}
			Log.Debug(string.Format("Kingdom created: {0}, ensuring troops exist.", k.Name));
			TroopBuilder.EnsureTroopsExist(Player.Kingdom);
		}

		// Token: 0x0400004F RID: 79
		public static bool ResetCultureTroops;

		// Token: 0x04000050 RID: 80
		private FactionSaveData _clanTroops;

		// Token: 0x04000051 RID: 81
		private FactionSaveData _kingdomTroops;

		// Token: 0x04000052 RID: 82
		private List<FactionSaveData> _cultureTroops;

		// Token: 0x04000053 RID: 83
		private List<FactionSaveData> _minorClanTroops;
	}
}
