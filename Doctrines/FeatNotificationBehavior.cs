using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Retinues.Doctrines
{
	// Token: 0x020000D6 RID: 214
	[SafeClass]
	public sealed class FeatNotificationBehavior : CampaignBehaviorBase
	{
		// Token: 0x060008AC RID: 2220 RVA: 0x0002BE1D File Offset: 0x0002A01D
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0002BE20 File Offset: 0x0002A020
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, delegate(IMission _)
			{
				this._inMission = true;
			});
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, delegate(IMission _)
			{
				this._inMission = false;
				this.TryFlush();
			});
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			DoctrineAPI.AddFeatCompletedListener(new Action<string>(this.OnFeatCompleted));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, delegate(CampaignGameStarter _)
			{
				this.TryFlush();
			});
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0002BE9A File Offset: 0x0002A09A
		private void OnFeatCompleted(string featKey)
		{
			if (string.IsNullOrEmpty(featKey))
			{
				return;
			}
			this._pendingFeatKeys.Enqueue(featKey);
			this.TryFlush();
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0002BEB7 File Offset: 0x0002A0B7
		private void OnHourlyTick()
		{
			this.TryFlush();
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x0002BEC0 File Offset: 0x0002A0C0
		public void TryFlush()
		{
			try
			{
				if (!this._inMission)
				{
					if (Mission.Current == null)
					{
						if (this._pendingFeatKeys.Count != 0)
						{
							if (!InformationManager.IsAnyInquiryActive())
							{
								string featKey = this._pendingFeatKeys.Dequeue();
								this.BuildAndShow(featKey);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0002BF2C File Offset: 0x0002A12C
		private void BuildAndShow(string featKey)
		{
			FeatDefinition item = FeatNotificationBehavior.FindFeat(DoctrineAPI.AllDoctrines(), featKey).Item2;
			object obj = L.T("feat_completed_title", "Feat Completed");
			Sound.Play2D("event:/ui/notification/quest_finished");
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), item.Description.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, delegate()
			{
				this.TryFlush();
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0002BFAC File Offset: 0x0002A1AC
		[return: TupleElementNames(new string[]
		{
			"doc",
			"feat"
		})]
		private static ValueTuple<DoctrineDefinition, FeatDefinition> FindFeat(IReadOnlyList<DoctrineDefinition> all, string featKey)
		{
			Func<FeatDefinition, bool> <>9__0;
			foreach (DoctrineDefinition doctrineDefinition in all)
			{
				List<FeatDefinition> feats = doctrineDefinition.Feats;
				FeatDefinition featDefinition;
				if (feats == null)
				{
					featDefinition = null;
				}
				else
				{
					Func<FeatDefinition, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((FeatDefinition x) => x.Key == featKey));
					}
					featDefinition = feats.FirstOrDefault(predicate);
				}
				FeatDefinition featDefinition2 = featDefinition;
				if (featDefinition2 != null)
				{
					return new ValueTuple<DoctrineDefinition, FeatDefinition>(doctrineDefinition, featDefinition2);
				}
			}
			return new ValueTuple<DoctrineDefinition, FeatDefinition>(null, null);
		}

		// Token: 0x04000253 RID: 595
		private readonly Queue<string> _pendingFeatKeys = new Queue<string>();

		// Token: 0x04000254 RID: 596
		private bool _inMission;
	}
}
