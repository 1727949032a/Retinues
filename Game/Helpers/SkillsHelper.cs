using System;
using System.Collections.Generic;
using Retinues.Utils;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A6 RID: 166
	[SafeClass]
	public static class SkillsHelper
	{
		// Token: 0x040001C6 RID: 454
		public static readonly HashSet<string> VanillaSkillIds = new HashSet<string>
		{
			"OneHanded",
			"TwoHanded",
			"Polearm",
			"Bow",
			"Crossbow",
			"Throwing",
			"Riding",
			"Athletics",
			"Crafting",
			"Tactics",
			"Scouting",
			"Roguery",
			"Charm",
			"Leadership",
			"Trade",
			"Steward",
			"Medicine",
			"Engineering"
		};

		// Token: 0x040001C7 RID: 455
		public static readonly HashSet<string> NavalDLCSkillIds = new HashSet<string>
		{
			"Mariner",
			"Boatswain",
			"Shipmaster"
		};
	}
}
