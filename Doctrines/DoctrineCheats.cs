using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TaleWorlds.Library;

namespace Retinues.Doctrines
{
	// Token: 0x020000D3 RID: 211
	public static class DoctrineCheats
	{
		// Token: 0x06000889 RID: 2185 RVA: 0x0002AC5C File Offset: 0x00028E5C
		[CommandLineFunctionality.CommandLineArgumentFunction("feat_list", "retinues")]
		public static string FeatList(List<string> args)
		{
			IReadOnlyList<DoctrineDefinition> readOnlyList = DoctrineAPI.AllDoctrines();
			if (readOnlyList == null || readOnlyList.Count == 0)
			{
				return "No doctrines discovered.";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (DoctrineDefinition doctrineDefinition in from x in readOnlyList
			orderby x.Column, x.Row
			select x)
			{
				DoctrineStatus doctrineStatus = DoctrineAPI.GetDoctrineStatus(doctrineDefinition.Key);
				stringBuilder.AppendLine(string.Format("[{0}] {1}  -  {2}", DoctrineCheats.TrimType(doctrineDefinition.Key), doctrineDefinition.Name, doctrineStatus));
				if (doctrineDefinition.Feats == null || doctrineDefinition.Feats.Count == 0)
				{
					stringBuilder.AppendLine("  (no feats)");
				}
				else
				{
					foreach (FeatDefinition featDefinition in doctrineDefinition.Feats)
					{
						int featProgress = DoctrineAPI.GetFeatProgress(featDefinition.Key);
						int featTarget = DoctrineAPI.GetFeatTarget(featDefinition.Key);
						bool flag = DoctrineAPI.IsFeatComplete(featDefinition.Key);
						stringBuilder.AppendLine(string.Format("  - {0} : {1}/{2} {3} - {4}", new object[]
						{
							DoctrineCheats.TrimType(featDefinition.Key),
							featProgress,
							featTarget,
							flag ? "[DONE]" : "",
							featDefinition.Description
						}));
					}
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x0002AE50 File Offset: 0x00029050
		[CommandLineFunctionality.CommandLineArgumentFunction("feat_add", "retinues")]
		public static string FeatAdd(List<string> args)
		{
			if (args.Count < 1)
			{
				return "Usage: retinues.feat_add <FeatNameOrType> [amount]";
			}
			string text;
			string result;
			if (DoctrineCheats.ResolveFeatType(args[0], out text, out result) == null)
			{
				return result;
			}
			int num = 1;
			if (args.Count >= 2 && !int.TryParse(args[1], out num))
			{
				return "amount must be an integer.";
			}
			int num2 = DoctrineAPI.AdvanceFeat(text, num);
			int featTarget = DoctrineAPI.GetFeatTarget(text);
			return string.Format("{0} advanced by {1}. Now {2}/{3}.", new object[]
			{
				DoctrineCheats.TrimType(text),
				num,
				num2,
				featTarget
			});
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0002AEEC File Offset: 0x000290EC
		[CommandLineFunctionality.CommandLineArgumentFunction("feat_set", "retinues")]
		public static string FeatSet(List<string> args)
		{
			if (args.Count < 2)
			{
				return "Usage: retinues.feat_set <FeatNameOrType> <amount>";
			}
			string text;
			string result;
			if (DoctrineCheats.ResolveFeatType(args[0], out text, out result) == null)
			{
				return result;
			}
			int num;
			if (!int.TryParse(args[1], out num))
			{
				return "amount must be an integer.";
			}
			DoctrineAPI.SetFeatProgress(text, num);
			int featTarget = DoctrineAPI.GetFeatTarget(text);
			bool flag = DoctrineAPI.IsFeatComplete(text);
			return string.Format("{0} set to {1}/{2} {3}.", new object[]
			{
				DoctrineCheats.TrimType(text),
				num,
				featTarget,
				flag ? "[DONE]" : ""
			});
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0002AF8C File Offset: 0x0002918C
		[CommandLineFunctionality.CommandLineArgumentFunction("feat_unlock", "retinues")]
		public static string FeatUnlock(List<string> args)
		{
			if (args.Count < 1)
			{
				return "Usage: retinues.feat_unlock <FeatNameOrType>";
			}
			string text;
			string result;
			if (DoctrineCheats.ResolveFeatType(args[0], out text, out result) == null)
			{
				return result;
			}
			int featTarget = DoctrineAPI.GetFeatTarget(text);
			DoctrineAPI.SetFeatProgress(text, featTarget);
			return DoctrineCheats.TrimType(text) + " marked complete.";
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x0002AFE0 File Offset: 0x000291E0
		[CommandLineFunctionality.CommandLineArgumentFunction("feat_unlock_all", "retinues")]
		public static string FeatUnlockAll(List<string> args)
		{
			IEnumerable<string> enumerable = DoctrineCheats.AllFeatKeys();
			int num = 0;
			foreach (string featKey in enumerable)
			{
				int featTarget = DoctrineAPI.GetFeatTarget(featKey);
				DoctrineAPI.SetFeatProgress(featKey, featTarget);
				num++;
			}
			return string.Format("Completed {0} feats.", num);
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x0002B048 File Offset: 0x00029248
		private static IEnumerable<string> AllFeatKeys()
		{
			return new DoctrineCheats.<AllFeatKeys>d__5(-2);
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x0002B054 File Offset: 0x00029254
		private static Type ResolveFeatType(string token, out string featKey, out string error)
		{
			featKey = null;
			error = null;
			if (string.IsNullOrWhiteSpace(token))
			{
				error = "Empty feat token.";
				return null;
			}
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>(StringComparer.Ordinal);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (DoctrineDefinition doctrineDefinition in (DoctrineAPI.AllDoctrines() ?? Array.Empty<DoctrineDefinition>()))
			{
				if (doctrineDefinition.Feats != null)
				{
					foreach (FeatDefinition featDefinition in doctrineDefinition.Feats)
					{
						string key = featDefinition.Key;
						Type typeByFullName = DoctrineCheats.GetTypeByFullName(key);
						if (!(typeByFullName == null))
						{
							dictionary[key] = typeByFullName;
							string key2 = DoctrineCheats.TrimType(key);
							if (!dictionary2.ContainsKey(key2))
							{
								dictionary2[key2] = key;
							}
						}
					}
				}
			}
			Type result;
			if (dictionary.TryGetValue(token, out result))
			{
				featKey = token;
				return result;
			}
			string text;
			if (dictionary2.TryGetValue(token, out text))
			{
				featKey = text;
				return dictionary[text];
			}
			string text2 = dictionary.Keys.FirstOrDefault((string k) => k.EndsWith(token, StringComparison.OrdinalIgnoreCase) || DoctrineCheats.TrimType(k).Equals(token, StringComparison.OrdinalIgnoreCase));
			if (text2 != null)
			{
				featKey = text2;
				return dictionary[text2];
			}
			error = "Feat not found: '" + token + "'. Try 'retinues.feat_list' to see available feats.";
			return null;
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x0002B1EC File Offset: 0x000293EC
		private static string TrimType(string full)
		{
			if (string.IsNullOrEmpty(full))
			{
				return full;
			}
			int num = full.LastIndexOf('.');
			string text = (num >= 0) ? full.Substring(num + 1) : full;
			int num2 = text.LastIndexOf('+');
			if (num2 < 0)
			{
				return text;
			}
			return text.Substring(num2 + 1);
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x0002B238 File Offset: 0x00029438
		private static Type GetTypeByFullName(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				return null;
			}
			Type type = Type.GetType(fullName, false);
			if (type != null)
			{
				return type;
			}
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					type = assembly.GetType(fullName, false);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
			return null;
		}
	}
}
