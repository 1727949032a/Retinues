using System;
using TaleWorlds.SaveSystem;

namespace Retinues.Features.Agents
{
	// Token: 0x020000CB RID: 203
	[Serializable]
	public class EquipmentPolicy
	{
		// Token: 0x0400022F RID: 559
		[SaveableField(1)]
		public bool FieldBattle;

		// Token: 0x04000230 RID: 560
		[SaveableField(5)]
		public bool NavalBattle;

		// Token: 0x04000231 RID: 561
		[SaveableField(2)]
		public bool SiegeDefense;

		// Token: 0x04000232 RID: 562
		[SaveableField(3)]
		public bool SiegeAssault;

		// Token: 0x04000233 RID: 563
		[SaveableField(4)]
		public bool GenderOverride;

		// Token: 0x04000234 RID: 564
		public static readonly EquipmentPolicy None = new EquipmentPolicy
		{
			FieldBattle = false,
			NavalBattle = false,
			SiegeDefense = false,
			SiegeAssault = false,
			GenderOverride = false
		};

		// Token: 0x04000235 RID: 565
		public static readonly EquipmentPolicy All = new EquipmentPolicy
		{
			FieldBattle = true,
			NavalBattle = true,
			SiegeDefense = true,
			SiegeAssault = true,
			GenderOverride = false
		};
	}
}
