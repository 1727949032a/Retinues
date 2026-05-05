using System;

namespace Retinues.Utils
{
	// Token: 0x02000031 RID: 49
	public sealed class GameTestResult
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x00005C4C File Offset: 0x00003E4C
		public GameTestResult(string name, string group, string description, bool passed, string message, Exception exception, TimeSpan duration)
		{
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00005C89 File Offset: 0x00003E89
		public string Name { get; } = name;

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00005C91 File Offset: 0x00003E91
		public string Group { get; } = group;

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00005C99 File Offset: 0x00003E99
		public string Description { get; } = description;

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00005CA1 File Offset: 0x00003EA1
		public bool Passed { get; } = passed;

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00005CA9 File Offset: 0x00003EA9
		public string Message { get; } = message;

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00005CB1 File Offset: 0x00003EB1
		public Exception Exception { get; } = exception;

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00005CB9 File Offset: 0x00003EB9
		public TimeSpan Duration { get; } = duration;
	}
}
