using System;
using Retinues.Configuration;
using Retinues.Game.Wrappers;
using Retinues.Utils;

namespace Retinues.Game
{
	// Token: 0x0200008D RID: 141
	[SafeClass]
	public abstract class BaseFactionMember : StringIdentifier
	{
		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600050A RID: 1290
		public abstract WFaction Clan { get; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600050B RID: 1291
		public abstract WFaction Kingdom { get; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001B0DC File Offset: 0x000192DC
		public WFaction PlayerFaction
		{
			get
			{
				WFaction clan = this.Clan;
				WFaction wfaction = (clan != null && clan.IsPlayerClan) ? Player.Clan : null;
				WFaction kingdom = this.Kingdom;
				WFaction wfaction2 = (kingdom != null && kingdom.IsPlayerKingdom) ? Player.Kingdom : null;
				if (Config.DisableKingdomTroops)
				{
					WFaction result;
					if ((result = wfaction) == null)
					{
						if (!(wfaction2 != null))
						{
							return null;
						}
						result = Player.Clan;
					}
					return result;
				}
				return wfaction ?? wfaction2;
			}
		}
	}
}
