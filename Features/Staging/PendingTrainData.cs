using System;
using TaleWorlds.SaveSystem;

namespace Retinues.Features.Staging
{
	// Token: 0x020000C2 RID: 194
	[Serializable]
	public class PendingTrainData : IPendingData
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x060007F2 RID: 2034 RVA: 0x0002827E File Offset: 0x0002647E
		// (set) Token: 0x060007F3 RID: 2035 RVA: 0x00028286 File Offset: 0x00026486
		string IPendingData.TroopId
		{
			get
			{
				return this.TroopId;
			}
			set
			{
				this.TroopId = value;
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x060007F4 RID: 2036 RVA: 0x0002828F File Offset: 0x0002648F
		// (set) Token: 0x060007F5 RID: 2037 RVA: 0x00028297 File Offset: 0x00026497
		int IPendingData.Remaining
		{
			get
			{
				return this.Remaining;
			}
			set
			{
				this.Remaining = value;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x060007F6 RID: 2038 RVA: 0x000282A0 File Offset: 0x000264A0
		// (set) Token: 0x060007F7 RID: 2039 RVA: 0x000282A8 File Offset: 0x000264A8
		float IPendingData.Carry
		{
			get
			{
				return this.Carry;
			}
			set
			{
				this.Carry = value;
			}
		}

		// Token: 0x0400020F RID: 527
		[SaveableField(1)]
		public string TroopId;

		// Token: 0x04000210 RID: 528
		[SaveableField(2)]
		public int Remaining;

		// Token: 0x04000211 RID: 529
		[SaveableField(3)]
		public float Carry;

		// Token: 0x04000212 RID: 530
		[SaveableField(4)]
		public string SkillId;

		// Token: 0x04000213 RID: 531
		[SaveableField(5)]
		public int PointsRemaining;

		// Token: 0x04000214 RID: 532
		[SaveableField(6)]
		public float PointsPerHour;
	}
}
