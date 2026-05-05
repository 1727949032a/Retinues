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
	// Token: 0x02000068 RID: 104
	[ViewModelMixin]
	public sealed class UnitPageScreen : BaseViewModelMixin<EncyclopediaUnitPageVM>
	{
		// Token: 0x060001FD RID: 509 RVA: 0x0000EA58 File Offset: 0x0000CC58
		public UnitPageScreen(EncyclopediaUnitPageVM vm) : base(vm)
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

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000EAA0 File Offset: 0x0000CCA0
		[DataSourceProperty]
		public BasicTooltipViewModel EditorHint
		{
			get
			{
				return Tooltip.MakeTooltip(null, L.S("encyclopedia_retinues_link_hint", "Open in the global editor."));
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000EAB8 File Offset: 0x0000CCB8
		[DataSourceMethod]
		public void ExecuteOpenEditor()
		{
			try
			{
				CharacterObject characterObject = base.ViewModel.Obj as CharacterObject;
				if (characterObject != null)
				{
					WCharacter wcharacter = State.PendingTroop = new WCharacter(characterObject);
					State.PendingFaction = wcharacter.Faction;
					ClanScreen.LaunchEditor(wcharacter.IsCustom ? EditorMode.Personal : EditorMode.Culture);
				}
				else
				{
					ClanScreen.LaunchEditor(EditorMode.Culture);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
