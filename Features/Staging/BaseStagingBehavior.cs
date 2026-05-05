using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Localization;

namespace Retinues.Features.Staging
{
	// Token: 0x020000BF RID: 191
	[SafeClass]
	public abstract class BaseStagingBehavior<T> : CampaignBehaviorBase where T : IPendingData
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00027470 File Offset: 0x00025670
		protected static List<string> MenuIds
		{
			get
			{
				return new List<string>(2)
				{
					"town",
					"castle"
				};
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x0002748E File Offset: 0x0002568E
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x00027495 File Offset: 0x00025695
		public static BaseStagingBehavior<T> Instance { get; private set; }

		// Token: 0x060007A7 RID: 1959 RVA: 0x0002749D File Offset: 0x0002569D
		protected BaseStagingBehavior()
		{
			BaseStagingBehavior<T>.Instance = this;
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x000274B6 File Offset: 0x000256B6
		public override void RegisterEvents()
		{
			Log.Debug("BaseUpgradeBehavior.RegisterEvents called.");
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060007A9 RID: 1961
		protected abstract string OptionId { get; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060007AA RID: 1962
		protected abstract string OptionText { get; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x000274D9 File Offset: 0x000256D9
		protected virtual GameMenuOption.LeaveType OptionIcon
		{
			get
			{
				return GameMenuOption.LeaveType.Wait;
			}
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x000274E0 File Offset: 0x000256E0
		public void OnSessionLaunched(CampaignGameStarter starter)
		{
			Log.Debug("BaseUpgradeBehavior.OnSessionLaunched called.");
			GameMenuOption.OnConsequenceDelegate <>9__0;
			foreach (string text in BaseStagingBehavior<T>.MenuIds)
			{
				CampaignGameStarter starter2 = starter;
				string menuId = text;
				string optionId = this.OptionId;
				string optionText = this.OptionText;
				GameMenuOption.OnConditionDelegate condition = new GameMenuOption.OnConditionDelegate(this.OptionCondition);
				GameMenuOption.OnConsequenceDelegate consequence;
				if ((consequence = <>9__0) == null)
				{
					consequence = (<>9__0 = delegate(MenuCallbackArgs args)
					{
						this.ShowPicker(starter, args);
					});
				}
				starter2.AddGameMenuOption(menuId, optionId, optionText, condition, consequence, false, 0, false, null);
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x00027590 File Offset: 0x00025790
		public Dictionary<string, Dictionary<string, T>> Pending
		{
			get
			{
				return this._pending;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060007AE RID: 1966
		// (set) Token: 0x060007AF RID: 1967
		protected abstract string SaveFieldName { get; set; }

		// Token: 0x060007B0 RID: 1968 RVA: 0x00027598 File Offset: 0x00025798
		public override void SyncData(IDataStore data)
		{
			if (this._pending == null)
			{
				this._pending = new Dictionary<string, Dictionary<string, T>>();
			}
			if (data.IsLoading)
			{
				Dictionary<string, Dictionary<string, T>> pending = this._pending;
				if (pending != null)
				{
					pending.Clear();
				}
			}
			data.SyncData<Dictionary<string, Dictionary<string, T>>>(this.SaveFieldName, ref this._pending);
			if (this._pending == null)
			{
				this._pending = new Dictionary<string, Dictionary<string, T>>();
			}
			Log.Info(string.Format("{0} troops with staged jobs.", this._pending.Count));
			Log.Dump(this._pending, LogLevel.Debug);
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060007B1 RID: 1969
		protected abstract string InquiryTitle { get; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060007B2 RID: 1970
		protected abstract string InquiryDescription { get; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060007B3 RID: 1971
		protected abstract string InquiryAffirmative { get; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060007B4 RID: 1972
		protected abstract string InquiryNegative { get; }

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x060007B5 RID: 1973
		protected abstract string ActionString { get; }

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x060007B6 RID: 1974
		protected abstract GameMenuOption.LeaveType LeaveType { get; }

		// Token: 0x060007B7 RID: 1975
		protected abstract void StartWait(CampaignGameStarter starter, string troopId, string objId, T data, Action onAfterCompleted = null);

		// Token: 0x060007B8 RID: 1976
		protected abstract string BuildElementTitle(WCharacter troop, T data);

		// Token: 0x060007B9 RID: 1977
		protected abstract void FinalModalSummary();

		// Token: 0x060007BA RID: 1978
		protected abstract T GetStagedChange(WCharacter troop, string objectKey);

		// Token: 0x060007BB RID: 1979
		protected abstract List<T> GetStagedChanges(WCharacter troop);

		// Token: 0x060007BC RID: 1980
		protected abstract void StageChange(WCharacter troop, object payload);

		// Token: 0x060007BD RID: 1981
		protected abstract void UnstageChange(WCharacter troop, string objectKey);

		// Token: 0x060007BE RID: 1982
		protected abstract void UnstageChanges(WCharacter troop);

		// Token: 0x060007BF RID: 1983 RVA: 0x00027624 File Offset: 0x00025824
		protected virtual void ShowPicker(CampaignGameStarter starter, MenuCallbackArgs args)
		{
			List<InquiryElement> inquiryElements = this.GetInquiryElements();
			if (inquiryElements.Count == 0)
			{
				BaseStagingBehavior<T>.RefreshManagedMenuOrDefault();
				return;
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(this.InquiryTitle, this.InquiryDescription, inquiryElements, true, 1, inquiryElements.Count, this.InquiryAffirmative, this.InquiryNegative, delegate(List<InquiryElement> selected)
			{
				Queue<ValueTuple<string, string>> queue = new Queue<ValueTuple<string, string>>();
				foreach (InquiryElement inquiryElement in selected)
				{
					ValueTuple<string, string> valueTuple = this.ParseIdentifier((string)inquiryElement.Identifier);
					string item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					queue.Enqueue(new ValueTuple<string, string>(item, item2));
				}
				this._batchActive = (queue.Count > 1);
				this.RunBatch(starter, queue);
			}, delegate(List<InquiryElement> _)
			{
			}, "", false), false, false);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x000276B8 File Offset: 0x000258B8
		private void RunBatch(CampaignGameStarter starter, [TupleElementNames(new string[]
		{
			"troopId",
			"objId"
		})] Queue<ValueTuple<string, string>> queue)
		{
			if (queue == null || queue.Count == 0)
			{
				if (this._batchActive)
				{
					this._batchActive = false;
					this.FinalModalSummary();
				}
				if (this.Pending.Count == 0)
				{
					BaseStagingBehavior<T>.RefreshManagedMenuOrDefault();
				}
				return;
			}
			ValueTuple<string, string> valueTuple = queue.Dequeue();
			string item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			T pending = this.GetPending(item, item2);
			if (pending == null)
			{
				this.RunBatch(starter, queue);
				return;
			}
			this.StartWait(starter, item, item2, pending, delegate
			{
				this.RunBatch(starter, queue);
			});
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00027774 File Offset: 0x00025974
		protected virtual List<InquiryElement> GetInquiryElements()
		{
			List<InquiryElement> list = new List<InquiryElement>();
			foreach (KeyValuePair<string, Dictionary<string, T>> keyValuePair in this._pending)
			{
				string key = keyValuePair.Key;
				foreach (KeyValuePair<string, T> keyValuePair2 in keyValuePair.Value)
				{
					string key2 = keyValuePair2.Key;
					T value = keyValuePair2.Value;
					WCharacter troop = new WCharacter(value.TroopId);
					bool flag = this.IsEntryEligible(troop, value);
					string text;
					if (!flag)
					{
						TextObject contextReason = ContextManager.GetContextReason(troop, BaseStagingBehavior<T>.Instance.ActionString);
						text = ((contextReason != null) ? contextReason.ToString() : null);
					}
					else
					{
						text = null;
					}
					string hint = text;
					list.Add(new InquiryElement(this.ComposeIdentifier(key, key2), this.BuildElementTitle(troop, value), this.BuildElementImage(troop, key2, value), flag, hint));
				}
			}
			return list;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x000278A0 File Offset: 0x00025AA0
		public T GetPending(string troopId, string objId)
		{
			Dictionary<string, T> dictionary;
			T result;
			if (this._pending.TryGetValue(troopId, out dictionary) && dictionary.TryGetValue(objId, out result))
			{
				return result;
			}
			return default(T);
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x000278D4 File Offset: 0x00025AD4
		public List<T> GetPending(string troopId)
		{
			Dictionary<string, T> dictionary;
			if (this._pending.TryGetValue(troopId, out dictionary))
			{
				return dictionary.Values.ToList<T>();
			}
			return new List<T>();
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00027904 File Offset: 0x00025B04
		public void SetPending(string troopId, string objId, T data)
		{
			Log.Debug("BaseStagingBehavior.SetPending: " + troopId + ", " + objId);
			if (!this._pending.ContainsKey(troopId))
			{
				this._pending[troopId] = new Dictionary<string, T>();
			}
			this._pending[troopId][objId] = data;
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0002795C File Offset: 0x00025B5C
		protected void RemovePending(string troopId, string objId)
		{
			Dictionary<string, T> dictionary;
			if (this.Pending.TryGetValue(troopId, out dictionary))
			{
				dictionary.Remove(objId);
				if (dictionary.Count == 0)
				{
					this.Pending.Remove(troopId);
				}
			}
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x00027996 File Offset: 0x00025B96
		protected bool IsEntryEligible(WCharacter troop, T data)
		{
			return troop.IsValid && BaseStagingBehavior<T>.CanEdit(troop) && data.Remaining > 0;
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x000279BA File Offset: 0x00025BBA
		protected ImageIdentifier BuildElementImage(WCharacter troop, string objId, T data)
		{
			return troop.ImageIdentifier;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x000279C2 File Offset: 0x00025BC2
		protected string ComposeIdentifier(string troopId, string objId)
		{
			return troopId + "::" + objId;
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x000279D0 File Offset: 0x00025BD0
		[return: TupleElementNames(new string[]
		{
			"troopId",
			"objId"
		})]
		protected ValueTuple<string, string> ParseIdentifier(string id)
		{
			string[] array = (id != null) ? id.Split(new string[]
			{
				"::"
			}, StringSplitOptions.None) : null;
			if (array == null || array.Length <= 1)
			{
				return new ValueTuple<string, string>((array != null) ? array[0] : null, null);
			}
			return new ValueTuple<string, string>(array[0], array[1]);
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00027A1D File Offset: 0x00025C1D
		protected bool OptionCondition(MenuCallbackArgs args)
		{
			args.optionLeaveType = this.LeaveType;
			return Settlement.CurrentSettlement != null && this._pending.Count > 0;
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00027A42 File Offset: 0x00025C42
		protected static bool IsInManagedMenu(out string currentId)
		{
			Campaign campaign = Campaign.Current;
			string text;
			if (campaign == null)
			{
				text = null;
			}
			else
			{
				MenuContext currentMenuContext = campaign.CurrentMenuContext;
				if (currentMenuContext == null)
				{
					text = null;
				}
				else
				{
					GameMenu gameMenu = currentMenuContext.GameMenu;
					text = ((gameMenu != null) ? gameMenu.StringId : null);
				}
			}
			currentId = text;
			return currentId != null && BaseStagingBehavior<T>.MenuIds.Contains(currentId);
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00027A84 File Offset: 0x00025C84
		protected static void RefreshManagedMenuOrDefault()
		{
			string menuId;
			if (BaseStagingBehavior<T>.IsInManagedMenu(out menuId))
			{
				GameMenu.SwitchToMenu(menuId);
				return;
			}
			Log.Debug("BaseStagingBehavior.RefreshManagedMenuOrDefault: current menu is not managed; skipping refresh.");
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00027AAB File Offset: 0x00025CAB
		protected static bool CanEdit(WCharacter troop)
		{
			return !Config.RestrictEditingToFiefs || ContextManager.IsAllowedInContext(troop, BaseStagingBehavior<T>.Instance.ActionString);
		}

		// Token: 0x04000204 RID: 516
		public Dictionary<string, Dictionary<string, T>> _pending = new Dictionary<string, Dictionary<string, T>>();

		// Token: 0x04000205 RID: 517
		protected bool _batchActive;
	}
}
