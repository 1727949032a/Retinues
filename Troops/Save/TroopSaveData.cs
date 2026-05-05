using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Catalog;
using Retinues.Features.Experience;
using Retinues.Features.Staging;
using Retinues.Game;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.Safety.Legacy;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Retinues.Troops.Save
{
	// Token: 0x0200003C RID: 60
	public class TroopSaveData
	{
		// Token: 0x06000135 RID: 309 RVA: 0x00008B09 File Offset: 0x00006D09
		public TroopSaveData()
		{
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00008B24 File Offset: 0x00006D24
		public TroopSaveData(WCharacter troop)
		{
			if (troop == null)
			{
				return;
			}
			this.StringId = troop.StringId;
			this.VanillaStringId = troop.VanillaStringId;
			this.Name = troop.Name;
			this.Level = troop.Level;
			this.IsFemale = troop.IsFemale;
			this.CultureId = troop.Culture.StringId;
			this.UpgradeTargets = (from t in troop.UpgradeTargets
			select new TroopSaveData(t)).ToList<TroopSaveData>();
			this.EquipmentData = new TroopEquipmentData(troop.Loadout.Equipments);
			this.SkillData = new TroopSkillData(troop.Skills);
			this.BodyData = (Config.EnableTroopCustomization ? new TroopBodySaveData(troop.Body) : null);
			this.Race = troop.Race;
			this.FormationClassOverride = troop.FormationClassOverride;
			this.IsCaptain = troop.IsCaptain;
			this.CaptainEnabled = troop.CaptainEnabled;
			this.IsMariner = troop.IsMariner;
			if (!troop.IsCaptain && troop.HasCaptainInstance)
			{
				WCharacter existingCaptain = troop.GetExistingCaptain();
				if (existingCaptain != null)
				{
					this.Captain = new TroopSaveData(existingCaptain);
				}
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00008C7F File Offset: 0x00006E7F
		private bool IsValid()
		{
			return !string.IsNullOrEmpty(this.StringId);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00008C90 File Offset: 0x00006E90
		private WCharacter MakeTroop(string stringId = null, WFaction faction = null, RootCategory category = RootCategory.Other, WCharacter parent = null)
		{
			if (!this.IsValid())
			{
				return null;
			}
			if (faction != null && category != RootCategory.Other)
			{
				return new WCharacter(faction, category, stringId ?? this.StringId);
			}
			if (parent != null)
			{
				return new WCharacter(parent, stringId ?? this.StringId);
			}
			if (stringId != null)
			{
				return new WCharacter(stringId);
			}
			return null;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008CF0 File Offset: 0x00006EF0
		public WCharacter Deserialize()
		{
			WCharacter troop = this.MakeTroop(this.StringId, null, RootCategory.Other, null);
			return this.DeserializeInternal(troop);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008D18 File Offset: 0x00006F18
		public WCharacter Deserialize(WFaction faction, RootCategory category)
		{
			WCharacter troop = this.MakeTroop(this.StringId, faction, category, null);
			return this.DeserializeInternal(troop);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008D3C File Offset: 0x00006F3C
		public WCharacter Deserialize(WCharacter parent)
		{
			WCharacter troop = this.MakeTroop(this.StringId, null, RootCategory.Other, parent);
			return this.DeserializeInternal(troop);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008D64 File Offset: 0x00006F64
		public WCharacter DeserializeInternal(WCharacter troop)
		{
			if (troop == null)
			{
				return null;
			}
			if (troop.IsVanilla)
			{
				troop.NeedsPersistence = true;
			}
			WCharacter wcharacter = new WCharacter(this.VanillaStringId);
			troop.FillFrom(wcharacter, troop.IsVanilla, false, false);
			troop.Name = this.Name;
			troop.Level = this.Level;
			troop.IsFemale = this.IsFemale;
			troop.Skills = this.SkillData.Deserialize();
			troop.Loadout.SetEquipments(this.EquipmentData.Deserialize(troop));
			foreach (TroopSaveData troopSaveData in (this.UpgradeTargets ?? new List<TroopSaveData>()))
			{
				troopSaveData.Deserialize(troop);
			}
			if (this.CultureId != wcharacter.Culture.StringId)
			{
				troop.Culture = new WCulture(MBObjectManager.Instance.GetObject<CultureObject>(this.CultureId));
				BodyHelper.ApplyPropertiesFromCulture(troop, this.CultureId);
			}
			if (this.Race >= 0)
			{
				troop.Race = this.Race;
				troop.Body.EnsureOwnBodyRange();
			}
			if (Config.EnableTroopCustomization)
			{
				TroopBodySaveData bodyData = this.BodyData;
				if (bodyData != null)
				{
					bodyData.Apply(troop.Body);
				}
			}
			troop.FormationClassOverride = this.FormationClassOverride;
			troop.ComputeDerivedProperties();
			try
			{
				if (this.StringId.StartsWith("ret_"))
				{
					WCharacter wcharacter2 = WCharacter.FromStringId(this.StringId);
					wcharacter2.FillFrom(troop, true, true, true);
					wcharacter2.Replace(troop);
					TroopXpBehavior.ReplacePoolKey(this.StringId, troop.StringId);
					BaseUpgradeBehavior<PendingEquipData>.ReplacePendingKey(this.StringId, troop.StringId);
					BaseUpgradeBehavior<PendingTrainData>.ReplacePendingKey(this.StringId, troop.StringId);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
			BodyHelper.ApplyTagsFromCulture(troop);
			troop.CaptainEnabled = this.CaptainEnabled;
			try
			{
				if (!this.IsCaptain && this.Captain != null && (DoctrineAPI.IsDoctrineUnlocked<Captains>() || Config.NoDoctrineRequirements))
				{
					WCharacter wcharacter3 = this.Captain.Deserialize();
					if (wcharacter3 != null)
					{
						if (troop.Faction != null)
						{
							wcharacter3.Faction = troop.Faction;
						}
						troop.BindCaptain(wcharacter3);
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
			}
			troop.IsMariner = this.IsMariner;
			return troop;
		}

		// Token: 0x0400006B RID: 107
		[SaveableField(1)]
		public string StringId;

		// Token: 0x0400006C RID: 108
		[SaveableField(2)]
		public string VanillaStringId;

		// Token: 0x0400006D RID: 109
		[SaveableField(3)]
		public string Name;

		// Token: 0x0400006E RID: 110
		[SaveableField(4)]
		public int Level;

		// Token: 0x0400006F RID: 111
		[SaveableField(5)]
		public bool IsFemale;

		// Token: 0x04000070 RID: 112
		[SaveableField(6)]
		public string CultureId;

		// Token: 0x04000071 RID: 113
		[SaveableField(7)]
		public List<TroopSaveData> UpgradeTargets = new List<TroopSaveData>();

		// Token: 0x04000072 RID: 114
		[SaveableField(8)]
		public TroopEquipmentData EquipmentData;

		// Token: 0x04000073 RID: 115
		[SaveableField(9)]
		public TroopSkillData SkillData;

		// Token: 0x04000074 RID: 116
		[SaveableField(10)]
		public TroopBodySaveData BodyData;

		// Token: 0x04000075 RID: 117
		[SaveableField(11)]
		public int Race;

		// Token: 0x04000076 RID: 118
		[SaveableField(12)]
		public FormationClass FormationClassOverride = FormationClass.NumberOfAllFormations;

		// Token: 0x04000077 RID: 119
		[SaveableField(13)]
		public TroopSaveData Captain;

		// Token: 0x04000078 RID: 120
		[SaveableField(14)]
		public bool IsCaptain;

		// Token: 0x04000079 RID: 121
		[SaveableField(15)]
		public bool CaptainEnabled;

		// Token: 0x0400007A RID: 122
		[SaveableField(16)]
		public bool IsMariner;
	}
}
