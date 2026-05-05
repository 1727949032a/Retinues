using System;
using TaleWorlds.Library;

namespace Retinues.Utils
{
	// Token: 0x02000036 RID: 54
	public static class BannerlordVersion
	{
		// Token: 0x06000101 RID: 257 RVA: 0x000067F8 File Offset: 0x000049F8
		public static bool Is12()
		{
			return BannerlordVersion.Version.Major == 1 && BannerlordVersion.Version.Minor == 2;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006828 File Offset: 0x00004A28
		public static bool Is13()
		{
			return BannerlordVersion.Version.Major == 1 && BannerlordVersion.Version.Minor == 3;
		}

		// Token: 0x0400004E RID: 78
		public static readonly ApplicationVersion Version = ApplicationVersion.FromParametersFile(null);
	}
}
