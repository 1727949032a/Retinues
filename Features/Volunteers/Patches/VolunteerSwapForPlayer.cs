using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Features.Volunteers.Patches
{
	// Token: 0x020000AE RID: 174
	[HarmonyPatch]
	internal static class VolunteerSwapForPlayer
	{
		// Token: 0x06000749 RID: 1865 RVA: 0x000250C8 File Offset: 0x000232C8
		public static void Initialize()
		{
			VolunteerSwapForPlayer.ClearSnapshot();
			IMbEvent<MobileParty, Settlement> onSettlementLeftEvent = CampaignEvents.OnSettlementLeftEvent;
			object typeFromHandle = typeof(VolunteerSwapForPlayer);
			Action<MobileParty, Settlement> action;
			if ((action = VolunteerSwapForPlayer.<>O.<0>__OnSettlementLeft) == null)
			{
				action = (VolunteerSwapForPlayer.<>O.<0>__OnSettlementLeft = new Action<MobileParty, Settlement>(VolunteerSwapForPlayer.OnSettlementLeft));
			}
			onSettlementLeftEvent.AddNonSerializedListener(typeFromHandle, action);
			IMbEvent onBeforeSaveEvent = CampaignEvents.OnBeforeSaveEvent;
			object typeFromHandle2 = typeof(VolunteerSwapForPlayer);
			Action action2;
			if ((action2 = VolunteerSwapForPlayer.<>O.<1>__OnBeforeSave) == null)
			{
				action2 = (VolunteerSwapForPlayer.<>O.<1>__OnBeforeSave = new Action(VolunteerSwapForPlayer.OnBeforeSave));
			}
			onBeforeSaveEvent.AddNonSerializedListener(typeFromHandle2, action2);
			IMbEvent<MobileParty, Settlement, Hero> settlementEntered = CampaignEvents.SettlementEntered;
			object typeFromHandle3 = typeof(VolunteerSwapForPlayer);
			Action<MobileParty, Settlement, Hero> action3;
			if ((action3 = VolunteerSwapForPlayer.<>O.<2>__OnSettlementEntered) == null)
			{
				action3 = (VolunteerSwapForPlayer.<>O.<2>__OnSettlementEntered = new Action<MobileParty, Settlement, Hero>(VolunteerSwapForPlayer.OnSettlementEntered));
			}
			settlementEntered.AddNonSerializedListener(typeFromHandle3, action3);
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00025167 File Offset: 0x00023367
		static VolunteerSwapForPlayer()
		{
			VolunteerSwapForPlayer.Initialize();
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00025178 File Offset: 0x00023378
		private static void SnapshotVolunteers(WSettlement settlement)
		{
			List<WNotable> list = (settlement != null) ? settlement.Notables : null;
			if (list == null || list.Count == 0)
			{
				return;
			}
			Dictionary<string, WCharacter[]> dictionary = new Dictionary<string, WCharacter[]>(StringComparer.Ordinal);
			foreach (WNotable wnotable in list)
			{
				if (!string.IsNullOrEmpty((wnotable != null) ? wnotable.StringId : null))
				{
					Hero hero = wnotable.Hero;
					CharacterObject[] array = (hero != null) ? hero.VolunteerTypes : null;
					if (array != null && array.Length != 0)
					{
						WCharacter[] array2 = new WCharacter[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = ((array[i] != null) ? new WCharacter(array[i]) : null);
						}
						dictionary[wnotable.StringId] = array2;
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return;
			}
			VolunteerSwapForPlayer._settlement = settlement;
			VolunteerSwapForPlayer._snapshot = dictionary;
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00025270 File Offset: 0x00023470
		private static void RestoreSnapshot()
		{
			if (VolunteerSwapForPlayer._snapshot == null || VolunteerSwapForPlayer._settlement == null)
			{
				return;
			}
			List<WNotable> notables = VolunteerSwapForPlayer._settlement.Notables;
			if (notables == null || notables.Count == 0)
			{
				VolunteerSwapForPlayer.ClearSnapshot();
				return;
			}
			foreach (WNotable wnotable in notables)
			{
				WCharacter[] array;
				if (!string.IsNullOrEmpty((wnotable != null) ? wnotable.StringId : null) && VolunteerSwapForPlayer._snapshot.TryGetValue(wnotable.StringId, out array))
				{
					Hero hero = wnotable.Hero;
					CharacterObject[] array2 = (hero != null) ? hero.VolunteerTypes : null;
					if (array2 != null)
					{
						int num = Math.Min(array2.Length, array.Length);
						for (int i = 0; i < num; i++)
						{
							if (array2[i] != null)
							{
								WCharacter wcharacter = array[i];
								array2[i] = ((wcharacter != null) ? wcharacter.Base : null);
							}
						}
					}
				}
			}
			VolunteerSwapForPlayer.ClearSnapshot();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00025374 File Offset: 0x00023574
		private static void ClearSnapshot()
		{
			VolunteerSwapForPlayer._snapshot = null;
			VolunteerSwapForPlayer._settlement = null;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00025382 File Offset: 0x00023582
		private static void OnSettlementLeft(MobileParty party, Settlement _)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			VolunteerSwapForPlayer.RestoreSnapshot();
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00025392 File Offset: 0x00023592
		private static void OnBeforeSave()
		{
			VolunteerSwapForPlayer.RestoreSnapshot();
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00025399 File Offset: 0x00023599
		private static void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party != MobileParty.MainParty)
			{
				return;
			}
			VolunteerSwapForPlayer.TryBeginSwap();
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000253AC File Offset: 0x000235AC
		private static void TryBeginSwap()
		{
			if (VolunteerSwapForPlayer._snapshot != null)
			{
				return;
			}
			WSettlement wsettlement = WSettlement.Current;
			if (wsettlement == null || string.IsNullOrEmpty(wsettlement.StringId))
			{
				return;
			}
			bool flag = wsettlement.PlayerFaction != null;
			if (!flag && Config.RestrictToOwnedSettlements)
			{
				return;
			}
			if (flag)
			{
				wsettlement.SwapVolunteers(null);
				VolunteerSwapForPlayer.SnapshotVolunteers(wsettlement);
				VolunteerSwapForPlayer.ApplyPlayerVolunteerProportion(wsettlement);
				return;
			}
			WFaction clan = Player.Clan;
			if (clan == null)
			{
				return;
			}
			VolunteerSwapForPlayer.SnapshotVolunteers(wsettlement);
			wsettlement.SwapVolunteers(clan);
			VolunteerSwapForPlayer.ApplyPlayerVolunteerProportion(wsettlement);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00025438 File Offset: 0x00023638
		private static void ApplyPlayerVolunteerProportion(WSettlement settlement)
		{
			if (settlement == null)
			{
				return;
			}
			float num = Config.CustomVolunteersProportion;
			if (num <= 0f)
			{
				num = 0f;
			}
			else if (num >= 1f)
			{
				return;
			}
			WCulture culture = settlement.Culture;
			if (culture == null)
			{
				return;
			}
			WCharacter rootBasic = culture.RootBasic;
			WCharacter rootElite = culture.RootElite;
			if ((rootBasic == null || !rootBasic.IsValid) && (rootElite == null || !rootElite.IsValid))
			{
				return;
			}
			int num2 = (int)CampaignTime.Now.ToDays;
			Random random = new Random(settlement.StringId.GetHashCode() * 397 ^ num2);
			foreach (WNotable wnotable in settlement.Notables)
			{
				Hero hero = wnotable.Hero;
				CharacterObject[] array = (hero != null) ? hero.VolunteerTypes : null;
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						CharacterObject characterObject = array[i];
						if (characterObject != null)
						{
							WCharacter wcharacter = new WCharacter(characterObject);
							if (!wcharacter.IsValid)
							{
								array[i] = null;
							}
							else if (wcharacter.IsCustom && random.NextDouble() > (double)num)
							{
								WCharacter wcharacter2 = wcharacter.IsElite ? rootElite : rootBasic;
								if (!(wcharacter2 == null) && wcharacter2.IsValid)
								{
									WCharacter wcharacter3 = TroopMatcher.PickBestFromTree(wcharacter2, wcharacter, null, false);
									if (!(wcharacter3 == null) && wcharacter3.IsValid)
									{
										array[i] = wcharacter3.Base;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040001E1 RID: 481
		private static WSettlement _settlement;

		// Token: 0x040001E2 RID: 482
		private static Dictionary<string, WCharacter[]> _snapshot;

		// Token: 0x040001E3 RID: 483
		private static readonly Random _rng = new Random();

		// Token: 0x02000184 RID: 388
		[HarmonyPatch(typeof(PlayerTownVisitCampaignBehavior), "game_menu_settlement_wait_on_init")]
		private static class VolunteerSwapForPlayer_WaitStart
		{
			// Token: 0x06000C26 RID: 3110 RVA: 0x00034EF7 File Offset: 0x000330F7
			[HarmonyPostfix]
			private static void Postfix()
			{
				VolunteerSwapForPlayer.RestoreSnapshot();
			}
		}

		// Token: 0x02000185 RID: 389
		[HarmonyPatch(typeof(PlayerTownVisitCampaignBehavior), "game_menu_recruit_volunteers_on_consequence")]
		internal static class VolunteerSwapForPlayer_Begin
		{
			// Token: 0x06000C27 RID: 3111 RVA: 0x00034EFE File Offset: 0x000330FE
			[HarmonyPostfix]
			private static void Postfix()
			{
				VolunteerSwapForPlayer.TryBeginSwap();
			}
		}

		// Token: 0x02000186 RID: 390
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040004A5 RID: 1189
			public static Action<MobileParty, Settlement> <0>__OnSettlementLeft;

			// Token: 0x040004A6 RID: 1190
			public static Action <1>__OnBeforeSave;

			// Token: 0x040004A7 RID: 1191
			public static Action<MobileParty, Settlement, Hero> <2>__OnSettlementEntered;
		}
	}
}
