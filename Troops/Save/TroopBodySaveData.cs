using System;
using Retinues.Game.Wrappers;
using TaleWorlds.SaveSystem;

namespace Retinues.Troops.Save
{
	// Token: 0x0200003D RID: 61
	public class TroopBodySaveData
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00008FE4 File Offset: 0x000071E4
		public TroopBodySaveData(WBody body)
		{
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000090A7 File Offset: 0x000072A7
		public TroopBodySaveData() : this(null)
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000090B0 File Offset: 0x000072B0
		public void Apply(WBody body)
		{
			body.SetDynamicEnd(true, new float?(this.AgeMin), new float?(this.WeightMin), new float?(this.BuildMin));
			body.SetDynamicEnd(false, new float?(this.AgeMax), new float?(this.WeightMax), new float?(this.BuildMax));
			body.Age = (body.AgeMin + body.AgeMax) / 2f;
			if (this.HeightMin > 0f && this.HeightMax > 0f)
			{
				body.HeightMin = this.HeightMin;
				body.HeightMax = this.HeightMax;
			}
		}

		// Token: 0x0400007B RID: 123
		[SaveableField(1)]
		public float AgeMin = (body != null) ? body.AgeMin : 0f;

		// Token: 0x0400007C RID: 124
		[SaveableField(2)]
		public float AgeMax = (body != null) ? body.AgeMax : 0f;

		// Token: 0x0400007D RID: 125
		[SaveableField(3)]
		public float WeightMin = (body != null) ? body.WeightMin : 0f;

		// Token: 0x0400007E RID: 126
		[SaveableField(4)]
		public float WeightMax = (body != null) ? body.WeightMax : 0f;

		// Token: 0x0400007F RID: 127
		[SaveableField(5)]
		public float BuildMin = (body != null) ? body.BuildMin : 0f;

		// Token: 0x04000080 RID: 128
		[SaveableField(6)]
		public float BuildMax = (body != null) ? body.BuildMax : 0f;

		// Token: 0x04000081 RID: 129
		[SaveableField(7)]
		public float HeightMin = (body != null) ? body.HeightMin : 0f;

		// Token: 0x04000082 RID: 130
		[SaveableField(8)]
		public float HeightMax = (body != null) ? body.HeightMax : 0f;
	}
}
