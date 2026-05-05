using System;
using TaleWorlds.CampaignSystem;

namespace Retinues.Utils
{
	// Token: 0x02000030 RID: 48
	public sealed class GameTestContext
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00005C30 File Offset: 0x00003E30
		public void EnsureCampaign()
		{
			if (Campaign.Current == null)
			{
				throw new GameTestAssertionException("No active campaign. Load a test save before running tests.");
			}
		}
	}
}
