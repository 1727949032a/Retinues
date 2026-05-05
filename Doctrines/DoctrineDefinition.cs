using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace Retinues.Doctrines
{
	// Token: 0x020000D1 RID: 209
	public sealed class DoctrineDefinition
	{
		// Token: 0x04000242 RID: 578
		public string Key;

		// Token: 0x04000243 RID: 579
		public TextObject Name;

		// Token: 0x04000244 RID: 580
		public TextObject Description;

		// Token: 0x04000245 RID: 581
		public int Column;

		// Token: 0x04000246 RID: 582
		public int Row;

		// Token: 0x04000247 RID: 583
		public string PrerequisiteKey;

		// Token: 0x04000248 RID: 584
		public int GoldCost;

		// Token: 0x04000249 RID: 585
		public int InfluenceCost;

		// Token: 0x0400024A RID: 586
		public List<FeatDefinition> Feats = new List<FeatDefinition>();
	}
}
