using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Retinues.Features.Unlocks
{
	// Token: 0x020000B0 RID: 176
	[SafeClass]
	public class UnlocksBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00025616 File Offset: 0x00023816
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x0002561D File Offset: 0x0002381D
		public static UnlocksBehavior Instance { get; private set; }

		// Token: 0x06000756 RID: 1878 RVA: 0x00025625 File Offset: 0x00023825
		public UnlocksBehavior()
		{
			UnlocksBehavior.Instance = this;
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000757 RID: 1879 RVA: 0x0002564C File Offset: 0x0002384C
		public Dictionary<string, int> ProgressByItemId
		{
			get
			{
				Dictionary<string, int> result;
				if ((result = this._progressByItemId) == null)
				{
					result = (this._progressByItemId = new Dictionary<string, int>());
				}
				return result;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x00025674 File Offset: 0x00023874
		public List<string> UnlockedItemIds
		{
			get
			{
				List<string> result;
				if ((result = this._unlockedItemIds) == null)
				{
					result = (this._unlockedItemIds = new List<string>());
				}
				return result;
			}
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0002569C File Offset: 0x0002389C
		public override void SyncData(IDataStore ds)
		{
			ds.SyncData<List<string>>("Retinues_Unlocks_Unlocked", ref this._unlockedItemIds);
			ds.SyncData<Dictionary<string, int>>("Retinues_Unlocks_Progress", ref this._progressByItemId);
			Log.Info(string.Format("{0} item defeat counts, {1} unlocked.", this.ProgressByItemId.Count, this.UnlockedItemIds.Count));
			Log.Dump(this._unlockedItemIds, LogLevel.Debug);
			Log.Dump(this._progressByItemId, LogLevel.Debug);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00025714 File Offset: 0x00023914
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnItemsDiscardedByPlayerEvent.AddNonSerializedListener(this, new Action<ItemRoster>(this.OnItemsDiscardedByPlayer));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.OnTick));
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00025780 File Offset: 0x00023980
		private void OnMissionStarted(IMission mission)
		{
			if (!Config.UnlockItemsFromKills || Config.AllEquipmentUnlocked)
			{
				return;
			}
			Log.Debug("Adding UnlocksMissionBehavior.");
			Mission mission2 = mission as Mission;
			if (mission2 != null)
			{
				mission2.AddMissionBehavior(new UnlocksMissionBehavior());
			}
			this._newlyUnlocked.Clear();
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x000257D4 File Offset: 0x000239D4
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (!Config.UnlockItemsFromKills || Config.AllEquipmentUnlocked)
			{
				return;
			}
			if (this._newlyUnlocked.Count == 0)
			{
				return;
			}
			if (mapEvent == null || !mapEvent.IsPlayerMapEvent)
			{
				return;
			}
			this.QueueUnlockNotification(this._newlyUnlocked);
			this._newlyUnlocked.Clear();
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00025834 File Offset: 0x00023A34
		private void OnItemsDiscardedByPlayer(ItemRoster roster)
		{
			if (!Config.UnlockItemsFromDiscards || Config.AllEquipmentUnlocked)
			{
				return;
			}
			if (roster == null || roster.Count == 0)
			{
				return;
			}
			Dictionary<ItemObject, int> dictionary = new Dictionary<ItemObject, int>(roster.Count);
			for (int i = 0; i < roster.Count; i++)
			{
				ItemRosterElement elementCopyAtIndex = roster.GetElementCopyAtIndex(i);
				ItemObject item = elementCopyAtIndex.EquipmentElement.Item;
				int amount = elementCopyAtIndex.Amount;
				if (new WItem(item).Slots.Count != 0 && item != null && amount > 0)
				{
					float num = (float)Config.RequiredKillsPerItem / (float)Config.RequiredDiscardsPerItem;
					int num2 = amount * (int)Math.Ceiling((double)num);
					int num3;
					if (dictionary.TryGetValue(item, out num3))
					{
						dictionary[item] = num3 + num2;
					}
					else
					{
						dictionary[item] = num2;
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return;
			}
			Log.Debug(string.Format("OnItemsDiscardedByPlayer: {0} stacks will contribute to unlock progress.", dictionary.Count));
			this.AddUnlockCounts(dictionary, false);
			this.QueueUnlockNotification(this._newlyUnlocked);
			this._newlyUnlocked.Clear();
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00025950 File Offset: 0x00023B50
		private void OnTick(float dt)
		{
			if (this._pendingNotifications.Count == 0)
			{
				return;
			}
			if (!UnlocksBehavior.IsOnWorldMap())
			{
				return;
			}
			List<ItemObject> items = this._pendingNotifications.ToList<ItemObject>();
			this._pendingNotifications.Clear();
			try
			{
				UnlocksBehavior.ShowUnlockNotification(items);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x000259B0 File Offset: 0x00023BB0
		public static bool InProgress(string itemId)
		{
			int num;
			return UnlocksBehavior.Instance != null && itemId != null && (UnlocksBehavior.Instance.ProgressByItemId.TryGetValue(itemId, out num) && num > 0) && num < Math.Max(1, Config.RequiredKillsPerItem);
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x000259F8 File Offset: 0x00023BF8
		public static int GetProgress(string itemId)
		{
			if (UnlocksBehavior.Instance == null || itemId == null)
			{
				return 0;
			}
			int result;
			UnlocksBehavior.Instance.ProgressByItemId.TryGetValue(itemId, out result);
			return result;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00025A25 File Offset: 0x00023C25
		public static bool IsUnlocked(string itemId)
		{
			return UnlocksBehavior.Instance != null && itemId != null && UnlocksBehavior.Instance.UnlockedItemIds.Contains(itemId);
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00025A43 File Offset: 0x00023C43
		public static void Unlock(ItemObject item)
		{
			if (UnlocksBehavior.Instance == null || item == null)
			{
				return;
			}
			if (UnlocksBehavior.Instance.UnlockedItemIds.Contains(item.StringId))
			{
				return;
			}
			UnlocksBehavior.Instance.UnlockedItemIds.Add(item.StringId);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00025A7D File Offset: 0x00023C7D
		public static void Lock(ItemObject item)
		{
			if (UnlocksBehavior.Instance == null || item == null)
			{
				return;
			}
			if (!UnlocksBehavior.Instance.UnlockedItemIds.Contains(item.StringId))
			{
				return;
			}
			UnlocksBehavior.Instance.UnlockedItemIds.Remove(item.StringId);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00025AB8 File Offset: 0x00023CB8
		public void Reset()
		{
			this.UnlockedItemIds.Clear();
			this.ProgressByItemId.Clear();
			Log.Info("All unlocks and progress have been reset.");
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00025ADC File Offset: 0x00023CDC
		private void QueueUnlockNotification(IEnumerable<ItemObject> items)
		{
			if (items == null)
			{
				return;
			}
			List<ItemObject> list = (from i in items
			where i != null
			select i).ToList<ItemObject>();
			if (list.Count == 0)
			{
				return;
			}
			Log.Debug(string.Format("QueueUnlockNotification: queuing {0} items.", list.Count));
			this._pendingNotifications.AddRange(list);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00025B48 File Offset: 0x00023D48
		private static bool IsOnWorldMap()
		{
			Game game = Game.Current;
			GameStateManager gameStateManager = (game != null) ? game.GameStateManager : null;
			return gameStateManager != null && gameStateManager.ActiveState is MapState;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00025B7C File Offset: 0x00023D7C
		internal ItemObject GetRandomItem(string cultureId, int tier)
		{
			CultureObject culture = MBObjectManager.Instance.GetObject<CultureObject>(cultureId);
			if (culture == null)
			{
				return null;
			}
			List<ItemObject> list = (from i in MBObjectManager.Instance.GetObjectTypeList<ItemObject>()
			where i != null && i.Tier == (ItemObject.ItemTiers)tier && i.Culture == culture && i.ItemType > ItemObject.ItemTypeEnum.Invalid
			select i).ToList<ItemObject>();
			if (list.Count == 0)
			{
				return null;
			}
			int index = MBRandom.RandomInt(list.Count);
			return list[index];
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00025BF0 File Offset: 0x00023DF0
		internal void AddOwnCultureBonuses(Dictionary<int, int> bonuses)
		{
			string format = "AddOwnCultureBonuses: {0} tiers to process for culture {1}.";
			object arg = bonuses.Count;
			WFaction clan = Player.Clan;
			object arg2;
			if (clan == null)
			{
				arg2 = null;
			}
			else
			{
				WCulture culture = clan.Culture;
				arg2 = ((culture != null) ? culture.Name : null);
			}
			Log.Info(string.Format(format, arg, arg2));
			Dictionary<ItemObject, int> dictionary = new Dictionary<ItemObject, int>();
			foreach (int num in bonuses.Keys)
			{
				WFaction clan2 = Player.Clan;
				string cultureId;
				if (clan2 == null)
				{
					cultureId = null;
				}
				else
				{
					WCulture culture2 = clan2.Culture;
					cultureId = ((culture2 != null) ? culture2.StringId : null);
				}
				ItemObject randomItem = this.GetRandomItem(cultureId, num);
				if (randomItem != null)
				{
					dictionary[randomItem] = bonuses[num];
				}
			}
			if (dictionary.Count > 0)
			{
				this.AddUnlockCounts(dictionary, false);
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00025CC0 File Offset: 0x00023EC0
		internal void AddUnlockCounts(Dictionary<ItemObject, int> battleCounts, bool addCultureBonuses = true)
		{
			if ((!Config.UnlockItemsFromKills && !Config.UnlockItemsFromDiscards) || Config.AllEquipmentUnlocked)
			{
				return;
			}
			addCultureBonuses = (addCultureBonuses && Config.PlayerCultureUnlockBonus);
			Log.Info(string.Format("AddBattleCounts: {0} items to process.", battleCounts.Count));
			if (battleCounts == null || battleCounts.Count == 0)
			{
				return;
			}
			int num = Math.Max(1, Config.RequiredKillsPerItem);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num2 = 0;
			foreach (KeyValuePair<ItemObject, int> keyValuePair in battleCounts)
			{
				num2++;
				try
				{
					ItemObject key = keyValuePair.Key;
					int value = keyValuePair.Value;
					if (key != null)
					{
						if (!key.IsCraftedByPlayer)
						{
							if (addCultureBonuses && key.Culture != null && Player.Clan != null)
							{
								BasicCultureObject culture = key.Culture;
								if (((culture != null) ? culture.StringId : null) != Player.Clan.Culture.StringId)
								{
									if (!dictionary.ContainsKey((int)key.Tier))
									{
										dictionary[(int)key.Tier] = 0;
									}
									Dictionary<int, int> dictionary2 = dictionary;
									int tier = (int)key.Tier;
									dictionary2[tier] += value;
								}
							}
							string stringId = key.StringId;
							int num3;
							this.ProgressByItemId.TryGetValue(stringId, out num3);
							int num4 = num3 + value;
							this.ProgressByItemId[stringId] = num4;
							if (num3 < num && num4 >= num && !this.UnlockedItemIds.Contains(stringId))
							{
								this.UnlockedItemIds.Add(stringId);
								this._newlyUnlocked.Add(key);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}
			if (addCultureBonuses)
			{
				this.AddOwnCultureBonuses(dictionary);
			}
			Log.Info("AddBattleCounts complete.");
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00025EDC File Offset: 0x000240DC
		private static void ShowUnlockNotification(List<ItemObject> items)
		{
			if (items == null || items.Count == 0)
			{
				return;
			}
			Log.Debug(string.Format("ShowUnlockNotification: {0} items.", items.Count));
			List<string> list = (from i in items
			where i != null
			select i).Select(delegate(ItemObject i)
			{
				TextObject name = i.Name;
				return ((name != null) ? name.ToString() : null) ?? i.StringId;
			}).ToList<string>();
			if (list.Count == 0)
			{
				return;
			}
			List<string> list2 = list.Take(10).ToList<string>();
			int num = list.Count - list2.Count;
			if (Config.UnlockPopup)
			{
				List<string> list3 = new List<string>(list2);
				if (num > 0)
				{
					string item = L.T("items_unlocked_more_popup", "... and {MORE} more items.").SetTextVariable("MORE", num).ToString();
					list3.Add(item);
				}
				string value = string.Join("\n", list3);
				Sound.Play2D("event:/ui/notification/trait_change");
				Notifications.Popup(L.T("items_unlocked_title", "New Gear Unlocked"), new TextObject(value, null), null, true);
				return;
			}
			string variable = string.Join(", ", list2);
			TextObject message;
			if (num > 0)
			{
				message = L.T("items_unlocked_log_more", "New gear unlocked: {ITEMS}, and {MORE} more items.").SetTextVariable("ITEMS", variable).SetTextVariable("MORE", num);
			}
			else
			{
				message = L.T("items_unlocked_log", "New gear unlocked: {ITEMS}.").SetTextVariable("ITEMS", variable);
			}
			Notifications.Log(message, "#ffffffe0");
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00026060 File Offset: 0x00024260
		[CommandLineFunctionality.CommandLineArgumentFunction("unlock_item", "retinues")]
		public static string UnlockItem(List<string> args)
		{
			if (args.Count != 1)
			{
				return "Usage: retinues.unlock_item [id]";
			}
			WItem witem;
			try
			{
				witem = new WItem(MBObjectManager.Instance.GetObject<ItemObject>(args[0]));
			}
			catch
			{
				return "Invalid item ID.";
			}
			try
			{
				witem.Unlock();
			}
			catch (Exception ex)
			{
				return "Failed to unlock item: " + ex.Message;
			}
			return string.Format("Unlocked item {0} ({1}).", witem.Name, witem);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000260EC File Offset: 0x000242EC
		[CommandLineFunctionality.CommandLineArgumentFunction("reset_unlocks", "retinues")]
		public static string ResetUnlocks(List<string> args)
		{
			UnlocksBehavior instance = UnlocksBehavior.Instance;
			if (instance != null)
			{
				instance.Reset();
			}
			return "All unlocks have been reset.";
		}

		// Token: 0x040001E5 RID: 485
		private Dictionary<string, int> _progressByItemId;

		// Token: 0x040001E6 RID: 486
		private List<string> _unlockedItemIds;

		// Token: 0x040001E7 RID: 487
		private readonly List<ItemObject> _newlyUnlocked = new List<ItemObject>();

		// Token: 0x040001E8 RID: 488
		private readonly List<ItemObject> _pendingNotifications = new List<ItemObject>();
	}
}
