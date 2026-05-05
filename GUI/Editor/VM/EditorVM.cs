using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Agents;
using Retinues.Features.Statistics;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor.VM.Doctrines;
using Retinues.GUI.Editor.VM.Equipment;
using Retinues.GUI.Editor.VM.Troop;
using Retinues.GUI.Helpers;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM
{
	// Token: 0x02000079 RID: 121
	[SafeClass]
	public class EditorVM : BaseVM
	{
		// Token: 0x06000282 RID: 642 RVA: 0x0001055D File Offset: 0x0000E75D
		public EditorVM()
		{
			this.TroopScreen = new TroopScreenVM();
			this.EquipmentScreen = new EquipmentScreenVM();
			this.DoctrineScreen = new DoctrineScreenVM();
			this.SwitchScreen(Screen.Troop);
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0001058D File Offset: 0x0000E78D
		// (set) Token: 0x06000284 RID: 644 RVA: 0x00010595 File Offset: 0x0000E795
		public Screen Screen { get; set; }

		// Token: 0x06000285 RID: 645 RVA: 0x000105A0 File Offset: 0x0000E7A0
		public void SwitchScreen(Screen value)
		{
			if (this.Screen == value && base.IsVisible)
			{
				return;
			}
			if (ClanScreen.IsStudioMode && value == Screen.Doctrine)
			{
				return;
			}
			Log.Info(string.Format("Switching screen from {0} to {1}", this.Screen, value));
			this.Screen = value;
			if (this.Screen == Screen.Troop)
			{
				this.TroopScreen.Show();
			}
			else
			{
				this.TroopScreen.Hide();
			}
			if (this.Screen == Screen.Equipment)
			{
				this.EquipmentScreen.Show();
			}
			else
			{
				this.EquipmentScreen.Hide();
			}
			if (this.Screen == Screen.Doctrine)
			{
				this.DoctrineScreen.Show();
			}
			else
			{
				this.DoctrineScreen.Hide();
			}
			base.OnPropertyChanged("InTroopScreen");
			base.OnPropertyChanged("InEquipmentScreen");
			base.OnPropertyChanged("InDoctrineScreen");
			base.OnPropertyChanged("ShowFactionButton");
			base.OnPropertyChanged("ShowDoctrinesButton");
			base.OnPropertyChanged("ShowEquipmentButton");
			base.OnPropertyChanged("ShowGlobalEditorLink");
			base.OnPropertyChanged("ShowPersonalEditorLink");
			base.OnPropertyChanged("EquipmentButtonText");
			base.OnPropertyChanged("DoctrinesButtonText");
			base.OnPropertyChanged("FactionButtonText");
			base.OnPropertyChanged("EquipmentButtonBrush");
			base.OnPropertyChanged("DoctrinesButtonBrush");
			base.OnPropertyChanged("ShowLinksPanel");
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000286 RID: 646 RVA: 0x000106F0 File Offset: 0x0000E8F0
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Faction] = new string[]
				{
					"FactionButtonText",
					"ShowFactionButton",
					"CultureBanner",
					"ClanBanner",
					"CultureName",
					"ClanName",
					"TroopEditorTitle",
					"EnableTopPanelButtons",
					"ShowStatsButton"
				};
				dictionary[UIEvent.Troop] = new string[]
				{
					"IsCaptain",
					"CaptainModeHint",
					"ShowCaptainModeButton"
				};
				dictionary[UIEvent.Appearance] = new string[]
				{
					"Model"
				};
				return dictionary;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00010794 File Offset: 0x0000E994
		// (set) Token: 0x06000288 RID: 648 RVA: 0x0001079C File Offset: 0x0000E99C
		[DataSourceProperty]
		public TroopScreenVM TroopScreen { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000289 RID: 649 RVA: 0x000107A5 File Offset: 0x0000E9A5
		// (set) Token: 0x0600028A RID: 650 RVA: 0x000107AD File Offset: 0x0000E9AD
		[DataSourceProperty]
		public EquipmentScreenVM EquipmentScreen { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600028B RID: 651 RVA: 0x000107B6 File Offset: 0x0000E9B6
		// (set) Token: 0x0600028C RID: 652 RVA: 0x000107BE File Offset: 0x0000E9BE
		[DataSourceProperty]
		public DoctrineScreenVM DoctrineScreen { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600028D RID: 653 RVA: 0x000107C7 File Offset: 0x0000E9C7
		[DataSourceProperty]
		public bool IsCaptain
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsCaptain;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600028E RID: 654 RVA: 0x000107D9 File Offset: 0x0000E9D9
		[DataSourceProperty]
		public string CaptainModeButtonText
		{
			get
			{
				return L.S("captain_mode_button", "Captain Mode");
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600028F RID: 655 RVA: 0x000107EA File Offset: 0x0000E9EA
		[DataSourceProperty]
		public BasicTooltipViewModel CaptainModeHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, this.IsCaptain ? L.S("disable_captain_mode_tooltip_text", "Disable Captain Mode for this troop.") : L.S("enable_captain_mode_tooltip_text", "Enable Captain Mode for this troop."));
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0001081A File Offset: 0x0000EA1A
		[DataSourceProperty]
		public string TroopEditorTitle
		{
			get
			{
				if (ClanScreen.EditorMode == EditorMode.Culture)
				{
					return L.S("troop_editor_title_culture", "Culture Editor");
				}
				if (ClanScreen.EditorMode == EditorMode.Heroes)
				{
					return L.S("troop_editor_title_heroes", "Heroes Editor");
				}
				return L.S("troop_editor_title", "Troop Editor");
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0001085A File Offset: 0x0000EA5A
		[DataSourceProperty]
		public bool EnableTopPanelButtons
		{
			get
			{
				return ClanScreen.EditorMode != EditorMode.Heroes;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000292 RID: 658 RVA: 0x00010867 File Offset: 0x0000EA67
		[DataSourceProperty]
		public BasicTooltipViewModel TopPanelButtonsHint
		{
			get
			{
				if (!this.EnableTopPanelButtons)
				{
					return Tooltip.MakeTooltip(null, L.S("disabled_in_hero_editor", "Heroes are tied to the saved world state."));
				}
				return null;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000293 RID: 659 RVA: 0x00010888 File Offset: 0x0000EA88
		[DataSourceProperty]
		public BannerImageIdentifierVM CultureBanner
		{
			get
			{
				WCulture culture = State.Culture;
				if (culture == null)
				{
					return null;
				}
				return culture.GetBannerImage(1f);
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0001089F File Offset: 0x0000EA9F
		[DataSourceProperty]
		public BannerImageIdentifierVM ClanBanner
		{
			get
			{
				WClan clan = State.Clan;
				if (clan == null)
				{
					return null;
				}
				return clan.GetBannerImage(1f);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000295 RID: 661 RVA: 0x000108B6 File Offset: 0x0000EAB6
		[DataSourceProperty]
		public string CultureName
		{
			get
			{
				WCulture culture = State.Culture;
				if (culture == null)
				{
					return null;
				}
				return culture.Name;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000296 RID: 662 RVA: 0x000108C8 File Offset: 0x0000EAC8
		[DataSourceProperty]
		public string ClanName
		{
			get
			{
				if (ClanScreen.EditorMode != EditorMode.Heroes)
				{
					return L.S("clan_select", "Select Clan");
				}
				WClan clan = State.Clan;
				if (clan == null)
				{
					return null;
				}
				return clan.Name;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000297 RID: 663 RVA: 0x000108F2 File Offset: 0x0000EAF2
		[DataSourceProperty]
		public BasicTooltipViewModel CultureBannerHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("select_culture_hint", "Select a culture."));
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000298 RID: 664 RVA: 0x00010909 File Offset: 0x0000EB09
		[DataSourceProperty]
		public BasicTooltipViewModel ClanBannerHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("select_clan_hint", "Select a clan."));
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00010920 File Offset: 0x0000EB20
		[DataSourceProperty]
		public CharacterViewModel Model
		{
			get
			{
				WCharacter troop = State.Troop;
				WEquipment equipment = State.Equipment;
				if (troop == null || equipment == null)
				{
					return null;
				}
				bool applyGenderOverride = CombatAgentBehavior.IsEnabled(troop, equipment.Index, PolicyToggleType.GenderOverride);
				CharacterViewModel model = troop.GetModel(equipment.Index, applyGenderOverride);
				if (model == null)
				{
					return null;
				}
				if (PreviewOverlay.IsEnabled)
				{
					Equipment baseEquipment = troop.Loadout.Get(equipment.Index).StagingPreview();
					Equipment equipment2;
					if (PreviewOverlay.TryBuildEquipment(troop, equipment.Index, baseEquipment, out equipment2))
					{
						model.SetEquipment(equipment2);
					}
				}
				return model;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600029A RID: 666 RVA: 0x000109A2 File Offset: 0x0000EBA2
		[DataSourceProperty]
		public string EquipmentButtonText
		{
			get
			{
				if (this.Screen != Screen.Equipment)
				{
					return L.S("equipment_button_text", "Equipment");
				}
				return L.S("close_equipment_button_text", "Back");
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600029B RID: 667 RVA: 0x000109CC File Offset: 0x0000EBCC
		[DataSourceProperty]
		public string DoctrinesButtonText
		{
			get
			{
				if (this.Screen != Screen.Doctrine)
				{
					return L.S("doctrines_button_text", "Doctrines");
				}
				return L.S("close_doctrines_button_text", "Back");
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600029C RID: 668 RVA: 0x000109F6 File Offset: 0x0000EBF6
		[DataSourceProperty]
		public string FactionButtonText
		{
			get
			{
				if (!(State.Faction == Player.Clan))
				{
					return L.S("switch_to_clan_troops", "Clan Troops");
				}
				return L.S("switch_to_kingdom_troops", "Kingdom Troops");
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00010A28 File Offset: 0x0000EC28
		[DataSourceProperty]
		public string GlobalEditorLinkText
		{
			get
			{
				return L.S("global_editor_link_text", "Global Editor");
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600029E RID: 670 RVA: 0x00010A39 File Offset: 0x0000EC39
		[DataSourceProperty]
		public string PersonalEditorLinkText
		{
			get
			{
				return L.S("personal_editor_link_text", "Personal Editor");
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600029F RID: 671 RVA: 0x00010A4A File Offset: 0x0000EC4A
		[DataSourceProperty]
		public string HelpText
		{
			get
			{
				return L.S("editor_help_text", "Help");
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x00010A5B File Offset: 0x0000EC5B
		[DataSourceProperty]
		public string StatsText
		{
			get
			{
				return L.S("troop_stats_text", "Statistics");
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x00010A6C File Offset: 0x0000EC6C
		[DataSourceProperty]
		public bool ShowFactionButton
		{
			get
			{
				return this.Screen == Screen.Troop && Player.Kingdom != null && !Config.DisableKingdomTroops && !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00010A9C File Offset: 0x0000EC9C
		[DataSourceProperty]
		public bool ShowDoctrinesButton
		{
			get
			{
				if (this.Screen != Screen.Equipment)
				{
					Option<bool> enableDoctrines = Config.EnableDoctrines;
					if (enableDoctrines != null && enableDoctrines)
					{
						return !ClanScreen.IsStudioMode;
					}
				}
				return false;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x00010AD0 File Offset: 0x0000ECD0
		[DataSourceProperty]
		public bool ShowEquipmentButton
		{
			get
			{
				return this.Screen != Screen.Doctrine;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x00010ADE File Offset: 0x0000ECDE
		[DataSourceProperty]
		public bool ShowLinksPanel
		{
			get
			{
				return this.Screen != Screen.Doctrine;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x00010AEC File Offset: 0x0000ECEC
		[DataSourceProperty]
		public bool ShowGlobalEditorLink
		{
			get
			{
				return !ClanScreen.IsStudioMode && Config.EnableGlobalEditor;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00010B01 File Offset: 0x0000ED01
		[DataSourceProperty]
		public bool ShowPersonalEditorLink
		{
			get
			{
				return ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x00010B08 File Offset: 0x0000ED08
		[DataSourceProperty]
		public bool ShowStatsButton
		{
			get
			{
				if (!ClanScreen.IsStudioMode)
				{
					WCharacter troop = State.Troop;
					return troop != null && troop.IsCustom;
				}
				return false;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00010B24 File Offset: 0x0000ED24
		[DataSourceProperty]
		public bool ShowCaptainModeButton
		{
			get
			{
				if (ClanScreen.IsStudioMode)
				{
					return false;
				}
				if (!DoctrineAPI.IsDoctrineUnlocked<Captains>())
				{
					return false;
				}
				WCharacter troop = State.Troop;
				return !(troop == null) && (troop.CanHaveCaptain || troop.IsCaptain);
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x00010B64 File Offset: 0x0000ED64
		[DataSourceProperty]
		public string EquipmentButtonBrush
		{
			get
			{
				if (this.Screen != Screen.Equipment)
				{
					return "ButtonBrush1";
				}
				return "ButtonBrush3";
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002AA RID: 682 RVA: 0x00010B7A File Offset: 0x0000ED7A
		[DataSourceProperty]
		public string DoctrinesButtonBrush
		{
			get
			{
				if (this.Screen != Screen.Doctrine)
				{
					return "ButtonBrush1";
				}
				return "ButtonBrush3";
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002AB RID: 683 RVA: 0x00010B90 File Offset: 0x0000ED90
		[DataSourceProperty]
		public bool InTroopScreen
		{
			get
			{
				return this.Screen == Screen.Troop;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00010B9B File Offset: 0x0000ED9B
		[DataSourceProperty]
		public bool InEquipmentScreen
		{
			get
			{
				return this.Screen == Screen.Equipment;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002AD RID: 685 RVA: 0x00010BA6 File Offset: 0x0000EDA6
		[DataSourceProperty]
		public bool InDoctrineScreen
		{
			get
			{
				return this.Screen == Screen.Doctrine;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00010BB1 File Offset: 0x0000EDB1
		[DataSourceProperty]
		public BasicTooltipViewModel GlobalEditorHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("global_editor_tooltip_text", "Open the global editor for all factions."));
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002AF RID: 687 RVA: 0x00010BC8 File Offset: 0x0000EDC8
		[DataSourceProperty]
		public BasicTooltipViewModel PersonalEditorHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("personal_editor_tooltip_text", "Open the editor for your clan and kingdom troops."));
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x00010BDF File Offset: 0x0000EDDF
		[DataSourceProperty]
		public BasicTooltipViewModel HelpHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("editor_help_tooltip_text", "Open the online Retinues documentation."));
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x00010BF6 File Offset: 0x0000EDF6
		[DataSourceProperty]
		public BasicTooltipViewModel StatsHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("troop_stats_tooltip_text", "View battle statistics for this troop."));
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00010C0D File Offset: 0x0000EE0D
		[DataSourceMethod]
		public void ExecuteShowGlobalEditor()
		{
			ClanScreen instance = ClanScreen.Instance;
			if (instance == null)
			{
				return;
			}
			instance.ExecuteOpenStudioMode();
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00010C1E File Offset: 0x0000EE1E
		[DataSourceMethod]
		public void ExecuteShowPersonalEditor()
		{
			ClanScreen instance = ClanScreen.Instance;
			if (instance == null)
			{
				return;
			}
			instance.ExecuteOpenPlayerMode();
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00010C30 File Offset: 0x0000EE30
		[DataSourceMethod]
		public void ExecuteShowHelp()
		{
			InformationManager.ShowInquiry(new InquiryData(L.S("docs_title", "Open Documentation"), L.S("docs_body", "This will open the Retinues documentation in your default web browser.\n\nContinue?"), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), delegate()
			{
				URL.OpenInBrowser("https://bolaft.github.io/bannerlord-retinues/");
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00010CD4 File Offset: 0x0000EED4
		[DataSourceMethod]
		public void ExecuteShowTroopStats()
		{
			if (State.Troop == null)
			{
				return;
			}
			TroopStatisticsBehavior.ShowForTroop(State.Troop);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00010CF0 File Offset: 0x0000EEF0
		[DataSourceMethod]
		public void ExecuteToggleCaptainMode()
		{
			if (!DoctrineAPI.IsDoctrineUnlocked<Captains>())
			{
				return;
			}
			WCharacter troop = State.Troop;
			if (troop == null)
			{
				return;
			}
			if (!troop.IsCustom)
			{
				return;
			}
			State.UpdateTroop(troop.IsCaptain ? troop.BaseTroop : troop.Captain);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00010D39 File Offset: 0x0000EF39
		[DataSourceMethod]
		public void ExecuteToggleEquipment()
		{
			this.SwitchScreen((this.Screen == Screen.Equipment) ? Screen.Troop : Screen.Equipment);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00010D4E File Offset: 0x0000EF4E
		[DataSourceMethod]
		public void ExecuteToggleDoctrines()
		{
			this.SwitchScreen((this.Screen == Screen.Doctrine) ? Screen.Troop : Screen.Doctrine);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00010D63 File Offset: 0x0000EF63
		[DataSourceMethod]
		public void ExecuteSwitchFaction()
		{
			State.UpdateFaction((State.Faction == Player.Clan) ? Player.Kingdom : Player.Clan);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00010D88 File Offset: 0x0000EF88
		[DataSourceMethod]
		public void ExecuteSelectCulture()
		{
			try
			{
				List<InquiryElement> list = new List<InquiryElement>();
				foreach (WCulture wculture in WCulture.All)
				{
					if (wculture != null && wculture.Name != null && (!(wculture.RootBasic == null) || !(wculture.RootElite == null)))
					{
						list.Add(new InquiryElement(wculture.Base, wculture.Name, wculture.ImageIdentifier, true, null));
					}
				}
				if (list.Count == 0)
				{
					Notifications.Popup(L.T("no_cultures_title", "No Cultures Found"), L.T("no_cultures_text", "No cultures are loaded in the current game."), null, true);
				}
				else
				{
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("select_culture_title", "Select Culture"), null, list, true, 1, 1, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
					{
						if (selected == null || selected.Count == 0)
						{
							return;
						}
						InquiryElement inquiryElement = selected[0];
						CultureObject cultureObject = ((inquiryElement != null) ? inquiryElement.Identifier : null) as CultureObject;
						if (cultureObject == null)
						{
							return;
						}
						if (cultureObject.StringId == State.Faction.Culture.StringId)
						{
							return;
						}
						ClanScreen.EditorMode = EditorMode.Culture;
						State.UpdateFaction(new WCulture(cultureObject));
					}, delegate(List<InquiryElement> _)
					{
					}, "", false), false, false);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00010F00 File Offset: 0x0000F100
		[DataSourceMethod]
		public void ExecuteSelectClan()
		{
			try
			{
				List<InquiryElement> list = new List<InquiryElement>();
				foreach (WClan wclan in WClan.All)
				{
					if (wclan != null && wclan.Name != null && !(((wclan != null) ? wclan.Culture : null) != State.Culture))
					{
						list.Add(new InquiryElement(wclan.Base, wclan.Name, wclan.ImageIdentifier, true, null));
					}
				}
				if (list.Count == 0)
				{
					TextObject title = L.T("no_clans_title", "No Clans Found");
					TextObject textObject = L.T("no_clans_text", "No clans exist for the {CULTURE} culture.");
					string tag = "CULTURE";
					WCulture culture = State.Culture;
					Notifications.Popup(title, textObject.SetTextVariable(tag, ((culture != null) ? culture.Name : null) ?? L.S("current", "Current")), null, true);
				}
				else
				{
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("select_clan_title", "Select Clan"), null, list, true, 1, 1, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
					{
						if (selected == null || selected.Count == 0)
						{
							return;
						}
						InquiryElement inquiryElement = selected[0];
						Clan clan = ((inquiryElement != null) ? inquiryElement.Identifier : null) as Clan;
						if (clan == null)
						{
							return;
						}
						if (clan.StringId == State.Faction.StringId)
						{
							return;
						}
						ClanScreen.EditorMode = EditorMode.Heroes;
						State.UpdateFaction(new WClan(clan));
					}, delegate(List<InquiryElement> _)
					{
					}, "", false), false, false);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000110A4 File Offset: 0x0000F2A4
		[DataSourceMethod]
		public void ExecuteExportAll()
		{
			TroopImportExport.PromptAndExport(TroopImportExport.SuggestTimestampName("troops"));
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000110B5 File Offset: 0x0000F2B5
		[DataSourceMethod]
		public void ExecuteImportAll()
		{
			TroopImportExport.PickAndImportUnified(delegate
			{
				State.UpdateFaction(State.Faction);
				base.OnPropertyChanged("CultureBanner");
				base.OnPropertyChanged("ClanBanner");
				base.OnPropertyChanged("CultureName");
				base.OnPropertyChanged("ClanName");
			});
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000110C8 File Offset: 0x0000F2C8
		[DataSourceMethod]
		public void ExecuteResetAll()
		{
			InformationManager.ShowInquiry(new InquiryData(L.S("reset_all", "Reset All Troop Data"), L.S("reset_all_body", "This will reset all culture troop data to their default values. Player clan and kingdom troops will not be affected.\n\nContinue?"), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), delegate()
			{
				FactionBehavior.ResetCultureTroops = true;
				Notifications.Popup(L.T("reset_culture_troops_title", "Culture Troops Reset"), L.T("reset_culture_troops_text", "All culture troops will be reset upon saving and reloading the game."), null, true);
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}
	}
}
