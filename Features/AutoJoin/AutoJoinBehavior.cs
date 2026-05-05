using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace Retinues.Features.AutoJoin
{
	// Token: 0x020000C9 RID: 201
	[SafeClass]
	public class AutoJoinBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x0002990A File Offset: 0x00027B0A
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x00029911 File Offset: 0x00027B11
		public static AutoJoinBehavior Instance { get; private set; }

		// Token: 0x0600083D RID: 2109 RVA: 0x00029919 File Offset: 0x00027B19
		public AutoJoinBehavior()
		{
			AutoJoinBehavior.Instance = this;
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00029934 File Offset: 0x00027B34
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<string, int>>("Retinues_RetinueHire_Caps", ref this._caps);
			dataStore.SyncData<float>("Retinues_RetinueHire_RenownReserve", ref this._renownReserve);
			dataStore.SyncData<float>("Retinues_RetinueHire_LastRenown", ref this._lastRenown);
			if (dataStore.IsLoading && this._lastRenown == 0f)
			{
				this._lastRenown = Player.Renown;
			}
			if (this._caps == null)
			{
				this._caps = new Dictionary<string, int>();
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x000299AA File Offset: 0x00027BAA
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnDailyTickParty));
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x000299C4 File Offset: 0x00027BC4
		private void OnDailyTickParty(MobileParty party)
		{
			if (party == null || !party.IsMainParty)
			{
				return;
			}
			List<WCharacter> hireableRetinues = AutoJoinBehavior.GetHireableRetinues();
			Log.Info(string.Format("[RetinueHire] Hireable: {0}, Renown: {1}, Reserve: {2}", hireableRetinues.Count, Player.Renown, this._renownReserve));
			if (hireableRetinues.Count == 0)
			{
				this._renownReserve = 0f;
				return;
			}
			float num = Player.Renown - this._lastRenown;
			this._lastRenown = Player.Renown;
			if (num > 0f)
			{
				this._renownReserve += num;
			}
			WParty wparty = new WParty(party);
			int num2 = Math.Max(0, wparty.PartySizeLimit - wparty.MemberRoster.Count);
			if (num2 <= 0)
			{
				return;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (WCharacter wcharacter in hireableRetinues)
			{
				dictionary[wcharacter.StringId] = wparty.MemberRoster.CountOf(wcharacter);
			}
			int num3 = 1000;
			while (num2 > 0 && num3-- > 0)
			{
				List<WCharacter> list = hireableRetinues.Where(delegate(WCharacter r)
				{
					if (AutoJoinBehavior.GetCountOf(r) >= AutoJoinBehavior.GetJoinCap(r))
					{
						return false;
					}
					int num9 = RetinueManager.RenownRequiredPerUnit(r);
					return num9 > 0 && (float)num9 <= this._renownReserve;
				}).ToList<WCharacter>();
				if (list.Count == 0)
				{
					break;
				}
				WCharacter wcharacter2 = list[AutoJoinBehavior.rng.Next(list.Count)];
				int num4 = RetinueManager.RenownRequiredPerUnit(wcharacter2);
				this._renownReserve -= (float)num4;
				wparty.MemberRoster.AddTroop(wcharacter2, 1, 0, 0, -1);
				num2--;
			}
			Dictionary<WCharacter, int> dictionary2 = new Dictionary<WCharacter, int>();
			foreach (WCharacter wcharacter3 in hireableRetinues)
			{
				int num5 = wparty.MemberRoster.CountOf(wcharacter3);
				int num7;
				int num6 = dictionary.TryGetValue(wcharacter3.StringId, out num7) ? num7 : 0;
				int num8 = num5 - num6;
				if (num8 > 0)
				{
					dictionary2[wcharacter3] = num8;
				}
			}
			if (dictionary2.Count > 0)
			{
				AutoJoinBehavior.ShowUnlockMessage(dictionary2);
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00029BE4 File Offset: 0x00027DE4
		public static int GetJoinCap(WCharacter troop)
		{
			if (troop == null || !troop.IsRetinue)
			{
				return 0;
			}
			int result;
			if (AutoJoinBehavior.Instance._caps.TryGetValue(troop.StringId, out result))
			{
				return result;
			}
			return 0;
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x00029C20 File Offset: 0x00027E20
		public static void SetJoinCap(WCharacter troop, int cap)
		{
			if (troop == null || !troop.IsRetinue)
			{
				return;
			}
			if (cap <= 0)
			{
				AutoJoinBehavior.Instance._caps.Remove(troop.StringId);
				return;
			}
			AutoJoinBehavior.Instance._caps[troop.StringId] = cap;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00029C70 File Offset: 0x00027E70
		private static int GetCountOf(WCharacter troop)
		{
			if (troop == null || !troop.IsRetinue)
			{
				return 0;
			}
			return new WParty(MobileParty.MainParty).MemberRoster.CountOf(troop);
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00029C9C File Offset: 0x00027E9C
		private static List<WCharacter> GetHireableRetinues()
		{
			List<WCharacter> list = new List<WCharacter>(Player.Clan.RetinueTroops);
			if (Player.Kingdom != null)
			{
				list.AddRange(Player.Kingdom.RetinueTroops);
			}
			return (from r in (from r in list
			where r != null
			group r by r.StringId into g
			select g.First<WCharacter>()).ToList<WCharacter>()
			where AutoJoinBehavior.GetJoinCap(r) > AutoJoinBehavior.GetCountOf(r)
			select r).ToList<WCharacter>();
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00029D74 File Offset: 0x00027F74
		private static void ShowUnlockMessage(Dictionary<WCharacter, int> hires)
		{
			string variable = string.Join(", ", from h in hires
			where h.Key != null
			select string.Format("{0} {1}", h.Value, h.Key.Name));
			Notifications.Log(L.T("retinue_hire_inquiry_body", "The following retinues have joined your party: {JOINED}.").SetTextVariable("JOINED", variable), "#c7f5caff");
		}

		// Token: 0x04000225 RID: 549
		private static readonly Random rng = new Random();

		// Token: 0x04000226 RID: 550
		private Dictionary<string, int> _caps = new Dictionary<string, int>();

		// Token: 0x04000227 RID: 551
		private float _renownReserve;

		// Token: 0x04000228 RID: 552
		private float _lastRenown;
	}
}
