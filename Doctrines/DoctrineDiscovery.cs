using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Retinues.Doctrines.Model;
using Retinues.Utils;

namespace Retinues.Doctrines
{
	// Token: 0x020000D4 RID: 212
	public static class DoctrineDiscovery
	{
		// Token: 0x06000892 RID: 2194 RVA: 0x0002B2B0 File Offset: 0x000294B0
		public static IReadOnlyList<Doctrine> DiscoverDoctrines(string namespaceStartsWith = "Retinues.Doctrines.Catalog")
		{
			List<Doctrine> list = new List<Doctrine>();
			try
			{
				foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
				{
					if (typeof(Doctrine).IsAssignableFrom(type) && !type.IsAbstract && type.FullName.StartsWith(namespaceStartsWith, StringComparison.Ordinal) && !(type.GetConstructor(Type.EmptyTypes) == null))
					{
						Doctrine item = (Doctrine)Activator.CreateInstance(type);
						list.Add(item);
					}
				}
				List<Doctrine> list2 = new List<Doctrine>();
				list2.AddRange(from d in list
				orderby d.Column, d.Row
				select d);
				return new <>z__ReadOnlyList<Doctrine>(list2);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
			return list;
		}
	}
}
