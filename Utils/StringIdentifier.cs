using System;

namespace Retinues.Utils
{
	// Token: 0x02000020 RID: 32
	[SafeClass]
	public abstract class StringIdentifier
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006C RID: 108
		public abstract string StringId { get; }

		// Token: 0x0600006D RID: 109 RVA: 0x000033DA File Offset: 0x000015DA
		public bool Equals(StringIdentifier other)
		{
			return other != null && this.StringId == ((other != null) ? other.StringId : null);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000033F8 File Offset: 0x000015F8
		public override bool Equals(object obj)
		{
			StringIdentifier stringIdentifier = obj as StringIdentifier;
			return stringIdentifier != null && this.Equals(stringIdentifier);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003418 File Offset: 0x00001618
		public override int GetHashCode()
		{
			return this.StringId.GetHashCode();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003425 File Offset: 0x00001625
		public override string ToString()
		{
			return this.StringId;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000342D File Offset: 0x0000162D
		public static bool operator ==(StringIdentifier left, StringIdentifier right)
		{
			if (left == null || right == null)
			{
				return left == null == (right == null);
			}
			return left == right || (left != null && right != null && left.Equals(right));
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003455 File Offset: 0x00001655
		public static bool operator !=(StringIdentifier left, StringIdentifier right)
		{
			return !(left == right);
		}
	}
}
