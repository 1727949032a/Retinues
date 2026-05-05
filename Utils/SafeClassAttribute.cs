using System;
using System.Collections.Generic;

namespace Retinues.Utils
{
	// Token: 0x02000027 RID: 39
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class SafeClassAttribute : Attribute
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004A4A File Offset: 0x00002C4A
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004A52 File Offset: 0x00002C52
		public bool PublicOnly { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00004A5B File Offset: 0x00002C5B
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00004A63 File Offset: 0x00002C63
		public bool IncludeAccessors { get; set; } = true;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004A6C File Offset: 0x00002C6C
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00004A74 File Offset: 0x00002C74
		public bool IncludeDerived { get; set; } = true;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00004A7D File Offset: 0x00002C7D
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00004A85 File Offset: 0x00002C85
		public bool SwallowByDefault { get; set; } = true;

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004A8E File Offset: 0x00002C8E
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00004A96 File Offset: 0x00002C96
		public bool UseIntFallback { get; set; } = true;

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00004A9F File Offset: 0x00002C9F
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00004AA7 File Offset: 0x00002CA7
		public int IntFallback { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00004AB0 File Offset: 0x00002CB0
		// (set) Token: 0x060000AE RID: 174 RVA: 0x00004AB8 File Offset: 0x00002CB8
		public bool UseLongFallback { get; set; } = true;

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00004AC1 File Offset: 0x00002CC1
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00004AC9 File Offset: 0x00002CC9
		public long LongFallback { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00004AD2 File Offset: 0x00002CD2
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00004ADA File Offset: 0x00002CDA
		public bool UseFloatFallback { get; set; } = true;

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00004AE3 File Offset: 0x00002CE3
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00004AEB File Offset: 0x00002CEB
		public float FloatFallback { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00004AF4 File Offset: 0x00002CF4
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00004AFC File Offset: 0x00002CFC
		public bool UseDoubleFallback { get; set; } = true;

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00004B05 File Offset: 0x00002D05
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00004B0D File Offset: 0x00002D0D
		public double DoubleFallback { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00004B16 File Offset: 0x00002D16
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00004B1E File Offset: 0x00002D1E
		public bool UseBoolFallback { get; set; } = true;

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00004B27 File Offset: 0x00002D27
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00004B2F File Offset: 0x00002D2F
		public bool BoolFallback { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00004B38 File Offset: 0x00002D38
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00004B40 File Offset: 0x00002D40
		public bool UseStringFallback { get; set; } = true;

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00004B49 File Offset: 0x00002D49
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00004B51 File Offset: 0x00002D51
		public string StringFallback { get; set; } = "";

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00004B5A File Offset: 0x00002D5A
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00004B62 File Offset: 0x00002D62
		public bool UseEmptyArrayFallback { get; set; } = true;

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00004B6B File Offset: 0x00002D6B
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00004B73 File Offset: 0x00002D73
		public bool UseEmptyEnumerableFallback { get; set; } = true;

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004B7C File Offset: 0x00002D7C
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00004B84 File Offset: 0x00002D84
		public Type OpenGenericListFallback { get; set; } = typeof(List<>);
	}
}
