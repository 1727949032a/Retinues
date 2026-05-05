using System;
using System.Collections.Generic;
using Retinues.Safety.Sanitizer;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace Retinues.Mods.BanditMilitias
{
	// Token: 0x0200005A RID: 90
	public static class BanditMilitiasCheats
	{
		// Token: 0x060001AB RID: 427 RVA: 0x0000BCF4 File Offset: 0x00009EF4
		[CommandLineFunctionality.CommandLineArgumentFunction("purge_bandit_militias", "retinues")]
		public static string PurgeBanditMilitias(List<string> args)
		{
			if (!ModCompatibility.HasBanditMilitias)
			{
				return "BanditMilitias mod not detected; cannot purge Bandit Militia parties.";
			}
			int num = 0;
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (((mobileParty != null) ? mobileParty.PartyComponent : null) != null && !(mobileParty.PartyComponent.GetType().FullName != "BanditMilitias.ModBanditMilitiaPartyComponent"))
				{
					num++;
					PartySanitizer.SanitizeParty(mobileParty, true);
				}
			}
			if (num == 0)
			{
				return "No Bandit Militia parties found (is BanditMilitias active in this save?).";
			}
			return string.Format("Sanitized {0} Bandit Militia parties.", num);
		}
	}
}
