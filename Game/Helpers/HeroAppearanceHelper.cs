using System;
using Helpers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A4 RID: 164
	[SafeClass]
	public static class HeroAppearanceHelper
	{
		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x00023A58 File Offset: 0x00021C58
		public static bool HasActiveSession
		{
			get
			{
				return HeroAppearanceHelper._currentTarget != null;
			}
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x00023A64 File Offset: 0x00021C64
		public static void OpenForHero(Hero hero, Action onClosed = null)
		{
			if (hero == null)
			{
				Log.Warn("[HeroAppearanceHelper] Tried to open appearance editor for null hero.");
				return;
			}
			if (Game.Current == null || Game.Current.GameStateManager == null)
			{
				Log.Warn("[HeroAppearanceHelper] Game or GameStateManager is null; cannot open appearance editor.");
				return;
			}
			if (Campaign.Current == null || !Campaign.Current.IsFaceGenEnabled)
			{
				Log.Warn("[HeroAppearanceHelper] FaceGen is disabled in this campaign.");
				return;
			}
			if (HeroAppearanceHelper._currentTarget != null)
			{
				Log.Warn("[HeroAppearanceHelper] A barber session is already active; ignoring new request.");
				return;
			}
			try
			{
				HeroAppearanceHelper._currentTarget = hero;
				HeroAppearanceHelper._onClosed = onClosed;
				HeroAppearanceHelper._needsPrime = true;
				HeroAppearanceHelper._hasPrimed = false;
				if (hero != Hero.MainHero)
				{
					HeroAppearanceHelper._savedMainBody = Hero.MainHero.BodyProperties;
					HeroAppearanceHelper._hasSavedMainBody = true;
					HeroAppearanceHelper._savedMainIsFemale = HeroAppearanceHelper.GetHeroIsFemale(Hero.MainHero);
					HeroAppearanceHelper._hasSavedMainGender = true;
					HeroAppearanceHelper.SetHeroIsFemale(Hero.MainHero, HeroAppearanceHelper.GetHeroIsFemale(hero));
					Hero mainHero = Hero.MainHero;
					BodyProperties bodyProperties = hero.BodyProperties;
					HeroAppearanceHelper.ApplyBodyPropertiesToHero(mainHero, bodyProperties);
				}
				string str = "[HeroAppearanceHelper] Session started for hero '";
				TextObject name = hero.Name;
				Log.Debug(str + (((name != null) ? name.ToString() : null) ?? hero.StringId) + "'.");
				IFaceGeneratorCustomFilter faceGeneratorFilter = CharacterHelper.GetFaceGeneratorFilter();
				BarberState gameState = Game.Current.GameStateManager.CreateState<BarberState>(new object[]
				{
					Hero.MainHero.CharacterObject,
					faceGeneratorFilter
				});
				GameStateManager.Current.PushState(gameState, 0);
				string str2 = "[HeroAppearanceHelper] Barber screen opened for target hero '";
				TextObject name2 = hero.Name;
				Log.Debug(str2 + (((name2 != null) ? name2.ToString() : null) ?? hero.StringId) + "'.");
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("[HeroAppearanceHelper] Failed to open appearance editor. Exception: {0}", arg));
				HeroAppearanceHelper.ClearSession(true);
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x00023C0C File Offset: 0x00021E0C
		internal static void PrimeFaceGen(IFaceGeneratorHandler handler)
		{
			if (!HeroAppearanceHelper._needsPrime || HeroAppearanceHelper._hasPrimed || handler == null)
			{
				return;
			}
			try
			{
				handler.DefaultFace();
				handler.ChangeToFaceCamera();
				HeroAppearanceHelper._hasPrimed = true;
				HeroAppearanceHelper._needsPrime = false;
				Log.Debug("[HeroAppearanceHelper] PrimeFaceGen: DefaultFace + camera/dress.");
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00023C70 File Offset: 0x00021E70
		public static void OnBarberClosed()
		{
			if (HeroAppearanceHelper._currentTarget == null)
			{
				Log.Debug("[HeroAppearanceHelper] OnBarberClosed called with no active session (ignored).");
				return;
			}
			string str = "[HeroAppearanceHelper] OnBarberClosed for '";
			TextObject name = HeroAppearanceHelper._currentTarget.Name;
			Log.Debug(str + (((name != null) ? name.ToString() : null) ?? HeroAppearanceHelper._currentTarget.StringId) + "'.");
			try
			{
				BodyProperties bodyProperties = Hero.MainHero.BodyProperties;
				bool heroIsFemale = HeroAppearanceHelper.GetHeroIsFemale(Hero.MainHero);
				HeroAppearanceHelper.ApplyBodyPropertiesToHero(HeroAppearanceHelper._currentTarget, bodyProperties);
				HeroAppearanceHelper.SetHeroIsFemale(HeroAppearanceHelper._currentTarget, heroIsFemale);
				string str2 = "[HeroAppearanceHelper] Applied edited body/gender to hero '";
				TextObject name2 = HeroAppearanceHelper._currentTarget.Name;
				Log.Debug(str2 + (((name2 != null) ? name2.ToString() : null) ?? HeroAppearanceHelper._currentTarget.StringId) + "'.");
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("[HeroAppearanceHelper] Error while applying edited body/gender: {0}", arg));
			}
			finally
			{
				Action onClosed = HeroAppearanceHelper._onClosed;
				HeroAppearanceHelper.ClearSession(true);
				try
				{
					if (onClosed != null)
					{
						onClosed();
					}
				}
				catch (Exception arg2)
				{
					Log.Error(string.Format("[HeroAppearanceHelper] Error in OnClosed callback: {0}", arg2));
				}
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00023D98 File Offset: 0x00021F98
		private static void ClearSession(bool restoreMain)
		{
			if (restoreMain && Hero.MainHero != null)
			{
				try
				{
					if (HeroAppearanceHelper._hasSavedMainBody)
					{
						HeroAppearanceHelper.ApplyBodyPropertiesToHero(Hero.MainHero, HeroAppearanceHelper._savedMainBody);
						Log.Debug("[HeroAppearanceHelper] Restored main hero body after barber.");
					}
					if (HeroAppearanceHelper._hasSavedMainGender)
					{
						HeroAppearanceHelper.SetHeroIsFemale(Hero.MainHero, HeroAppearanceHelper._savedMainIsFemale);
						Log.Debug("[HeroAppearanceHelper] Restored main hero gender after barber.");
					}
				}
				catch (Exception arg)
				{
					Log.Error(string.Format("[HeroAppearanceHelper] Failed to restore main hero appearance: {0}", arg));
				}
			}
			Log.Debug("[HeroAppearanceHelper] Clearing session.");
			HeroAppearanceHelper._currentTarget = null;
			HeroAppearanceHelper._savedMainBody = default(BodyProperties);
			HeroAppearanceHelper._hasSavedMainBody = false;
			HeroAppearanceHelper._savedMainIsFemale = false;
			HeroAppearanceHelper._hasSavedMainGender = false;
			HeroAppearanceHelper._onClosed = null;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00023E48 File Offset: 0x00022048
		private static void ApplyBodyPropertiesToHero(Hero hero, in BodyProperties body)
		{
			if (hero == null)
			{
				return;
			}
			try
			{
				BodyProperties bodyProperties = body;
				DynamicBodyProperties dynamicProperties = bodyProperties.DynamicProperties;
				bodyProperties = body;
				StaticBodyProperties staticProperties = bodyProperties.StaticProperties;
				hero.StaticBodyProperties = staticProperties;
				hero.SetBirthDay(CampaignTime.YearsFromNow(-dynamicProperties.Age));
				hero.Weight = dynamicProperties.Weight;
				hero.Build = dynamicProperties.Build;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00023EC8 File Offset: 0x000220C8
		private static bool GetHeroIsFemale(Hero hero)
		{
			return hero != null && hero.IsFemale;
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x00023ED5 File Offset: 0x000220D5
		private static void SetHeroIsFemale(Hero hero, bool value)
		{
			if (hero == null)
			{
				return;
			}
			hero.IsFemale = value;
		}

		// Token: 0x040001B9 RID: 441
		private static Hero _currentTarget;

		// Token: 0x040001BA RID: 442
		private static BodyProperties _savedMainBody;

		// Token: 0x040001BB RID: 443
		private static bool _hasSavedMainBody;

		// Token: 0x040001BC RID: 444
		private static bool _savedMainIsFemale;

		// Token: 0x040001BD RID: 445
		private static bool _hasSavedMainGender;

		// Token: 0x040001BE RID: 446
		private static bool _needsPrime;

		// Token: 0x040001BF RID: 447
		private static bool _hasPrimed;

		// Token: 0x040001C0 RID: 448
		private static Action _onClosed;
	}
}
