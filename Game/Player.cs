using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace Retinues.Game
{
	// Token: 0x0200008E RID: 142
	[SafeClass]
	public static class Player
	{
		// Token: 0x0600050E RID: 1294 RVA: 0x0001B153 File Offset: 0x00019353
		public static void Reset()
		{
			Player._culture = null;
			Player._character = null;
			Player._clan = null;
			Player._kingdom = null;
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x0001B16D File Offset: 0x0001936D
		public static WFaction Clan
		{
			get
			{
				if (Player._clan == null)
				{
					Player._clan = new WFaction(Hero.MainHero.Clan);
				}
				return Player._clan;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x0001B18F File Offset: 0x0001938F
		public static WCulture Culture
		{
			get
			{
				if (Player._culture == null)
				{
					Player._culture = new WCulture(Hero.MainHero.Culture);
				}
				return Player._culture;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x0001B1B1 File Offset: 0x000193B1
		public static WCharacter Character
		{
			get
			{
				if (Player._character == null)
				{
					Player._character = new WCharacter(Hero.MainHero.CharacterObject);
				}
				return Player._character;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x0001B1D3 File Offset: 0x000193D3
		public static WParty Party
		{
			get
			{
				return new WParty(MobileParty.MainParty);
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0001B1DF File Offset: 0x000193DF
		public static IFaction MapFaction
		{
			get
			{
				return Hero.MainHero.MapFaction;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x0001B1EB File Offset: 0x000193EB
		public static string Name
		{
			get
			{
				return Hero.MainHero.Name.ToString();
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x0001B1FC File Offset: 0x000193FC
		public static bool IsFemale
		{
			get
			{
				return Hero.MainHero.IsFemale;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x0001B208 File Offset: 0x00019408
		public static float Renown
		{
			get
			{
				return Hero.MainHero.Clan.Renown;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0001B219 File Offset: 0x00019419
		public static int Gold
		{
			get
			{
				return Hero.MainHero.Gold;
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001B225 File Offset: 0x00019425
		public static void ChangeGold(int amount)
		{
			Hero.MainHero.ChangeHeroGold(amount);
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x0001B232 File Offset: 0x00019432
		public static int Influence
		{
			get
			{
				if (TaleWorlds.CampaignSystem.Clan.PlayerClan == null)
				{
					return 0;
				}
				return (int)TaleWorlds.CampaignSystem.Clan.PlayerClan.Influence;
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001B248 File Offset: 0x00019448
		public static void ChangeInfluence(int amount)
		{
			if (TaleWorlds.CampaignSystem.Clan.PlayerClan == null)
			{
				return;
			}
			TaleWorlds.CampaignSystem.Clan.PlayerClan.Influence = Math.Max(0f, (float)(Player.Influence + amount));
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x0001B26E File Offset: 0x0001946E
		public static bool IsKingdomLeader
		{
			get
			{
				return Hero.MainHero.IsKingdomLeader;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x0001B27A File Offset: 0x0001947A
		public static WFaction Kingdom
		{
			get
			{
				if (!Player.IsKingdomLeader)
				{
					return null;
				}
				WFaction result;
				if ((result = Player._kingdom) == null)
				{
					result = (Player._kingdom = new WFaction(Hero.MainHero.Clan.Kingdom));
				}
				return result;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x0001B2A8 File Offset: 0x000194A8
		public static IEnumerable<WCharacter> Troops
		{
			get
			{
				return new Player.<get_Troops>d__32(-2);
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x0001B2B1 File Offset: 0x000194B1
		public static bool IsArmyLeader
		{
			get
			{
				Army army = Player.Party.Army;
				string a;
				if (army == null)
				{
					a = null;
				}
				else
				{
					MobileParty leaderParty = army.LeaderParty;
					a = ((leaderParty != null) ? leaderParty.StringId : null);
				}
				return a == Player.Party.StringId;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x0001B2E4 File Offset: 0x000194E4
		public static WSettlement CurrentSettlement
		{
			get
			{
				WParty party = Player.Party;
				bool flag;
				if (party == null)
				{
					flag = (null != null);
				}
				else
				{
					MobileParty @base = party.Base;
					flag = (((@base != null) ? @base.CurrentSettlement : null) != null);
				}
				if (!flag)
				{
					return null;
				}
				return new WSettlement(Player.Party.Base.CurrentSettlement);
			}
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001B31C File Offset: 0x0001951C
		[CommandLineFunctionality.CommandLineArgumentFunction("list_custom_troops", "retinues")]
		public static string ListCustomTroops(List<string> args)
		{
			List<string> list = (from t in Player.Troops
			select t.StringId + ": " + t.Name).ToList<string>();
			if (list.Count == 0)
			{
				return "No active custom troops found.";
			}
			return string.Join("\n", list);
		}

		// Token: 0x04000156 RID: 342
		private static WFaction _clan;

		// Token: 0x04000157 RID: 343
		private static WCulture _culture;

		// Token: 0x04000158 RID: 344
		private static WCharacter _character;

		// Token: 0x04000159 RID: 345
		private static WFaction _kingdom;
	}
}
