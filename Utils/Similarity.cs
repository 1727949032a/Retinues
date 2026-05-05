using System;
using System.Collections.Generic;
using System.Linq;

namespace Retinues.Utils
{
	// Token: 0x0200002C RID: 44
	public static class Similarity
	{
		// Token: 0x060000DB RID: 219 RVA: 0x000059F4 File Offset: 0x00003BF4
		public static double Jaccard(HashSet<string> a, HashSet<string> b)
		{
			if (a.Count == 0 && b.Count == 0)
			{
				return 1.0;
			}
			if (a.Count == 0 || b.Count == 0)
			{
				return 0.0;
			}
			int num = a.Count((string s) => b.Contains(s));
			int num2 = a.Count + b.Count - num;
			if (num2 != 0)
			{
				return (double)num / (double)num2;
			}
			return 0.0;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005A88 File Offset: 0x00003C88
		public static double Cosine(Dictionary<string, int> a, Dictionary<string, int> b)
		{
			if (a.Count == 0 && b.Count == 0)
			{
				return 1.0;
			}
			if (a.Count == 0 || b.Count == 0)
			{
				return 0.0;
			}
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			foreach (KeyValuePair<string, int> keyValuePair in a)
			{
				long num4 = (long)keyValuePair.Value;
				num2 += num4 * num4;
				int num5;
				if (b.TryGetValue(keyValuePair.Key, out num5))
				{
					num += num4 * (long)num5;
				}
			}
			foreach (int num6 in b.Values)
			{
				long num7 = (long)num6;
				num3 += num7 * num7;
			}
			if (num2 == 0L || num3 == 0L)
			{
				return 0.0;
			}
			return (double)num / (Math.Sqrt((double)num2) * Math.Sqrt((double)num3));
		}
	}
}
