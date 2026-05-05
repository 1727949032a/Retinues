using System;
using System.Collections.Generic;
using Retinues.Features.Staging;
using Retinues.Utils;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000050 RID: 80
	[SafeClass]
	public sealed class TroopTrainBehavior : BaseUpgradeBehavior<PendingTrainData>
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600017C RID: 380 RVA: 0x0000AC87 File Offset: 0x00008E87
		// (set) Token: 0x0600017D RID: 381 RVA: 0x0000AC8F File Offset: 0x00008E8F
		protected override string SaveFieldName { get; set; } = "Retinues_Train_Pending";

		// Token: 0x0600017E RID: 382 RVA: 0x0000AC98 File Offset: 0x00008E98
		protected override void OnGameLoadFinished()
		{
			Log.Info("Legacy migration: TroopTrainBehavior.OnGameLoadFinished called.");
			if (this._pending == null || this._pending.Count == 0)
			{
				Log.Info("Legacy migration: no legacy train jobs to migrate.");
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<string, Dictionary<string, PendingTrainData>> keyValuePair in this._pending)
			{
				string key = keyValuePair.Key;
				foreach (KeyValuePair<string, PendingTrainData> keyValuePair2 in keyValuePair.Value)
				{
					string key2 = keyValuePair2.Key;
					PendingTrainData value = keyValuePair2.Value;
					if (!string.IsNullOrEmpty(key2) && value != null)
					{
						Log.Info("Migrating train job for troop " + key + ", skill " + key2);
						BaseStagingBehavior<PendingTrainData>.Instance.SetPending(key, key2, value);
						num2++;
					}
				}
				num++;
			}
			this._pending.Clear();
			Log.Info(string.Format("Legacy migration: migrated {0} train job(s) for {1} troop(s) ", num2, num) + string.Format("into TrainStagingBehavior (skipped {0} duplicate key(s)).", num3));
		}
	}
}
