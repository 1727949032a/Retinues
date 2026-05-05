using System;
using System.Collections.Generic;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;

namespace Retinues.Features.Unlocks
{
	// Token: 0x020000B1 RID: 177
	[SafeClass]
	public class UnlocksMissionBehavior : Combat
	{
		// Token: 0x0600076D RID: 1901 RVA: 0x00026104 File Offset: 0x00024304
		protected override void OnEndMission()
		{
			if (!Config.UnlockItemsFromKills)
			{
				return;
			}
			if (Config.AllEquipmentUnlocked)
			{
				return;
			}
			if (base.IsDefeat)
			{
				return;
			}
			Log.Info("OnEndMission (victory): counting items for unlocks.");
			Dictionary<ItemObject, int> dictionary = new Dictionary<ItemObject, int>();
			foreach (Combat.Kill kill in base.Kills)
			{
				if (!kill.VictimIsPlayerTroop && !kill.VictimIsPlayer && (!kill.VictimIsAllyTroop || DoctrineAPI.IsDoctrineUnlocked<PragmaticScavengers>()) && !kill.KillerIsEnemyTroop && (!kill.KillerIsAllyTroop || DoctrineAPI.IsDoctrineUnlocked<BattlefieldTithes>()))
				{
					int num = 1;
					if (DoctrineAPI.IsDoctrineUnlocked<LionsShare>() && kill.KillerIsPlayer)
					{
						num = 2;
					}
					foreach (WItem witem in UnlocksMissionBehavior.GetLoot(kill.LootCode))
					{
						if (UnlocksMissionBehavior.IsUnlockable(witem.Base))
						{
							int num2;
							dictionary[witem.Base] = (dictionary.TryGetValue(witem.Base, out num2) ? (num2 + num) : num);
						}
					}
				}
			}
			UnlocksBehavior.Instance.AddUnlockCounts(dictionary, true);
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x00026260 File Offset: 0x00024460
		private static bool IsUnlockable(ItemObject i)
		{
			return i != null && i.ItemType != ItemObject.ItemTypeEnum.Invalid;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x00026272 File Offset: 0x00024472
		private static IEnumerable<WItem> GetLoot(string equipmentCode)
		{
			UnlocksMissionBehavior.<GetLoot>d__2 <GetLoot>d__ = new UnlocksMissionBehavior.<GetLoot>d__2(-2);
			<GetLoot>d__.<>3__equipmentCode = equipmentCode;
			return <GetLoot>d__;
		}
	}
}
