using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Statistics;
using Retinues.Game.Helpers;
using Retinues.Mods;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000090 RID: 144
	[SafeClass]
	public class WCharacter : BaseFactionMember
	{
		// Token: 0x06000541 RID: 1345 RVA: 0x0001C2B0 File Offset: 0x0001A4B0
		public WCharacter(CharacterObject characterObject)
		{
			if (characterObject == null)
			{
				throw new ArgumentNullException("characterObject");
			}
			this._co = characterObject;
			this._stringId = characterObject.StringId;
			this._isLegacyCustom = characterObject.StringId.StartsWith("ret_");
			this._isCustom = (characterObject.StringId.StartsWith("retinues_custom_") || characterObject.StringId.StartsWith("ret_"));
			this._factionCachedVersion = -1;
			this._formationClassOverride = FormationClass.NumberOfAllFormations;
			this.CombatSkills = new List<SkillObject>(8)
			{
				DefaultSkills.Athletics,
				DefaultSkills.Riding,
				DefaultSkills.OneHanded,
				DefaultSkills.TwoHanded,
				DefaultSkills.Polearm,
				DefaultSkills.Bow,
				DefaultSkills.Crossbow,
				DefaultSkills.Throwing
			};
			this.HeroSkills = new List<SkillObject>(10)
			{
				DefaultSkills.Crafting,
				DefaultSkills.Scouting,
				DefaultSkills.Tactics,
				DefaultSkills.Roguery,
				DefaultSkills.Charm,
				DefaultSkills.Leadership,
				DefaultSkills.Trade,
				DefaultSkills.Steward,
				DefaultSkills.Medicine,
				DefaultSkills.Engineering
			};
			base..ctor();
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001C414 File Offset: 0x0001A614
		public static WCharacter FromStringId(string stringId)
		{
			return new WCharacter(MBObjectManager.Instance.GetObject<CharacterObject>(stringId));
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x0001C426 File Offset: 0x0001A626
		public static List<string> ActiveStubIds { get; } = new List<string>();

		// Token: 0x06000544 RID: 1348 RVA: 0x0001C430 File Offset: 0x0001A630
		public static CharacterObject AllocateStub(string stringId = null)
		{
			if (!string.IsNullOrWhiteSpace(stringId))
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(stringId);
				if (@object != null && @object.StringId.StartsWith("retinues_custom_") && !WCharacter.ActiveStubIds.Contains(@object.StringId))
				{
					return @object;
				}
			}
			foreach (CharacterObject characterObject in MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
			{
				if (characterObject.StringId.StartsWith("retinues_custom_") && !WCharacter.ActiveStubIds.Contains(characterObject.StringId))
				{
					return characterObject;
				}
			}
			throw new InvalidOperationException("No free stub CharacterObject available.");
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001C4F4 File Offset: 0x0001A6F4
		private static string NullifyLegacyIds(string stringId)
		{
			if (stringId == null || !stringId.StartsWith("ret_"))
			{
				return stringId;
			}
			return null;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001C509 File Offset: 0x0001A709
		public WCharacter(WFaction faction, RootCategory category, string stringId = null) : this(WCharacter.NullifyLegacyIds(stringId) ?? WCharacter.AllocateStub(stringId).StringId)
		{
			faction.SetRoot(category, this);
			this.Initialize(faction);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001C535 File Offset: 0x0001A735
		public WCharacter(WCharacter parent, string stringId = null) : this(WCharacter.NullifyLegacyIds(stringId) ?? WCharacter.AllocateStub(stringId).StringId)
		{
			this.Initialize(parent.Faction);
			this.Parent = parent;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001C565 File Offset: 0x0001A765
		private void Initialize(BaseFaction faction)
		{
			WCharacter.ActiveStubIds.Add(this.StringId);
			if (faction != null)
			{
				faction.InvalidateCategoryCache();
			}
			this.Faction = faction;
			this.HiddenInEncyclopedia = false;
			this.IsNotTransferableInHideouts = false;
			this.IsNotTransferableInPartyScreen = false;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001C59C File Offset: 0x0001A79C
		public void ComputeDerivedProperties()
		{
			this.FormationClass = this.ComputeFormationClass();
			this.UpgradeItemRequirement = this.Loadout.ComputeUpgradeItemRequirement();
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001C5BB File Offset: 0x0001A7BB
		public WCharacter(string stringId) : this(MBObjectManager.Instance.GetObject<CharacterObject>(stringId))
		{
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x0001C5CE File Offset: 0x0001A7CE
		public static HashSet<string> EditedVanillaRootIds { get; } = new HashSet<string>(StringComparer.Ordinal);

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x0001C5D5 File Offset: 0x0001A7D5
		// (set) Token: 0x0600054D RID: 1357 RVA: 0x0001C600 File Offset: 0x0001A800
		public bool NeedsPersistence
		{
			get
			{
				return this.IsCustom || (this.IsVanilla && WCharacter.EditedVanillaRootIds.Contains(this.Root.StringId));
			}
			set
			{
				if (this.IsCustom)
				{
					return;
				}
				if (value)
				{
					WCharacter.EditedVanillaRootIds.Add(this.Root.StringId);
					return;
				}
				WCharacter.EditedVanillaRootIds.Remove(this.Root.StringId);
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001C63B File Offset: 0x0001A83B
		public CharacterObject Base
		{
			get
			{
				return this._co;
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x0001C643 File Offset: 0x0001A843
		public override string StringId
		{
			get
			{
				return this._stringId;
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x0001C64C File Offset: 0x0001A84C
		public string VanillaStringId
		{
			get
			{
				string result;
				if (!WCharacter.VanillaStringIdMap.TryGetValue(this.StringId, out result))
				{
					return this.StringId;
				}
				return result;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000551 RID: 1361 RVA: 0x0001C675 File Offset: 0x0001A875
		public bool IsCustom
		{
			get
			{
				return this._isCustom;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x0001C67D File Offset: 0x0001A87D
		public bool IsLegacyCustom
		{
			get
			{
				return this._isLegacyCustom;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000553 RID: 1363 RVA: 0x0001C685 File Offset: 0x0001A885
		public bool IsVanilla
		{
			get
			{
				return !this._isCustom;
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0001C690 File Offset: 0x0001A890
		public bool IsRetinue
		{
			get
			{
				return this.Faction != null && this.Faction.IsRetinue(this);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0001C6AE File Offset: 0x0001A8AE
		public bool IsRegular
		{
			get
			{
				return this.Faction != null && this.Faction.IsRegular(this);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x0001C6CC File Offset: 0x0001A8CC
		public bool IsElite
		{
			get
			{
				return this.Faction != null && this.Faction.IsElite(this);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x0001C6EC File Offset: 0x0001A8EC
		public bool CanHaveCaptain
		{
			get
			{
				BaseFaction baseFaction;
				return this._isCustom && !this.IsCaptain && !WCharacter.IsCaptainId(this._stringId) && !this.Base.IsHero && (!BaseFaction.TroopFactionMap.TryGetValue(this._stringId, out baseFaction) || !(baseFaction != null) || !baseFaction.IsRetinueId(this._stringId));
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0001C757 File Offset: 0x0001A957
		internal static bool IsCustomId(string id)
		{
			return !string.IsNullOrEmpty(id) && (id.StartsWith("retinues_custom_") || id.StartsWith("ret_"));
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0001C77D File Offset: 0x0001A97D
		internal static bool TryGetBaseIdFromCaptainId(string id, out string baseId)
		{
			return WCharacter.BaseIdByCaptainId.TryGetValue(id, out baseId);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0001C78B File Offset: 0x0001A98B
		internal static bool IsCaptainId(string id)
		{
			return !string.IsNullOrEmpty(id) && WCharacter.BaseIdByCaptainId.ContainsKey(id);
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001C7A4 File Offset: 0x0001A9A4
		internal static bool IsCaptainEnabledId(string baseId)
		{
			bool flag;
			return !string.IsNullOrEmpty(baseId) && WCharacter.CaptainEnabledCache.TryGetValue(baseId, out flag) && flag;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001C7CB File Offset: 0x0001A9CB
		internal static bool TryGetCaptainObject(string baseId, out CharacterObject captain)
		{
			return WCharacter.CaptainObjectByBaseId.TryGetValue(baseId, out captain) && captain != null;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001C7E2 File Offset: 0x0001A9E2
		private static void RegisterCaptainPair(string baseId, WCharacter captain)
		{
			if (string.IsNullOrEmpty(baseId) || ((captain != null) ? captain.Base : null) == null)
			{
				return;
			}
			WCharacter.CaptainObjectByBaseId[baseId] = captain.Base;
			WCharacter.BaseIdByCaptainId[captain.StringId] = baseId;
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x0001C81D File Offset: 0x0001AA1D
		public static void ClearCaptainCaches()
		{
			WCharacter.CaptainCache.Clear();
			WCharacter.CaptainEnabledCache.Clear();
			WCharacter.CaptainObjectByBaseId.Clear();
			WCharacter.BaseIdByCaptainId.Clear();
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x0001C848 File Offset: 0x0001AA48
		internal bool HasCaptainInstance
		{
			get
			{
				WCharacter left;
				return this._captain != null || (WCharacter.CaptainCache.TryGetValue(this.StringId, out left) && left != null);
			}
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001C884 File Offset: 0x0001AA84
		internal WCharacter GetExistingCaptain()
		{
			if (this._captain != null)
			{
				return this._captain;
			}
			WCharacter wcharacter;
			if (WCharacter.CaptainCache.TryGetValue(this.StringId, out wcharacter) && wcharacter != null)
			{
				return wcharacter;
			}
			return null;
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000561 RID: 1377 RVA: 0x0001C8C8 File Offset: 0x0001AAC8
		public WCharacter Captain
		{
			get
			{
				if (!this.CanHaveCaptain)
				{
					return null;
				}
				if (!DoctrineAPI.IsDoctrineUnlocked<Captains>() && !Config.NoDoctrineRequirements)
				{
					return null;
				}
				WCharacter wcharacter;
				if (WCharacter.CaptainCache.TryGetValue(this.StringId, out wcharacter) && wcharacter != null)
				{
					wcharacter.IsCaptain = true;
					if (wcharacter.BaseTroop == null)
					{
						wcharacter.BaseTroop = this;
					}
					this._captain = wcharacter;
					return wcharacter;
				}
				if (this._captain == null)
				{
					this._captain = this.CreateCaptain();
				}
				if (this._captain != null)
				{
					WCharacter.CaptainCache[this.StringId] = this._captain;
					this._captain.IsCaptain = true;
					this._captain.BaseTroop = this;
				}
				return this._captain;
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x0001C98C File Offset: 0x0001AB8C
		// (set) Token: 0x06000563 RID: 1379 RVA: 0x0001C9CA File Offset: 0x0001ABCA
		public bool CaptainEnabled
		{
			get
			{
				if (this.IsCaptain)
				{
					WCharacter baseTroop = this.BaseTroop;
					return baseTroop != null && baseTroop.CaptainEnabled;
				}
				bool flag;
				return WCharacter.CaptainEnabledCache.TryGetValue(this.StringId, out flag) && flag;
			}
			set
			{
				if (this.IsCaptain)
				{
					if (this.BaseTroop != null)
					{
						this.BaseTroop.CaptainEnabled = value;
					}
					return;
				}
				WCharacter.CaptainEnabledCache[this.StringId] = value;
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001CA08 File Offset: 0x0001AC08
		public void BindCaptain(WCharacter captain)
		{
			if (captain == null)
			{
				return;
			}
			this._captain = captain;
			captain.IsCaptain = true;
			captain.BaseTroop = this;
			WCharacter.CaptainCache[this.StringId] = captain;
			WCharacter.RegisterCaptainPair(this.StringId, captain);
			if (this.Faction != null)
			{
				captain.Faction = this.Faction;
			}
			captain.HiddenInEncyclopedia = this.HiddenInEncyclopedia;
			captain.IsNotTransferableInPartyScreen = false;
			captain.IsNotTransferableInHideouts = false;
			if (captain.IsCustom && !WCharacter.ActiveStubIds.Contains(captain.StringId))
			{
				WCharacter.ActiveStubIds.Add(captain.StringId);
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001CAB0 File Offset: 0x0001ACB0
		private WCharacter CreateCaptain()
		{
			if (!this.CanHaveCaptain)
			{
				return null;
			}
			WCharacter wcharacter;
			if (WCharacter.CaptainCache.TryGetValue(this.StringId, out wcharacter) && wcharacter != null)
			{
				return wcharacter;
			}
			WCharacter wcharacter2 = new WCharacter(WCharacter.AllocateStub(null));
			WCharacter.ActiveStubIds.Add(wcharacter2.StringId);
			if (this.Faction != null)
			{
				wcharacter2.Faction = this.Faction;
			}
			wcharacter2.FillFrom(this, false, true, true);
			wcharacter2.IsCaptain = true;
			wcharacter2.BaseTroop = this;
			WCharacter.RegisterCaptainPair(this.StringId, wcharacter2);
			if (!this.IsMaxTier)
			{
				wcharacter2.Level += 5;
			}
			wcharacter2.Name = L.T("captain_name", "{NAME} Captain").SetTextVariable("NAME", this.Name).ToString();
			wcharacter2.HiddenInEncyclopedia = true;
			wcharacter2.IsNotTransferableInPartyScreen = false;
			wcharacter2.IsNotTransferableInHideouts = false;
			return wcharacter2;
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x0001CB98 File Offset: 0x0001AD98
		public WCharacter Root
		{
			get
			{
				WCharacter wcharacter = this;
				while (wcharacter.Parent != null)
				{
					wcharacter = wcharacter.Parent;
				}
				return wcharacter;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x0001CBC0 File Offset: 0x0001ADC0
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x0001CBF4 File Offset: 0x0001ADF4
		public WCharacter Parent
		{
			get
			{
				if (!this.IsCustom)
				{
					return VanillaHelper.GetParent(this);
				}
				WCharacter result;
				if (!WCharacter.UpgradeMap.TryGetValue(this.StringId, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				if (this.IsVanilla)
				{
					return;
				}
				WCharacter parent = this.Parent;
				BaseFaction faction = this.Faction;
				if (this.Parent != null)
				{
					List<WCharacter> list = this.Parent.UpgradeTargets.ToList<WCharacter>();
					if (list.Remove(this))
					{
						this.Parent.UpgradeTargets = list.ToArray();
					}
				}
				if (value != null)
				{
					List<WCharacter> list2 = value.UpgradeTargets.ToList<WCharacter>();
					if (!list2.Any((WCharacter wc) => wc.StringId == this.StringId))
					{
						list2.Add(this);
						value.UpgradeTargets = list2.ToArray();
					}
					this.Faction = value.Faction;
				}
				if (value == null)
				{
					WCharacter.UpgradeMap.Remove(this.StringId);
				}
				else
				{
					WCharacter.UpgradeMap[this.StringId] = value;
				}
				if (faction != null)
				{
					faction.InvalidateCategoryCache();
				}
				if (this.Faction != null && this.Faction != faction)
				{
					this.Faction.InvalidateCategoryCache();
				}
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001CCF6 File Offset: 0x0001AEF6
		public IEnumerable<WCharacter> Tree
		{
			get
			{
				WCharacter.<get_Tree>d__76 <get_Tree>d__ = new WCharacter.<get_Tree>d__76(-2);
				<get_Tree>d__.<>4__this = this;
				return <get_Tree>d__;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001CD08 File Offset: 0x0001AF08
		// (set) Token: 0x0600056B RID: 1387 RVA: 0x0001CD84 File Offset: 0x0001AF84
		public BaseFaction Faction
		{
			get
			{
				if (this.IsVanilla)
				{
					WCulture result;
					if ((result = this._cultureCached) == null)
					{
						result = (this._cultureCached = new WCulture(this.Base.Culture));
					}
					return result;
				}
				int troopFactionMapVersion = BaseFaction.TroopFactionMapVersion;
				if (this._factionCachedVersion == troopFactionMapVersion)
				{
					return this._factionCached;
				}
				BaseFaction baseFaction;
				this._factionCached = (BaseFaction.TroopFactionMap.TryGetValue(this.StringId, out baseFaction) ? baseFaction : null);
				this._factionCachedVersion = troopFactionMapVersion;
				return this._factionCached;
			}
			set
			{
				if (value == null)
				{
					BaseFaction.TroopFactionMap.Remove(this.StringId);
				}
				else
				{
					BaseFaction.TroopFactionMap[this.StringId] = value;
				}
				BaseFaction.TouchTroopFactionMap();
				this._factionCached = value;
				this._factionCachedVersion = BaseFaction.TroopFactionMapVersion;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001CDD5 File Offset: 0x0001AFD5
		public override WFaction Clan
		{
			get
			{
				if (!this.IsCustom)
				{
					return null;
				}
				return Player.Clan;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x0001CDE6 File Offset: 0x0001AFE6
		public override WFaction Kingdom
		{
			get
			{
				if (!this.IsCustom)
				{
					return null;
				}
				return Player.Kingdom;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001CDF7 File Offset: 0x0001AFF7
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x0001CE09 File Offset: 0x0001B009
		public virtual string Name
		{
			get
			{
				return this.Base.Name.ToString();
			}
			set
			{
				Reflector.InvokeMethod(this.Base, "SetName", new Type[]
				{
					typeof(TextObject)
				}, new object[]
				{
					new TextObject(value, null)
				});
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x0001CE46 File Offset: 0x0001B046
		public int Tier
		{
			get
			{
				return this.Base.Tier;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x0001CE53 File Offset: 0x0001B053
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x0001CE60 File Offset: 0x0001B060
		public virtual int Level
		{
			get
			{
				return this.Base.Level;
			}
			set
			{
				this.Base.Level = value;
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x0001CE75 File Offset: 0x0001B075
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x0001CE88 File Offset: 0x0001B088
		public virtual WCulture Culture
		{
			get
			{
				return new WCulture(this.Base.Culture);
			}
			set
			{
				try
				{
					if (!(value == null))
					{
						if (!this.IsHero)
						{
							PropertyInfo property = typeof(BasicCharacterObject).GetProperty("Culture", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							if (property != null)
							{
								property.SetValue(this.Base, value.Base, null);
							}
							this.NeedsPersistence = true;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x0001CF00 File Offset: 0x0001B100
		// (set) Token: 0x06000576 RID: 1398 RVA: 0x0001CF17 File Offset: 0x0001B117
		public FormationClass FormationClassOverride
		{
			get
			{
				if (!Config.AllowFormationOverrides)
				{
					return FormationClass.NumberOfAllFormations;
				}
				return this._formationClassOverride;
			}
			set
			{
				if (!Config.AllowFormationOverrides)
				{
					return;
				}
				this._formationClassOverride = value;
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x0001CF34 File Offset: 0x0001B134
		// (set) Token: 0x06000578 RID: 1400 RVA: 0x0001CF44 File Offset: 0x0001B144
		public FormationClass FormationClass
		{
			get
			{
				return this.Base.GetFormationClass();
			}
			set
			{
				try
				{
					Reflector.SetPropertyValue(this.Base, "DefaultFormationClass", value);
					Reflector.SetPropertyValue(this.Base, "DefaultFormationGroup", (int)value);
					bool flag = value == FormationClass.Ranged || value == FormationClass.HorseArcher;
					bool flag2 = value == FormationClass.Cavalry || value == FormationClass.HorseArcher;
					Reflector.SetFieldValue(this.Base, "_isRanged", flag);
					Reflector.SetFieldValue(this.Base, "_isMounted", flag2);
					this.NeedsPersistence = true;
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0001CFE8 File Offset: 0x0001B1E8
		public FormationClass ComputeFormationClass()
		{
			if (this.FormationClassOverride == FormationClass.NumberOfAllFormations)
			{
				return this.Loadout.Battle.ComputeFormationClass();
			}
			return this.FormationClassOverride;
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x0001D00B File Offset: 0x0001B20B
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x0001D013 File Offset: 0x0001B213
		public bool IsCivilian { get; set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x0001D01C File Offset: 0x0001B21C
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x0001D024 File Offset: 0x0001B224
		public bool IsMercenary { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x0001D02D File Offset: 0x0001B22D
		public bool IsHero
		{
			get
			{
				return this.Base.IsHero;
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x0001D03A File Offset: 0x0001B23A
		public bool IsRuler
		{
			get
			{
				Hero heroObject = this.Base.HeroObject;
				return heroObject != null && heroObject.IsFactionLeader;
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x0001D052 File Offset: 0x0001B252
		public bool IsClanLeader
		{
			get
			{
				Hero heroObject = this.Base.HeroObject;
				return heroObject != null && heroObject.IsClanLeader;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x0001D06A File Offset: 0x0001B26A
		public int MaxTier
		{
			get
			{
				return (this.IsElite ? 6 : 5) + ((ModCompatibility.HasTier7Unlocker > false) ? 1 : 0);
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x0001D081 File Offset: 0x0001B281
		public bool IsMaxTier
		{
			get
			{
				return this.Tier >= this.MaxTier;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x0001D094 File Offset: 0x0001B294
		public bool IsDeletable
		{
			get
			{
				return this.IsCustom && !(this.Parent == null) && this.IsRegular && !this.IsHero && !this.UpgradeTargets.Any<WCharacter>();
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x0001D0D4 File Offset: 0x0001B2D4
		// (set) Token: 0x06000585 RID: 1413 RVA: 0x0001D0E6 File Offset: 0x0001B2E6
		public bool HiddenInEncyclopedia
		{
			get
			{
				return Reflector.GetPropertyValue<bool>(this.Base, "HiddenInEncyclopedia");
			}
			set
			{
				Reflector.SetPropertyValue(this.Base, "HiddenInEncyclopedia", value);
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x0001D0FE File Offset: 0x0001B2FE
		// (set) Token: 0x06000587 RID: 1415 RVA: 0x0001D10B File Offset: 0x0001B30B
		public bool IsNotTransferableInHideouts
		{
			get
			{
				return this.Base.IsNotTransferableInHideouts;
			}
			set
			{
				this.Base.SetTransferableInHideouts(!value);
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x0001D11C File Offset: 0x0001B31C
		// (set) Token: 0x06000589 RID: 1417 RVA: 0x0001D129 File Offset: 0x0001B329
		public bool IsNotTransferableInPartyScreen
		{
			get
			{
				return this.Base.IsNotTransferableInPartyScreen;
			}
			set
			{
				this.Base.SetTransferableInPartyScreen(!value);
			}
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x0001D13A File Offset: 0x0001B33A
		public static void ClearSkillCaches()
		{
			WCharacter._moddedSkills = null;
			WCharacter._navalDlcSkills = null;
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x0001D148 File Offset: 0x0001B348
		public static List<SkillObject> ModdedSkills
		{
			get
			{
				if (WCharacter._moddedSkills != null)
				{
					return WCharacter._moddedSkills;
				}
				WCharacter._moddedSkills = (from s in MBObjectManager.Instance.GetObjectTypeList<SkillObject>()
				where !SkillsHelper.VanillaSkillIds.Contains(s.StringId) && !SkillsHelper.NavalDLCSkillIds.Contains(s.StringId)
				select s).ToList<SkillObject>();
				return WCharacter._moddedSkills;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0001D1A0 File Offset: 0x0001B3A0
		public static List<SkillObject> NavalDLCSkills
		{
			get
			{
				if (!ModCompatibility.HasNavalDLC)
				{
					return new List<SkillObject>();
				}
				if (WCharacter._navalDlcSkills != null)
				{
					return WCharacter._navalDlcSkills;
				}
				WCharacter._navalDlcSkills = new List<SkillObject>();
				foreach (string objectName in SkillsHelper.NavalDLCSkillIds)
				{
					SkillObject @object = MBObjectManager.Instance.GetObject<SkillObject>(objectName);
					if (@object != null)
					{
						WCharacter._navalDlcSkills.Add(@object);
					}
				}
				return WCharacter._navalDlcSkills;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0001D230 File Offset: 0x0001B430
		public List<SkillObject> ExtraSkills
		{
			get
			{
				if (!this.IsHero)
				{
					return WCharacter.ModdedSkills;
				}
				List<SkillObject> navalDLCSkills = WCharacter.NavalDLCSkills;
				List<SkillObject> moddedSkills = WCharacter.ModdedSkills;
				List<SkillObject> list = new List<SkillObject>(navalDLCSkills.Count + moddedSkills.Count);
				list.AddRange(navalDLCSkills);
				list.AddRange(moddedSkills);
				return list;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0001D278 File Offset: 0x0001B478
		public List<SkillObject> TroopSkills
		{
			get
			{
				if (!this.IsHero)
				{
					return this.CombatSkills;
				}
				List<SkillObject> combatSkills = this.CombatSkills;
				List<SkillObject> heroSkills = this.HeroSkills;
				List<SkillObject> list = new List<SkillObject>(combatSkills.Count + heroSkills.Count);
				list.AddRange(combatSkills);
				list.AddRange(heroSkills);
				return list;
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x0001D2C4 File Offset: 0x0001B4C4
		public List<SkillObject> AllSkills
		{
			get
			{
				List<SkillObject> troopSkills = this.TroopSkills;
				List<SkillObject> extraSkills = this.ExtraSkills;
				List<SkillObject> list = new List<SkillObject>(troopSkills.Count + extraSkills.Count);
				list.AddRange(troopSkills);
				list.AddRange(extraSkills);
				return list;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000590 RID: 1424 RVA: 0x0001D2FF File Offset: 0x0001B4FF
		// (set) Token: 0x06000591 RID: 1425 RVA: 0x0001D338 File Offset: 0x0001B538
		public virtual Dictionary<SkillObject, int> Skills
		{
			get
			{
				return this.AllSkills.ToDictionary((SkillObject skill) => skill, new Func<SkillObject, int>(this.GetSkill));
			}
			set
			{
				foreach (SkillObject skillObject in this.AllSkills)
				{
					int num;
					int value2 = (value != null && value.TryGetValue(skillObject, out num)) ? num : 0;
					this.SetSkill(skillObject, value2);
				}
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0001D3A0 File Offset: 0x0001B5A0
		public virtual int GetSkill(SkillObject skill)
		{
			return this.Base.GetSkillValue(skill);
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0001D3AE File Offset: 0x0001B5AE
		public virtual void SetSkill(SkillObject skill, int value)
		{
			((PropertyOwner<SkillObject>)Reflector.GetFieldValue<MBCharacterSkills>(this.Base, "DefaultCharacterSkills").Skills).SetPropertyValue(skill, value);
			this.NeedsPersistence = true;
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x0001D3D8 File Offset: 0x0001B5D8
		// (set) Token: 0x06000595 RID: 1429 RVA: 0x0001D3EA File Offset: 0x0001B5EA
		public virtual bool IsFemale
		{
			get
			{
				return Reflector.GetPropertyValue<bool>(this.Base, "IsFemale");
			}
			set
			{
				Reflector.SetPropertyValue(this.Base, "IsFemale", value);
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x0001D409 File Offset: 0x0001B609
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x0001D416 File Offset: 0x0001B616
		public int Race
		{
			get
			{
				return this.Base.Race;
			}
			set
			{
				Reflector.SetPropertyValue(this.Base, "Race", value);
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0001D435 File Offset: 0x0001B635
		public WBody Body
		{
			get
			{
				return new WBody(this);
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x0001D43D File Offset: 0x0001B63D
		public WLoadout Loadout
		{
			get
			{
				return new WLoadout(this);
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001D445 File Offset: 0x0001B645
		public bool IsRanged
		{
			get
			{
				return this.Loadout.Battle.HasNonThrowableRangedWeapons;
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x0001D457 File Offset: 0x0001B657
		public bool IsMounted
		{
			get
			{
				return this.Loadout.Battle.HasMount;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001D46C File Offset: 0x0001B66C
		// (set) Token: 0x0600059D RID: 1437 RVA: 0x0001D4BC File Offset: 0x0001B6BC
		public WCharacter[] UpgradeTargets
		{
			get
			{
				return (from obj in Reflector.GetPropertyValue<CharacterObject[]>(this.Base, "UpgradeTargets") ?? Array.Empty<CharacterObject>()
				select new WCharacter(obj)).ToArray<WCharacter>();
			}
			set
			{
				object @base = this.Base;
				string propertyName = "UpgradeTargets";
				object obj;
				if (value == null)
				{
					obj = null;
				}
				else
				{
					obj = (from wc in value
					select wc.Base).ToArray<CharacterObject>();
				}
				Reflector.SetPropertyValue(@base, propertyName, obj ?? Array.Empty<CharacterObject>());
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x0001D512 File Offset: 0x0001B712
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x0001D51F File Offset: 0x0001B71F
		public ItemCategory UpgradeItemRequirement
		{
			get
			{
				return this.Base.UpgradeRequiresItemFromCategory;
			}
			set
			{
				Reflector.SetPropertyValue(this.Base, "UpgradeRequiresItemFromCategory", value);
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0001D534 File Offset: 0x0001B734
		public void Remove(WCharacter replacement = null)
		{
			if (!this.IsCustom)
			{
				return;
			}
			string[] array = new string[6];
			array[0] = "Removing troop ";
			array[1] = this.Name;
			array[2] = " from parent ";
			int num = 3;
			WCharacter parent = this.Parent;
			array[num] = (((parent != null) ? parent.Name : null) ?? "null");
			array[4] = " and faction ";
			int num2 = 5;
			BaseFaction faction = this.Faction;
			array[num2] = (((faction != null) ? faction.Name : null) ?? "null");
			Log.Debug(string.Concat(array));
			this.Parent = null;
			BaseFaction faction2 = this.Faction;
			this.Faction = null;
			this.HiddenInEncyclopedia = true;
			this.IsNotTransferableInPartyScreen = false;
			this.IsNotTransferableInHideouts = false;
			if (this.IsActive)
			{
				WCharacter.ActiveStubIds.Remove(this.StringId);
			}
			if (faction2 != null)
			{
				faction2.InvalidateCategoryCache();
			}
			WCharacter[] upgradeTargets = this.UpgradeTargets;
			for (int i = 0; i < upgradeTargets.Length; i++)
			{
				upgradeTargets[i].Remove(null);
			}
			TroopStatisticsBehavior.Clear(this);
			if (replacement != null)
			{
				Log.Info(string.Concat(new string[]
				{
					"Replacing existing instances of ",
					this.Name,
					" with ",
					replacement.Name,
					"."
				}));
			}
			else
			{
				Log.Info("Replacing existing instances of " + this.Name + " with best match from culture.");
			}
			this.Replace(replacement ?? TroopMatcher.PickBestFromFaction(this.Culture, this, false, false, null));
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001D6A4 File Offset: 0x0001B8A4
		public void Replace(WCharacter other)
		{
			if (other == null || other == this)
			{
				return;
			}
			foreach (WParty wparty in WParty.All)
			{
				if (wparty != null)
				{
					wparty.MemberRoster.SwapTroop(this, other);
				}
				if (wparty != null)
				{
					wparty.PrisonRoster.SwapTroop(this, other);
				}
			}
			foreach (WSettlement wsettlement in WSettlement.All)
			{
				foreach (WNotable wnotable in wsettlement.Notables)
				{
					wnotable.SwapVolunteer(this, other);
				}
			}
			int num = 0;
			while (num < this.UpgradeTargets.Length && other.UpgradeTargets.Length > num)
			{
				this.UpgradeTargets[num].Replace(other.UpgradeTargets[num]);
				num++;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x0001D7C8 File Offset: 0x0001B9C8
		public bool IsActive
		{
			get
			{
				return !this.IsCustom || WCharacter.ActiveStubIds.Contains(this.StringId);
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
		public bool IsValid
		{
			get
			{
				return this.IsActive && this.Base != null && !string.IsNullOrWhiteSpace(this.StringId) && !string.IsNullOrWhiteSpace(this.Name) && !this.LooksLikeEmptyStub();
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0001D81B File Offset: 0x0001BA1B
		private bool LooksLikeEmptyStub()
		{
			return !this.IsVanilla && !(this.Name != this.StringId) && this.Level == 1;
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x0001D848 File Offset: 0x0001BA48
		public CharacterCode CharacterCode
		{
			get
			{
				return CharacterCode.CreateFrom(this.Base);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x0001D855 File Offset: 0x0001BA55
		public CharacterImageIdentifierVM Image
		{
			get
			{
				return new CharacterImageIdentifierVM(this.CharacterCode);
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0001D862 File Offset: 0x0001BA62
		public ImageIdentifier ImageIdentifier
		{
			get
			{
				return new CharacterImageIdentifier(this.CharacterCode);
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001D870 File Offset: 0x0001BA70
		public bool TryGetModel(int index, out CharacterViewModel model, out Exception error)
		{
			error = null;
			bool result;
			try
			{
				CharacterViewModel characterViewModel = new CharacterViewModel(CharacterViewModel.StanceTypes.None);
				characterViewModel.FillFrom(this.Base, -1, null);
				characterViewModel.SetEquipment(this.Loadout.Get(index).StagingPreview());
				if (this.Faction != null)
				{
					characterViewModel.ArmorColor1 = this.Faction.Color;
					characterViewModel.ArmorColor2 = this.Faction.Color2;
					BaseBannerFaction baseBannerFaction = this.Faction as BaseBannerFaction;
					characterViewModel.BannerCodeText = ((baseBannerFaction != null) ? baseBannerFaction.Banner.Serialize() : null);
				}
				model = characterViewModel;
				result = true;
			}
			catch (AccessViolationException ex)
			{
				error = ex;
				model = null;
				result = false;
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
				error = ex2;
				model = null;
				result = false;
			}
			return result;
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0001D948 File Offset: 0x0001BB48
		public CharacterViewModel GetModel(int index = 0)
		{
			CharacterViewModel result;
			Exception ex;
			this.TryGetModel(index, out result, out ex);
			return result;
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0001D964 File Offset: 0x0001BB64
		public CharacterViewModel GetModel(int index, bool applyGenderOverride)
		{
			if (!applyGenderOverride)
			{
				return this.GetModel(index);
			}
			CharacterViewModel characterViewModel;
			Exception ex;
			if (!this.TryGetModel(index, out characterViewModel, out ex))
			{
				return null;
			}
			characterViewModel.IsFemale = !this.IsFemale;
			return characterViewModel;
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0001D99C File Offset: 0x0001BB9C
		public void FillFrom(WCharacter src, bool keepUpgrades = true, bool keepEquipment = true, bool keepSkills = true)
		{
			if (this.IsVanilla && !this.NeedsPersistence)
			{
				Log.Error("Cannot FillFrom on an unedited vanilla troop.");
				return;
			}
			CharacterObjectHelper.CopyInto(src.Base, this._co);
			WCharacter.VanillaStringIdMap[this.StringId] = src.VanillaStringId;
			this.UpgradeTargets = (keepUpgrades ? src.UpgradeTargets.ToArray<WCharacter>() : Array.Empty<WCharacter>());
			MBCharacterSkills value = (MBCharacterSkills)Activator.CreateInstance(typeof(MBCharacterSkills), true);
			Reflector.SetFieldValue(this._co, "DefaultCharacterSkills", value);
			if (keepSkills)
			{
				this.Skills = this.AllSkills.ToDictionary((SkillObject skill) => skill, new Func<SkillObject, int>(src.GetSkill));
			}
			else
			{
				this.Skills = new Dictionary<SkillObject, int>();
			}
			if (keepEquipment)
			{
				if (Config.CopyAllSetsOnUnlock)
				{
					this.Loadout.FillFrom(src.Loadout, true);
				}
				else
				{
					this.Loadout.FillFrom(src.Loadout, false);
				}
				this.UpgradeItemRequirement = this.Loadout.ComputeUpgradeItemRequirement();
				this.FormationClass = this.ComputeFormationClass();
			}
			else
			{
				this.Loadout.Clear();
			}
			if (ModCompatibility.HasNavalDLC)
			{
				this.IsMariner = src.IsMariner;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x0001DAEE File Offset: 0x0001BCEE
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x0001DB00 File Offset: 0x0001BD00
		public bool IsMariner
		{
			get
			{
				return WCharacter.NavalTraitHelper.GetMarinerLevel(this.Base) > 0;
			}
			set
			{
				int level = (value > false) ? 1 : 0;
				WCharacter.NavalTraitHelper.SetMarinerLevel(this.Base, level);
				this.NeedsPersistence = true;
			}
		}

		// Token: 0x0400015E RID: 350
		public const string CustomIdPrefix = "retinues_custom_";

		// Token: 0x0400015F RID: 351
		public const string LegacyCustomIdPrefix = "ret_";

		// Token: 0x04000162 RID: 354
		private readonly CharacterObject _co;

		// Token: 0x04000163 RID: 355
		private readonly string _stringId;

		// Token: 0x04000164 RID: 356
		public static Dictionary<string, string> VanillaStringIdMap = new Dictionary<string, string>();

		// Token: 0x04000165 RID: 357
		private readonly bool _isLegacyCustom;

		// Token: 0x04000166 RID: 358
		private readonly bool _isCustom;

		// Token: 0x04000167 RID: 359
		public bool IsCaptain;

		// Token: 0x04000168 RID: 360
		private static readonly Dictionary<string, CharacterObject> CaptainObjectByBaseId = new Dictionary<string, CharacterObject>(StringComparer.Ordinal);

		// Token: 0x04000169 RID: 361
		private static readonly Dictionary<string, string> BaseIdByCaptainId = new Dictionary<string, string>(StringComparer.Ordinal);

		// Token: 0x0400016A RID: 362
		private static readonly Dictionary<string, WCharacter> CaptainCache = new Dictionary<string, WCharacter>(StringComparer.Ordinal);

		// Token: 0x0400016B RID: 363
		private static readonly Dictionary<string, bool> CaptainEnabledCache = new Dictionary<string, bool>(StringComparer.Ordinal);

		// Token: 0x0400016C RID: 364
		public WCharacter BaseTroop;

		// Token: 0x0400016D RID: 365
		private WCharacter _captain;

		// Token: 0x0400016E RID: 366
		public static readonly Dictionary<string, WCharacter> UpgradeMap = new Dictionary<string, WCharacter>();

		// Token: 0x0400016F RID: 367
		private BaseFaction _factionCached;

		// Token: 0x04000170 RID: 368
		private int _factionCachedVersion;

		// Token: 0x04000171 RID: 369
		private WCulture _cultureCached;

		// Token: 0x04000172 RID: 370
		private FormationClass _formationClassOverride;

		// Token: 0x04000175 RID: 373
		public readonly List<SkillObject> CombatSkills;

		// Token: 0x04000176 RID: 374
		public readonly List<SkillObject> HeroSkills;

		// Token: 0x04000177 RID: 375
		private static List<SkillObject> _moddedSkills;

		// Token: 0x04000178 RID: 376
		private static List<SkillObject> _navalDlcSkills;

		// Token: 0x02000162 RID: 354
		public static class NavalTraitHelper
		{
			// Token: 0x06000B6C RID: 2924 RVA: 0x0003336C File Offset: 0x0003156C
			private static TraitObject TryGetNavalSoldierTrait()
			{
				if (WCharacter.NavalTraitHelper._navalSoldierTrait != null)
				{
					return WCharacter.NavalTraitHelper._navalSoldierTrait;
				}
				TraitObject result;
				try
				{
					MBObjectManager instance = MBObjectManager.Instance;
					TraitObject traitObject = (instance != null) ? instance.GetObject<TraitObject>("NavalSoldier") : null;
					if (traitObject == null)
					{
						result = null;
					}
					else
					{
						WCharacter.NavalTraitHelper._navalSoldierTrait = traitObject;
						result = WCharacter.NavalTraitHelper._navalSoldierTrait;
					}
				}
				catch
				{
					result = null;
				}
				return result;
			}

			// Token: 0x06000B6D RID: 2925 RVA: 0x000333CC File Offset: 0x000315CC
			public static void Reset()
			{
				WCharacter.NavalTraitHelper._navalSoldierTrait = null;
				WCharacter.NavalTraitHelper._characterTraitsField = null;
				WCharacter.NavalTraitHelper._setPropertyValueMethod = null;
				WCharacter.NavalTraitHelper._isMarinerProperty = null;
			}

			// Token: 0x06000B6E RID: 2926 RVA: 0x000333E8 File Offset: 0x000315E8
			public static int GetMarinerLevel(CharacterObject co)
			{
				if (co == null || !ModCompatibility.HasNavalDLC)
				{
					return 0;
				}
				TraitObject traitObject = WCharacter.NavalTraitHelper.TryGetNavalSoldierTrait();
				if (traitObject == null)
				{
					return 0;
				}
				int result;
				try
				{
					result = co.GetTraitLevel(traitObject);
				}
				catch
				{
					result = 0;
				}
				return result;
			}

			// Token: 0x06000B6F RID: 2927 RVA: 0x00033430 File Offset: 0x00031630
			public static void SetMarinerLevel(CharacterObject co, int level)
			{
				if (co == null || !ModCompatibility.HasNavalDLC)
				{
					return;
				}
				TraitObject traitObject = WCharacter.NavalTraitHelper.TryGetNavalSoldierTrait();
				if (traitObject == null)
				{
					return;
				}
				level = ((level > 0) ? 1 : 0);
				try
				{
					if (co.IsHero)
					{
						Hero heroObject = co.HeroObject;
						if (heroObject != null)
						{
							heroObject.SetTraitLevel(traitObject, level);
						}
					}
					else
					{
						if (WCharacter.NavalTraitHelper._characterTraitsField == null)
						{
							WCharacter.NavalTraitHelper._characterTraitsField = typeof(CharacterObject).GetField("_characterTraits", BindingFlags.Instance | BindingFlags.NonPublic);
						}
						if (!(WCharacter.NavalTraitHelper._characterTraitsField == null))
						{
							object obj = WCharacter.NavalTraitHelper._characterTraitsField.GetValue(co);
							if (obj == null)
							{
								obj = Activator.CreateInstance(WCharacter.NavalTraitHelper._characterTraitsField.FieldType);
								WCharacter.NavalTraitHelper._characterTraitsField.SetValue(co, obj);
							}
							if (WCharacter.NavalTraitHelper._setPropertyValueMethod == null)
							{
								WCharacter.NavalTraitHelper._setPropertyValueMethod = obj.GetType().GetMethod("SetPropertyValue", BindingFlags.Instance | BindingFlags.Public, null, new Type[]
								{
									typeof(TraitObject),
									typeof(int)
								}, null);
							}
							MethodInfo setPropertyValueMethod = WCharacter.NavalTraitHelper._setPropertyValueMethod;
							if (setPropertyValueMethod != null)
							{
								setPropertyValueMethod.Invoke(obj, new object[]
								{
									traitObject,
									level
								});
							}
							if (WCharacter.NavalTraitHelper._isMarinerProperty == null)
							{
								WCharacter.NavalTraitHelper._isMarinerProperty = typeof(CharacterObject).GetProperty("IsMariner", BindingFlags.Instance | BindingFlags.Public);
							}
							PropertyInfo isMarinerProperty = WCharacter.NavalTraitHelper._isMarinerProperty;
							if (isMarinerProperty != null)
							{
								isMarinerProperty.SetValue(co, level > 0);
							}
						}
					}
				}
				catch
				{
				}
			}

			// Token: 0x0400041F RID: 1055
			private static TraitObject _navalSoldierTrait;

			// Token: 0x04000420 RID: 1056
			private static FieldInfo _characterTraitsField;

			// Token: 0x04000421 RID: 1057
			private static MethodInfo _setPropertyValueMethod;

			// Token: 0x04000422 RID: 1058
			private static PropertyInfo _isMarinerProperty;
		}
	}
}
