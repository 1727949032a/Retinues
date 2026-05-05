using System;
using System.Collections.Generic;
using Retinues.Configuration;
using TaleWorlds.Localization;

namespace Retinues.Doctrines.Model
{
	// Token: 0x020000D8 RID: 216
	public abstract class Doctrine
	{
		// Token: 0x1700038B RID: 907
		// (get) Token: 0x060008CA RID: 2250
		public abstract TextObject Name { get; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x060008CB RID: 2251
		public abstract TextObject Description { get; }

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x0002C7F1 File Offset: 0x0002A9F1
		public virtual int Column
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x0002C7F4 File Offset: 0x0002A9F4
		public virtual int Row
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x0002C7F7 File Offset: 0x0002A9F7
		public virtual bool IsDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x0002C7FA File Offset: 0x0002A9FA
		public virtual TextObject DisabledMessage
		{
			get
			{
				return this.Description;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x0002C804 File Offset: 0x0002AA04
		public int GoldCost
		{
			get
			{
				int num;
				switch (this.Row)
				{
				case 0:
					num = 1000;
					break;
				case 1:
					num = 5000;
					break;
				case 2:
					num = 25000;
					break;
				case 3:
					num = 100000;
					break;
				default:
					num = 0;
					break;
				}
				return (int)((float)num * Config.DoctrineGoldCostMultiplier);
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0002C860 File Offset: 0x0002AA60
		public int InfluenceCost
		{
			get
			{
				int num;
				switch (this.Row)
				{
				case 0:
					num = 50;
					break;
				case 1:
					num = 100;
					break;
				case 2:
					num = 200;
					break;
				case 3:
					num = 500;
					break;
				default:
					num = 0;
					break;
				}
				return (int)((float)num * Config.DoctrineInfluenceCostMultiplier);
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x0002C8B6 File Offset: 0x0002AAB6
		public string Key
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x0002C8C3 File Offset: 0x0002AAC3
		public virtual IEnumerable<Feat> InstantiateFeats()
		{
			Doctrine.<InstantiateFeats>d__18 <InstantiateFeats>d__ = new Doctrine.<InstantiateFeats>d__18(-2);
			<InstantiateFeats>d__.<>4__this = this;
			return <InstantiateFeats>d__;
		}
	}
}
