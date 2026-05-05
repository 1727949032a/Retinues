using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace Retinues.Features.Statistics
{
	// Token: 0x020000BC RID: 188
	[Serializable]
	public sealed class TroopCombatStats
	{
		// Token: 0x040001F3 RID: 499
		[SaveableField(1)]
		public string TroopId;

		// Token: 0x040001F4 RID: 500
		[SaveableField(2)]
		public int TotalBattles;

		// Token: 0x040001F5 RID: 501
		[SaveableField(3)]
		public int BattlesWon;

		// Token: 0x040001F6 RID: 502
		[SaveableField(4)]
		public int BattlesLost;

		// Token: 0x040001F7 RID: 503
		[SaveableField(5)]
		public int FieldBattles;

		// Token: 0x040001F8 RID: 504
		[SaveableField(6)]
		public int SiegeBattles;

		// Token: 0x040001F9 RID: 505
		[SaveableField(7)]
		public int HideoutBattles;

		// Token: 0x040001FA RID: 506
		[SaveableField(8)]
		public int VillageRaidBattles;

		// Token: 0x040001FB RID: 507
		[SaveableField(9)]
		public int OtherBattles;

		// Token: 0x040001FC RID: 508
		[SaveableField(10)]
		public int TotalKills;

		// Token: 0x040001FD RID: 509
		[SaveableField(11)]
		public int TotalDeaths;

		// Token: 0x040001FE RID: 510
		[SaveableField(12)]
		public Dictionary<string, int> KillsByTroopId = new Dictionary<string, int>();

		// Token: 0x040001FF RID: 511
		[SaveableField(13)]
		public Dictionary<string, int> DeathsByTroopId = new Dictionary<string, int>();

		// Token: 0x04000200 RID: 512
		[SaveableField(14)]
		public Dictionary<string, int> FactionsFought = new Dictionary<string, int>();
	}
}
