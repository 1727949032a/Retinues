using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Staging;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Troop.Panel
{
	// Token: 0x0200007D RID: 125
	[SafeClass]
	public sealed class TroopSkillVM : BaseVM
	{
		// Token: 0x06000366 RID: 870 RVA: 0x00013CED File Offset: 0x00011EED
		public TroopSkillVM(SkillObject skill)
		{
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000367 RID: 871 RVA: 0x00013CFC File Offset: 0x00011EFC
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"Value",
					"ValueColor",
					"IsStaged",
					"CanIncrement",
					"CanDecrement",
					"IncrementHint",
					"DecrementHint",
					"IsSmall"
				};
				dictionary[UIEvent.Train] = new string[]
				{
					"Value",
					"ValueColor",
					"IsStaged",
					"CanIncrement",
					"CanDecrement",
					"IncrementHint",
					"DecrementHint"
				};
				dictionary[UIEvent.Equip] = new string[]
				{
					"CanIncrement",
					"CanDecrement",
					"IncrementHint",
					"DecrementHint"
				};
				dictionary[UIEvent.Equipment] = new string[]
				{
					"CanIncrement",
					"CanDecrement",
					"IncrementHint",
					"DecrementHint"
				};
				return dictionary;
			}
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00013DFC File Offset: 0x00011FFC
		protected override void OnTroopChange()
		{
			TroopSkillVM.PlayerWarnedAboutRetraining = false;
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000369 RID: 873 RVA: 0x00013E04 File Offset: 0x00012004
		private SkillData? SkillInfo
		{
			get
			{
				SkillData value;
				if (!State.SkillData.TryGetValue(this.Skill, out value))
				{
					return null;
				}
				return new SkillData?(value);
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600036A RID: 874 RVA: 0x00013E35 File Offset: 0x00012035
		private TextObject CantIncrementReason
		{
			get
			{
				return SkillManager.GetIncrementSkillReason(State.Troop, this.Skill);
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600036B RID: 875 RVA: 0x00013E47 File Offset: 0x00012047
		private TextObject CantDecrementReason
		{
			get
			{
				return SkillManager.GetDecrementSkillReason(State.Troop, this.Skill);
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00013E5C File Offset: 0x0001205C
		[DataSourceProperty]
		public int Value
		{
			get
			{
				SkillData? skillInfo = this.SkillInfo;
				int num = (skillInfo != null) ? skillInfo.GetValueOrDefault().Value : 0;
				skillInfo = this.SkillInfo;
				int? num2;
				if (skillInfo == null)
				{
					num2 = null;
				}
				else
				{
					PendingTrainData train = skillInfo.GetValueOrDefault().Train;
					num2 = ((train != null) ? new int?(train.PointsRemaining) : null);
				}
				int? num3 = num2;
				return num + num3.GetValueOrDefault();
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x0600036D RID: 877 RVA: 0x00013ED1 File Offset: 0x000120D1
		[DataSourceProperty]
		public string ValueColor
		{
			get
			{
				if (!this.IsStaged)
				{
					return "#F4E1C4FF";
				}
				return "#ebaf2fff";
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00013EE8 File Offset: 0x000120E8
		[DataSourceProperty]
		public bool IsStaged
		{
			get
			{
				if (this.SkillInfo == null)
				{
					return false;
				}
				SkillData? skillData;
				PendingTrainData train = skillData.GetValueOrDefault().Train;
				int? num = (train != null) ? new int?(train.PointsRemaining) : null;
				int num2 = 0;
				return num.GetValueOrDefault() > num2 & num != null;
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600036F RID: 879 RVA: 0x00013F41 File Offset: 0x00012141
		[DataSourceProperty]
		public string SkillId
		{
			get
			{
				SkillObject skill = this.Skill;
				return ((skill != null) ? skill.StringId : null) ?? string.Empty;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00013F5E File Offset: 0x0001215E
		[DataSourceProperty]
		public bool CanIncrement
		{
			get
			{
				return this.CantIncrementReason == null;
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000371 RID: 881 RVA: 0x00013F6C File Offset: 0x0001216C
		[DataSourceProperty]
		public bool CanDecrement
		{
			get
			{
				return this.CantDecrementReason == null;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00013F7A File Offset: 0x0001217A
		[DataSourceProperty]
		public bool IsSmall
		{
			get
			{
				WCharacter troop = State.Troop;
				return troop != null && troop.IsHero;
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00013F8C File Offset: 0x0001218C
		[DataSourceProperty]
		public BasicTooltipViewModel IncrementHint
		{
			get
			{
				if (!this.CanIncrement)
				{
					return Tooltip.MakeTooltip(null, this.CantIncrementReason.ToString());
				}
				return null;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00013FA9 File Offset: 0x000121A9
		[DataSourceProperty]
		public BasicTooltipViewModel DecrementHint
		{
			get
			{
				if (!this.CanDecrement)
				{
					return Tooltip.MakeTooltip(null, this.CantDecrementReason.ToString());
				}
				return null;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000375 RID: 885 RVA: 0x00013FC6 File Offset: 0x000121C6
		[DataSourceProperty]
		public BasicTooltipViewModel SkillHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, this.Skill.Name.ToString());
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00013FDE File Offset: 0x000121DE
		[DataSourceMethod]
		public void ExecuteIncrement()
		{
			this.Modify(true);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00013FE7 File Offset: 0x000121E7
		[DataSourceMethod]
		public void ExecuteDecrement()
		{
			this.Modify(false);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00013FF0 File Offset: 0x000121F0
		private void Modify(bool increment)
		{
			TroopSkillVM.<>c__DisplayClass34_0 CS$<>8__locals1 = new TroopSkillVM.<>c__DisplayClass34_0();
			CS$<>8__locals1.increment = increment;
			CS$<>8__locals1.<>4__this = this;
			if (!Config.TrainingTroopsTakesTime && !ContextManager.IsAllowedInContextWithPopup(State.Troop, L.S("action_modify", "modify")))
			{
				return;
			}
			bool flag = ClanScreen.IsStudioMode || (Config.BaseSkillXpCost == 0 && Config.SkillXpCostPerPoint == 0);
			bool flag2 = ClanScreen.IsStudioMode || !Config.TrainingTroopsTakesTime;
			CS$<>8__locals1.capped = (!flag || !flag2);
			if (!ClanScreen.IsStudioMode && !DoctrineAPI.IsDoctrineUnlocked<AdaptiveTraining>() && !Config.ForceXpRefunds && !CS$<>8__locals1.increment && !this.IsStaged && !TroopSkillVM.PlayerWarnedAboutRetraining && Config.BaseSkillXpCost + Config.SkillXpCostPerPoint > 0)
			{
				InformationManager.ShowInquiry(new InquiryData(L.S("warning", "Warning"), L.S("lower_skill_no_refund", "Lowering this troop's skill will not refund any experience points. Continue anyway?"), true, true, L.S("continue", "Continue"), L.S("cancel", "Cancel"), delegate()
				{
					base.<Modify>g__DoModify|0();
					TroopSkillVM.PlayerWarnedAboutRetraining = true;
				}, delegate()
				{
				}, "", 0f, null, null, null), false, false);
				return;
			}
			CS$<>8__locals1.<Modify>g__DoModify|0();
		}

		// Token: 0x040000F1 RID: 241
		private readonly SkillObject Skill = skill;

		// Token: 0x040000F2 RID: 242
		private static bool PlayerWarnedAboutRetraining = ClanScreen.IsStudioMode;
	}
}
