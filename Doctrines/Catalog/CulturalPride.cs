using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Game.Events;
using Retinues.Game.Wrappers;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Catalog
{
	// Token: 0x020000E2 RID: 226
	public sealed class CulturalPride : Doctrine
	{
		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000914 RID: 2324 RVA: 0x0002CF31 File Offset: 0x0002B131
		public override TextObject Name
		{
			get
			{
				return L.T("cultural_pride", "Cultural Pride");
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0002CF42 File Offset: 0x0002B142
		public override TextObject Description
		{
			get
			{
				return L.T("cultural_pride_description", "20% rebate on clan culture gear.");
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000916 RID: 2326 RVA: 0x0002CF53 File Offset: 0x0002B153
		public override int Column
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x0002CF56 File Offset: 0x0002B156
		public override int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000918 RID: 2328 RVA: 0x0002CF59 File Offset: 0x0002B159
		public override bool IsDisabled
		{
			get
			{
				return !Config.EquippingTroopsCostsGold || Config.EquipmentCostMultiplier <= 0f;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x0002CF7D File Offset: 0x0002B17D
		public override TextObject DisabledMessage
		{
			get
			{
				return L.T("cultural_pride_disabled_message", "Disabled: equipment costs are disabled in config.");
			}
		}

		// Token: 0x020001AF RID: 431
		public sealed class CP_TournamentOwnCultureGear : Feat
		{
			// Token: 0x17000494 RID: 1172
			// (get) Token: 0x06000CD6 RID: 3286 RVA: 0x00036370 File Offset: 0x00034570
			public override TextObject Description
			{
				get
				{
					return L.T("cultural_pride_tournament_own_culture_gear", "Win a tournament wearing only armor of your own culture.");
				}
			}

			// Token: 0x17000495 RID: 1173
			// (get) Token: 0x06000CD7 RID: 3287 RVA: 0x00036381 File Offset: 0x00034581
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CD8 RID: 3288 RVA: 0x00036384 File Offset: 0x00034584
			public override void OnTournamentFinished(Tournament tournament)
			{
				if (tournament.Winner != Player.Character)
				{
					return;
				}
				if (ModCompatibility.SkipItemCultureChecks)
				{
					base.AdvanceProgress(1);
				}
				List<ItemObject> list = new List<ItemObject>();
				Equipment firstBattleEquipment = CharacterObject.PlayerCharacter.FirstBattleEquipment;
				EquipmentIndex[] array = new EquipmentIndex[5];
				RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.C79F55149DCD4B043EB77B7C0CD8D0A5BCDF9924D41BDC401A512772EEEA2327).FieldHandle);
				foreach (EquipmentIndex index in array)
				{
					ItemObject item2 = firstBattleEquipment[index].Item;
					if (item2 != null)
					{
						BasicCultureObject culture = item2.Culture;
						if (((culture != null) ? culture.StringId : null) != Player.Culture.StringId)
						{
							string format = "CP_TournamentOwnCultureGear: item {0} ({1}) does not match player culture ({2})";
							object name = item2.Name;
							BasicCultureObject culture2 = item2.Culture;
							Log.Info(string.Format(format, name, (culture2 != null) ? culture2.StringId : null, Player.Culture));
							if (!list.Contains(item2))
							{
								list.Add(item2);
							}
						}
					}
				}
				if (list.Count > 0)
				{
					Log.Message("Cultural Pride: progress for tournament win blocked by: " + string.Join(", ", list.Select(delegate(ItemObject item)
					{
						string format2 = "{0} ({1})";
						object name2 = item.Name;
						BasicCultureObject culture3 = item.Culture;
						return string.Format(format2, name2, (culture3 != null) ? culture3.Name : null);
					})) + ".");
					return;
				}
				base.AdvanceProgress(1);
			}
		}

		// Token: 0x020001B0 RID: 432
		public sealed class CP_FullSet100Kills : Feat
		{
			// Token: 0x17000496 RID: 1174
			// (get) Token: 0x06000CDA RID: 3290 RVA: 0x000364C7 File Offset: 0x000346C7
			public override TextObject Description
			{
				get
				{
					return L.T("cultural_pride_full_set_100_kills", "Get 100 kills in battle with troops wearing no foreign gear.");
				}
			}

			// Token: 0x17000497 RID: 1175
			// (get) Token: 0x06000CDB RID: 3291 RVA: 0x000364D8 File Offset: 0x000346D8
			public override int Target
			{
				get
				{
					return 100;
				}
			}

			// Token: 0x06000CDC RID: 3292 RVA: 0x000364DC File Offset: 0x000346DC
			public override void OnBattleEnd(Battle battle)
			{
				Dictionary<WCharacter, List<WItem>> dictionary = new Dictionary<WCharacter, List<WItem>>();
				foreach (Combat.Kill kill in battle.Kills)
				{
					if (kill.KillerIsPlayerTroop)
					{
						WCharacter killer = kill.Killer;
						if (killer.IsCustom)
						{
							bool flag = true;
							foreach (WItem witem in killer.Loadout.Battle.Items)
							{
								if (!(witem.Culture == null) && !(witem.Culture.StringId == "neutral_culture") && witem.Culture != killer.Culture)
								{
									flag = false;
									if (!dictionary.ContainsKey(killer))
									{
										dictionary[killer] = new List<WItem>();
									}
									if (!dictionary[killer].Contains(witem))
									{
										dictionary[killer].Add(witem);
									}
								}
							}
							if (flag)
							{
								base.AdvanceProgress(1);
							}
						}
					}
				}
				foreach (KeyValuePair<WCharacter, List<WItem>> keyValuePair in dictionary)
				{
					string[] array = new string[7];
					array[0] = "Cultural Pride: progress for ";
					array[1] = keyValuePair.Key.Name;
					array[2] = " (";
					int num = 3;
					WCulture culture = keyValuePair.Key.Culture;
					array[num] = ((culture != null) ? culture.Name : null);
					array[4] = ") blocked by: ";
					array[5] = string.Join(", ", keyValuePair.Value.Select(delegate(WItem item)
					{
						string name = item.Name;
						string str = " (";
						WCulture culture2 = item.Culture;
						return name + str + ((culture2 != null) ? culture2.Name : null) + ")";
					}));
					array[6] = ".";
					Log.Message(string.Concat(array));
				}
			}
		}

		// Token: 0x020001B1 RID: 433
		public sealed class CP_DefeatForeignRuler : Feat
		{
			// Token: 0x17000498 RID: 1176
			// (get) Token: 0x06000CDE RID: 3294 RVA: 0x000366F4 File Offset: 0x000348F4
			public override TextObject Description
			{
				get
				{
					return L.T("cultural_pride_defeat_foreign_ruler", "Defeat a ruler of a different culture in battle.");
				}
			}

			// Token: 0x17000499 RID: 1177
			// (get) Token: 0x06000CDF RID: 3295 RVA: 0x00036705 File Offset: 0x00034905
			public override int Target
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x06000CE0 RID: 3296 RVA: 0x00036708 File Offset: 0x00034908
			public override void OnBattleEnd(Battle battle)
			{
				if (battle.IsLost)
				{
					return;
				}
				foreach (WCharacter wcharacter in battle.EnemyLeaders)
				{
					if (wcharacter != null && wcharacter.IsRuler && !(((wcharacter != null) ? wcharacter.Culture : null) == Player.Culture))
					{
						base.AdvanceProgress(1);
						break;
					}
				}
			}
		}
	}
}
