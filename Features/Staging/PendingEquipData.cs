using System;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Retinues.Features.Staging
{
	// Token: 0x020000C0 RID: 192
	[Serializable]
	public class PendingEquipData : IPendingData
	{
		// Token: 0x1700036C RID: 876
		// (get) Token: 0x060007CE RID: 1998 RVA: 0x00027ACB File Offset: 0x00025CCB
		// (set) Token: 0x060007CF RID: 1999 RVA: 0x00027AD3 File Offset: 0x00025CD3
		public EquipmentIndex Slot
		{
			get
			{
				return (EquipmentIndex)this.SlotValue;
			}
			set
			{
				this.SlotValue = (int)value;
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00027ADC File Offset: 0x00025CDC
		// (set) Token: 0x060007D1 RID: 2001 RVA: 0x00027AE4 File Offset: 0x00025CE4
		string IPendingData.TroopId
		{
			get
			{
				return this.TroopId;
			}
			set
			{
				this.TroopId = value;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x060007D2 RID: 2002 RVA: 0x00027AED File Offset: 0x00025CED
		// (set) Token: 0x060007D3 RID: 2003 RVA: 0x00027AF5 File Offset: 0x00025CF5
		int IPendingData.Remaining
		{
			get
			{
				return this.Remaining;
			}
			set
			{
				this.Remaining = value;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x00027AFE File Offset: 0x00025CFE
		// (set) Token: 0x060007D5 RID: 2005 RVA: 0x00027B06 File Offset: 0x00025D06
		float IPendingData.Carry
		{
			get
			{
				return this.Carry;
			}
			set
			{
				this.Carry = value;
			}
		}

		// Token: 0x04000206 RID: 518
		[SaveableField(1)]
		public string TroopId;

		// Token: 0x04000207 RID: 519
		[SaveableField(2)]
		public int Remaining;

		// Token: 0x04000208 RID: 520
		[SaveableField(3)]
		public float Carry;

		// Token: 0x04000209 RID: 521
		[SaveableField(4)]
		public string ItemId;

		// Token: 0x0400020A RID: 522
		[SaveableField(5)]
		public int SlotValue;

		// Token: 0x0400020B RID: 523
		[SaveableField(6)]
		public int CategoryValue;

		// Token: 0x0400020C RID: 524
		[SaveableField(7)]
		public int EquipmentIndex;
	}
}
