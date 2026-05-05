using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Agents;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor.VM.Equipment.List;
using Retinues.GUI.Editor.VM.Equipment.Panel;
using Retinues.GUI.Helpers;
using Retinues.Managers;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Equipment
{
	// Token: 0x02000082 RID: 130
	[SafeClass]
	public sealed class EquipmentScreenVM : BaseVM
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060003CD RID: 973 RVA: 0x0001538C File Offset: 0x0001358C
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Faction] = new string[]
				{
					"Weapon1Slot",
					"Weapon2Slot",
					"Weapon3Slot",
					"Weapon4Slot",
					"HeadSlot",
					"CapeSlot",
					"BodySlot",
					"GlovesSlot",
					"LegSlot",
					"HorseSlot",
					"HorseHarnessSlot",
					"CanUnstage",
					"CanUnequip",
					"EquipmentName",
					"CanSelectPrevSet",
					"CanSelectNextSet",
					"CanRemoveSet",
					"CanCreateSet",
					"RemoveSetHint",
					"CreateSetHint",
					"CopyEquipmentHint",
					"PasteEquipmentHint",
					"CopyEquipmentIconColor",
					"PasteEquipmentIconColor"
				};
				dictionary[UIEvent.Equipment] = new string[]
				{
					"Weapon1Slot",
					"Weapon2Slot",
					"Weapon3Slot",
					"Weapon4Slot",
					"HeadSlot",
					"CapeSlot",
					"BodySlot",
					"GlovesSlot",
					"LegSlot",
					"HorseSlot",
					"HorseHarnessSlot",
					"CanUnstage",
					"CanUnequip",
					"EquipmentName",
					"CanSelectPrevSet",
					"CanSelectNextSet",
					"CanRemoveSet",
					"CanCreateSet",
					"RemoveSetHint",
					"CreateSetHint",
					"GenderOverrideIcon",
					"SetIsCivilian",
					"SetIsBattle",
					"SetIsEnabledForFieldBattle",
					"SetIsEnabledForNavalBattle",
					"SetIsEnabledForSiegeDefense",
					"SetIsEnabledForSiegeAssault",
					"SetHasGenderOverride",
					"FieldBattleHint",
					"NavalBattleHint",
					"SiegeDefenseHint",
					"SiegeAssaultHint",
					"SiegeHint",
					"GenderOverrideHint",
					"PreviewModeHint",
					"CivilianHint",
					"CanToggleEnableForFieldBattle",
					"CanToggleEnableForNavalBattle",
					"CanToggleEnableForSiegeDefense",
					"CanToggleEnableForSiegeAssault",
					"CopyEquipmentHint",
					"PasteEquipmentHint",
					"CopyEquipmentIconColor",
					"PasteEquipmentIconColor"
				};
				dictionary[UIEvent.Equip] = new string[]
				{
					"CanUnstage",
					"CanUnequip"
				};
				dictionary[UIEvent.Slot] = new string[]
				{
					"CanUnstage",
					"CanShowCrafted",
					"ShowCrafted",
					"ShowCraftedHint"
				};
				dictionary[UIEvent.Appearance] = new string[]
				{
					"GenderOverrideIcon"
				};
				return dictionary;
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001566D File Offset: 0x0001386D
		protected override void OnTroopChange()
		{
			PreviewOverlay.Disable();
			base.OnPropertyChanged("InPreviewMode");
			base.OnPropertyChanged("PreviewModeHint");
			this._editCivilianSets = State.Troop.IsCivilian;
			base.OnPropertyChanged("EditCivilianSets");
			this.EnsureValidSetForCurrentMode();
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060003CF RID: 975 RVA: 0x000156AB File Offset: 0x000138AB
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x000156B3 File Offset: 0x000138B3
		[DataSourceProperty]
		public EquipmentListVM EquipmentList { get; set; } = new EquipmentListVM();

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x000156BC File Offset: 0x000138BC
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x000156C4 File Offset: 0x000138C4
		[DataSourceProperty]
		public EquipmentSlotVM Weapon1Slot { get; set; } = new EquipmentSlotVM(EquipmentIndex.WeaponItemBeginSlot);

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x000156CD File Offset: 0x000138CD
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x000156D5 File Offset: 0x000138D5
		[DataSourceProperty]
		public EquipmentSlotVM Weapon2Slot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Weapon1);

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x000156DE File Offset: 0x000138DE
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x000156E6 File Offset: 0x000138E6
		[DataSourceProperty]
		public EquipmentSlotVM Weapon3Slot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Weapon2);

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x000156EF File Offset: 0x000138EF
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x000156F7 File Offset: 0x000138F7
		[DataSourceProperty]
		public EquipmentSlotVM Weapon4Slot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Weapon3);

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00015700 File Offset: 0x00013900
		// (set) Token: 0x060003DA RID: 986 RVA: 0x00015708 File Offset: 0x00013908
		[DataSourceProperty]
		public EquipmentSlotVM HeadSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.NumAllWeaponSlots);

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00015711 File Offset: 0x00013911
		// (set) Token: 0x060003DC RID: 988 RVA: 0x00015719 File Offset: 0x00013919
		[DataSourceProperty]
		public EquipmentSlotVM CapeSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Cape);

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060003DD RID: 989 RVA: 0x00015722 File Offset: 0x00013922
		// (set) Token: 0x060003DE RID: 990 RVA: 0x0001572A File Offset: 0x0001392A
		[DataSourceProperty]
		public EquipmentSlotVM BodySlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Body);

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060003DF RID: 991 RVA: 0x00015733 File Offset: 0x00013933
		// (set) Token: 0x060003E0 RID: 992 RVA: 0x0001573B File Offset: 0x0001393B
		[DataSourceProperty]
		public EquipmentSlotVM GlovesSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Gloves);

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x00015744 File Offset: 0x00013944
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x0001574C File Offset: 0x0001394C
		[DataSourceProperty]
		public EquipmentSlotVM LegSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.Leg);

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x00015755 File Offset: 0x00013955
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x0001575D File Offset: 0x0001395D
		[DataSourceProperty]
		public EquipmentSlotVM HorseSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.ArmorItemEndSlot);

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x00015766 File Offset: 0x00013966
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x0001576E File Offset: 0x0001396E
		[DataSourceProperty]
		public EquipmentSlotVM HorseHarnessSlot { get; set; } = new EquipmentSlotVM(EquipmentIndex.HorseHarness);

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x00015778 File Offset: 0x00013978
		private IEnumerable<EquipmentSlotVM> EquipmentSlots
		{
			get
			{
				return new <>z__ReadOnlyArray<EquipmentSlotVM>(new EquipmentSlotVM[]
				{
					this.Weapon1Slot,
					this.Weapon2Slot,
					this.Weapon3Slot,
					this.Weapon4Slot,
					this.HeadSlot,
					this.CapeSlot,
					this.BodySlot,
					this.GlovesSlot,
					this.LegSlot,
					this.HorseSlot,
					this.HorseHarnessSlot
				});
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x000157F8 File Offset: 0x000139F8
		public EquipmentScreenVM()
		{
			Dictionary<EquipmentIndex, EquipmentSlotVM> dictionary = new Dictionary<EquipmentIndex, EquipmentSlotVM>(13);
			dictionary[EquipmentIndex.WeaponItemBeginSlot] = this.Weapon1Slot;
			dictionary[EquipmentIndex.Weapon1] = this.Weapon2Slot;
			dictionary[EquipmentIndex.Weapon2] = this.Weapon3Slot;
			dictionary[EquipmentIndex.Weapon3] = this.Weapon4Slot;
			dictionary[EquipmentIndex.NumAllWeaponSlots] = this.HeadSlot;
			dictionary[EquipmentIndex.Cape] = this.CapeSlot;
			dictionary[EquipmentIndex.Body] = this.BodySlot;
			dictionary[EquipmentIndex.Gloves] = this.GlovesSlot;
			dictionary[EquipmentIndex.Leg] = this.LegSlot;
			dictionary[EquipmentIndex.ArmorItemEndSlot] = this.HorseSlot;
			dictionary[EquipmentIndex.HorseHarness] = this.HorseHarnessSlot;
			this._slotsByIndex = dictionary;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0001594C File Offset: 0x00013B4C
		private EquipmentSlotVM GetSlotVm(EquipmentIndex index)
		{
			EquipmentSlotVM result;
			if (!this._slotsByIndex.TryGetValue(index, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001596C File Offset: 0x00013B6C
		private int CountEnabled(PolicyToggleType t)
		{
			WCharacter troop = State.Troop;
			if (troop == null)
			{
				return 0;
			}
			int num = 0;
			List<WEquipment> equipments = troop.Loadout.Equipments;
			for (int i = 0; i < equipments.Count; i++)
			{
				if (!equipments[i].IsCivilian && CombatAgentBehavior.IsEnabled(troop, i, t))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x000159C6 File Offset: 0x00013BC6
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x000159D0 File Offset: 0x00013BD0
		[DataSourceProperty]
		public bool EditCivilianSets
		{
			get
			{
				return this._editCivilianSets;
			}
			set
			{
				if (this._editCivilianSets == value)
				{
					return;
				}
				this._editCivilianSets = value;
				base.OnPropertyChanged("EditCivilianSets");
				base.OnPropertyChanged("CivilianHint");
				this.EnsureValidSetForCurrentMode();
				base.OnPropertyChanged("EquipmentName");
				base.OnPropertyChanged("CanSelectPrevSet");
				base.OnPropertyChanged("CanSelectNextSet");
				base.OnPropertyChanged("CanCreateSet");
				base.OnPropertyChanged("CanRemoveSet");
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00015A44 File Offset: 0x00013C44
		private void EnsureValidSetForCurrentMode()
		{
			WCharacter troop = State.Troop;
			if (troop == null)
			{
				return;
			}
			WEquipment current = State.Equipment;
			if (current == null)
			{
				WEquipment wequipment = this._editCivilianSets ? troop.Loadout.Civilian : troop.Loadout.Battle;
				if (wequipment != null)
				{
					State.UpdateEquipment(wequipment);
				}
				return;
			}
			if (current.IsCivilian != this._editCivilianSets)
			{
				WEquipment wequipment2 = this._editCivilianSets ? troop.Loadout.Civilian : troop.Loadout.Battle;
				if (wequipment2 != null)
				{
					State.UpdateEquipment(wequipment2);
				}
				return;
			}
			List<WEquipment> source = this._editCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets;
			if (!source.Any((WEquipment e) => e.Base == current.Base))
			{
				WEquipment wequipment3 = source.FirstOrDefault<WEquipment>();
				if (wequipment3 != null)
				{
					State.UpdateEquipment(wequipment3);
				}
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x00015B2E File Offset: 0x00013D2E
		[DataSourceProperty]
		public bool ShowEquipmentCheckboxes
		{
			get
			{
				return base.IsVisible && !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x00015B42 File Offset: 0x00013D42
		[DataSourceProperty]
		public bool HasNavalDLC
		{
			get
			{
				return ModCompatibility.HasNavalDLC;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00015B4C File Offset: 0x00013D4C
		[DataSourceProperty]
		public bool CanShowCrafted
		{
			get
			{
				return (DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>() || ClanScreen.IsStudioMode) && EquipmentListVM.WeaponSlots.Contains(State.Slot.ToString());
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x00015B86 File Offset: 0x00013D86
		[DataSourceProperty]
		public bool ShowCrafted
		{
			get
			{
				return this.EquipmentList.ShowCrafted;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00015B93 File Offset: 0x00013D93
		[DataSourceProperty]
		public bool CanUnstage
		{
			get
			{
				return this.EquipmentSlots.Any((EquipmentSlotVM s) => s.IsStaged);
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x00015BBF File Offset: 0x00013DBF
		[DataSourceProperty]
		public bool CanUnequip
		{
			get
			{
				WEquipment equipment = State.Equipment;
				return equipment != null && equipment.Items.Any<WItem>();
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00015BD6 File Offset: 0x00013DD6
		[DataSourceProperty]
		public string UnequipAllButtonText
		{
			get
			{
				return L.S("unequip_all_button_text", "Unequip All");
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00015BE7 File Offset: 0x00013DE7
		[DataSourceProperty]
		public string UnstageAllButtonText
		{
			get
			{
				return L.S("unstage_all_button_text", "Reset Changes");
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00015BF8 File Offset: 0x00013DF8
		[DataSourceProperty]
		public string EquipmentName
		{
			get
			{
				WCharacter troop = State.Troop;
				WEquipment eq = State.Equipment;
				if (troop == null || eq == null)
				{
					return "–";
				}
				List<WEquipment> list = this.EditCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets;
				int num = list.FindIndex((WEquipment e) => e.Base == eq.Base);
				if (num < 0)
				{
					return (eq.Index + 1).ToString();
				}
				return string.Format("{0}/{1}", (num + 1).ToString(), list.Count);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00015CA4 File Offset: 0x00013EA4
		[DataSourceProperty]
		public bool CanSelectPrevSet
		{
			get
			{
				WCharacter troop = State.Troop;
				WEquipment eq = State.Equipment;
				return !(troop == null) && eq != null && (this.EditCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets).FindIndex((WEquipment e) => e.Base == eq.Base) > 0;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00015D10 File Offset: 0x00013F10
		[DataSourceProperty]
		public bool CanSelectNextSet
		{
			get
			{
				WCharacter troop = State.Troop;
				WEquipment eq = State.Equipment;
				if (troop == null || eq == null)
				{
					return false;
				}
				List<WEquipment> list = this.EditCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets;
				int num = list.FindIndex((WEquipment e) => e.Base == eq.Base);
				return num >= 0 && num < list.Count - 1;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00015D8C File Offset: 0x00013F8C
		[DataSourceProperty]
		public bool ShowSetControls
		{
			get
			{
				return !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x00015D96 File Offset: 0x00013F96
		[DataSourceProperty]
		public bool CanCreateSet
		{
			get
			{
				return !State.Troop.IsHero;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x00015DA8 File Offset: 0x00013FA8
		[DataSourceProperty]
		public bool CanRemoveSet
		{
			get
			{
				WCharacter troop = State.Troop;
				WEquipment equipment = State.Equipment;
				if (troop == null || equipment == null)
				{
					return false;
				}
				if (troop.IsHero)
				{
					return false;
				}
				int num = troop.Loadout.CivilianSets.Count<WEquipment>();
				int num2 = troop.Loadout.BattleSets.Count<WEquipment>();
				if (!equipment.IsCivilian)
				{
					return num2 > 1;
				}
				return num > 1;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x00015E0C File Offset: 0x0001400C
		[DataSourceProperty]
		public bool SetIsCivilian
		{
			get
			{
				WEquipment equipment = State.Equipment;
				return equipment != null && equipment.IsCivilian;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x00015E1E File Offset: 0x0001401E
		[DataSourceProperty]
		public bool SetIsBattle
		{
			get
			{
				WEquipment equipment = State.Equipment;
				return equipment != null && !equipment.IsCivilian;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x00015E33 File Offset: 0x00014033
		[DataSourceProperty]
		public bool SetIsEnabledForFieldBattle
		{
			get
			{
				return !this.SetIsCivilian && CombatAgentBehavior.IsEnabled(State.Troop, State.Equipment.Index, PolicyToggleType.FieldBattle);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x00015E54 File Offset: 0x00014054
		[DataSourceProperty]
		public bool SetIsEnabledForSiegeDefense
		{
			get
			{
				return !this.SetIsCivilian && CombatAgentBehavior.IsEnabled(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeDefense);
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000400 RID: 1024 RVA: 0x00015E75 File Offset: 0x00014075
		[DataSourceProperty]
		public bool SetIsEnabledForSiegeAssault
		{
			get
			{
				return !this.SetIsCivilian && CombatAgentBehavior.IsEnabled(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeAssault);
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x00015E96 File Offset: 0x00014096
		[DataSourceProperty]
		public bool SetIsEnabledForNavalBattle
		{
			get
			{
				return !this.SetIsCivilian && CombatAgentBehavior.IsEnabled(State.Troop, State.Equipment.Index, PolicyToggleType.NavalBattle);
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x00015EB7 File Offset: 0x000140B7
		[DataSourceProperty]
		public bool SetHasGenderOverride
		{
			get
			{
				return CombatAgentBehavior.IsEnabled(State.Troop, State.Equipment.Index, PolicyToggleType.GenderOverride);
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x00015ECE File Offset: 0x000140CE
		[DataSourceProperty]
		public string GenderOverrideIcon
		{
			get
			{
				WCharacter troop = State.Troop;
				if (troop == null || !troop.IsFemale)
				{
					return "SPGeneral\\GeneralFlagIcons\\female_only";
				}
				return "SPGeneral\\GeneralFlagIcons\\male_only";
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x00015EEE File Offset: 0x000140EE
		[DataSourceProperty]
		public bool InPreviewMode
		{
			get
			{
				return PreviewOverlay.IsEnabled;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x00015EF5 File Offset: 0x000140F5
		[DataSourceProperty]
		public string PreviewModeIcon
		{
			get
			{
				return "Inventory\\icon_inspect";
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x00015EFC File Offset: 0x000140FC
		[DataSourceProperty]
		public string PreviewModeText
		{
			get
			{
				return L.S("preview_mode_text", "Preview Mode");
			}
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00015F10 File Offset: 0x00014110
		protected override void OnSlotChange()
		{
			this.EquipmentList.ShowCrafted = false;
			EquipmentIndex? lastSlotIndex = this._lastSlotIndex;
			EquipmentIndex slot = State.Slot;
			if (lastSlotIndex != null && lastSlotIndex.Value != slot)
			{
				EquipmentSlotVM slotVm = this.GetSlotVm(lastSlotIndex.Value);
				if (slotVm != null)
				{
					slotVm.OnSlotChanged();
				}
			}
			EquipmentSlotVM slotVm2 = this.GetSlotVm(slot);
			if (slotVm2 != null)
			{
				slotVm2.OnSlotChanged();
			}
			this._lastSlotIndex = new EquipmentIndex?(slot);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00015F7F File Offset: 0x0001417F
		protected override void OnEquipmentChange()
		{
			EquipmentSlotVM slotVm = this.GetSlotVm(State.Slot);
			if (slotVm == null)
			{
				return;
			}
			slotVm.OnEquipmentChanged();
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00015F96 File Offset: 0x00014196
		protected override void OnEquipChange()
		{
			EquipmentSlotVM slotVm = this.GetSlotVm(State.Slot);
			if (slotVm != null)
			{
				slotVm.OnEquipChanged();
			}
			if (State.Slot == EquipmentIndex.ArmorItemEndSlot)
			{
				EquipmentSlotVM slotVm2 = this.GetSlotVm(EquipmentIndex.HorseHarness);
				if (slotVm2 == null)
				{
					return;
				}
				slotVm2.OnEquipChanged();
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x00015FCC File Offset: 0x000141CC
		[DataSourceProperty]
		public BasicTooltipViewModel RemoveSetHint
		{
			get
			{
				if (this.CanRemoveSet)
				{
					return null;
				}
				if (!State.Troop.IsHero)
				{
					return Tooltip.MakeTooltip(null, L.T("remove_set_hint", "At least one set of this type must remain.").ToString());
				}
				return Tooltip.MakeTooltip(null, L.T("remove_set_hero_hint", "Cannot remove equipment sets for heroes.").ToString());
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x00016024 File Offset: 0x00014224
		[DataSourceProperty]
		public BasicTooltipViewModel CreateSetHint
		{
			get
			{
				if (this.CanCreateSet)
				{
					return null;
				}
				if (!State.Troop.IsHero)
				{
					return Tooltip.MakeTooltip(null, L.T("create_set_hint", "Disabled due to conflicting mods (Shokuho).").ToString());
				}
				return Tooltip.MakeTooltip(null, L.T("create_set_hero_hint", "Cannot create equipment sets for heroes.").ToString());
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0001607C File Offset: 0x0001427C
		[DataSourceProperty]
		public BasicTooltipViewModel CivilianHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, this.EditCivilianSets ? L.S("civilian_set_enabled_hint", "Uncheck this box to switch to battle sets.") : L.S("civilian_set_disabled_hint", "Check this box to switch to civilian sets."));
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x000160AC File Offset: 0x000142AC
		[DataSourceProperty]
		public bool CanToggleEnableForFieldBattle
		{
			get
			{
				return this.SetIsBattle && (!this.SetIsEnabledForFieldBattle || this.CountEnabled(PolicyToggleType.FieldBattle) > 1);
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x000160CC File Offset: 0x000142CC
		[DataSourceProperty]
		public bool CanToggleEnableForSiegeDefense
		{
			get
			{
				return this.SetIsBattle && (!this.SetIsEnabledForSiegeDefense || this.CountEnabled(PolicyToggleType.SiegeDefense) > 1);
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x000160EC File Offset: 0x000142EC
		[DataSourceProperty]
		public bool CanToggleEnableForSiegeAssault
		{
			get
			{
				return this.SetIsBattle && (!this.SetIsEnabledForSiegeAssault || this.CountEnabled(PolicyToggleType.SiegeAssault) > 1);
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x0001610C File Offset: 0x0001430C
		[DataSourceProperty]
		public bool CanToggleEnableForNavalBattle
		{
			get
			{
				return !this.SetIsCivilian && (!this.SetIsEnabledForNavalBattle || this.CountEnabled(PolicyToggleType.NavalBattle) > 1);
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0001612C File Offset: 0x0001432C
		[DataSourceProperty]
		public BasicTooltipViewModel FieldBattleHint
		{
			get
			{
				if (!this.SetIsBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_set_disabled", "Can't enable for civilian sets."));
				}
				if (this.CanToggleEnableForFieldBattle || !this.SetIsEnabledForFieldBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_field_ok", "Enable for field battles."));
				}
				return Tooltip.MakeTooltip(null, L.S("hint_last_enabled", "At least one battle set must remain enabled for each battle type."));
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00016194 File Offset: 0x00014394
		[DataSourceProperty]
		public BasicTooltipViewModel NavalBattleHint
		{
			get
			{
				if (!this.SetIsBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_set_disabled", "Can't enable for civilian sets."));
				}
				if (this.CanToggleEnableForNavalBattle || !this.SetIsEnabledForNavalBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_naval_ok", "Enable for naval battles."));
				}
				return Tooltip.MakeTooltip(null, L.S("hint_last_enabled", "At least one battle set must remain enabled for each battle type."));
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x000161FC File Offset: 0x000143FC
		[DataSourceProperty]
		public BasicTooltipViewModel SiegeDefenseHint
		{
			get
			{
				if (!this.SetIsBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_set_disabled", "Can't enable for civilian sets."));
				}
				if (this.CanToggleEnableForSiegeDefense || !this.SetIsEnabledForSiegeDefense)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_def_ok", "Enable for siege defense."));
				}
				return Tooltip.MakeTooltip(null, L.S("hint_last_enabled", "At least one battle set must remain enabled for each battle type."));
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x00016264 File Offset: 0x00014464
		[DataSourceProperty]
		public BasicTooltipViewModel SiegeAssaultHint
		{
			get
			{
				if (!this.SetIsBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_set_disabled", "Can't enable for civilian sets."));
				}
				if (this.CanToggleEnableForSiegeAssault || !this.SetIsEnabledForSiegeAssault)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_assault_ok", "Enable for siege assault."));
				}
				return Tooltip.MakeTooltip(null, L.S("hint_last_enabled", "At least one battle set must remain enabled for each battle type."));
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x000162CC File Offset: 0x000144CC
		[DataSourceProperty]
		public BasicTooltipViewModel SiegeHint
		{
			get
			{
				if (!this.SetIsBattle)
				{
					return Tooltip.MakeTooltip(null, L.S("hint_set_disabled", "Can't enable for civilian sets."));
				}
				if ((this.CanToggleEnableForSiegeAssault || !this.SetIsEnabledForSiegeAssault) && (this.CanToggleEnableForSiegeDefense || !this.SetIsEnabledForSiegeDefense))
				{
					return Tooltip.MakeTooltip(null, L.S("hint_siege_ok", "Enable for siege battles."));
				}
				return Tooltip.MakeTooltip(null, L.S("hint_last_enabled", "At least one battle set must remain enabled for each battle type."));
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x00016342 File Offset: 0x00014542
		[DataSourceProperty]
		public BasicTooltipViewModel GenderOverrideHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("gender_override_hint", "If enabled, troops spawning with this equipment set will be of the opposite gender."));
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x00016359 File Offset: 0x00014559
		[DataSourceProperty]
		public BasicTooltipViewModel PreviewModeHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, this.InPreviewMode ? L.S("preview_mode_disable_hint", "Disable Preview Mode.") : L.S("preview_mode_enable_hint", "Preview Mode: see how equipment looks on the troop without applying changes."));
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0001638C File Offset: 0x0001458C
		[DataSourceProperty]
		public BasicTooltipViewModel ShowCraftedHint
		{
			get
			{
				if (this.CanShowCrafted)
				{
					return Tooltip.MakeTooltip(null, this.ShowCrafted ? L.S("hide_crafted_hint", "Hide crafted items.") : L.S("show_crafted_hint", "Show crafted items."));
				}
				if (DoctrineAPI.IsDoctrineUnlocked<ClanicTraditions>())
				{
					return Tooltip.MakeTooltip(null, L.S("show_crafted_weapon_hint", "Only weapon slots can have crafted items."));
				}
				return Tooltip.MakeTooltip(null, L.S("show_crafted_disabled_hint", "Unlock the 'Clan Traditions' doctrine to show crafted items."));
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x00016402 File Offset: 0x00014602
		[DataSourceProperty]
		public BasicTooltipViewModel CopyEquipmentHint
		{
			get
			{
				if (State.Equipment == null)
				{
					return Tooltip.MakeTooltip(null, L.S("copy_equipment_disabled", "No equipment set selected."));
				}
				return Tooltip.MakeTooltip(null, L.S("copy_equipment_hint", "Copy this equipment set to the clipboard."));
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00016436 File Offset: 0x00014636
		[DataSourceProperty]
		public BasicTooltipViewModel PasteEquipmentHint
		{
			get
			{
				if (EquipmentScreenVM.Clipboard == null)
				{
					return Tooltip.MakeTooltip(null, L.S("paste_equipment_empty", "Clipboard is empty. Copy an equipment set first."));
				}
				return Tooltip.MakeTooltip(null, L.S("paste_equipment_hint", "Paste the copied equipment onto this set."));
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0001646A File Offset: 0x0001466A
		[DataSourceProperty]
		public string CopyEquipmentIconColor
		{
			get
			{
				if (!this._copyIsHovered)
				{
					return "#f8d28ab4";
				}
				return "#fdae1ae8";
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0001647F File Offset: 0x0001467F
		[DataSourceProperty]
		public string PasteEquipmentIconColor
		{
			get
			{
				if (EquipmentScreenVM.Clipboard == null)
				{
					return "#46433db4";
				}
				if (!this._pasteIsHovered)
				{
					return "#f8d28ab4";
				}
				return "#fdae1ae8";
			}
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000164A1 File Offset: 0x000146A1
		[DataSourceMethod]
		public void ExecuteBeginHoverCopyIcon()
		{
			this._copyIsHovered = true;
			base.OnPropertyChanged("CopyEquipmentIconColor");
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000164B5 File Offset: 0x000146B5
		[DataSourceMethod]
		public void ExecuteEndHoverCopyIcon()
		{
			this._copyIsHovered = false;
			base.OnPropertyChanged("CopyEquipmentIconColor");
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x000164C9 File Offset: 0x000146C9
		[DataSourceMethod]
		public void ExecuteBeginHoverPasteIcon()
		{
			this._pasteIsHovered = true;
			base.OnPropertyChanged("PasteEquipmentIconColor");
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x000164DD File Offset: 0x000146DD
		[DataSourceMethod]
		public void ExecuteEndHoverPasteIcon()
		{
			this._pasteIsHovered = false;
			base.OnPropertyChanged("PasteEquipmentIconColor");
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x000164F1 File Offset: 0x000146F1
		[DataSourceMethod]
		public void ExecuteToggleShowCrafted()
		{
			if (!this.CanShowCrafted)
			{
				return;
			}
			this.EquipmentList.ShowCrafted = !this.EquipmentList.ShowCrafted;
			base.OnPropertyChanged("ShowCrafted");
			base.OnPropertyChanged("ShowCraftedHint");
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0001652B File Offset: 0x0001472B
		[DataSourceMethod]
		public void ExecuteToggleEnableSetForFieldBattle()
		{
			if (!this.CanToggleEnableForFieldBattle)
			{
				return;
			}
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.FieldBattle);
			base.OnPropertyChanged("SetIsEnabledForFieldBattle");
			base.OnPropertyChanged("FieldBattleHint");
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00016561 File Offset: 0x00014761
		[DataSourceMethod]
		public void ExecuteToggleEnableSetForNavalBattle()
		{
			if (!this.CanToggleEnableForNavalBattle)
			{
				return;
			}
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.NavalBattle);
			base.OnPropertyChanged("SetIsEnabledForNavalBattle");
			base.OnPropertyChanged("NavalBattleHint");
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00016597 File Offset: 0x00014797
		[DataSourceMethod]
		public void ExecuteToggleEnableSetForSiegeDefense()
		{
			if (!this.CanToggleEnableForSiegeDefense)
			{
				return;
			}
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeDefense);
			base.OnPropertyChanged("SetIsEnabledForSiegeDefense");
			base.OnPropertyChanged("SiegeDefenseHint");
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000165CD File Offset: 0x000147CD
		[DataSourceMethod]
		public void ExecuteToggleEnableSetForSiegeAssault()
		{
			if (!this.CanToggleEnableForSiegeAssault)
			{
				return;
			}
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeAssault);
			base.OnPropertyChanged("SetIsEnabledForSiegeAssault");
			base.OnPropertyChanged("SiegeAssaultHint");
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00016604 File Offset: 0x00014804
		[DataSourceMethod]
		public void ExecuteToggleEnableSetForSiege()
		{
			if (!this.CanToggleEnableForSiegeAssault && !this.CanToggleEnableForSiegeDefense)
			{
				return;
			}
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeAssault);
			CombatAgentBehavior.Toggle(State.Troop, State.Equipment.Index, PolicyToggleType.SiegeDefense);
			base.OnPropertyChanged("SetIsEnabledForSiegeAssault");
			base.OnPropertyChanged("SetIsEnabledForSiegeDefense");
			base.OnPropertyChanged("SiegeAssaultHint");
			base.OnPropertyChanged("SiegeDefenseHint");
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00016678 File Offset: 0x00014878
		[DataSourceMethod]
		public void ExecuteToggleGenderOverride()
		{
			WCharacter troop = State.Troop;
			WEquipment equipment = State.Equipment;
			if (troop == null || equipment == null)
			{
				return;
			}
			CombatAgentBehavior.Toggle(troop, equipment.Index, PolicyToggleType.GenderOverride);
			base.OnPropertyChanged("SetHasGenderOverride");
			base.OnPropertyChanged("GenderOverrideHint");
			State.UpdateAppearance();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x000166C6 File Offset: 0x000148C6
		[DataSourceMethod]
		public void ExecuteTogglePreviewMode()
		{
			PreviewOverlay.Toggle();
			State.UpdateEquipment(State.Equipment);
			base.OnPropertyChanged("InPreviewMode");
			base.OnPropertyChanged("PreviewModeHint");
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x000166F0 File Offset: 0x000148F0
		[DataSourceMethod]
		public void ExecuteUnequipAll()
		{
			if (!this.CanUnequip)
			{
				return;
			}
			InformationManager.ShowInquiry(new InquiryData(L.S("unequip_all", "Unequip All"), L.T("unequip_all_text", "Unequip all items worn by {TROOP_NAME}?").SetTextVariable("TROOP_NAME", State.Troop.Name).ToString(), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
			{
				WCharacter troop = State.Troop;
				int index = State.Equipment.Index;
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					PendingEquipData pendingEquipData = EquipStagingBehavior.Get(troop, slot, index);
					if (pendingEquipData != null)
					{
						WItem stagedItem = new WItem(pendingEquipData.ItemId);
						EquipmentManager.RollbackStagedEquip(troop, index, slot, stagedItem);
					}
					EquipmentManager.TryUnequip(troop, index, slot);
				}
				State.UpdateEquipment(State.Equipment);
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x000167B4 File Offset: 0x000149B4
		[DataSourceMethod]
		public void ExecuteUnstageAll()
		{
			if (!this.CanUnstage)
			{
				return;
			}
			InformationManager.ShowInquiry(new InquiryData(L.S("unstage_all", "Reset Changes"), L.T("unstage_all_text", "Revert all staged equipment changes for {TROOP_NAME}?").SetTextVariable("TROOP_NAME", State.Troop.Name).ToString(), true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
			{
				WCharacter troop = State.Troop;
				int index = State.Equipment.Index;
				foreach (EquipmentIndex slot in WEquipment.Slots)
				{
					PendingEquipData pendingEquipData = EquipStagingBehavior.Get(troop, slot, index);
					if (pendingEquipData != null)
					{
						WItem stagedItem = new WItem(pendingEquipData.ItemId);
						EquipmentManager.RollbackStagedEquip(troop, index, slot, stagedItem);
					}
				}
				State.UpdateEquipment(State.Equipment);
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00016878 File Offset: 0x00014A78
		[DataSourceMethod]
		public void ExecuteCopyEquipment()
		{
			if (State.Equipment == null)
			{
				return;
			}
			EquipmentScreenVM.Clipboard = State.Equipment;
			base.OnPropertyChanged("CopyEquipmentHint");
			base.OnPropertyChanged("PasteEquipmentHint");
			base.OnPropertyChanged("CopyEquipmentIconColor");
			base.OnPropertyChanged("PasteEquipmentIconColor");
			Notifications.Log(L.S("equipment_copied", "Equipment set copied to clipboard."), "#ffffffe0");
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000168DC File Offset: 0x00014ADC
		[DataSourceMethod]
		public void ExecutePasteEquipment()
		{
			if (EquipmentScreenVM.Clipboard == null)
			{
				return;
			}
			WEquipment source = EquipmentScreenVM.Clipboard;
			WEquipment target = State.Equipment;
			if (target == null)
			{
				return;
			}
			WCharacter troop = target.Loadout.Troop;
			if (troop == null)
			{
				return;
			}
			HashSet<EquipmentIndex> hashSet = new HashSet<EquipmentIndex>();
			List<EquipmentIndex> list = new List<EquipmentIndex>();
			foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
			{
				WItem witem = source.Get(equipmentIndex);
				if (!(witem == null))
				{
					if (EquipmentManager.IsUnlockedForPaste(troop, equipmentIndex, witem))
					{
						hashSet.Add(equipmentIndex);
					}
					else
					{
						list.Add(equipmentIndex);
					}
				}
			}
			if (hashSet.Count == 0)
			{
				Notifications.Popup(L.T("paste_equip_all_locked_title", "Nothing Unlocked"), L.T("paste_equip_all_locked_text", "No items from this equipment set are currently unlocked for this troop."), null, true);
				return;
			}
			HashSet<EquipmentIndex> allowedSlotsOrNull = (list.Count > 0) ? hashSet : null;
			int num = EquipmentManager.QuotePasteGoldCost(source, target, allowedSlotsOrNull);
			int gold = Player.Gold;
			if (num > 0 && gold < num)
			{
				Notifications.Popup(L.T("paste_equip_cannot_afford_title", "Cannot Afford"), L.T("paste_equip_cannot_afford_text", "You need {GOLD} gold to paste this equipment, but you only have {CURRENT}.").SetTextVariable("GOLD", num).SetTextVariable("CURRENT", gold), null, true);
				return;
			}
			string name = source.Loadout.Troop.Name;
			string variable = (source.Index + 1).ToString();
			bool flag = list.Count > 0;
			TextObject title = flag ? L.T("paste_equip_locked_confirm_title", "Confirm Partial Equipment Copy") : L.T("paste_equip_confirm_title", "Confirm Equipment Copy");
			TextObject description;
			if (flag)
			{
				description = ((num > 0) ? L.T("paste_equip_locked_confirm_text_cost", "Some items in {TROOP}'s equipment n°{INDEX} are still locked and will be skipped. Pasting the unlocked items will cost {GOLD} gold. Continue?").SetTextVariable("GOLD", num).SetTextVariable("TROOP", name).SetTextVariable("INDEX", variable) : L.T("paste_equip_locked_confirm_text_free", "Some items in {TROOP}'s equipment n°{INDEX} are still locked and will be skipped. Paste the unlocked items onto the current set?").SetTextVariable("TROOP", name).SetTextVariable("INDEX", variable));
			}
			else
			{
				description = ((num > 0) ? L.T("paste_equip_confirm_text_cost", "Pasting {TROOP}'s equipment n°{INDEX} will cost {GOLD} gold. Continue?").SetTextVariable("GOLD", num).SetTextVariable("TROOP", name).SetTextVariable("INDEX", variable) : L.T("paste_equip_confirm_text_free", "Paste {TROOP}'s equipment n°{INDEX} onto the current set?").SetTextVariable("TROOP", name).SetTextVariable("INDEX", variable));
			}
			Notifications.ConfirmationPopup(title, description, delegate
			{
				EquipmentManager.PasteResult pasteResult = EquipmentManager.TryPasteEquipment(source, target, allowedSlotsOrNull);
				if (!pasteResult.Ok)
				{
					string text;
					switch (pasteResult.Reason)
					{
					case EquipmentManager.EquipFailReason.NotAllowed:
						text = EquipmentScreenVM.BuildPasteNotAllowedMessage(pasteResult);
						break;
					case EquipmentManager.EquipFailReason.NotEnoughStock:
						text = L.S("paste_failed_not_enough_stock", "You lack enough copies of required items.");
						break;
					case EquipmentManager.EquipFailReason.NotEnoughGold:
						text = L.S("paste_failed_not_enough_gold", "You do not have enough gold.");
						break;
					case EquipmentManager.EquipFailReason.NotCivilian:
						text = L.S("paste_failed_not_civilian", "A non-civilian item cannot be equipped in a civilian set.");
						break;
					default:
						text = L.S("paste_failed_generic", "The equipment paste failed.");
						break;
					}
					string fallback = text;
					Notifications.Popup(L.T("paste_equip_failed_title", "Copy Failed"), L.T("paste_equip_failed_text", fallback), null, true);
					return;
				}
				EquipmentScreenVM.Clipboard = null;
				State.UpdateEquipment(State.Equipment);
			}, null, null, true);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00016BA0 File Offset: 0x00014DA0
		private static string BuildPasteNotAllowedMessage(EquipmentManager.PasteResult res)
		{
			EquipmentManager.EquipLimitReason details = res.Details;
			if (details == EquipmentManager.EquipLimitReason.None)
			{
				return L.S("paste_failed_not_allowed", "Some items cannot be equipped by this troop.");
			}
			List<string> list = new List<string>();
			if (details.HasFlag(EquipmentManager.EquipLimitReason.MountT1))
			{
				list.Add("• " + L.S("paste_failed_reason_mount_t1", "Tier 1 troops are not allowed to have a mount."));
			}
			if (details.HasFlag(EquipmentManager.EquipLimitReason.TierDifference))
			{
				list.Add("• " + L.S("paste_failed_reason_tier_diff", "Some items are above the allowed tier difference for this troop."));
			}
			if (details.HasFlag(EquipmentManager.EquipLimitReason.Skill))
			{
				list.Add("• " + L.S("paste_failed_reason_skill", "This troop does not meet the skill requirements for some items."));
			}
			return L.S("paste_failed_not_allowed_header", "This equipment set cannot be applied to this troop:") + "\n\n" + string.Join("\n", list);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00016C88 File Offset: 0x00014E88
		[DataSourceMethod]
		public void ExecutePrevSet()
		{
			WCharacter troop = State.Troop;
			WEquipment eq = State.Equipment;
			if (troop == null || eq == null)
			{
				return;
			}
			List<WEquipment> list = this.EditCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets;
			int num = list.FindIndex((WEquipment e) => e.Base == eq.Base);
			if (num <= 0)
			{
				return;
			}
			State.UpdateEquipment(list[num - 1]);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00016D08 File Offset: 0x00014F08
		[DataSourceMethod]
		public void ExecuteNextSet()
		{
			WCharacter troop = State.Troop;
			WEquipment eq = State.Equipment;
			if (troop == null || eq == null)
			{
				return;
			}
			List<WEquipment> list = this.EditCivilianSets ? troop.Loadout.CivilianSets : troop.Loadout.BattleSets;
			int num = list.FindIndex((WEquipment e) => e.Base == eq.Base);
			if (num < 0 || num >= list.Count - 1)
			{
				return;
			}
			State.UpdateEquipment(list[num + 1]);
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00016D90 File Offset: 0x00014F90
		[DataSourceMethod]
		public void ExecuteRemoveSet()
		{
			if (!this.CanRemoveSet)
			{
				return;
			}
			WCharacter troop = State.Troop;
			WEquipment eq = State.Equipment;
			if (troop == null || eq == null)
			{
				return;
			}
			string titleText = L.S("remove_set_title", "Remove Set");
			string text = L.T("remove_set_text", "Remove set n°{EQUIPMENT} for {TROOP}?").SetTextVariable("EQUIPMENT", this.EquipmentName).SetTextVariable("TROOP", troop.Name).ToString();
			InformationManager.ShowInquiry(new InquiryData(titleText, text, true, true, L.S("confirm", "Confirm"), L.S("cancel", "Cancel"), delegate()
			{
				EquipmentManager.TryDeleteSet(troop, eq.Index);
				State.FixCombatPolicies(troop);
				WEquipment wequipment = this.EditCivilianSets ? troop.Loadout.CivilianSets.FirstOrDefault<WEquipment>() : troop.Loadout.BattleSets.FirstOrDefault<WEquipment>();
				if (wequipment == null)
				{
					wequipment = troop.Loadout.Battle;
				}
				if (wequipment != null)
				{
					State.UpdateEquipment(wequipment);
				}
			}, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00016E90 File Offset: 0x00015090
		[DataSourceMethod]
		public void ExecuteCreateSet()
		{
			if (!this.CanCreateSet)
			{
				return;
			}
			WCharacter troop = State.Troop;
			WEquipment src = State.Equipment;
			if (troop == null || src == null)
			{
				return;
			}
			string titleText = L.S("create_set_title", "Create Equipment Set");
			string text = L.S("create_set_text", "Do you want to copy the current set or create an empty set?");
			InformationManager.ShowInquiry(new InquiryData(titleText, text, true, true, L.S("copy_current", "Copy Current"), L.S("empty_set", "Empty"), delegate()
			{
				this.ExecuteCreateSet_CopyFlow(troop, src);
			}, delegate()
			{
				this.ExecuteCreateSet_EmptyFlow(troop);
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00016F54 File Offset: 0x00015154
		private void ExecuteCreateSet_EmptyFlow(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			WEquipment wequipment = this.EditCivilianSets ? troop.Loadout.CreateCivilianSet() : troop.Loadout.CreateBattleSet();
			if (wequipment == null)
			{
				return;
			}
			State.FixCombatPolicies(troop);
			State.UpdateEquipment(wequipment);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00016F9C File Offset: 0x0001519C
		private void ExecuteCreateSet_CopyFlow(WCharacter troop, WEquipment src)
		{
			if (troop == null || src == null)
			{
				return;
			}
			List<ValueTuple<EquipmentIndex, WItem>> plan = this.CollectCopyPlan(src);
			WEquipment wequipment = this.EditCivilianSets ? troop.Loadout.CreateCivilianSet() : troop.Loadout.CreateBattleSet();
			if (wequipment == null)
			{
				return;
			}
			EquipmentScreenVM.CopyItemsInto(wequipment, plan);
			State.FixCombatPolicies(troop);
			State.UpdateEquipment(wequipment);
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00016FF8 File Offset: 0x000151F8
		[return: TupleElementNames(new string[]
		{
			"slot",
			"item"
		})]
		private List<ValueTuple<EquipmentIndex, WItem>> CollectCopyPlan(WEquipment src)
		{
			List<ValueTuple<EquipmentIndex, WItem>> list = new List<ValueTuple<EquipmentIndex, WItem>>(WEquipment.Slots.Count);
			foreach (EquipmentIndex equipmentIndex in WEquipment.Slots)
			{
				WItem witem = src.Get(equipmentIndex);
				if (witem != null)
				{
					list.Add(new ValueTuple<EquipmentIndex, WItem>(equipmentIndex, witem));
				}
			}
			return list;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00017074 File Offset: 0x00015274
		private static void CopyItemsInto(WEquipment dst, [TupleElementNames(new string[]
		{
			"slot",
			"item"
		})] List<ValueTuple<EquipmentIndex, WItem>> plan)
		{
			foreach (ValueTuple<EquipmentIndex, WItem> valueTuple in plan)
			{
				EquipmentIndex item = valueTuple.Item1;
				WItem item2 = valueTuple.Item2;
				dst.SetItem(item, item2);
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x000170D0 File Offset: 0x000152D0
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x000170D7 File Offset: 0x000152D7
		private static WEquipment Clipboard { get; set; }

		// Token: 0x06000438 RID: 1080 RVA: 0x000170E0 File Offset: 0x000152E0
		public override void Show()
		{
			base.Show();
			this.EquipmentList.Show();
			foreach (EquipmentSlotVM equipmentSlotVM in this.EquipmentSlots)
			{
				equipmentSlotVM.Show();
			}
			this.EquipmentList.RefreshFilter();
			base.OnPropertyChanged("ShowEquipmentCheckboxes");
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00017154 File Offset: 0x00015354
		public override void Hide()
		{
			foreach (EquipmentSlotVM equipmentSlotVM in this.EquipmentSlots)
			{
				equipmentSlotVM.Hide();
			}
			this.EquipmentList.Hide();
			base.Hide();
			base.OnPropertyChanged("ShowEquipmentCheckboxes");
		}

		// Token: 0x0400010D RID: 269
		private readonly Dictionary<EquipmentIndex, EquipmentSlotVM> _slotsByIndex;

		// Token: 0x0400010E RID: 270
		private EquipmentIndex? _lastSlotIndex = new EquipmentIndex?(State.Slot);

		// Token: 0x0400010F RID: 271
		private bool _editCivilianSets;

		// Token: 0x04000110 RID: 272
		private const string HoveredColor = "#fdae1ae8";

		// Token: 0x04000111 RID: 273
		private const string EnabledColor = "#f8d28ab4";

		// Token: 0x04000112 RID: 274
		private const string DisabledColor = "#46433db4";

		// Token: 0x04000113 RID: 275
		private bool _copyIsHovered;

		// Token: 0x04000114 RID: 276
		private bool _pasteIsHovered;
	}
}
