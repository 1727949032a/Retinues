using System;
using System.Runtime.CompilerServices;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;

namespace Retinues.Game.Menu
{
	// Token: 0x0200009E RID: 158
	[SafeClass]
	public static class TimedWaitMenu
	{
		// Token: 0x060006ED RID: 1773 RVA: 0x000226FC File Offset: 0x000208FC
		public static void Start(CampaignGameStarter starter, string idSuffix, string title, float durationHours, Action onCompleted, Action onAborted, GameMenu.MenuOverlayType overlay = GameMenu.MenuOverlayType.None, Action<float> onWholeHour = null)
		{
			if (starter == null)
			{
				throw new ArgumentNullException("starter");
			}
			if (durationHours <= 0f)
			{
				durationHours = 0.01f;
			}
			Log.Debug(string.Format("TimedWaitMenu.Start called: {0}, {1:0.##}h, title='{2}'", idSuffix, durationHours, title));
			TimedWaitMenu._menuId = string.Format("ret_wait_{0}_{1}", idSuffix, ++TimedWaitMenu._runSeq);
			TimedWaitMenu._targetHours = durationHours;
			TimedWaitMenu._progressHours = 0f;
			TimedWaitMenu._onCompleted = onCompleted;
			TimedWaitMenu._onAborted = onAborted;
			TimedWaitMenu._onWholeHour = onWholeHour;
			TimedWaitMenu._kickUnpauseOnce = true;
			Campaign campaign = Campaign.Current;
			string text;
			if (campaign == null)
			{
				text = null;
			}
			else
			{
				MenuContext currentMenuContext = campaign.CurrentMenuContext;
				if (currentMenuContext == null)
				{
					text = null;
				}
				else
				{
					GameMenu gameMenu = currentMenuContext.GameMenu;
					text = ((gameMenu != null) ? gameMenu.StringId : null);
				}
			}
			TimedWaitMenu._returnMenuId = (text ?? TimedWaitMenu.GuessFallbackMenu());
			TimedWaitMenu.AddOrReplaceWaitMenu(starter, title, overlay);
			TimedWaitMenu.OpenWaitMenu();
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x000227D0 File Offset: 0x000209D0
		private static void AddOrReplaceWaitMenu(CampaignGameStarter starter, string title, GameMenu.MenuOverlayType overlay)
		{
			Log.Debug(string.Concat(new string[]
			{
				"TimedWaitMenu.AddOrReplaceWaitMenu called: ",
				TimedWaitMenu._menuId,
				", title='",
				title,
				"'"
			}));
			string menuId = TimedWaitMenu._menuId;
			OnInitDelegate initDelegate;
			if ((initDelegate = TimedWaitMenu.<>O.<0>__WaitMenu_OnInit) == null)
			{
				initDelegate = (TimedWaitMenu.<>O.<0>__WaitMenu_OnInit = new OnInitDelegate(TimedWaitMenu.WaitMenu_OnInit));
			}
			OnConditionDelegate condition;
			if ((condition = TimedWaitMenu.<>O.<1>__WaitMenu_OnCondition) == null)
			{
				condition = (TimedWaitMenu.<>O.<1>__WaitMenu_OnCondition = new OnConditionDelegate(TimedWaitMenu.WaitMenu_OnCondition));
			}
			OnConsequenceDelegate consequence;
			if ((consequence = TimedWaitMenu.<>O.<2>__WaitMenu_OnConsequence) == null)
			{
				consequence = (TimedWaitMenu.<>O.<2>__WaitMenu_OnConsequence = new OnConsequenceDelegate(TimedWaitMenu.WaitMenu_OnConsequence));
			}
			OnTickDelegate tick;
			if ((tick = TimedWaitMenu.<>O.<3>__WaitMenu_OnTick) == null)
			{
				tick = (TimedWaitMenu.<>O.<3>__WaitMenu_OnTick = new OnTickDelegate(TimedWaitMenu.WaitMenu_OnTick));
			}
			starter.AddWaitGameMenu(menuId, title, initDelegate, condition, consequence, tick, GameMenu.MenuAndOptionType.WaitMenuShowProgressAndHoursOption, overlay, TimedWaitMenu._targetHours, GameMenu.MenuFlags.None, null);
			string menuId2 = TimedWaitMenu._menuId;
			string optionId = "ret_wait_cancel";
			string optionText = L.S("cancel", "Cancel");
			GameMenuOption.OnConditionDelegate condition2;
			if ((condition2 = TimedWaitMenu.<>O.<4>__WaitMenu_Cancel_OnCondition) == null)
			{
				condition2 = (TimedWaitMenu.<>O.<4>__WaitMenu_Cancel_OnCondition = new GameMenuOption.OnConditionDelegate(TimedWaitMenu.WaitMenu_Cancel_OnCondition));
			}
			GameMenuOption.OnConsequenceDelegate consequence2;
			if ((consequence2 = TimedWaitMenu.<>O.<5>__WaitMenu_Cancel_OnConsequence) == null)
			{
				consequence2 = (TimedWaitMenu.<>O.<5>__WaitMenu_Cancel_OnConsequence = new GameMenuOption.OnConsequenceDelegate(TimedWaitMenu.WaitMenu_Cancel_OnConsequence));
			}
			starter.AddGameMenuOption(menuId2, optionId, optionText, condition2, consequence2, true, 0, false, null);
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x000228EB File Offset: 0x00020AEB
		private static void OpenWaitMenu()
		{
			Log.Debug("TimedWaitMenu.OpenWaitMenu called: " + TimedWaitMenu._menuId);
			GameMenu.SwitchToMenu(TimedWaitMenu._menuId);
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0002290B File Offset: 0x00020B0B
		private static void WaitMenu_OnInit(MenuCallbackArgs args)
		{
			Log.Debug("TimedWaitMenu.WaitMenu_OnInit called: " + TimedWaitMenu._menuId);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = true;
			}
			TimedWaitMenu._nextWholeHour = 1f;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0002293D File Offset: 0x00020B3D
		private static bool WaitMenu_OnCondition(MenuCallbackArgs args)
		{
			return true;
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x00022940 File Offset: 0x00020B40
		private static void WaitMenu_OnTick(MenuCallbackArgs args, CampaignTime dt)
		{
			if (TimedWaitMenu._kickUnpauseOnce)
			{
				TimedWaitMenu._kickUnpauseOnce = false;
				TimedWaitMenu.EnsureUnpaused();
			}
			TimedWaitMenu._progressHours += (float)dt.ToHours;
			while (TimedWaitMenu._onWholeHour != null && TimedWaitMenu._progressHours >= TimedWaitMenu._nextWholeHour && TimedWaitMenu._nextWholeHour <= TimedWaitMenu._targetHours)
			{
				TimedWaitMenu._onWholeHour(TimedWaitMenu._nextWholeHour);
				TimedWaitMenu._nextWholeHour += 1f;
			}
			float progressOfWaitingInMenu = (TimedWaitMenu._targetHours > 0f) ? ((float)Math.Min(1.0, (double)(TimedWaitMenu._progressHours / TimedWaitMenu._targetHours))) : 1f;
			MenuContext menuContext = args.MenuContext;
			if (menuContext != null)
			{
				GameMenu gameMenu = menuContext.GameMenu;
				if (gameMenu != null)
				{
					gameMenu.SetProgressOfWaitingInMenu(progressOfWaitingInMenu);
				}
			}
			if (TimedWaitMenu._progressHours >= TimedWaitMenu._targetHours)
			{
				TimedWaitMenu.Finish(true);
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00022A10 File Offset: 0x00020C10
		private static void WaitMenu_OnConsequence(MenuCallbackArgs args)
		{
			Log.Debug("TimedWaitMenu.WaitMenu_OnConsequence called: " + TimedWaitMenu._menuId);
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00022A26 File Offset: 0x00020C26
		private static bool WaitMenu_Cancel_OnCondition(MenuCallbackArgs args)
		{
			return true;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00022A29 File Offset: 0x00020C29
		private static void WaitMenu_Cancel_OnConsequence(MenuCallbackArgs args)
		{
			Log.Debug("TimedWaitMenu.WaitMenu_Cancel_OnConsequence called: " + TimedWaitMenu._menuId);
			TimedWaitMenu.Finish(false);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00022A48 File Offset: 0x00020C48
		private static void Finish(bool completed)
		{
			Log.Debug(string.Format("TimedWaitMenu.Finish called: {0}, completed={1}", TimedWaitMenu._menuId, completed));
			int runSeq = TimedWaitMenu._runSeq;
			string menuId = TimedWaitMenu._menuId;
			string text = string.IsNullOrEmpty(TimedWaitMenu._returnMenuId) ? TimedWaitMenu.GuessFallbackMenu() : TimedWaitMenu._returnMenuId;
			try
			{
				if (completed)
				{
					Action onCompleted = TimedWaitMenu._onCompleted;
					if (onCompleted != null)
					{
						onCompleted();
					}
				}
				else
				{
					Action onAborted = TimedWaitMenu._onAborted;
					if (onAborted != null)
					{
						onAborted();
					}
				}
				if (TimedWaitMenu._runSeq == runSeq && !(TimedWaitMenu._menuId != menuId))
				{
					if (PlayerEncounter.Current != null)
					{
						PlayerEncounter.Current.IsPlayerWaiting = false;
					}
					if (!string.IsNullOrEmpty(text))
					{
						GameMenu.SwitchToMenu(text);
					}
				}
			}
			finally
			{
				if (TimedWaitMenu._runSeq == runSeq && TimedWaitMenu._menuId == menuId)
				{
					TimedWaitMenu._menuId = (TimedWaitMenu._returnMenuId = null);
					TimedWaitMenu._onCompleted = (TimedWaitMenu._onAborted = null);
					TimedWaitMenu._targetHours = (TimedWaitMenu._progressHours = 0f);
					TimedWaitMenu._onWholeHour = null;
					TimedWaitMenu._nextWholeHour = 1f;
				}
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00022B54 File Offset: 0x00020D54
		private static string GuessFallbackMenu()
		{
			Log.Debug("TimedWaitMenu.GuessFallbackMenu called.");
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null)
			{
				if (currentSettlement.IsTown)
				{
					return "town";
				}
				if (currentSettlement.IsCastle)
				{
					return "castle";
				}
				if (currentSettlement.IsVillage)
				{
					return "village";
				}
				if (currentSettlement.IsHideout)
				{
					return "hideout_place";
				}
			}
			return "town";
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00022BB4 File Offset: 0x00020DB4
		private static void EnsureUnpaused()
		{
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return;
			}
			if (campaign.TimeControlMode == CampaignTimeControlMode.Stop || campaign.TimeControlMode == CampaignTimeControlMode.FastForwardStop)
			{
				campaign.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForwardForPartyWaitTime;
			}
		}

		// Token: 0x040001A0 RID: 416
		private static string _menuId;

		// Token: 0x040001A1 RID: 417
		private static string _returnMenuId;

		// Token: 0x040001A2 RID: 418
		private static float _targetHours;

		// Token: 0x040001A3 RID: 419
		private static float _progressHours;

		// Token: 0x040001A4 RID: 420
		private static float _nextWholeHour = 1f;

		// Token: 0x040001A5 RID: 421
		private static int _runSeq = 0;

		// Token: 0x040001A6 RID: 422
		private static Action _onCompleted;

		// Token: 0x040001A7 RID: 423
		private static Action _onAborted;

		// Token: 0x040001A8 RID: 424
		private static Action<float> _onWholeHour;

		// Token: 0x040001A9 RID: 425
		private static bool _kickUnpauseOnce;

		// Token: 0x02000174 RID: 372
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400046A RID: 1130
			public static OnInitDelegate <0>__WaitMenu_OnInit;

			// Token: 0x0400046B RID: 1131
			public static OnConditionDelegate <1>__WaitMenu_OnCondition;

			// Token: 0x0400046C RID: 1132
			public static OnConsequenceDelegate <2>__WaitMenu_OnConsequence;

			// Token: 0x0400046D RID: 1133
			public static OnTickDelegate <3>__WaitMenu_OnTick;

			// Token: 0x0400046E RID: 1134
			public static GameMenuOption.OnConditionDelegate <4>__WaitMenu_Cancel_OnCondition;

			// Token: 0x0400046F RID: 1135
			public static GameMenuOption.OnConsequenceDelegate <5>__WaitMenu_Cancel_OnConsequence;
		}
	}
}
