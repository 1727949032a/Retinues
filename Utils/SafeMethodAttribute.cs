using System;

namespace Retinues.Utils
{
	// Token: 0x02000026 RID: 38
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class SafeMethodAttribute : Attribute
	{
		// Token: 0x0600009D RID: 157 RVA: 0x00004A15 File Offset: 0x00002C15
		public SafeMethodAttribute(object fallback = null, bool swallow = true, Type fallbackType = null)
		{
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00004A32 File Offset: 0x00002C32
		public object Fallback { get; } = fallback;

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004A3A File Offset: 0x00002C3A
		public Type FallbackType { get; } = fallbackType;

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00004A42 File Offset: 0x00002C42
		public bool Swallow { get; } = swallow;
	}
}
