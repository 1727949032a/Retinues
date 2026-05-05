using System;
using System.Collections.Generic;
using System.Reflection;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Retinues.Safety.Sanitizer
{
	// Token: 0x02000042 RID: 66
	[SafeClass]
	public static class PartySanitizer
	{
		// Token: 0x06000154 RID: 340 RVA: 0x00009A00 File Offset: 0x00007C00
		public static void SanitizeParty(MobileParty mp, bool replaceAllCustom = false)
		{
			if (mp == null)
			{
				return;
			}
			PartySanitizer.SanitizeRoster(mp.MemberRoster, mp, replaceAllCustom);
			PartySanitizer.SanitizeRoster(mp.PrisonRoster, mp, replaceAllCustom);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00009A20 File Offset: 0x00007C20
		public static void SanitizeRoster(TroopRoster roster, MobileParty party = null, bool replaceAllCustom = false)
		{
			if (roster == null)
			{
				return;
			}
			List<Action> list = new List<Action>();
			try
			{
				for (int i = roster.Count - 1; i >= 0; i--)
				{
					TroopRosterElement e = roster.GetElementCopyAtIndex(i);
					if (!SanitizerBehavior.IsCharacterValid(e.Character, replaceAllCustom))
					{
						string format = "Invalid troop '{0}' found at index {1} in {2}.";
						CharacterObject character = e.Character;
						object arg = ((character != null) ? character.StringId : null) ?? "NULL";
						object arg2 = i;
						object obj;
						if (party == null)
						{
							obj = null;
						}
						else
						{
							TextObject name = party.Name;
							obj = ((name != null) ? name.ToString() : null);
						}
						Log.Warn(string.Format(format, arg, arg2, obj ?? "unknown party"));
						list.Add(delegate
						{
							PartySanitizer.ReplaceInvalidTroop(roster, e);
						});
					}
				}
				foreach (Action action in list)
				{
					action();
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Failed while cleaning roster", null);
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00009B60 File Offset: 0x00007D60
		private static void ReplaceInvalidTroop(TroopRoster roster, TroopRosterElement element)
		{
			try
			{
				if (element.Character == null)
				{
					PartySanitizer.RemoveNullEntries(roster);
				}
				else
				{
					CharacterObject characterObject = null;
					try
					{
						WCharacter wcharacter = new WCharacter(element.Character);
						characterObject = TroopMatcher.PickBestFromFaction(wcharacter.Culture, wcharacter, false, false, null).Base;
					}
					catch
					{
					}
					if (characterObject == null)
					{
						WCulture culture = Player.Culture;
						CharacterObject characterObject2;
						if (culture == null)
						{
							characterObject2 = null;
						}
						else
						{
							WCharacter rootBasic = culture.RootBasic;
							characterObject2 = ((rootBasic != null) ? rootBasic.Base : null);
						}
						characterObject = characterObject2;
					}
					if (characterObject == null)
					{
						MBObjectManager instance = MBObjectManager.Instance;
						characterObject = ((instance != null) ? instance.GetObject<CharacterObject>("looter") : null);
					}
					if (characterObject == null)
					{
						roster.RemoveIf((TroopRosterElement e) => e.Character == element.Character);
						string str = "Could not find fallback for invalid troop '";
						CharacterObject character = element.Character;
						Log.Warn(str + (((character != null) ? character.StringId : null) ?? "NULL") + "'; removed from roster.");
					}
					else
					{
						int num = roster.FindIndexOfTroop(element.Character);
						roster.AddToCounts(characterObject, element.Number, false, element.WoundedNumber, element.Xp, true, (num >= 0) ? num : -1);
						roster.AddToCounts(element.Character, -element.Number, false, -element.WoundedNumber, 0, true, -1);
						Log.Info(string.Concat(new string[]
						{
							"Replaced '",
							element.Character.StringId,
							"' with '",
							characterObject.StringId,
							"' ",
							string.Format("(count: {0}, wounded: {1}).", element.Number, element.WoundedNumber)
						}));
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "ReplaceInvalidTroop failed while sanitizing roster", null);
			}
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00009D70 File Offset: 0x00007F70
		private static void RemoveNullEntries(TroopRoster roster)
		{
			try
			{
				if (roster != null)
				{
					if (PartySanitizer.TroopRosterDataField == null || PartySanitizer.TroopRosterCountField == null)
					{
						Log.Warn("RemoveNullEntries: could not reflect TroopRoster.data/_count; aborting.");
					}
					else
					{
						TroopRosterElement[] array = (TroopRosterElement[])PartySanitizer.TroopRosterDataField.GetValue(roster);
						int num = (int)PartySanitizer.TroopRosterCountField.GetValue(roster);
						if (array != null && num > 0)
						{
							int num2 = 0;
							int num3 = 0;
							for (int i = 0; i < num; i++)
							{
								if (array[i].Character == null)
								{
									num3++;
								}
								else
								{
									if (num2 != i)
									{
										array[num2] = array[i];
									}
									num2++;
								}
							}
							for (int j = num2; j < num; j++)
							{
								array[j] = default(TroopRosterElement);
							}
							if (num3 > 0)
							{
								PartySanitizer.TroopRosterCountField.SetValue(roster, num2);
								roster.UpdateVersion();
								Log.Info(string.Format("Removed {0} NULL troop roster element(s) from roster.", num3));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "RemoveNullEntries failed while sanitizing roster", null);
			}
		}

		// Token: 0x04000087 RID: 135
		private static readonly FieldInfo TroopRosterDataField = typeof(TroopRoster).GetField("data", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000088 RID: 136
		private static readonly FieldInfo TroopRosterCountField = typeof(TroopRoster).GetField("_count", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
