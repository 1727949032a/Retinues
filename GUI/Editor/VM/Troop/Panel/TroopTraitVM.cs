using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Retinues.Game.Wrappers;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Editor.VM.Troop.Panel
{
	// Token: 0x0200007E RID: 126
	[SafeClass]
	public sealed class TroopTraitVM : BaseVM
	{
		// Token: 0x0600037A RID: 890 RVA: 0x0001417B File Offset: 0x0001237B
		public TroopTraitVM(TraitObject trait)
		{
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600037B RID: 891 RVA: 0x0001418C File Offset: 0x0001238C
		protected override Dictionary<UIEvent, string[]> EventMap
		{
			get
			{
				Dictionary<UIEvent, string[]> dictionary = new Dictionary<UIEvent, string[]>();
				dictionary[UIEvent.Troop] = new string[]
				{
					"Value",
					"ValueText",
					"Sprite",
					"SpriteColor",
					"CanIncrement",
					"CanDecrement"
				};
				return dictionary;
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600037C RID: 892 RVA: 0x000141DD File Offset: 0x000123DD
		private WHero Hero
		{
			get
			{
				return State.Troop as WHero;
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600037D RID: 893 RVA: 0x000141E9 File Offset: 0x000123E9
		public int Value
		{
			get
			{
				WHero hero = this.Hero;
				if (hero == null)
				{
					return 0;
				}
				return hero.GetTrait(this._trait);
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00014202 File Offset: 0x00012402
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._trait.Name.ToString();
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600037F RID: 895 RVA: 0x00014214 File Offset: 0x00012414
		[DataSourceProperty]
		public string ValueText
		{
			get
			{
				return this.Value.ToString("+#;-#;0");
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000380 RID: 896 RVA: 0x00014234 File Offset: 0x00012434
		[DataSourceProperty]
		public string Sprite
		{
			get
			{
				int value = this.Value;
				int num = (value == 0) ? 1 : value;
				string stringId = this._trait.StringId;
				return string.Format("SPGeneral\\SPTraits\\{0}_{1}", stringId.ToLower(), num);
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00014274 File Offset: 0x00012474
		[DataSourceProperty]
		public string SpriteColor
		{
			get
			{
				switch (this.Value)
				{
				case -2:
					return "#ff4d4dff";
				case -1:
					return "#ff9999ff";
				case 1:
					return "#f7db5eff";
				case 2:
					return "#ffdb4dff";
				}
				return "#c0c0c0ff";
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000382 RID: 898 RVA: 0x000142CE File Offset: 0x000124CE
		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				string title = null;
				TextObject description = this._trait.Description;
				return Tooltip.MakeTooltip(title, ((description != null) ? description.ToString() : null) ?? string.Empty);
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000383 RID: 899 RVA: 0x000142F6 File Offset: 0x000124F6
		[DataSourceProperty]
		public bool CanIncrement
		{
			get
			{
				return this.Value < this._trait.MaxValue;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000384 RID: 900 RVA: 0x0001430B File Offset: 0x0001250B
		[DataSourceProperty]
		public bool CanDecrement
		{
			get
			{
				return this.Value > this._trait.MinValue;
			}
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00014320 File Offset: 0x00012520
		[DataSourceMethod]
		public void ExecuteIncrement()
		{
			this.Change(1);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00014329 File Offset: 0x00012529
		[DataSourceMethod]
		public void ExecuteDecrement()
		{
			this.Change(-1);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00014334 File Offset: 0x00012534
		private void Change(int delta)
		{
			if (delta > 0 && !this.CanIncrement)
			{
				return;
			}
			if (delta < 0 && !this.CanDecrement)
			{
				return;
			}
			WHero hero = this.Hero;
			int value = ((hero != null) ? hero.GetTrait(this._trait) : 0) + delta;
			WHero hero2 = this.Hero;
			if (hero2 != null)
			{
				hero2.SetTrait(this._trait, value);
			}
			base.OnPropertyChanged("Value");
			base.OnPropertyChanged("ValueText");
			base.OnPropertyChanged("Sprite");
			base.OnPropertyChanged("SpriteColor");
			base.OnPropertyChanged("CanIncrement");
			base.OnPropertyChanged("CanDecrement");
		}

		// Token: 0x040000F3 RID: 243
		private readonly TraitObject _trait = trait;
	}
}
