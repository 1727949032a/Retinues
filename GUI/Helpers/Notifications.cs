using System;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Retinues.GUI.Helpers
{
	// Token: 0x02000065 RID: 101
	[SafeClass]
	public static class Notifications
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x0000E858 File Offset: 0x0000CA58
		public static void Popup(TextObject title, TextObject description, TextObject buttonText = null, bool pauseGame = true)
		{
			if (buttonText == null)
			{
				buttonText = GameTexts.FindText("str_ok", null);
			}
			InformationManager.ShowInquiry(new InquiryData(title.ToString(), description.ToString(), false, true, null, buttonText.ToString(), null, null, "", 0f, null, null, null), pauseGame, false);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
		public static void ConfirmationPopup(TextObject title, TextObject description, Action onConfirm, TextObject confirmText = null, TextObject cancelText = null, bool pauseGame = true)
		{
			if (confirmText == null)
			{
				confirmText = L.T("confirm", "Confirm");
			}
			if (cancelText == null)
			{
				cancelText = L.T("cancel", "Cancel");
			}
			InformationManager.ShowInquiry(new InquiryData(title.ToString(), description.ToString(), true, true, confirmText.ToString(), cancelText.ToString(), onConfirm, null, "", 0f, null, null, null), pauseGame, false);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000E915 File Offset: 0x0000CB15
		public static void Information(TextObject message, WCharacter announcer)
		{
			MBInformationManager.AddQuickInformation(message, 5000, announcer.Base, null, "");
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000E92E File Offset: 0x0000CB2E
		public static void Log(string message, string color = "#ffffffe0")
		{
			InformationManager.DisplayMessage(new InformationMessage(message, Color.ConvertStringToColor(color)));
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000E941 File Offset: 0x0000CB41
		public static void Log(TextObject message, string color = "#ffffffe0")
		{
			Notifications.Log(message.ToString(), color);
		}
	}
}
