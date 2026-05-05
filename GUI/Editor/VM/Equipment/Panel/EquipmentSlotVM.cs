using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Equipment.Panel
{
	// Token: 0x02000083 RID: 131
	[SafeClass]
	public sealed class EquipmentSlotVM : BaseButtonVM
	{
		// Token: 0x0600043A RID: 1082 RVA: 0x000171BC File Offset: 0x000153BC
		public EquipmentSlotVM(EquipmentIndex index) : base(false)
		{
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x000171CC File Offset: 0x000153CC
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Equip] = new string[]
				{
					"ItemText",
					"ItemTextColor",
					"IsStaged",
					"ImageId",
					"ImageAdditionalArgs",
					"ImageTextureProviderName",
					"Hint",
					"EquipChangeHint",
					"IsEnabled"
				};
				dictionary[UIEvent.Equipment] = new string[]
				{
					"ItemText",
					"ItemTextColor",
					"IsStaged",
					"ImageId",
					"ImageAdditionalArgs",
					"ImageTextureProviderName",
					"Hint",
					"EquipChangeHint",
					"IsEnabled"
				};
				dictionary[UIEvent.Slot] = new string[]
				{
					"IsSelected"
				};
				return dictionary;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x000172A1 File Offset: 0x000154A1
		private bool HasPendingItem
		{
			get
			{
				return State.EquipData[this.Index].Equip != null;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x000172BB File Offset: 0x000154BB
		private WItem Item
		{
			get
			{
				if (!this.HasPendingItem)
				{
					return State.Equipment.Get(this.Index);
				}
				return new WItem(State.EquipData[this.Index].Equip.ItemId);
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x000172F8 File Offset: 0x000154F8
		[DataSourceProperty]
		public string Label
		{
			get
			{
				switch (this.Index)
				{
				case EquipmentIndex.WeaponItemBeginSlot:
					return L.S("weapon_1_slot_text", "Weapon 1");
				case EquipmentIndex.Weapon1:
					return L.S("weapon_2_slot_text", "Weapon 2");
				case EquipmentIndex.Weapon2:
					return L.S("weapon_3_slot_text", "Weapon 3");
				case EquipmentIndex.Weapon3:
					return L.S("weapon_4_slot_text", "Weapon 4");
				case EquipmentIndex.NumAllWeaponSlots:
					return L.S("head_slot_text", "Head");
				case EquipmentIndex.Body:
					return L.S("body_slot_text", "Body");
				case EquipmentIndex.Leg:
					return L.S("leg_slot_text", "Legs");
				case EquipmentIndex.Gloves:
					return L.S("gloves_slot_text", "Gloves");
				case EquipmentIndex.Cape:
					return L.S("cape_slot_text", "Cape");
				case EquipmentIndex.ArmorItemEndSlot:
					return L.S("horse_slot_text", "Horse");
				case EquipmentIndex.HorseHarness:
					return L.S("horse_harness_slot_text", "Harness");
				}
				return string.Empty;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x00017420 File Offset: 0x00015620
		[DataSourceProperty]
		public string ItemText
		{
			get
			{
				if (!(this.Item == null))
				{
					string text;
					if (!this.HasPendingItem)
					{
						WItem item = this.Item;
						text = ((item != null) ? item.Name : null);
					}
					else
					{
						text = this.Item.Name + string.Format(" ({0}h)", State.EquipData[this.Index].Equip.Remaining);
					}
					return Format.Crop(text, ClanScreen.IsStudioMode ? 75 : 50);
				}
				return string.Empty;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x000174A8 File Offset: 0x000156A8
		[DataSourceProperty]
		public string ItemTextColor
		{
			get
			{
				if (!this.IsStaged)
				{
					return "#F4E1C4FF";
				}
				return "#ebaf2fff";
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x000174BD File Offset: 0x000156BD
		[DataSourceProperty]
		public bool IsStaged
		{
			get
			{
				return this.HasPendingItem;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x000174C5 File Offset: 0x000156C5
		[DataSourceProperty]
		public string ImageId
		{
			get
			{
				WItem item = this.Item;
				if (item == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = item.Image;
				if (image == null)
				{
					return null;
				}
				return image.Id;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x000174E3 File Offset: 0x000156E3
		[DataSourceProperty]
		public string ImageAdditionalArgs
		{
			get
			{
				WItem item = this.Item;
				if (item == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = item.Image;
				if (image == null)
				{
					return null;
				}
				return image.AdditionalArgs;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x00017501 File Offset: 0x00015701
		[DataSourceProperty]
		public string ImageTextureProviderName
		{
			get
			{
				WItem item = this.Item;
				if (item == null)
				{
					return null;
				}
				ItemImageIdentifierVM image = item.Image;
				if (image == null)
				{
					return null;
				}
				return image.TextureProviderName;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x0001751F File Offset: 0x0001571F
		[DataSourceProperty]
		public CharacterEquipmentItemVM Hint
		{
			get
			{
				WItem item = this.Item;
				if (((item != null) ? item.Base : null) == null)
				{
					return null;
				}
				return new CharacterEquipmentItemVM(this.Item.Base);
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00017547 File Offset: 0x00015747
		[DataSourceProperty]
		public BasicTooltipViewModel EquipChangeHint
		{
			get
			{
				if (!this.IsStaged)
				{
					return null;
				}
				return Tooltip.MakeTooltip(null, L.S("equip_change_tooltip_body", "This is a pending item change.\n\nTo apply the change, select 'Equip troops' from a fief's town menu."));
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x00017568 File Offset: 0x00015768
		[DataSourceProperty]
		public override bool IsSelected
		{
			get
			{
				return State.Slot == this.Index;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00017578 File Offset: 0x00015778
		[DataSourceProperty]
		public override bool IsEnabled
		{
			get
			{
				return (!Config.DisallowMountsForT1Troops || State.Troop.Tier > 1 || State.Troop.IsHero || (this.Index != EquipmentIndex.ArmorItemEndSlot && this.Index != EquipmentIndex.HorseHarness)) && (this.Index != EquipmentIndex.HorseHarness || !(State.Equipment.Get(EquipmentIndex.ArmorItemEndSlot) == null));
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x000175E1 File Offset: 0x000157E1
		[DataSourceMethod]
		public void ExecuteSelect()
		{
			State.UpdateSlot(this.Index);
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x000175EE File Offset: 0x000157EE
		public EquipmentIndex SlotIndex
		{
			get
			{
				return this.Index;
			}
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x000175F6 File Offset: 0x000157F6
		public void OnSlotChanged()
		{
			base.OnPropertyChanged("IsSelected");
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00017604 File Offset: 0x00015804
		public void OnEquipmentChanged()
		{
			base.OnPropertyChanged("ItemText");
			base.OnPropertyChanged("ItemTextColor");
			base.OnPropertyChanged("IsStaged");
			base.OnPropertyChanged("ImageId");
			base.OnPropertyChanged("ImageAdditionalArgs");
			base.OnPropertyChanged("ImageTextureProviderName");
			base.OnPropertyChanged("Hint");
			base.OnPropertyChanged("EquipChangeHint");
			base.OnPropertyChanged("IsEnabled");
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00017674 File Offset: 0x00015874
		public void OnEquipChanged()
		{
			this.OnEquipmentChanged();
		}

		// Token: 0x04000116 RID: 278
		private readonly EquipmentIndex Index = index;
	}
}
