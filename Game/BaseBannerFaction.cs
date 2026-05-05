using System;
using System.Runtime.CompilerServices;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace Retinues.Game
{
	// Token: 0x0200008A RID: 138
	[SafeClass]
	public abstract class BaseBannerFaction : BaseFaction
	{
		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060004CB RID: 1227
		public abstract Banner BaseBanner { get; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001A53C File Offset: 0x0001873C
		public virtual Banner Banner
		{
			get
			{
				Banner banner = this.BaseBanner;
				if (BaseBannerFaction.IsEmptyBanner(banner))
				{
					banner = this.CreateFallbackBanner(base.Culture.Base);
				}
				return banner;
			}
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0001A56C File Offset: 0x0001876C
		public BannerImageIdentifierVM GetBannerImage(float scale = 1f)
		{
			Banner scaledBanner = this.GetScaledBanner(scale);
			if (scaledBanner == null)
			{
				return null;
			}
			return new BannerImageIdentifierVM(scaledBanner, true);
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001A58D File Offset: 0x0001878D
		public Banner GetScaledBanner(float scale)
		{
			if (scale == 1f)
			{
				return this.Banner;
			}
			if (this.Banner == null)
			{
				return null;
			}
			return BaseBannerFaction.ScaleBannerIcon(this.Banner, scale);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001A5B4 File Offset: 0x000187B4
		protected static bool IsEmptyBanner(Banner banner)
		{
			if (banner == null)
			{
				return true;
			}
			try
			{
				MBReadOnlyList<BannerData> bannerDataList = banner.BannerDataList;
				if (bannerDataList == null || bannerDataList.Count == 0)
				{
					return true;
				}
				if (banner.GetPrimaryColor() == 4294967295U)
				{
					return true;
				}
			}
			catch
			{
				return true;
			}
			return false;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001A604 File Offset: 0x00018804
		protected static Banner ScaleBannerIcon(Banner src, float scale)
		{
			if (src == null || scale == 1f)
			{
				return src;
			}
			Banner banner = new Banner(src);
			MBReadOnlyList<BannerData> bannerDataList = banner.BannerDataList;
			for (int i = 1; i < bannerDataList.Count; i++)
			{
				bannerDataList[i].Size *= scale;
			}
			return banner;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001A658 File Offset: 0x00018858
		protected Banner CreateFallbackBanner(BasicCultureObject basicCulture)
		{
			if (basicCulture == null)
			{
				return Banner.CreateRandomClanBanner(-1);
			}
			Banner banner = Banner.CreateRandomClanBanner(-1);
			try
			{
				WCharacter wcharacter = this.RootBasic ?? this.RootElite;
				if (wcharacter != null)
				{
					foreach (WCharacter wcharacter2 in wcharacter.Tree)
					{
						Banner banner2;
						if (wcharacter2 == null)
						{
							banner2 = null;
						}
						else
						{
							WCulture culture = wcharacter2.Culture;
							if (culture == null)
							{
								banner2 = null;
							}
							else
							{
								CultureObject @base = culture.Base;
								banner2 = ((@base != null) ? @base.Banner : null);
							}
						}
						Banner banner3 = banner2;
						if (!BaseBannerFaction.IsEmptyBanner(banner3))
						{
							banner = new Banner(banner3);
							break;
						}
					}
				}
				if (!BaseBannerFaction.IsEmptyBanner(banner))
				{
					try
					{
						banner = new Banner(banner.Serialize());
						ValueTuple<uint, uint> safeCultureColors = BaseBannerFaction.GetSafeCultureColors(basicCulture, banner);
						uint item = safeCultureColors.Item1;
						uint item2 = safeCultureColors.Item2;
						if (BannerManager.GetColorId(item) >= 0 && BannerManager.GetColorId(item2) >= 0)
						{
							banner.ChangeBackgroundColor(item, item2);
							banner.ChangeIconColors(item2);
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
			return banner;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001A77C File Offset: 0x0001897C
		[return: TupleElementNames(new string[]
		{
			"primary",
			"secondary"
		})]
		private static ValueTuple<uint, uint> GetSafeCultureColors(BasicCultureObject culture, Banner fallbackBanner)
		{
			uint num = culture.Color2;
			uint num2 = culture.Color;
			if (num == 4294967295U || BannerManager.GetColorId(num) < 0)
			{
				num = ((fallbackBanner != null) ? fallbackBanner.GetFirstIconColor() : ((fallbackBanner != null) ? fallbackBanner.GetPrimaryColor() : uint.MaxValue));
			}
			if (num2 == 4294967295U || BannerManager.GetColorId(num2) < 0)
			{
				num2 = ((fallbackBanner != null) ? fallbackBanner.GetPrimaryColor() : num);
			}
			if (num == 4294967295U || BannerManager.GetColorId(num) < 0)
			{
				num = ((fallbackBanner != null) ? fallbackBanner.GetPrimaryColor() : 0U);
			}
			if (num2 == 4294967295U || BannerManager.GetColorId(num2) < 0)
			{
				num2 = ((fallbackBanner != null) ? fallbackBanner.GetFirstIconColor() : num);
			}
			return new ValueTuple<uint, uint>(num, num2);
		}
	}
}
