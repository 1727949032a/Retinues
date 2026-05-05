using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Troop.List
{
	// Token: 0x02000080 RID: 128
	[SafeClass]
	public sealed class TroopListVM : BaseListVM
	{
		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600038C RID: 908 RVA: 0x000144B4 File Offset: 0x000126B4
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Faction] = new string[]
				{
					"RetinueToggleText"
				};
				return dictionary;
			}
		}

		// Token: 0x0600038D RID: 909 RVA: 0x000144DD File Offset: 0x000126DD
		protected override void OnFactionChange()
		{
			this.Build();
		}

		// Token: 0x0600038E RID: 910 RVA: 0x000144E8 File Offset: 0x000126E8
		public void Build()
		{
			MBBindingList<TroopRowVM> mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.RetinueTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.RetinueTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.EliteTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.EliteTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.BasicTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.BasicTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.MilitiaTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.MilitiaTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.CaravanTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.CaravanTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.VillagerTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.VillagerTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.MercenaryTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.MercenaryTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.BanditTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.BanditTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.CivilianTroops
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.CivilianTroops = mbbindingList;
			mbbindingList = new MBBindingList<TroopRowVM>();
			foreach (TroopRowVM item in from t in State.Faction.Heroes
			select new TroopRowVM(t, null))
			{
				mbbindingList.Add(item);
			}
			this.Heroes = mbbindingList;
			foreach (TroopRowVM troopRowVM in this.CivilianTroops)
			{
				troopRowVM.RowTroop.IsCivilian = true;
			}
			foreach (TroopRowVM troopRowVM2 in this.MercenaryTroops)
			{
				troopRowVM2.RowTroop.IsMercenary = true;
			}
			if (this.EliteTroops.Count == 0 && !ClanScreen.IsStudioMode)
			{
				this.EliteTroops.Add(new TroopRowVM(null, L.S("acquire_fief_to_unlock", "Acquire a fief to unlock clan troops.")));
			}
			if (this.BasicTroops.Count == 0 && !ClanScreen.IsStudioMode)
			{
				this.BasicTroops.Add(new TroopRowVM(null, L.S("acquire_fief_to_unlock", "Acquire a fief to unlock clan troops.")));
			}
			if (this.MilitiaTroops.Count == 0 && !ClanScreen.IsStudioMode)
			{
				this.MilitiaTroops.Add(new TroopRowVM(null, L.S("stalwart_militia_to_unlock", "Unlock with the Stalwart Militia doctrine.")));
			}
			if (this.CaravanTroops.Count == 0 && !ClanScreen.IsStudioMode)
			{
				this.CaravanTroops.Add(new TroopRowVM(null, L.S("road_wardens_to_unlock", "Unlock with the Road Wardens doctrine.")));
			}
			if (this.VillagerTroops.Count == 0 && !ClanScreen.IsStudioMode)
			{
				this.VillagerTroops.Add(new TroopRowVM(null, L.S("armed_peasantry_to_unlock", "Unlock with the Armed Peasantry doctrine.")));
			}
			foreach (BaseListElementVM baseListElementVM in this.Rows)
			{
				baseListElementVM.IsVisible = base.IsVisible;
			}
			base.OnPropertyChanged("RetinueTroops");
			base.OnPropertyChanged("EliteTroops");
			base.OnPropertyChanged("BasicTroops");
			base.OnPropertyChanged("MilitiaTroops");
			base.OnPropertyChanged("CaravanTroops");
			base.OnPropertyChanged("VillagerTroops");
			base.OnPropertyChanged("MercenaryTroops");
			base.OnPropertyChanged("BanditTroops");
			base.OnPropertyChanged("CivilianTroops");
			base.OnPropertyChanged("Heroes");
			base.OnPropertyChanged("ShowRetinueList");
			base.OnPropertyChanged("ShowEliteList");
			base.OnPropertyChanged("ShowBasicList");
			base.OnPropertyChanged("ShowMilitiaList");
			base.OnPropertyChanged("ShowCaravanList");
			base.OnPropertyChanged("ShowVillagerList");
			base.OnPropertyChanged("ShowMercenaryList");
			base.OnPropertyChanged("ShowBanditList");
			base.OnPropertyChanged("ShowCivilianList");
			base.OnPropertyChanged("ShowHeroesList");
			this.RefreshFilter();
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600038F RID: 911 RVA: 0x00014C0C File Offset: 0x00012E0C
		// (set) Token: 0x06000390 RID: 912 RVA: 0x00014C14 File Offset: 0x00012E14
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> RetinueTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000391 RID: 913 RVA: 0x00014C1D File Offset: 0x00012E1D
		// (set) Token: 0x06000392 RID: 914 RVA: 0x00014C25 File Offset: 0x00012E25
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> EliteTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000393 RID: 915 RVA: 0x00014C2E File Offset: 0x00012E2E
		// (set) Token: 0x06000394 RID: 916 RVA: 0x00014C36 File Offset: 0x00012E36
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> BasicTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000395 RID: 917 RVA: 0x00014C3F File Offset: 0x00012E3F
		// (set) Token: 0x06000396 RID: 918 RVA: 0x00014C47 File Offset: 0x00012E47
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> MilitiaTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000397 RID: 919 RVA: 0x00014C50 File Offset: 0x00012E50
		// (set) Token: 0x06000398 RID: 920 RVA: 0x00014C58 File Offset: 0x00012E58
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> CaravanTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000399 RID: 921 RVA: 0x00014C61 File Offset: 0x00012E61
		// (set) Token: 0x0600039A RID: 922 RVA: 0x00014C69 File Offset: 0x00012E69
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> VillagerTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600039B RID: 923 RVA: 0x00014C72 File Offset: 0x00012E72
		// (set) Token: 0x0600039C RID: 924 RVA: 0x00014C7A File Offset: 0x00012E7A
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> MercenaryTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600039D RID: 925 RVA: 0x00014C83 File Offset: 0x00012E83
		// (set) Token: 0x0600039E RID: 926 RVA: 0x00014C8B File Offset: 0x00012E8B
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> BanditTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00014C94 File Offset: 0x00012E94
		// (set) Token: 0x060003A0 RID: 928 RVA: 0x00014C9C File Offset: 0x00012E9C
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> CivilianTroops { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00014CA5 File Offset: 0x00012EA5
		// (set) Token: 0x060003A2 RID: 930 RVA: 0x00014CAD File Offset: 0x00012EAD
		[DataSourceProperty]
		public MBBindingList<TroopRowVM> Heroes { get; set; } = new MBBindingList<TroopRowVM>();

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x00014CB6 File Offset: 0x00012EB6
		[DataSourceProperty]
		public string RegularToggleText
		{
			get
			{
				return L.S("list_toggle_regular", "Regular");
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x00014CC7 File Offset: 0x00012EC7
		[DataSourceProperty]
		public string EliteToggleText
		{
			get
			{
				return L.S("list_toggle_elite", "Elite");
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x00014CD8 File Offset: 0x00012ED8
		[DataSourceProperty]
		public string RetinueToggleText
		{
			get
			{
				if (!(State.Faction == Player.Kingdom))
				{
					return L.S("retinue", "Retinue");
				}
				if (!Player.IsFemale)
				{
					return L.S("king_guard", "King's Guard");
				}
				return L.S("queen_guard", "Queen's Guard");
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00014D2C File Offset: 0x00012F2C
		[DataSourceProperty]
		public string MilitiaToggleText
		{
			get
			{
				return L.S("list_toggle_militia", "Militia");
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x00014D3D File Offset: 0x00012F3D
		[DataSourceProperty]
		public string CaravanToggleText
		{
			get
			{
				return L.S("list_toggle_caravan", "Caravans");
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00014D4E File Offset: 0x00012F4E
		[DataSourceProperty]
		public string VillagerToggleText
		{
			get
			{
				return L.S("list_toggle_villager", "Villagers");
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x00014D5F File Offset: 0x00012F5F
		[DataSourceProperty]
		public string MercenaryToggleText
		{
			get
			{
				return L.S("list_toggle_mercenary", "Mercenaries");
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00014D70 File Offset: 0x00012F70
		[DataSourceProperty]
		public string BanditToggleText
		{
			get
			{
				return L.S("list_toggle_bandit", "Bandits");
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060003AB RID: 939 RVA: 0x00014D81 File Offset: 0x00012F81
		[DataSourceProperty]
		public string CivilianToggleText
		{
			get
			{
				return L.S("list_toggle_civilian", "Civilians");
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060003AC RID: 940 RVA: 0x00014D92 File Offset: 0x00012F92
		[DataSourceProperty]
		public string HeroToggleText
		{
			get
			{
				return L.S("list_toggle_hero", "Heroes");
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00014DA3 File Offset: 0x00012FA3
		[DataSourceProperty]
		public bool ShowRetinueList
		{
			get
			{
				return this.RetinueTroops.Count > 0 || !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060003AE RID: 942 RVA: 0x00014DBD File Offset: 0x00012FBD
		[DataSourceProperty]
		public bool ShowEliteList
		{
			get
			{
				return this.EliteTroops.Count > 0 || ClanScreen.EditorMode != EditorMode.Heroes;
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00014DDA File Offset: 0x00012FDA
		[DataSourceProperty]
		public bool ShowBasicList
		{
			get
			{
				return this.BasicTroops.Count > 0 || ClanScreen.EditorMode != EditorMode.Heroes;
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x00014DF7 File Offset: 0x00012FF7
		[DataSourceProperty]
		public bool ShowMilitiaList
		{
			get
			{
				return this.MilitiaTroops.Count > 0 || !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x00014E11 File Offset: 0x00013011
		[DataSourceProperty]
		public bool ShowCaravanList
		{
			get
			{
				return this.CaravanTroops.Count > 0 || !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00014E2B File Offset: 0x0001302B
		[DataSourceProperty]
		public bool ShowVillagerList
		{
			get
			{
				return this.VillagerTroops.Count > 0 || !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x00014E45 File Offset: 0x00013045
		[DataSourceProperty]
		public bool ShowMercenaryList
		{
			get
			{
				return this.MercenaryTroops.Count > 0 && ClanScreen.EditorMode == EditorMode.Culture;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00014E5F File Offset: 0x0001305F
		[DataSourceProperty]
		public bool ShowBanditList
		{
			get
			{
				return this.BanditTroops.Count > 0 && ClanScreen.EditorMode == EditorMode.Culture;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x00014E79 File Offset: 0x00013079
		[DataSourceProperty]
		public bool ShowCivilianList
		{
			get
			{
				return this.CivilianTroops.Count > 0 && ClanScreen.EditorMode == EditorMode.Culture;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00014E93 File Offset: 0x00013093
		[DataSourceProperty]
		public bool ShowHeroesList
		{
			get
			{
				return this.Heroes.Count > 0 && ClanScreen.EditorMode == EditorMode.Heroes;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x00014EB0 File Offset: 0x000130B0
		public override List<BaseListElementVM> Rows
		{
			get
			{
				List<BaseListElementVM> list = new List<BaseListElementVM>();
				list.AddRange(this.RetinueTroops);
				list.AddRange(this.EliteTroops);
				list.AddRange(this.BasicTroops);
				list.AddRange(this.MilitiaTroops);
				list.AddRange(this.CaravanTroops);
				list.AddRange(this.VillagerTroops);
				list.AddRange(this.MercenaryTroops);
				list.AddRange(this.BanditTroops);
				list.AddRange(this.CivilianTroops);
				list.AddRange(this.Heroes);
				return list;
			}
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00014F3A File Offset: 0x0001313A
		public override void Show()
		{
			base.Show();
			this.Build();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00014F48 File Offset: 0x00013148
		public override void Hide()
		{
			foreach (BaseListElementVM baseListElementVM in this.Rows)
			{
				baseListElementVM.Hide();
			}
			base.Hide();
		}
	}
}
