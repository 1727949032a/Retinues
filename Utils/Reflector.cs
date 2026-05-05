using System;
using System.Globalization;
using System.Reflection;

namespace Retinues.Utils
{
	// Token: 0x02000025 RID: 37
	public static class Reflector
	{
		// Token: 0x06000093 RID: 147 RVA: 0x0000452C File Offset: 0x0000272C
		private static PropertyInfo ResolveProperty(Type type, string name)
		{
			Type type2 = type;
			while (type2 != null)
			{
				PropertyInfo property = type2.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (property != null)
				{
					return property;
				}
				type2 = type2.BaseType;
			}
			return null;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004564 File Offset: 0x00002764
		private static FieldInfo ResolveField(Type type, string name)
		{
			Type type2 = type;
			while (type2 != null)
			{
				FieldInfo field = type2.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (field != null)
				{
					return field;
				}
				type2 = type2.BaseType;
			}
			return null;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000459C File Offset: 0x0000279C
		private static MethodInfo ResolveMethod(Type type, string name, Type[] parameterTypes)
		{
			Type type2 = type;
			while (type2 != null)
			{
				MethodInfo methodInfo = (parameterTypes != null) ? type2.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, parameterTypes, null) : type2.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (methodInfo != null)
				{
					return methodInfo;
				}
				type2 = type2.BaseType;
			}
			return null;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000045E4 File Offset: 0x000027E4
		public static TReturn GetPropertyValue<TReturn>(object instance, string propertyName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			Type type = instance.GetType();
			PropertyInfo propertyInfo = Reflector.ResolveProperty(type, propertyName);
			if (propertyInfo == null)
			{
				throw new MissingMemberException(type.FullName, propertyName);
			}
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new MissingMethodException(type.FullName + "." + propertyName + " has no getter.");
			}
			return (TReturn)((object)Reflector.ConvertIfNeeded(getMethod.Invoke(instance, null), typeof(TReturn)));
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004660 File Offset: 0x00002860
		public static object GetPropertyValue(object instance, string propertyName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			Type type = instance.GetType();
			PropertyInfo propertyInfo = Reflector.ResolveProperty(type, propertyName);
			if (propertyInfo == null)
			{
				throw new MissingMemberException(type.FullName, propertyName);
			}
			MethodInfo getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new MissingMethodException(type.FullName + "." + propertyName + " has no getter.");
			}
			return getMethod.Invoke(instance, null);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000046C8 File Offset: 0x000028C8
		public static void SetPropertyValue(object instance, string propertyName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			Type type = instance.GetType();
			PropertyInfo propertyInfo = Reflector.ResolveProperty(type, propertyName);
			if (propertyInfo == null)
			{
				throw new MissingMemberException(type.FullName, propertyName);
			}
			Type propertyType = propertyInfo.PropertyType;
			object obj = Reflector.ConvertIfNeeded(value, propertyType);
			MethodInfo setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod != null)
			{
				setMethod.Invoke(instance, new object[]
				{
					obj
				});
				return;
			}
			string text = char.ToLowerInvariant(propertyName[0]).ToString() + propertyName.Substring(1);
			foreach (string name in new string[]
			{
				"<" + propertyName + ">k__BackingField",
				"_" + text,
				"_" + propertyName,
				"m_" + text,
				text,
				propertyName
			})
			{
				FieldInfo fieldInfo = Reflector.ResolveField(type, name);
				if (fieldInfo != null)
				{
					object value2 = Reflector.ConvertIfNeeded(obj, fieldInfo.FieldType);
					fieldInfo.SetValue(instance, value2);
					return;
				}
			}
			throw new MissingMethodException(type.FullName + "." + propertyName + " has no setter and no recognizable backing field was found.");
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000480C File Offset: 0x00002A0C
		public static TReturn GetFieldValue<TReturn>(object instance, string fieldName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			FieldInfo fieldInfo = Reflector.ResolveField(instance.GetType(), fieldName);
			if (fieldInfo == null)
			{
				throw new MissingMemberException(instance.GetType().FullName, fieldName);
			}
			return (TReturn)((object)Reflector.ConvertIfNeeded(fieldInfo.GetValue(instance), typeof(TReturn)));
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004864 File Offset: 0x00002A64
		public static void SetFieldValue(object instance, string fieldName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			FieldInfo fieldInfo = Reflector.ResolveField(instance.GetType(), fieldName);
			if (fieldInfo == null)
			{
				throw new MissingMemberException(instance.GetType().FullName, fieldName);
			}
			FieldInfo fieldInfo2 = fieldInfo;
			fieldInfo2.SetValue(instance, Reflector.ConvertIfNeeded(value, fieldInfo2.FieldType));
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000048B8 File Offset: 0x00002AB8
		public static object InvokeMethod(object instance, string methodName, Type[] parameterTypes, params object[] args)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			Type type = instance.GetType();
			MethodInfo methodInfo2;
			if (parameterTypes == null)
			{
				Type[] array;
				if (args == null || args.Length == 0)
				{
					array = Type.EmptyTypes;
				}
				else
				{
					array = Array.ConvertAll<object, Type>(args, (object a) => ((a != null) ? a.GetType() : null) ?? typeof(object));
				}
				Type[] parameterTypes2 = array;
				MethodInfo methodInfo;
				if ((methodInfo = Reflector.ResolveMethod(type, methodName, parameterTypes2)) == null && (methodInfo = Reflector.ResolveMethod(type, methodName, null)) == null)
				{
					throw new MissingMethodException(type.FullName, methodName);
				}
				methodInfo2 = methodInfo;
			}
			else
			{
				MethodInfo methodInfo3 = Reflector.ResolveMethod(type, methodName, parameterTypes);
				if (methodInfo3 == null)
				{
					throw new MissingMethodException(type.FullName, methodName);
				}
				methodInfo2 = methodInfo3;
			}
			return methodInfo2.Invoke(instance, args);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000495C File Offset: 0x00002B5C
		private static object ConvertIfNeeded(object value, Type targetType)
		{
			if (targetType == typeof(void))
			{
				return null;
			}
			if (value == null)
			{
				if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
				{
					return null;
				}
				return Activator.CreateInstance(targetType);
			}
			else
			{
				Type type = value.GetType();
				if (targetType.IsAssignableFrom(type))
				{
					return value;
				}
				Type underlyingType = Nullable.GetUnderlyingType(targetType);
				if (underlyingType != null)
				{
					return Reflector.ConvertIfNeeded(value, underlyingType);
				}
				if (!targetType.IsEnum)
				{
					return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
				}
				if (type == typeof(string))
				{
					return Enum.Parse(targetType, (string)value, true);
				}
				return Enum.ToObject(targetType, Convert.ChangeType(value, Enum.GetUnderlyingType(targetType), CultureInfo.InvariantCulture));
			}
		}

		// Token: 0x04000022 RID: 34
		public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
	}
}
