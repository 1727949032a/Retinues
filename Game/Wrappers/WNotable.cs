using System;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Game.Wrappers
{
	// Token: 0x02000099 RID: 153
	[SafeClass]
	public class WNotable : WHero
	{
		// Token: 0x06000694 RID: 1684 RVA: 0x00020F1A File Offset: 0x0001F11A
		public WNotable(Hero notable, WSettlement settlement) : base(notable)
		{
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x00020F2A File Offset: 0x0001F12A
		public WSettlement Settlement
		{
			get
			{
				return this._settlement;
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00020F32 File Offset: 0x0001F132
		public void SwapVolunteers(WFaction faction)
		{
			this.SwapVolunteers(faction, null, 0f);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00020F44 File Offset: 0x0001F144
		public void SwapVolunteers(WFaction primaryFaction, WFaction secondaryFaction, float secondaryProportion)
		{
			if (base.Base == null || primaryFaction == null)
			{
				return;
			}
			CharacterObject[] volunteerTypes = base.Hero.VolunteerTypes;
			if (volunteerTypes == null || volunteerTypes.Length == 0)
			{
				return;
			}
			bool flag = secondaryFaction != null && secondaryProportion > 0f && secondaryProportion <= 1f;
			int num = 0;
			for (int i = 0; i < volunteerTypes.Length; i++)
			{
				CharacterObject characterObject = volunteerTypes[i];
				if (characterObject != null)
				{
					WCharacter wcharacter = new WCharacter(characterObject);
					if (!wcharacter.IsValid)
					{
						volunteerTypes[i] = null;
					}
					else if (!(wcharacter.Faction == primaryFaction) && (!flag || !(wcharacter.Faction == secondaryFaction)))
					{
						WFaction wfaction = primaryFaction;
						if (flag && WNotable.rng.NextDouble() < (double)secondaryProportion)
						{
							wfaction = secondaryFaction;
						}
						if (!(wfaction == null))
						{
							WCharacter wcharacter2 = TroopMatcher.PickBestFromFaction(wfaction, wcharacter, true, false, null);
							if (wcharacter2 != null)
							{
								volunteerTypes[i] = wcharacter2.Base;
								num++;
							}
						}
					}
				}
			}
			if (num > 0)
			{
				string text = ((primaryFaction != null) ? primaryFaction.StringId : null) ?? "null";
				string text2 = flag ? ((secondaryFaction != null) ? secondaryFaction.StringId : null) : null;
				string text3 = (text2 != null) ? string.Format(" + mix {0} (p={1:0.##})", text2, secondaryProportion) : "";
				Log.Debug(string.Format("{0} ({1}): swapped {2} volunteers to {3}{4}.", new object[]
				{
					this.Name,
					this.Settlement.Name,
					num,
					text,
					text3
				}));
			}
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x000210CC File Offset: 0x0001F2CC
		public void SwapVolunteer(WCharacter oldTroop, WCharacter newTroop)
		{
			if (base.Base == null || oldTroop == null || newTroop == null)
			{
				return;
			}
			CharacterObject[] volunteerTypes = base.Hero.VolunteerTypes;
			if (volunteerTypes == null || volunteerTypes.Length == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < volunteerTypes.Length; i++)
			{
				CharacterObject characterObject = volunteerTypes[i];
				if (characterObject != null && new WCharacter(characterObject) == oldTroop)
				{
					volunteerTypes[i] = newTroop.Base;
					num++;
					break;
				}
			}
			if (num > 0)
			{
				Log.Debug(string.Format("{0} ({1}): swapped {2} {3} ({4}) to {5} ({6}).", new object[]
				{
					this.Name,
					this.Settlement.Name,
					num,
					oldTroop.Name,
					oldTroop,
					newTroop.Name,
					newTroop
				}));
			}
		}

		// Token: 0x04000194 RID: 404
		private static readonly Random rng = new Random();

		// Token: 0x04000195 RID: 405
		private readonly WSettlement _settlement = settlement;
	}
}
