using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Utils;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Retinues.GUI
{
	// Token: 0x02000061 RID: 97
	public static class SpriteLoader
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000E324 File Offset: 0x0000C524
		public static void LoadAllCategories()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot resourceDepot = UIResourceManager.ResourceDepot;
			foreach (KeyValuePair<string, SpriteCategory> keyValuePair in spriteData.SpriteCategories)
			{
				SpriteCategory value = keyValuePair.Value;
				if (!value.IsLoaded)
				{
					Log.Info("Loading sprite category '" + value.Name + "'...");
					value.Load(resourceContext, resourceDepot);
				}
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000E3B8 File Offset: 0x0000C5B8
		public static void LoadCategories(params string[] names)
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot resourceDepot = UIResourceManager.ResourceDepot;
			foreach (string text in names.Distinct<string>())
			{
				SpriteCategory spriteCategory;
				if (spriteData.SpriteCategories.TryGetValue(text, out spriteCategory) && !spriteCategory.IsLoaded)
				{
					Log.Info("Loading sprite category '" + text + "'...");
					spriteCategory.Load(resourceContext, resourceDepot);
				}
			}
		}
	}
}
