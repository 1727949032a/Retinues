using System;
using Retinues.Game.Wrappers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A0 RID: 160
	public static class AgentHelper
	{
		// Token: 0x060006FC RID: 1788 RVA: 0x00022C64 File Offset: 0x00020E64
		public static WCharacter TroopFromAgentBuildData(AgentBuildData data, bool origin = false)
		{
			if (data == null)
			{
				return null;
			}
			BasicCharacterObject basicCharacterObject;
			if (!origin)
			{
				basicCharacterObject = data.AgentCharacter;
			}
			else
			{
				IAgentOriginBase agentOrigin = data.AgentOrigin;
				basicCharacterObject = ((agentOrigin != null) ? agentOrigin.Troop : null);
			}
			BasicCharacterObject basicCharacterObject2 = basicCharacterObject;
			if (basicCharacterObject2 == null)
			{
				BasicCharacterObject basicCharacterObject3;
				if (!origin)
				{
					IAgentOriginBase agentOrigin2 = data.AgentOrigin;
					basicCharacterObject3 = ((agentOrigin2 != null) ? agentOrigin2.Troop : null);
				}
				else
				{
					basicCharacterObject3 = data.AgentCharacter;
				}
				basicCharacterObject2 = basicCharacterObject3;
			}
			CharacterObject characterObject = basicCharacterObject2 as CharacterObject;
			if (characterObject == null)
			{
				return null;
			}
			return new WCharacter(characterObject);
		}
	}
}
