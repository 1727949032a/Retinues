using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Game.Wrappers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A7 RID: 167
	public class VanillaHelper
	{
		// Token: 0x0600071E RID: 1822 RVA: 0x0002425C File Offset: 0x0002245C
		private static VanillaHelper.CultureCache GetOrBuildCache(CharacterObject sample)
		{
			CultureObject cultureObject = (sample != null) ? sample.Culture : null;
			if (cultureObject == null)
			{
				return null;
			}
			string stringId = cultureObject.StringId;
			if (string.IsNullOrEmpty(stringId))
			{
				return null;
			}
			VanillaHelper.<>c__DisplayClass2_0 CS$<>8__locals1;
			if (VanillaHelper._cache.TryGetValue(stringId, out CS$<>8__locals1.c))
			{
				return CS$<>8__locals1.c;
			}
			CS$<>8__locals1.c = new VanillaHelper.CultureCache
			{
				CultureId = stringId,
				BasicRoot = cultureObject.BasicTroop,
				EliteRoot = cultureObject.EliteBasicTroop
			};
			VanillaHelper.<GetOrBuildCache>g__Crawl|2_0(CS$<>8__locals1.c.BasicRoot, CS$<>8__locals1.c.BasicSet, ref CS$<>8__locals1);
			VanillaHelper.<GetOrBuildCache>g__Crawl|2_0(CS$<>8__locals1.c.EliteRoot, CS$<>8__locals1.c.EliteSet, ref CS$<>8__locals1);
			VanillaHelper._cache[stringId] = CS$<>8__locals1.c;
			return CS$<>8__locals1.c;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00024324 File Offset: 0x00022524
		public static WCharacter GetParent(WCharacter node)
		{
			if (((node != null) ? node.Base : null) == null)
			{
				return null;
			}
			VanillaHelper.CultureCache orBuildCache = VanillaHelper.GetOrBuildCache(node.Base);
			if (orBuildCache == null)
			{
				return null;
			}
			string text2;
			string text = orBuildCache.ParentMap.TryGetValue(node.StringId, out text2) ? text2 : null;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(text);
			if (@object == null)
			{
				return null;
			}
			return new WCharacter(@object);
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x000243A8 File Offset: 0x000225A8
		[CompilerGenerated]
		internal static void <GetOrBuildCache>g__Crawl|2_0(CharacterObject root, HashSet<string> set, ref VanillaHelper.<>c__DisplayClass2_0 A_2)
		{
			if (root == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal)
			{
				root.StringId
			};
			Queue<CharacterObject> queue = new Queue<CharacterObject>();
			queue.Enqueue(root);
			set.Add(root.StringId);
			while (queue.Count > 0)
			{
				CharacterObject characterObject = queue.Dequeue();
				foreach (CharacterObject characterObject2 in characterObject.UpgradeTargets ?? Array.Empty<CharacterObject>())
				{
					if (((characterObject2 != null) ? characterObject2.Culture : null) == root.Culture && hashSet.Add(characterObject2.StringId))
					{
						set.Add(characterObject2.StringId);
						A_2.c.ParentMap[characterObject2.StringId] = characterObject.StringId;
						queue.Enqueue(characterObject2);
					}
				}
			}
		}

		// Token: 0x040001C8 RID: 456
		private static readonly Dictionary<string, VanillaHelper.CultureCache> _cache = new Dictionary<string, VanillaHelper.CultureCache>(StringComparer.Ordinal);

		// Token: 0x0200017D RID: 381
		private sealed class CultureCache
		{
			// Token: 0x04000478 RID: 1144
			public string CultureId;

			// Token: 0x04000479 RID: 1145
			public CharacterObject BasicRoot;

			// Token: 0x0400047A RID: 1146
			public CharacterObject EliteRoot;

			// Token: 0x0400047B RID: 1147
			public readonly HashSet<string> BasicSet = new HashSet<string>(StringComparer.Ordinal);

			// Token: 0x0400047C RID: 1148
			public readonly HashSet<string> EliteSet = new HashSet<string>(StringComparer.Ordinal);

			// Token: 0x0400047D RID: 1149
			public readonly Dictionary<string, string> ParentMap = new Dictionary<string, string>(StringComparer.Ordinal);
		}
	}
}
