using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Retinues.Troops;
using Retinues.Troops.Save;
using Retinues.Utils;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000052 RID: 82
	[SafeClass]
	internal static class LegacyTroopImporter
	{
		// Token: 0x06000183 RID: 387 RVA: 0x0000AE24 File Offset: 0x00009024
		public static bool IsLegacyExport(string absPath)
		{
			if (string.IsNullOrWhiteSpace(absPath) || !File.Exists(absPath))
			{
				return false;
			}
			try
			{
				using (FileStream fileStream = File.OpenRead(absPath))
				{
					using (XmlReader xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
					{
						IgnoreComments = true
					}))
					{
						while (xmlReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element)
							{
								if (xmlReader.Name == "Troops")
								{
									Log.Debug("IsLegacyExport: detected legacy export '" + absPath + "'.");
									return true;
								}
								return false;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Debug("IsLegacyExport: unable to probe '" + absPath + "': " + ex.Message);
			}
			return false;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000AF04 File Offset: 0x00009104
		public static TroopImportExport.RetinuesTroopsPackage LoadLegacyPackage(string fileNameOrPath)
		{
			TroopImportExport.RetinuesTroopsPackage result;
			try
			{
				string text = LegacyTroopImporter.ResolvePath(fileNameOrPath);
				if (text == null)
				{
					Log.Warn("LoadLegacyPackage: file not found '" + fileNameOrPath + "'.");
					result = null;
				}
				else
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(LegacyTroopsContainer));
					LegacyTroopsContainer legacyTroopsContainer;
					using (FileStream fileStream = File.OpenRead(text))
					{
						legacyTroopsContainer = (LegacyTroopsContainer)xmlSerializer.Deserialize(fileStream);
					}
					ValueTuple<FactionSaveData, FactionSaveData> valueTuple = LegacyTroopSaveConverter.ConvertLegacyFactionData(((legacyTroopsContainer != null) ? legacyTroopsContainer.Troops : null) ?? new List<LegacyTroopSaveData>());
					FactionSaveData item = valueTuple.Item1;
					FactionSaveData item2 = valueTuple.Item2;
					result = new TroopImportExport.RetinuesTroopsPackage
					{
						Factions = new TroopImportExport.FactionExportData
						{
							clanData = item,
							kingdomData = item2
						}
					};
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "LoadLegacyPackage failed", null);
				result = null;
			}
			return result;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000AFEC File Offset: 0x000091EC
		private static string ResolvePath(string fileNameOrPath)
		{
			if (File.Exists(fileNameOrPath))
			{
				return Path.GetFullPath(fileNameOrPath);
			}
			string text = Path.Combine(TroopImportExport.DefaultDir, fileNameOrPath);
			if (File.Exists(text))
			{
				return Path.GetFullPath(text);
			}
			if (!text.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
			{
				text += ".xml";
				if (File.Exists(text))
				{
					return Path.GetFullPath(text);
				}
			}
			return null;
		}

		// Token: 0x0400008F RID: 143
		private const string LegacyRoot = "Troops";
	}
}
