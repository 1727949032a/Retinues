using System;
using System.Collections.Generic;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Troop.Panel
{
	// Token: 0x0200007F RID: 127
	[SafeClass]
	public sealed class TroopUpgradeTargetVM : BaseVM
	{
		// Token: 0x06000388 RID: 904 RVA: 0x000143D0 File Offset: 0x000125D0
		public TroopUpgradeTargetVM(WCharacter upgradeTarget)
		{
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000389 RID: 905 RVA: 0x000143E0 File Offset: 0x000125E0
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"Name"
				};
				dictionary[UIEvent.Equip] = new string[]
				{
					"FormationClassIcon"
				};
				return dictionary;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600038A RID: 906 RVA: 0x0001441E File Offset: 0x0001261E
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return Format.Crop(this.UpgradeTarget.Name, 40);
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00014434 File Offset: 0x00012634
		[DataSourceProperty]
		public string FormationClassIcon
		{
			get
			{
				WCharacter upgradeTarget = this.UpgradeTarget;
				FormationClass? formationClass = (upgradeTarget != null) ? new FormationClass?(upgradeTarget.FormationClass) : null;
				if (formationClass != null)
				{
					switch (formationClass.GetValueOrDefault())
					{
					case FormationClass.Infantry:
						return "General\\TroopTypeIcons\\icon_troop_type_infantry";
					case FormationClass.Ranged:
						return "General\\TroopTypeIcons\\icon_troop_type_bow";
					case FormationClass.Cavalry:
						return "General\\TroopTypeIcons\\icon_troop_type_cavalry";
					case FormationClass.HorseArcher:
						return "General\\TroopTypeIcons\\icon_troop_type_horse_archer";
					}
				}
				return "General\\TroopTypeIcons\\icon_troop_type_infantry";
			}
		}

		// Token: 0x040000F4 RID: 244
		private readonly WCharacter UpgradeTarget = upgradeTarget;
	}
}
