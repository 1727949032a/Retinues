using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Retinues.Doctrines
{
	// Token: 0x020000D7 RID: 215
	[SafeClass]
	public sealed class FeatServiceBehavior : CampaignBehaviorBase
	{
		// Token: 0x060008B8 RID: 2232 RVA: 0x0002C083 File Offset: 0x0002A283
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x0002C088 File Offset: 0x0002A288
		public override void RegisterEvents()
		{
			Log.Debug("Registering FeatServiceBehavior events.");
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, delegate(CampaignGameStarter _)
			{
				DoctrineAPI.AddCatalogBuiltListener(new Action(this.RefreshActiveFeats));
				DoctrineAPI.AddFeatCompletedListener(delegate(string _)
				{
					this.RefreshActiveFeats();
				});
				DoctrineAPI.AddDoctrineUnlockedListener(delegate(string _)
				{
					this.RefreshActiveFeats();
				});
				Config.OptionChanged += this.OnConfigOptionChanged;
			});
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnUnitRecruitedEvent.AddNonSerializedListener(this, new Action<CharacterObject, int>(this.OnUnitRecruited));
			CampaignEvents.PlayerUpgradedTroopsEvent.AddNonSerializedListener(this, new Action<CharacterObject, CharacterObject, int>(this.PlayerUpgradedTroops));
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x0002C157 File Offset: 0x0002A357
		private void OnConfigOptionChanged(string key, object value)
		{
			this.RefreshActiveFeats();
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x0002C15F File Offset: 0x0002A35F
		private void OnDailyTick()
		{
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.OnDailyTick();
			}, Array.Empty<object>());
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0002C18C File Offset: 0x0002A38C
		private void OnMissionStarted(IMission iMission)
		{
			Mission mission = iMission as Mission;
			if (mission == null)
			{
				return;
			}
			MobileParty mainParty = MobileParty.MainParty;
			MapEvent mapEvent = (mainParty != null) ? mainParty.MapEvent : null;
			if (mapEvent != null && mapEvent.IsPlayerMapEvent)
			{
				Log.Info("Mission Type: Battle");
				Battle battle = mission.GetMissionBehavior<Battle>();
				if (battle == null)
				{
					battle = new Battle(null);
					mission.AddMissionBehavior(battle);
				}
				if (mission.GetMissionBehavior<FeatServiceBehavior.MissionEndRelay>() == null)
				{
					mission.AddMissionBehavior(new FeatServiceBehavior.MissionEndRelay(this));
				}
				this.NotifyFeats(delegate(Feat feat, object[] args)
				{
					feat.OnBattleStart((Battle)args[0]);
				}, new object[]
				{
					battle
				});
				return;
			}
			if (mission.CombatType == Mission.MissionCombatType.Combat && mission.Mode == MissionMode.StartUp)
			{
				Log.Info("Mission Type: Combat");
				Combat combat = mission.GetMissionBehavior<Combat>();
				if (combat == null)
				{
					combat = new Combat();
					mission.AddMissionBehavior(combat);
				}
				if (mission.GetMissionBehavior<FeatServiceBehavior.MissionEndRelay>() == null)
				{
					mission.AddMissionBehavior(new FeatServiceBehavior.MissionEndRelay(this));
				}
				this.NotifyFeats(delegate(Feat feat, object[] args)
				{
					feat.OnArenaStart((Combat)args[0]);
				}, new object[]
				{
					combat
				});
			}
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0002C29C File Offset: 0x0002A49C
		internal void OnMissionEnded(Mission mission)
		{
			Battle battle = (mission != null) ? mission.GetMissionBehavior<Battle>() : null;
			if (battle != null)
			{
				this.NotifyFeats(delegate(Feat feat, object[] args)
				{
					feat.OnBattleEnd((Battle)args[0]);
				}, new object[]
				{
					battle
				});
				battle.LogReport();
				return;
			}
			Combat combat = (mission != null) ? mission.GetMissionBehavior<Combat>() : null;
			if (combat != null)
			{
				this.NotifyFeats(delegate(Feat feat, object[] args)
				{
					feat.OnArenaEnd((Combat)args[0]);
				}, new object[]
				{
					combat
				});
			}
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return;
			}
			FeatNotificationBehavior campaignBehavior = campaign.GetCampaignBehavior<FeatNotificationBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			campaignBehavior.TryFlush();
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0002C348 File Offset: 0x0002A548
		internal void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			Tournament tournament = new Tournament(new WSettlement((town != null) ? town.Settlement : null), new WCharacter(winner), (from p in participants.ToList<CharacterObject>()
			select new WCharacter(p)).ToList<WCharacter>());
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.OnTournamentFinished((Tournament)args[0]);
			}, new object[]
			{
				tournament
			});
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return;
			}
			FeatNotificationBehavior campaignBehavior = campaign.GetCampaignBehavior<FeatNotificationBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			campaignBehavior.TryFlush();
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0002C3EC File Offset: 0x0002A5EC
		private void OnSettlementOwnerChanged(Settlement s, bool _, Hero n, Hero o, Hero __, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail d)
		{
			if (s == null)
			{
				return;
			}
			if (!new WFaction((n != null) ? n.Clan : null).IsPlayerClan)
			{
				return;
			}
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.OnSettlementOwnerChanged((SettlementOwnerChange)args[0]);
			}, new object[]
			{
				new SettlementOwnerChange(new WSettlement(s), d, new WHero(o), new WHero(n))
			});
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x0002C45E File Offset: 0x0002A65E
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.OnQuestCompleted((Quest)args[0]);
			}, new object[]
			{
				new Quest(quest, details == QuestBase.QuestCompleteDetails.Success)
			});
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0002C498 File Offset: 0x0002A698
		private void OnUnitRecruited(CharacterObject troop, int amount)
		{
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.OnTroopRecruited((WCharacter)args[0], (int)args[1]);
			}, new object[]
			{
				new WCharacter(troop),
				amount
			});
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x0002C4D8 File Offset: 0x0002A6D8
		private void PlayerUpgradedTroops(CharacterObject upgradeFromTroop, CharacterObject upgradeToTroop, int number)
		{
			this.NotifyFeats(delegate(Feat feat, object[] args)
			{
				feat.PlayerUpgradedTroops((WCharacter)args[0], (WCharacter)args[1], (int)args[2]);
			}, new object[]
			{
				new WCharacter(upgradeFromTroop),
				new WCharacter(upgradeToTroop),
				number
			});
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002C52C File Offset: 0x0002A72C
		private void RefreshActiveFeats()
		{
			this._activeFeats.Clear();
			if (!Config.EnableFeatRequirements)
			{
				return;
			}
			IReadOnlyList<DoctrineDefinition> readOnlyList = DoctrineAPI.AllDoctrines();
			if (readOnlyList == null || readOnlyList.Count == 0)
			{
				return;
			}
			foreach (DoctrineDefinition doctrineDefinition in readOnlyList)
			{
				DoctrineStatus doctrineStatus = DoctrineAPI.GetDoctrineStatus(doctrineDefinition.Key);
				if (doctrineStatus != DoctrineStatus.Unlocked && doctrineStatus != DoctrineStatus.Locked)
				{
					foreach (FeatDefinition featDefinition in doctrineDefinition.Feats)
					{
						if (!DoctrineAPI.IsFeatComplete(featDefinition.Key))
						{
							Type typeByFullName = FeatServiceBehavior.GetTypeByFullName(featDefinition.Key);
							if (!(typeByFullName == null))
							{
								Feat feat = Activator.CreateInstance(typeByFullName) as Feat;
								if (feat != null)
								{
									this._activeFeats.Add(feat);
								}
							}
						}
					}
				}
			}
			string str = string.Join(", ", from f in this._activeFeats
			select f.GetType().FullName);
			Log.Info("Active feats refreshed: " + str + ".");
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0002C680 File Offset: 0x0002A880
		private static Type GetTypeByFullName(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				return null;
			}
			Type type = Type.GetType(fullName, false);
			if (type != null)
			{
				return type;
			}
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					type = assembly.GetType(fullName, false);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
			return null;
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
		private void NotifyFeats(Action<Feat, object[]> action, params object[] args)
		{
			if (!Config.EnableFeatRequirements)
			{
				return;
			}
			foreach (Feat arg in this._activeFeats.ToList<Feat>())
			{
				try
				{
					action(arg, args);
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}
		}

		// Token: 0x04000255 RID: 597
		private readonly List<Feat> _activeFeats = new List<Feat>();

		// Token: 0x020001A0 RID: 416
		private sealed class MissionEndRelay : MissionBehavior
		{
			// Token: 0x06000C8B RID: 3211 RVA: 0x00035B9F File Offset: 0x00033D9F
			public MissionEndRelay(FeatServiceBehavior host)
			{
			}

			// Token: 0x17000479 RID: 1145
			// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00035BAE File Offset: 0x00033DAE
			public override MissionBehaviorType BehaviorType
			{
				get
				{
					return MissionBehaviorType.Other;
				}
			}

			// Token: 0x06000C8D RID: 3213 RVA: 0x00035BB1 File Offset: 0x00033DB1
			protected override void OnEndMission()
			{
				FeatServiceBehavior host = this._host;
				if (host == null)
				{
					return;
				}
				host.OnMissionEnded(base.Mission);
			}

			// Token: 0x04000507 RID: 1287
			private readonly FeatServiceBehavior _host = host;
		}
	}
}
