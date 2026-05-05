using System;
using TaleWorlds.Engine;

namespace Retinues.Utils
{
	// Token: 0x0200002D RID: 45
	[SafeClass]
	public static class Sound
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00005BA4 File Offset: 0x00003DA4
		public static void Play2D(string eventPath)
		{
			if (string.IsNullOrEmpty(eventPath))
			{
				return;
			}
			try
			{
				SoundEvent.PlaySound2D(eventPath);
			}
			catch (Exception arg)
			{
				Log.Debug(string.Format("[Sound] Failed to play '{0}': {1}", eventPath, arg));
			}
		}

		// Token: 0x0400003B RID: 59
		public const string QuestFinished = "event:/ui/notification/quest_finished";

		// Token: 0x0400003C RID: 60
		public const string ReignDecision = "event:/ui/reign/decision";

		// Token: 0x0400003D RID: 61
		public const string TraitChange = "event:/ui/notification/trait_change";

		// Token: 0x0400003E RID: 62
		public const string Education = "event:/ui/notification/education";
	}
}
