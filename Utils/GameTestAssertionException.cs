using System;

namespace Retinues.Utils
{
	// Token: 0x0200002F RID: 47
	public sealed class GameTestAssertionException : Exception
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x00005C1D File Offset: 0x00003E1D
		public GameTestAssertionException(string message) : base(message)
		{
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00005C26 File Offset: 0x00003E26
		public GameTestAssertionException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
