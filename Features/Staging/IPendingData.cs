using System;

namespace Retinues.Features.Staging
{
	// Token: 0x020000BE RID: 190
	public interface IPendingData
	{
		// Token: 0x1700035C RID: 860
		// (get) Token: 0x0600079E RID: 1950
		// (set) Token: 0x0600079F RID: 1951
		string TroopId { get; set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x060007A0 RID: 1952
		// (set) Token: 0x060007A1 RID: 1953
		int Remaining { get; set; }

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x060007A2 RID: 1954
		// (set) Token: 0x060007A3 RID: 1955
		float Carry { get; set; }
	}
}
