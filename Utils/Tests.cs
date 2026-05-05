using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TaleWorlds.Library;

namespace Retinues.Utils
{
	// Token: 0x02000032 RID: 50
	[SafeClass]
	public static class Tests
	{
		// Token: 0x060000EE RID: 238 RVA: 0x00005CC4 File Offset: 0x00003EC4
		private static void EnsureDiscovered()
		{
			if (Tests._discovered)
			{
				return;
			}
			Tests._discovered = true;
			try
			{
				Assembly assembly = typeof(Tests).Assembly;
				BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				Type[] types = assembly.GetTypes();
				for (int i = 0; i < types.Length; i++)
				{
					MethodInfo[] methods = types[i].GetMethods(bindingAttr);
					for (int j = 0; j < methods.Length; j++)
					{
						MethodInfo method = methods[j];
						GameTestAttribute customAttribute = method.GetCustomAttribute<GameTestAttribute>();
						if (customAttribute != null)
						{
							ParameterInfo[] parameters = method.GetParameters();
							if (!(method.ReturnType != typeof(void)) && parameters.Length <= 1)
							{
								bool acceptsContext = parameters.Length == 1 && parameters[0].ParameterType == typeof(GameTestContext);
								Action<GameTestContext> action = delegate(GameTestContext ctx)
								{
									object obj;
									if (!acceptsContext)
									{
										obj = null;
									}
									else
									{
										(obj = new object[1])[0] = ctx;
									}
									object[] parameters2 = obj;
									method.Invoke(null, parameters2);
								};
								Tests.RegisterInternal(customAttribute.Name ?? method.Name, customAttribute.Group ?? "default", customAttribute.Description, action);
							}
						}
					}
				}
				Log.Info(string.Format("[Tests] Discovered {0} in-game tests in assembly {1}.", Tests._tests.Count, typeof(Tests).Assembly.GetName().Name));
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("[Tests] Failed to discover in-game tests: {0}", arg));
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00005E60 File Offset: 0x00004060
		private static void RegisterInternal(string name, string group, string description, Action<GameTestContext> action)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = "UnnamedTest";
			}
			if (Tests._tests.FirstOrDefault((Tests.RegisteredTest t) => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase) && string.Equals(t.Group, group, StringComparison.OrdinalIgnoreCase)) != null)
			{
				Log.Warn(string.Concat(new string[]
				{
					"[Tests] Duplicate test registration ignored for ",
					group,
					".",
					name,
					"."
				}));
				return;
			}
			Tests._tests.Add(new Tests.RegisteredTest(name, group, description, action));
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00005F0C File Offset: 0x0000410C
		public static string RunAllTests(string groupFilter = null, string nameFilter = null, bool stopOnFirstFailure = false)
		{
			Tests.EnsureDiscovered();
			GameTestContext gameTestContext = new GameTestContext();
			gameTestContext.EnsureCampaign();
			List<Tests.RegisteredTest> list = (from t in Tests._tests
			where (string.IsNullOrEmpty(groupFilter) || t.Group.Equals(groupFilter, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrEmpty(nameFilter) || t.Name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0)
			select t).ToList<Tests.RegisteredTest>();
			if (list.Count == 0)
			{
				return "[Tests] No tests matched the specified filters.";
			}
			List<GameTestResult> list2 = new List<GameTestResult>(list.Count);
			Stopwatch stopwatch = Stopwatch.StartNew();
			Log.Info(string.Format("[Tests] Starting in-game test run. Count={0}, GroupFilter={1}, NameFilter={2}.", list.Count, groupFilter ?? "*", nameFilter ?? "*"));
			foreach (Tests.RegisteredTest registeredTest in list)
			{
				Stopwatch stopwatch2 = Stopwatch.StartNew();
				bool flag = false;
				Exception ex = null;
				string text;
				try
				{
					registeredTest.Action(gameTestContext);
					flag = true;
					text = "OK";
				}
				catch (GameTestAssertionException ex2)
				{
					text = ex2.Message;
					ex = ex2;
				}
				catch (Exception ex3)
				{
					text = "Unexpected exception: " + ex3.Message;
					ex = ex3;
				}
				stopwatch2.Stop();
				GameTestResult item = new GameTestResult(registeredTest.Name, registeredTest.Group, registeredTest.Description, flag, text, ex, stopwatch2.Elapsed);
				list2.Add(item);
				if (flag)
				{
					Log.Debug(string.Format("[Tests] PASS: {0}.{1} in {2} ms. {3}", new object[]
					{
						registeredTest.Group,
						registeredTest.Name,
						stopwatch2.ElapsedMilliseconds,
						registeredTest.Description
					}));
				}
				else
				{
					Log.Error(string.Format("[Tests] FAIL: {0}.{1} in {2} ms. {3}\n{4}", new object[]
					{
						registeredTest.Group,
						registeredTest.Name,
						stopwatch2.ElapsedMilliseconds,
						text,
						ex
					}));
					if (stopOnFirstFailure)
					{
						break;
					}
				}
			}
			stopwatch.Stop();
			return Tests.FormatSummary(list2, stopwatch.Elapsed);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000615C File Offset: 0x0000435C
		public static void AssertTrue(bool condition, string message = null, [CallerMemberName] string member = null)
		{
			if (condition)
			{
				return;
			}
			string str = message ?? "Expected condition to be true.";
			throw new GameTestAssertionException("[" + member + "] " + str);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006190 File Offset: 0x00004390
		public static void AssertFalse(bool condition, string message = null, [CallerMemberName] string member = null)
		{
			if (!condition)
			{
				return;
			}
			string str = message ?? "Expected condition to be false.";
			throw new GameTestAssertionException("[" + member + "] " + str);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000061C4 File Offset: 0x000043C4
		public static void AssertEqual<T>(T expected, T actual, string message = null, [CallerMemberName] string member = null)
		{
			if (EqualityComparer<T>.Default.Equals(expected, actual))
			{
				return;
			}
			string text = message ?? "Values are not equal.";
			throw new GameTestAssertionException(string.Format("[{0}] {1} Expected={2}, Actual={3}", new object[]
			{
				member,
				text,
				expected,
				actual
			}));
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000621C File Offset: 0x0000441C
		public static void AssertNotNull(object value, string message = null, [CallerMemberName] string member = null)
		{
			if (value != null)
			{
				return;
			}
			string str = message ?? "Value was null.";
			throw new GameTestAssertionException("[" + member + "] " + str);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00006250 File Offset: 0x00004450
		private static string FormatSummary(IReadOnlyCollection<GameTestResult> results, TimeSpan totalDuration)
		{
			int count = results.Count;
			int num = results.Count((GameTestResult r) => r.Passed);
			int num2 = count - num;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("[Tests] Run complete. Total={0}, Passed={1}, Failed={2}, Duration={3:F0} ms.", new object[]
			{
				count,
				num,
				num2,
				totalDuration.TotalMilliseconds
			}));
			foreach (GameTestResult gameTestResult in from r in results
			orderby r.Group, r.Name
			select r)
			{
				string text = gameTestResult.Passed ? "PASS" : "FAIL";
				stringBuilder.AppendLine(string.Format(" - [{0}] {1}.{2} ({3:F0} ms) : {4}", new object[]
				{
					text,
					gameTestResult.Group,
					gameTestResult.Name,
					gameTestResult.Duration.TotalMilliseconds,
					gameTestResult.Message
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000063C8 File Offset: 0x000045C8
		[CommandLineFunctionality.CommandLineArgumentFunction("run_tests", "retinues")]
		public static string RunTests(List<string> args)
		{
			string groupFilter = null;
			string nameFilter = null;
			bool stopOnFirstFailure = false;
			if (args.Count > 0)
			{
				groupFilter = ((args[0] == "-") ? null : args[0]);
			}
			if (args.Count > 1)
			{
				nameFilter = ((args[1] == "-") ? null : args[1]);
			}
			if (args.Count > 2 && args[2] == "--stop")
			{
				stopOnFirstFailure = true;
			}
			return Tests.RunAllTests(groupFilter, nameFilter, stopOnFirstFailure);
		}

		// Token: 0x04000049 RID: 73
		private static readonly List<Tests.RegisteredTest> _tests = new List<Tests.RegisteredTest>();

		// Token: 0x0400004A RID: 74
		private static bool _discovered;

		// Token: 0x02000108 RID: 264
		private sealed class RegisteredTest
		{
			// Token: 0x17000452 RID: 1106
			// (get) Token: 0x06000A49 RID: 2633 RVA: 0x000308DC File Offset: 0x0002EADC
			public string Name { get; }

			// Token: 0x17000453 RID: 1107
			// (get) Token: 0x06000A4A RID: 2634 RVA: 0x000308E4 File Offset: 0x0002EAE4
			public string Group { get; }

			// Token: 0x17000454 RID: 1108
			// (get) Token: 0x06000A4B RID: 2635 RVA: 0x000308EC File Offset: 0x0002EAEC
			public string Description { get; }

			// Token: 0x17000455 RID: 1109
			// (get) Token: 0x06000A4C RID: 2636 RVA: 0x000308F4 File Offset: 0x0002EAF4
			public Action<GameTestContext> Action { get; }

			// Token: 0x06000A4D RID: 2637 RVA: 0x000308FC File Offset: 0x0002EAFC
			public RegisteredTest(string name, string group, string description, Action<GameTestContext> action)
			{
				this.Name = name;
				this.Group = group;
				this.Description = description;
				this.Action = action;
			}
		}
	}
}
