using System;
using Retinues.Game.Wrappers;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Localization;

namespace Retinues.GUI.Helpers
{
	// Token: 0x02000064 RID: 100
	[SafeClass]
	public static class Icons
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x0000E730 File Offset: 0x0000C930
		public static string GetFormationClassIcon(WCharacter troop)
		{
			FormationClass? formationClass = (troop != null) ? new FormationClass?(troop.FormationClass) : null;
			string text;
			if (formationClass != null)
			{
				switch (formationClass.GetValueOrDefault())
				{
				case FormationClass.Infantry:
					text = "General\\TroopTypeIcons\\icon_troop_type_infantry";
					goto IL_B5;
				case FormationClass.Ranged:
					text = "General\\TroopTypeIcons\\icon_troop_type_bow";
					goto IL_B5;
				case FormationClass.Cavalry:
					text = "General\\TroopTypeIcons\\icon_troop_type_cavalry";
					goto IL_B5;
				case FormationClass.HorseArcher:
					text = "General\\TroopTypeIcons\\icon_troop_type_horse_archer";
					goto IL_B5;
				case FormationClass.NumberOfDefaultFormations:
					text = "General\\TroopTypeIcons\\icon_troop_type_bow";
					goto IL_B5;
				case FormationClass.HeavyInfantry:
					text = "General\\TroopTypeIcons\\icon_troop_type_infantry";
					goto IL_B5;
				case FormationClass.LightCavalry:
					text = "General\\TroopTypeIcons\\icon_troop_type_cavalry";
					goto IL_B5;
				case FormationClass.HeavyCavalry:
					text = "General\\TroopTypeIcons\\icon_troop_type_cavalry";
					goto IL_B5;
				case FormationClass.NumberOfRegularFormations:
					text = "General\\TroopTypeIcons\\icon_troop_type_cavalry";
					goto IL_B5;
				case FormationClass.Bodyguard:
					text = "General\\TroopTypeIcons\\icon_troop_type_infantry";
					goto IL_B5;
				}
			}
			text = "General\\TroopTypeIcons\\icon_troop_type_infantry";
			IL_B5:
			string text2 = text;
			if (ModCompatibility.HasNavalDLC && troop != null && troop.IsMariner && (troop.FormationClass == FormationClass.Infantry || troop.FormationClass == FormationClass.Ranged))
			{
				text2 += "_mariner_big";
			}
			return text2;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000E82A File Offset: 0x0000CA2A
		public static StringItemWithHintVM GetTierIconData(WCharacter troop)
		{
			if (troop == null)
			{
				return new StringItemWithHintVM(string.Empty, new TextObject(string.Empty, null));
			}
			return CampaignUIHelper.GetCharacterTierData(troop.Base, true);
		}
	}
}
