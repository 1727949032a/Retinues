using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Game
{
	// Token: 0x0200008C RID: 140
	public abstract class BaseFaction : StringIdentifier
	{
		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060004D4 RID: 1236 RVA: 0x0001A819 File Offset: 0x00018A19
		// (set) Token: 0x060004D5 RID: 1237 RVA: 0x0001A820 File Offset: 0x00018A20
		public static int TroopFactionMapVersion { get; set; }

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001A828 File Offset: 0x00018A28
		internal static void TouchTroopFactionMap()
		{
			BaseFaction.TroopFactionMapVersion++;
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060004D7 RID: 1239
		public abstract override string StringId { get; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060004D8 RID: 1240
		public abstract string Name { get; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060004D9 RID: 1241
		public abstract uint Color { get; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060004DA RID: 1242
		public abstract uint Color2 { get; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x0001A838 File Offset: 0x00018A38
		public WCulture Culture
		{
			get
			{
				WCulture wculture = this as WCulture;
				if (wculture != null)
				{
					return wculture;
				}
				WClan wclan = this as WClan;
				if (wclan != null)
				{
					return new WCulture(wclan.Base.Culture);
				}
				WFaction wfaction = this as WFaction;
				if (wfaction != null)
				{
					return new WCulture(wfaction.Base.Culture);
				}
				return null;
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001A888 File Offset: 0x00018A88
		public WCharacter GetRoot(RootCategory category)
		{
			WCharacter result;
			switch (category)
			{
			case RootCategory.RetinueBasic:
				result = this.RetinueBasic;
				break;
			case RootCategory.RetinueElite:
				result = this.RetinueElite;
				break;
			case RootCategory.RootBasic:
				result = this.RootBasic;
				break;
			case RootCategory.RootElite:
				result = this.RootElite;
				break;
			case RootCategory.MilitiaMelee:
				result = this.MilitiaMelee;
				break;
			case RootCategory.MilitiaMeleeElite:
				result = this.MilitiaMeleeElite;
				break;
			case RootCategory.MilitiaRanged:
				result = this.MilitiaRanged;
				break;
			case RootCategory.MilitiaRangedElite:
				result = this.MilitiaRangedElite;
				break;
			case RootCategory.CaravanGuard:
				result = this.CaravanGuard;
				break;
			case RootCategory.CaravanMaster:
				result = this.CaravanMaster;
				break;
			case RootCategory.Villager:
				result = this.Villager;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001A930 File Offset: 0x00018B30
		public void SetRoot(RootCategory category, WCharacter troop)
		{
			switch (category)
			{
			case RootCategory.RetinueBasic:
				this.RetinueBasic = troop;
				break;
			case RootCategory.RetinueElite:
				this.RetinueElite = troop;
				break;
			case RootCategory.RootBasic:
				this.RootBasic = troop;
				break;
			case RootCategory.RootElite:
				this.RootElite = troop;
				break;
			case RootCategory.MilitiaMelee:
				this.MilitiaMelee = troop;
				break;
			case RootCategory.MilitiaMeleeElite:
				this.MilitiaMeleeElite = troop;
				break;
			case RootCategory.MilitiaRanged:
				this.MilitiaRanged = troop;
				break;
			case RootCategory.MilitiaRangedElite:
				this.MilitiaRangedElite = troop;
				break;
			case RootCategory.CaravanGuard:
				this.CaravanGuard = troop;
				break;
			case RootCategory.CaravanMaster:
				this.CaravanMaster = troop;
				break;
			case RootCategory.Villager:
				this.Villager = troop;
				break;
			}
			this.InvalidateCategoryCache();
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x0001A9D8 File Offset: 0x00018BD8
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x0001A9E0 File Offset: 0x00018BE0
		public virtual WCharacter RetinueElite { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x0001A9E9 File Offset: 0x00018BE9
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x0001A9F1 File Offset: 0x00018BF1
		public virtual WCharacter RetinueBasic { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x0001A9FA File Offset: 0x00018BFA
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x0001AA02 File Offset: 0x00018C02
		public virtual WCharacter RootElite { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0001AA0B File Offset: 0x00018C0B
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x0001AA13 File Offset: 0x00018C13
		public virtual WCharacter RootBasic { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0001AA1C File Offset: 0x00018C1C
		// (set) Token: 0x060004E7 RID: 1255 RVA: 0x0001AA24 File Offset: 0x00018C24
		public virtual WCharacter MilitiaMelee { get; set; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x0001AA2D File Offset: 0x00018C2D
		// (set) Token: 0x060004E9 RID: 1257 RVA: 0x0001AA35 File Offset: 0x00018C35
		public virtual WCharacter MilitiaMeleeElite { get; set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x0001AA3E File Offset: 0x00018C3E
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x0001AA46 File Offset: 0x00018C46
		public virtual WCharacter MilitiaRanged { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x0001AA4F File Offset: 0x00018C4F
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x0001AA57 File Offset: 0x00018C57
		public virtual WCharacter MilitiaRangedElite { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x0001AA60 File Offset: 0x00018C60
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x0001AA68 File Offset: 0x00018C68
		public virtual WCharacter CaravanGuard { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x0001AA71 File Offset: 0x00018C71
		// (set) Token: 0x060004F1 RID: 1265 RVA: 0x0001AA79 File Offset: 0x00018C79
		public virtual WCharacter CaravanMaster { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x0001AA82 File Offset: 0x00018C82
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x0001AA8A File Offset: 0x00018C8A
		public virtual WCharacter Villager { get; set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060004F4 RID: 1268 RVA: 0x0001AA93 File Offset: 0x00018C93
		public IEnumerable<WCharacter> Troops
		{
			get
			{
				BaseFaction.<get_Troops>d__63 <get_Troops>d__ = new BaseFaction.<get_Troops>d__63(-2);
				<get_Troops>d__.<>4__this = this;
				return <get_Troops>d__;
			}
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001AAA3 File Offset: 0x00018CA3
		protected List<WCharacter> GetActiveList(List<WCharacter> list)
		{
			if (list != null)
			{
				return (from t in list
				where t != null && t.IsActive && t != null && t.Body.Age >= (float)18
				select t).ToList<WCharacter>();
			}
			return new List<WCharacter>();
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001AAD8 File Offset: 0x00018CD8
		protected List<WCharacter> GetActiveList(List<CharacterObject> list)
		{
			return this.GetActiveList((from t in list
			where t != null
			select new WCharacter(t)).ToList<WCharacter>());
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0001AB39 File Offset: 0x00018D39
		public List<WCharacter> RetinueTroops
		{
			get
			{
				return this.GetActiveList(new List<WCharacter>(2)
				{
					this.RetinueElite,
					this.RetinueBasic
				});
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x0001AB60 File Offset: 0x00018D60
		public List<WCharacter> RegularTroops
		{
			get
			{
				List<WCharacter> eliteTroops = this.EliteTroops;
				List<WCharacter> basicTroops = this.BasicTroops;
				List<WCharacter> list = new List<WCharacter>(eliteTroops.Count + basicTroops.Count);
				list.AddRange(eliteTroops);
				list.AddRange(basicTroops);
				return list;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x0001AB9B File Offset: 0x00018D9B
		public List<WCharacter> EliteTroops
		{
			get
			{
				if (!(this.RootElite != null))
				{
					return new List<WCharacter>();
				}
				return this.GetActiveList(this.RootElite.Tree.ToList<WCharacter>());
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x0001ABC7 File Offset: 0x00018DC7
		public List<WCharacter> BasicTroops
		{
			get
			{
				if (!(this.RootBasic != null))
				{
					return new List<WCharacter>();
				}
				return this.GetActiveList(this.RootBasic.Tree.ToList<WCharacter>());
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x0001ABF3 File Offset: 0x00018DF3
		public List<WCharacter> MilitiaTroops
		{
			get
			{
				return this.GetActiveList(new List<WCharacter>(4)
				{
					this.MilitiaMelee,
					this.MilitiaMeleeElite,
					this.MilitiaRanged,
					this.MilitiaRangedElite
				});
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x0001AC31 File Offset: 0x00018E31
		public List<WCharacter> CaravanTroops
		{
			get
			{
				return this.GetActiveList(new List<WCharacter>(2)
				{
					this.CaravanGuard,
					this.CaravanMaster
				});
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x0001AC57 File Offset: 0x00018E57
		public List<WCharacter> VillagerTroops
		{
			get
			{
				return this.GetActiveList(new List<WCharacter>(1)
				{
					this.Villager
				});
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060004FE RID: 1278 RVA: 0x0001AC71 File Offset: 0x00018E71
		public virtual List<WCharacter> MercenaryTroops
		{
			get
			{
				return new List<WCharacter>();
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x0001AC78 File Offset: 0x00018E78
		public virtual List<WCharacter> BanditTroops
		{
			get
			{
				return new List<WCharacter>();
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000500 RID: 1280 RVA: 0x0001AC7F File Offset: 0x00018E7F
		public virtual List<WCharacter> CivilianTroops
		{
			get
			{
				return new List<WCharacter>();
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x0001AC86 File Offset: 0x00018E86
		public virtual List<WHero> Heroes
		{
			get
			{
				return new List<WHero>();
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001AC8D File Offset: 0x00018E8D
		public void InvalidateCategoryCache()
		{
			this._categoryCacheDirty = true;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001AC98 File Offset: 0x00018E98
		private void EnsureCategoryCache()
		{
			if (!this._categoryCacheDirty)
			{
				return;
			}
			if (!(this is WFaction))
			{
				return;
			}
			this._categoryCacheDirty = false;
			this._retinueIds.Clear();
			this._regularIds.Clear();
			this._eliteIds.Clear();
			foreach (WCharacter wcharacter in this.GetActiveList(new List<WCharacter>(2)
			{
				this.RetinueElite,
				this.RetinueBasic
			}))
			{
				this._retinueIds.Add(wcharacter.StringId);
			}
			if (this.RetinueElite != null && this.RetinueElite.IsActive)
			{
				this._eliteIds.Add(this.RetinueElite.StringId);
			}
			List<WCharacter> list = (this.RootElite != null) ? this.GetActiveList(this.RootElite.Tree.ToList<WCharacter>()) : new List<WCharacter>();
			List<WCharacter> list2 = (this.RootBasic != null) ? this.GetActiveList(this.RootBasic.Tree.ToList<WCharacter>()) : new List<WCharacter>();
			foreach (WCharacter wcharacter2 in list)
			{
				this._eliteIds.Add(wcharacter2.StringId);
				this._regularIds.Add(wcharacter2.StringId);
			}
			foreach (WCharacter wcharacter3 in list2)
			{
				this._regularIds.Add(wcharacter3.StringId);
			}
			foreach (WCharacter wcharacter4 in this.GetActiveList(new List<WCharacter>(4)
			{
				this.MilitiaMelee,
				this.MilitiaMeleeElite,
				this.MilitiaRanged,
				this.MilitiaRangedElite
			}))
			{
				if (wcharacter4 == this.MilitiaMeleeElite || wcharacter4 == this.MilitiaRangedElite)
				{
					this._eliteIds.Add(wcharacter4.StringId);
				}
			}
			if (this.CaravanMaster != null && this.CaravanMaster.IsActive)
			{
				this._eliteIds.Add(this.CaravanMaster.StringId);
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001AF50 File Offset: 0x00019150
		public bool IsRetinueId(string troopId)
		{
			if (string.IsNullOrWhiteSpace(troopId))
			{
				return false;
			}
			if (this is WFaction)
			{
				this.EnsureCategoryCache();
				return this._retinueIds.Contains(troopId);
			}
			return (this.RetinueElite != null && this.RetinueElite.IsActive && this.RetinueElite.StringId == troopId) || (this.RetinueBasic != null && this.RetinueBasic.IsActive && this.RetinueBasic.StringId == troopId);
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001AFE0 File Offset: 0x000191E0
		public bool IsRetinue(WCharacter troop)
		{
			return !(troop == null) && !troop.IsHero && !troop.IsVanilla && this.IsRetinueId(troop.StringId);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001B00B File Offset: 0x0001920B
		public bool IsRegular(WCharacter troop)
		{
			if (troop == null || troop.IsHero)
			{
				return false;
			}
			if (troop.IsVanilla)
			{
				return true;
			}
			this.EnsureCategoryCache();
			return this._regularIds.Contains(troop.StringId);
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001B044 File Offset: 0x00019244
		public bool IsElite(WCharacter troop)
		{
			if (troop == null || troop.IsHero)
			{
				return false;
			}
			if (troop.IsVanilla)
			{
				return this.EliteTroops.Contains(troop);
			}
			this.EnsureCategoryCache();
			return this._eliteIds.Contains(troop.StringId);
		}

		// Token: 0x04000145 RID: 325
		public static Dictionary<string, BaseFaction> TroopFactionMap = new Dictionary<string, BaseFaction>();

		// Token: 0x04000152 RID: 338
		private bool _categoryCacheDirty = true;

		// Token: 0x04000153 RID: 339
		private readonly HashSet<string> _retinueIds = new HashSet<string>(StringComparer.Ordinal);

		// Token: 0x04000154 RID: 340
		private readonly HashSet<string> _regularIds = new HashSet<string>(StringComparer.Ordinal);

		// Token: 0x04000155 RID: 341
		private readonly HashSet<string> _eliteIds = new HashSet<string>(StringComparer.Ordinal);
	}
}
