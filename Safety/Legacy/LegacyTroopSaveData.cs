using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.SaveSystem;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000053 RID: 83
	public sealed class LegacyTroopSaveData
	{
		// Token: 0x04000090 RID: 144
		[SaveableField(1)]
		public string StringId;

		// Token: 0x04000091 RID: 145
		[SaveableField(2)]
		public string VanillaStringId;

		// Token: 0x04000092 RID: 146
		[SaveableField(3)]
		public string Name;

		// Token: 0x04000093 RID: 147
		[SaveableField(4)]
		public int Level;

		// Token: 0x04000094 RID: 148
		[SaveableField(5)]
		public bool IsFemale;

		// Token: 0x04000095 RID: 149
		[SaveableField(6)]
		public string SkillCode;

		// Token: 0x04000096 RID: 150
		[SaveableField(7)]
		public string EquipmentCode;

		// Token: 0x04000097 RID: 151
		[SaveableField(8)]
		[XmlArray("UpgradeTargets")]
		[XmlArrayItem("TroopSaveData")]
		public List<LegacyTroopSaveData> UpgradeTargets = new List<LegacyTroopSaveData>();

		// Token: 0x04000098 RID: 152
		[SaveableField(9)]
		public int XpPool;

		// Token: 0x04000099 RID: 153
		[SaveableField(10)]
		public List<string> EquipmentCodes = new List<string>();

		// Token: 0x0400009A RID: 154
		[SaveableField(11)]
		public string CultureId;

		// Token: 0x0400009B RID: 155
		[SaveableField(12)]
		public float AgeMin;

		// Token: 0x0400009C RID: 156
		[SaveableField(13)]
		public float AgeMax;

		// Token: 0x0400009D RID: 157
		[SaveableField(14)]
		public float WeightMin;

		// Token: 0x0400009E RID: 158
		[SaveableField(15)]
		public float WeightMax;

		// Token: 0x0400009F RID: 159
		[SaveableField(16)]
		public float BuildMin;

		// Token: 0x040000A0 RID: 160
		[SaveableField(17)]
		public float BuildMax;

		// Token: 0x040000A1 RID: 161
		[SaveableField(18)]
		public float HeightMin;

		// Token: 0x040000A2 RID: 162
		[SaveableField(19)]
		public float HeightMax;

		// Token: 0x040000A3 RID: 163
		[SaveableField(20)]
		public int Race;
	}
}
