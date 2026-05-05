using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Retinues.Game.Helpers
{
	// Token: 0x020000A1 RID: 161
	[SafeClass]
	public static class BodyHelper
	{
		// Token: 0x060006FD RID: 1789 RVA: 0x00022CC8 File Offset: 0x00020EC8
		public static void ApplyNextAgePreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.AgePresets, 1, () => t.Body.AgeMin, delegate(float v)
			{
				t.Body.AgeMin = v;
			}, () => t.Body.AgeMax, delegate(float v)
			{
				t.Body.AgeMax = v;
			});
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00022D24 File Offset: 0x00020F24
		public static void ApplyPrevAgePreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.AgePresets, -1, () => t.Body.AgeMin, delegate(float v)
			{
				t.Body.AgeMin = v;
			}, () => t.Body.AgeMax, delegate(float v)
			{
				t.Body.AgeMax = v;
			});
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00022D80 File Offset: 0x00020F80
		public static void ApplyNextWeightPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.WeightPresets, 1, () => t.Body.WeightMin, delegate(float v)
			{
				t.Body.WeightMin = v;
			}, () => t.Body.WeightMax, delegate(float v)
			{
				t.Body.WeightMax = v;
			});
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00022DDC File Offset: 0x00020FDC
		public static void ApplyPrevWeightPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.WeightPresets, -1, () => t.Body.WeightMin, delegate(float v)
			{
				t.Body.WeightMin = v;
			}, () => t.Body.WeightMax, delegate(float v)
			{
				t.Body.WeightMax = v;
			});
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00022E38 File Offset: 0x00021038
		public static void ApplyNextBuildPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.BuildPresets, 1, () => t.Body.BuildMin, delegate(float v)
			{
				t.Body.BuildMin = v;
			}, () => t.Body.BuildMax, delegate(float v)
			{
				t.Body.BuildMax = v;
			});
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00022E94 File Offset: 0x00021094
		public static void ApplyPrevBuildPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.BuildPresets, -1, () => t.Body.BuildMin, delegate(float v)
			{
				t.Body.BuildMin = v;
			}, () => t.Body.BuildMax, delegate(float v)
			{
				t.Body.BuildMax = v;
			});
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x00022EF0 File Offset: 0x000210F0
		public static void ApplyNextHeightPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.HeightPresets, 1, () => t.Body.HeightMin, delegate(float v)
			{
				t.Body.HeightMin = v;
			}, () => t.Body.HeightMax, delegate(float v)
			{
				t.Body.HeightMax = v;
			});
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00022F4C File Offset: 0x0002114C
		public static void ApplyPrevHeightPreset(WCharacter t)
		{
			BodyHelper.ApplyPresetStep(t, BodyHelper.HeightPresets, -1, () => t.Body.HeightMin, delegate(float v)
			{
				t.Body.HeightMin = v;
			}, () => t.Body.HeightMax, delegate(float v)
			{
				t.Body.HeightMax = v;
			});
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00022FA8 File Offset: 0x000211A8
		private static void ApplyPresetStep(WCharacter t, [TupleElementNames(new string[]
		{
			"min",
			"max"
		})] List<ValueTuple<float, float>> presets, int step, Func<float> getMin, Action<float> setMin, Func<float> getMax, Action<float> setMax)
		{
			if (t == null || presets == null || presets.Count == 0)
			{
				return;
			}
			float curMin = getMin();
			float curMax = getMax();
			int num = (BodyHelper.NearestPresetIndex(presets, curMin, curMax) + step) % presets.Count;
			if (num < 0)
			{
				num += presets.Count;
			}
			ValueTuple<float, float> valueTuple = presets[num];
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			setMin(item);
			setMax(item2);
			BodyHelper.ApplyTagsFromCulture(t);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00023024 File Offset: 0x00021224
		private static int NearestPresetIndex([TupleElementNames(new string[]
		{
			"min",
			"max"
		})] List<ValueTuple<float, float>> presets, float curMin, float curMax)
		{
			int result = 0;
			float num = float.MaxValue;
			for (int i = 0; i < presets.Count; i++)
			{
				ValueTuple<float, float> valueTuple = presets[i];
				float item = valueTuple.Item1;
				float item2 = valueTuple.Item2;
				float num2 = Math.Abs(item - curMin) + Math.Abs(item2 - curMax);
				if (num2 < num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00023080 File Offset: 0x00021280
		public static void ApplyPropertiesFromCulture(WCharacter troop, CultureObject culture)
		{
			if (troop == null || culture == null)
			{
				return;
			}
			CharacterObject characterObject = culture.BasicTroop ?? culture.EliteBasicTroop;
			if (characterObject == null)
			{
				return;
			}
			CharacterObject @base = troop.Base;
			Hero hero = (@base != null) ? @base.HeroObject : null;
			if (hero != null)
			{
				try
				{
					BodyProperties bodyPropertiesMin = characterObject.GetBodyPropertiesMin(false);
					BodyProperties bodyPropertiesMax = characterObject.GetBodyPropertiesMax(false);
					float num = (bodyPropertiesMin.Age + bodyPropertiesMax.Age) * 0.5f;
					float weight = (bodyPropertiesMin.Weight + bodyPropertiesMax.Weight) * 0.5f;
					float build = (bodyPropertiesMin.Build + bodyPropertiesMax.Build) * 0.5f;
					DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(num, weight, build);
					StaticBodyProperties staticProperties = bodyPropertiesMin.StaticProperties;
					BodyProperties bodyProperties = new BodyProperties(dynamicBodyProperties, staticProperties);
					hero.StaticBodyProperties = bodyProperties.StaticProperties;
					hero.SetBirthDay(CampaignTime.YearsFromNow(-num));
					troop.NeedsPersistence = true;
				}
				catch (Exception ex)
				{
					Log.Exception(ex, "", null);
				}
				return;
			}
			troop.Body.EnsureOwnBodyRange();
			object propertyValue = Reflector.GetPropertyValue<object>(troop.Base, "BodyPropertyRange");
			troop.Race = characterObject.Race;
			BodyProperties bodyPropertiesMin2 = characterObject.GetBodyPropertiesMin(false);
			BodyProperties bodyPropertiesMax2 = characterObject.GetBodyPropertiesMax(false);
			Reflector.InvokeMethod(propertyValue, "Init", new Type[]
			{
				typeof(BodyProperties),
				typeof(BodyProperties)
			}, new object[]
			{
				bodyPropertiesMin2,
				bodyPropertiesMax2
			});
			troop.Body.Age = (bodyPropertiesMin2.Age + bodyPropertiesMax2.Age) * 0.5f;
			BodyHelper.ApplyTagsFromCulture(troop);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00023224 File Offset: 0x00021424
		public static void ApplyPropertiesFromCulture(WCharacter troop, string cultureId)
		{
			CultureObject @object = MBObjectManager.Instance.GetObject<CultureObject>(cultureId);
			BodyHelper.ApplyPropertiesFromCulture(troop, @object);
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00023244 File Offset: 0x00021444
		public static void ApplyTagsFromCulture(WCharacter troop)
		{
			try
			{
				CharacterObject characterObject = (troop != null) ? troop.Base : null;
				CultureObject cultureObject = (troop != null) ? troop.Culture.Base : null;
				if (characterObject != null && cultureObject != null)
				{
					BasicCharacterObject basicCharacterObject = characterObject;
					if (basicCharacterObject != null)
					{
						if (!basicCharacterObject.IsHero)
						{
							CharacterObject characterObject2 = null;
							if (troop.IsFemale)
							{
								characterObject2 = cultureObject.VillageWoman;
							}
							if (characterObject2 == null)
							{
								characterObject2 = (cultureObject.BasicTroop ?? cultureObject.EliteBasicTroop);
							}
							if (characterObject2 == null)
							{
								Log.Warn("[BodyHelper] No BasicTroop/EliteBasicTroop for culture '" + cultureObject.StringId + "', aborting.");
							}
							else
							{
								MBBodyProperty bodyPropertyRange = characterObject2.BodyPropertyRange;
								MBBodyProperty bodyPropertyRange2 = basicCharacterObject.BodyPropertyRange;
								if (bodyPropertyRange == null || bodyPropertyRange2 == null)
								{
									Log.Warn("[BodyHelper] Missing BodyPropertyRange on template or target, aborting.");
								}
								else
								{
									string text = bodyPropertyRange.HairTags ?? string.Empty;
									string text2 = bodyPropertyRange.BeardTags ?? string.Empty;
									string text3 = bodyPropertyRange.TattooTags ?? string.Empty;
									bool flag = !string.IsNullOrEmpty(text);
									bool flag2 = !string.IsNullOrEmpty(text2);
									bool flag3 = !string.IsNullOrEmpty(text3);
									if (!flag && !flag2 && !flag3)
									{
										Log.Info("[BodyHelper] Template has no tags, nothing to apply.");
									}
									else if ((flag && !string.Equals(bodyPropertyRange2.HairTags, text, StringComparison.Ordinal)) || (flag2 && !string.Equals(bodyPropertyRange2.BeardTags, text2, StringComparison.Ordinal)) || (flag3 && !string.Equals(bodyPropertyRange2.TattooTags, text3, StringComparison.Ordinal)))
									{
										MBBodyProperty mbbodyProperty = MBBodyProperty.CreateFrom(bodyPropertyRange2);
										if (flag)
										{
											mbbodyProperty.HairTags = text;
										}
										if (flag2)
										{
											mbbodyProperty.BeardTags = text2;
										}
										if (flag3)
										{
											mbbodyProperty.TattooTags = text3;
										}
										Reflector.SetPropertyValue(basicCharacterObject, "BodyPropertyRange", mbbodyProperty);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x040001AB RID: 427
		[TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public static readonly List<ValueTuple<float, float>> AgePresets = new List<ValueTuple<float, float>>(4)
		{
			new ValueTuple<float, float>(22f, 29f),
			new ValueTuple<float, float>(30f, 49f),
			new ValueTuple<float, float>(50f, 69f),
			new ValueTuple<float, float>(70f, 99f)
		};

		// Token: 0x040001AC RID: 428
		[TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public static readonly List<ValueTuple<float, float>> WeightPresets = new List<ValueTuple<float, float>>(6)
		{
			new ValueTuple<float, float>(0.01f, 0.15f),
			new ValueTuple<float, float>(0.16f, 0.3f),
			new ValueTuple<float, float>(0.31f, 0.55f),
			new ValueTuple<float, float>(0.56f, 0.7f),
			new ValueTuple<float, float>(0.71f, 0.85f),
			new ValueTuple<float, float>(0.86f, 0.99f)
		};

		// Token: 0x040001AD RID: 429
		[TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public static readonly List<ValueTuple<float, float>> BuildPresets = new List<ValueTuple<float, float>>(6)
		{
			new ValueTuple<float, float>(0.01f, 0.15f),
			new ValueTuple<float, float>(0.16f, 0.3f),
			new ValueTuple<float, float>(0.31f, 0.55f),
			new ValueTuple<float, float>(0.56f, 0.7f),
			new ValueTuple<float, float>(0.71f, 0.85f),
			new ValueTuple<float, float>(0.86f, 0.99f)
		};

		// Token: 0x040001AE RID: 430
		[TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public static readonly List<ValueTuple<float, float>> HeightPresets = new List<ValueTuple<float, float>>(6)
		{
			new ValueTuple<float, float>(0.01f, 0.15f),
			new ValueTuple<float, float>(0.16f, 0.3f),
			new ValueTuple<float, float>(0.31f, 0.55f),
			new ValueTuple<float, float>(0.56f, 0.7f),
			new ValueTuple<float, float>(0.71f, 0.85f),
			new ValueTuple<float, float>(0.86f, 0.99f)
		};
	}
}
