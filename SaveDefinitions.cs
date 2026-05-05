using System;
using System.Collections.Generic;
using Retinues.Features.Agents;
using Retinues.Features.Staging;
using Retinues.Features.Statistics;
using Retinues.Safety.Legacy;
using Retinues.Troops.Save;
using TaleWorlds.SaveSystem;

namespace Retinues
{
	// Token: 0x0200001C RID: 28
	public sealed class SaveDefinitions : SaveableTypeDefiner
	{
		// Token: 0x06000048 RID: 72 RVA: 0x0000235D File Offset: 0x0000055D
		public SaveDefinitions() : base(90787)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000236C File Offset: 0x0000056C
		protected override void DefineClassTypes()
		{
			base.DefineClassTypes();
			base.AddClassDefinition(typeof(TroopSaveData), 70910, null);
			base.AddClassDefinition(typeof(TroopBodySaveData), 70911, null);
			base.AddClassDefinition(typeof(TroopEquipmentData), 70912, null);
			base.AddClassDefinition(typeof(TroopSkillData), 70913, null);
			base.AddClassDefinition(typeof(FactionSaveData), 70920, null);
			base.AddClassDefinition(typeof(TroopCombatStats), 70930, null);
			base.AddClassDefinition(typeof(PendingTrainData), 70001, null);
			base.AddClassDefinition(typeof(PendingEquipData), 70002, null);
			base.AddClassDefinition(typeof(EquipmentPolicy), 200901, null);
			base.AddClassDefinition(typeof(LegacyTroopSaveData), 70992, null);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000245C File Offset: 0x0000065C
		protected override void DefineContainerDefinitions()
		{
			base.DefineContainerDefinitions();
			base.ConstructContainerDefinition(typeof(Dictionary<string, PendingTrainData>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, PendingEquipData>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<string, PendingTrainData>>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<string, PendingEquipData>>));
			base.ConstructContainerDefinition(typeof(List<TroopSaveData>));
			base.ConstructContainerDefinition(typeof(List<string>));
			base.ConstructContainerDefinition(typeof(List<FactionSaveData>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, TroopCombatStats>));
			base.ConstructContainerDefinition(typeof(Dictionary<int, byte>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<int, byte>>));
			base.ConstructContainerDefinition(typeof(Dictionary<int, EquipmentPolicy>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, Dictionary<int, EquipmentPolicy>>));
			base.ConstructContainerDefinition(typeof(Dictionary<string, int>));
			base.ConstructContainerDefinition(typeof(List<LegacyTroopSaveData>));
		}
	}
}
