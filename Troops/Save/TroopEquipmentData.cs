using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Game.Wrappers;
using TaleWorlds.SaveSystem;

namespace Retinues.Troops.Save
{
	// Token: 0x0200003F RID: 63
	public class TroopEquipmentData
	{
		// Token: 0x06000143 RID: 323 RVA: 0x000092C4 File Offset: 0x000074C4
		public TroopEquipmentData(List<WEquipment> equipments)
		{
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00009337 File Offset: 0x00007537
		public TroopEquipmentData() : this(new List<WEquipment>())
		{
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00009344 File Offset: 0x00007544
		public List<WEquipment> Deserialize(WCharacter owner)
		{
			List<string> codes = this.Codes;
			List<WEquipment> list = new List<WEquipment>((codes != null) ? codes.Count : 0);
			bool flag;
			if (this.Civilians != null)
			{
				int count = this.Civilians.Count;
				List<string> codes2 = this.Codes;
				int? num = (codes2 != null) ? new int?(codes2.Count) : null;
				flag = (count == num.GetValueOrDefault() & num != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			int num2 = 0;
			for (;;)
			{
				int num3 = num2;
				List<string> codes3 = this.Codes;
				if (num3 >= ((codes3 != null) ? codes3.Count : 0))
				{
					break;
				}
				string code = this.Codes[num2];
				bool? forceCivilian;
				if (flag2)
				{
					forceCivilian = new bool?(this.Civilians[num2]);
				}
				else
				{
					forceCivilian = new bool?(num2 == 1);
				}
				WEquipment item = WEquipment.FromCode(code, owner.Loadout, num2, forceCivilian);
				list.Add(item);
				num2++;
			}
			return list;
		}

		// Token: 0x04000084 RID: 132
		[SaveableField(1)]
		public List<string> Codes = (from we in equipments
		select we.Code).ToList<string>();

		// Token: 0x04000085 RID: 133
		[SaveableField(2)]
		public List<bool> Civilians = (from we in equipments
		select we.IsCivilian).ToList<bool>();
	}
}
