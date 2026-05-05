using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Retinues.Safety.Legacy
{
	// Token: 0x02000051 RID: 81
	[XmlRoot("Troops")]
	public class LegacyTroopsContainer
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000ADFF File Offset: 0x00008FFF
		// (set) Token: 0x06000181 RID: 385 RVA: 0x0000AE07 File Offset: 0x00009007
		[XmlElement("TroopSaveData")]
		public List<LegacyTroopSaveData> Troops { get; set; } = new List<LegacyTroopSaveData>();
	}
}
