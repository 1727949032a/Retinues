using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000095 RID: 149
	[SafeClass]
	public class WHero : WCharacter
	{
		// Token: 0x06000618 RID: 1560 RVA: 0x0001EE08 File Offset: 0x0001D008
		public WHero(Hero hero)
		{
			if (hero == null)
			{
				throw new ArgumentNullException("hero");
			}
			this._hero = hero;
			base..ctor((hero != null) ? hero.CharacterObject : null);
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000619 RID: 1561 RVA: 0x0001EE32 File Offset: 0x0001D032
		public Hero Hero
		{
			get
			{
				return this._hero;
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x0001EE3A File Offset: 0x0001D03A
		public override string StringId
		{
			get
			{
				return this._hero.StringId;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x0001EE47 File Offset: 0x0001D047
		// (set) Token: 0x0600061C RID: 1564 RVA: 0x0001EE7C File Offset: 0x0001D07C
		public override string Name
		{
			get
			{
				TextObject firstName = this._hero.FirstName;
				string result;
				if ((result = ((firstName != null) ? firstName.ToString() : null)) == null)
				{
					TextObject name = this._hero.Name;
					if (name == null)
					{
						return null;
					}
					result = name.ToString();
				}
				return result;
			}
			set
			{
				if (this._hero == null || string.IsNullOrWhiteSpace(value))
				{
					return;
				}
				TextObject textObject = new TextObject(value, null);
				TextObject name = this._hero.Name;
				TextObject textObject2;
				if (name != null)
				{
					textObject2 = name.CopyTextObject();
					TextObject firstName = this._hero.FirstName;
					string text = (firstName != null) ? firstName.ToString() : null;
					if ((textObject2.Value ?? string.Empty).Contains("{FIRSTNAME}"))
					{
						textObject2.SetTextVariable("FIRSTNAME", textObject);
					}
					else
					{
						string text2 = name.ToString();
						string value2;
						if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && text2.StartsWith(text))
						{
							value2 = value + text2.Substring(text.Length);
						}
						else
						{
							value2 = value;
						}
						textObject2 = new TextObject(value2, null);
					}
				}
				else
				{
					textObject2 = new TextObject(value, null);
				}
				this._hero.SetName(textObject2, textObject);
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x0001EF67 File Offset: 0x0001D167
		// (set) Token: 0x0600061E RID: 1566 RVA: 0x0001EF88 File Offset: 0x0001D188
		public override WCulture Culture
		{
			get
			{
				if (this._hero.Culture != null)
				{
					return new WCulture(this._hero.Culture);
				}
				return null;
			}
			set
			{
				CultureObject cultureObject = (value != null) ? value.Base : null;
				if (cultureObject == null || cultureObject == this._hero.Culture)
				{
					return;
				}
				this._hero.Culture = cultureObject;
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x0001EFC7 File Offset: 0x0001D1C7
		public override WFaction Clan
		{
			get
			{
				if (this._hero.Clan != null)
				{
					return new WFaction(this._hero.Clan);
				}
				return null;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001EFE8 File Offset: 0x0001D1E8
		public override WFaction Kingdom
		{
			get
			{
				Clan clan = this._hero.Clan;
				if (((clan != null) ? clan.Kingdom : null) != null)
				{
					return new WFaction(this._hero.Clan.Kingdom);
				}
				return null;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x0001F01A File Offset: 0x0001D21A
		public bool IsPartyLeader
		{
			get
			{
				return this._hero.IsPartyLeader;
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x0001F027 File Offset: 0x0001D227
		public bool IsCompanion
		{
			get
			{
				return this._hero.IsPlayerCompanion;
			}
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0001F034 File Offset: 0x0001D234
		public override int GetSkill(SkillObject skill)
		{
			if (this._hero == null || skill == null)
			{
				return 0;
			}
			return this._hero.GetSkillValue(skill);
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0001F04F File Offset: 0x0001D24F
		public override void SetSkill(SkillObject skill, int value)
		{
			if (this._hero == null || skill == null)
			{
				return;
			}
			this._hero.SetSkillValue(skill, value);
			base.NeedsPersistence = true;
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000625 RID: 1573 RVA: 0x0001F074 File Offset: 0x0001D274
		// (set) Token: 0x06000626 RID: 1574 RVA: 0x0001F10C File Offset: 0x0001D30C
		public override Dictionary<SkillObject, int> Skills
		{
			get
			{
				if (this._hero == null)
				{
					return base.AllSkills.ToDictionary((SkillObject s) => s, (SkillObject _) => 0);
				}
				return base.AllSkills.ToDictionary((SkillObject s) => s, (SkillObject s) => this._hero.GetSkillValue(s));
			}
			set
			{
				if (this._hero == null)
				{
					return;
				}
				foreach (SkillObject skillObject in base.AllSkills)
				{
					int num;
					int value2 = (value != null && value.TryGetValue(skillObject, out num)) ? num : 0;
					this._hero.SetSkillValue(skillObject, value2);
				}
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000627 RID: 1575 RVA: 0x0001F188 File Offset: 0x0001D388
		public static TraitObject[] PersonalityTraits
		{
			get
			{
				return new TraitObject[]
				{
					DefaultTraits.Mercy,
					DefaultTraits.Valor,
					DefaultTraits.Honor,
					DefaultTraits.Generosity,
					DefaultTraits.Calculating
				};
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x0001F1B8 File Offset: 0x0001D3B8
		public int GetTrait(TraitObject trait)
		{
			if (this._hero == null || trait == null)
			{
				return 0;
			}
			return this._hero.GetTraitLevel(trait);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0001F1D3 File Offset: 0x0001D3D3
		public void SetTrait(TraitObject trait, int value)
		{
			if (this._hero == null || trait == null)
			{
				return;
			}
			this._hero.SetTraitLevel(trait, value);
			base.NeedsPersistence = true;
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0001F1F8 File Offset: 0x0001D3F8
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x0001F248 File Offset: 0x0001D448
		public Dictionary<TraitObject, int> Traits
		{
			get
			{
				if (this._hero == null)
				{
					return new Dictionary<TraitObject, int>();
				}
				return WHero.PersonalityTraits.ToDictionary((TraitObject tr) => tr, (TraitObject tr) => this._hero.GetTraitLevel(tr));
			}
			set
			{
				if (this._hero == null || value == null)
				{
					return;
				}
				foreach (TraitObject traitObject in WHero.PersonalityTraits)
				{
					int value2;
					value.TryGetValue(traitObject, out value2);
					this._hero.SetTraitLevel(traitObject, value2);
				}
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x0001F297 File Offset: 0x0001D497
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x0001F2AF File Offset: 0x0001D4AF
		public override bool IsFemale
		{
			get
			{
				Hero hero = this._hero;
				if (hero == null)
				{
					return base.IsFemale;
				}
				return hero.IsFemale;
			}
			set
			{
				if (this._hero != null)
				{
					this._hero.IsFemale = value;
				}
				else
				{
					base.IsFemale = value;
				}
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001F2D5 File Offset: 0x0001D4D5
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x0001F2E2 File Offset: 0x0001D4E2
		public override int Level
		{
			get
			{
				return this._hero.Level;
			}
			set
			{
				this._hero.Level = value;
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x0001F2F8 File Offset: 0x0001D4F8
		// (set) Token: 0x06000631 RID: 1585 RVA: 0x0001F370 File Offset: 0x0001D570
		public string Surname
		{
			get
			{
				TextObject firstName = this._hero.FirstName;
				string text = (firstName != null) ? firstName.ToString() : null;
				TextObject name = this._hero.Name;
				string text2 = (name != null) ? name.ToString() : null;
				if (string.IsNullOrEmpty(text2))
				{
					return string.Empty;
				}
				if (!string.IsNullOrEmpty(text) && text2.StartsWith(text))
				{
					return text2.Substring(text.Length).TrimStart(Array.Empty<char>());
				}
				return string.Empty;
			}
			set
			{
				TextObject firstName = this._hero.FirstName;
				string text = ((firstName != null) ? firstName.ToString() : null) ?? string.Empty;
				TextObject firstName2 = new TextObject(text, null);
				TextObject fullName;
				if (string.IsNullOrWhiteSpace(value))
				{
					fullName = new TextObject(text, null);
				}
				else
				{
					fullName = new TextObject(text + " " + value.Trim(), null);
				}
				this._hero.SetName(fullName, firstName2);
				base.NeedsPersistence = true;
			}
		}

		// Token: 0x0400018B RID: 395
		private readonly Hero _hero;
	}
}
