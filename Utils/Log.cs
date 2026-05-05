using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Retinues.Configuration;
using TaleWorlds.Library;

namespace Retinues.Utils
{
	// Token: 0x02000022 RID: 34
	public static class Log
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003469 File Offset: 0x00001669
		public static LogLevel MinFileLevel
		{
			get
			{
				if (!Config.DebugMode)
				{
					return LogLevel.Debug;
				}
				return LogLevel.Debug;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000347A File Offset: 0x0000167A
		public static LogLevel MinInGameLevel
		{
			get
			{
				if (!Config.DebugMode)
				{
					return LogLevel.Critical;
				}
				return LogLevel.Info;
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000348C File Offset: 0x0000168C
		static Log()
		{
			try
			{
				Log.LogFile = Path.Combine(Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.FullName, "debug.log");
			}
			catch
			{
				Log.LogFile = "debug.log";
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000034F0 File Offset: 0x000016F0
		public static void Trace(string message)
		{
			Log.Write(LogLevel.Trace, message, null);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000034FA File Offset: 0x000016FA
		public static void Debug(string message)
		{
			Log.Write(LogLevel.Debug, message, null);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00003504 File Offset: 0x00001704
		public static void Info(string message)
		{
			Log.Write(LogLevel.Info, message, null);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000350E File Offset: 0x0000170E
		public static void Success(string message)
		{
			Log.Write(LogLevel.Success, message, null);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00003518 File Offset: 0x00001718
		public static void Warn(string message)
		{
			Log.Write(LogLevel.Warn, message, null);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003522 File Offset: 0x00001722
		public static void Error(string message)
		{
			Log.Write(LogLevel.Error, message, null);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000352C File Offset: 0x0000172C
		public static void Critical(string message)
		{
			Log.Write(LogLevel.Critical, message, null);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003536 File Offset: 0x00001736
		public static void Message(string message)
		{
			Log.Write(LogLevel.Message, message, null);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003540 File Offset: 0x00001740
		public static void Dump(object obj, LogLevel level = LogLevel.Debug)
		{
			Log.<>c__DisplayClass16_0 CS$<>8__locals1;
			CS$<>8__locals1.sb = new StringBuilder();
			Log.<Dump>g__DumpRecursive|16_0(obj, 0, ref CS$<>8__locals1);
			Log.Write(level, CS$<>8__locals1.sb.ToString(), null);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003574 File Offset: 0x00001774
		public static void Exception(Exception ex, string context = "", string caller = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ex.GetType().Name).Append(": ").Append(ex.Message);
			if (!string.IsNullOrWhiteSpace(context))
			{
				stringBuilder.Append(" | ").Append(context);
			}
			stringBuilder.AppendLine();
			Log.AppendStackTrace(stringBuilder, ex);
			IDictionary data = ex.Data;
			if (data != null && data.Count > 0)
			{
				stringBuilder.AppendLine("Data:");
				foreach (object obj in ex.Data)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					StringBuilder stringBuilder2 = stringBuilder.Append("  ").Append(dictionaryEntry.Key).Append(": ");
					object value = dictionaryEntry.Value;
					stringBuilder2.AppendLine((value != null) ? value.ToString() : null);
				}
			}
			Log.Write(LogLevel.Error, stringBuilder.ToString(), caller);
			Exception innerException = ex.InnerException;
			int num = 1;
			while (innerException != null)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("[INNER ").Append(num).Append("] ").Append(innerException.GetType().Name).Append(": ").AppendLine(innerException.Message);
				Log.AppendStackTrace(stringBuilder3, innerException);
				Log.Write(LogLevel.Error, stringBuilder3.ToString(), caller);
				innerException = innerException.InnerException;
				num++;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003704 File Offset: 0x00001904
		public static void Trace(int up = 1, LogLevel level = LogLevel.Debug)
		{
			string callerAboveLabel = Caller.GetCallerAboveLabel(up, 0, false);
			Log.Write(level, "Called by: " + callerAboveLabel, null);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000372C File Offset: 0x0000192C
		private static void AppendStackTrace(StringBuilder sb, Exception ex)
		{
			sb.AppendLine("Exception stack:");
			StackFrame[] array = new StackTrace(ex, true).GetFrames() ?? Array.Empty<StackFrame>();
			foreach (StackFrame stackFrame in array)
			{
				MethodBase method = stackFrame.GetMethod();
				if (!(method == null))
				{
					Type declaringType = method.DeclaringType;
					string value = ((declaringType != null) ? declaringType.FullName : null) ?? "<unknown>";
					string name = method.Name;
					ParameterInfo[] parameters = method.GetParameters();
					string value2 = string.Join(", ", from p in parameters
					select p.ParameterType.Name + " " + p.Name);
					string fileName = stackFrame.GetFileName();
					int fileLineNumber = stackFrame.GetFileLineNumber();
					int fileColumnNumber = stackFrame.GetFileColumnNumber();
					sb.Append("  at ").Append(value).Append(".").Append(name).Append("(").Append(value2).Append(")");
					if (!string.IsNullOrEmpty(fileName) && fileLineNumber > 0)
					{
						sb.Append(" in ").Append(fileName).Append(":line ").Append(fileLineNumber).Append(":col ").Append(fileColumnNumber);
					}
					sb.AppendLine();
				}
			}
			if (array.Length == 0 && !string.IsNullOrEmpty(ex.StackTrace))
			{
				sb.AppendLine("(raw ex.StackTrace follows)");
				sb.AppendLine(ex.StackTrace);
			}
			string value3 = Log.PruneManagedStack(Environment.StackTrace, 100);
			if (!string.IsNullOrEmpty(value3))
			{
				sb.AppendLine("Managed call stack:");
				sb.AppendLine(value3);
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000038E8 File Offset: 0x00001AE8
		private static string PruneManagedStack(string envStack, int maxLines = 100)
		{
			if (string.IsNullOrEmpty(envStack))
			{
				return null;
			}
			string[] hints = new string[]
			{
				"System.Environment.GetStackTrace",
				"System.Environment.get_StackTrace",
				"Retinues.Utils.Log.AppendStackTrace",
				"Retinues.Utils.Log.Exception",
				"Retinues.Utils.Log.Error",
				"Retinues.Utils.Caller",
				"Retinues.Utils.SafeMethodPatcher",
				"HarmonyLib.",
				"System.RuntimeMethodHandle",
				"System.Reflection.RuntimeMethodInfo"
			};
			List<string> list = (from l in envStack.Split(new char[]
			{
				'\r',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries)
			select l.TrimEnd(Array.Empty<char>())).ToList<string>();
			int num = 0;
			while (num < list.Count && Log.ContainsAny(list[num], hints))
			{
				num++;
			}
			if (num >= list.Count)
			{
				return null;
			}
			while (num < list.Count && list[num].IndexOf("HarmonyLib.", StringComparison.Ordinal) >= 0)
			{
				num++;
			}
			IEnumerable<string> values = list.Skip(num).Take(maxLines);
			return string.Join(Environment.NewLine, values);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003A00 File Offset: 0x00001C00
		private static bool ContainsAny(string line, string[] hints)
		{
			foreach (string value in hints)
			{
				if (line.IndexOf(value, StringComparison.Ordinal) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003A30 File Offset: 0x00001C30
		private static void Write(LogLevel level, string message, string caller = null)
		{
			if (caller == null)
			{
				caller = Caller.GetLabel(0, false, 24);
			}
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string line = string.Format("[{0}] [{1}] {2}: {3}", new object[]
			{
				text,
				level,
				caller,
				message
			});
			if (level >= Log.MinFileLevel)
			{
				Log.WriteToFile(line);
			}
			if (level >= Log.MinInGameLevel)
			{
				Log.WriteInGame(message, level);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003AA4 File Offset: 0x00001CA4
		private static void WriteInGame(string message, LogLevel level)
		{
			Color color = Log.LevelColor(level);
			InformationManager.DisplayMessage(new InformationMessage(message, color));
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003AC4 File Offset: 0x00001CC4
		private static void WriteToFile(string line)
		{
			try
			{
				object fileLock = Log._fileLock;
				lock (fileLock)
				{
					File.AppendAllText(Log.LogFile, line + Environment.NewLine);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003B24 File Offset: 0x00001D24
		private static Color LevelColor(LogLevel level)
		{
			string color;
			switch (level)
			{
			case LogLevel.Trace:
				color = "#9E9E9EFF";
				break;
			case LogLevel.Debug:
				color = "#64B5F6FF";
				break;
			case LogLevel.Info:
				color = "#2196F3FF";
				break;
			case LogLevel.Success:
				color = "#43A047FF";
				break;
			case LogLevel.Warn:
				color = "#FFA000FF";
				break;
			case LogLevel.Error:
				color = "#E53935FF";
				break;
			case LogLevel.Critical:
				color = "#B71C1CFF";
				break;
			case LogLevel.Message:
				color = "#ffffffe0";
				break;
			default:
				color = "#ffffffff";
				break;
			}
			return Color.ConvertStringToColor(color);
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00003BA8 File Offset: 0x00001DA8
		public static int LogFileLength
		{
			get
			{
				int result;
				try
				{
					object fileLock = Log._fileLock;
					lock (fileLock)
					{
						if (!File.Exists(Log.LogFile))
						{
							result = 0;
						}
						else
						{
							result = File.ReadLines(Log.LogFile).Count<string>();
						}
					}
				}
				catch
				{
					result = 0;
				}
				return result;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003C14 File Offset: 0x00001E14
		public static void Truncate(int keepLastNLines)
		{
			if (keepLastNLines < 0)
			{
				keepLastNLines = 0;
			}
			try
			{
				object fileLock = Log._fileLock;
				lock (fileLock)
				{
					if (File.Exists(Log.LogFile))
					{
						if (keepLastNLines == 0)
						{
							File.WriteAllText(Log.LogFile, string.Empty);
						}
						else
						{
							Queue<string> queue = new Queue<string>(keepLastNLines);
							using (FileStream fileStream = new FileStream(Log.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
							{
								using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true))
								{
									string item;
									while ((item = streamReader.ReadLine()) != null)
									{
										if (queue.Count == keepLastNLines)
										{
											queue.Dequeue();
										}
										queue.Enqueue(item);
									}
								}
							}
							string text = Log.LogFile + ".tmp";
							using (StreamWriter streamWriter = new StreamWriter(text, false, Encoding.UTF8))
							{
								bool flag2 = true;
								foreach (string value in queue)
								{
									if (!flag2)
									{
										streamWriter.Write(Environment.NewLine);
									}
									streamWriter.Write(value);
									flag2 = false;
								}
							}
							File.Copy(text, Log.LogFile, true);
							File.Delete(text);
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003DF8 File Offset: 0x00001FF8
		[CompilerGenerated]
		internal static void <Dump>g__DumpRecursive|16_0(object o, int depth, ref Log.<>c__DisplayClass16_0 A_2)
		{
			if (o == null)
			{
				A_2.sb.Append("<null> ");
				return;
			}
			Type type = o.GetType();
			IDictionary dictionary = o as IDictionary;
			if (dictionary != null)
			{
				A_2.sb.Append(string.Format("Dictionary<{0},{1}>[{2}] ", type.GenericTypeArguments[0].Name, type.GenericTypeArguments[1].Name, dictionary.Count));
				foreach (object obj in dictionary)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					A_2.sb.Append("Key: ");
					Log.<Dump>g__DumpRecursive|16_0(dictionaryEntry.Key, depth + 1, ref A_2);
					A_2.sb.Append("Value: ");
					Log.<Dump>g__DumpRecursive|16_0(dictionaryEntry.Value, depth + 1, ref A_2);
				}
				return;
			}
			IEnumerable enumerable = o as IEnumerable;
			if (enumerable != null && !(o is string))
			{
				StringBuilder sb = A_2.sb;
				string str = "List<";
				Type elementType = type.GetElementType();
				string str2;
				if ((str2 = ((elementType != null) ? elementType.Name : null)) == null)
				{
					Type type2 = type.GenericTypeArguments.FirstOrDefault<Type>();
					str2 = (((type2 != null) ? type2.Name : null) ?? type.Name);
				}
				sb.Append(str + str2 + ">: ");
				int num = 0;
				foreach (object o2 in enumerable)
				{
					A_2.sb.Append(string.Format("[{0}]: ", num));
					Log.<Dump>g__DumpRecursive|16_0(o2, depth + 1, ref A_2);
					num++;
				}
				if (num == 0)
				{
					A_2.sb.Append("<empty> ");
				}
				return;
			}
			try
			{
				string text = o.ToString();
				if (string.IsNullOrEmpty(text))
				{
					text = "<" + type.FullName + ">";
				}
				A_2.sb.Append(text + " ");
			}
			catch (Exception ex)
			{
				A_2.sb.Append(string.Concat(new string[]
				{
					"Log.Dump failed for object of type ",
					type.FullName,
					": ",
					ex.Message,
					" "
				}));
			}
		}

		// Token: 0x0400001C RID: 28
		private const string LogFileName = "debug.log";

		// Token: 0x0400001D RID: 29
		private static readonly object _fileLock = new object();

		// Token: 0x0400001E RID: 30
		private static readonly string LogFile;
	}
}
