using System;
using Retinues.Game.Events;
using Retinues.Utils;

namespace Retinues.Features.Experience
{
	// Token: 0x020000C5 RID: 197
	[SafeClass]
	public class BattleMissionXpBehavior : Combat
	{
		// Token: 0x06000819 RID: 2073 RVA: 0x00028D80 File Offset: 0x00026F80
		protected override void OnEndMission()
		{
			Log.Info("BattleMissionXpBehavior: OnEndMission - awarding XP for kills.");
			foreach (Combat.Kill kill in base.Kills)
			{
				if (kill.KillerIsPlayerTroop && kill.Killer.IsCustom)
				{
					int num = (kill.Victim.Tier + 1) * 10;
					if (num > 0)
					{
						TroopXpBehavior.Add(kill.Killer, num, false);
					}
				}
			}
			Log.Info("BattleMissionXpBehavior: OnEndMission complete.");
		}

		// Token: 0x0400021B RID: 539
		private const int XpPerTier = 10;
	}
}
