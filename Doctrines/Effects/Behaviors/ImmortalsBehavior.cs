using System;
using System.Collections.Generic;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Doctrines.Effects.Behaviors
{
	// Token: 0x020000DC RID: 220
	[SafeClass]
	public sealed class ImmortalsBehavior : MissionBehavior
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0002CA74 File Offset: 0x0002AC74
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Other;
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x0002CA78 File Offset: 0x0002AC78
		private bool Enabled
		{
			get
			{
				if (DoctrineAPI.IsDoctrineUnlocked<Immortals>())
				{
					MobileParty mainParty = MobileParty.MainParty;
					if (((mainParty != null) ? mainParty.MapEvent : null) != null && MobileParty.MainParty.MapEvent.IsPlayerMapEvent)
					{
						Mission mission = base.Mission;
						return ((mission != null) ? mission.MainAgent : null) != null;
					}
				}
				return false;
			}
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x0002CAC8 File Offset: 0x0002ACC8
		public override void OnAgentRemoved(Agent victim, Agent killer, AgentState state, KillingBlow blow)
		{
			if (!this.Enabled)
			{
				return;
			}
			if (victim == null || !victim.IsHuman)
			{
				return;
			}
			if (state != AgentState.Killed)
			{
				return;
			}
			if (victim.Team != base.Mission.MainAgent.Team)
			{
				return;
			}
			BasicCharacterObject character = victim.Character;
			if (character == null || character.IsHero)
			{
				return;
			}
			CharacterObject characterObject = character as CharacterObject;
			if (characterObject == null)
			{
				return;
			}
			if (!new WCharacter(characterObject).IsRetinue)
			{
				return;
			}
			int num;
			this._retinueDeaths.TryGetValue(characterObject, out num);
			this._retinueDeaths[characterObject] = num + 1;
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x0002CB54 File Offset: 0x0002AD54
		protected override void OnEndMission()
		{
			if (!this.Enabled)
			{
				return;
			}
			MobileParty mainParty = MobileParty.MainParty;
			TroopRoster troopRoster = (mainParty != null) ? mainParty.MemberRoster : null;
			if (troopRoster == null || this._retinueDeaths.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<CharacterObject, int> keyValuePair in this._retinueDeaths)
			{
				CharacterObject key = keyValuePair.Key;
				int value = keyValuePair.Value;
				int num = 0;
				for (int i = 0; i < value; i++)
				{
					if (MBRandom.RandomFloat < 0.2f)
					{
						num++;
					}
				}
				if (num > 0 && key != null)
				{
					try
					{
						Log.Info(string.Format("Immortals: Restoring {0} of {1} fallen retinue '{2}'", num, value, key.Name));
						troopRoster.AddToCounts(key, num, false, num, 0, true, -1);
					}
					catch
					{
						troopRoster.AddToCounts(key, num, false, num, 0, true, -1);
					}
				}
			}
			this._retinueDeaths.Clear();
		}

		// Token: 0x04000256 RID: 598
		private const float SurvivalChance = 0.2f;

		// Token: 0x04000257 RID: 599
		private readonly Dictionary<CharacterObject, int> _retinueDeaths = new Dictionary<CharacterObject, int>();
	}
}
