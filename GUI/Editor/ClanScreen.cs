using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using Retinues.GUI.Editor.VM;
using Retinues.Mods;
using Retinues.Utils;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor
{
	// Token: 0x02000070 RID: 112
	[ViewModelMixin("RefreshValues", true)]
	public sealed class ClanScreen : BaseViewModelMixin<ClanManagementVM>
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000EE9B File Offset: 0x0000D09B
		// (set) Token: 0x0600022A RID: 554 RVA: 0x0000EEA2 File Offset: 0x0000D0A2
		public static EditorMode EditorMode { get; set; } = EditorMode.Personal;

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600022B RID: 555 RVA: 0x0000EEAA File Offset: 0x0000D0AA
		public static bool IsStudioMode
		{
			get
			{
				return ClanScreen.EditorMode != EditorMode.Personal;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000EEB8 File Offset: 0x0000D0B8
		public static void LaunchEditor(EditorMode mode = EditorMode.Culture)
		{
			try
			{
				ClanScreen.EditorMode = mode;
				Game game = Game.Current;
				GameStateManager gameStateManager = (game != null) ? game.GameStateManager : null;
				if (gameStateManager != null)
				{
					ClanState gameState = gameStateManager.CreateState<ClanState>();
					gameStateManager.PushState(gameState, 0);
					if (mode == EditorMode.Personal)
					{
						ClanScreen instance = ClanScreen.Instance;
						if (instance != null)
						{
							instance.SelectEditorTab();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600022D RID: 557 RVA: 0x0000EF24 File Offset: 0x0000D124
		// (set) Token: 0x0600022E RID: 558 RVA: 0x0000EF2B File Offset: 0x0000D12B
		public static ClanScreen Instance { get; private set; }

		// Token: 0x0600022F RID: 559 RVA: 0x0000EF34 File Offset: 0x0000D134
		public ClanScreen(ClanManagementVM vm) : base(vm)
		{
			try
			{
				Log.Info("Initializing ClanTroopScreen...");
				try
				{
					SpriteLoader.LoadCategories(new string[]
					{
						"ui_charactercreation",
						"ui_characterdeveloper",
						"ui_inventory"
					});
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
				State.ResetAll();
				this.Editor = new EditorVM();
				base.ViewModel.PropertyChangedWithBoolValue += this.OnVanillaTabChanged;
				ClanHotkeyGate.Active = true;
				ClanHotkeyGate.RequireShift = false;
				if (ClanScreen.IsStudioMode)
				{
					this.SelectEditorTab();
				}
				ClanScreen.Instance = this;
				Log.Info("ClanTroopScreen initialized.");
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000F004 File Offset: 0x0000D204
		public override void OnFinalize()
		{
			try
			{
				ClanScreen.EditorMode = EditorMode.Personal;
				ClanHotkeyGate.Active = false;
				base.OnFinalize();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000231 RID: 561 RVA: 0x0000F044 File Offset: 0x0000D244
		// (set) Token: 0x06000232 RID: 562 RVA: 0x0000F04C File Offset: 0x0000D24C
		[DataSourceProperty]
		public EditorVM Editor { get; private set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000233 RID: 563 RVA: 0x0000F055 File Offset: 0x0000D255
		[DataSourceProperty]
		public string TroopsTabText
		{
			get
			{
				return L.S("troops_tab_text", "Troops");
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000F066 File Offset: 0x0000D266
		[DataSourceProperty]
		public bool IsTroopsSelected
		{
			get
			{
				EditorVM editor = this.Editor;
				return editor != null && editor.IsVisible;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000235 RID: 565 RVA: 0x0000F079 File Offset: 0x0000D279
		[DataSourceProperty]
		public bool IsTopPanelVisible
		{
			get
			{
				return !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000F083 File Offset: 0x0000D283
		[DataSourceProperty]
		public bool IsFinancePanelVisible
		{
			get
			{
				return !this.IsTroopsSelected && !ClanScreen.IsStudioMode;
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000F098 File Offset: 0x0000D298
		[DataSourceMethod]
		[SafeMethod(null, true, null)]
		public void ExecuteSelectTroops()
		{
			try
			{
				EditorVM editor = this.Editor;
				if (editor == null || !editor.IsVisible)
				{
					this.SelectEditorTab();
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000F0E4 File Offset: 0x0000D2E4
		[DataSourceMethod]
		public void ExecuteOpenPlayerMode()
		{
			this.OpenEditor(EditorMode.Personal);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000F0ED File Offset: 0x0000D2ED
		[DataSourceMethod]
		public void ExecuteOpenStudioMode()
		{
			this.OpenEditor(EditorMode.Culture);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000F0F6 File Offset: 0x0000D2F6
		private void OpenEditor(EditorMode mode = EditorMode.Personal)
		{
			Log.Info(string.Format("Opening Editor in mode: {0}", mode));
			ClanScreen.EditorMode = mode;
			State.ResetAll();
			this.Editor = new EditorVM();
			base.OnPropertyChanged("Editor");
			this.SelectEditorTab();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000F134 File Offset: 0x0000D334
		private void SelectEditorTab()
		{
			if (ModCompatibility.ForceClanTabsReset)
			{
				this.ForceResetExternalTabs();
			}
			this.UnselectVanillaTabs();
			this.Editor.Show();
			this.UpdateVisibilityFlags();
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000F15A File Offset: 0x0000D35A
		private void HideEditor()
		{
			this.Editor.Hide();
			this.UpdateVisibilityFlags();
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000F170 File Offset: 0x0000D370
		public void UnselectVanillaTabs()
		{
			try
			{
				base.ViewModel.IsMembersSelected = false;
				base.ViewModel.IsFiefsSelected = false;
				base.ViewModel.IsPartiesSelected = false;
				base.ViewModel.IsIncomeSelected = false;
				base.ViewModel.ClanMembers.IsSelected = false;
				base.ViewModel.ClanParties.IsSelected = false;
				base.ViewModel.ClanFiefs.IsSelected = false;
				base.ViewModel.ClanIncome.IsSelected = false;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000F210 File Offset: 0x0000D410
		private void UpdateVisibilityFlags()
		{
			base.OnPropertyChanged("IsTroopsSelected");
			base.OnPropertyChanged("IsTopPanelVisible");
			base.OnPropertyChanged("IsFinancePanelVisible");
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000F234 File Offset: 0x0000D434
		private void OnVanillaTabChanged(object sender, PropertyChangedWithBoolValueEventArgs e)
		{
			try
			{
				if (e.Value)
				{
					string propertyName = e.PropertyName;
					if (propertyName == "IsMembersSelected" || propertyName == "IsFiefsSelected" || propertyName == "IsPartiesSelected" || propertyName == "IsIncomeSelected" || propertyName == "CourtSelected" || propertyName == "DemesneSelected")
					{
						Log.Debug("Vanilla tab selected (" + e.PropertyName + "), hiding troop editor.");
						this.HideEditor();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000F2E8 File Offset: 0x0000D4E8
		private void ForceResetExternalTabs()
		{
			try
			{
				base.ViewModel.SetSelectedCategory(0);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}
	}
}
