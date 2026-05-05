using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Experience;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using Retinues.Utils;

namespace Retinues.Managers
{
	// Token: 0x0200005E RID: 94
	[SafeClass]
	public static class RetinueManager
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000D93F File Offset: 0x0000BB3F
		public static int EliteRetinueCap
		{
			get
			{
				return (int)((float)RetinueManager.RetinueCapBase * Config.MaxEliteRetinueRatio);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000D953 File Offset: 0x0000BB53
		public static int BasicRetinueCap
		{
			get
			{
				return (int)((float)RetinueManager.RetinueCapBase * Config.MaxBasicRetinueRatio);
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000D968 File Offset: 0x0000BB68
		private static int RetinueCapBase
		{
			get
			{
				WParty party = Player.Party;
				int num = (party != null) ? party.PartySizeLimit : 0;
				if (DoctrineAPI.IsDoctrineUnlocked<Vanguard>())
				{
					num = (int)((float)num * 1.15f);
				}
				return num;
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000D999 File Offset: 0x0000BB99
		public static int RetinueCapFor(WCharacter retinue)
		{
			if (!retinue.IsElite)
			{
				return RetinueManager.BasicRetinueCap;
			}
			return RetinueManager.EliteRetinueCap;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000D9AE File Offset: 0x0000BBAE
		public static int ConversionGoldCostPerUnit(WCharacter retinue)
		{
			if (retinue == null || !retinue.IsRetinue)
			{
				return 0;
			}
			return ((retinue.Tier <= 0) ? 1 : retinue.Tier) * Config.GoldConversionCostPerTier;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000D9E0 File Offset: 0x0000BBE0
		public static int ConversionInfluenceCostPerUnit(WCharacter retinue)
		{
			if (retinue == null || !retinue.IsRetinue)
			{
				return 0;
			}
			return ((retinue.Tier <= 0) ? 1 : retinue.Tier) * Config.InfluenceConversionCostPerTier;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000DA12 File Offset: 0x0000BC12
		public static int RenownRequiredPerUnit(WCharacter retinue)
		{
			if (retinue == null || !retinue.IsRetinue)
			{
				return 0;
			}
			return ((retinue.Tier <= 0) ? 1 : retinue.Tier) * Config.RenownRequiredPerTier;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000DA44 File Offset: 0x0000BC44
		public static int GetMaxConvertible(WCharacter from, WCharacter to)
		{
			int num = Player.Party.MemberRoster.CountOf(from);
			if (to.IsRetinue)
			{
				int num2 = Player.Party.MemberRoster.CountOf(to);
				int num3 = RetinueManager.RetinueCapFor(to);
				num = Math.Min(num, Math.Max(0, num3 - num2));
			}
			return num;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000DA94 File Offset: 0x0000BC94
		public static void Convert(WCharacter from, WCharacter to, int amountRequested)
		{
			int maxConvertible = RetinueManager.GetMaxConvertible(from, to);
			int num = Math.Min(amountRequested, maxConvertible);
			if (num <= 0)
			{
				return;
			}
			int num2 = to.IsRetinue ? (RetinueManager.ConversionGoldCostPerUnit(to) * num) : 0;
			int num3 = to.IsRetinue ? (RetinueManager.ConversionInfluenceCostPerUnit(to) * num) : 0;
			if (Player.Gold < num2)
			{
				return;
			}
			if (Player.Influence < num3)
			{
				return;
			}
			if (num2 > 0)
			{
				Player.ChangeGold(-num2);
			}
			if (num3 > 0)
			{
				Player.ChangeInfluence(-num3);
			}
			Player.Party.MemberRoster.RemoveTroop(from, num, 0);
			Player.Party.MemberRoster.AddTroop(to, num, 0, 0, -1);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000DB2C File Offset: 0x0000BD2C
		public static List<WCharacter> GetRetinueSourceTroops(WCharacter retinue)
		{
			RetinueManager.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.retinue = retinue;
			CS$<>8__locals1.sources = new List<WCharacter>(2);
			if (CS$<>8__locals1.retinue == null || !CS$<>8__locals1.retinue.IsRetinue)
			{
				return CS$<>8__locals1.sources;
			}
			RetinueManager.<GetRetinueSourceTroops>g__AddSourceIfValid|12_0(CS$<>8__locals1.retinue.Faction, ref CS$<>8__locals1);
			RetinueManager.<GetRetinueSourceTroops>g__AddSourceIfValid|12_0(CS$<>8__locals1.retinue.Culture, ref CS$<>8__locals1);
			return CS$<>8__locals1.sources;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000DB9A File Offset: 0x0000BD9A
		public static int RankUpCost(WCharacter retinue)
		{
			return ((retinue != null) ? retinue.Tier : 1) * Config.RankUpCostPerTier;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000DBB4 File Offset: 0x0000BDB4
		public static void RankUp(WCharacter retinue)
		{
			Log.Info("Attempting to rank up retinue '" + ((retinue != null) ? retinue.Name : null) + "'.");
			Log.Info(string.Format("Current tier: {0}, IsMaxTier: {1}", (retinue != null) ? new int?(retinue.Tier) : null, (retinue != null) ? new bool?(retinue.IsMaxTier) : null));
			if (retinue == null || retinue.IsMaxTier)
			{
				return;
			}
			int num = RetinueManager.RankUpCost(retinue);
			Log.Info(string.Format("Rank up cost: {0} gold, current player gold: {1}.", num, Player.Gold));
			if (Player.Gold < num)
			{
				return;
			}
			if ((Config.SkillXpCostPerPoint > 0 || Config.BaseSkillXpCost > 0) && !TroopXpBehavior.TrySpend(retinue, num))
			{
				return;
			}
			Player.ChangeGold(-num);
			retinue.Level = (retinue.Tier + 1) * 5 + 5;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000DCB0 File Offset: 0x0000BEB0
		[CompilerGenerated]
		internal static void <GetRetinueSourceTroops>g__AddSourceIfValid|12_0(BaseFaction f, ref RetinueManager.<>c__DisplayClass12_0 A_1)
		{
			if (f == null)
			{
				return;
			}
			WCharacter wcharacter = A_1.retinue.IsElite ? f.RootElite : f.RootBasic;
			if (wcharacter == null)
			{
				return;
			}
			WCharacter wcharacter2 = TroopMatcher.PickBestFromTree(wcharacter, A_1.retinue, null, true);
			if (wcharacter2 != null && wcharacter2.IsValid)
			{
				A_1.sources.Add(wcharacter2);
			}
		}
	}
}
