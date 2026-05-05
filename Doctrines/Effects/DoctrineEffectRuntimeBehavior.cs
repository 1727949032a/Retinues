using System;
using Retinues.Doctrines.Effects.Behaviors;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Retinues.Doctrines.Effects
{
	// Token: 0x020000DA RID: 218
	[SafeClass]
	public sealed class DoctrineEffectRuntimeBehavior : CampaignBehaviorBase
	{
		// Token: 0x060008EA RID: 2282 RVA: 0x0002C983 File Offset: 0x0002AB83
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x0002C985 File Offset: 0x0002AB85
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0002C9A0 File Offset: 0x0002ABA0
		private void OnMissionStarted(IMission iMission)
		{
			try
			{
				Mission mission = iMission as Mission;
				if (mission != null)
				{
					if (mission.GetMissionBehavior<ImmortalsBehavior>() == null)
					{
						mission.AddMissionBehavior(new ImmortalsBehavior());
					}
					if (mission.GetMissionBehavior<IndomitableBehavior>() == null)
					{
						mission.AddMissionBehavior(new IndomitableBehavior());
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
