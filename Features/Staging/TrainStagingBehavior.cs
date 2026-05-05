using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Game.Menu;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Retinues.Features.Staging
{
	// Token: 0x020000C3 RID: 195
	[SafeClass]
	public class TrainStagingBehavior : BaseStagingBehavior<PendingTrainData>
	{
		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x000282B9 File Offset: 0x000264B9
		// (set) Token: 0x060007FA RID: 2042 RVA: 0x000282C1 File Offset: 0x000264C1
		protected override string SaveFieldName { get; set; } = "Retinues_Train_Pending";

		// Token: 0x060007FB RID: 2043 RVA: 0x000282CA File Offset: 0x000264CA
		protected override PendingTrainData GetStagedChange(WCharacter troop, string objectKey)
		{
			if (troop == null || string.IsNullOrEmpty(objectKey))
			{
				return null;
			}
			return base.GetPending(troop.StringId, objectKey);
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x000282EC File Offset: 0x000264EC
		protected override List<PendingTrainData> GetStagedChanges(WCharacter troop)
		{
			if (troop == null)
			{
				return new List<PendingTrainData>();
			}
			return base.GetPending(troop.StringId);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x0002830C File Offset: 0x0002650C
		protected override void StageChange(WCharacter troop, object payload)
		{
			if (troop == null)
			{
				return;
			}
			if (payload is TrainStagingBehavior.TrainChange)
			{
				TrainStagingBehavior.TrainChange trainChange = (TrainStagingBehavior.TrainChange)payload;
				if (trainChange.Skill != null)
				{
					if (!Config.TrainingTroopsTakesTime || ClanScreen.IsStudioMode)
					{
						TrainStagingBehavior.ApplyChange(troop.StringId, trainChange.Skill, trainChange.Points);
						return;
					}
					int num = Math.Max(1, Config.TrainingTimeMultiplier);
					float pointsPerHour = 1f / (float)num;
					string stringId = troop.StringId;
					string stringId2 = trainChange.Skill.StringId;
					PendingTrainData pending = base.GetPending(stringId, stringId2);
					if (pending != null)
					{
						pending.Remaining += num * trainChange.Points;
						pending.PointsRemaining += trainChange.Points;
						pending.PointsPerHour = (float)pending.PointsRemaining / (float)Math.Max(1, pending.Remaining);
						if (pending.PointsPerHour <= 0f)
						{
							pending.PointsPerHour = pointsPerHour;
						}
					}
					else
					{
						base.SetPending(stringId, stringId2, new PendingTrainData
						{
							TroopId = stringId,
							Remaining = num * trainChange.Points,
							SkillId = stringId2,
							PointsRemaining = trainChange.Points,
							PointsPerHour = pointsPerHour,
							Carry = 0f
						});
					}
					string text;
					if (BaseStagingBehavior<PendingTrainData>.IsInManagedMenu(out text))
					{
						BaseStagingBehavior<PendingTrainData>.RefreshManagedMenuOrDefault();
					}
					return;
				}
			}
			Log.Warn("TroopTrainBehavior.StageChange called with invalid payload.");
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00028464 File Offset: 0x00026664
		protected override void UnstageChange(WCharacter troop, string objectKey)
		{
			if (troop == null || string.IsNullOrEmpty(objectKey))
			{
				return;
			}
			base.RemovePending(troop.StringId, objectKey);
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00028488 File Offset: 0x00026688
		protected override void UnstageChanges(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			foreach (KeyValuePair<SkillObject, int> keyValuePair in troop.Skills)
			{
				base.RemovePending(troop.StringId, keyValuePair.Key.StringId);
			}
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x000284F8 File Offset: 0x000266F8
		public static PendingTrainData Get(WCharacter troop, SkillObject skill)
		{
			return ((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).GetStagedChange(troop, (skill != null) ? skill.StringId : null);
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00028516 File Offset: 0x00026716
		public static List<PendingTrainData> Get(WCharacter troop)
		{
			return ((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).GetStagedChanges(troop);
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00028528 File Offset: 0x00026728
		public static void Stage(WCharacter troop, SkillObject skill, int points = 1)
		{
			((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).StageChange(troop, new TrainStagingBehavior.TrainChange(skill, points));
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00028546 File Offset: 0x00026746
		public static void Unstage(WCharacter troop, SkillObject skill)
		{
			((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).UnstageChange(troop, (skill != null) ? skill.StringId : null);
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00028564 File Offset: 0x00026764
		public static void Unstage(WCharacter troop)
		{
			((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).UnstageChanges(troop);
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00028578 File Offset: 0x00026778
		public static void ApplyChange(string troopId, SkillObject skill, int delta)
		{
			if (string.IsNullOrEmpty(troopId) || skill == null)
			{
				return;
			}
			WCharacter wcharacter = new WCharacter(troopId);
			if (delta > 0)
			{
				for (int i = 0; i < delta; i++)
				{
					int skill2 = wcharacter.GetSkill(skill);
					wcharacter.SetSkill(skill, skill2 + 1);
				}
				return;
			}
			if (delta < 0)
			{
				PendingTrainData pending = ((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).GetPending(troopId, skill.StringId);
				if (pending != null && pending.PointsRemaining > 0)
				{
					int num = Math.Min(-delta, pending.PointsRemaining);
					pending.PointsRemaining -= num;
					pending.Remaining -= (int)((float)num / Math.Max(1E-06f, pending.PointsPerHour));
					if (pending.PointsRemaining <= 0)
					{
						((TrainStagingBehavior)BaseStagingBehavior<PendingTrainData>.Instance).Pending[troopId].Remove(skill.StringId);
					}
					delta += num;
					if (delta >= 0)
					{
						return;
					}
				}
				for (int j = 0; j < -delta; j++)
				{
					int skill3 = wcharacter.GetSkill(skill);
					if (skill3 <= 0)
					{
						break;
					}
					wcharacter.SetSkill(skill, skill3 - 1);
				}
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x00028684 File Offset: 0x00026884
		protected override string OptionId
		{
			get
			{
				return "ret_train_pending";
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x0002868B File Offset: 0x0002688B
		protected override string OptionText
		{
			get
			{
				return L.S("upgrade_train_pending_btn", "Train troops");
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x0002869C File Offset: 0x0002689C
		protected override string InquiryTitle
		{
			get
			{
				return L.S("upgrade_train_select_troop", "Select troops to train");
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x000286AD File Offset: 0x000268AD
		protected override string InquiryDescription
		{
			get
			{
				return L.S("upgrade_train_choose_troop", "Choose one or more troops to start training now.");
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600080A RID: 2058 RVA: 0x000286BE File Offset: 0x000268BE
		protected override string InquiryAffirmative
		{
			get
			{
				return L.S("upgrade_train_begin", "Begin training");
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x000286CF File Offset: 0x000268CF
		protected override string InquiryNegative
		{
			get
			{
				return L.S("cancel", "Cancel");
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x0600080C RID: 2060 RVA: 0x000286E0 File Offset: 0x000268E0
		protected override string ActionString
		{
			get
			{
				return L.S("action_modify", "modify");
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x000286F1 File Offset: 0x000268F1
		protected override GameMenuOption.LeaveType LeaveType
		{
			get
			{
				return GameMenuOption.LeaveType.Recruit;
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x000286F8 File Offset: 0x000268F8
		protected override string BuildElementTitle(WCharacter troop, PendingTrainData data)
		{
			SkillObject @object = MBObjectManager.Instance.GetObject<SkillObject>(data.SkillId);
			return string.Format("{0}\n+{1} {2} ({3}h)", new object[]
			{
				troop.Name,
				data.PointsRemaining,
				@object.Name,
				data.Remaining
			});
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x00028754 File Offset: 0x00026954
		protected override void FinalModalSummary()
		{
			if (this._batchedActions.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<WCharacter, List<SkillObject>> keyValuePair in this._batchedActions)
			{
				WCharacter key = keyValuePair.Key;
				List<SkillObject> value = keyValuePair.Value;
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (SkillObject skillObject in value)
				{
					if (!dictionary.ContainsKey(skillObject.Name.ToString()))
					{
						dictionary[skillObject.Name.ToString()] = 0;
					}
					Dictionary<string, int> dictionary2 = dictionary;
					string key2 = skillObject.Name.ToString();
					dictionary2[key2]++;
				}
				List<string> list2 = new List<string>();
				foreach (KeyValuePair<string, int> keyValuePair2 in dictionary)
				{
					list2.Add(string.Format("{0} {1}", keyValuePair2.Value, keyValuePair2.Key));
				}
				TextObject textObject = L.T("equip_complete_summary_line", "{TROOP}: {SKILLS}").SetTextVariable("TROOP", key.Name).SetTextVariable("SKILLS", string.Join(", ", list2));
				list.Add(textObject.ToString());
			}
			TextObject description = L.T("train_complete_summary", "The following troops have completed their training:\n\n{SUMMARY}").SetTextVariable("SUMMARY", string.Join("\n", list));
			Notifications.Popup(L.T("train_complete", "Training Complete"), description, null, true);
			this._batchedActions.Clear();
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0002896C File Offset: 0x00026B6C
		protected override void StartWait(CampaignGameStarter starter, string troopId, string objId, PendingTrainData data, Action onAfterCompleted = null)
		{
			WCharacter troop = new WCharacter(troopId);
			SkillObject skill = MBObjectManager.Instance.GetObject<SkillObject>(objId);
			TimedWaitMenu.Start(starter, "train_" + troopId + "_" + objId, L.T("upgrade_train_progress", "Training {NAME}...").SetTextVariable("NAME", troop.Name).ToString(), (float)data.Remaining, delegate
			{
				while (data.PointsRemaining > 0)
				{
					TrainStagingBehavior.ApplyChange(troopId, skill, 1);
					data.PointsRemaining--;
				}
				this.RemovePending(troopId, objId);
				TextObject textObject = L.T("training_complete_text", "{TROOP} has completed their {SKILL} training.").SetTextVariable("TROOP", troop.Name).SetTextVariable("SKILL", skill.Name);
				if (!this._batchActive)
				{
					Notifications.Popup(L.T("training_complete", "Training Complete"), textObject, null, true);
				}
				else
				{
					if (!this._batchedActions.ContainsKey(troop))
					{
						this._batchedActions[troop] = new List<SkillObject>();
					}
					this._batchedActions[troop].Add(skill);
					Log.Message(textObject.ToString());
				}
				Action onAfterCompleted2 = onAfterCompleted;
				if (onAfterCompleted2 == null)
				{
					return;
				}
				onAfterCompleted2();
			}, delegate
			{
				Action onAfterCompleted2 = onAfterCompleted;
				if (onAfterCompleted2 == null)
				{
					return;
				}
				onAfterCompleted2();
			}, GameMenu.MenuOverlayType.SettlementWithBoth, delegate(float _)
			{
				if (data.Remaining > 0 && data.PointsRemaining > 0)
				{
					data.Remaining--;
					data.Carry += data.PointsPerHour;
					int num = Math.Min((int)Math.Floor((double)data.Carry), data.PointsRemaining);
					if (num > 0)
					{
						TrainStagingBehavior.ApplyChange(troopId, skill, num);
						data.PointsRemaining -= num;
						data.Carry -= (float)num;
					}
				}
			});
		}

		// Token: 0x04000215 RID: 533
		private const int BaseTrainingTime = 1;

		// Token: 0x04000217 RID: 535
		private readonly Dictionary<WCharacter, List<SkillObject>> _batchedActions = new Dictionary<WCharacter, List<SkillObject>>();

		// Token: 0x02000192 RID: 402
		public readonly struct TrainChange
		{
			// Token: 0x06000C4F RID: 3151 RVA: 0x000354EE File Offset: 0x000336EE
			public TrainChange(SkillObject skill, int points = 1)
			{
				this.Skill = skill;
				this.Points = Math.Max(1, points);
			}

			// Token: 0x040004CD RID: 1229
			public readonly SkillObject Skill;

			// Token: 0x040004CE RID: 1230
			public readonly int Points;
		}
	}
}
