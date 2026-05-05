using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Safety.Fixes;
using Retinues.Utils;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.Troops
{
	// Token: 0x02000038 RID: 56
	[SafeClass]
	public static class TroopBuilder
	{
		// Token: 0x0600010B RID: 267 RVA: 0x00006BCC File Offset: 0x00004DCC
		public static void EnsureTroopsExist(WFaction faction)
		{
			if (faction == null)
			{
				return;
			}
			if (faction.IsPlayerKingdom && Config.DisableKingdomTroops)
			{
				return;
			}
			if (!TroopBuilder.UpdateFactionTroops(faction))
			{
				return;
			}
			WParty.SwapAll(true, false, true, true, true, true, false, false, false, false);
			PartyLeaderFixBehavior.FixPartyLeaders();
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00006C18 File Offset: 0x00004E18
		private static bool UpdateFactionTroops(WFaction faction)
		{
			if (faction == null)
			{
				return false;
			}
			Log.Debug("Ensuring troops exist for faction: " + (((faction != null) ? faction.Name : null) ?? "null"));
			TroopBuilder.EnsureRetinueTroops(faction);
			if (!faction.HasFiefs && !Config.NoFiefRequirements)
			{
				Log.Debug("Skipping non-retinue troop initialization for fiefless faction.");
				return false;
			}
			return false | TroopBuilder.EnsureRegularTroops(faction) | TroopBuilder.EnsureMilitiaTroops(faction) | TroopBuilder.EnsureSpecialTroops(faction);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006C90 File Offset: 0x00004E90
		private static void EnsureRetinueTroops(WFaction faction)
		{
			if (faction.RetinueElite == null)
			{
				TroopBuilder.CreateRetinueTroop(faction, true);
			}
			else
			{
				Log.Debug("Elite retinue found, no need to initialize.");
			}
			if (faction.RetinueBasic == null)
			{
				TroopBuilder.CreateRetinueTroop(faction, false);
				return;
			}
			Log.Debug("Basic retinue found, no need to initialize.");
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006CC8 File Offset: 0x00004EC8
		private static bool EnsureRegularTroops(WFaction faction)
		{
			bool hasBasic = faction.RootBasic != null;
			bool hasElite = faction.RootElite != null;
			Log.Debug(string.Format("Custom troop presence: Basic={0}, Elite={1}.", hasBasic, hasElite));
			if (hasBasic & hasElite)
			{
				return false;
			}
			if (!faction.HasFiefs && !Config.NoFiefRequirements)
			{
				return false;
			}
			string titleText = L.S("custom_troops_inquiry_title", "Custom Troops Unlocked");
			TextObject textObject = L.T("custom_troops_inquiry_body", "Your {FACTION}'s custom troops are now unlocked.\n\nWould you like to clone the entire {CULTURE} troop tree, or would you prefer building them from scratch?\n\nCopying your culture's troops will provide you with good gear and good troops. Starting from scratch is the more difficult choice.\n\nThis decision is irreversible.").SetTextVariable("FACTION", faction.IsPlayerClan ? "clan" : "kingdom");
			string tag = "CULTURE";
			WCulture culture = faction.Culture;
			string text = textObject.SetTextVariable(tag, ((culture != null) ? culture.Name : null) ?? "culture").ToString();
			bool isAffirmativeOptionShown = true;
			bool isNegativeOptionShown = true;
			TextObject textObject2 = L.T("create_from_culture", "Copy {CULTURE}'s Troops");
			string tag2 = "CULTURE";
			WCulture culture2 = faction.Culture;
			InformationManager.ShowInquiry(new InquiryData(titleText, text, isAffirmativeOptionShown, isNegativeOptionShown, textObject2.SetTextVariable(tag2, ((culture2 != null) ? culture2.Name : null) ?? "Culture").ToString(), L.S("create_from_scratch", "Start from Scratch"), delegate()
			{
				base.<EnsureRegularTroops>g__CreateAllTroops|0(true);
			}, delegate()
			{
				base.<EnsureRegularTroops>g__CreateAllTroops|0(false);
			}, "", 0f, null, null, null), false, false);
			return false;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006E54 File Offset: 0x00005054
		private static bool EnsureMilitiaTroops(WFaction faction)
		{
			if (!DoctrineAPI.IsDoctrineUnlocked<StalwartMilitia>() && !Config.NoDoctrineRequirements)
			{
				return false;
			}
			bool flag = faction.MilitiaMelee != null;
			bool flag2 = faction.MilitiaMeleeElite != null;
			bool flag3 = faction.MilitiaRanged != null;
			bool flag4 = faction.MilitiaRangedElite != null;
			Log.Debug(string.Format("Militia presence: Melee={0}, MeleeElite={1}, ", flag, flag2) + string.Format("Ranged={0}, RangedElite={1}.", flag3, flag4));
			bool result = false;
			if (!flag)
			{
				TroopBuilder.CreateMilitiaTroop(faction, false, true);
				result = true;
			}
			if (!flag2)
			{
				TroopBuilder.CreateMilitiaTroop(faction, true, true);
				result = true;
			}
			if (!flag3)
			{
				TroopBuilder.CreateMilitiaTroop(faction, false, false);
				result = true;
			}
			if (!flag4)
			{
				TroopBuilder.CreateMilitiaTroop(faction, true, false);
				result = true;
			}
			return result;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00006F20 File Offset: 0x00005120
		private static bool EnsureSpecialTroops(WFaction faction)
		{
			WCulture culture = faction.Culture;
			bool result = false;
			if (DoctrineAPI.IsDoctrineUnlocked<RoadWardens>() || Config.NoDoctrineRequirements)
			{
				if (faction.CaravanGuard == null && ((culture != null) ? culture.CaravanGuard : null) != null)
				{
					Log.Info("Creating Caravan Guard troop for faction.");
					TroopBuilder.CreateSpecialTroop(culture.CaravanGuard, faction, RootCategory.CaravanGuard);
					result = true;
				}
				if (faction.CaravanMaster == null && ((culture != null) ? culture.CaravanMaster : null) != null)
				{
					Log.Info("Creating Caravan Master troop for faction.");
					TroopBuilder.CreateSpecialTroop(culture.CaravanMaster, faction, RootCategory.CaravanMaster);
					result = true;
				}
			}
			if ((DoctrineAPI.IsDoctrineUnlocked<ArmedPeasantry>() || Config.NoDoctrineRequirements) && faction.Villager == null && ((culture != null) ? culture.Villager : null) != null)
			{
				Log.Info("Creating Villager troop for faction.");
				TroopBuilder.CreateSpecialTroop(culture.Villager, faction, RootCategory.Villager);
				result = true;
			}
			return result;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00007000 File Offset: 0x00005200
		private static void CreateRetinueTroop(WFaction faction, bool isElite)
		{
			Log.Info(string.Format("Creating retinue troop for faction {0} (elite={1})", faction.Name, isElite));
			WCharacter wcharacter = isElite ? faction.Culture.RootElite : faction.Culture.RootBasic;
			if (wcharacter == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Cannot create retinue troop for faction ",
					faction.Name,
					" because its culture ",
					faction.Culture.Name,
					" has no ",
					isElite ? "elite" : "basic",
					" root troop."
				}));
				return;
			}
			WCharacter src = TroopBuilder.FindTemplate(wcharacter, (faction == Player.Kingdom) ? 2 : 0);
			WCharacter wcharacter2 = isElite ? new WCharacter(faction, RootCategory.RetinueElite, null) : new WCharacter(faction, RootCategory.RetinueBasic, null);
			wcharacter2.FillFrom(src, false, true, true);
			wcharacter2.Name = TroopBuilder.MakeRetinueName(faction, isElite);
			TroopBuilder.Initialize(wcharacter2);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000070F0 File Offset: 0x000052F0
		private static string MakeRetinueName(WFaction faction, bool isElite)
		{
			TextObject textObject;
			if (faction == Player.Kingdom)
			{
				if (isElite)
				{
					if (Player.IsFemale)
					{
						textObject = L.T("retinue_female_kingdom", "{FACTION} Queen's Champion");
					}
					else
					{
						textObject = L.T("retinue_male_kingdom", "{FACTION} King's Champion");
					}
				}
				else
				{
					textObject = L.T("retinue_royal_guard", "{FACTION} Royal Guard");
				}
			}
			else if (isElite)
			{
				textObject = L.T("retinue_house_champion", "{FACTION} House Champion");
			}
			else
			{
				textObject = L.T("retinue_house_guard", "{FACTION} House Guard");
			}
			return textObject.SetTextVariable("FACTION", faction.Name).ToString();
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00007188 File Offset: 0x00005388
		private static void CreateMilitiaTroop(WFaction faction, bool isElite, bool isMelee)
		{
			WCharacter wcharacter;
			if (isMelee)
			{
				wcharacter = (isElite ? faction.Culture.MilitiaMeleeElite : faction.Culture.MilitiaMelee);
			}
			else
			{
				wcharacter = (isElite ? faction.Culture.MilitiaRangedElite : faction.Culture.MilitiaRanged);
			}
			if (wcharacter == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"Cannot create militia troop for faction ",
					faction.Name,
					" because its culture ",
					faction.Culture.Name,
					" has no ",
					isElite ? "elite" : "basic",
					" ",
					isMelee ? "melee" : "ranged",
					" militia troop."
				}));
				return;
			}
			RootCategory category = isMelee ? (isElite ? RootCategory.MilitiaMeleeElite : RootCategory.MilitiaMelee) : (isElite ? RootCategory.MilitiaRangedElite : RootCategory.MilitiaRanged);
			WCharacter wcharacter2 = new WCharacter(faction, category, null);
			wcharacter2.FillFrom(wcharacter, false, true, true);
			wcharacter2.Name = TroopBuilder.BuildTroopName(wcharacter, faction);
			wcharacter2.Level += 5;
			TroopBuilder.Initialize(wcharacter2);
			Log.Info(string.Format("Created militia troop {0} for {1} (from {2})", wcharacter2.Name, faction.Name, wcharacter));
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000072B4 File Offset: 0x000054B4
		private static void CreateSpecialTroop(WCharacter tpl, WFaction faction, RootCategory category)
		{
			if (tpl == null)
			{
				return;
			}
			WCharacter wcharacter = new WCharacter(faction, category, null);
			wcharacter.FillFrom(tpl, false, true, true);
			wcharacter.Name = TroopBuilder.BuildTroopName(tpl, faction);
			TroopBuilder.Initialize(wcharacter);
			Log.Info(string.Format("Created special troop {0} (from {1})", wcharacter.Name, tpl));
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00007308 File Offset: 0x00005508
		public static void CreateTroops(WFaction faction, bool isElite, bool copyWholeTree)
		{
			WCulture culture = faction.Culture;
			WCharacter wcharacter = isElite ? ((culture != null) ? culture.RootElite : null) : ((culture != null) ? culture.RootBasic : null);
			if (wcharacter == null)
			{
				Log.Warn(string.Concat(new string[]
				{
					"Cannot clone ",
					isElite ? "elite" : "basic",
					" troops for ",
					faction.Name,
					": no ",
					isElite ? "elite" : "basic",
					" culture root."
				}));
				return;
			}
			List<WCharacter> list = (from t in TroopBuilder.CloneTroopTreeRecursive(wcharacter, isElite, faction, null, copyWholeTree, null)
			where t != null
			select t).ToList<WCharacter>();
			Log.Info(string.Format("Cloned {0} {1} troops for {2}.", list.Count, isElite ? "elite" : "basic", faction.Name));
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00007405 File Offset: 0x00005605
		private static IEnumerable<WCharacter> CloneTroopTreeRecursive(WCharacter vanilla, bool isElite, WFaction faction, WCharacter parent, bool copyWholeTree, Dictionary<string, WCharacter> cache = null)
		{
			TroopBuilder.<CloneTroopTreeRecursive>d__11 <CloneTroopTreeRecursive>d__ = new TroopBuilder.<CloneTroopTreeRecursive>d__11(-2);
			<CloneTroopTreeRecursive>d__.<>3__vanilla = vanilla;
			<CloneTroopTreeRecursive>d__.<>3__isElite = isElite;
			<CloneTroopTreeRecursive>d__.<>3__faction = faction;
			<CloneTroopTreeRecursive>d__.<>3__parent = parent;
			<CloneTroopTreeRecursive>d__.<>3__copyWholeTree = copyWholeTree;
			<CloneTroopTreeRecursive>d__.<>3__cache = cache;
			return <CloneTroopTreeRecursive>d__;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000743C File Offset: 0x0000563C
		private static string BuildTroopName(WCharacter vanilla, WFaction faction)
		{
			WCulture culture = faction.Culture;
			string text = (culture != null) ? culture.Name : null;
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(faction.Name) || string.IsNullOrEmpty(vanilla.Name))
			{
				return vanilla.Name;
			}
			string[] array = vanilla.Name.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2.Contains(text) && text.Length * 100 / text2.Length >= 80)
				{
					array[i] = faction.Name;
					return string.Join(" ", array);
				}
			}
			return faction.Name + " " + vanilla.Name;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000074F8 File Offset: 0x000056F8
		private static WCharacter FindTemplate(WCharacter root, int tierBonus = 0)
		{
			if (root == null)
			{
				return null;
			}
			WCharacter result = root;
			int num = root.Tier;
			int num2 = num + tierBonus;
			foreach (WCharacter wcharacter in root.Tree)
			{
				int tier = wcharacter.Tier;
				if (tier > num && tier <= num2)
				{
					num = tier;
					result = wcharacter;
					if (num == num2)
					{
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007578 File Offset: 0x00005778
		private static void Initialize(WCharacter troop)
		{
			troop.ComputeDerivedProperties();
			foreach (WEquipment wequipment in troop.Loadout.Equipments)
			{
				foreach (WItem witem in wequipment.Items)
				{
					witem.Unlock();
				}
			}
		}
	}
}
