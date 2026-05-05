using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Wrappers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Retinues.Features.Experience
{
	// Token: 0x020000C7 RID: 199
	[SafeClass]
	public class TroopXpBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000820 RID: 2080 RVA: 0x0002927F File Offset: 0x0002747F
		// (set) Token: 0x06000821 RID: 2081 RVA: 0x00029286 File Offset: 0x00027486
		public static TroopXpBehavior Instance { get; private set; }

		// Token: 0x06000822 RID: 2082 RVA: 0x0002928E File Offset: 0x0002748E
		public TroopXpBehavior()
		{
			TroopXpBehavior.Instance = this;
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x000292A7 File Offset: 0x000274A7
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, int>>("Retinues_Xp_Pools", ref this._xpPools);
			Log.Info(string.Format("{0} troop XP pools.", this._xpPools.Count));
			Log.Dump(this._xpPools, LogLevel.Debug);
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x000292E6 File Offset: 0x000274E6
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnDailyTickParty));
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00029318 File Offset: 0x00027518
		private void OnMissionStarted(IMission mission)
		{
			Log.Debug("Adding TroopXpMissionBehavior.");
			Mission mission2 = mission as Mission;
			if (mission2 == null)
			{
				return;
			}
			Log.Info(string.Format("Mission mode: {0}.", mission2.Mode));
			mission2.AddMissionBehavior(new BattleMissionXpBehavior());
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00029360 File Offset: 0x00027560
		private void OnDailyTickParty(MobileParty mobileParty)
		{
			if (mobileParty == null || !mobileParty.IsMainParty)
			{
				return;
			}
			foreach (WRosterElement wrosterElement in new WParty(mobileParty).MemberRoster.Elements)
			{
				if (wrosterElement.Troop.IsCustom)
				{
					int num = (int)(Campaign.Current.Models.PartyTrainingModel.GetEffectiveDailyExperience(mobileParty, wrosterElement.Base).ResultNumber * (float)wrosterElement.Number * 0.2f);
					if (num > 0)
					{
						Log.Debug(string.Format("Granting training XP for {0}: {1} XP", wrosterElement.Troop.Name, num));
						TroopXpBehavior.Add(wrosterElement.Troop, num, false);
					}
				}
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00029430 File Offset: 0x00027630
		public static int Get(WCharacter troop)
		{
			if (troop == null || TroopXpBehavior.Instance == null)
			{
				return 0;
			}
			return TroopXpBehavior.Instance.GetPool(TroopXpBehavior.PoolKey(troop));
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00029454 File Offset: 0x00027654
		public static void Set(WCharacter troop, int value)
		{
			if (troop == null || TroopXpBehavior.Instance == null || value < 0)
			{
				return;
			}
			TroopXpBehavior.Instance._xpPools[TroopXpBehavior.PoolKey(troop)] = value;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00029484 File Offset: 0x00027684
		public static void Add(WCharacter troop, int delta, bool isRefund = false)
		{
			if (troop == null || TroopXpBehavior.Instance == null || delta == 0)
			{
				return;
			}
			if (troop.IsMariner && delta > 0 && !isRefund)
			{
				delta = (int)((float)delta * 0.8f);
			}
			int pool = TroopXpBehavior.Instance.GetPool(TroopXpBehavior.PoolKey(troop));
			TroopXpBehavior.Instance._xpPools[TroopXpBehavior.PoolKey(troop)] = Math.Max(0, pool + delta);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x000294F0 File Offset: 0x000276F0
		public static void ReplacePoolKey(string oldKey, string newKey)
		{
			if (TroopXpBehavior.Instance == null || string.IsNullOrEmpty(oldKey) || string.IsNullOrEmpty(newKey))
			{
				return;
			}
			int value;
			if (TroopXpBehavior.Instance._xpPools.TryGetValue(oldKey, out value))
			{
				TroopXpBehavior.Instance._xpPools[newKey] = value;
				TroopXpBehavior.Instance._xpPools.Remove(oldKey);
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0002954C File Offset: 0x0002774C
		public static bool TrySpend(WCharacter troop, int amount)
		{
			if (Config.SkillXpCostPerPoint == 0 && Config.BaseSkillXpCost == 0)
			{
				return true;
			}
			if (amount <= 0)
			{
				return true;
			}
			if (troop == null)
			{
				return false;
			}
			if (TroopXpBehavior.Get(troop) < amount)
			{
				return false;
			}
			TroopXpBehavior.Add(troop, -amount, false);
			return true;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00029599 File Offset: 0x00027799
		public static void Refund(WCharacter troop, int amount, bool force = false)
		{
			if (troop == null || amount <= 0)
			{
				return;
			}
			if (!DoctrineAPI.IsDoctrineUnlocked<AdaptiveTraining>() && !Config.ForceXpRefunds && !force)
			{
				return;
			}
			TroopXpBehavior.Add(troop, amount, true);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x000295C8 File Offset: 0x000277C8
		public static void RefundOnePoint(WCharacter troop, int skillValue, bool force = false)
		{
			if (troop == null)
			{
				return;
			}
			int amount = SkillManager.SkillPointXpCost(skillValue - 1);
			TroopXpBehavior.Refund(troop, amount, force);
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x000295F0 File Offset: 0x000277F0
		internal static bool SharedPool
		{
			get
			{
				return Config.SharedXpPool;
			}
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x000295FC File Offset: 0x000277FC
		internal static string PoolKey(WCharacter troop)
		{
			if (troop != null && troop.IsCaptain && troop.BaseTroop != null)
			{
				troop = troop.BaseTroop;
			}
			if (TroopXpBehavior.SharedPool)
			{
				return "_shared";
			}
			if (troop == null)
			{
				return null;
			}
			return troop.StringId;
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00029638 File Offset: 0x00027838
		internal int GetPool(string key)
		{
			int result;
			if (key == null || !this._xpPools.TryGetValue(key, out result))
			{
				return 0;
			}
			return result;
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0002965C File Offset: 0x0002785C
		[CommandLineFunctionality.CommandLineArgumentFunction("troop_xp_add", "retinues")]
		public static string TroopXpAdd(List<string> args)
		{
			if (args.Count == 0 || args.Count > 2)
			{
				return "Usage: retinues.troop_xp_add [id] [amount]";
			}
			WCharacter wcharacter;
			try
			{
				wcharacter = new WCharacter(args[0]);
			}
			catch
			{
				return "Invalid troop ID.";
			}
			int num;
			if (args.Count == 1)
			{
				num = 1000;
			}
			else
			{
				try
				{
					num = int.Parse(args[1]);
				}
				catch
				{
					return "Amount must be an integer.";
				}
			}
			try
			{
				TroopXpBehavior.Add(wcharacter, num, false);
			}
			catch (Exception ex)
			{
				return "Failed to add XP: " + ex.Message;
			}
			return string.Format("Added {0} XP to {1} ({2}).", num, wcharacter.Name, wcharacter);
		}

		// Token: 0x0400021E RID: 542
		public const float MarinerPenaltyMultiplier = 0.8f;

		// Token: 0x0400021F RID: 543
		public const float TrainingXpMultiplier = 0.2f;

		// Token: 0x04000221 RID: 545
		private Dictionary<string, int> _xpPools = new Dictionary<string, int>();
	}
}
