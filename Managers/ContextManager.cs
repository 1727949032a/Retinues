using System;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.Localization;

namespace Retinues.Managers
{
	// Token: 0x0200005C RID: 92
	[SafeClass]
	public static class ContextManager
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x0000C1FB File Offset: 0x0000A3FB
		public static bool IsAllowedInContext(WCharacter troop, string action)
		{
			return ClanScreen.IsStudioMode || ContextManager.GetContextReason(troop, action) == null;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000C214 File Offset: 0x0000A414
		public static TextObject GetContextReason(WCharacter troop, string action)
		{
			if (troop == null)
			{
				return null;
			}
			BaseFaction faction = troop.Faction;
			if (faction == null)
			{
				return null;
			}
			if (!Config.RestrictEditingToFiefs)
			{
				return null;
			}
			WSettlement currentSettlement = Player.CurrentSettlement;
			if (troop.IsRetinue)
			{
				if (currentSettlement != null)
				{
					return null;
				}
				return L.T("not_in_settlement_text", "You must be in a settlement to {ACTION} this troop.").SetTextVariable("ACTION", action);
			}
			else if (faction == Player.Clan)
			{
				if (((currentSettlement != null) ? currentSettlement.Clan : null) == Player.Clan)
				{
					return null;
				}
				return L.T("not_in_clan_fief_text", "You must be in one of your clan's fiefs to {ACTION} this troop.").SetTextVariable("ACTION", action);
			}
			else
			{
				if (!(faction == Player.Kingdom))
				{
					return null;
				}
				if (((currentSettlement != null) ? currentSettlement.Kingdom : null) == Player.Kingdom)
				{
					return null;
				}
				return L.T("not_in_kingdom_fief_text", "You must be in one of your kingdom's fiefs to {ACTION} this troop.").SetTextVariable("ACTION", action);
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000C308 File Offset: 0x0000A508
		public static bool IsAllowedInContextWithPopup(WCharacter troop, string action)
		{
			if (ClanScreen.IsStudioMode)
			{
				return true;
			}
			TextObject contextReason = ContextManager.GetContextReason(troop, action);
			if (contextReason == null)
			{
				return true;
			}
			BaseFaction faction = troop.Faction;
			TextObject title = L.T("not_allowed_title", "Not Allowed");
			if (troop.IsRetinue && faction == Player.Clan)
			{
				title = L.T("not_in_settlement", "Not in Settlement");
			}
			else if (faction == Player.Clan)
			{
				title = L.T("not_in_clan_fief", "Not in Clan Fief");
			}
			else if (faction == Player.Kingdom)
			{
				title = L.T("not_in_kingdom_fief", "Not in Kingdom Fief");
			}
			Notifications.Popup(title, contextReason, null, true);
			return false;
		}
	}
}
