using System;
using System.Collections.Generic;
using Retinues.Utils;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace Retinues.GUI.Helpers
{
	// Token: 0x02000066 RID: 102
	[SafeClass]
	public static class Tooltip
	{
		// Token: 0x060001F9 RID: 505 RVA: 0x0000E94F File Offset: 0x0000CB4F
		public static BasicTooltipViewModel MakeTooltip(string title, string description)
		{
			return new BasicTooltipViewModel(delegate()
			{
				List<TooltipProperty> list = new List<TooltipProperty>();
				if (!string.IsNullOrEmpty(title))
				{
					list.Add(new TooltipProperty("", title, 0, false, TooltipProperty.TooltipPropertyFlags.Title));
				}
				if (!string.IsNullOrEmpty(description))
				{
					list.Add(new TooltipProperty("", description, 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
				return list;
			});
		}
	}
}
