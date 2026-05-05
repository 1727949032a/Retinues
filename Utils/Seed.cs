using System;
using System.Text;

namespace Retinues.Utils
{
	// Token: 0x0200002B RID: 43
	public static class Seed
	{
		// Token: 0x060000DA RID: 218 RVA: 0x000059A4 File Offset: 0x00003BA4
		public static int FromString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			uint num = 2166136261U;
			foreach (byte b in bytes)
			{
				num ^= (uint)b;
				num *= 16777619U;
			}
			return (int)(num & 2147483647U);
		}
	}
}
