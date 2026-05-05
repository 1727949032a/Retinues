using System;

namespace Retinues.GUI.Editor
{
	// Token: 0x02000075 RID: 117
	public readonly struct EquipChangeDelta
	{
		// Token: 0x06000255 RID: 597 RVA: 0x0000F9E0 File Offset: 0x0000DBE0
		public EquipChangeDelta(string oldEquippedId, string newEquippedId, string oldStagedId, string newStagedId)
		{
			this.OldEquippedId = oldEquippedId;
			this.NewEquippedId = newEquippedId;
			this.OldStagedId = oldStagedId;
			this.NewStagedId = newStagedId;
		}

		// Token: 0x040000CA RID: 202
		public readonly string OldEquippedId;

		// Token: 0x040000CB RID: 203
		public readonly string NewEquippedId;

		// Token: 0x040000CC RID: 204
		public readonly string OldStagedId;

		// Token: 0x040000CD RID: 205
		public readonly string NewStagedId;
	}
}
