using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Features.AutoJoin;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor.VM.Troop.List;
using Retinues.GUI.Editor.VM.Troop.Panel;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Troop
{
	// Token: 0x0200007A RID: 122
	[SafeClass]
	public class TroopScreenVM : BaseVM
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x000111A4 File Offset: 0x0000F3A4
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"RemoveTroopButtonIsVisible",
					"RemoveTroopButtonIsEnabled",
					"RemoveTroopButtonText",
					"RemoveButtonHint",
					"ShowRetinueCap",
					"RetinueCapMax",
					"RetinueCapValue",
					"CanRaiseRetinueCap",
					"CanLowerRetinueCap",
					"RetinueJoinText",
					"CountInParty",
					"GenderIcon",
					"CaptainIsEnabled",
					"EnableCaptainButtonText",
					"EnableCaptainButtonBrush",
					"ShowEnableCaptainToggle",
					"ShowHeroAppearanceButton",
					"CustomizationIsEnabled"
				};
				dictionary[UIEvent.Party] = new string[]
				{
					"RetinueJoinText",
					"CountInParty"
				};
				dictionary[UIEvent.Appearance] = new string[]
				{
					"GenderIcon"
				};
				return dictionary;
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x00011291 File Offset: 0x0000F491
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00011299 File Offset: 0x0000F499
		[DataSourceProperty]
		public TroopListVM TroopList { get; private set; } = new TroopListVM();

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x000112A2 File Offset: 0x0000F4A2
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x000112AA File Offset: 0x0000F4AA
		[DataSourceProperty]
		public TroopPanelVM TroopPanel { get; private set; } = new TroopPanelVM();

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x000112B4 File Offset: 0x0000F4B4
		[DataSourceProperty]
		public int CountInParty
		{
			get
			{
				int result;
				if (!State.PartyData.TryGetValue(State.Troop, out result))
				{
					return 0;
				}
				return result;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x000112D7 File Offset: 0x0000F4D7
		[DataSourceProperty]
		public int RetinueCapValue
		{
			get
			{
				return AutoJoinBehavior.GetJoinCap(State.Troop);
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x000112E3 File Offset: 0x0000F4E3
		[DataSourceProperty]
		public int RetinueCapMax
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop == null || !troop.IsElite)
				{
					return RetinueManager.BasicRetinueCap;
				}
				return RetinueManager.EliteRetinueCap;
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00011303 File Offset: 0x0000F503
		[DataSourceProperty]
		public bool CaptainIsEnabled
		{
			get
			{
				if (State.Troop.IsCaptain)
				{
					WCharacter baseTroop = State.Troop.BaseTroop;
					return baseTroop != null && baseTroop.CaptainEnabled;
				}
				return false;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x00011328 File Offset: 0x0000F528
		[DataSourceProperty]
		public bool RemoveTroopButtonIsVisible
		{
			get
			{
				if (!ClanScreen.IsStudioMode)
				{
					WCharacter troop = State.Troop;
					if (troop != null && troop.IsRegular)
					{
						WCharacter troop2 = State.Troop;
						if (troop2 != null && troop2.IsCustom)
						{
							return base.IsVisible;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0001135F File Offset: 0x0000F55F
		[DataSourceProperty]
		public bool RemoveTroopButtonIsEnabled
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsDeletable;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002CB RID: 715 RVA: 0x00011371 File Offset: 0x0000F571
		[DataSourceProperty]
		public bool ShowRetinueCap
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsRetinue;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00011383 File Offset: 0x0000F583
		[DataSourceProperty]
		public bool CanLowerRetinueCap
		{
			get
			{
				return this.RetinueCapValue > 0;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0001138E File Offset: 0x0000F58E
		[DataSourceProperty]
		public bool CanRaiseRetinueCap
		{
			get
			{
				return this.RetinueCapValue < this.RetinueCapMax;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0001139E File Offset: 0x0000F59E
		// (set) Token: 0x060002CF RID: 719 RVA: 0x000113A6 File Offset: 0x0000F5A6
		[DataSourceProperty]
		public bool ShowCustomization { get; set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000113AF File Offset: 0x0000F5AF
		[DataSourceProperty]
		public bool CustomizationIsEnabled
		{
			get
			{
				if (Config.EnableTroopCustomization)
				{
					WCharacter troop = State.Troop;
					return troop != null && !troop.IsHero;
				}
				return false;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x000113D2 File Offset: 0x0000F5D2
		[DataSourceProperty]
		public bool ShowHeroAppearanceButton
		{
			get
			{
				return State.Troop is WHero && State.Troop.IsHero && base.IsVisible;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x000113F4 File Offset: 0x0000F5F4
		[DataSourceProperty]
		public bool ShowEnableCaptainToggle
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsCaptain && base.IsVisible;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00011411 File Offset: 0x0000F611
		[DataSourceProperty]
		public string RemoveTroopButtonText
		{
			get
			{
				return L.S("remove_button_text", "Remove");
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00011422 File Offset: 0x0000F622
		[DataSourceProperty]
		public string RetinueCapText
		{
			get
			{
				return L.S("retinue_cap_text", "Hiring Limit");
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00011434 File Offset: 0x0000F634
		[DataSourceProperty]
		public string RetinueJoinText
		{
			get
			{
				if (State.Troop == null || !State.Troop.IsRetinue)
				{
					return string.Empty;
				}
				if (this.RetinueCapValue == 0)
				{
					return L.S("retinue_join_text_none", "No new retinues will join.");
				}
				if (this.CountInParty >= this.RetinueCapValue)
				{
					return L.S("retinue_join_text_full", "The hiring limit has been reached.");
				}
				return L.T("retinue_join_text", "One new retinue per {COST} renown earned.").SetTextVariable("COST", RetinueManager.RenownRequiredPerUnit(State.Troop)).ToString();
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x000114BE File Offset: 0x0000F6BE
		[DataSourceProperty]
		public string HeroAppearanceButtonText
		{
			get
			{
				return L.S("hero_appearance_button_text", "Appearance");
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x000114CF File Offset: 0x0000F6CF
		[DataSourceProperty]
		public string EnableCaptainButtonText
		{
			get
			{
				if (!this.CaptainIsEnabled)
				{
					return L.S("enable_captain_button_text", "Enable Captain");
				}
				return L.S("disable_captain_button_text", "Disable Captain");
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x000114F8 File Offset: 0x0000F6F8
		[DataSourceProperty]
		public string EnableCaptainButtonBrush
		{
			get
			{
				if (!this.CaptainIsEnabled)
				{
					return "Popup.Done.Button";
				}
				return "Popup.Delete.Button";
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x0001150D File Offset: 0x0000F70D
		[DataSourceProperty]
		public string GenderIcon
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop == null || !troop.IsFemale)
				{
					return "SPGeneral\\GeneralFlagIcons\\male_only";
				}
				return "SPGeneral\\GeneralFlagIcons\\female_only";
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060002DA RID: 730 RVA: 0x00011530 File Offset: 0x0000F730
		[DataSourceProperty]
		public BasicTooltipViewModel RemoveButtonHint
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop != null && troop.IsDeletable)
				{
					return null;
				}
				string title = null;
				WCharacter troop2 = State.Troop;
				string description;
				if (!(((troop2 != null) ? troop2.Parent : null) == null))
				{
					WCharacter troop3 = State.Troop;
					int? num;
					if (troop3 == null)
					{
						num = null;
					}
					else
					{
						WCharacter[] upgradeTargets = troop3.UpgradeTargets;
						num = ((upgradeTargets != null) ? new int?(upgradeTargets.Count<WCharacter>()) : null);
					}
					int? num2 = num;
					description = ((num2.GetValueOrDefault() > 0) ? L.S("cant_remove_troop_with_targets", "Troops that have upgrade targets cannot be removed.") : string.Empty);
				}
				else
				{
					description = L.S("cant_remove_root_troop", "Root troops cannot be removed.");
				}
				return Tooltip.MakeTooltip(title, description);
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002DB RID: 731 RVA: 0x000115D5 File Offset: 0x0000F7D5
		[DataSourceProperty]
		public BasicTooltipViewModel RetinueCapHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("retinue_cap_tooltip_body", "Retinues will join you for free over time as long as you keep earning renown.\n\nYou can set a hiring limit to control how many retinues can join your party."));
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002DC RID: 732 RVA: 0x000115EC File Offset: 0x0000F7EC
		[DataSourceProperty]
		public BasicTooltipViewModel CustomizationHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("customization_hint", this.ShowCustomization ? "Hide customization controls" : "Show customization controls"));
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002DD RID: 733 RVA: 0x00011612 File Offset: 0x0000F812
		[DataSourceProperty]
		public BasicTooltipViewModel GenderToggleHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("gender_toggle_hint", "Toggle Gender"));
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00011629 File Offset: 0x0000F829
		[DataSourceProperty]
		public BasicTooltipViewModel GenderHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("gender_hint", "Gender"));
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002DF RID: 735 RVA: 0x00011640 File Offset: 0x0000F840
		[DataSourceProperty]
		public BasicTooltipViewModel AgeHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("age_hint", "Age"));
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00011657 File Offset: 0x0000F857
		[DataSourceProperty]
		public BasicTooltipViewModel HeightHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("height_hint", "Height"));
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0001166E File Offset: 0x0000F86E
		[DataSourceProperty]
		public BasicTooltipViewModel WeightHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("weight_hint", "Weight"));
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x00011685 File Offset: 0x0000F885
		[DataSourceProperty]
		public BasicTooltipViewModel BuildHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("build_hint", "Build"));
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0001169C File Offset: 0x0000F89C
		[DataSourceProperty]
		public BasicTooltipViewModel HeroAppearanceHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("hero_appearance_hint", "Open the full appearance editor for this hero."));
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x000116B4 File Offset: 0x0000F8B4
		[DataSourceMethod]
		public void ExecuteRaiseRetinueCap()
		{
			if (State.Troop == null)
			{
				return;
			}
			if (!this.CanRaiseRetinueCap)
			{
				return;
			}
			int cap = Math.Min(this.RetinueCapValue + BaseVM.BatchInput(false), this.RetinueCapMax);
			AutoJoinBehavior.SetJoinCap(State.Troop, cap);
			base.OnPropertyChanged("RetinueCapValue");
			base.OnPropertyChanged("CanRaiseRetinueCap");
			base.OnPropertyChanged("CanLowerRetinueCap");
			base.OnPropertyChanged("RetinueJoinText");
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00011728 File Offset: 0x0000F928
		[DataSourceMethod]
		public void ExecuteLowerRetinueCap()
		{
			if (State.Troop == null)
			{
				return;
			}
			if (!this.CanLowerRetinueCap)
			{
				return;
			}
			int cap = Math.Max(this.RetinueCapValue - BaseVM.BatchInput(false), 0);
			AutoJoinBehavior.SetJoinCap(State.Troop, cap);
			base.OnPropertyChanged("RetinueCapValue");
			base.OnPropertyChanged("CanRaiseRetinueCap");
			base.OnPropertyChanged("CanLowerRetinueCap");
			base.OnPropertyChanged("RetinueJoinText");
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00011798 File Offset: 0x0000F998
		[DataSourceMethod]
		public void ExecuteToggleCaptainEnabled()
		{
			WCharacter troop = State.Troop;
			if (((troop != null) ? troop.BaseTroop : null) == null || !State.Troop.IsCaptain)
			{
				return;
			}
			WCharacter baseTroop = State.Troop.BaseTroop;
			baseTroop.CaptainEnabled = !baseTroop.CaptainEnabled;
			State.UpdateTroop(State.Troop);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x000117F0 File Offset: 0x0000F9F0
		[DataSourceMethod]
		public void ExecuteRemoveTroop()
		{
			if (State.Troop == null)
			{
				return;
			}
			if (!State.Troop.IsDeletable)
			{
				return;
			}
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_remove", "remove")))
			{
				return;
			}
			string titleText = L.S("remove_troop", "Remove Troop");
			TextObject textObject = L.T("remove_troop_text", "Are you sure you want to permanently remove {TROOP_NAME}?\n\nTheir equipment will be stocked for later use, and existing troops will be converted to their {CULTURE} counterpart.").SetTextVariable("TROOP_NAME", State.Troop.Name);
			string tag = "CULTURE";
			WCulture culture = State.Troop.Culture;
			InformationManager.ShowInquiry(new InquiryData(titleText, textObject.SetTextVariable(tag, (culture != null) ? culture.Name : null).ToString(), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
			{
				WCharacter troop = State.Troop;
				foreach (WEquipment wequipment in troop.Loadout.Equipments)
				{
					int index = wequipment.Index;
					foreach (EquipmentIndex slot in WEquipment.Slots)
					{
						PendingEquipData pendingEquipData = EquipStagingBehavior.Get(troop, slot, index);
						if (pendingEquipData != null)
						{
							WItem stagedItem = new WItem(pendingEquipData.ItemId);
							EquipmentManager.RollbackStagedEquip(troop, index, slot, stagedItem);
							EquipStagingBehavior.Unstage(troop, slot, index);
						}
					}
				}
				TrainStagingBehavior.Unstage(troop);
				troop.Remove(null);
				State.UpdateFaction(State.Faction);
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00011902 File Offset: 0x0000FB02
		[DataSourceMethod]
		public void ExecuteToggleCustomization()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			this.ShowCustomization = !this.ShowCustomization;
			base.OnPropertyChanged("ShowCustomization");
			base.OnPropertyChanged("CustomizationHint");
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00011938 File Offset: 0x0000FB38
		[DataSourceMethod]
		public void ExecuteChangeGender()
		{
			AppearanceGuard.TryApply(State.Troop, State.Equipment.Index, delegate
			{
				State.Troop.IsFemale = !State.Troop.IsFemale;
				if (!State.Troop.IsHero)
				{
					BodyHelper.ApplyPropertiesFromCulture(State.Troop, State.Troop.Culture.Base);
				}
				return true;
			}, delegate
			{
				State.UpdateAppearance();
				base.OnPropertyChanged("GenderIcon");
			}, true);
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00011986 File Offset: 0x0000FB86
		[DataSourceMethod]
		public void ExecuteNextAgePreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyNextAgePreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002EB RID: 747 RVA: 0x000119A4 File Offset: 0x0000FBA4
		[DataSourceMethod]
		public void ExecutePrevAgePreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyPrevAgePreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000119C2 File Offset: 0x0000FBC2
		[DataSourceMethod]
		public void ExecuteNextHeightPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyNextHeightPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002ED RID: 749 RVA: 0x000119E0 File Offset: 0x0000FBE0
		[DataSourceMethod]
		public void ExecutePrevHeightPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyPrevHeightPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002EE RID: 750 RVA: 0x000119FE File Offset: 0x0000FBFE
		[DataSourceMethod]
		public void ExecuteNextWeightPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyNextWeightPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00011A1C File Offset: 0x0000FC1C
		[DataSourceMethod]
		public void ExecutePrevWeightPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyPrevWeightPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00011A3A File Offset: 0x0000FC3A
		[DataSourceMethod]
		public void ExecuteNextBuildPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyNextBuildPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00011A58 File Offset: 0x0000FC58
		[DataSourceMethod]
		public void ExecutePrevBuildPreset()
		{
			if (!Config.EnableTroopCustomization)
			{
				return;
			}
			BodyHelper.ApplyPrevBuildPreset(State.Troop);
			State.UpdateAppearance();
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00011A78 File Offset: 0x0000FC78
		[DataSourceMethod]
		public void ExecuteOpenHeroAppearance()
		{
			WCharacter troop = State.Troop;
			WHero hero = troop as WHero;
			if (hero == null)
			{
				return;
			}
			WCharacter snapshotTroop = State.Troop;
			BaseFaction snapshotFaction = State.Faction;
			AppearanceGuard.TryApply(State.Troop, State.Equipment.Index, () => true, delegate
			{
				HeroAppearanceHelper.OpenForHero(hero.Hero, null);
				State.PendingFaction = snapshotFaction;
				State.PendingTroop = snapshotTroop;
			}, true);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00011AFD File Offset: 0x0000FCFD
		public override void Show()
		{
			base.Show();
			this.TroopList.Show();
			this.TroopPanel.Show();
			base.OnPropertyChanged("RemoveTroopButtonIsVisible");
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00011B26 File Offset: 0x0000FD26
		public override void Hide()
		{
			this.TroopList.Hide();
			this.TroopPanel.Hide();
			base.Hide();
			base.OnPropertyChanged("RemoveTroopButtonIsVisible");
		}
	}
}
