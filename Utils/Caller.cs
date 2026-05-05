using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Retinues.Utils
{
	// Token: 0x0200001E RID: 30
	public static class Caller
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00002D44 File Offset: 0x00000F44
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Caller.Info Get(int skip = 0, int maxFrames = 24, bool includeFileInfo = false, bool includeNamespaceInLabel = false, Func<MethodBase, bool> extraSkipPredicate = null)
		{
			try
			{
				StackTrace stackTrace = new StackTrace(1 + skip, includeFileInfo);
				int num = Math.Min(stackTrace.FrameCount, Math.Max(4, maxFrames));
				for (int i = 0; i < num; i++)
				{
					StackFrame frame = stackTrace.GetFrame(i);
					MethodBase methodBase = (frame != null) ? frame.GetMethod() : null;
					if (!(methodBase == null))
					{
						Type declaringType = methodBase.DeclaringType;
						if (!(declaringType == null))
						{
							string text;
							if ((text = declaringType.FullName) == null)
							{
								text = (declaringType.Name ?? "");
							}
							string text2 = text;
							if (!Caller.StartsWithAny(text2, Caller._skipNamespaces) && !Caller._skipTypes.Contains(text2) && !methodBase.Name.Contains("Invoke") && (extraSkipPredicate == null || !extraSkipPredicate(methodBase)))
							{
								string name = declaringType.Name;
								string text3 = Caller.NormalizeMethodName(methodBase);
								string text4 = declaringType.Namespace ?? string.Empty;
								string label = (includeNamespaceInLabel && !string.IsNullOrEmpty(text4)) ? string.Concat(new string[]
								{
									text4,
									".",
									name,
									".",
									text3
								}) : (name + "." + text3);
								string fileName = null;
								int line = 0;
								if (includeFileInfo)
								{
									fileName = frame.GetFileName();
									line = frame.GetFileLineNumber();
								}
								Caller.Info info = new Caller.Info();
								info.Method = methodBase;
								info.DeclaringType = declaringType;
								Assembly assembly = declaringType.Assembly;
								string assemblyName;
								if (assembly == null)
								{
									assemblyName = null;
								}
								else
								{
									AssemblyName name2 = assembly.GetName();
									assemblyName = ((name2 != null) ? name2.Name : null);
								}
								info.AssemblyName = assemblyName;
								info.Namespace = text4;
								info.TypeName = name;
								info.MethodName = text3;
								info.Label = label;
								info.FileName = fileName;
								info.Line = line;
								return info;
							}
						}
					}
				}
			}
			catch
			{
			}
			return new Caller.Info
			{
				Label = "<unknown>"
			};
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002F48 File Offset: 0x00001148
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetLabel(int skip = 0, bool includeNamespace = false, int maxFrames = 24)
		{
			Caller.Info info = Caller.Get(skip, maxFrames, false, includeNamespace, null);
			return ((info != null) ? info.Label : null) ?? "<unknown>";
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00002F6C File Offset: 0x0000116C
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsBlocked(IReadOnlyCollection<string> blacklist, StringComparison cmp = StringComparison.Ordinal, bool allowSubstring = true, int skip = 0)
		{
			if (blacklist == null || blacklist.Count == 0)
			{
				return false;
			}
			string label = Caller.GetLabel(skip + 1, false, 24);
			foreach (string text in blacklist)
			{
				if (string.Equals(label, text, cmp))
				{
					return true;
				}
				if (allowSubstring && label.IndexOf(text, cmp) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002FEC File Offset: 0x000011EC
		private static bool StartsWithAny(string s, string[] prefixes)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			for (int i = 0; i < prefixes.Length; i++)
			{
				if (s.StartsWith(prefixes[i], StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003020 File Offset: 0x00001220
		private static string NormalizeMethodName(MethodBase m)
		{
			string name = m.Name;
			bool flag = name == ".ctor" || name == ".cctor";
			if (flag)
			{
				Type declaringType = m.DeclaringType;
				return ((declaringType != null) ? declaringType.Name : null) ?? name;
			}
			return name;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003070 File Offset: 0x00001270
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Caller.Info GetNth(int n, int skip = 0, int maxFrames = 24, bool includeFileInfo = false, bool includeNamespaceInLabel = false, Func<MethodBase, bool> extraSkipPredicate = null)
		{
			if (n < 0)
			{
				n = 0;
			}
			try
			{
				StackTrace stackTrace = new StackTrace(1 + skip, includeFileInfo);
				int num = Math.Min(stackTrace.FrameCount, Math.Max(4, maxFrames));
				int num2 = -1;
				for (int i = 0; i < num; i++)
				{
					StackFrame frame = stackTrace.GetFrame(i);
					MethodBase methodBase = (frame != null) ? frame.GetMethod() : null;
					if (!(methodBase == null))
					{
						Type declaringType = methodBase.DeclaringType;
						if (!(declaringType == null))
						{
							string text;
							if ((text = declaringType.FullName) == null)
							{
								text = (declaringType.Name ?? "");
							}
							string text2 = text;
							if (!Caller.StartsWithAny(text2, Caller._skipNamespaces) && !Caller._skipTypes.Contains(text2) && !methodBase.Name.Contains("Invoke") && (extraSkipPredicate == null || !extraSkipPredicate(methodBase)))
							{
								num2++;
								if (num2 == n)
								{
									string name = declaringType.Name;
									string text3 = Caller.NormalizeMethodName(methodBase);
									string text4 = declaringType.Namespace ?? string.Empty;
									string label = (includeNamespaceInLabel && !string.IsNullOrEmpty(text4)) ? string.Concat(new string[]
									{
										text4,
										".",
										name,
										".",
										text3
									}) : (name + "." + text3);
									string fileName = null;
									int line = 0;
									if (includeFileInfo)
									{
										fileName = frame.GetFileName();
										line = frame.GetFileLineNumber();
									}
									Caller.Info info = new Caller.Info();
									info.Method = methodBase;
									info.DeclaringType = declaringType;
									Assembly assembly = declaringType.Assembly;
									string assemblyName;
									if (assembly == null)
									{
										assemblyName = null;
									}
									else
									{
										AssemblyName name2 = assembly.GetName();
										assemblyName = ((name2 != null) ? name2.Name : null);
									}
									info.AssemblyName = assemblyName;
									info.Namespace = text4;
									info.TypeName = name;
									info.MethodName = text3;
									info.Label = label;
									info.FileName = fileName;
									info.Line = line;
									return info;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			return new Caller.Info
			{
				Label = "<unknown>"
			};
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003290 File Offset: 0x00001490
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Caller.Info GetCallerAbove(int up = 1, int skip = 0, bool includeNamespaceInLabel = false)
		{
			return Caller.GetNth(up, skip, 24, false, includeNamespaceInLabel, null);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000329E File Offset: 0x0000149E
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetCallerAboveLabel(int up = 1, int skip = 0, bool includeNamespace = false)
		{
			Caller.Info callerAbove = Caller.GetCallerAbove(up, skip, includeNamespace);
			return ((callerAbove != null) ? callerAbove.Label : null) ?? "<unknown>";
		}

		// Token: 0x04000011 RID: 17
		private static readonly string[] _skipNamespaces = new string[]
		{
			"System.",
			"Microsoft.",
			"HarmonyLib.",
			"TaleWorlds.Engine",
			"TaleWorlds.Library",
			"TaleWorlds.DotNet",
			"Retinues.Utils"
		};

		// Token: 0x04000012 RID: 18
		private static readonly HashSet<string> _skipTypes = new HashSet<string>(StringComparer.Ordinal)
		{
			"Retinues.Utils.Log",
			"Retinues.Utils.Caller"
		};

		// Token: 0x020000FD RID: 253
		public sealed class Info
		{
			// Token: 0x17000443 RID: 1091
			// (get) Token: 0x06000A16 RID: 2582 RVA: 0x0003060F File Offset: 0x0002E80F
			// (set) Token: 0x06000A17 RID: 2583 RVA: 0x00030617 File Offset: 0x0002E817
			public MethodBase Method { get; set; }

			// Token: 0x17000444 RID: 1092
			// (get) Token: 0x06000A18 RID: 2584 RVA: 0x00030620 File Offset: 0x0002E820
			// (set) Token: 0x06000A19 RID: 2585 RVA: 0x00030628 File Offset: 0x0002E828
			public Type DeclaringType { get; set; }

			// Token: 0x17000445 RID: 1093
			// (get) Token: 0x06000A1A RID: 2586 RVA: 0x00030631 File Offset: 0x0002E831
			// (set) Token: 0x06000A1B RID: 2587 RVA: 0x00030639 File Offset: 0x0002E839
			public string AssemblyName { get; set; }

			// Token: 0x17000446 RID: 1094
			// (get) Token: 0x06000A1C RID: 2588 RVA: 0x00030642 File Offset: 0x0002E842
			// (set) Token: 0x06000A1D RID: 2589 RVA: 0x0003064A File Offset: 0x0002E84A
			public string Namespace { get; set; }

			// Token: 0x17000447 RID: 1095
			// (get) Token: 0x06000A1E RID: 2590 RVA: 0x00030653 File Offset: 0x0002E853
			// (set) Token: 0x06000A1F RID: 2591 RVA: 0x0003065B File Offset: 0x0002E85B
			public string TypeName { get; set; }

			// Token: 0x17000448 RID: 1096
			// (get) Token: 0x06000A20 RID: 2592 RVA: 0x00030664 File Offset: 0x0002E864
			// (set) Token: 0x06000A21 RID: 2593 RVA: 0x0003066C File Offset: 0x0002E86C
			public string MethodName { get; set; }

			// Token: 0x17000449 RID: 1097
			// (get) Token: 0x06000A22 RID: 2594 RVA: 0x00030675 File Offset: 0x0002E875
			// (set) Token: 0x06000A23 RID: 2595 RVA: 0x0003067D File Offset: 0x0002E87D
			public string Label { get; set; }

			// Token: 0x1700044A RID: 1098
			// (get) Token: 0x06000A24 RID: 2596 RVA: 0x00030686 File Offset: 0x0002E886
			// (set) Token: 0x06000A25 RID: 2597 RVA: 0x0003068E File Offset: 0x0002E88E
			public string FileName { get; set; }

			// Token: 0x1700044B RID: 1099
			// (get) Token: 0x06000A26 RID: 2598 RVA: 0x00030697 File Offset: 0x0002E897
			// (set) Token: 0x06000A27 RID: 2599 RVA: 0x0003069F File Offset: 0x0002E89F
			public int Line { get; set; }
		}
	}
}
