using System;

namespace Retinues.Utils
{
	// Token: 0x0200002E RID: 46
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GameTestAttribute : Attribute
	{
		// Token: 0x060000DE RID: 222 RVA: 0x00005BE8 File Offset: 0x00003DE8
		public GameTestAttribute(string name, string group = "default", string description = null)
		{
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00005C05 File Offset: 0x00003E05
		public string Name { get; } = name;

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00005C0D File Offset: 0x00003E0D
		public string Group { get; } = group;

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00005C15 File Offset: 0x00003E15
		public string Description { get; } = description;
	}
}
