using System;
using System.Collections.Generic;
using Retinues.Features.Staging;
using Retinues.Utils;

namespace Retinues.Safety.Legacy
{
	// Token: 0x0200004F RID: 79
	[SafeClass]
	public sealed class TroopEquipBehavior : BaseUpgradeBehavior<PendingEquipData>
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000AAEF File Offset: 0x00008CEF
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000AAF7 File Offset: 0x00008CF7
		protected override string SaveFieldName { get; set; } = "Retinues_Equip_Pending";

		// Token: 0x0600017A RID: 378 RVA: 0x0000AB00 File Offset: 0x00008D00
		protected override void OnGameLoadFinished()
		{
			Log.Info("Legacy migration: TroopEquipBehavior.OnGameLoadFinished called.");
			if (this._pending == null || this._pending.Count == 0)
			{
				Log.Info("Legacy migration: no legacy equip jobs to migrate.");
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<string, Dictionary<string, PendingEquipData>> keyValuePair in this._pending)
			{
				string key = keyValuePair.Key;
				Dictionary<string, PendingEquipData> value = keyValuePair.Value;
				if (!string.IsNullOrEmpty(key) && value != null && value.Count != 0)
				{
					foreach (KeyValuePair<string, PendingEquipData> keyValuePair2 in value)
					{
						string key2 = keyValuePair2.Key;
						PendingEquipData value2 = keyValuePair2.Value;
						if (!string.IsNullOrEmpty(key2) && value2 != null)
						{
							Log.Info("Migrating equip job for troop " + key + ", object " + key2);
							BaseStagingBehavior<PendingEquipData>.Instance.SetPending(key, key2, value2);
							num2++;
						}
					}
					num++;
				}
			}
			this._pending.Clear();
			Log.Info(string.Format("Legacy migration: migrated {0} equip job(s) for {1} troop(s) ", num2, num) + string.Format("into EquipStagingBehavior (skipped {0} duplicate key(s)).", num3));
		}
	}
}
