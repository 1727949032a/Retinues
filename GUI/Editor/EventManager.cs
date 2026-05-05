using System;
using System.Collections.Generic;
using Retinues.Utils;

namespace Retinues.GUI.Editor
{
	// Token: 0x02000072 RID: 114
	[SafeClass]
	public static class EventManager
	{
		// Token: 0x06000242 RID: 578 RVA: 0x0000F328 File Offset: 0x0000D528
		internal static void Register(BaseVM vm)
		{
			if (vm == null)
			{
				return;
			}
			object @lock = EventManager._lock;
			lock (@lock)
			{
				EventManager._listeners.Add(new WeakReference<BaseVM>(vm));
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000F378 File Offset: 0x0000D578
		internal static void Unregister(BaseVM vm)
		{
			if (vm == null)
			{
				return;
			}
			object @lock = EventManager._lock;
			lock (@lock)
			{
				for (int i = EventManager._listeners.Count - 1; i >= 0; i--)
				{
					BaseVM baseVM;
					if (EventManager._listeners[i].TryGetTarget(out baseVM) && baseVM == vm)
					{
						EventManager._listeners.RemoveAt(i);
						break;
					}
					BaseVM baseVM2;
					if (!EventManager._listeners[i].TryGetTarget(out baseVM2))
					{
						EventManager._listeners.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000F410 File Offset: 0x0000D610
		public static void Fire(UIEvent e)
		{
			EventManager.NotifySnapshot(delegate(BaseVM vm)
			{
				vm.__OnGlobalPulse(e);
			});
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000F430 File Offset: 0x0000D630
		public static void FireBatch(Action emit)
		{
			if (emit == null)
			{
				return;
			}
			EventManager.BeginPulse();
			try
			{
				emit();
			}
			finally
			{
				EventManager.EndPulse();
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000F464 File Offset: 0x0000D664
		public static void FireSequence(params UIEvent[] events)
		{
			EventManager.BeginPulse();
			try
			{
				if (events != null)
				{
					for (int i = 0; i < events.Length; i++)
					{
						EventManager.Fire(events[i]);
					}
				}
			}
			finally
			{
				EventManager.EndPulse();
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000F4AC File Offset: 0x0000D6AC
		public static void BeginPulse()
		{
			bool flag = false;
			object @lock = EventManager._lock;
			lock (@lock)
			{
				EventManager._depth++;
				if (EventManager._depth == 1)
				{
					flag = true;
				}
			}
			if (flag)
			{
				EventManager.NotifySnapshot(delegate(BaseVM vm)
				{
					vm.__BeginPulse();
				});
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000F524 File Offset: 0x0000D724
		public static void EndPulse()
		{
			bool flag = false;
			object @lock = EventManager._lock;
			lock (@lock)
			{
				if (EventManager._depth > 0 && --EventManager._depth == 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				EventManager.NotifySnapshot(delegate(BaseVM vm)
				{
					vm.__EndPulse();
				});
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000F5A0 File Offset: 0x0000D7A0
		private static void NotifySnapshot(Action<BaseVM> action)
		{
			if (action == null)
			{
				return;
			}
			object @lock = EventManager._lock;
			List<BaseVM> list;
			lock (@lock)
			{
				list = new List<BaseVM>(EventManager._listeners.Count);
				for (int i = EventManager._listeners.Count - 1; i >= 0; i--)
				{
					BaseVM baseVM;
					if (EventManager._listeners[i].TryGetTarget(out baseVM) && baseVM != null)
					{
						list.Add(baseVM);
					}
					else
					{
						EventManager._listeners.RemoveAt(i);
					}
				}
			}
			foreach (BaseVM obj in list)
			{
				try
				{
					action(obj);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x040000C3 RID: 195
		private static readonly List<WeakReference<BaseVM>> _listeners = new List<WeakReference<BaseVM>>();

		// Token: 0x040000C4 RID: 196
		private static readonly object _lock = new object();

		// Token: 0x040000C5 RID: 197
		private static int _depth;
	}
}
