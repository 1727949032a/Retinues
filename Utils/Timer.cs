using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Retinues.Utils
{
	// Token: 0x02000034 RID: 52
	public static class Timer
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006482 File Offset: 0x00004682
		public static bool IsRunning
		{
			get
			{
				return Timer._running;
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00006489 File Offset: 0x00004689
		public static void Start()
		{
			Timer.Total.Reset();
			Timer.Total.Start();
			Timer.Segments.Clear();
			Timer._running = true;
			Log.Debug("Timer.Start");
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000064BC File Offset: 0x000046BC
		public static void Begin(string label)
		{
			if (string.IsNullOrEmpty(label))
			{
				return;
			}
			if (!Timer._running)
			{
				return;
			}
			Timer.Segment segment;
			if (!Timer.Segments.TryGetValue(label, out segment))
			{
				segment = new Timer.Segment
				{
					Label = label
				};
				Timer.Segments[label] = segment;
			}
			segment.Depth++;
			if (segment.Depth == 1 && !segment.Stopwatch.IsRunning)
			{
				segment.Stopwatch.Start();
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00006534 File Offset: 0x00004734
		public static void End(string label)
		{
			if (string.IsNullOrEmpty(label))
			{
				return;
			}
			if (!Timer._running)
			{
				return;
			}
			Timer.Segment segment;
			if (!Timer.Segments.TryGetValue(label, out segment))
			{
				return;
			}
			if (segment.Depth > 0)
			{
				segment.Depth--;
			}
			if (segment.Depth <= 0)
			{
				segment.Depth = 0;
				if (segment.Stopwatch.IsRunning)
				{
					segment.Stopwatch.Stop();
				}
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000065A4 File Offset: 0x000047A4
		public static void Stop()
		{
			if (!Timer._running)
			{
				return;
			}
			Timer._running = false;
			Timer.Total.Stop();
			foreach (Timer.Segment segment in Timer.Segments.Values)
			{
				if (segment.Stopwatch.IsRunning)
				{
					segment.Stopwatch.Stop();
				}
				segment.Depth = 0;
			}
			double num = Timer.Total.Elapsed.TotalSeconds;
			if (num <= 0.0)
			{
				double num2 = 0.0;
				foreach (Timer.Segment segment2 in Timer.Segments.Values)
				{
					num2 += segment2.Elapsed.TotalSeconds;
				}
				num = num2;
			}
			Log.Debug(string.Format("Time: {0:0.00000}s", num));
			if (Timer.Segments.Count == 0)
			{
				return;
			}
			foreach (Timer.Segment segment3 in Timer.Segments.Values)
			{
				double totalSeconds = segment3.Elapsed.TotalSeconds;
				double num3 = (num > 0.0) ? (totalSeconds / num * 100.0) : 0.0;
				Log.Debug(string.Format("{0}: {1:0.00000}s ({2:0.0}%)", segment3.Label, totalSeconds, num3));
			}
		}

		// Token: 0x0400004B RID: 75
		private static readonly Stopwatch Total = new Stopwatch();

		// Token: 0x0400004C RID: 76
		private static readonly Dictionary<string, Timer.Segment> Segments = new Dictionary<string, Timer.Segment>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x0400004D RID: 77
		private static bool _running;

		// Token: 0x0200010D RID: 269
		private sealed class Segment
		{
			// Token: 0x17000456 RID: 1110
			// (get) Token: 0x06000A59 RID: 2649 RVA: 0x00030A1B File Offset: 0x0002EC1B
			public TimeSpan Elapsed
			{
				get
				{
					return this.Stopwatch.Elapsed;
				}
			}

			// Token: 0x040002FF RID: 767
			public string Label;

			// Token: 0x04000300 RID: 768
			public Stopwatch Stopwatch = new Stopwatch();

			// Token: 0x04000301 RID: 769
			public int Depth;
		}
	}
}
