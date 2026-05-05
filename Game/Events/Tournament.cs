using System;
using System.Collections.Generic;
using Retinues.Game.Wrappers;
using Retinues.Utils;

namespace Retinues.Game.Events
{
	// Token: 0x020000AC RID: 172
	[SafeClass]
	public class Tournament
	{
		// Token: 0x06000747 RID: 1863 RVA: 0x00024E5F File Offset: 0x0002305F
		public Tournament(WSettlement town, WCharacter winner, List<WCharacter> participants)
		{
		}

		// Token: 0x040001DE RID: 478
		public WSettlement Town = town;

		// Token: 0x040001DF RID: 479
		public WCharacter Winner = winner;

		// Token: 0x040001E0 RID: 480
		public List<WCharacter> Participants = participants ?? new List<WCharacter>();
	}
}
