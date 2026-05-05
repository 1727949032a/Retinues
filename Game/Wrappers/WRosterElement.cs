using System;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.Roster;

namespace Retinues.Game.Wrappers
{
	// Token: 0x0200009C RID: 156
	[SafeClass]
	public class WRosterElement
	{
		// Token: 0x060006CC RID: 1740 RVA: 0x00021E7C File Offset: 0x0002007C
		public WRosterElement(TroopRosterElement element, WRoster roster, int index)
		{
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x00021EAA File Offset: 0x000200AA
		public TroopRosterElement Base
		{
			get
			{
				return this._element;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060006CE RID: 1742 RVA: 0x00021EB2 File Offset: 0x000200B2
		public WRoster Roster
		{
			get
			{
				return this._roster;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x00021EBA File Offset: 0x000200BA
		public WCharacter Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x00021EC2 File Offset: 0x000200C2
		public int Index
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00021ECA File Offset: 0x000200CA
		public int Number
		{
			get
			{
				return this._roster.Base.GetElementNumber(this._index);
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00021EE2 File Offset: 0x000200E2
		public int WoundedNumber
		{
			get
			{
				return this._roster.Base.GetElementWoundedNumber(this._index);
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00021EFA File Offset: 0x000200FA
		public int Xp
		{
			get
			{
				return this._roster.Base.GetElementXp(this._index);
			}
		}

		// Token: 0x0400019B RID: 411
		private readonly int _index = index;

		// Token: 0x0400019C RID: 412
		private readonly TroopRosterElement _element = element;

		// Token: 0x0400019D RID: 413
		private readonly WRoster _roster = roster;

		// Token: 0x0400019E RID: 414
		private readonly WCharacter _troop = new WCharacter(element.Character);
	}
}
