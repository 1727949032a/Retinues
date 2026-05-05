using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace Retinues.Utils
{
	// Token: 0x0200002A RID: 42
	public static class SafeMethodPatcher
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00004C1C File Offset: 0x00002E1C
		public static void ApplyAll(Harmony harmony, params Assembly[] assemblies)
		{
			if (harmony == null)
			{
				throw new ArgumentNullException("harmony");
			}
			if (SafeMethodPatcher._applied)
			{
				return;
			}
			SafeMethodPatcher._applied = true;
			if (assemblies == null || assemblies.Length == 0)
			{
				assemblies = new Assembly[]
				{
					Assembly.GetExecutingAssembly()
				};
			}
			IEnumerable<Assembly> source = assemblies;
			Func<Assembly, IEnumerable<Type>> selector;
			if ((selector = SafeMethodPatcher.<>O.<0>__SafeGetTypes) == null)
			{
				selector = (SafeMethodPatcher.<>O.<0>__SafeGetTypes = new Func<Assembly, IEnumerable<Type>>(SafeMethodPatcher.SafeGetTypes));
			}
			Type[] array = source.SelectMany(selector).ToArray<Type>();
			foreach (Type type in array)
			{
				SafeMethodPatcher.PatchExplicitSafeMethods(harmony, type);
				SafeMethodPatcher.PatchExplicitSafeProperties(harmony, type);
			}
			Type[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Type t = array2[i];
				SafeClassAttribute customAttribute = t.GetCustomAttribute<SafeClassAttribute>();
				if (customAttribute != null)
				{
					IEnumerable<Type> enumerable = new <>z__ReadOnlySingleElementList<Type>(t);
					if (customAttribute.IncludeDerived)
					{
						enumerable = enumerable.Concat(from x in array
						where x != t && x.IsSubclassOf(t)
						select x);
					}
					foreach (Type type2 in enumerable.Distinct<Type>())
					{
						SafeMethodPatcher.PatchTypeByClassRule(harmony, type2, customAttribute);
					}
				}
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004D5C File Offset: 0x00002F5C
		private static void PatchExplicitSafeMethods(Harmony harmony, Type type)
		{
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			foreach (MethodInfo methodInfo in type.GetMethods(bindingAttr))
			{
				SafeMethodAttribute customAttribute = methodInfo.GetCustomAttribute<SafeMethodAttribute>();
				if (customAttribute != null)
				{
					SafeMethodPatcher.PatchOne(harmony, methodInfo, new SafeMethodPatcher.Behavior
					{
						Fallback = customAttribute.Fallback,
						FallbackType = customAttribute.FallbackType,
						Swallow = customAttribute.Swallow
					});
				}
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004DD0 File Offset: 0x00002FD0
		private static void PatchExplicitSafeProperties(Harmony harmony, Type type)
		{
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingAttr))
			{
				SafeMethodAttribute customAttribute = propertyInfo.GetCustomAttribute<SafeMethodAttribute>();
				if (customAttribute != null)
				{
					if (propertyInfo.GetMethod != null)
					{
						SafeMethodPatcher.PatchOne(harmony, propertyInfo.GetMethod, new SafeMethodPatcher.Behavior
						{
							Fallback = customAttribute.Fallback,
							FallbackType = customAttribute.FallbackType,
							Swallow = customAttribute.Swallow
						});
					}
					if (propertyInfo.SetMethod != null)
					{
						SafeMethodPatcher.PatchOne(harmony, propertyInfo.SetMethod, new SafeMethodPatcher.Behavior
						{
							Fallback = customAttribute.Fallback,
							FallbackType = customAttribute.FallbackType,
							Swallow = customAttribute.Swallow
						});
					}
				}
				else
				{
					MethodInfo getMethod = propertyInfo.GetMethod;
					SafeMethodAttribute safeMethodAttribute = (getMethod != null) ? getMethod.GetCustomAttribute<SafeMethodAttribute>() : null;
					if (safeMethodAttribute != null && propertyInfo.GetMethod != null)
					{
						SafeMethodPatcher.PatchOne(harmony, propertyInfo.GetMethod, new SafeMethodPatcher.Behavior
						{
							Fallback = safeMethodAttribute.Fallback,
							FallbackType = safeMethodAttribute.FallbackType,
							Swallow = safeMethodAttribute.Swallow
						});
					}
					MethodInfo setMethod = propertyInfo.SetMethod;
					SafeMethodAttribute safeMethodAttribute2 = (setMethod != null) ? setMethod.GetCustomAttribute<SafeMethodAttribute>() : null;
					if (safeMethodAttribute2 != null && propertyInfo.SetMethod != null)
					{
						SafeMethodPatcher.PatchOne(harmony, propertyInfo.SetMethod, new SafeMethodPatcher.Behavior
						{
							Fallback = safeMethodAttribute2.Fallback,
							FallbackType = safeMethodAttribute2.FallbackType,
							Swallow = safeMethodAttribute2.Swallow
						});
					}
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004F80 File Offset: 0x00003180
		private static void PatchTypeByClassRule(Harmony harmony, Type type, SafeClassAttribute cfg)
		{
			BindingFlags bindingFlags = cfg.PublicOnly ? BindingFlags.Public : (BindingFlags.Public | BindingFlags.NonPublic);
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | bindingFlags;
			foreach (MethodInfo methodInfo in type.GetMethods(bindingAttr))
			{
				if (!methodInfo.IsSpecialName && methodInfo.GetCustomAttribute<UnsafeMethodAttribute>() == null && SafeMethodPatcher.IsHarmonyPatchable(methodInfo))
				{
					SafeMethodAttribute customAttribute = methodInfo.GetCustomAttribute<SafeMethodAttribute>();
					SafeMethodPatcher.Behavior behavior = (customAttribute != null) ? new SafeMethodPatcher.Behavior
					{
						Fallback = customAttribute.Fallback,
						FallbackType = customAttribute.FallbackType,
						Swallow = customAttribute.Swallow,
						ClassCfg = cfg
					} : new SafeMethodPatcher.Behavior
					{
						Swallow = cfg.SwallowByDefault,
						ClassCfg = cfg
					};
					SafeMethodPatcher.PatchOne(harmony, methodInfo, behavior);
				}
			}
			if (!cfg.IncludeAccessors)
			{
				return;
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingAttr))
			{
				if (propertyInfo.GetCustomAttribute<UnsafePropertyAttribute>() == null)
				{
					SafeMethodAttribute customAttribute2 = propertyInfo.GetCustomAttribute<SafeMethodAttribute>();
					if (propertyInfo.GetMethod != null)
					{
						SafeMethodAttribute safeMethodAttribute = propertyInfo.GetMethod.GetCustomAttribute<SafeMethodAttribute>() ?? customAttribute2;
						if (safeMethodAttribute != null)
						{
							SafeMethodPatcher.PatchOne(harmony, propertyInfo.GetMethod, new SafeMethodPatcher.Behavior
							{
								Fallback = safeMethodAttribute.Fallback,
								FallbackType = safeMethodAttribute.FallbackType,
								Swallow = safeMethodAttribute.Swallow,
								ClassCfg = cfg
							});
						}
						else
						{
							SafeMethodPatcher.PatchOne(harmony, propertyInfo.GetMethod, new SafeMethodPatcher.Behavior
							{
								Swallow = cfg.SwallowByDefault,
								ClassCfg = cfg
							});
						}
					}
					if (propertyInfo.SetMethod != null)
					{
						SafeMethodAttribute safeMethodAttribute2 = propertyInfo.SetMethod.GetCustomAttribute<SafeMethodAttribute>() ?? customAttribute2;
						if (safeMethodAttribute2 != null)
						{
							SafeMethodPatcher.PatchOne(harmony, propertyInfo.SetMethod, new SafeMethodPatcher.Behavior
							{
								Fallback = safeMethodAttribute2.Fallback,
								FallbackType = safeMethodAttribute2.FallbackType,
								Swallow = safeMethodAttribute2.Swallow,
								ClassCfg = cfg
							});
						}
						else
						{
							SafeMethodPatcher.PatchOne(harmony, propertyInfo.SetMethod, new SafeMethodPatcher.Behavior
							{
								Swallow = cfg.SwallowByDefault,
								ClassCfg = cfg
							});
						}
					}
				}
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000051DC File Offset: 0x000033DC
		private static void PatchOne(Harmony harmony, MethodBase method, SafeMethodPatcher.Behavior behavior)
		{
			Patches patchInfo = Harmony.GetPatchInfo(method);
			if (patchInfo != null)
			{
				ReadOnlyCollection<Patch> finalizers = patchInfo.Finalizers;
				if (finalizers == null || !finalizers.Any((Patch p) => p.owner == harmony.Id))
				{
					ReadOnlyCollection<Patch> prefixes = patchInfo.Prefixes;
					if (prefixes == null || !prefixes.Any((Patch p) => p.owner == harmony.Id))
					{
						ReadOnlyCollection<Patch> postfixes = patchInfo.Postfixes;
						if (postfixes == null || !postfixes.Any((Patch p) => p.owner == harmony.Id))
						{
							goto IL_78;
						}
					}
				}
				return;
			}
			IL_78:
			SafeMethodPatcher._behaviorCache[method] = behavior;
			MethodInfo methodInfo = method as MethodInfo;
			HarmonyMethod finalizer = (methodInfo != null && methodInfo.ReturnType != typeof(void)) ? new HarmonyMethod(typeof(SafeMethodPatcher).GetMethod("FinalizerGeneric", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
			{
				methodInfo.ReturnType
			})) : new HarmonyMethod(typeof(SafeMethodPatcher).GetMethod("FinalizerVoid", BindingFlags.Static | BindingFlags.NonPublic));
			HarmonyMethod prefix = null;
			HarmonyMethod postfix = null;
			try
			{
				harmony.Patch(method, prefix, postfix, null, finalizer);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000530C File Offset: 0x0000350C
		private static Exception FinalizerVoid(Exception __exception, MethodBase __originalMethod)
		{
			if (__exception == null)
			{
				return null;
			}
			ref SafeMethodPatcher.Behavior behavior = SafeMethodPatcher.GetBehavior(__originalMethod);
			Log.Exception(__exception, __originalMethod.ToString(), null);
			if (!behavior.Swallow)
			{
				return __exception;
			}
			return null;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005330 File Offset: 0x00003530
		private static Exception FinalizerGeneric<T>(Exception __exception, ref T __result, MethodBase __originalMethod)
		{
			if (__exception == null)
			{
				return null;
			}
			SafeMethodPatcher.Behavior behavior = SafeMethodPatcher.GetBehavior(__originalMethod);
			Log.Exception(__exception, __originalMethod.ToString(), null);
			if (behavior.Swallow)
			{
				object obj;
				if (SafeMethodPatcher.TryResolveFallback(typeof(T), behavior, out obj) && obj is T)
				{
					T t = (T)((object)obj);
					__result = t;
				}
				else
				{
					__result = default(T);
				}
				return null;
			}
			return __exception;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00005394 File Offset: 0x00003594
		private static SafeMethodPatcher.Behavior GetBehavior(MethodBase method)
		{
			SafeMethodPatcher.Behavior result;
			if (SafeMethodPatcher._behaviorCache.TryGetValue(method, out result))
			{
				return result;
			}
			SafeMethodAttribute customAttribute = method.GetCustomAttribute<SafeMethodAttribute>();
			if (customAttribute != null)
			{
				return new SafeMethodPatcher.Behavior
				{
					Fallback = customAttribute.Fallback,
					FallbackType = customAttribute.FallbackType,
					Swallow = customAttribute.Swallow
				};
			}
			if ((method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) && method.DeclaringType != null)
			{
				string name = method.Name.Substring(4);
				PropertyInfo property = method.DeclaringType.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				SafeMethodAttribute safeMethodAttribute = (property != null) ? property.GetCustomAttribute<SafeMethodAttribute>() : null;
				if (safeMethodAttribute != null)
				{
					return new SafeMethodPatcher.Behavior
					{
						Fallback = safeMethodAttribute.Fallback,
						FallbackType = safeMethodAttribute.FallbackType,
						Swallow = safeMethodAttribute.Swallow
					};
				}
			}
			return new SafeMethodPatcher.Behavior
			{
				Swallow = true
			};
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005494 File Offset: 0x00003694
		private static bool TryResolveFallback(Type returnType, SafeMethodPatcher.Behavior beh, out object value)
		{
			if (beh.Fallback != null)
			{
				value = beh.Fallback;
				return true;
			}
			if (beh.FallbackType != null && SafeMethodPatcher.TryConstruct(beh.FallbackType, returnType, out value))
			{
				return true;
			}
			if (beh.ClassCfg != null)
			{
				SafeClassAttribute classCfg = beh.ClassCfg;
				if (classCfg.UseEmptyArrayFallback && returnType.IsArray && returnType.GetArrayRank() == 1)
				{
					Type elementType = returnType.GetElementType();
					value = typeof(Array).GetMethod("Empty").MakeGenericMethod(new Type[]
					{
						elementType
					}).Invoke(null, null);
					return true;
				}
				Type type;
				if (classCfg.UseEmptyEnumerableFallback && SafeMethodPatcher.TryGetEnumerableItemType(returnType, out type))
				{
					Type type2 = classCfg.OpenGenericListFallback ?? typeof(List<>);
					if (type2.IsGenericTypeDefinition && type2.GetGenericArguments().Length == 1 && SafeMethodPatcher.TryConstruct(type2.MakeGenericType(new Type[]
					{
						type
					}), returnType, out value))
					{
						return true;
					}
				}
				if (returnType == typeof(int) && classCfg.UseIntFallback)
				{
					value = classCfg.IntFallback;
					return true;
				}
				if (returnType == typeof(long) && classCfg.UseLongFallback)
				{
					value = classCfg.LongFallback;
					return true;
				}
				if (returnType == typeof(float) && classCfg.UseFloatFallback)
				{
					value = classCfg.FloatFallback;
					return true;
				}
				if (returnType == typeof(double) && classCfg.UseDoubleFallback)
				{
					value = classCfg.DoubleFallback;
					return true;
				}
				if (returnType == typeof(bool) && classCfg.UseBoolFallback)
				{
					value = classCfg.BoolFallback;
					return true;
				}
				if (returnType == typeof(string) && classCfg.UseStringFallback)
				{
					value = classCfg.StringFallback;
					return true;
				}
			}
			value = null;
			return false;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000567C File Offset: 0x0000387C
		private static bool TryConstruct(Type toConstruct, Type returnType, out object value)
		{
			value = null;
			try
			{
				if (toConstruct.ContainsGenericParameters)
				{
					return false;
				}
				object obj = Activator.CreateInstance(toConstruct);
				if (obj == null)
				{
					return false;
				}
				if (returnType.IsAssignableFrom(toConstruct))
				{
					value = obj;
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000056D0 File Offset: 0x000038D0
		private static bool TryGetEnumerableItemType(Type t, out Type item)
		{
			item = null;
			if (t.IsArray)
			{
				item = t.GetElementType();
				return item != null;
			}
			if (t.IsGenericType)
			{
				Type genericTypeDefinition = t.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(IEnumerable<>) || genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IReadOnlyList<>) || genericTypeDefinition == typeof(IReadOnlyCollection<>))
				{
					item = t.GetGenericArguments()[0];
					return true;
				}
			}
			Type type = t.GetInterfaces().FirstOrDefault((Type i) => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			if (type != null)
			{
				item = type.GetGenericArguments()[0];
				return true;
			}
			return false;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000057B0 File Offset: 0x000039B0
		private static void LogException(MethodBase method, Exception ex)
		{
			try
			{
				Type declaringType = method.DeclaringType;
				string caller = (((declaringType != null) ? declaringType.FullName : null) ?? "<unknown>") + "." + method.Name;
				Log.Exception(ex, "", caller);
			}
			catch
			{
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000580C File Offset: 0x00003A0C
		private static IEnumerable<Type> SafeGetTypes(Assembly asm)
		{
			IEnumerable<Type> result;
			try
			{
				result = asm.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				result = from t in ex.Types
				where t != null
				select t;
			}
			return result;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005860 File Offset: 0x00003A60
		private static bool IsHarmonyPatchable(MethodBase m)
		{
			if (m.IsAbstract || m.GetMethodBody() == null)
			{
				return false;
			}
			if (m.DeclaringType != null && m.DeclaringType.ContainsGenericParameters)
			{
				return false;
			}
			MethodInfo methodInfo = m as MethodInfo;
			if (methodInfo != null)
			{
				if (methodInfo.ContainsGenericParameters)
				{
					return false;
				}
				Type returnType = methodInfo.ReturnType;
				if (returnType.IsGenericParameter || returnType.ContainsGenericParameters)
				{
					return false;
				}
				if (SafeMethodPatcher.<IsHarmonyPatchable>g__IsByRefLikeOrUnsupported|16_0(returnType))
				{
					return false;
				}
			}
			ParameterInfo[] parameters = m.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType.IsGenericParameter || parameterType.ContainsGenericParameters)
				{
					return false;
				}
				if (SafeMethodPatcher.<IsHarmonyPatchable>g__IsByRefLikeOrUnsupported|16_0(parameterType))
				{
					return false;
				}
			}
			return m.GetCustomAttribute<CompilerGeneratedAttribute>() == null || !(m.Name == "MoveNext");
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000593C File Offset: 0x00003B3C
		[CompilerGenerated]
		internal static bool <IsHarmonyPatchable>g__IsByRefLikeOrUnsupported|16_0(Type t)
		{
			if (t.IsPointer)
			{
				return true;
			}
			string text;
			if (!t.IsByRef)
			{
				text = t.FullName;
			}
			else
			{
				Type elementType = t.GetElementType();
				text = ((elementType != null) ? elementType.FullName : null);
			}
			string text2 = text;
			return text2 != null && (text2.StartsWith("System.Span`1", StringComparison.Ordinal) || text2.StartsWith("System.ReadOnlySpan`1", StringComparison.Ordinal) || text2 == "System.TypedReference");
		}

		// Token: 0x04000039 RID: 57
		private static bool _applied;

		// Token: 0x0400003A RID: 58
		private static readonly ConcurrentDictionary<MethodBase, SafeMethodPatcher.Behavior> _behaviorCache = new ConcurrentDictionary<MethodBase, SafeMethodPatcher.Behavior>();

		// Token: 0x02000102 RID: 258
		private struct Behavior
		{
			// Token: 0x040002E6 RID: 742
			public object Fallback;

			// Token: 0x040002E7 RID: 743
			public Type FallbackType;

			// Token: 0x040002E8 RID: 744
			public bool Swallow;

			// Token: 0x040002E9 RID: 745
			public SafeClassAttribute ClassCfg;
		}

		// Token: 0x02000103 RID: 259
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040002EA RID: 746
			public static Func<Assembly, IEnumerable<Type>> <0>__SafeGetTypes;
		}
	}
}
