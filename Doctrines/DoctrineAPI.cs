using System;
using System.Collections.Generic;
using System.Linq;
using Retinues.Doctrines.Model;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;

namespace Retinues.Doctrines
{
	// Token: 0x020000D2 RID: 210
	[SafeClass]
	public static class DoctrineAPI
	{
		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x0002A7A3 File Offset: 0x000289A3
		private static DoctrineServiceBehavior Svc
		{
			get
			{
				Campaign campaign = Campaign.Current;
				if (campaign == null)
				{
					return null;
				}
				return campaign.GetCampaignBehavior<DoctrineServiceBehavior>();
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0002A7B5 File Offset: 0x000289B5
		private static bool EnsureSvc(out DoctrineServiceBehavior svc)
		{
			svc = DoctrineAPI.Svc;
			return svc != null;
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0002A7C4 File Offset: 0x000289C4
		public static IReadOnlyList<DoctrineDefinition> AllDoctrines()
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior))
			{
				return Array.Empty<DoctrineDefinition>();
			}
			return doctrineServiceBehavior.AllDoctrines().ToList<DoctrineDefinition>();
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0002A7EF File Offset: 0x000289EF
		public static DoctrineDefinition GetDoctrine<TDoctrine>() where TDoctrine : Doctrine
		{
			return DoctrineAPI.GetDoctrine(typeof(TDoctrine));
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0002A800 File Offset: 0x00028A00
		public static DoctrineDefinition GetDoctrine(Type doctrineType)
		{
			if (doctrineType == null)
			{
				return null;
			}
			return DoctrineAPI.GetDoctrine(doctrineType.FullName);
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0002A818 File Offset: 0x00028A18
		public static DoctrineDefinition GetDoctrine(string doctrineKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(doctrineKey))
			{
				return null;
			}
			return doctrineServiceBehavior.GetDoctrine(doctrineKey);
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0002A83F File Offset: 0x00028A3F
		public static DoctrineStatus GetDoctrineStatus<TDoctrine>() where TDoctrine : Doctrine
		{
			return DoctrineAPI.GetDoctrineStatus(typeof(TDoctrine));
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0002A850 File Offset: 0x00028A50
		public static DoctrineStatus GetDoctrineStatus(Type doctrineType)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || doctrineType == null)
			{
				return DoctrineStatus.Locked;
			}
			return doctrineServiceBehavior.GetDoctrineStatus(doctrineType.FullName);
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0002A880 File Offset: 0x00028A80
		public static DoctrineStatus GetDoctrineStatus(string doctrineKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(doctrineKey))
			{
				return DoctrineStatus.Locked;
			}
			return doctrineServiceBehavior.GetDoctrineStatus(doctrineKey);
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0002A8A7 File Offset: 0x00028AA7
		public static bool IsDoctrineUnlocked<TDoctrine>() where TDoctrine : Doctrine
		{
			return DoctrineAPI.IsDoctrineUnlocked(typeof(TDoctrine));
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0002A8B8 File Offset: 0x00028AB8
		public static bool IsDoctrineUnlocked(Type doctrineType)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			return DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) && !(doctrineType == null) && doctrineServiceBehavior.IsDoctrineUnlocked(doctrineType.FullName);
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0002A8E8 File Offset: 0x00028AE8
		public static bool IsDoctrineUnlocked(string doctrineKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			return DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) && !string.IsNullOrEmpty(doctrineKey) && doctrineServiceBehavior.IsDoctrineUnlocked(doctrineKey);
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0002A90F File Offset: 0x00028B0F
		public static bool TryAcquireDoctrine<TDoctrine>(out string reason) where TDoctrine : Doctrine
		{
			return DoctrineAPI.TryAcquireDoctrine(typeof(TDoctrine), out reason);
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0002A924 File Offset: 0x00028B24
		public static bool TryAcquireDoctrine(Type doctrineType, out string reason)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || doctrineType == null)
			{
				reason = L.S("doctrine_service_unavailable", "Doctrine service is unavailable.");
				return false;
			}
			return doctrineServiceBehavior.TryAcquireDoctrine(doctrineType.FullName, out reason);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0002A964 File Offset: 0x00028B64
		public static bool TryAcquireDoctrine(string doctrineKey, out string reason)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(doctrineKey))
			{
				reason = L.S("doctrine_service_unavailable", "Doctrine service is unavailable.");
				return false;
			}
			return doctrineServiceBehavior.TryAcquireDoctrine(doctrineKey, out reason);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x0002A99D File Offset: 0x00028B9D
		public static int GetFeatProgress<TFeat>() where TFeat : Feat
		{
			return DoctrineAPI.GetFeatProgress(typeof(TFeat));
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0002A9B0 File Offset: 0x00028BB0
		public static int GetFeatProgress(Type featType)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || featType == null)
			{
				return 0;
			}
			return doctrineServiceBehavior.GetFeatProgress(featType.FullName);
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0002A9DD File Offset: 0x00028BDD
		public static int GetFeatTarget<TFeat>() where TFeat : Feat
		{
			return DoctrineAPI.GetFeatTarget(typeof(TFeat));
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0002A9F0 File Offset: 0x00028BF0
		public static int GetFeatTarget(Type featType)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || featType == null)
			{
				return 0;
			}
			return doctrineServiceBehavior.GetFeatTarget(featType.FullName);
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0002AA1D File Offset: 0x00028C1D
		public static bool IsFeatComplete<TFeat>() where TFeat : Feat
		{
			return DoctrineAPI.IsFeatComplete(typeof(TFeat));
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x0002AA30 File Offset: 0x00028C30
		public static bool IsFeatComplete(Type featType)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			return DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) && !(featType == null) && doctrineServiceBehavior.IsFeatComplete(featType.FullName);
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x0002AA5D File Offset: 0x00028C5D
		public static void SetFeatProgress<TFeat>(int amount) where TFeat : Feat
		{
			DoctrineAPI.SetFeatProgress(typeof(TFeat), amount);
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0002AA70 File Offset: 0x00028C70
		public static void SetFeatProgress(Type featType, int amount)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || featType == null)
			{
				return;
			}
			doctrineServiceBehavior.SetFeatProgress(featType.FullName, amount);
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x0002AA9D File Offset: 0x00028C9D
		public static int AdvanceFeat<TFeat>(int amount = 1) where TFeat : Feat
		{
			return DoctrineAPI.AdvanceFeat(typeof(TFeat), amount);
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x0002AAB0 File Offset: 0x00028CB0
		public static int AdvanceFeat(Type featType, int amount = 1)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || featType == null)
			{
				return 0;
			}
			return doctrineServiceBehavior.AdvanceFeat(featType.FullName, amount);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0002AAE0 File Offset: 0x00028CE0
		public static int GetFeatProgress(string featKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(featKey))
			{
				return 0;
			}
			return doctrineServiceBehavior.GetFeatProgress(featKey);
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0002AB08 File Offset: 0x00028D08
		public static int GetFeatTarget(string featKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(featKey))
			{
				return 0;
			}
			return doctrineServiceBehavior.GetFeatTarget(featKey);
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0002AB30 File Offset: 0x00028D30
		public static bool IsFeatComplete(string featKey)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			return DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) && !string.IsNullOrEmpty(featKey) && doctrineServiceBehavior.IsFeatComplete(featKey);
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0002AB58 File Offset: 0x00028D58
		public static void SetFeatProgress(string featKey, int amount)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(featKey))
			{
				return;
			}
			doctrineServiceBehavior.SetFeatProgress(featKey, amount);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0002AB80 File Offset: 0x00028D80
		public static int AdvanceFeat(string featKey, int amount = 1)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || string.IsNullOrEmpty(featKey))
			{
				return 0;
			}
			return doctrineServiceBehavior.AdvanceFeat(featKey, amount);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0002ABA8 File Offset: 0x00028DA8
		public static void AddDoctrineUnlockedListener(Action<string> listener)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || listener == null)
			{
				return;
			}
			doctrineServiceBehavior.DoctrineUnlocked += listener;
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x0002ABCC File Offset: 0x00028DCC
		public static void RemoveDoctrineUnlockedListener(Action<string> listener)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || listener == null)
			{
				return;
			}
			doctrineServiceBehavior.DoctrineUnlocked -= listener;
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x0002ABF0 File Offset: 0x00028DF0
		public static void AddCatalogBuiltListener(Action listener)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || listener == null)
			{
				return;
			}
			doctrineServiceBehavior.CatalogBuilt += listener;
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0002AC14 File Offset: 0x00028E14
		public static void AddFeatCompletedListener(Action<string> listener)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || listener == null)
			{
				return;
			}
			doctrineServiceBehavior.FeatCompleted += listener;
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x0002AC38 File Offset: 0x00028E38
		public static void RemoveFeatCompletedListener(Action<string> listener)
		{
			DoctrineServiceBehavior doctrineServiceBehavior;
			if (!DoctrineAPI.EnsureSvc(out doctrineServiceBehavior) || listener == null)
			{
				return;
			}
			doctrineServiceBehavior.FeatCompleted -= listener;
		}
	}
}
