using System;
using System.Collections.Generic;

namespace Retinues.Configuration
{
	// Token: 0x020000F5 RID: 245
	public interface IOption
	{
		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000989 RID: 2441
		string Section { get; }

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600098A RID: 2442
		string Name { get; }

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x0600098B RID: 2443
		string Key { get; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x0600098C RID: 2444
		string Hint { get; }

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x0600098D RID: 2445
		Type Type { get; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x0600098E RID: 2446
		bool RequiresRestart { get; }

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x0600098F RID: 2447
		int MinValue { get; }

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000990 RID: 2448
		int MaxValue { get; }

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000991 RID: 2449
		object Default { get; }

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000992 RID: 2450
		IReadOnlyDictionary<string, object> PresetOverrides { get; }

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000993 RID: 2451
		bool IsDisabled { get; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000994 RID: 2452
		object DisabledOverrideBoxed { get; }

		// Token: 0x06000995 RID: 2453
		object GetObject();

		// Token: 0x06000996 RID: 2454
		void SetObject(object value);
	}
}
