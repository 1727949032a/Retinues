using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Retinues.Troops.Save
{
	// Token: 0x0200003E RID: 62
	public class TroopSkillData
	{
		// Token: 0x06000140 RID: 320 RVA: 0x00009158 File Offset: 0x00007358
		public TroopSkillData(Dictionary<SkillObject, int> skills)
		{
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00009195 File Offset: 0x00007395
		public TroopSkillData() : this(new Dictionary<SkillObject, int>())
		{
		}

		// Token: 0x06000142 RID: 322 RVA: 0x000091A4 File Offset: 0x000073A4
		public Dictionary<SkillObject, int> Deserialize()
		{
			Dictionary<SkillObject, int> dictionary = new Dictionary<SkillObject, int>();
			if (string.IsNullOrWhiteSpace(this.Code))
			{
				return dictionary;
			}
			foreach (KeyValuePair<string, int> keyValuePair in (from part in this.Code.Split(new char[]
			{
				';'
			})
			select part.Split(new char[]
			{
				':'
			}) into parts
			where parts.Length == 2
			select parts).ToDictionary((string[] parts) => parts[0], (string[] parts) => int.Parse(parts[1])))
			{
				SkillObject @object = MBObjectManager.Instance.GetObject<SkillObject>(keyValuePair.Key);
				if (@object != null)
				{
					dictionary[@object] = keyValuePair.Value;
				}
			}
			return dictionary;
		}

		// Token: 0x04000083 RID: 131
		[SaveableField(1)]
		public string Code = string.Join(";", from kv in skills
		select string.Format("{0}:{1}", kv.Key.StringId, kv.Value));
	}
}
