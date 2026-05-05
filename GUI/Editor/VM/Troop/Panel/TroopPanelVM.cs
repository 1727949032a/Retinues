using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Features.Experience;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Troop.Panel
{
	// Token: 0x0200007C RID: 124
	[SafeClass]
	public sealed class TroopPanelVM : BaseVM
	{
		// Token: 0x0600030D RID: 781 RVA: 0x00011EF4 File Offset: 0x000100F4
		public TroopPanelVM()
		{
			this.BuildSkillRows(true);
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00011F44 File Offset: 0x00010144
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"ConversionRows",
					"SkillsRow1",
					"SkillsRow2",
					"SkillsRow3",
					"Name",
					"GenderText",
					"TierText",
					"SkillsHeaderText",
					"CanRankUp",
					"CanAddUpgrade",
					"AddUpgradeHint",
					"TroopXpIsEnabled",
					"TroopXpText",
					"SkillCapText",
					"SkillPointsTotal",
					"SkillPointsUsed",
					"UpgradeTargets",
					"IsRetinue",
					"IsRegular",
					"ShowUpgradesHeader",
					"ShowSkillSummary",
					"ShowExtrasSection",
					"ShowMariner",
					"HasExtraSkills",
					"IsMariner",
					"IsCustomRegular",
					"HasPendingConversions",
					"PendingTotalGoldCost",
					"PendingTotalInfluenceCost",
					"PendingTotalCount",
					"RetinueCap",
					"CultureText",
					"RaceText",
					"CanChangeRace",
					"FormationClassIcon",
					"FormationClassText",
					"IsHero",
					"Surname",
					"SurnameDisplay",
					"Traits",
					"IsCaptain",
					"CaptainIsEnabled",
					"CaptainText1",
					"CaptainText2"
				};
				dictionary[UIEvent.Train] = new string[]
				{
					"SkillPointsUsed",
					"TroopXpText",
					"TrainingRequired",
					"TrainingRequiredText",
					"TrainingRequiredTextColor",
					"TrainingRequiredHint"
				};
				dictionary[UIEvent.Conversion] = new string[]
				{
					"HasPendingConversions",
					"PendingTotalGoldCost",
					"PendingTotalInfluenceCost",
					"PendingTotalCount"
				};
				dictionary[UIEvent.Equip] = new string[]
				{
					"FormationClassIcon",
					"FormationClassText"
				};
				return dictionary;
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00012170 File Offset: 0x00010370
		protected override void OnTroopChange()
		{
			this._needsRebuild = true;
			if (base.IsVisible)
			{
				this.Build();
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00012187 File Offset: 0x00010387
		protected override void OnConversionChange()
		{
			this._needsRebuild = true;
			if (base.IsVisible)
			{
				this.Build();
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x000121A0 File Offset: 0x000103A0
		private void Build()
		{
			if (this._needsRebuild)
			{
				this._needsRebuild = false;
				if (State.Troop.IsRetinue)
				{
					Dictionary<WCharacter, int> conversionData = State.ConversionData;
					IEnumerable<WCharacter> enumerable = (conversionData != null) ? conversionData.Keys : null;
					IEnumerable<WCharacter> source = enumerable ?? Enumerable.Empty<WCharacter>();
					MBBindingList<TroopConversionRowVM> mbbindingList = new MBBindingList<TroopConversionRowVM>();
					foreach (TroopConversionRowVM item in from conversion in source
					select new TroopConversionRowVM(conversion))
					{
						mbbindingList.Add(item);
					}
					this._conversionRows = mbbindingList;
				}
				else
				{
					this._conversionRows = new MBBindingList<TroopConversionRowVM>();
				}
				base.OnPropertyChanged("ConversionRows");
			}
			foreach (TroopConversionRowVM troopConversionRowVM in this.ConversionRows)
			{
				troopConversionRowVM.IsVisible = base.IsVisible;
			}
			this.BuildSkillRows(false);
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000312 RID: 786 RVA: 0x000122B0 File Offset: 0x000104B0
		[DataSourceProperty]
		public MBBindingList<TroopConversionRowVM> ConversionRows
		{
			get
			{
				return this._conversionRows;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000313 RID: 787 RVA: 0x000122B8 File Offset: 0x000104B8
		[DataSourceProperty]
		public MBBindingList<TroopSkillVM> SkillsRow1
		{
			get
			{
				return this._skillsRow1;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000314 RID: 788 RVA: 0x000122C0 File Offset: 0x000104C0
		[DataSourceProperty]
		public MBBindingList<TroopSkillVM> SkillsRow2
		{
			get
			{
				return this._skillsRow2;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000315 RID: 789 RVA: 0x000122C8 File Offset: 0x000104C8
		[DataSourceProperty]
		public MBBindingList<TroopSkillVM> SkillsRow3
		{
			get
			{
				return this._skillsRow3;
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000316 RID: 790 RVA: 0x000122D0 File Offset: 0x000104D0
		[DataSourceProperty]
		public MBBindingList<TroopTraitVM> Traits
		{
			get
			{
				if (this._traits == null)
				{
					this._traits = new MBBindingList<TroopTraitVM>();
					foreach (TraitObject trait in WHero.PersonalityTraits)
					{
						this._traits.Add(new TroopTraitVM(trait));
					}
				}
				return this._traits;
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0001231F File Offset: 0x0001051F
		[DataSourceProperty]
		public string NameHeaderText
		{
			get
			{
				return L.S("name_header_text", "Name");
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000318 RID: 792 RVA: 0x00012330 File Offset: 0x00010530
		[DataSourceProperty]
		public string CultureHeaderText
		{
			get
			{
				if (!this.CanChangeRace)
				{
					return L.S("culture_header_text", "Culture");
				}
				return L.S("culture_and_race_header_text", "Culture & Race");
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000319 RID: 793 RVA: 0x00012359 File Offset: 0x00010559
		[DataSourceProperty]
		public string CaptainHeaderText
		{
			get
			{
				return L.S("captain_header_text", "Captain");
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0001236A File Offset: 0x0001056A
		[DataSourceProperty]
		public string SkillsHeaderText
		{
			get
			{
				if (!this.ShowSkillSummary)
				{
					return L.S("skills_header_text_no_formation", "Skills");
				}
				return L.S("skills_header_text", "Skills & Formation");
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00012393 File Offset: 0x00010593
		[DataSourceProperty]
		public string TraitsHeaderText
		{
			get
			{
				return L.S("traits_header_text", "Personality Traits");
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600031C RID: 796 RVA: 0x000123A4 File Offset: 0x000105A4
		[DataSourceProperty]
		public string FormationClassHeaderText
		{
			get
			{
				return L.S("formation_class_header_text", "Formation Class");
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600031D RID: 797 RVA: 0x000123B5 File Offset: 0x000105B5
		[DataSourceProperty]
		public string UpgradesHeaderText
		{
			get
			{
				return L.S("upgrades_header_text", "Upgrades");
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600031E RID: 798 RVA: 0x000123C6 File Offset: 0x000105C6
		[DataSourceProperty]
		public string TransferHeaderText
		{
			get
			{
				return L.S("transfer_header_text", "Transfer");
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600031F RID: 799 RVA: 0x000123D7 File Offset: 0x000105D7
		[DataSourceProperty]
		public string Name
		{
			get
			{
				WCharacter troop = State.Troop;
				return Format.Crop((troop != null) ? troop.Name : null, 35);
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000320 RID: 800 RVA: 0x000123F4 File Offset: 0x000105F4
		[DataSourceProperty]
		public string Surname
		{
			get
			{
				WHero whero = State.Troop as WHero;
				if (whero == null)
				{
					return string.Empty;
				}
				return whero.Surname;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0001241C File Offset: 0x0001061C
		[DataSourceProperty]
		public string SurnameDisplay
		{
			get
			{
				WHero whero = State.Troop as WHero;
				if (whero == null)
				{
					return string.Empty;
				}
				if (!string.IsNullOrEmpty(whero.Surname))
				{
					return whero.Surname;
				}
				return "...";
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000322 RID: 802 RVA: 0x00012456 File Offset: 0x00010656
		[DataSourceProperty]
		public string GenderText
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop == null || !troop.IsFemale)
				{
					return L.S("male", "Male");
				}
				return L.S("female", "Female");
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0001248A File Offset: 0x0001068A
		[DataSourceProperty]
		public string CultureText
		{
			get
			{
				WCharacter troop = State.Troop;
				string text;
				if (troop == null)
				{
					text = null;
				}
				else
				{
					WCulture culture = troop.Culture;
					text = ((culture != null) ? culture.Name : null);
				}
				return text ?? L.S("unknown", "Unknown");
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000324 RID: 804 RVA: 0x000124BC File Offset: 0x000106BC
		[DataSourceProperty]
		public string RaceText
		{
			get
			{
				WCharacter troop = State.Troop;
				return TroopPanelVM.GetRaceName((troop != null) ? troop.Race : -1) ?? L.S("unknown", "Unknown");
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000325 RID: 805 RVA: 0x000124E7 File Offset: 0x000106E7
		[DataSourceProperty]
		public bool CanChangeRace
		{
			get
			{
				return FaceGen.GetRaceCount() > 1;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000326 RID: 806 RVA: 0x000124F4 File Offset: 0x000106F4
		[DataSourceProperty]
		public string TierText
		{
			get
			{
				WCharacter troop = State.Troop;
				int num = (troop != null) ? troop.Tier : 0;
				string text;
				switch (num)
				{
				case 0:
					text = "0";
					break;
				case 1:
					text = "I";
					break;
				case 2:
					text = "II";
					break;
				case 3:
					text = "III";
					break;
				case 4:
					text = "IV";
					break;
				case 5:
					text = "V";
					break;
				case 6:
					text = "VI";
					break;
				case 7:
					text = "VII";
					break;
				case 8:
					text = "VIII";
					break;
				case 9:
					text = "IX";
					break;
				case 10:
					text = "X";
					break;
				default:
					text = num.ToString();
					break;
				}
				string str = text;
				return L.S("tier", "Tier") + " " + str;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000327 RID: 807 RVA: 0x000125C3 File Offset: 0x000107C3
		[DataSourceProperty]
		public bool CanRankUp
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop != null && troop.IsRetinue)
				{
					WCharacter troop2 = State.Troop;
					return troop2 != null && !troop2.IsMaxTier;
				}
				return false;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000328 RID: 808 RVA: 0x000125ED File Offset: 0x000107ED
		[DataSourceProperty]
		public bool TroopXpIsEnabled
		{
			get
			{
				return !ClanScreen.IsStudioMode && (Config.BaseSkillXpCost > 0 || Config.SkillXpCostPerPoint > 0);
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000329 RID: 809 RVA: 0x00012614 File Offset: 0x00010814
		[DataSourceProperty]
		public string TroopXpText
		{
			get
			{
				return L.T("troop_xp", "{XP} xp").SetTextVariable("XP", TroopXpBehavior.Get(State.Troop)).ToString();
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600032A RID: 810 RVA: 0x0001263E File Offset: 0x0001083E
		[DataSourceProperty]
		public bool ShowSkillSummary
		{
			get
			{
				return ClanScreen.EditorMode != EditorMode.Heroes;
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0001264C File Offset: 0x0001084C
		[DataSourceProperty]
		public string SkillCapText
		{
			get
			{
				int variable = (State.Troop != null) ? SkillManager.SkillCapByTier(State.Troop) : 0;
				return L.T("skill_cap_text", "{CAP} skill cap").SetTextVariable("CAP", variable).ToString();
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00012693 File Offset: 0x00010893
		[DataSourceProperty]
		public int SkillPointsTotal
		{
			get
			{
				if (!(State.Troop != null))
				{
					return 0;
				}
				return SkillManager.SkillTotalByTier(State.Troop);
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600032D RID: 813 RVA: 0x000126AE File Offset: 0x000108AE
		[DataSourceProperty]
		public int SkillPointsUsed
		{
			get
			{
				Dictionary<SkillObject, SkillData> skillData = State.SkillData;
				if (skillData == null)
				{
					return 0;
				}
				return skillData.Sum(delegate(KeyValuePair<SkillObject, SkillData> kv)
				{
					int value2 = kv.Value.Value;
					PendingTrainData train = kv.Value.Train;
					return value2 + ((train != null) ? train.PointsRemaining : 0);
				});
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600032E RID: 814 RVA: 0x000126E0 File Offset: 0x000108E0
		[DataSourceProperty]
		public int TrainingRequired
		{
			get
			{
				Dictionary<SkillObject, SkillData> skillData = State.SkillData;
				int? num;
				if (skillData == null)
				{
					num = null;
				}
				else
				{
					num = skillData.Sum(delegate(KeyValuePair<SkillObject, SkillData> kv)
					{
						PendingTrainData train = kv.Value.Train;
						if (train == null)
						{
							return null;
						}
						return new int?(train.Remaining);
					});
				}
				int? num2 = num;
				return num2.GetValueOrDefault();
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0001272D File Offset: 0x0001092D
		[DataSourceProperty]
		public bool TrainingTakesTime
		{
			get
			{
				return Config.TrainingTroopsTakesTime && !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00012745 File Offset: 0x00010945
		[DataSourceProperty]
		public string TrainingRequiredText
		{
			get
			{
				if (this.TrainingRequired == 0)
				{
					return L.S("no_training_required", "No training required");
				}
				return L.T("training_required", "{HOURS} hours of training required").SetTextVariable("HOURS", this.TrainingRequired).ToString();
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000331 RID: 817 RVA: 0x00012783 File Offset: 0x00010983
		[DataSourceProperty]
		public string TrainingRequiredTextColor
		{
			get
			{
				if (this.TrainingRequired <= 0)
				{
					return "#F4E1C4FF";
				}
				return "#ebaf2fff";
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00012799 File Offset: 0x00010999
		[DataSourceProperty]
		public BasicTooltipViewModel TrainingRequiredHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("training_required_tooltip_body", "Before skill point increases are applied, troops must undergo training.\n\nThis is done by selecting 'Train troops' from a fief's town menu."));
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000333 RID: 819 RVA: 0x000127B0 File Offset: 0x000109B0
		[DataSourceProperty]
		public bool IsHero
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsHero;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000334 RID: 820 RVA: 0x000127C2 File Offset: 0x000109C2
		[DataSourceProperty]
		public bool IsRegular
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsRegular;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000335 RID: 821 RVA: 0x000127D4 File Offset: 0x000109D4
		[DataSourceProperty]
		public bool ShowUpgradesHeader
		{
			get
			{
				return this.IsRegular && (!ClanScreen.IsStudioMode || this.UpgradeTargets.Count > 0);
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000336 RID: 822 RVA: 0x000127F8 File Offset: 0x000109F8
		[DataSourceProperty]
		public bool IsCustomRegular
		{
			get
			{
				bool isRegular = this.IsRegular;
				WCharacter troop = State.Troop;
				bool flag = troop != null && troop.IsCustom;
				return isRegular && flag;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000337 RID: 823 RVA: 0x0001281F File Offset: 0x00010A1F
		[DataSourceProperty]
		public bool CanAddUpgrade
		{
			get
			{
				return this.CantAddUpgradeReason == null;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000338 RID: 824 RVA: 0x0001282D File Offset: 0x00010A2D
		[DataSourceProperty]
		public string AddUpgradeButtonText
		{
			get
			{
				return L.S("add_upgrade_button_text", "Add Upgrade");
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000339 RID: 825 RVA: 0x0001283E File Offset: 0x00010A3E
		[DataSourceProperty]
		public TextObject CantAddUpgradeReason
		{
			get
			{
				return UpgradeManager.GetAddUpgradeToTroopReason(State.Troop);
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600033A RID: 826 RVA: 0x0001284A File Offset: 0x00010A4A
		[DataSourceProperty]
		public BasicTooltipViewModel AddUpgradeHint
		{
			get
			{
				if (!this.CanAddUpgrade)
				{
					return Tooltip.MakeTooltip(null, this.CantAddUpgradeReason.ToString());
				}
				return null;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00012867 File Offset: 0x00010A67
		[DataSourceProperty]
		public bool IsRetinue
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsRetinue;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00012879 File Offset: 0x00010A79
		[DataSourceProperty]
		public string ButtonApplyConversionsText
		{
			get
			{
				return L.S("apply_conversions_button_text", "Convert");
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600033D RID: 829 RVA: 0x0001288A File Offset: 0x00010A8A
		[DataSourceProperty]
		public string ButtonClearConversionsText
		{
			get
			{
				return L.S("clear_conversions_button_text", "Clear");
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0001289B File Offset: 0x00010A9B
		[DataSourceProperty]
		public bool HasPendingConversions
		{
			get
			{
				return this.ConversionRows.Any((TroopConversionRowVM r) => r.HasPendingConversions);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600033F RID: 831 RVA: 0x000128C7 File Offset: 0x00010AC7
		[DataSourceProperty]
		public int PendingTotalGoldCost
		{
			get
			{
				return this.ConversionRows.Sum((TroopConversionRowVM r) => r.GoldConversionCost);
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000340 RID: 832 RVA: 0x000128F3 File Offset: 0x00010AF3
		[DataSourceProperty]
		public int PendingTotalInfluenceCost
		{
			get
			{
				return this.ConversionRows.Sum((TroopConversionRowVM r) => r.InfluenceConversionCost);
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0001291F File Offset: 0x00010B1F
		[DataSourceProperty]
		public int PendingTotalCount
		{
			get
			{
				return this.ConversionRows.Sum((TroopConversionRowVM r) => Math.Abs(r.PendingAmount));
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000342 RID: 834 RVA: 0x0001294B File Offset: 0x00010B4B
		[DataSourceProperty]
		public int RetinueCap
		{
			get
			{
				if (!(State.Troop != null))
				{
					return 0;
				}
				return RetinueManager.RetinueCapFor(State.Troop);
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000343 RID: 835 RVA: 0x00012968 File Offset: 0x00010B68
		public MBBindingList<TroopUpgradeTargetVM> UpgradeTargets
		{
			get
			{
				MBBindingList<TroopUpgradeTargetVM> mbbindingList = new MBBindingList<TroopUpgradeTargetVM>();
				foreach (TroopUpgradeTargetVM item in from t in State.Troop.UpgradeTargets
				select new TroopUpgradeTargetVM(t))
				{
					mbbindingList.Add(item);
				}
				return mbbindingList;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000129E4 File Offset: 0x00010BE4
		[DataSourceProperty]
		public bool AllowFormationOverrides
		{
			get
			{
				return Config.AllowFormationOverrides;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000345 RID: 837 RVA: 0x000129F0 File Offset: 0x00010BF0
		[DataSourceProperty]
		public string FormationClassText
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop == null)
				{
					return null;
				}
				return troop.FormationClass.GetLocalizedName().ToString();
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00012A0C File Offset: 0x00010C0C
		[DataSourceProperty]
		public string FormationClassIcon
		{
			get
			{
				return Icons.GetFormationClassIcon(State.Troop);
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000347 RID: 839 RVA: 0x00012A18 File Offset: 0x00010C18
		[DataSourceProperty]
		public BasicTooltipViewModel FormationClassHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("formation_class_hint", "Change Formation Class"));
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000348 RID: 840 RVA: 0x00012A2F File Offset: 0x00010C2F
		[DataSourceProperty]
		public bool IsCaptain
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsCaptain;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000349 RID: 841 RVA: 0x00012A41 File Offset: 0x00010C41
		[DataSourceProperty]
		public bool CaptainIsEnabled
		{
			get
			{
				if (this.IsCaptain)
				{
					WCharacter baseTroop = State.Troop.BaseTroop;
					return baseTroop != null && baseTroop.CaptainEnabled;
				}
				return false;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600034A RID: 842 RVA: 0x00012A64 File Offset: 0x00010C64
		[DataSourceProperty]
		public string CaptainText1
		{
			get
			{
				TextObject textObject = L.T("captain_text_1", "{TROOP} captain.");
				string tag = "TROOP";
				WCharacter troop = State.Troop;
				string text;
				if (troop == null)
				{
					text = null;
				}
				else
				{
					WCharacter baseTroop = troop.BaseTroop;
					text = ((baseTroop != null) ? baseTroop.Name : null);
				}
				return textObject.SetTextVariable(tag, text ?? L.S("troop", "troop")).ToString();
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x0600034B RID: 843 RVA: 0x00012ABF File Offset: 0x00010CBF
		[DataSourceProperty]
		public string CaptainText2
		{
			get
			{
				if (!this.CaptainIsEnabled)
				{
					return L.S("captain_text_2_disabled", "Disabled: no captains will appear for this troop.");
				}
				return L.S("captain_text_2_enabled", "Enabled: out of fifteen troops, one will be a captain.");
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00012AE8 File Offset: 0x00010CE8
		[DataSourceMethod]
		public void ExecuteRename()
		{
			string name2 = State.Troop.Name;
			InformationManager.ShowTextInquiry(new TextInquiryData(L.S("rename_troop", "Rename Troop"), L.S("enter_new_name", "Enter a new name:"), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(string name)
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					return;
				}
				State.Troop.Name = name.Trim();
				State.UpdateTroop(State.Troop);
			}, delegate()
			{
			}, false, null, "", name2), false, false);
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00012B90 File Offset: 0x00010D90
		[DataSourceMethod]
		public void ExecuteRenameSurname()
		{
			WCharacter troop = State.Troop;
			WHero hero = troop as WHero;
			if (hero == null)
			{
				return;
			}
			InformationManager.ShowTextInquiry(new TextInquiryData(L.S("rename_hero_surname", "Change Surname / Title"), L.S("enter_new_surname", "Enter a surname or title (leave blank to remove):"), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(string surname)
			{
				hero.Surname = (((surname != null) ? surname.Trim() : null) ?? string.Empty);
				State.UpdateTroop(State.Troop);
			}, delegate()
			{
			}, false, null, "", hero.Surname), false, false);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00012C48 File Offset: 0x00010E48
		[DataSourceMethod]
		public void ExecuteChangeCulture()
		{
			try
			{
				if (!(State.Troop == null))
				{
					List<InquiryElement> list = new List<InquiryElement>();
					foreach (WCulture wculture in WCulture.All)
					{
						if (wculture != null && wculture.Name != null)
						{
							WCharacter wcharacter = wculture.RootBasic ?? wculture.RootElite;
							if (((wcharacter != null) ? wcharacter.ImageIdentifier : null) != null)
							{
								list.Add(new InquiryElement(wculture.Base, wculture.Name, wcharacter.ImageIdentifier, true, null));
							}
						}
					}
					if (list.Count == 0)
					{
						Notifications.Popup(L.T("no_cultures_title", "No Cultures Found"), L.T("no_cultures_text", "No cultures are loaded in the current game."), null, true);
					}
					else
					{
						MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("change_culture_title", "Change Culture"), null, list, true, 1, 1, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
						{
							if (selected == null || selected.Count == 0)
							{
								return;
							}
							InquiryElement inquiryElement = selected[0];
							object obj = (inquiryElement != null) ? inquiryElement.Identifier : null;
							CultureObject culture = obj as CultureObject;
							if (culture == null)
							{
								return;
							}
							AppearanceGuard.TryApply(State.Troop, State.Equipment.Index, delegate
							{
								if (culture.StringId == State.Troop.Culture.StringId)
								{
									return false;
								}
								State.Troop.Culture = new WCulture(culture);
								float ageMin = State.Troop.Body.AgeMin;
								float ageMax = State.Troop.Body.AgeMax;
								float age = State.Troop.Body.Age;
								BodyHelper.ApplyPropertiesFromCulture(State.Troop, culture);
								State.Troop.Body.AgeMin = ageMin;
								State.Troop.Body.AgeMax = ageMax;
								State.Troop.Body.Age = age;
								return true;
							}, delegate
							{
								State.UpdateTroop(State.Troop);
							}, true);
						}, delegate(List<InquiryElement> _)
						{
						}, "", false), false, false);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x00012DD8 File Offset: 0x00010FD8
		[DataSourceMethod]
		public void ExecuteChangeRace()
		{
			try
			{
				if (!(State.Troop == null) && this.CanChangeRace)
				{
					IReadOnlyList<string> readOnlyList = TroopPanelVM.EnsureRaceNames();
					if (readOnlyList == null || readOnlyList.Count == 0)
					{
						Notifications.Popup(L.T("no_races_title", "No Races Found"), L.T("no_races_text", "No alternative races are available."), null, true);
					}
					else
					{
						List<InquiryElement> list = new List<InquiryElement>(readOnlyList.Count);
						for (int i = 0; i < readOnlyList.Count; i++)
						{
							string title = string.IsNullOrWhiteSpace(readOnlyList[i]) ? string.Format("Race {0}", i) : readOnlyList[i];
							list.Add(new InquiryElement(i, title, null, true, null));
						}
						MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("change_race_title", "Change Race"), L.S("change_race_desc", string.Empty), list, true, 1, 1, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
						{
							if (selected != null && selected.Count > 0)
							{
								InquiryElement inquiryElement = selected[0];
								object obj = (inquiryElement != null) ? inquiryElement.Identifier : null;
								if (obj is int)
								{
									int race = (int)obj;
									AppearanceGuard.TryApply(State.Troop, State.Equipment.Index, delegate
									{
										if (race == State.Troop.Race)
										{
											return false;
										}
										State.Troop.Race = race;
										State.Troop.Body.EnsureOwnBodyRange();
										return true;
									}, delegate
									{
										State.UpdateTroop(State.Troop);
									}, true);
								}
							}
						}, delegate(List<InquiryElement> _)
						{
						}, "", false), false, false);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00012F50 File Offset: 0x00011150
		[DataSourceMethod]
		public void ExecuteChangeFormationClass()
		{
			try
			{
				if (Config.AllowFormationOverrides)
				{
					if (!(State.Troop == null))
					{
						List<FormationClass> list = new List<FormationClass>(5)
						{
							FormationClass.NumberOfAllFormations,
							FormationClass.Infantry,
							FormationClass.Ranged,
							FormationClass.Cavalry,
							FormationClass.HorseArcher
						};
						if (Config.AdditionalFormationOverrides)
						{
							list.Add(FormationClass.LightCavalry);
							list.Add(FormationClass.HeavyCavalry);
							list.Add(FormationClass.HeavyInfantry);
							list.Add(FormationClass.NumberOfDefaultFormations);
							list.Add(FormationClass.Bodyguard);
							list.Add(FormationClass.NumberOfRegularFormations);
						}
						List<InquiryElement> list2 = new List<InquiryElement>(list.Count);
						foreach (FormationClass formationClass in list)
						{
							string title = (formationClass == FormationClass.NumberOfAllFormations) ? L.S("formation_auto", "Auto") : formationClass.GetLocalizedName().ToString();
							list2.Add(new InquiryElement(formationClass, title, null, true, null));
						}
						MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(L.S("change_formation_class_title", "Change Formation Class"), null, list2, true, 1, 1, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(List<InquiryElement> selected)
						{
							if (selected == null || selected.Count == 0)
							{
								return;
							}
							InquiryElement inquiryElement = selected[0];
							object obj = (inquiryElement != null) ? inquiryElement.Identifier : null;
							if (!(obj is FormationClass))
							{
								return;
							}
							FormationClass formationClass2 = (FormationClass)obj;
							if (formationClass2 == State.Troop.FormationClassOverride)
							{
								return;
							}
							State.Troop.FormationClassOverride = formationClass2;
							State.Troop.FormationClass = State.Troop.ComputeFormationClass();
							State.UpdateTroop(State.Troop);
						}, delegate(List<InquiryElement> _)
						{
						}, "", false), false, false);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00013118 File Offset: 0x00011318
		[DataSourceMethod]
		public void ExecuteAddUpgrade()
		{
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_modify", "modify")))
			{
				return;
			}
			if (ClanScreen.IsStudioMode)
			{
				return;
			}
			InformationManager.ShowTextInquiry(new TextInquiryData(L.S("add_upgrade", "Add Upgrade"), L.S("enter_new_troop_name", "Enter the name of the new troop:"), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate(string name)
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					return;
				}
				WCharacter troop = UpgradeManager.AddUpgradeTarget(State.Troop, name);
				State.UpdateFaction(State.Faction);
				State.UpdateTroop(troop);
			}, delegate()
			{
			}, false, null, "", State.Troop.Name), false, false);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000131E4 File Offset: 0x000113E4
		[DataSourceMethod]
		public void ExecuteRankUp()
		{
			if (ClanScreen.IsStudioMode)
			{
				return;
			}
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_modify", "modify")))
			{
				return;
			}
			int num = RetinueManager.RankUpCost(State.Troop);
			if (SkillManager.SkillPointsLeft(State.Troop) > 0)
			{
				Notifications.Popup(L.T("rank_up_not_maxed_out", "Not Maxed Out"), L.T("rank_up_not_maxed_out_text", "Max out this retinue's skills before you can rank up."), null, true);
				return;
			}
			if (State.Troop == State.Faction.RetinueBasic && State.Troop.Tier >= State.Faction.RetinueElite.Tier)
			{
				Notifications.Popup(L.T("rank_up_cant_outrank_elite_title", "Cannot Outrank Elite"), L.T("rank_up_cant_outrank_elite_text", "{TROOP_NAME} can't outrank {ELITE_RETINUE}.").SetTextVariable("TROOP_NAME", State.Troop.Name).SetTextVariable("ELITE_RETINUE", State.Faction.RetinueElite.Name), null, true);
				return;
			}
			if (Player.Gold < num)
			{
				Notifications.Popup(L.T("rank_up_not_enough_gold_title", "Not enough gold"), L.T("rank_up_not_enough_gold_text", "You do not have enough gold to rank up {TROOP_NAME}.\n\nRank up cost: {COST} gold.").SetTextVariable("TROOP_NAME", State.Troop.Name).SetTextVariable("COST", num), null, true);
				return;
			}
			if (TroopXpBehavior.Get(State.Troop) < num && this.TroopXpIsEnabled)
			{
				Notifications.Popup(L.T("rank_up_not_enough_xp_title", "Not enough XP"), L.T("rank_up_not_enough_xp_text", "You do not have enough XP to rank up {TROOP_NAME}.\n\nRank up cost: {COST} XP.").SetTextVariable("TROOP_NAME", State.Troop.Name).SetTextVariable("COST", num), null, true);
				return;
			}
			string variable = this.TroopXpIsEnabled ? L.T("rank_up_costs_text", "It will cost you {COST_GOLD} gold and {COST_XP} XP.").SetTextVariable("COST_GOLD", num).SetTextVariable("COST_XP", num).ToString() : L.T("rank_up_gold_cost_text", "It will cost you {COST} gold.").SetTextVariable("COST", num).ToString();
			InformationManager.ShowInquiry(new InquiryData(L.S("rank_up", "Rank Up"), L.T("increase_troop_tier", "Increase {TROOP_NAME}'s tier?\n\n{text}").SetTextVariable("TROOP_NAME", State.Troop.Name).SetTextVariable("text", variable).ToString(), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
			{
				RetinueManager.RankUp(State.Troop);
				base.OnPropertyChanged("TierText");
				base.OnPropertyChanged("SkillCapText");
				base.OnPropertyChanged("SkillPointsTotal");
				base.OnPropertyChanged("CanRankUp");
				State.UpdateTroop(State.Troop);
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00013478 File Offset: 0x00011678
		[DataSourceMethod]
		public void ExecuteApplyConversions()
		{
			if (!this.HasPendingConversions)
			{
				return;
			}
			if (!ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_convert", "convert")))
			{
				return;
			}
			if (this.PendingTotalGoldCost > Player.Gold)
			{
				Notifications.Popup(L.T("convert_not_enough_gold_title", "Not enough gold"), L.T("convert_not_enough_gold_text", "You do not have enough gold to hire these retinues."), null, true);
				return;
			}
			if (this.PendingTotalInfluenceCost > Player.Influence)
			{
				Notifications.Popup(L.T("convert_not_enough_influence_title", "Not enough influence"), L.T("convert_not_enough_influence_text", "You do not have enough influence to hire these retinues."), null, true);
				return;
			}
			foreach (KeyValuePair<WCharacter, int> keyValuePair in State.ConversionData)
			{
				Log.Info(string.Format("Processing conversion: {0} => {1}", keyValuePair.Key.Name, keyValuePair.Value));
				if (keyValuePair.Value != 0)
				{
					bool flag = keyValuePair.Value > 0;
					WCharacter from = flag ? keyValuePair.Key : State.Troop;
					WCharacter to = flag ? State.Troop : keyValuePair.Key;
					int amountRequested = Math.Abs(keyValuePair.Value);
					RetinueManager.Convert(from, to, amountRequested);
				}
			}
			State.ClearPendingConversions();
			State.UpdatePartyData(null);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000135D0 File Offset: 0x000117D0
		[DataSourceMethod]
		public void ExecuteClearConversions()
		{
			State.ClearPendingConversions();
		}

		// Token: 0x06000355 RID: 853 RVA: 0x000135D7 File Offset: 0x000117D7
		[DataSourceMethod]
		public void ExecuteToggleMariner()
		{
			if (State.Troop == null || !this.ShowMariner)
			{
				return;
			}
			State.Troop.IsMariner = !State.Troop.IsMariner;
			State.UpdateTroop(State.Troop);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00013610 File Offset: 0x00011810
		private static string GetRaceName(int raceIndex)
		{
			if (raceIndex < 0)
			{
				return L.S("unknown", "Unknown");
			}
			IReadOnlyList<string> readOnlyList = TroopPanelVM.EnsureRaceNames();
			if (readOnlyList != null && raceIndex < readOnlyList.Count)
			{
				return readOnlyList[raceIndex] ?? string.Format("Race {0}", raceIndex);
			}
			return string.Format("Race {0}", raceIndex);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00013670 File Offset: 0x00011870
		private static IReadOnlyList<string> EnsureRaceNames()
		{
			if (TroopPanelVM._cachedRaceNames != null)
			{
				return TroopPanelVM._cachedRaceNames;
			}
			string[] array = FaceGen.GetRaceNames() ?? Array.Empty<string>();
			if (array.Length == 0)
			{
				return TroopPanelVM._cachedRaceNames = new List<string>();
			}
			List<string> list = new List<string>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(TroopPanelVM.FormatRaceName(array[i]));
			}
			TroopPanelVM._cachedRaceNames = list;
			return TroopPanelVM._cachedRaceNames;
		}

		// Token: 0x06000358 RID: 856 RVA: 0x000136DC File Offset: 0x000118DC
		private static string FormatRaceName(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
			{
				return null;
			}
			string[] array = (from p in raw.Replace('_', ' ').Trim().Split(new char[]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries)
			select p.ToLowerInvariant()).ToArray<string>();
			if (array.Length == 0)
			{
				return null;
			}
			string text = string.Join(" ", array.Take(2));
			if (text.Length > 15)
			{
				text = text.Substring(0, 15);
				int num = text.LastIndexOf(' ');
				if (num > 0)
				{
					text = text.Substring(0, num);
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = array[0];
			}
			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text);
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0001379B File Offset: 0x0001199B
		[DataSourceProperty]
		public bool ShowExtrasSection
		{
			get
			{
				return this.HasExtraSkills || this.ShowMariner;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x0600035A RID: 858 RVA: 0x000137AD File Offset: 0x000119AD
		[DataSourceProperty]
		public BasicTooltipViewModel MarinerToggleHint
		{
			get
			{
				if (!ClanScreen.IsStudioMode)
				{
					return Tooltip.MakeTooltip(null, L.S("mariner_toggle_hint", "Mariner troops are better suited for naval combat, but earn XP at a slightly reduced rate."));
				}
				return Tooltip.MakeTooltip(null, L.S("mariner_toggle_studio_hint", "Mariner troops are better suited for naval combat."));
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600035B RID: 859 RVA: 0x000137E1 File Offset: 0x000119E1
		[DataSourceProperty]
		public bool HasExtraSkills
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.ExtraSkills.Count > 0;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600035C RID: 860 RVA: 0x000137FB File Offset: 0x000119FB
		[DataSourceProperty]
		public bool ShowMariner
		{
			get
			{
				return ModCompatibility.HasNavalDLC && !this.IsHero;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0001380F File Offset: 0x00011A0F
		[DataSourceProperty]
		public bool IsMariner
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsMariner;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00013821 File Offset: 0x00011A21
		[DataSourceProperty]
		public string MarinerLabel
		{
			get
			{
				return L.S("mariner_label", "Mariner");
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600035F RID: 863 RVA: 0x00013832 File Offset: 0x00011A32
		// (set) Token: 0x06000360 RID: 864 RVA: 0x0001383A File Offset: 0x00011A3A
		[DataSourceProperty]
		public bool ShowExtraSkills
		{
			get
			{
				return this._showExtraSkills;
			}
			set
			{
				if (this._showExtraSkills == value)
				{
					return;
				}
				this._showExtraSkills = value;
				base.OnPropertyChanged("ShowExtraSkills");
				this.BuildSkillRows(true);
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0001385F File Offset: 0x00011A5F
		[DataSourceProperty]
		public string ShowExtraSkillsLabel
		{
			get
			{
				return L.S("show_extra_skills_label", "Show More Skills");
			}
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00013870 File Offset: 0x00011A70
		public void BuildSkillRows(bool force = false)
		{
			if (this.WasHero == this.IsHero && !force)
			{
				return;
			}
			this.WasHero = this.IsHero;
			IEnumerable<SkillObject> source;
			if (this.ShowExtraSkills && this.HasExtraSkills)
			{
				source = State.Troop.ExtraSkills;
			}
			else
			{
				source = State.Troop.TroopSkills;
			}
			foreach (TroopSkillVM troopSkillVM in this.SkillsRow1.Concat(this.SkillsRow2).Concat(this.SkillsRow3).ToList<TroopSkillVM>())
			{
				troopSkillVM.Hide();
			}
			this._skillsRow1.Clear();
			this._skillsRow2.Clear();
			this._skillsRow3.Clear();
			List<TroopSkillVM> source2 = (from s in source
			select new TroopSkillVM(s)).ToList<TroopSkillVM>();
			int num = this.IsHero ? 6 : 4;
			List<TroopSkillVM> list = source2.Take(num).ToList<TroopSkillVM>();
			List<TroopSkillVM> list2 = source2.Skip(num).Take(num).ToList<TroopSkillVM>();
			List<TroopSkillVM> list3 = this.IsHero ? source2.Skip(num * 2).Take(num).ToList<TroopSkillVM>() : new List<TroopSkillVM>();
			foreach (TroopSkillVM troopSkillVM2 in list)
			{
				this._skillsRow1.Add(troopSkillVM2);
				if (base.IsVisible)
				{
					troopSkillVM2.Show();
				}
			}
			foreach (TroopSkillVM troopSkillVM3 in list2)
			{
				this._skillsRow2.Add(troopSkillVM3);
				if (base.IsVisible)
				{
					troopSkillVM3.Show();
				}
			}
			foreach (TroopSkillVM troopSkillVM4 in list3)
			{
				this._skillsRow3.Add(troopSkillVM4);
				if (base.IsVisible)
				{
					troopSkillVM4.Show();
				}
			}
			base.OnPropertyChanged("SkillsRow1");
			base.OnPropertyChanged("SkillsRow2");
			base.OnPropertyChanged("SkillsRow3");
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00013AE0 File Offset: 0x00011CE0
		public override void Show()
		{
			base.Show();
			if (this._needsRebuild)
			{
				this.Build();
			}
			foreach (TroopConversionRowVM troopConversionRowVM in this.ConversionRows)
			{
				troopConversionRowVM.Show();
			}
			foreach (TroopSkillVM troopSkillVM in this.SkillsRow1.Concat(this.SkillsRow2).Concat(this.SkillsRow3))
			{
				troopSkillVM.Show();
			}
			foreach (TroopTraitVM troopTraitVM in this.Traits)
			{
				troopTraitVM.Show();
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00013BC8 File Offset: 0x00011DC8
		public override void Hide()
		{
			foreach (TroopConversionRowVM troopConversionRowVM in this.ConversionRows)
			{
				troopConversionRowVM.Hide();
			}
			foreach (TroopSkillVM troopSkillVM in this.SkillsRow1.Concat(this.SkillsRow2).Concat(this.SkillsRow3))
			{
				troopSkillVM.Hide();
			}
			foreach (TroopTraitVM troopTraitVM in this.Traits)
			{
				troopTraitVM.Hide();
			}
			base.Hide();
		}

		// Token: 0x040000E8 RID: 232
		private bool _needsRebuild = true;

		// Token: 0x040000E9 RID: 233
		private MBBindingList<TroopConversionRowVM> _conversionRows = new MBBindingList<TroopConversionRowVM>();

		// Token: 0x040000EA RID: 234
		private readonly MBBindingList<TroopSkillVM> _skillsRow1 = new MBBindingList<TroopSkillVM>();

		// Token: 0x040000EB RID: 235
		private readonly MBBindingList<TroopSkillVM> _skillsRow2 = new MBBindingList<TroopSkillVM>();

		// Token: 0x040000EC RID: 236
		private readonly MBBindingList<TroopSkillVM> _skillsRow3 = new MBBindingList<TroopSkillVM>();

		// Token: 0x040000ED RID: 237
		private MBBindingList<TroopTraitVM> _traits;

		// Token: 0x040000EE RID: 238
		private static List<string> _cachedRaceNames;

		// Token: 0x040000EF RID: 239
		private bool _showExtraSkills;

		// Token: 0x040000F0 RID: 240
		private bool WasHero;
	}
}
