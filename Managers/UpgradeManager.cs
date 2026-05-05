using System;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Managers
{
	// Token: 0x02000060 RID: 96
	[SafeClass]
	public static class UpgradeManager
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
		public static WCharacter AddUpgradeTarget(WCharacter troop, string targetName)
		{
			Log.Debug("AddUpgradeTarget: '" + targetName + "' for " + ((troop != null) ? troop.Name : null));
			WCharacter wcharacter = new WCharacter(troop, null);
			wcharacter.FillFrom(troop, false, !Config.EquippingTroopsCostsGold, true);
			wcharacter.Name = targetName.Trim();
			wcharacter.Level = troop.Tier * 5 + 8;
			return wcharacter;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000E24F File Offset: 0x0000C44F
		public static bool CanAddUpgradeToTroop(WCharacter character)
		{
			return UpgradeManager.GetAddUpgradeToTroopReason(character) == null;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000E260 File Offset: 0x0000C460
		public static TextObject GetAddUpgradeToTroopReason(WCharacter character)
		{
			if (character == null)
			{
				return L.T("invalid_args", "Invalid arguments.");
			}
			if (!character.IsRegular)
			{
				return L.T("not_regular_no_upgrade", "Only regular troops can be upgraded.");
			}
			if (character.IsHero)
			{
				return L.T("hero_no_upgrade", "Heroes cannot be upgraded.");
			}
			if (character.IsMaxTier)
			{
				return L.T("max_tier", "Troop is at max tier.");
			}
			int num = character.IsElite ? Config.MaxEliteUpgrades : Config.MaxBasicUpgrades;
			if (DoctrineAPI.IsDoctrineUnlocked<MastersAtArms>() && character.IsElite)
			{
				num++;
			}
			num = Math.Min(num, 4);
			if (character.UpgradeTargets.Count<WCharacter>() >= num)
			{
				return L.T("max_upgrades_reached", "Troop has reached maximum amount of upgrades.");
			}
			return null;
		}
	}
}
