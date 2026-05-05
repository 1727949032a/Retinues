using System;
using System.Collections.Generic;
using System.Globalization;
using Retinues.Utils;

namespace Retinues.Configuration
{
	// Token: 0x020000F6 RID: 246
	public sealed class Option<T> : IOption
	{
		// Token: 0x06000997 RID: 2455 RVA: 0x0002FF78 File Offset: 0x0002E178
		public Option(Func<string> section, Func<string> name, string key, Func<string> hint, T @default, int minValue = 0, int maxValue = 1000, bool requiresRestart = false, IReadOnlyDictionary<string, object> presetOverrides = null, bool disabled = false, T disabledOverride = default(T))
		{
			Func<string> section2 = section;
			if (section == null && (section2 = Option<T>.<>c.<>9__0_0) == null)
			{
				section2 = (Option<T>.<>c.<>9__0_0 = (() => L.S("mcm_section_general", "General")));
			}
			this._section = section2;
			Func<string> name2 = name;
			if (name == null && (name2 = Option<T>.<>c.<>9__0_1) == null)
			{
				name2 = (Option<T>.<>c.<>9__0_1 = (() => string.Empty));
			}
			this._name = name2;
			Func<string> hint2 = hint;
			if (hint == null && (hint2 = Option<T>.<>c.<>9__0_2) == null)
			{
				hint2 = (Option<T>.<>c.<>9__0_2 = (() => string.Empty));
			}
			this._hint = hint2;
			this.Key = key;
			this.RequiresRestart = requiresRestart;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.DefaultTyped = @default;
			this.Getter = (() => default(T));
			this.Setter = delegate(T _)
			{
			};
			this.PresetOverrides = (presetOverrides ?? new Dictionary<string, object>());
			this.IsDisabled = disabled;
			this.DisabledOverride = disabledOverride;
			base..ctor();
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x0003009C File Offset: 0x0002E29C
		public string Section
		{
			get
			{
				return this._section();
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x000300A9 File Offset: 0x0002E2A9
		public string Name
		{
			get
			{
				return this._name();
			}
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x000300B6 File Offset: 0x0002E2B6
		public string Key { get; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x0600099B RID: 2459 RVA: 0x000300BE File Offset: 0x0002E2BE
		public string Hint
		{
			get
			{
				return this._hint();
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x0600099C RID: 2460 RVA: 0x000300CB File Offset: 0x0002E2CB
		public bool RequiresRestart { get; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x000300D3 File Offset: 0x0002E2D3
		public int MinValue { get; }

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x0600099E RID: 2462 RVA: 0x000300DB File Offset: 0x0002E2DB
		public int MaxValue { get; }

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x000300E3 File Offset: 0x0002E2E3
		public T DefaultTyped { get; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060009A0 RID: 2464 RVA: 0x000300EB File Offset: 0x0002E2EB
		// (set) Token: 0x060009A1 RID: 2465 RVA: 0x000300F3 File Offset: 0x0002E2F3
		internal Func<T> Getter { get; set; }

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060009A2 RID: 2466 RVA: 0x000300FC File Offset: 0x0002E2FC
		// (set) Token: 0x060009A3 RID: 2467 RVA: 0x00030104 File Offset: 0x0002E304
		internal Action<T> Setter { get; set; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060009A4 RID: 2468 RVA: 0x0003010D File Offset: 0x0002E30D
		public Type Type
		{
			get
			{
				return typeof(T);
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060009A5 RID: 2469 RVA: 0x00030119 File Offset: 0x0002E319
		public object Default
		{
			get
			{
				return this.DefaultTyped;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060009A6 RID: 2470 RVA: 0x00030126 File Offset: 0x0002E326
		public IReadOnlyDictionary<string, object> PresetOverrides { get; }

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x060009A7 RID: 2471 RVA: 0x0003012E File Offset: 0x0002E32E
		public bool IsDisabled { get; }

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x060009A8 RID: 2472 RVA: 0x00030136 File Offset: 0x0002E336
		public T DisabledOverride { get; }

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x060009A9 RID: 2473 RVA: 0x0003013E File Offset: 0x0002E33E
		public object DisabledOverrideBoxed
		{
			get
			{
				return this.DisabledOverride;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060009AA RID: 2474 RVA: 0x0003014B File Offset: 0x0002E34B
		// (set) Token: 0x060009AB RID: 2475 RVA: 0x00030158 File Offset: 0x0002E358
		public T Value
		{
			get
			{
				return this.Getter();
			}
			set
			{
				this.Setter(value);
			}
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00030166 File Offset: 0x0002E366
		public static implicit operator T(Option<T> o)
		{
			return o.Value;
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0003016E File Offset: 0x0002E36E
		public object GetObject()
		{
			return this.Value;
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0003017B File Offset: 0x0002E37B
		public void SetObject(object value)
		{
			this.Setter((T)((object)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)));
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x000301A4 File Offset: 0x0002E3A4
		public void ApplyPreset(ConfigPreset preset)
		{
			object @object;
			switch (preset)
			{
			case ConfigPreset.Freeform:
				if (!this.PresetOverrides.TryGetValue("freeform", out @object))
				{
					@object = this.DefaultTyped;
					goto IL_64;
				}
				goto IL_64;
			case ConfigPreset.Realistic:
				if (!this.PresetOverrides.TryGetValue("realistic", out @object))
				{
					@object = this.DefaultTyped;
					goto IL_64;
				}
				goto IL_64;
			}
			@object = this.DefaultTyped;
			IL_64:
			this.SetObject(@object);
		}

		// Token: 0x040002BC RID: 700
		private readonly Func<string> _section;

		// Token: 0x040002BD RID: 701
		private readonly Func<string> _name;

		// Token: 0x040002BE RID: 702
		private readonly Func<string> _hint;
	}
}
