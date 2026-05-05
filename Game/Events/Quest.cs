using System;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Game.Events
{
	// Token: 0x020000AA RID: 170
	[SafeClass]
	public class Quest
	{
		// Token: 0x0600073F RID: 1855 RVA: 0x00024DA8 File Offset: 0x00022FA8
		public Quest(QuestBase quest, bool isSuccessful)
		{
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x00024DBE File Offset: 0x00022FBE
		public string StringId
		{
			get
			{
				QuestBase quest = this._quest;
				if (quest == null)
				{
					return null;
				}
				return quest.StringId;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x00024DD1 File Offset: 0x00022FD1
		public WHero Giver
		{
			get
			{
				QuestBase quest = this._quest;
				if (((quest != null) ? quest.QuestGiver : null) == null)
				{
					return null;
				}
				return new WHero(this._quest.QuestGiver);
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x00024DF9 File Offset: 0x00022FF9
		public bool NoPayment
		{
			get
			{
				QuestBase quest = this._quest;
				return quest != null && quest.RewardGold == 0;
			}
		}

		// Token: 0x040001D8 RID: 472
		private readonly QuestBase _quest = quest;

		// Token: 0x040001D9 RID: 473
		public bool IsSuccessful = isSuccessful;
	}
}
