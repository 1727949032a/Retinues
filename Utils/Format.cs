using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Retinues.Utils
{
	// Token: 0x0200001F RID: 31
	[SafeClass]
	public static class Format
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00003337 File Offset: 0x00001537
		public static string CamelCaseToTitle(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			text = text.Replace('_', ' ');
			text = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
			text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
			return text;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003374 File Offset: 0x00001574
		public static string Crop(string text, int maxLength)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (text.Length > maxLength)
			{
				return text.Substring(0, maxLength) + "(...)";
			}
			return text;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000033A0 File Offset: 0x000015A0
		public static string Number(int value)
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			numberFormatInfo.NumberGroupSeparator = " ";
			return value.ToString("N0", numberFormatInfo);
		}
	}
}
