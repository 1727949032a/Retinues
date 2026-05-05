using System;
using System.Collections.Generic;
using Retinues.Features.Staging;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Safety.Legacy
{
	// Token: 0x0200004E RID: 78
	[SafeClass]
	public abstract class BaseUpgradeBehavior<T> : CampaignBehaviorBase where T : IPendingData
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600016E RID: 366 RVA: 0x0000A978 File Offset: 0x00008B78
		// (set) Token: 0x0600016F RID: 367 RVA: 0x0000A97F File Offset: 0x00008B7F
		public static BaseUpgradeBehavior<T> Instance { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000A987 File Offset: 0x00008B87
		public Dictionary<string, Dictionary<string, T>> Pending
		{
			get
			{
				return this._pending;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000171 RID: 369
		// (set) Token: 0x06000172 RID: 370
		protected abstract string SaveFieldName { get; set; }

		// Token: 0x06000173 RID: 371 RVA: 0x0000A990 File Offset: 0x00008B90
		public override void SyncData(IDataStore data)
		{
			if (!data.IsLoading)
			{
				return;
			}
			BaseUpgradeBehavior<T>.Instance = this;
			if (this._pending == null)
			{
				this._pending = new Dictionary<string, Dictionary<string, T>>();
			}
			Dictionary<string, Dictionary<string, T>> pending = this._pending;
			if (pending != null)
			{
				pending.Clear();
			}
			data.SyncData<Dictionary<string, Dictionary<string, T>>>(this.SaveFieldName, ref this._pending);
			if (this._pending == null)
			{
				this._pending = new Dictionary<string, Dictionary<string, T>>();
			}
			Log.Info(string.Format("Legacy migration: {0} troops with staged jobs.", this._pending.Count));
			Log.Dump(this._pending, LogLevel.Debug);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000AA21 File Offset: 0x00008C21
		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x06000175 RID: 373
		protected abstract void OnGameLoadFinished();

		// Token: 0x06000176 RID: 374 RVA: 0x0000AA3C File Offset: 0x00008C3C
		public static void ReplacePendingKey(string oldTroopId, string newTroopId)
		{
			BaseUpgradeBehavior<T> instance = BaseUpgradeBehavior<T>.Instance;
			if (instance == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(oldTroopId) || string.IsNullOrEmpty(newTroopId))
			{
				return;
			}
			Dictionary<string, T> dictionary;
			if (!instance._pending.TryGetValue(oldTroopId, out dictionary) || dictionary == null)
			{
				return;
			}
			instance._pending.Remove(oldTroopId);
			instance._pending[newTroopId] = dictionary;
			foreach (T t in dictionary.Values)
			{
				t.TroopId = newTroopId;
			}
		}

		// Token: 0x0400008B RID: 139
		public Dictionary<string, Dictionary<string, T>> _pending = new Dictionary<string, Dictionary<string, T>>();
	}
}
