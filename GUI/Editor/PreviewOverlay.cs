using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Retinues.Game.Wrappers;
using Retinues.Utils;
using TaleWorlds.Core;

namespace Retinues.GUI.Editor
{
	// Token: 0x02000073 RID: 115
	[SafeClass]
	internal static class PreviewOverlay
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000F69A File Offset: 0x0000D89A
		// (set) Token: 0x0600024C RID: 588 RVA: 0x0000F6A1 File Offset: 0x0000D8A1
		public static bool IsEnabled { get; private set; }

		// Token: 0x0600024D RID: 589 RVA: 0x0000F6A9 File Offset: 0x0000D8A9
		public static void Enable()
		{
			if (PreviewOverlay.IsEnabled)
			{
				return;
			}
			Log.Debug("[PreviewOverlay] Enabling preview mode.");
			PreviewOverlay.IsEnabled = true;
			PreviewOverlay._map.Clear();
			EventManager.Fire(UIEvent.Appearance);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000F6D3 File Offset: 0x0000D8D3
		public static void Disable()
		{
			if (!PreviewOverlay.IsEnabled)
			{
				return;
			}
			Log.Debug("[PreviewOverlay] Disabling preview mode and clearing overlays.");
			PreviewOverlay.IsEnabled = false;
			PreviewOverlay._map.Clear();
			EventManager.Fire(UIEvent.Appearance);
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000F6FD File Offset: 0x0000D8FD
		public static void Toggle()
		{
			if (PreviewOverlay.IsEnabled)
			{
				PreviewOverlay.Disable();
				return;
			}
			PreviewOverlay.Enable();
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000F711 File Offset: 0x0000D911
		public static void ClearAll()
		{
			if (PreviewOverlay._map.Count == 0)
			{
				return;
			}
			Log.Debug("[PreviewOverlay] Clearing all preview overlays.");
			PreviewOverlay._map.Clear();
			if (PreviewOverlay.IsEnabled)
			{
				EventManager.Fire(UIEvent.Appearance);
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000F744 File Offset: 0x0000D944
		public static void ClearForTroop(WCharacter troop)
		{
			if (troop == null || PreviewOverlay._map.Count == 0)
			{
				return;
			}
			string stringId = troop.StringId;
			List<ValueTuple<string, int, EquipmentIndex>> list = new List<ValueTuple<string, int, EquipmentIndex>>();
			foreach (ValueTuple<string, int, EquipmentIndex> valueTuple in PreviewOverlay._map.Keys)
			{
				if (valueTuple.Item1 == stringId)
				{
					list.Add(valueTuple);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (ValueTuple<string, int, EquipmentIndex> key in list)
			{
				PreviewOverlay._map.Remove(key);
			}
			if (PreviewOverlay.IsEnabled)
			{
				EventManager.Fire(UIEvent.Appearance);
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000F828 File Offset: 0x0000DA28
		public static void SetPreview(WCharacter troop, int setIndex, EquipmentIndex slot, WItem item)
		{
			if (!PreviewOverlay.IsEnabled || troop == null)
			{
				return;
			}
			ValueTuple<string, int, EquipmentIndex> key = new ValueTuple<string, int, EquipmentIndex>(troop.StringId, setIndex, slot);
			if (item == null)
			{
				Log.Debug(string.Format("[PreviewOverlay] Setting EMPTY preview for {0}, set {1}, slot {2}.", troop.StringId, setIndex, slot));
				PreviewOverlay._map[key] = null;
			}
			else
			{
				Log.Debug(string.Format("[PreviewOverlay] Setting preview {0} for {1}, set {2}, slot {3}.", new object[]
				{
					item.StringId,
					troop.StringId,
					setIndex,
					slot
				}));
				PreviewOverlay._map[key] = item;
			}
			EventManager.Fire(UIEvent.Appearance);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000F8D8 File Offset: 0x0000DAD8
		public static bool TryBuildEquipment(WCharacter troop, int setIndex, Equipment baseEquipment, out Equipment result)
		{
			result = null;
			if (!PreviewOverlay.IsEnabled || troop == null || baseEquipment == null)
			{
				return false;
			}
			string stringId = troop.StringId;
			bool flag = false;
			Equipment equipment = new Equipment(baseEquipment);
			foreach (KeyValuePair<ValueTuple<string, int, EquipmentIndex>, WItem> keyValuePair in PreviewOverlay._map)
			{
				ValueTuple<string, int, EquipmentIndex> key = keyValuePair.Key;
				string item = key.Item1;
				int item2 = key.Item2;
				EquipmentIndex item3 = key.Item3;
				if (!(item != stringId) && item2 == setIndex)
				{
					WItem value = keyValuePair.Value;
					if (value == null)
					{
						equipment[item3] = default(EquipmentElement);
						flag = true;
					}
					else if (value.Base != null)
					{
						equipment[item3] = new EquipmentElement(value.Base, null, null, false);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			result = equipment;
			return true;
		}

		// Token: 0x040000C6 RID: 198
		[TupleElementNames(new string[]
		{
			"TroopId",
			"SetIndex",
			"Slot"
		})]
		private static readonly Dictionary<ValueTuple<string, int, EquipmentIndex>, WItem> _map = new Dictionary<ValueTuple<string, int, EquipmentIndex>, WItem>();
	}
}
