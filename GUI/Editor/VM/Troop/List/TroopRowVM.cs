using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor.VM.Troop.List
{
	// Token: 0x02000081 RID: 129
	[SafeClass]
	public sealed class TroopRowVM : BaseListElementVM
	{
		// Token: 0x060003BB RID: 955 RVA: 0x00015021 File Offset: 0x00013221
		public TroopRowVM(WCharacter rowTroop, string placeholderText = null) : base(true)
		{
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060003BC RID: 956 RVA: 0x00015038 File Offset: 0x00013238
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"IsPlaceholder",
					"NameText",
					"TierIconData",
					"IsSelected",
					"FormationClassIcon",
					"ShowPlayerIcon",
					"ShowRulerIcon",
					"ShowClanLeaderIcon"
				};
				dictionary[UIEvent.Equipment] = new string[]
				{
					"FormationClassIcon"
				};
				dictionary[UIEvent.Appearance] = new string[]
				{
					"ImageId",
					"ImageAdditionalArgs",
					"ImageTextureProviderName"
				};
				dictionary[UIEvent.Equip] = new string[]
				{
					"FormationClassIcon"
				};
				return dictionary;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060003BD RID: 957 RVA: 0x000150E8 File Offset: 0x000132E8
		[DataSourceProperty]
		public string ImageId
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				if (rowTroop == null)
				{
					return null;
				}
				return rowTroop.Image.Id;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060003BE RID: 958 RVA: 0x00015100 File Offset: 0x00013300
		[DataSourceProperty]
		public string ImageAdditionalArgs
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				if (rowTroop == null)
				{
					return null;
				}
				return rowTroop.Image.AdditionalArgs;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060003BF RID: 959 RVA: 0x00015118 File Offset: 0x00013318
		[DataSourceProperty]
		public string ImageTextureProviderName
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				if (rowTroop == null)
				{
					return null;
				}
				return rowTroop.Image.TextureProviderName;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x00015130 File Offset: 0x00013330
		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				if (rowTroop == null || !rowTroop.IsHero)
				{
					return Icons.GetTierIconData(this.RowTroop);
				}
				return null;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x00015153 File Offset: 0x00013353
		[DataSourceProperty]
		public string FormationClassIcon
		{
			get
			{
				return Icons.GetFormationClassIcon(this.RowTroop);
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x00015160 File Offset: 0x00013360
		[DataSourceProperty]
		public bool IsPlaceholder
		{
			get
			{
				return this.RowTroop == null;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x0001516E File Offset: 0x0001336E
		[DataSourceProperty]
		public bool IsTroop
		{
			get
			{
				return this.RowTroop != null;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x0001517C File Offset: 0x0001337C
		[DataSourceProperty]
		public override bool IsSelected
		{
			get
			{
				WCharacter troop = State.Troop;
				if (this.RowTroop == null || troop == null)
				{
					return false;
				}
				if (troop.IsCaptain && troop.BaseTroop != null)
				{
					return this.RowTroop == troop.BaseTroop;
				}
				return this.RowTroop == troop;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x000151DC File Offset: 0x000133DC
		[DataSourceProperty]
		public override bool IsEnabled
		{
			get
			{
				return this.RowTroop != null;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x000151EA File Offset: 0x000133EA
		[DataSourceProperty]
		public bool ShowPlayerIcon
		{
			get
			{
				return this.RowTroop == Player.Character;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x000151FC File Offset: 0x000133FC
		[DataSourceProperty]
		public bool ShowCompanionIcon
		{
			get
			{
				if (this.RowTroop == null)
				{
					return false;
				}
				WHero whero = this.RowTroop as WHero;
				return whero != null && whero.IsCompanion;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00015224 File Offset: 0x00013424
		[DataSourceProperty]
		public bool ShowRulerIcon
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				return rowTroop != null && rowTroop.IsRuler;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00015237 File Offset: 0x00013437
		[DataSourceProperty]
		public bool ShowClanLeaderIcon
		{
			get
			{
				WCharacter rowTroop = this.RowTroop;
				return rowTroop != null && rowTroop.IsClanLeader && !this.ShowRulerIcon && !this.ShowPlayerIcon;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00015260 File Offset: 0x00013460
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				if (this.IsPlaceholder)
				{
					return this.PlaceholderText ?? L.T("troop_list.placeholder", "No Troops Available").ToString();
				}
				WCharacter rowTroop = this.RowTroop;
				string text = (rowTroop != null) ? rowTroop.Name : null;
				WHero whero = this.RowTroop as WHero;
				string text2 = (whero != null) ? whero.Surname : null;
				string text3 = string.IsNullOrEmpty(text2) ? text : (text + " " + text2);
				WCharacter rowTroop2 = this.RowTroop;
				if (((rowTroop2 != null) ? rowTroop2.Parent : null) == null)
				{
					WCharacter rowTroop3 = this.RowTroop;
					if (rowTroop3 != null && !rowTroop3.IsMercenary)
					{
						return text3;
					}
				}
				int val = 0;
				WCharacter rowTroop4 = this.RowTroop;
				int num = Math.Max(val, (rowTroop4 != null) ? (rowTroop4.Tier - 1) : 0);
				return new string(' ', num * 4) + text3;
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00015338 File Offset: 0x00013538
		public override bool FilterMatch(string filter)
		{
			if (this.RowTroop == null)
			{
				return true;
			}
			string value = filter.Trim().ToLowerInvariant();
			return this.RowTroop.Name.ToString().ToLowerInvariant().Contains(value);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001537C File Offset: 0x0001357C
		[DataSourceMethod]
		public void ExecuteSelect()
		{
			State.UpdateTroop(this.RowTroop);
		}

		// Token: 0x040000FF RID: 255
		public readonly WCharacter RowTroop = rowTroop;

		// Token: 0x04000100 RID: 256
		private readonly string PlaceholderText = placeholderText;
	}
}
