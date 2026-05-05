using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A2 RID: 162
	public class CharacterObjectHelper
	{
		// Token: 0x0600070B RID: 1803 RVA: 0x00023630 File Offset: 0x00021830
		public static CharacterObject CopyInto(CharacterObject src, CharacterObject tgt)
		{
			if (src == null || tgt == null)
			{
				return tgt;
			}
			CharacterObject value = ((CharacterObject)CharacterObjectHelper.F_originCharacter.GetValue(src)) ?? src;
			CharacterObjectHelper.F_originCharacter.SetValue(tgt, value);
			CharacterObjectHelper.F_occupation.SetValue(tgt, CharacterObjectHelper.F_occupation.GetValue(src));
			CharacterObjectHelper.F_persona.SetValue(tgt, CharacterObjectHelper.F_persona.GetValue(src));
			PropertyOwner<TraitObject> propertyOwner = (PropertyOwner<TraitObject>)CharacterObjectHelper.F_characterTraits.GetValue(src);
			CharacterObjectHelper.F_characterTraits.SetValue(tgt, new PropertyOwner<TraitObject>(propertyOwner));
			CharacterObjectHelper.F_civilianEquipmentTemplate.SetValue(tgt, CharacterObjectHelper.F_civilianEquipmentTemplate.GetValue(src));
			CharacterObjectHelper.F_battleEquipmentTemplate.SetValue(tgt, CharacterObjectHelper.F_battleEquipmentTemplate.GetValue(src));
			CharacterObjectHelper.M_fillFrom.Invoke(tgt, new object[]
			{
				src
			});
			CharacterObjectHelper.InstallFreshRosterFromSourceBattleSets(src, tgt);
			return tgt;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00023700 File Offset: 0x00021900
		protected static void InstallFreshRosterFromSourceBattleSets(CharacterObject src, CharacterObject tgt)
		{
			try
			{
				IEnumerable<Equipment> battleEquipments = src.BattleEquipments;
				List<Equipment> list = ((battleEquipments != null) ? battleEquipments.ToList<Equipment>() : null) ?? new List<Equipment>();
				if (list.Count == 0)
				{
					list.Add(new Equipment(Equipment.EquipmentType.Battle));
				}
				List<Equipment> list2 = new List<Equipment>(list.Count);
				foreach (Equipment equipment in list)
				{
					string text = (equipment != null) ? equipment.CalculateEquipmentCode() : null;
					Equipment equipment2 = (text != null) ? Equipment.CreateFromEquipmentCode(text) : new Equipment(Equipment.EquipmentType.Battle);
					try
					{
						FieldInfo fieldInfo = AccessTools.Field(typeof(Equipment), "_equipmentType");
						if (fieldInfo != null)
						{
							fieldInfo.SetValue(equipment2, Equipment.EquipmentType.Battle);
						}
					}
					catch
					{
					}
					list2.Add(equipment2);
				}
				MBEquipmentRoster mbequipmentRoster = (MBEquipmentRoster)Activator.CreateInstance(typeof(MBEquipmentRoster), true);
				if (CharacterObjectHelper.F_roster_equipments != null)
				{
					CharacterObjectHelper.F_roster_equipments.SetValue(mbequipmentRoster, new MBList<Equipment>(list2));
				}
				else
				{
					PropertyInfo propertyInfo = AccessTools.Property(typeof(MBEquipmentRoster), "AllEquipments");
					if (propertyInfo != null)
					{
						propertyInfo.SetValue(mbequipmentRoster, new MBReadOnlyList<Equipment>(list2), null);
					}
				}
				if (CharacterObjectHelper.F_roster_default != null)
				{
					CharacterObjectHelper.F_roster_default.SetValue(mbequipmentRoster, list2[0]);
				}
				CharacterObjectHelper.F_equipmentRoster.SetValue(tgt, mbequipmentRoster);
			}
			catch
			{
				try
				{
					CharacterObjectHelper.F_equipmentRoster.SetValue(tgt, (MBEquipmentRoster)Activator.CreateInstance(typeof(MBEquipmentRoster), true));
				}
				catch
				{
				}
			}
		}

		// Token: 0x040001AF RID: 431
		protected static readonly FieldInfo F_originCharacter = AccessTools.Field(typeof(CharacterObject), "_originCharacter");

		// Token: 0x040001B0 RID: 432
		protected static readonly FieldInfo F_occupation = AccessTools.Field(typeof(CharacterObject), "_occupation");

		// Token: 0x040001B1 RID: 433
		protected static readonly FieldInfo F_persona = AccessTools.Field(typeof(CharacterObject), "_persona");

		// Token: 0x040001B2 RID: 434
		protected static readonly FieldInfo F_characterTraits = AccessTools.Field(typeof(CharacterObject), "_characterTraits");

		// Token: 0x040001B3 RID: 435
		protected static readonly FieldInfo F_civilianEquipmentTemplate = AccessTools.Field(typeof(CharacterObject), "_civilianEquipmentTemplate");

		// Token: 0x040001B4 RID: 436
		protected static readonly FieldInfo F_battleEquipmentTemplate = AccessTools.Field(typeof(CharacterObject), "_battleEquipmentTemplate");

		// Token: 0x040001B5 RID: 437
		protected static readonly MethodInfo M_fillFrom = AccessTools.Method(typeof(CharacterObject), "FillFrom", new Type[]
		{
			typeof(CharacterObject)
		}, null);

		// Token: 0x040001B6 RID: 438
		protected static readonly FieldInfo F_equipmentRoster = AccessTools.Field(typeof(BasicCharacterObject), "_equipmentRoster");

		// Token: 0x040001B7 RID: 439
		protected static readonly FieldInfo F_roster_equipments = AccessTools.Field(typeof(MBEquipmentRoster), "_equipments");

		// Token: 0x040001B8 RID: 440
		protected static readonly FieldInfo F_roster_default = AccessTools.Field(typeof(MBEquipmentRoster), "_defaultEquipment");
	}
}
