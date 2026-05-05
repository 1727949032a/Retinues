using System;
using Retinues.Game.Helpers;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;

namespace Retinues.GUI.Helpers
{
	// Token: 0x02000063 RID: 99
	[SafeClass]
	public static class AppearanceGuard
	{
		// Token: 0x060001EF RID: 495 RVA: 0x0000E528 File Offset: 0x0000C728
		public static bool TryApply(WCharacter troop, int equipmentIndex, Func<bool> applyChange, Action onSuccess = null, bool retryOnFailure = true)
		{
			if (troop == null)
			{
				return false;
			}
			if (applyChange == null)
			{
				return false;
			}
			AppearanceGuard.Snapshot snapshot = new AppearanceGuard.Snapshot(troop);
			bool result;
			try
			{
				CharacterViewModel characterViewModel;
				Exception ex;
				if (!applyChange())
				{
					result = false;
				}
				else if (!troop.TryGetModel(equipmentIndex, out characterViewModel, out ex))
				{
					if (ex is AccessViolationException)
					{
						Log.Info("AccessViolation detected when applying appearance change.");
						snapshot.Restore(troop);
						if (retryOnFailure)
						{
							Log.Info("Retrying appearance change.");
							return AppearanceGuard.TryApply(troop, equipmentIndex, applyChange, onSuccess, false);
						}
						AppearanceGuard.ShowFailurePopup();
					}
					else
					{
						snapshot.Restore(troop);
						Notifications.Popup(L.T("appearance_error_title", "Appearance Error"), L.T("appearance_error_body", "An error occurred while updating this troop's appearance. The previous appearance has been restored. See log for details."), null, true);
					}
					if (onSuccess != null)
					{
						onSuccess();
					}
					result = false;
				}
				else if (characterViewModel == null)
				{
					Log.Info("Null model detected when applying appearance change.");
					if (retryOnFailure)
					{
						Log.Info("Retrying appearance change.");
						result = AppearanceGuard.TryApply(troop, equipmentIndex, applyChange, onSuccess, false);
					}
					else
					{
						snapshot.Restore(troop);
						AppearanceGuard.ShowFailurePopup();
						if (onSuccess != null)
						{
							onSuccess();
						}
						result = false;
					}
				}
				else
				{
					if (onSuccess != null)
					{
						onSuccess();
					}
					result = true;
				}
			}
			catch (Exception ex2)
			{
				Log.Exception(ex2, "", null);
				snapshot.Restore(troop);
				Notifications.Popup(L.T("appearance_error_title", "Appearance Error"), L.T("appearance_error_body", "An error occurred while updating this troop's appearance. The previous appearance has been restored. See log for details."), null, true);
				if (onSuccess != null)
				{
					onSuccess();
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000E68C File Offset: 0x0000C88C
		private static void ShowFailurePopup()
		{
			TextObject title;
			TextObject description;
			if (AppearanceGuard.HasAlternateSpecies())
			{
				title = L.T("no_valid_model_title_species", "No Valid Model");
				description = L.T("no_valid_model_body_species", "This combination of gender, culture and species does not have a valid model.\n\nThe previous appearance has been restored.");
			}
			else
			{
				title = L.T("no_valid_model_title", "No Valid Model");
				description = L.T("no_valid_model_body", "This combination of gender and culture does not have a valid model.\n\nThe previous appearance has been restored.");
			}
			Notifications.Popup(title, description, null, true);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000E6EC File Offset: 0x0000C8EC
		private static bool HasAlternateSpecies()
		{
			bool result;
			try
			{
				string[] raceNames = FaceGen.GetRaceNames();
				result = (raceNames != null && raceNames.Length > 1);
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = false;
			}
			return result;
		}

		// Token: 0x02000139 RID: 313
		public readonly struct Snapshot
		{
			// Token: 0x06000AC4 RID: 2756 RVA: 0x00031990 File Offset: 0x0002FB90
			public Snapshot(WCharacter troop)
			{
				if (troop == null)
				{
					this.Culture = null;
					this.Race = -1;
					this.IsFemale = false;
					return;
				}
				WCulture culture = troop.Culture;
				this.Culture = ((culture != null) ? culture.Base : null);
				this.Race = troop.Race;
				this.IsFemale = troop.IsFemale;
			}

			// Token: 0x06000AC5 RID: 2757 RVA: 0x000319EC File Offset: 0x0002FBEC
			public void Restore(WCharacter troop)
			{
				if (troop == null)
				{
					return;
				}
				try
				{
					if (this.Culture != null)
					{
						troop.Culture = new WCulture(this.Culture);
						BodyHelper.ApplyPropertiesFromCulture(troop, this.Culture);
					}
					if (this.Race >= 0)
					{
						troop.Race = this.Race;
					}
					troop.IsFemale = this.IsFemale;
					troop.Body.EnsureOwnBodyRange();
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
			}

			// Token: 0x0400038E RID: 910
			public readonly CultureObject Culture;

			// Token: 0x0400038F RID: 911
			public readonly int Race;

			// Token: 0x04000390 RID: 912
			public readonly bool IsFemale;
		}
	}
}
