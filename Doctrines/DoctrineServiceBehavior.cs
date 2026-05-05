using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Configuration;
using Retinues.Doctrines.Model;
using Retinues.Game;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Retinues.Doctrines
{
	// Token: 0x020000D5 RID: 213
	[SafeClass]
	public sealed class DoctrineServiceBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000893 RID: 2195 RVA: 0x0002B3B0 File Offset: 0x000295B0
		public override void SyncData(IDataStore dataStore)
		{
			HashSet<string> unlocked = this._unlocked;
			List<string> list = ((unlocked != null) ? unlocked.ToList<string>() : null) ?? new List<string>();
			dataStore.SyncData<List<string>>("Retinues_Doctrines_Unlocked", ref list);
			HashSet<string> unlocked2;
			if (list != null)
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (string item in list)
				{
					hashSet.Add(item);
				}
				unlocked2 = hashSet;
			}
			else
			{
				unlocked2 = new HashSet<string>();
			}
			this._unlocked = unlocked2;
			dataStore.SyncData<Dictionary<string, int>>("Retinues_Doctrines_FeatProgress", ref this._featProgress);
			if (this._featProgress == null)
			{
				this._featProgress = new Dictionary<string, int>();
			}
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0002B46C File Offset: 0x0002966C
		public override void RegisterEvents()
		{
			Log.Debug("Registering doctrine service events");
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, delegate(CampaignGameStarter _)
			{
				this.BuildCatalogIfNeeded();
			});
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000895 RID: 2197 RVA: 0x0002B490 File Offset: 0x00029690
		// (remove) Token: 0x06000896 RID: 2198 RVA: 0x0002B4C8 File Offset: 0x000296C8
		public event Action<string> DoctrineUnlocked;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000897 RID: 2199 RVA: 0x0002B500 File Offset: 0x00029700
		// (remove) Token: 0x06000898 RID: 2200 RVA: 0x0002B538 File Offset: 0x00029738
		public event Action<string> FeatCompleted;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000899 RID: 2201 RVA: 0x0002B570 File Offset: 0x00029770
		// (remove) Token: 0x0600089A RID: 2202 RVA: 0x0002B5A8 File Offset: 0x000297A8
		public event Action CatalogBuilt;

		// Token: 0x0600089B RID: 2203 RVA: 0x0002B5E0 File Offset: 0x000297E0
		public IEnumerable<DoctrineDefinition> AllDoctrines()
		{
			return from d in this._defsByKey.Values
			orderby d.Column, d.Row
			select d;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0002B640 File Offset: 0x00029840
		public DoctrineDefinition GetDoctrine(string key)
		{
			DoctrineDefinition result;
			if (!this._defsByKey.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0002B660 File Offset: 0x00029860
		public bool IsDoctrineUnlocked(string key)
		{
			return this._unlocked.Contains(key);
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0002B670 File Offset: 0x00029870
		public DoctrineStatus GetDoctrineStatus(string key)
		{
			DoctrineDefinition doctrine = this.GetDoctrine(key);
			if (doctrine == null)
			{
				return DoctrineStatus.Locked;
			}
			if (this._unlocked.Contains(key))
			{
				return DoctrineStatus.Unlocked;
			}
			string effectivePrerequisiteKey = this.GetEffectivePrerequisiteKey(key);
			if (!string.IsNullOrEmpty(effectivePrerequisiteKey) && !this._unlocked.Contains(effectivePrerequisiteKey))
			{
				return DoctrineStatus.Locked;
			}
			if (!Config.EnableFeatRequirements)
			{
				return DoctrineStatus.InProgress;
			}
			List<FeatDefinition> feats = doctrine.Feats;
			if (feats == null || feats.Count == 0)
			{
				return DoctrineStatus.InProgress;
			}
			if (feats.Any((FeatDefinition f) => !this.IsFeatComplete(f.Key)))
			{
				return DoctrineStatus.Unlockable;
			}
			return DoctrineStatus.InProgress;
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0002B6F4 File Offset: 0x000298F4
		private string GetEffectivePrerequisiteKey(string key)
		{
			DoctrineDefinition doctrine = this.GetDoctrine(key);
			if (doctrine == null)
			{
				return null;
			}
			string prerequisiteKey = doctrine.PrerequisiteKey;
			if (string.IsNullOrEmpty(prerequisiteKey))
			{
				return null;
			}
			HashSet<string> hashSet = new HashSet<string>();
			while (!string.IsNullOrEmpty(prerequisiteKey) && hashSet.Add(prerequisiteKey))
			{
				if (!this.IsDoctrineDisabled(prerequisiteKey))
				{
					return prerequisiteKey;
				}
				DoctrineDefinition doctrine2 = this.GetDoctrine(prerequisiteKey);
				if (doctrine2 == null)
				{
					break;
				}
				prerequisiteKey = doctrine2.PrerequisiteKey;
			}
			return null;
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0002B758 File Offset: 0x00029958
		public bool TryAcquireDoctrine(string key, out string reason)
		{
			reason = null;
			DoctrineDefinition doctrine = this.GetDoctrine(key);
			if (doctrine == null)
			{
				reason = "Unknown doctrine.";
				return false;
			}
			DoctrineStatus doctrineStatus = this.GetDoctrineStatus(key);
			if (doctrineStatus != DoctrineStatus.InProgress)
			{
				reason = ((doctrineStatus == DoctrineStatus.Locked) ? "Prerequisite not met." : "Feats incomplete.");
				return false;
			}
			Hero mainHero = Hero.MainHero;
			int num = (mainHero != null) ? mainHero.Gold : 0;
			Clan playerClan = Clan.PlayerClan;
			float num2 = (playerClan != null) ? playerClan.Influence : 0f;
			if (num < doctrine.GoldCost)
			{
				reason = "Not enough gold.";
				return false;
			}
			if (num2 < (float)doctrine.InfluenceCost)
			{
				reason = "Not enough influence.";
				return false;
			}
			Player.ChangeGold(-doctrine.GoldCost);
			Player.ChangeInfluence(-doctrine.InfluenceCost);
			this._unlocked.Add(key);
			Action<string> doctrineUnlocked = this.DoctrineUnlocked;
			if (doctrineUnlocked != null)
			{
				doctrineUnlocked(key);
			}
			return true;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0002B820 File Offset: 0x00029A20
		public int GetFeatTarget(string featKey)
		{
			if (!Config.EnableFeatRequirements)
			{
				return 0;
			}
			string text2;
			string text = this._featToDoctrine.TryGetValue(featKey, out text2) ? text2 : null;
			if (text == null)
			{
				return 0;
			}
			DoctrineDefinition doctrine = this.GetDoctrine(text);
			object obj;
			if (doctrine == null)
			{
				obj = null;
			}
			else
			{
				List<FeatDefinition> feats = doctrine.Feats;
				obj = ((feats != null) ? feats.FirstOrDefault((FeatDefinition f) => f.Key == featKey) : null);
			}
			object obj2 = obj;
			if (obj2 == null)
			{
				return 0;
			}
			return obj2.Target;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0002B8A0 File Offset: 0x00029AA0
		public int GetFeatProgress(string featKey)
		{
			int result;
			if (!this._featProgress.TryGetValue(featKey, out result))
			{
				return 0;
			}
			return result;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0002B8C0 File Offset: 0x00029AC0
		public bool IsFeatComplete(string featKey)
		{
			if (!Config.EnableFeatRequirements)
			{
				return true;
			}
			int featTarget = this.GetFeatTarget(featKey);
			return featTarget <= 0 || this.GetFeatProgress(featKey) >= featTarget;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0002B8F8 File Offset: 0x00029AF8
		public void SetFeatProgress(string featKey, int amount)
		{
			if (string.IsNullOrEmpty(featKey) || amount < 0)
			{
				return;
			}
			if (!this._featToDoctrine.ContainsKey(featKey))
			{
				return;
			}
			int featTarget = this.GetFeatTarget(featKey);
			int num = Math.Min((featTarget <= 0) ? amount : amount, (featTarget > 0) ? featTarget : int.MaxValue);
			this._featProgress[featKey] = num;
			if (featTarget > 0 && num >= featTarget)
			{
				Action<string> featCompleted = this.FeatCompleted;
				if (featCompleted == null)
				{
					return;
				}
				featCompleted(featKey);
			}
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0002B96C File Offset: 0x00029B6C
		public int AdvanceFeat(string featKey, int amount = 1)
		{
			if (string.IsNullOrEmpty(featKey) || amount <= 0)
			{
				return this.GetFeatProgress(featKey);
			}
			if (!this._featToDoctrine.ContainsKey(featKey))
			{
				return this.GetFeatProgress(featKey);
			}
			int featTarget = this.GetFeatTarget(featKey);
			int featProgress = this.GetFeatProgress(featKey);
			int num = Math.Min((featTarget <= 0) ? (featProgress + amount) : (featProgress + amount), (featTarget > 0) ? featTarget : int.MaxValue);
			this._featProgress[featKey] = num;
			if (featTarget > 0 && featProgress < featTarget && num >= featTarget)
			{
				Action<string> featCompleted = this.FeatCompleted;
				if (featCompleted != null)
				{
					featCompleted(featKey);
				}
			}
			return num;
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x0002BA00 File Offset: 0x00029C00
		private void BuildCatalogIfNeeded()
		{
			if (this._defsByKey.Count > 0)
			{
				return;
			}
			Log.Debug("Building doctrine catalog...");
			IReadOnlyList<Doctrine> readOnlyList = DoctrineDiscovery.DiscoverDoctrines("Retinues.Doctrines.Catalog");
			List<DoctrineDefinition> list = new List<DoctrineDefinition>();
			this._featToDoctrine.Clear();
			this._modelsByKey = readOnlyList.ToDictionary((Doctrine d) => d.Key, (Doctrine d) => d);
			foreach (Doctrine doctrine in readOnlyList)
			{
				string key = doctrine.Key;
				List<FeatDefinition> list2 = new List<FeatDefinition>();
				foreach (Feat feat in doctrine.InstantiateFeats())
				{
					string key2 = feat.Key;
					list2.Add(new FeatDefinition
					{
						Key = key2,
						Description = feat.Description,
						Target = feat.Target
					});
					this._featToDoctrine[key2] = key;
					feat.OnRegister();
				}
				list.Add(new DoctrineDefinition
				{
					Key = key,
					Name = doctrine.Name,
					Description = doctrine.Description,
					Column = doctrine.Column,
					Row = doctrine.Row,
					GoldCost = doctrine.GoldCost,
					InfluenceCost = doctrine.InfluenceCost,
					Feats = list2
				});
			}
			Dictionary<ValueTuple<int, int>, DoctrineDefinition> dictionary = new Dictionary<ValueTuple<int, int>, DoctrineDefinition>();
			foreach (DoctrineDefinition doctrineDefinition in list)
			{
				dictionary[new ValueTuple<int, int>(doctrineDefinition.Column, doctrineDefinition.Row)] = doctrineDefinition;
			}
			foreach (DoctrineDefinition doctrineDefinition2 in list)
			{
				DoctrineDefinition doctrineDefinition3;
				if (doctrineDefinition2.Row <= 0)
				{
					doctrineDefinition2.PrerequisiteKey = null;
				}
				else if (dictionary.TryGetValue(new ValueTuple<int, int>(doctrineDefinition2.Column, doctrineDefinition2.Row - 1), out doctrineDefinition3))
				{
					doctrineDefinition2.PrerequisiteKey = doctrineDefinition3.Key;
				}
				else
				{
					doctrineDefinition2.PrerequisiteKey = null;
				}
			}
			this._defsByKey = list.ToDictionary((DoctrineDefinition d) => d.Key, (DoctrineDefinition d) => d);
			Action catalogBuilt = this.CatalogBuilt;
			if (catalogBuilt == null)
			{
				return;
			}
			catalogBuilt();
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x0002BD30 File Offset: 0x00029F30
		public bool IsDoctrineDisabled(string key)
		{
			Doctrine doctrine;
			return this._modelsByKey != null && this._modelsByKey.TryGetValue(key, out doctrine) && doctrine.IsDisabled;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x0002BD60 File Offset: 0x00029F60
		public TextObject GetDoctrineDescription(string key)
		{
			DoctrineDefinition doctrine = this.GetDoctrine(key);
			if (doctrine == null)
			{
				return new TextObject(string.Empty, null);
			}
			Doctrine doctrine2;
			if (this._modelsByKey != null && this._modelsByKey.TryGetValue(key, out doctrine2) && doctrine2.IsDisabled)
			{
				TextObject result;
				if ((result = doctrine2.DisabledMessage) == null)
				{
					result = (doctrine2.Description ?? doctrine.Description);
				}
				return result;
			}
			return doctrine.Description;
		}

		// Token: 0x0400024B RID: 587
		private HashSet<string> _unlocked = new HashSet<string>();

		// Token: 0x0400024C RID: 588
		private Dictionary<string, int> _featProgress = new Dictionary<string, int>();

		// Token: 0x0400024D RID: 589
		private Dictionary<string, DoctrineDefinition> _defsByKey = new Dictionary<string, DoctrineDefinition>();

		// Token: 0x0400024E RID: 590
		private readonly Dictionary<string, string> _featToDoctrine = new Dictionary<string, string>();

		// Token: 0x0400024F RID: 591
		private Dictionary<string, Doctrine> _modelsByKey = new Dictionary<string, Doctrine>();
	}
}
