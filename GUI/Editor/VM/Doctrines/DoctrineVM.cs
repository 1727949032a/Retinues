using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Doctrines
{
	// Token: 0x02000086 RID: 134
	[SafeClass]
	public sealed class DoctrineVM : BaseVM
	{
		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x00019A48 File Offset: 0x00017C48
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				return new Dictionary<UIEvent, string[]>();
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00019A50 File Offset: 0x00017C50
		public DoctrineVM(string doctrineId)
		{
			try
			{
				this._id = doctrineId;
				Campaign campaign = Campaign.Current;
				this._svc = ((campaign != null) ? campaign.GetCampaignBehavior<DoctrineServiceBehavior>() : null);
				DoctrineServiceBehavior svc = this._svc;
				this._def = ((svc != null) ? svc.GetDoctrine(this._id) : null);
				DoctrineDefinition def = this._def;
				string text;
				if (def == null)
				{
					text = null;
				}
				else
				{
					TextObject name = def.Name;
					text = ((name != null) ? name.ToString() : null);
				}
				this._name = (text ?? this._id);
				this.Refresh();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x00019AF4 File Offset: 0x00017CF4
		[DataSourceProperty]
		public string ButtonText
		{
			get
			{
				if (!this.IsEnabled)
				{
					return this._name;
				}
				if (!Config.EnableFeatRequirements)
				{
					return this._name;
				}
				DoctrineDefinition def = this._def;
				int? num;
				if (def == null)
				{
					num = null;
				}
				else
				{
					List<FeatDefinition> feats = def.Feats;
					num = ((feats != null) ? new int?(feats.Count) : null);
				}
				int? num2 = num;
				int valueOrDefault = num2.GetValueOrDefault();
				int num3 = 0;
				if (valueOrDefault > 0 && this._svc != null)
				{
					num3 = this._def.Feats.Count((FeatDefinition f) => this._svc.IsFeatComplete(f.Key));
				}
				return string.Format("{0} ({1}/{2})", this._name, num3, valueOrDefault);
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x00019BA6 File Offset: 0x00017DA6
		[DataSourceProperty]
		public string ButtonBrush
		{
			get
			{
				if (this.Status != DoctrineStatus.Unlocked)
				{
					return "ButtonBrush2";
				}
				return "ButtonBrush1";
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x00019BBC File Offset: 0x00017DBC
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				DoctrineServiceBehavior svc = this._svc;
				return (svc == null || !svc.IsDoctrineDisabled(this._id)) && this.Status != DoctrineStatus.Locked && this.Status != DoctrineStatus.Unlocked;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x00019BF0 File Offset: 0x00017DF0
		[DataSourceProperty]
		public DoctrineStatus Status
		{
			get
			{
				return DoctrineAPI.GetDoctrineStatus(this._id);
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x00019C00 File Offset: 0x00017E00
		[DataSourceProperty]
		public string Description
		{
			get
			{
				if (this._svc == null)
				{
					DoctrineDefinition def = this._def;
					string text;
					if (def == null)
					{
						text = null;
					}
					else
					{
						TextObject description = def.Description;
						text = ((description != null) ? description.ToString() : null);
					}
					return text ?? string.Empty;
				}
				TextObject doctrineDescription = this._svc.GetDoctrineDescription(this._id);
				return ((doctrineDescription != null) ? doctrineDescription.ToString() : null) ?? string.Empty;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00019C64 File Offset: 0x00017E64
		[DataSourceProperty]
		public MBBindingList<DoctrineDescriptionLineVM> DescriptionLines
		{
			get
			{
				MBBindingList<DoctrineDescriptionLineVM> mbbindingList = new MBBindingList<DoctrineDescriptionLineVM>();
				int num = 45;
				string[] array = this.Description.Split(new char[]
				{
					' '
				});
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in array)
				{
					if (stringBuilder.Length + text.Length + 1 > num)
					{
						mbbindingList.Add(new DoctrineDescriptionLineVM(stringBuilder.ToString().TrimEnd(Array.Empty<char>())));
						stringBuilder.Clear();
					}
					stringBuilder.Append(text).Append(' ');
				}
				if (stringBuilder.Length > 0)
				{
					mbbindingList.Add(new DoctrineDescriptionLineVM(stringBuilder.ToString().TrimEnd(Array.Empty<char>())));
				}
				return mbbindingList;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x00019D19 File Offset: 0x00017F19
		[DataSourceProperty]
		public bool ShowCostRow
		{
			get
			{
				return this.ShowGoldCost || this.ShowInfluenceCost;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00019D2B File Offset: 0x00017F2B
		[DataSourceProperty]
		public bool ShowGoldCost
		{
			get
			{
				return this.GoldCost > 0;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x00019D36 File Offset: 0x00017F36
		[DataSourceProperty]
		public bool ShowInfluenceCost
		{
			get
			{
				return this.InfluenceCost > 0;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x00019D41 File Offset: 0x00017F41
		[DataSourceProperty]
		public int GoldCost
		{
			get
			{
				DoctrineDefinition def = this._def;
				if (def == null)
				{
					return 0;
				}
				return def.GoldCost;
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00019D54 File Offset: 0x00017F54
		[DataSourceProperty]
		public int InfluenceCost
		{
			get
			{
				DoctrineDefinition def = this._def;
				if (def == null)
				{
					return 0;
				}
				return def.InfluenceCost;
			}
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00019D68 File Offset: 0x00017F68
		[DataSourceMethod]
		[SafeMethod(null, true, null)]
		public void ExecuteShowPopup()
		{
			Campaign campaign = Campaign.Current;
			DoctrineServiceBehavior doctrineServiceBehavior = (campaign != null) ? campaign.GetCampaignBehavior<DoctrineServiceBehavior>() : null;
			DoctrineDefinition doctrineDefinition = (doctrineServiceBehavior != null) ? doctrineServiceBehavior.GetDoctrine(this._id) : null;
			if (doctrineServiceBehavior == null || doctrineDefinition == null)
			{
				return;
			}
			List<FeatDefinition> list = doctrineDefinition.Feats ?? new List<FeatDefinition>();
			int count = list.Count;
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FeatDefinition featDefinition in list)
			{
				bool flag = doctrineServiceBehavior.IsFeatComplete(featDefinition.Key);
				if (flag)
				{
					num++;
				}
				string value = flag ? "■" : "□";
				int featProgress = doctrineServiceBehavior.GetFeatProgress(featDefinition.Key);
				int featTarget = doctrineServiceBehavior.GetFeatTarget(featDefinition.Key);
				if (featTarget > 0)
				{
					stringBuilder.Append("    ").Append(value).Append("  ").Append(featDefinition.Description).Append(" (").Append(featProgress).Append('/').Append(featTarget).Append(")\n");
				}
				else
				{
					stringBuilder.Append("    ").Append(value).Append("  ").Append(featDefinition.Description).Append('\n');
				}
			}
			string text = (count == 0) ? L.S("feats_no_reqs", "No requirements.") : string.Format("{0}:\n\n{1}", L.S("feats_reqs", "Requirements"), stringBuilder);
			string text2;
			if (!this.ShowCostRow)
			{
				text2 = string.Empty;
			}
			else
			{
				List<string> list2 = new List<string>();
				if (this.ShowGoldCost)
				{
					list2.Add(L.T("doctrine_costs_gold", "{GOLD} Gold").SetTextVariable("GOLD", this.GoldCost).ToString());
				}
				if (this.ShowInfluenceCost)
				{
					list2.Add(L.T("doctrine_costs_influence", "{INFLUENCE} Influence").SetTextVariable("INFLUENCE", this.InfluenceCost).ToString());
				}
				string text3 = string.Join(", ", list2);
				text2 = (string.IsNullOrEmpty(text3) ? string.Empty : (L.S("doctrine_costs_label", "Cost:") + " " + text3 + "."));
			}
			string text4 = (!Config.EnableFeatRequirements) ? (string.IsNullOrEmpty(text2) ? this.Description : (this.Description + "\n\n" + text2)) : (string.IsNullOrEmpty(text2) ? (this.Description + "\n\n" + text) : string.Concat(new string[]
			{
				this.Description,
				"\n\n",
				text,
				"\n\n",
				text2
			}));
			bool flag2 = count == 0 || num == count;
			bool flag3 = doctrineServiceBehavior.IsDoctrineUnlocked(this._id);
			if (flag2 && !flag3)
			{
				InformationManager.ShowInquiry(new InquiryData(this._name, text4.ToString(), true, true, L.S("unlock_btn", "Unlock"), GameTexts.FindText("str_cancel", null).ToString(), delegate()
				{
					string text5;
					if (DoctrineAPI.TryAcquireDoctrine(this._id, out text5))
					{
						Sound.Play2D("event:/ui/reign/decision");
						DoctrineColumnVM column = this.Column;
						if (column != null)
						{
							column.Refresh();
						}
						WFaction wfaction = State.Faction as WFaction;
						if (wfaction != null)
						{
							TroopBuilder.EnsureTroopsExist(wfaction);
							State.UpdateFaction(null);
						}
						EventManager.Fire(UIEvent.Slot);
						return;
					}
					InformationManager.DisplayMessage(new InformationMessage(string.IsNullOrEmpty(text5) ? L.S("unlock_failed", "Cannot unlock.") : text5));
				}, delegate()
				{
				}, "", 0f, null, null, null), true, false);
				return;
			}
			InformationManager.ShowInquiry(new InquiryData(this._name, text4.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0001A10C File Offset: 0x0001830C
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x0001A114 File Offset: 0x00018314
		public DoctrineColumnVM Column { get; set; }

		// Token: 0x060004BB RID: 1211 RVA: 0x0001A120 File Offset: 0x00018320
		public void Refresh()
		{
			base.OnPropertyChanged("ButtonBrush");
			base.OnPropertyChanged("Status");
			base.OnPropertyChanged("ButtonText");
			base.OnPropertyChanged("Description");
			base.OnPropertyChanged("IsEnabled");
			base.OnPropertyChanged("GoldCost");
			base.OnPropertyChanged("InfluenceCost");
			base.OnPropertyChanged("ShowGoldCost");
			base.OnPropertyChanged("ShowInfluenceCost");
			base.OnPropertyChanged("ShowCostRow");
		}

		// Token: 0x0400012F RID: 303
		private readonly string _id;

		// Token: 0x04000130 RID: 304
		private readonly DoctrineServiceBehavior _svc;

		// Token: 0x04000131 RID: 305
		private readonly DoctrineDefinition _def;

		// Token: 0x04000132 RID: 306
		private readonly string _name;
	}
}
