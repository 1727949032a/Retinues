using System;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Retinues.Game.Wrappers
{
	// Token: 0x0200008F RID: 143
	[SafeClass]
	public class WBody
	{
		// Token: 0x06000521 RID: 1313 RVA: 0x0001B371 File Offset: 0x00019571
		public WBody(WCharacter owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this._owner = owner;
			base..ctor();
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000522 RID: 1314 RVA: 0x0001B38F File Offset: 0x0001958F
		private CharacterObject Base
		{
			get
			{
				return this._owner.Base;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000523 RID: 1315 RVA: 0x0001B39C File Offset: 0x0001959C
		private Hero Hero
		{
			get
			{
				CharacterObject @base = this.Base;
				if (@base == null)
				{
					return null;
				}
				return @base.HeroObject;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000524 RID: 1316 RVA: 0x0001B3AF File Offset: 0x000195AF
		private bool IsHero
		{
			get
			{
				return this.Hero != null;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000525 RID: 1317 RVA: 0x0001B3BA File Offset: 0x000195BA
		// (set) Token: 0x06000526 RID: 1318 RVA: 0x0001B3DB File Offset: 0x000195DB
		public float Age
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.Age;
				}
				return this.Hero.Age;
			}
			set
			{
				if (this.IsHero)
				{
					this.Hero.SetBirthDay(CampaignTime.YearsFromNow(-value));
					return;
				}
				Reflector.SetPropertyValue(this.Base, "Age", value);
				this._owner.NeedsPersistence = true;
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001B41C File Offset: 0x0001961C
		// (set) Token: 0x06000528 RID: 1320 RVA: 0x0001B454 File Offset: 0x00019654
		public float AgeMin
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMin(false).Age;
				}
				return this.Hero.Age;
			}
			set
			{
				if (this.IsHero)
				{
					this.Hero.SetBirthDay(CampaignTime.YearsFromNow(-value));
					return;
				}
				this.SetDynamicEnd(true, new float?(value), null, null);
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x0001B49C File Offset: 0x0001969C
		// (set) Token: 0x0600052A RID: 1322 RVA: 0x0001B4D4 File Offset: 0x000196D4
		public float AgeMax
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMax(false).Age;
				}
				return this.Hero.Age;
			}
			set
			{
				if (this.IsHero)
				{
					this.Hero.SetBirthDay(CampaignTime.YearsFromNow(-value));
					return;
				}
				this.SetDynamicEnd(false, new float?(value), null, null);
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x0001B51C File Offset: 0x0001971C
		// (set) Token: 0x0600052C RID: 1324 RVA: 0x0001B554 File Offset: 0x00019754
		public float WeightMin
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMin(false).Weight;
				}
				return this.Hero.Weight;
			}
			set
			{
				if (this.IsHero)
				{
					this.SetHeroDynamic(null, new float?(value), null);
					return;
				}
				this.SetDynamicEnd(true, null, new float?(value), null);
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x0001B5A8 File Offset: 0x000197A8
		// (set) Token: 0x0600052E RID: 1326 RVA: 0x0001B5E0 File Offset: 0x000197E0
		public float WeightMax
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMax(false).Weight;
				}
				return this.Hero.Weight;
			}
			set
			{
				if (this.IsHero)
				{
					this.SetHeroDynamic(null, new float?(value), null);
					return;
				}
				this.SetDynamicEnd(false, null, new float?(value), null);
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x0001B634 File Offset: 0x00019834
		// (set) Token: 0x06000530 RID: 1328 RVA: 0x0001B66C File Offset: 0x0001986C
		public float BuildMin
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMin(false).Build;
				}
				return this.Hero.Build;
			}
			set
			{
				if (this.IsHero)
				{
					this.SetHeroDynamic(null, null, new float?(value));
					return;
				}
				this.SetDynamicEnd(true, null, null, new float?(value));
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000531 RID: 1329 RVA: 0x0001B6C0 File Offset: 0x000198C0
		// (set) Token: 0x06000532 RID: 1330 RVA: 0x0001B6F8 File Offset: 0x000198F8
		public float BuildMax
		{
			get
			{
				if (!this.IsHero)
				{
					return this.Base.GetBodyPropertiesMax(false).Build;
				}
				return this.Hero.Build;
			}
			set
			{
				if (this.IsHero)
				{
					this.SetHeroDynamic(null, null, new float?(value));
					return;
				}
				this.SetDynamicEnd(false, null, null, new float?(value));
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001B74C File Offset: 0x0001994C
		public void SetDynamicEnd(bool minEnd, float? age, float? weight, float? build)
		{
			try
			{
				if (this.IsHero)
				{
					this.SetHeroDynamic(age, weight, build);
				}
				else
				{
					this.EnsureOwnBodyRange();
					BodyProperties bodyPropertiesMin = this.Base.GetBodyPropertiesMin(false);
					BodyProperties bodyPropertiesMax = this.Base.GetBodyPropertiesMax(false);
					BodyProperties bodyProperties = minEnd ? bodyPropertiesMin : bodyPropertiesMax;
					BodyProperties bodyProperties2 = minEnd ? bodyPropertiesMax : bodyPropertiesMin;
					DynamicBodyProperties dynamicProperties = bodyProperties.DynamicProperties;
					DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(age ?? dynamicProperties.Age, weight ?? dynamicProperties.Weight, build ?? dynamicProperties.Build);
					BodyProperties bodyProperties3 = new BodyProperties(dynamicBodyProperties, bodyProperties.StaticProperties);
					BodyProperties bodyProperties4 = minEnd ? bodyProperties3 : bodyProperties2;
					BodyProperties bodyProperties5 = minEnd ? bodyProperties2 : bodyProperties3;
					Reflector.InvokeMethod(Reflector.GetPropertyValue<object>(this.Base, "BodyPropertyRange"), "Init", new Type[]
					{
						typeof(BodyProperties),
						typeof(BodyProperties)
					}, new object[]
					{
						bodyProperties4,
						bodyProperties5
					});
					this._owner.NeedsPersistence = true;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001B8B8 File Offset: 0x00019AB8
		private void SetHeroDynamic(float? age, float? weight, float? build)
		{
			if (!this.IsHero)
			{
				return;
			}
			try
			{
				BodyProperties bodyProperties = this.Hero.BodyProperties;
				DynamicBodyProperties dynamicProperties = bodyProperties.DynamicProperties;
				DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(age ?? dynamicProperties.Age, weight ?? dynamicProperties.Weight, build ?? dynamicProperties.Build);
				BodyProperties bodyProperties2 = new BodyProperties(dynamicBodyProperties, bodyProperties.StaticProperties);
				this.Hero.StaticBodyProperties = bodyProperties2.StaticProperties;
				this.Hero.Weight = dynamicBodyProperties.Weight;
				this.Hero.Build = dynamicBodyProperties.Build;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001B9A0 File Offset: 0x00019BA0
		public void EnsureOwnBodyRange()
		{
			if (this.IsHero)
			{
				return;
			}
			try
			{
				object propertyValue = Reflector.GetPropertyValue<object>(this.Base, "BodyPropertyRange");
				if (propertyValue == null)
				{
					BodyProperties bodyPropertiesMin = this.Base.GetBodyPropertiesMin(false);
					BodyProperties bodyPropertiesMax = this.Base.GetBodyPropertiesMax(false);
					object obj = Activator.CreateInstance(typeof(BodyProperties).Assembly.GetType("TaleWorlds.Core.MBBodyProperty"));
					if (obj != null)
					{
						Reflector.InvokeMethod(obj, "Init", new Type[]
						{
							typeof(BodyProperties),
							typeof(BodyProperties)
						}, new object[]
						{
							bodyPropertiesMin,
							bodyPropertiesMax
						});
						Reflector.SetPropertyValue(this.Base, "BodyPropertyRange", obj);
					}
				}
				else
				{
					try
					{
						object obj2 = Reflector.InvokeMethod(propertyValue, "Clone", Type.EmptyTypes, Array.Empty<object>());
						if (obj2 != null)
						{
							Reflector.SetPropertyValue(this.Base, "BodyPropertyRange", obj2);
							return;
						}
					}
					catch
					{
					}
					BodyProperties bodyPropertiesMin2 = this.Base.GetBodyPropertiesMin(false);
					BodyProperties bodyPropertiesMax2 = this.Base.GetBodyPropertiesMax(false);
					object obj3 = Activator.CreateInstance(propertyValue.GetType());
					Reflector.InvokeMethod(obj3, "Init", new Type[]
					{
						typeof(BodyProperties),
						typeof(BodyProperties)
					}, new object[]
					{
						bodyPropertiesMin2,
						bodyPropertiesMax2
					});
					Reflector.SetPropertyValue(this.Base, "BodyPropertyRange", obj3);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0001BB64 File Offset: 0x00019D64
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0001BB6D File Offset: 0x00019D6D
		public float HeightMin
		{
			get
			{
				return this.ReadHeight(true);
			}
			set
			{
				this.SetHeight(true, value);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0001BB77 File Offset: 0x00019D77
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x0001BB80 File Offset: 0x00019D80
		public float HeightMax
		{
			get
			{
				return this.ReadHeight(false);
			}
			set
			{
				this.SetHeight(false, value);
			}
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001BB8C File Offset: 0x00019D8C
		private float ReadHeight(bool minEnd)
		{
			if (this.IsHero)
			{
				StaticBodyProperties staticBodyProperties = this.Hero.StaticBodyProperties;
				int bitsValueFromKey = WBody.GetBitsValueFromKey(WBody.GetKeyPart(staticBodyProperties, 8), 19, 6);
				int num = 63;
				if (num <= 0)
				{
					return 0f;
				}
				return (float)bitsValueFromKey / (float)num;
			}
			else
			{
				StaticBodyProperties staticProperties = (minEnd ? this.Base.GetBodyPropertiesMin(false) : this.Base.GetBodyPropertiesMax(false)).StaticProperties;
				int bitsValueFromKey2 = WBody.GetBitsValueFromKey(WBody.GetKeyPart(staticProperties, 8), 19, 6);
				int num2 = 63;
				if (num2 <= 0)
				{
					return 0f;
				}
				return (float)bitsValueFromKey2 / (float)num2;
			}
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001BC20 File Offset: 0x00019E20
		private void SetHeight(bool minEnd, float value01)
		{
			try
			{
				if (this.IsHero)
				{
					int newValue = (int)Math.Round((double)(Math.Max(0f, Math.Min(1f, value01)) * 63f));
					StaticBodyProperties staticBodyProperties = this.Hero.StaticBodyProperties;
					ulong num = WBody.GetKeyPart(staticBodyProperties, 8);
					num = WBody.SetBits(num, 19, 6, newValue);
					StaticBodyProperties staticBodyProperties2 = WBody.SetKeyPart(staticBodyProperties, 8, num);
					this.Hero.StaticBodyProperties = staticBodyProperties2;
				}
				else
				{
					this.SetStaticChannelEnd(minEnd, 8, 19, 6, value01);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001BCBC File Offset: 0x00019EBC
		private void SetStaticChannelEnd(bool minEnd, int partIdx, int startBit, int numBits, float value01)
		{
			try
			{
				this.EnsureOwnBodyRange();
				int newValue = (int)Math.Round((double)(Math.Max(0f, Math.Min(1f, value01)) * (float)((1 << numBits) - 1)));
				BodyProperties bodyPropertiesMin = this.Base.GetBodyPropertiesMin(false);
				BodyProperties bodyPropertiesMax = this.Base.GetBodyPropertiesMax(false);
				BodyProperties bodyProperties = minEnd ? bodyPropertiesMin : bodyPropertiesMax;
				BodyProperties bodyProperties2 = minEnd ? bodyPropertiesMax : bodyPropertiesMin;
				StaticBodyProperties staticProperties = bodyProperties.StaticProperties;
				ulong num = WBody.GetKeyPart(staticProperties, partIdx);
				num = WBody.SetBits(num, startBit, numBits, newValue);
				StaticBodyProperties staticBodyProperties = WBody.SetKeyPart(staticProperties, partIdx, num);
				BodyProperties bodyProperties3 = new BodyProperties(bodyProperties.DynamicProperties, staticBodyProperties);
				BodyProperties bodyProperties4 = minEnd ? bodyProperties3 : bodyProperties2;
				BodyProperties bodyProperties5 = minEnd ? bodyProperties2 : bodyProperties3;
				Reflector.InvokeMethod(Reflector.GetPropertyValue<object>(this.Base, "BodyPropertyRange"), "Init", new Type[]
				{
					typeof(BodyProperties),
					typeof(BodyProperties)
				}, new object[]
				{
					bodyProperties4,
					bodyProperties5
				});
				this._owner.NeedsPersistence = true;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001BE04 File Offset: 0x0001A004
		private static int GetBitsValueFromKey(ulong part, int startBit, int numBits)
		{
			ulong num = part >> startBit;
			ulong num2 = (1UL << numBits) - 1UL;
			return (int)(num & num2);
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001BE28 File Offset: 0x0001A028
		private static ulong SetBits(ulong part, int startBit, int numBits, int newValue)
		{
			ulong num = (1UL << numBits) - 1UL << startBit;
			return (part & ~num) | (ulong)((ulong)((long)newValue) << startBit);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001BE54 File Offset: 0x0001A054
		private static ulong GetKeyPart(in StaticBodyProperties sp, int idx)
		{
			ulong result;
			switch (idx)
			{
			case 1:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart1;
				break;
			}
			case 2:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart2;
				break;
			}
			case 3:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart3;
				break;
			}
			case 4:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart4;
				break;
			}
			case 5:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart5;
				break;
			}
			case 6:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart6;
				break;
			}
			case 7:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart7;
				break;
			}
			default:
			{
				StaticBodyProperties staticBodyProperties = sp;
				result = staticBodyProperties.KeyPart8;
				break;
			}
			}
			return result;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001BF10 File Offset: 0x0001A110
		private static StaticBodyProperties SetKeyPart(in StaticBodyProperties sp, int idx, ulong val)
		{
			StaticBodyProperties result;
			switch (idx)
			{
			case 1:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart2 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart3 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart4 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart5 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				ulong keyPart6 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(val, keyPart, keyPart2, keyPart3, keyPart4, keyPart5, keyPart6, staticBodyProperties.KeyPart8);
				break;
			}
			case 2:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart7 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart8 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart9 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart10 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart11 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				ulong keyPart12 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart7, val, keyPart8, keyPart9, keyPart10, keyPart11, keyPart12, staticBodyProperties.KeyPart8);
				break;
			}
			case 3:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart13 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart14 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart15 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart16 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart17 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				ulong keyPart18 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart13, keyPart14, val, keyPart15, keyPart16, keyPart17, keyPart18, staticBodyProperties.KeyPart8);
				break;
			}
			case 4:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart19 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart20 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart21 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart22 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart23 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				ulong keyPart24 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart19, keyPart20, keyPart21, val, keyPart22, keyPart23, keyPart24, staticBodyProperties.KeyPart8);
				break;
			}
			case 5:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart25 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart26 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart27 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart28 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart29 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				ulong keyPart30 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart25, keyPart26, keyPart27, keyPart28, val, keyPart29, keyPart30, staticBodyProperties.KeyPart8);
				break;
			}
			case 6:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart31 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart32 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart33 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart34 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart35 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart36 = staticBodyProperties.KeyPart7;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart31, keyPart32, keyPart33, keyPart34, keyPart35, val, keyPart36, staticBodyProperties.KeyPart8);
				break;
			}
			case 7:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart37 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart38 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart39 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart40 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart41 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart42 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart37, keyPart38, keyPart39, keyPart40, keyPart41, keyPart42, val, staticBodyProperties.KeyPart8);
				break;
			}
			default:
			{
				StaticBodyProperties staticBodyProperties = sp;
				ulong keyPart43 = staticBodyProperties.KeyPart1;
				staticBodyProperties = sp;
				ulong keyPart44 = staticBodyProperties.KeyPart2;
				staticBodyProperties = sp;
				ulong keyPart45 = staticBodyProperties.KeyPart3;
				staticBodyProperties = sp;
				ulong keyPart46 = staticBodyProperties.KeyPart4;
				staticBodyProperties = sp;
				ulong keyPart47 = staticBodyProperties.KeyPart5;
				staticBodyProperties = sp;
				ulong keyPart48 = staticBodyProperties.KeyPart6;
				staticBodyProperties = sp;
				result = new StaticBodyProperties(keyPart43, keyPart44, keyPart45, keyPart46, keyPart47, keyPart48, staticBodyProperties.KeyPart7, val);
				break;
			}
			}
			return result;
		}

		// Token: 0x0400015A RID: 346
		private readonly WCharacter _owner;

		// Token: 0x0400015B RID: 347
		private const int HEIGHT_PART = 8;

		// Token: 0x0400015C RID: 348
		private const int HEIGHT_START = 19;

		// Token: 0x0400015D RID: 349
		private const int HEIGHT_BITS = 6;
	}
}
