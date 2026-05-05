using System;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.MountAndBlade;

namespace Retinues.Doctrines.Effects.Behaviors
{
	// Token: 0x020000DD RID: 221
	[SafeClass]
	public sealed class IndomitableBehavior : MissionBehavior
	{
		// Token: 0x1700039B RID: 923
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x0002CC87 File Offset: 0x0002AE87
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Other;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0002CC8C File Offset: 0x0002AE8C
		private bool Enabled
		{
			get
			{
				if (!DoctrineAPI.IsDoctrineUnlocked<Indomitable>())
				{
					return false;
				}
				MobileParty mainParty = MobileParty.MainParty;
				if (mainParty == null)
				{
					return false;
				}
				MapEvent mapEvent = mainParty.MapEvent;
				return ((mapEvent != null) ? new bool?(mapEvent.IsPlayerMapEvent) : null).GetValueOrDefault();
			}
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x0002CCD4 File Offset: 0x0002AED4
		public override void OnAgentCreated(Agent agent)
		{
			if (!this.Enabled || agent == null || !agent.IsHuman)
			{
				return;
			}
			Team team = agent.Team;
			Agent mainAgent = base.Mission.MainAgent;
			if (team != ((mainAgent != null) ? mainAgent.Team : null))
			{
				return;
			}
			CharacterObject characterObject = agent.Character as CharacterObject;
			if (characterObject == null || characterObject.IsHero)
			{
				return;
			}
			if (!new WCharacter(characterObject).IsRetinue)
			{
				return;
			}
			agent.Health += 5f;
		}

		// Token: 0x04000258 RID: 600
		private const float Bonus = 5f;
	}
}
