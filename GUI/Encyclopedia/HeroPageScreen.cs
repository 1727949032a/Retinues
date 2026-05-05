using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.GUI.Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace Retinues.GUI.Encyclopedia
{
	// Token: 0x02000067 RID: 103
	[ViewModelMixin]
	public sealed class HeroPageScreen : BaseViewModelMixin<EncyclopediaHeroPageVM>
	{
		// Token: 0x060001FA RID: 506 RVA: 0x0000E974 File Offset: 0x0000CB74
		public HeroPageScreen(EncyclopediaHeroPageVM vm) : base(vm)
		{
			try
			{
				SpriteLoader.LoadCategories(new string[]
				{
					"ui_clan"
				});
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000E9BC File Offset: 0x0000CBBC
		[DataSourceProperty]
		public BasicTooltipViewModel EditorHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("encyclopedia_retinues_link_hint", "Open in the global editor."));
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000E9D4 File Offset: 0x0000CBD4
		[DataSourceMethod]
		public void ExecuteOpenEditor()
		{
			try
			{
				Hero hero = base.ViewModel.Obj as Hero;
				if (hero != null)
				{
					State.PendingTroop = new WHero(hero);
					State.PendingFaction = ((hero.Clan != null) ? new WClan(hero.Clan) : ((hero.Culture != null) ? new WCulture(hero.Culture) : null));
				}
				ClanScreen.LaunchEditor(EditorMode.Heroes);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
