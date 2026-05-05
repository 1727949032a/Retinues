using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MCM.Abstractions;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.FluentBuilder;
using MCM.Abstractions.FluentBuilder.Models;
using MCM.Common;
using Retinues.GUI.Helpers;
using Retinues.Safety.Sanitizer;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Retinues.Configuration
{
	// Token: 0x020000F4 RID: 244
	public static class Config
	{
		// Token: 0x06000974 RID: 2420 RVA: 0x0002D3A4 File Offset: 0x0002B5A4
		public static Option<T> CreateOption<T>(Func<string> section, Func<string> name, string key, Func<string> hint, T @default, int minValue = 0, int maxValue = 1000, bool requiresRestart = false, IReadOnlyDictionary<string, object> presets = null, bool disabled = false, T disabledOverride = default(T))
		{
			return new Option<T>(section, name, key, hint, @default, minValue, maxValue, requiresRestart, presets, disabled, disabledOverride);
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000975 RID: 2421 RVA: 0x0002D3C8 File Offset: 0x0002B5C8
		// (remove) Token: 0x06000976 RID: 2422 RVA: 0x0002D3FC File Offset: 0x0002B5FC
		public static event Action<string, object> OptionChanged;

		// Token: 0x06000977 RID: 2423 RVA: 0x0002D430 File Offset: 0x0002B630
		private static void RaiseOptionChanged(string key, object value)
		{
			try
			{
				Action<string, object> optionChanged = Config.OptionChanged;
				if (optionChanged != null)
				{
					optionChanged(key, value);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0002D470 File Offset: 0x0002B670
		internal static void SetRawValue(string key, object value)
		{
			Config._values[key] = value;
			Config.RaiseOptionChanged(key, value);
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0002D488 File Offset: 0x0002B688
		public static bool RegisterWithMCM()
		{
			bool result;
			try
			{
				Log.Info("Config.RegisterWithMCM: attempting to register with MCM…");
				Config.DiscoverOptions();
				if (Config.BuildMcmMenu())
				{
					Log.Info("Config.RegisterWithMCM: Config options registered with MCM.");
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
				result = false;
			}
			return result;
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0002D4E0 File Offset: 0x0002B6E0
		public static void ApplyPresetsToAll(ConfigPreset preset)
		{
			Config.DiscoverOptions();
			string text = null;
			switch (preset)
			{
			case ConfigPreset.Freeform:
				text = "freeform";
				break;
			case ConfigPreset.Realistic:
				text = "realistic";
				break;
			}
			foreach (IOption option in Config._all)
			{
				object @default;
				if (text == null)
				{
					@default = option.Default;
				}
				else if (!option.PresetOverrides.TryGetValue(text, out @default))
				{
					@default = option.Default;
				}
				option.SetObject(@default);
			}
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0002D580 File Offset: 0x0002B780
		public static void SaveSettings()
		{
			try
			{
				if (Config._mcmSettings == null)
				{
					Log.Debug("[MCM] SaveSettings() called but _mcmSettings is null; skipping.");
				}
				else
				{
					BaseSettingsProvider instance = BaseSettingsProvider.Instance;
					if (instance == null)
					{
						Log.Debug("[MCM] SaveSettings() called but BaseSettingsProvider.Instance is null; skipping.");
					}
					else
					{
						instance.SaveSettings(Config._mcmSettings);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Warn("[MCM] SaveSettings() failed.");
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0002D5EC File Offset: 0x0002B7EC
		private static void DiscoverOptions()
		{
			if (Config._all.Count > 0)
			{
				return;
			}
			IEnumerable<FieldInfo> enumerable = from f in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public)
			where typeof(IOption).IsAssignableFrom(f.FieldType)
			orderby f.MetadataToken
			select f;
			int num = 0;
			foreach (FieldInfo fieldInfo in enumerable)
			{
				IOption option = (IOption)fieldInfo.GetValue(null);
				Config._all.Add(option);
				Config._byKey[option.Key] = option;
				Config._ordinalByKey[option.Key] = num++;
				Config._values[option.Key] = option.Default;
				Type fieldType = fieldInfo.FieldType;
				Type type = fieldType.GenericTypeArguments[0];
				PropertyInfo property = fieldType.GetProperty("Getter", BindingFlags.Instance | BindingFlags.NonPublic);
				PropertyInfo property2 = fieldType.GetProperty("Setter", BindingFlags.Instance | BindingFlags.NonPublic);
				MethodInfo methodInfo = typeof(Config).GetMethod("MakeGetter", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
				{
					type
				});
				MethodInfo methodInfo2 = typeof(Config).GetMethod("MakeSetter", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
				{
					type
				});
				object value = methodInfo.Invoke(null, new object[]
				{
					option.Key
				});
				object value2 = methodInfo2.Invoke(null, new object[]
				{
					option.Key
				});
				property.SetValue(option, value);
				property2.SetValue(option, value2);
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0002D7BC File Offset: 0x0002B9BC
		private static Func<T> MakeGetter<T>(string key)
		{
			return delegate()
			{
				T result = (T)((object)Convert.ChangeType(Config._values[key], typeof(T), CultureInfo.InvariantCulture));
				IOption option;
				if (Config._byKey.TryGetValue(key, out option))
				{
					Option<T> option2 = option as Option<T>;
					if (option2 != null && option2.IsDisabled)
					{
						return option2.DisabledOverride;
					}
				}
				return result;
			};
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0002D7D5 File Offset: 0x0002B9D5
		private static Action<T> MakeSetter<T>(string key)
		{
			return delegate(T v)
			{
				Config.SetRawValue(key, v);
			};
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x0002D7F0 File Offset: 0x0002B9F0
		private static bool BuildMcmMenu()
		{
			ISettingsBuilder settingsBuilder = BaseSettingsBuilder.Create("Retinues.Settings", "Retinues");
			if (settingsBuilder == null)
			{
				Log.Warn("MCM not ready (BaseSettingsBuilder.Create returned null). Skipping registration this tick.");
				return false;
			}
			settingsBuilder.SetFolderName("Retinues").SetFormat("xml");
			settingsBuilder.CreateGroup(L.S("mcm_section_import_export", "Import & Export"), delegate(ISettingsPropertyGroupBuilder group)
			{
				group.SetGroupOrder(-100);
				int order = 0;
				group.AddButton("ExportButton", L.S("mcm_ie_export_btn_text", "Export Troops to XML"), new ProxyRef<Action>(() => delegate()
				{
					if (!Config.InCampaign())
					{
						Log.Message(L.S("not_in_running_campaign", "Not in a running campaign. Load a save first."));
						return;
					}
					try
					{
						Directory.CreateDirectory(TroopImportExport.DefaultDir);
						TroopImportExport.PromptAndExport(string.IsNullOrWhiteSpace(Config._exportName) ? TroopImportExport.SuggestTimestampName("troops") : Config._exportName);
					}
					catch (Exception ex)
					{
						Notifications.Popup(L.T("export_fail_title", "Export Failed"), L.T("export_fail_body", ex.Message), null, true);
					}
				}, delegate(Action _)
				{
				}), L.S("mcm_ie_export_btn", "Export"), delegate(ISettingsPropertyButtonBuilder b)
				{
					int order = order;
					order++;
					b.SetOrder(order).SetRequireRestart(false).SetHintText(L.S("mcm_ie_export_btn_hint", "Exports troop definitions to XML."));
				});
				group.AddButton("ImportFromXml", L.S("mcm_ie_import_pick_btn_text", "Import Troops from XML"), new ProxyRef<Action>(() => delegate()
				{
					if (!Config.InCampaign())
					{
						Log.Message(L.S("not_in_running_campaign", "Not in a running campaign. Load a save first."));
						return;
					}
					try
					{
						TroopImportExport.PickAndImportUnified(null);
					}
					catch (Exception ex)
					{
						Notifications.Popup(L.T("import_fail_title", "Import Failed"), L.T("import_fail_body", ex.Message), null, true);
					}
				}, delegate(Action _)
				{
				}), L.S("mcm_ie_import_btn", "Import"), delegate(ISettingsPropertyButtonBuilder b)
				{
					int order = order;
					order++;
					b.SetOrder(order).SetRequireRestart(false).SetHintText(L.S("mcm_ie_import_btn_hint", "Imports troop definitions from XML."));
				});
			});
			string[] source = new string[]
			{
				L.S("mcm_section_global_editor", "Global Editor"),
				L.S("mcm_section_doctrines", "Doctrines"),
				L.S("mcm_section_ui", "User Interface"),
				L.S("mcm_section_retinues", "Retinues"),
				L.S("mcm_section_troop_unlocks", "Troop Unlocks"),
				L.S("mcm_section_recruitment", "Recruitment"),
				L.S("mcm_section_equipment_unlocks", "Equipment Unlocks"),
				L.S("mcm_section_equipment", "Equipment"),
				L.S("mcm_section_skills", "Skills"),
				L.S("mcm_section_restrictions", "Restrictions"),
				L.S("mcm_section_skill_caps", "Skill Caps"),
				L.S("mcm_section_skill_totals", "Skill Totals"),
				L.S("mcm_section_debug", "Debug")
			};
			Dictionary<string, int> index = source.Select((string name, int i) => new ValueTuple<string, int>(name, i)).ToDictionary(([TupleElementNames(new string[]
			{
				"name",
				"i"
			})] ValueTuple<string, int> x) => x.Item1, ([TupleElementNames(new string[]
			{
				"name",
				"i"
			})] ValueTuple<string, int> x) => x.Item2, StringComparer.InvariantCulture);
			using (IEnumerator<IGrouping<string, IOption>> enumerator = (from o in Config._all
			group o by o.Section).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IGrouping<string, IOption> groupBySection = enumerator.Current;
					settingsBuilder.CreateGroup(groupBySection.Key, delegate(ISettingsPropertyGroupBuilder group)
					{
						int num;
						group.SetGroupOrder(index.TryGetValue(groupBySection.Key, out num) ? num : int.MaxValue);
						int order = 0;
						using (IEnumerator<IOption> enumerator2 = groupBySection.OrderBy(delegate(IOption o)
						{
							int result;
							if (!Config._ordinalByKey.TryGetValue(o.Key, out result))
							{
								return int.MaxValue;
							}
							return result;
						}).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IOption opt = enumerator2.Current;
								string id = opt.Key;
								string name = opt.Name;
								if (opt.IsDisabled)
								{
									Config._values[id] = opt.DisabledOverrideBoxed;
								}
								else
								{
									TypeCode typeCode = Type.GetTypeCode(opt.Type);
									if (typeCode <= TypeCode.Int32)
									{
										if (typeCode != TypeCode.Boolean)
										{
											if (typeCode == TypeCode.Int32)
											{
												group.AddInteger(id, name, opt.MinValue, opt.MaxValue, new ProxyRef<int>(() => Convert.ToInt32(Config._values[id], CultureInfo.InvariantCulture), delegate(int v)
												{
													Config.SetRawValue(id, v);
												}), delegate(ISettingsPropertyIntegerBuilder b)
												{
													int order = order;
													order++;
													b.SetOrder(order).SetHintText(opt.Hint).SetRequireRestart(opt.RequiresRestart);
												});
											}
										}
										else
										{
											group.AddBool(id, name, new ProxyRef<bool>(() => (bool)Config._values[id], delegate(bool v)
											{
												Config.SetRawValue(id, v);
											}), delegate(ISettingsPropertyBoolBuilder b)
											{
												int order = order;
												order++;
												b.SetOrder(order).SetHintText(opt.Hint).SetRequireRestart(opt.RequiresRestart);
											});
										}
									}
									else if (typeCode - TypeCode.Single > 1)
									{
										if (typeCode == TypeCode.String)
										{
											group.AddText(id, name, new ProxyRef<string>(() => (string)Config._values[id], delegate(string v)
											{
												Config.SetRawValue(id, v);
											}), delegate(ISettingsPropertyTextBuilder b)
											{
												int order = order;
												order++;
												b.SetOrder(order).SetHintText(opt.Hint).SetRequireRestart(opt.RequiresRestart);
											});
										}
									}
									else
									{
										group.AddFloatingInteger(id, name, (float)opt.MinValue, (float)opt.MaxValue, new ProxyRef<float>(() => Convert.ToSingle(Config._values[id], CultureInfo.InvariantCulture), delegate(float v)
										{
											Config.SetRawValue(id, v);
										}), delegate(ISettingsPropertyFloatingIntegerBuilder b)
										{
											int order = order;
											order++;
											b.SetOrder(order).SetHintText(opt.Hint).SetRequireRestart(opt.RequiresRestart);
										});
									}
								}
							}
						}
						string value = L.S("mcm_section_debug", "Debug");
						if (groupBySection.Key.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							int tailOrder = order + 999;
							group.AddButton("Danger_RemoveAllCustomTroopData", L.S("mcm_debug_remove_all_title", "Purge Custom Troop Data"), new ProxyRef<Action>(() => delegate()
							{
								if (!Config.InCampaign())
								{
									Log.Message(L.S("not_in_running_campaign", "Not in a running campaign. Load a save first."));
									return;
								}
								Config.ConfirmTroopReplace(L.S("mcm_debug_remove_all_confirm_title", "Remove all custom troop data?"), L.S("mcm_debug_remove_all_confirm_body", "This will permanently purge all Retinues custom troops from the current world so you can safely uninstall the mod.\n\nThis operation is IRREVERSIBLE. Backup your save before proceeding."), delegate
								{
									try
									{
										SanitizerBehavior.Sanitize(true);
										InformationManager.ShowInquiry(new InquiryData(L.S("mcm_purge_done_title", "Purge Complete"), L.S("mcm_purge_done_body", "All custom troop data has been removed.\n\nSave your game, quit, remove the mod, then reload.\n\nIf your save still crashes after doing this, download the 'Retinues - Disabled' file from the Nexus mod page as a last resort."), true, true, L.S("mcm_purge_done_open_nexus", "Open Nexus Page"), L.S("str_ok", "OK"), delegate()
										{
											URL.OpenInBrowser("https://www.nexusmods.com/mountandblade2bannerlord/mods/8847?tab=files");
										}, delegate()
										{
										}, "", 0f, null, null, null), false, false);
									}
									catch (Exception ex)
									{
										Log.Message("Sanitization failed, see log for details.");
										Log.Exception(ex, "", null);
									}
								});
							}, delegate(Action _)
							{
							}), L.S("mcm_debug_remove_all_btn", "Purge"), delegate(ISettingsPropertyButtonBuilder b)
							{
								b.SetOrder(tailOrder).SetRequireRestart(false).SetHintText(L.S("mcm_debug_remove_all_hint", "Will purge the current save of all Retinues custom troop data so the mod can be uninstalled safely. Warning: this action is irreversible."));
							});
						}
					});
				}
			}
			Config._all.ToDictionary((IOption o) => o.Key, (IOption o) => o.Default);
			Dictionary<string, object> freeform = Config._all.ToDictionary((IOption o) => o.Key, delegate(IOption o)
			{
				object result;
				if (!o.PresetOverrides.TryGetValue("freeform", out result))
				{
					return o.Default;
				}
				return result;
			});
			Dictionary<string, object> realistic = Config._all.ToDictionary((IOption o) => o.Key, delegate(IOption o)
			{
				object result;
				if (!o.PresetOverrides.TryGetValue("realistic", out result))
				{
					return o.Default;
				}
				return result;
			});
			settingsBuilder.CreatePreset("freeform", "Freeform", delegate(ISettingsPresetBuilder p)
			{
				Config.<BuildMcmMenu>g__ApplyPreset|103_4(p, freeform);
			});
			settingsBuilder.CreatePreset("realistic", "Realistic", delegate(ISettingsPresetBuilder p)
			{
				Config.<BuildMcmMenu>g__ApplyPreset|103_4(p, realistic);
			});
			Config._mcmSettings = settingsBuilder.BuildAsGlobal();
			(from x in Config._all
			group x by x.Section).ToDictionary((IGrouping<string, IOption> g) => g.Key, (IGrouping<string, IOption> g) => g.Count<IOption>());
			if (Config._mcmSettings == null)
			{
				return false;
			}
			Config.HookMcmSettings(Config._mcmSettings);
			Config._mcmSettings.Register();
			return true;
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0002DC1C File Offset: 0x0002BE1C
		private static bool InCampaign()
		{
			bool result;
			try
			{
				result = (Campaign.Current != null);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0002DC4C File Offset: 0x0002BE4C
		private static string SuggestDefaultExportName()
		{
			string str = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
			return "troops_" + str + ".xml";
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0002DC7C File Offset: 0x0002BE7C
		private static void ConfirmTroopReplace(string title, string body, Action onConfirm)
		{
			InformationManager.ShowInquiry(new InquiryData(title, body, true, true, L.S("continue", "Continue"), L.S("cancel", "Cancel"), onConfirm, delegate()
			{
			}, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0002DCE4 File Offset: 0x0002BEE4
		public static void LogDump()
		{
			try
			{
				Config.DiscoverOptions();
				Log.Info("Retinues Config:");
				foreach (IGrouping<string, IOption> grouping in (from o in Config._all
				group o by o.Section).OrderBy(delegate(IGrouping<string, IOption> g)
				{
					int num = int.MaxValue;
					foreach (IOption option2 in g)
					{
						int num2;
						if (Config._ordinalByKey.TryGetValue(option2.Key, out num2) && num2 < num)
						{
							num = num2;
						}
					}
					return num;
				}))
				{
					Log.Info(string.Format("[Section] {0} ({1} options)", grouping.Key, grouping.Count<IOption>()));
					foreach (IOption option in grouping.OrderBy(delegate(IOption o)
					{
						int result;
						if (!Config._ordinalByKey.TryGetValue(o.Key, out result))
						{
							return int.MaxValue;
						}
						return result;
					}))
					{
						object @object = option.GetObject();
						object @default = option.Default;
						string text = Config.FormatConfigValue(@object);
						string text2 = Config.FormatConfigValue(@default);
						string text3 = (!object.Equals(@object, @default)) ? "*" : " ";
						string text4 = string.IsNullOrWhiteSpace(option.Name) ? option.Key : (option.Name + " [" + option.Key + "]");
						if (option.IsDisabled)
						{
							Log.Info(string.Concat(new string[]
							{
								text3,
								" ",
								text4,
								" = ",
								text,
								" (DISABLED; override)"
							}));
						}
						else
						{
							Log.Info(string.Concat(new string[]
							{
								text3,
								" ",
								text4,
								" = ",
								text,
								" (default: ",
								text2,
								")"
							}));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0002DF30 File Offset: 0x0002C130
		private static string FormatConfigValue(object value)
		{
			if (value == null)
			{
				return "null";
			}
			return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null";
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0002DF50 File Offset: 0x0002C150
		private static void HookMcmSettings(object settings)
		{
			if (settings == null)
			{
				return;
			}
			Config._mcmSettingsInstance = settings;
			Config._mcmSettingsType = settings.GetType();
			Action<string, object> value;
			if ((value = Config.<>O.<0>__SyncOptionToMcm) == null)
			{
				value = (Config.<>O.<0>__SyncOptionToMcm = new Action<string, object>(Config.SyncOptionToMcm));
			}
			Config.OptionChanged -= value;
			Action<string, object> value2;
			if ((value2 = Config.<>O.<0>__SyncOptionToMcm) == null)
			{
				value2 = (Config.<>O.<0>__SyncOptionToMcm = new Action<string, object>(Config.SyncOptionToMcm));
			}
			Config.OptionChanged += value2;
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0002DFB4 File Offset: 0x0002C1B4
		private static void SyncOptionToMcm(string key, object value)
		{
			object mcmSettingsInstance = Config._mcmSettingsInstance;
			Type mcmSettingsType = Config._mcmSettingsType;
			if (mcmSettingsInstance == null || mcmSettingsType == null)
			{
				return;
			}
			if (Config._isSyncingWithMcm)
			{
				return;
			}
			try
			{
				PropertyInfo property = mcmSettingsType.GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property != null && property.CanWrite)
				{
					Config._isSyncingWithMcm = true;
					Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
					object value2 = Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
					property.SetValue(mcmSettingsInstance, value2);
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
			finally
			{
				Config._isSyncingWithMcm = false;
			}
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0002E05C File Offset: 0x0002C25C
		// Note: this type is marked as 'beforefieldinit'.
		static Config()
		{
			Func<string> section = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name = () => L.S("mcm_option_max_elite_retinue_ratio", "Max Elite Retinue Ratio");
			string key = "MaxEliteRetinueRatio";
			Func<string> hint = () => L.S("mcm_option_max_elite_retinue_ratio_hint", "Maximum proportion of elite retinues in player party.");
			float @default = 0.1f;
			int minValue = 0;
			int maxValue = 1;
			bool requiresRestart = false;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["freeform"] = 1f;
			dictionary["realistic"] = 0.05f;
			Config.MaxEliteRetinueRatio = Config.CreateOption<float>(section, name, key, hint, @default, minValue, maxValue, requiresRestart, dictionary, false, 0f);
			Func<string> section2 = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name2 = () => L.S("mcm_option_max_basic_retinue_ratio", "Max Basic Retinue Ratio");
			string key2 = "MaxBasicRetinueRatio";
			Func<string> hint2 = () => L.S("mcm_option_max_basic_retinue_ratio_hint", "Maximum proportion of basic retinues in player party.");
			float default2 = 0.2f;
			int minValue2 = 0;
			int maxValue2 = 1;
			bool requiresRestart2 = false;
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["freeform"] = 1f;
			dictionary2["realistic"] = 0.1f;
			Config.MaxBasicRetinueRatio = Config.CreateOption<float>(section2, name2, key2, hint2, default2, minValue2, maxValue2, requiresRestart2, dictionary2, false, 0f);
			Func<string> section3 = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name3 = () => L.S("mcm_option_rank_up_cost_per_tier", "Rank Up Cost (Per Tier)");
			string key3 = "RankUpCostPerTier";
			Func<string> hint3 = () => L.S("mcm_option_rank_up_cost_per_tier_hint", "Rank up cost for retinue troops per tier.");
			int default3 = 1000;
			int minValue3 = 0;
			int maxValue3 = 5000;
			bool requiresRestart3 = false;
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3["freeform"] = 0;
			dictionary3["realistic"] = 2000;
			Config.RankUpCostPerTier = Config.CreateOption<int>(section3, name3, key3, hint3, default3, minValue3, maxValue3, requiresRestart3, dictionary3, false, 0);
			Func<string> section4 = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name4 = () => L.S("mcm_option_gold_conversion_cost_per_tier", "Gold Conversion Cost (Per Tier)");
			string key4 = "GoldConversionCostPerTier";
			Func<string> hint4 = () => L.S("mcm_option_gold_conversion_cost_per_tier_hint", "Gold cost to convert a troop into a retinue, per tier of the target retinue troop.");
			int default4 = 50;
			int minValue4 = 0;
			int maxValue4 = 200;
			bool requiresRestart4 = false;
			Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
			dictionary4["freeform"] = 0;
			dictionary4["realistic"] = 100;
			Config.GoldConversionCostPerTier = Config.CreateOption<int>(section4, name4, key4, hint4, default4, minValue4, maxValue4, requiresRestart4, dictionary4, false, 0);
			Func<string> section5 = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name5 = () => L.S("mcm_option_influence_conversion_cost_per_tier", "Influence Conversion Cost (Per Tier)");
			string key5 = "InfluenceConversionCostPerTier";
			Func<string> hint5 = () => L.S("mcm_option_influence_conversion_cost_per_tier_hint", "Influence cost to convert a troop into a retinue, per tier of the target retinue troop.");
			int default5 = 5;
			int minValue5 = 0;
			int maxValue5 = 10;
			bool requiresRestart5 = false;
			Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
			dictionary5["freeform"] = 0;
			dictionary5["realistic"] = 10;
			Config.InfluenceConversionCostPerTier = Config.CreateOption<int>(section5, name5, key5, hint5, default5, minValue5, maxValue5, requiresRestart5, dictionary5, false, 0);
			Func<string> section6 = () => L.S("mcm_section_retinues", "Retinues");
			Func<string> name6 = () => L.S("mcm_option_renown_required_per_tier", "Renown Required (Per Tier)");
			string key6 = "RenownRequiredPerTier";
			Func<string> hint6 = () => L.S("mcm_option_renown_required_per_tier_hint", "Renown required for a retinue to join automatically, per tier of the retinue troop.");
			int default6 = 10;
			int minValue6 = 1;
			int maxValue6 = 100;
			bool requiresRestart6 = false;
			Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
			dictionary6["freeform"] = 10;
			dictionary6["realistic"] = 20;
			Config.RenownRequiredPerTier = Config.CreateOption<int>(section6, name6, key6, hint6, default6, minValue6, maxValue6, requiresRestart6, dictionary6, false, 0);
			Func<string> section7 = () => L.S("mcm_section_troop_unlocks", "Troop Unlocks");
			Func<string> name7 = () => L.S("mcm_option_no_fief_requirements", "No Fief Requirements");
			string key7 = "NoFiefRequirement";
			Func<string> hint7 = () => L.S("mcm_option_no_fief_requirements_hint", "Troops can be unlocked without having to own a fief.");
			bool default7 = false;
			int minValue7 = 0;
			int maxValue7 = 1000;
			bool requiresRestart7 = false;
			Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
			dictionary7["freeform"] = true;
			dictionary7["realistic"] = false;
			Config.NoFiefRequirements = Config.CreateOption<bool>(section7, name7, key7, hint7, default7, minValue7, maxValue7, requiresRestart7, dictionary7, false, false);
			Func<string> section8 = () => L.S("mcm_section_troop_unlocks", "Troop Unlocks");
			Func<string> name8 = () => L.S("mcm_option_no_doctrine_requirements", "No Doctrine Requirements");
			string key8 = "NoDoctrineRequirements";
			Func<string> hint8 = () => L.S("mcm_option_no_doctrine_requirements_hint", "Special troops (militias, villagers, caravan guards) can be acquired without the appropriate doctrines.");
			bool default8 = false;
			int minValue8 = 0;
			int maxValue8 = 1000;
			bool requiresRestart8 = false;
			Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
			dictionary8["freeform"] = true;
			dictionary8["realistic"] = false;
			Config.NoDoctrineRequirements = Config.CreateOption<bool>(section8, name8, key8, hint8, default8, minValue8, maxValue8, requiresRestart8, dictionary8, false, false);
			Config.DisableKingdomTroops = Config.CreateOption<bool>(() => L.S("mcm_section_troop_unlocks", "Troop Unlocks"), () => L.S("mcm_option_disable_kingdom_troops", "Disable Kingdom Troops"), "DisableKingdomTroops", () => L.S("mcm_option_disable_kingdom_troops_hint", "The custom kingdom troop tree will be disabled and clan troops will be used instead."), false, 0, 1000, false, null, false, false);
			Func<string> section9 = () => L.S("mcm_section_troop_unlocks", "Troop Unlocks");
			Func<string> name9 = () => L.S("mcm_option_copy_all_sets_on_unlock", "Copy All Sets On Unlock");
			string key9 = "CopyAllSetsOnUnlock";
			Func<string> hint9 = () => L.S("mcm_option_copy_all_sets_on_unlock_hint", "When unlocking a new troop, copy all equipment sets from the original troop instead of only the main battle and civilian sets.");
			bool default9 = false;
			int minValue9 = 0;
			int maxValue9 = 1000;
			bool requiresRestart9 = false;
			Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
			dictionary9["freeform"] = true;
			dictionary9["realistic"] = false;
			Config.CopyAllSetsOnUnlock = Config.CreateOption<bool>(section9, name9, key9, hint9, default9, minValue9, maxValue9, requiresRestart9, dictionary9, false, false);
			Config.CustomVolunteersProportion = Config.CreateOption<float>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_custom_volunteer_proportion", "Custom Volunteer Proportion"), "CustomVolunteerProportion", () => L.S("mcm_option_custom_volunteer_proportion_hint", "Chance for each vanilla volunteer to be replaced by a custom troop (0 = never, 1 = always). Set a lower value if you want to keep some vanilla volunteers in your settlements."), 1f, 0, 1, false, null, false, 0f);
			Config.KingdomVolunteersInClanFiefsProportion = Config.CreateOption<float>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_kingdom_volunteers_in_clan_fiefs_proportion", "Kingdom Volunteers In Clan Fiefs Proportion"), "KingdomVolunteersInClanFiefsProportion", () => L.S("mcm_option_kingdom_volunteers_in_clan_fiefs_proportion_hint", "Chance for each volunteer in the player clan's fiefs to be a kingdom troop (0 = never, 1 = always). Set a higher value if you want to mix kingdom troops with clan troops."), 0f, 0, 1, false, null, false, 0f);
			Config.ClanVolunteersInKingdomFiefsProportion = Config.CreateOption<float>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_clan_volunteers_in_kingdom_fiefs_proportion", "Clan Volunteers In Kingdom Fiefs Proportion"), "ClanVolunteersInKingdomFiefsProportion", () => L.S("mcm_option_clan_volunteers_in_kingdom_fiefs_proportion_hint", "Chance for each volunteer in the player kingdom's fiefs to be a clan troop (0 = never, 1 = always). Set a higher value if you want to mix clan troops with kingdom troops."), 0f, 0, 1, false, null, false, 0f);
			Func<string> section10 = () => L.S("mcm_section_recruitment", "Recruitment");
			Func<string> name10 = () => L.S("mcm_option_restrict_to_owned_settlements", "Restrict To Owned Settlements");
			string key10 = "RestrictToOwnedSettlements";
			Func<string> hint10 = () => L.S("mcm_option_restrict_to_owned_settlements_hint", "Custom troops can only be recruited in settlements owned by the player's clan or kingdom.");
			bool default10 = true;
			int minValue10 = 0;
			int maxValue10 = 1000;
			bool requiresRestart10 = false;
			Dictionary<string, object> dictionary10 = new Dictionary<string, object>();
			dictionary10["freeform"] = false;
			dictionary10["realistic"] = true;
			Config.RestrictToOwnedSettlements = Config.CreateOption<bool>(section10, name10, key10, hint10, default10, minValue10, maxValue10, requiresRestart10, dictionary10, false, false);
			Func<string> section11 = () => L.S("mcm_section_recruitment", "Recruitment");
			Func<string> name11 = () => L.S("mcm_option_restrict_to_same_culture_settlements", "Restrict To Same Culture Settlements");
			string key11 = "RestrictToSameCultureSettlements";
			Func<string> hint11 = () => L.S("mcm_option_restrict_to_same_culture_settlements_hint", "Volunteers in settlements of a different culture will not be replaced by custom troops.");
			bool default11 = false;
			int minValue11 = 0;
			int maxValue11 = 1000;
			bool requiresRestart11 = false;
			Dictionary<string, object> dictionary11 = new Dictionary<string, object>();
			dictionary11["freeform"] = false;
			dictionary11["realistic"] = true;
			Config.RestrictToSameCultureSettlements = Config.CreateOption<bool>(section11, name11, key11, hint11, default11, minValue11, maxValue11, requiresRestart11, dictionary11, false, false);
			Config.VassalLordsCanRecruitCustomTroops = Config.CreateOption<bool>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_vassal_lords_recruit_custom_troops", "Vassal Lords Recruit Custom Troops"), "VassalLordsCanRecruitCustomTroops", () => L.S("mcm_option_vassal_lords_recruit_custom_troops_hint", "Lords of the player's clan or kingdom can recruit custom troops in their fiefs."), true, 0, 1000, false, null, false, false);
			Config.VassalLordsRecruitCustomTroopsAnywhere = Config.CreateOption<bool>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_vassal_lords_recruit_custom_troops_anywhere", "Vassal Lords Recruit Custom Troops Anywhere"), "VassalLordsRecruitCustomTroopsAnywhere", () => L.S("mcm_option_vassal_lords_recruit_custom_troops_anywhere_hint", "Lords of the player's clan or kingdom can recruit custom troops in any settlement."), false, 0, 1000, false, null, false, false);
			Config.AllLordsCanRecruitCustomTroops = Config.CreateOption<bool>(() => L.S("mcm_section_recruitment", "Recruitment"), () => L.S("mcm_option_all_lords_recruit_custom_troops", "All Lords Recruit Custom Troops"), "AllLordsCanRecruitCustomTroops", () => L.S("mcm_option_all_lords_recruit_custom_troops_hint", "Any lord can recruit custom troops in the player's fiefs."), true, 0, 1000, false, null, false, false);
			Config.EnableGlobalEditor = Config.CreateOption<bool>(() => L.S("mcm_section_global_editor", "Global Editor"), () => L.S("mcm_option_global_editor_enabled", "Enable Global Troop Editor"), "EnableGlobalEditor", () => L.S("mcm_option_global_editor_enabled_hint", "Enables the global troop editor to modify any troop in the game. Disable if you encounter issues with non-player troops or other mods."), true, 0, 1000, true, null, false, false);
			Config.VanillaUpgradeRequirements = Config.CreateOption<bool>(() => L.S("mcm_section_global_editor", "Global Editor"), () => L.S("mcm_option_vanilla_upgrade_requirements", "Vanilla Upgrade Requirements"), "VanillaUpgradeRequirements", () => L.S("mcm_option_vanilla_upgrade_requirements_hint", "Vanilla troops retain their original upgrade item requirements when edited."), true, 0, 1000, false, null, false, false);
			Func<string> section12 = () => L.S("mcm_section_restrictions", "Restrictions");
			Func<string> name12 = () => L.S("mcm_option_restrict_editing_to_fiefs", "Restrict Editing To Fiefs");
			string key12 = "RestrictEditingToFiefs";
			Func<string> hint12 = () => L.S("mcm_option_restrict_editing_to_fiefs_hint", "Player can only edit troops when in a fief owned by their clan or kingdom (retinues can be edited in any settlement).");
			bool default12 = false;
			int minValue12 = 0;
			int maxValue12 = 1000;
			bool requiresRestart12 = false;
			Dictionary<string, object> dictionary12 = new Dictionary<string, object>();
			dictionary12["freeform"] = false;
			dictionary12["realistic"] = true;
			Config.RestrictEditingToFiefs = Config.CreateOption<bool>(section12, name12, key12, hint12, default12, minValue12, maxValue12, requiresRestart12, dictionary12, false, false);
			Func<string> section13 = () => L.S("mcm_section_restrictions", "Restrictions");
			Func<string> name13 = () => L.S("mcm_option_max_elite_upgrades", "Max Elite Upgrades");
			string key13 = "MaxEliteUpgrades";
			Func<string> hint13 = () => L.S("mcm_option_max_elite_upgrades_hint", "Maximum number of upgrade paths each elite troop can have.");
			int default13 = 1;
			int minValue13 = 1;
			int maxValue13 = 4;
			bool requiresRestart13 = false;
			Dictionary<string, object> dictionary13 = new Dictionary<string, object>();
			dictionary13["freeform"] = 4;
			dictionary13["realistic"] = 1;
			Config.MaxEliteUpgrades = Config.CreateOption<int>(section13, name13, key13, hint13, default13, minValue13, maxValue13, requiresRestart13, dictionary13, false, 0);
			Func<string> section14 = () => L.S("mcm_section_restrictions", "Restrictions");
			Func<string> name14 = () => L.S("mcm_option_max_basic_upgrades", "Max Basic Upgrades");
			string key14 = "MaxBasicUpgrades";
			Func<string> hint14 = () => L.S("mcm_option_max_basic_upgrades_hint", "Maximum number of upgrade paths each basic troop can have.");
			int default14 = 2;
			int minValue14 = 1;
			int maxValue14 = 4;
			bool requiresRestart14 = false;
			Dictionary<string, object> dictionary14 = new Dictionary<string, object>();
			dictionary14["freeform"] = 4;
			dictionary14["realistic"] = 2;
			Config.MaxBasicUpgrades = Config.CreateOption<int>(section14, name14, key14, hint14, default14, minValue14, maxValue14, requiresRestart14, dictionary14, false, 0);
			Config.EnableDoctrines = Config.CreateOption<bool>(() => L.S("mcm_section_doctrines", "Doctrines"), () => L.S("mcm_option_enable_doctrines", "Enable Doctrines"), "EnableDoctrines", () => L.S("mcm_option_enable_doctrines_hint", "Enables the Doctrines system and its bonuses. Warning: saving with doctrines disabled will clear all doctrine data in the save."), true, 0, 1000, true, null, false, false);
			Config.EnableFeatRequirements = Config.CreateOption<bool>(() => L.S("mcm_section_doctrines", "Doctrines"), () => L.S("mcm_option_enable_feat_requirements", "Enable Feat Requirements"), "EnableFeatRequirements", () => L.S("mcm_option_enable_feat_requirements_hint", "Enables feat requirements for unlocking doctrines. Warning: saving with feats disabled will clear all feat data in the save."), true, 0, 1000, true, null, false, false);
			Func<string> section15 = () => L.S("mcm_section_doctrines", "Doctrines");
			Func<string> name15 = () => L.S("mcm_option_doctrine_gold_cost_multiplier", "Doctrine Gold Cost Multiplier");
			string key15 = "DoctrineGoldCostMultiplier";
			Func<string> hint15 = () => L.S("mcm_option_doctrine_gold_cost_multiplier_hint", "Multiplier for doctrine gold costs.");
			float default15 = 1f;
			int minValue15 = 0;
			int maxValue15 = 5;
			bool requiresRestart15 = true;
			Dictionary<string, object> dictionary15 = new Dictionary<string, object>();
			dictionary15["freeform"] = 0f;
			dictionary15["realistic"] = 1f;
			Config.DoctrineGoldCostMultiplier = Config.CreateOption<float>(section15, name15, key15, hint15, default15, minValue15, maxValue15, requiresRestart15, dictionary15, false, 0f);
			Func<string> section16 = () => L.S("mcm_section_doctrines", "Doctrines");
			Func<string> name16 = () => L.S("mcm_option_doctrine_influence_cost_multiplier", "Doctrine Influence Cost Multiplier");
			string key16 = "DoctrineInfluenceCostMultiplier";
			Func<string> hint16 = () => L.S("mcm_option_doctrine_influence_cost_multiplier_hint", "Multiplier for doctrine influence costs.");
			float default16 = 1f;
			int minValue16 = 0;
			int maxValue16 = 5;
			bool requiresRestart16 = true;
			Dictionary<string, object> dictionary16 = new Dictionary<string, object>();
			dictionary16["freeform"] = 0f;
			dictionary16["realistic"] = 1f;
			Config.DoctrineInfluenceCostMultiplier = Config.CreateOption<float>(section16, name16, key16, hint16, default16, minValue16, maxValue16, requiresRestart16, dictionary16, false, 0f);
			Func<string> section17 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name17 = () => L.S("mcm_option_equipping_troops_costs_gold", "Equipping Troops Costs Gold");
			string key17 = "EquippingTroopsCostsGold";
			Func<string> hint17 = () => L.S("mcm_option_equipping_troops_costs_gold_hint", "Upgrading troop equipment costs money.");
			bool default17 = true;
			int minValue17 = 0;
			int maxValue17 = 1000;
			bool requiresRestart17 = false;
			Dictionary<string, object> dictionary17 = new Dictionary<string, object>();
			dictionary17["freeform"] = false;
			dictionary17["realistic"] = true;
			Config.EquippingTroopsCostsGold = Config.CreateOption<bool>(section17, name17, key17, hint17, default17, minValue17, maxValue17, requiresRestart17, dictionary17, false, false);
			Func<string> section18 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name18 = () => L.S("mcm_option_equipment_cost_multiplier", "Equipment Cost Multiplier");
			string key18 = "EquipmentCostMultiplier";
			Func<string> hint18 = () => L.S("mcm_option_equipment_cost_multiplier_hint", "Multiplier for equipment prices compared to base game prices.");
			float default18 = 2f;
			int minValue18 = 0;
			int maxValue18 = 5;
			bool requiresRestart18 = false;
			Dictionary<string, object> dictionary18 = new Dictionary<string, object>();
			dictionary18["freeform"] = 0f;
			dictionary18["realistic"] = 4f;
			Config.EquipmentCostMultiplier = Config.CreateOption<float>(section18, name18, key18, hint18, default18, minValue18, maxValue18, requiresRestart18, dictionary18, false, 0f);
			Config.EquipmentCostReductionPerPurchase = Config.CreateOption<float>(() => L.S("mcm_section_equipment", "Equipment"), () => L.S("mcm_option_equipment_cost_reduction_per_purchase", "Equipment Cost Reduction Per Purchase"), "EquipmentCostReductionPerPurchase", () => L.S("mcm_option_equipment_cost_reduction_per_purchase_hint", "Each time a troop purchases an item, the cost for future purchases of that item is reduced by this proportion (0 = no reduction, 1 = free after first purchase)."), 0.2f, 0, 1, false, null, false, 0f);
			Func<string> section19 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name19 = () => L.S("mcm_option_equipping_troops_takes_time", "Equipping Troops Takes Time");
			string key19 = "EquippingTroopsTakesTime";
			Func<string> hint19 = () => L.S("mcm_option_equipping_troops_takes_time_hint", "To apply item changes, troops must spend time upgrading equipment in a fief.");
			bool default19 = false;
			int minValue19 = 0;
			int maxValue19 = 1000;
			bool requiresRestart19 = false;
			Dictionary<string, object> dictionary19 = new Dictionary<string, object>();
			dictionary19["freeform"] = false;
			dictionary19["realistic"] = true;
			Config.EquippingTroopsTakesTime = Config.CreateOption<bool>(section19, name19, key19, hint19, default19, minValue19, maxValue19, requiresRestart19, dictionary19, false, false);
			Func<string> section20 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name20 = () => L.S("mcm_option_equipment_time_multiplier", "Equipment Time Multiplier");
			string key20 = "EquipmentTimeMultiplier";
			Func<string> hint20 = () => L.S("mcm_option_equipment_time_multiplier_hint", "Multiplier for equipment change time.");
			int default20 = 2;
			int minValue20 = 1;
			int maxValue20 = 5;
			bool requiresRestart20 = false;
			Dictionary<string, object> dictionary20 = new Dictionary<string, object>();
			dictionary20["freeform"] = 2;
			dictionary20["realistic"] = 4;
			Config.EquipmentTimeMultiplier = Config.CreateOption<int>(section20, name20, key20, hint20, default20, minValue20, maxValue20, requiresRestart20, dictionary20, false, 0);
			Func<string> section21 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name21 = () => L.S("mcm_option_restrict_items_to_town_inventory", "Restrict Items To Town Inventory");
			string key21 = "RestrictItemsToTownInventory";
			Func<string> hint21 = () => L.S("mcm_option_restrict_items_to_town_inventory_hint", "Troops equipment can only be purchased if available in the current town inventory.");
			bool default21 = false;
			int minValue21 = 0;
			int maxValue21 = 1000;
			bool requiresRestart21 = false;
			Dictionary<string, object> dictionary21 = new Dictionary<string, object>();
			dictionary21["realistic"] = true;
			Config.RestrictItemsToTownInventory = Config.CreateOption<bool>(section21, name21, key21, hint21, default21, minValue21, maxValue21, requiresRestart21, dictionary21, false, false);
			Func<string> section22 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name22 = () => L.S("mcm_option_allowed_tier_difference", "Allowed Tier Difference");
			string key22 = "AllowedTierDifference";
			Func<string> hint22 = () => L.S("mcm_option_allowed_tier_difference_hint", "Maximum allowed difference between troop tier and item tier.");
			int default22 = 3;
			int minValue22 = 0;
			int maxValue22 = 6;
			bool requiresRestart22 = false;
			Dictionary<string, object> dictionary22 = new Dictionary<string, object>();
			dictionary22["freeform"] = 6;
			dictionary22["realistic"] = 2;
			Config.AllowedTierDifference = Config.CreateOption<int>(section22, name22, key22, hint22, default22, minValue22, maxValue22, requiresRestart22, dictionary22, false, 0);
			Func<string> section23 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name23 = () => L.S("mcm_option_disallow_mounts_for_t1_troops", "Disallow Mounts For T1 Troops");
			string key23 = "DisallowMountsForT1Troops";
			Func<string> hint23 = () => L.S("mcm_option_disallow_mounts_for_t1_troops_hint", "Tier 1 troops cannot have mounts.");
			bool default23 = true;
			int minValue23 = 0;
			int maxValue23 = 1000;
			bool requiresRestart23 = false;
			Dictionary<string, object> dictionary23 = new Dictionary<string, object>();
			dictionary23["freeform"] = false;
			dictionary23["realistic"] = true;
			Config.DisallowMountsForT1Troops = Config.CreateOption<bool>(section23, name23, key23, hint23, default23, minValue23, maxValue23, requiresRestart23, dictionary23, false, false);
			Config.ForceMainBattleSetInCombat = Config.CreateOption<bool>(() => L.S("mcm_section_equipment", "Equipment"), () => L.S("mcm_option_force_main_battle_set_in_combat", "Force Main Battle Set In Combat"), "ForceMainBattleSetInCombat", () => L.S("mcm_option_force_main_battle_set_in_combat_hint", "Troops always use their main battle equipment set in combat, ignoring alternate sets. Use this setting if you experience issues with troops using incorrect equipment sets in battle."), false, 0, 1000, false, null, false, false);
			Config.AllowFormationOverrides = Config.CreateOption<bool>(() => L.S("mcm_section_equipment", "Equipment"), () => L.S("mcm_option_allow_formation_overrides", "Allow Formation Overrides"), "AllowFormationOverrides", () => L.S("mcm_option_allow_formation_overrides_hint", "Allow manual overriding of troop formation class. If enabled, may cause awkward AI behavior and slow down the pre-battle formation screen."), false, 0, 1000, false, null, false, false);
			Config.AdditionalFormationOverrides = Config.CreateOption<bool>(() => L.S("mcm_section_equipment", "Equipment"), () => L.S("mcm_option_additional_formation_overrides", "Additional Formation Overrides"), "AdditionalFormationOverrides", () => L.S("mcm_option_additional_formation_overrides_hint", "Adds special formation classes (skirmisher, bodyguard, etc.) to the list of selectable formation overrides. Requires 'Allow Formation Overrides' to be enabled to have an effect."), false, 0, 1000, false, null, true, false);
			Func<string> section24 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name24 = () => L.S("mcm_option_no_civilian_set_upgrade_requirements", "No Civilian Set Upgrade Requirements");
			string key24 = "NoCivilianSetUpgradeRequirements";
			Func<string> hint24 = () => L.S("mcm_option_no_civilian_set_upgrade_requirements_hint", "When checking mount requirements for upgrades, ignore any horse in civilian sets.");
			bool default24 = true;
			int minValue24 = 0;
			int maxValue24 = 1000;
			bool requiresRestart24 = false;
			Dictionary<string, object> dictionary24 = new Dictionary<string, object>();
			dictionary24["freeform"] = true;
			dictionary24["realistic"] = false;
			Config.NoCivilianSetUpgradeRequirements = Config.CreateOption<bool>(section24, name24, key24, hint24, default24, minValue24, maxValue24, requiresRestart24, dictionary24, false, false);
			Func<string> section25 = () => L.S("mcm_section_equipment", "Equipment");
			Func<string> name25 = () => L.S("mcm_option_no_noble_horse_upgrade_requirements", "No Noble Horse Upgrade Requirements");
			string key25 = "NoNobleHorseUpgradeRequirements";
			Func<string> hint25 = () => L.S("mcm_option_no_noble_horse_upgrade_requirements_hint", "Troops never require noble horses for upgrades, a war horse is always enough.");
			bool default25 = true;
			int minValue25 = 0;
			int maxValue25 = 1000;
			bool requiresRestart25 = false;
			Dictionary<string, object> dictionary25 = new Dictionary<string, object>();
			dictionary25["freeform"] = true;
			dictionary25["realistic"] = false;
			Config.NoNobleHorseUpgradeRequirements = Config.CreateOption<bool>(section25, name25, key25, hint25, default25, minValue25, maxValue25, requiresRestart25, dictionary25, false, false);
			Func<string> section26 = () => L.S("mcm_section_skills", "Skills");
			Func<string> name26 = () => L.S("mcm_option_training_troops_takes_time", "Training Troops Takes Time");
			string key26 = "TrainingTroopsTakesTime";
			Func<string> hint26 = () => L.S("mcm_option_training_troops_takes_time_hint", "To apply skill increases, troops must spend time training in a fief.");
			bool default26 = false;
			int minValue26 = 0;
			int maxValue26 = 1000;
			bool requiresRestart26 = false;
			Dictionary<string, object> dictionary26 = new Dictionary<string, object>();
			dictionary26["freeform"] = false;
			dictionary26["realistic"] = true;
			Config.TrainingTroopsTakesTime = Config.CreateOption<bool>(section26, name26, key26, hint26, default26, minValue26, maxValue26, requiresRestart26, dictionary26, false, false);
			Func<string> section27 = () => L.S("mcm_section_skills", "Skills");
			Func<string> name27 = () => L.S("mcm_option_training_time_multiplier", "Training Time Multiplier");
			string key27 = "TrainingTimeMultiplier";
			Func<string> hint27 = () => L.S("mcm_option_training_time_multiplier_hint", "Multiplier for troop training time.");
			int default27 = 2;
			int minValue27 = 1;
			int maxValue27 = 5;
			bool requiresRestart27 = false;
			Dictionary<string, object> dictionary27 = new Dictionary<string, object>();
			dictionary27["freeform"] = 2;
			dictionary27["realistic"] = 3;
			Config.TrainingTimeMultiplier = Config.CreateOption<int>(section27, name27, key27, hint27, default27, minValue27, maxValue27, requiresRestart27, dictionary27, false, 0);
			Func<string> section28 = () => L.S("mcm_section_skills", "Skills");
			Func<string> name28 = () => L.S("mcm_option_skill_xp_cost_base", "Skill XP Cost (Base)");
			string key28 = "BaseSkillXpCost";
			Func<string> hint28 = () => L.S("mcm_option_skill_xp_cost_base_hint", "Base XP cost for increasing a skill.");
			int default28 = 100;
			int minValue28 = 0;
			int maxValue28 = 1000;
			bool requiresRestart28 = false;
			Dictionary<string, object> dictionary28 = new Dictionary<string, object>();
			dictionary28["freeform"] = 0;
			dictionary28["realistic"] = 200;
			Config.BaseSkillXpCost = Config.CreateOption<int>(section28, name28, key28, hint28, default28, minValue28, maxValue28, requiresRestart28, dictionary28, false, 0);
			Func<string> section29 = () => L.S("mcm_section_skills", "Skills");
			Func<string> name29 = () => L.S("mcm_option_skill_xp_cost_per_point", "Skill XP Cost (Per Point)");
			string key29 = "SkillXpCostPerPoint";
			Func<string> hint29 = () => L.S("mcm_option_skill_xp_cost_per_point_hint", "Scalable XP cost for each additional skill point.");
			int default29 = 1;
			int minValue29 = 0;
			int maxValue29 = 10;
			bool requiresRestart29 = false;
			Dictionary<string, object> dictionary29 = new Dictionary<string, object>();
			dictionary29["freeform"] = 0;
			dictionary29["realistic"] = 2;
			Config.SkillXpCostPerPoint = Config.CreateOption<int>(section29, name29, key29, hint29, default29, minValue29, maxValue29, requiresRestart29, dictionary29, false, 0);
			Config.SharedXpPool = Config.CreateOption<bool>(() => L.S("mcm_section_skills", "Skills"), () => L.S("mcm_option_shared_xp_pool", "Shared XP Pool"), "SharedXpPool", () => L.S("mcm_option_shared_xp_pool_hint", "All edited troops share a single XP pool instead of having individual XP."), false, 0, 1000, false, null, false, false);
			Config.ForceXpRefunds = Config.CreateOption<bool>(() => L.S("mcm_section_skills", "Skills"), () => L.S("mcm_option_force_xp_refunds", "Force XP Refunds"), "ForceXpRefunds", () => L.S("mcm_option_force_xp_refunds_hint", "When lowering a troop's skill, always refund the XP previously spent on those points."), false, 0, 1000, false, null, false, false);
			Func<string> section30 = () => L.S("mcm_section_skills", "Skills");
			Func<string> name30 = () => L.S("mcm_option_cannot_raise_skill_above_upgrade_level", "Cannot Raise Skill Above Upgrade's Level");
			string key30 = "CannotRaiseSkillAboveUpgradeLevel";
			Func<string> hint30 = () => L.S("mcm_option_cannot_raise_skill_above_upgrade_level_hint", "Troop skills cannot be raised above the skill level of an upgrade target.");
			bool default30 = true;
			int minValue30 = 0;
			int maxValue30 = 1000;
			bool requiresRestart30 = false;
			Dictionary<string, object> dictionary30 = new Dictionary<string, object>();
			dictionary30["freeform"] = false;
			dictionary30["realistic"] = true;
			Config.CannotRaiseSkillAboveUpgradeLevel = Config.CreateOption<bool>(section30, name30, key30, hint30, default30, minValue30, maxValue30, requiresRestart30, dictionary30, false, false);
			Func<string> section31 = () => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks");
			Func<string> name31 = () => L.S("mcm_option_all_equipment_unlocked", "All Equipment Unlocked");
			string key31 = "AllEquipmentUnlocked";
			Func<string> hint31 = () => L.S("mcm_option_all_equipment_unlocked_hint", "All items are always available.");
			bool default31 = false;
			int minValue31 = 0;
			int maxValue31 = 1000;
			bool requiresRestart31 = false;
			Dictionary<string, object> dictionary31 = new Dictionary<string, object>();
			dictionary31["freeform"] = true;
			dictionary31["realistic"] = false;
			Config.AllEquipmentUnlocked = Config.CreateOption<bool>(section31, name31, key31, hint31, default31, minValue31, maxValue31, requiresRestart31, dictionary31, false, false);
			Config.AllCultureEquipmentUnlocked = Config.CreateOption<bool>(() => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks"), () => L.S("mcm_option_all_culture_equipment_unlocked", "All Culture Equipment Unlocked"), "AllCultureEquipmentUnlocked", () => L.S("mcm_option_all_culture_equipment_unlocked_hint", "Player clan and kingdom culture items are always available."), false, 0, 1000, false, null, false, false);
			Func<string> section32 = () => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks");
			Func<string> name32 = () => L.S("mcm_option_unlock_items_from_kills", "Unlock Items From Kills");
			string key32 = "UnlockItemsFromKills";
			Func<string> hint32 = () => L.S("mcm_option_unlock_items_from_kills_hint", "Unlock equipment by defeating enemies wearing it.");
			bool default32 = true;
			int minValue32 = 0;
			int maxValue32 = 1000;
			bool requiresRestart32 = false;
			Dictionary<string, object> dictionary32 = new Dictionary<string, object>();
			dictionary32["freeform"] = false;
			dictionary32["realistic"] = true;
			Config.UnlockItemsFromKills = Config.CreateOption<bool>(section32, name32, key32, hint32, default32, minValue32, maxValue32, requiresRestart32, dictionary32, false, false);
			Func<string> section33 = () => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks");
			Func<string> name33 = () => L.S("mcm_option_required_kills_per_item", "Required Kills Per Item");
			string key33 = "RequiredKillsPerItem";
			Func<string> hint33 = () => L.S("mcm_option_required_kills_per_item_hint", "How many enemies wearing an item must be defeated to unlock it.");
			int default33 = 100;
			int minValue33 = 1;
			int maxValue33 = 1000;
			bool requiresRestart33 = false;
			Dictionary<string, object> dictionary33 = new Dictionary<string, object>();
			dictionary33["freeform"] = 100;
			dictionary33["realistic"] = 200;
			Config.RequiredKillsPerItem = Config.CreateOption<int>(section33, name33, key33, hint33, default33, minValue33, maxValue33, requiresRestart33, dictionary33, false, 0);
			Config.UnlockItemsFromDiscards = Config.CreateOption<bool>(() => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks"), () => L.S("mcm_option_unlock_items_from_discards", "Unlock Items From Discards"), "UnlockItemsFromDiscards", () => L.S("mcm_option_unlock_items_from_discards_hint", "Unlock equipment by discarding it."), false, 0, 1000, false, null, false, false);
			Func<string> section34 = () => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks");
			Func<string> name34 = () => L.S("mcm_option_required_discards_per_item", "Required Discards Per Item");
			string key34 = "DiscardsForUnlock";
			Func<string> hint34 = () => L.S("mcm_option_required_discards_per_item_hint", "How many times an item must be discarded to unlock it.");
			int default34 = 10;
			int minValue34 = 1;
			int maxValue34 = 100;
			bool requiresRestart34 = false;
			Dictionary<string, object> dictionary34 = new Dictionary<string, object>();
			dictionary34["freeform"] = 10;
			dictionary34["realistic"] = 20;
			Config.RequiredDiscardsPerItem = Config.CreateOption<int>(section34, name34, key34, hint34, default34, minValue34, maxValue34, requiresRestart34, dictionary34, false, 0);
			Func<string> section35 = () => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks");
			Func<string> name35 = () => L.S("mcm_option_player_culture_unlock_bonus", "Player Culture Unlock Bonus");
			string key35 = "PlayerCultureUnlockBonus";
			Func<string> hint35 = () => L.S("mcm_option_player_culture_unlock_bonus_hint", "Whether item unlock progression also adds progress to random items of the player culture.");
			bool default35 = true;
			int minValue35 = 0;
			int maxValue35 = 1000;
			bool requiresRestart35 = false;
			Dictionary<string, object> dictionary35 = new Dictionary<string, object>();
			dictionary35["freeform"] = true;
			dictionary35["realistic"] = false;
			Config.PlayerCultureUnlockBonus = Config.CreateOption<bool>(section35, name35, key35, hint35, default35, minValue35, maxValue35, requiresRestart35, dictionary35, false, false);
			Config.UnlockPopup = Config.CreateOption<bool>(() => L.S("mcm_section_equipment_unlocks", "Equipment Unlocks"), () => L.S("mcm_option_unlock_popup", "Unlock Popup"), "UnlockPopup", () => L.S("mcm_option_unlock_popup_hint", "Displays a popup notification when items are unlocked. If disabled, unlocks are only shown in the log."), true, 0, 1000, false, null, false, false);
			Config.DebugMode = Config.CreateOption<bool>(() => L.S("mcm_section_debug", "Debug"), () => L.S("mcm_option_debug_mode", "Debug Mode"), "DebugMode", () => L.S("mcm_option_debug_mode_hint", "Displays debug logs in game."), false, 0, 1000, false, null, false, false);
			Config.EnableEditorHotkey = Config.CreateOption<bool>(() => L.S("mcm_section_ui", "User Interface"), () => L.S("mcm_option_enable_editor_hotkey", "Enable Editor Hotkey (Shift + R)"), "EnableEditorHotkey", () => L.S("mcm_option_enable_editor_hotkey_hint", "Enables the hotkey (Shift + R) to open the editor from the campaign map."), false, 0, 1000, false, null, false, false);
			Config.EnableItemComparisonIcons = Config.CreateOption<bool>(() => L.S("mcm_section_ui", "User Interface"), () => L.S("mcm_option_enable_item_comparison_icons", "Enable Item Comparison Icons"), "EnableItemComparisonIcons", () => L.S("mcm_option_enable_item_comparison_icons_hint", "Adds comparison icons to the equipment list to show if an item is better or worse than the equipped one when browsing troop equipment."), true, 0, 1000, false, null, false, false);
			Config.EnableTroopCustomization = Config.CreateOption<bool>(() => L.S("mcm_section_ui", "User Interface"), () => L.S("mcm_option_enable_troop_customization", "Enable Appearance Controls"), "EnableTroopCustomization", () => L.S("mcm_option_enable_troop_customization_hint", "Adds appearance customization controls (age, height, weight, build) to the editor. Cosmetic only; no gameplay effect."), true, 0, 1000, false, null, false, false);
			Config.MaxEquipmentRowsPerPage = Config.CreateOption<int>(() => L.S("mcm_section_ui", "User Interface"), () => L.S("mcm_option_max_equipment_rows_per_page", "Max Equipment Rows Per Page"), "MaxEquipmentRowsPerPage", () => L.S("mcm_option_max_equipment_rows_per_page_hint", "Maximum number of equipment rows to show per page in the troop editor. Smaller values improve UI reactivity."), 100, 10, 1000, false, null, false, 0);
			Func<string> section36 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name36 = () => L.S("mcm_option_retinue_skill_cap_bonus", "Retinue Skill Cap Bonus");
			string key36 = "RetinueSkillCapBonus";
			Func<string> hint36 = () => L.S("mcm_option_retinue_skill_cap_bonus_hint", "Additional skill cap for retinue troops.");
			int default36 = 5;
			int minValue36 = 0;
			int maxValue36 = 50;
			bool requiresRestart36 = false;
			Dictionary<string, object> dictionary36 = new Dictionary<string, object>();
			dictionary36["freeform"] = 50;
			dictionary36["realistic"] = 0;
			Config.RetinueSkillCapBonus = Config.CreateOption<int>(section36, name36, key36, hint36, default36, minValue36, maxValue36, requiresRestart36, dictionary36, false, 0);
			Func<string> section37 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name37 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "0").ToString();
			string key37 = "SkillCapTier0";
			Func<string> hint37 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "0").ToString();
			int default37 = 20;
			int minValue37 = 20;
			int maxValue37 = 360;
			bool requiresRestart37 = false;
			Dictionary<string, object> dictionary37 = new Dictionary<string, object>();
			dictionary37["freeform"] = 360;
			Config.SkillCapTier0 = Config.CreateOption<int>(section37, name37, key37, hint37, default37, minValue37, maxValue37, requiresRestart37, dictionary37, false, 0);
			Func<string> section38 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name38 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "1").ToString();
			string key38 = "SkillCapTier1";
			Func<string> hint38 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "1").ToString();
			int default38 = 20;
			int minValue38 = 20;
			int maxValue38 = 360;
			bool requiresRestart38 = false;
			Dictionary<string, object> dictionary38 = new Dictionary<string, object>();
			dictionary38["freeform"] = 360;
			Config.SkillCapTier1 = Config.CreateOption<int>(section38, name38, key38, hint38, default38, minValue38, maxValue38, requiresRestart38, dictionary38, false, 0);
			Func<string> section39 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name39 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "2").ToString();
			string key39 = "SkillCapTier2";
			Func<string> hint39 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "2").ToString();
			int default39 = 50;
			int minValue39 = 20;
			int maxValue39 = 360;
			bool requiresRestart39 = false;
			Dictionary<string, object> dictionary39 = new Dictionary<string, object>();
			dictionary39["freeform"] = 360;
			Config.SkillCapTier2 = Config.CreateOption<int>(section39, name39, key39, hint39, default39, minValue39, maxValue39, requiresRestart39, dictionary39, false, 0);
			Func<string> section40 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name40 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "3").ToString();
			string key40 = "SkillCapTier3";
			Func<string> hint40 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "3").ToString();
			int default40 = 80;
			int minValue40 = 20;
			int maxValue40 = 360;
			bool requiresRestart40 = false;
			Dictionary<string, object> dictionary40 = new Dictionary<string, object>();
			dictionary40["freeform"] = 360;
			Config.SkillCapTier3 = Config.CreateOption<int>(section40, name40, key40, hint40, default40, minValue40, maxValue40, requiresRestart40, dictionary40, false, 0);
			Func<string> section41 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name41 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "4").ToString();
			string key41 = "SkillCapTier4";
			Func<string> hint41 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "4").ToString();
			int default41 = 120;
			int minValue41 = 20;
			int maxValue41 = 360;
			bool requiresRestart41 = false;
			Dictionary<string, object> dictionary41 = new Dictionary<string, object>();
			dictionary41["freeform"] = 360;
			Config.SkillCapTier4 = Config.CreateOption<int>(section41, name41, key41, hint41, default41, minValue41, maxValue41, requiresRestart41, dictionary41, false, 0);
			Func<string> section42 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name42 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "5").ToString();
			string key42 = "SkillCapTier5";
			Func<string> hint42 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "5").ToString();
			int default42 = 160;
			int minValue42 = 20;
			int maxValue42 = 360;
			bool requiresRestart42 = false;
			Dictionary<string, object> dictionary42 = new Dictionary<string, object>();
			dictionary42["freeform"] = 360;
			Config.SkillCapTier5 = Config.CreateOption<int>(section42, name42, key42, hint42, default42, minValue42, maxValue42, requiresRestart42, dictionary42, false, 0);
			Func<string> section43 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name43 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "6").ToString();
			string key43 = "SkillCapTier6";
			Func<string> hint43 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "6").ToString();
			int default43 = 260;
			int minValue43 = 20;
			int maxValue43 = 360;
			bool requiresRestart43 = false;
			Dictionary<string, object> dictionary43 = new Dictionary<string, object>();
			dictionary43["freeform"] = 360;
			Config.SkillCapTier6 = Config.CreateOption<int>(section43, name43, key43, hint43, default43, minValue43, maxValue43, requiresRestart43, dictionary43, false, 0);
			Func<string> section44 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name44 = () => L.T("mcm_option_skill_cap", "Tier {TIER} Cap").SetTextVariable("TIER", "7+").ToString();
			string key44 = "SkillCapTier7Plus";
			Func<string> hint44 = () => L.T("mcm_option_skill_cap_hint", "The maximum skill level for tier {TIER} troops.").SetTextVariable("TIER", "7+").ToString();
			int default44 = 360;
			int minValue44 = 20;
			int maxValue44 = 360;
			bool requiresRestart44 = false;
			Dictionary<string, object> dictionary44 = new Dictionary<string, object>();
			dictionary44["freeform"] = 360;
			Config.SkillCapTier7Plus = Config.CreateOption<int>(section44, name44, key44, hint44, default44, minValue44, maxValue44, requiresRestart44, dictionary44, false, 0);
			Func<string> section45 = () => L.S("mcm_section_skill_caps", "Skill Caps");
			Func<string> name45 = () => L.S("mcm_option_skill_cap_heroes", "Hero Skill Cap");
			string key45 = "SkillCapHeroes";
			Func<string> hint45 = () => L.S("mcm_option_skill_cap_heroes_hint", "The maximum skill level for hero troops.");
			int default45 = 420;
			int minValue45 = 20;
			int maxValue45 = 420;
			bool requiresRestart45 = false;
			Dictionary<string, object> dictionary45 = new Dictionary<string, object>();
			dictionary45["freeform"] = 420;
			Config.SkillCapHeroes = Config.CreateOption<int>(section45, name45, key45, hint45, default45, minValue45, maxValue45, requiresRestart45, dictionary45, false, 0);
			Func<string> section46 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name46 = () => L.S("mcm_option_retinue_skill_total_bonus", "Retinue Skill Total Bonus");
			string key46 = "RetinueSkillTotalBonus";
			Func<string> hint46 = () => L.S("mcm_option_retinue_skill_total_bonus_hint", "Additional skill total for retinue troops.");
			int default46 = 10;
			int minValue46 = 0;
			int maxValue46 = 100;
			bool requiresRestart46 = false;
			Dictionary<string, object> dictionary46 = new Dictionary<string, object>();
			dictionary46["freeform"] = 100;
			dictionary46["realistic"] = 0;
			Config.RetinueSkillTotalBonus = Config.CreateOption<int>(section46, name46, key46, hint46, default46, minValue46, maxValue46, requiresRestart46, dictionary46, false, 0);
			Func<string> section47 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name47 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "0").ToString();
			string key47 = "SkillTotalTier0";
			Func<string> hint47 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "0").ToString();
			int default47 = 90;
			int minValue47 = 90;
			int maxValue47 = 1600;
			bool requiresRestart47 = false;
			Dictionary<string, object> dictionary47 = new Dictionary<string, object>();
			dictionary47["freeform"] = 1600;
			Config.SkillTotalTier0 = Config.CreateOption<int>(section47, name47, key47, hint47, default47, minValue47, maxValue47, requiresRestart47, dictionary47, false, 0);
			Func<string> section48 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name48 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "1").ToString();
			string key48 = "SkillTotalTier1";
			Func<string> hint48 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "1").ToString();
			int default48 = 90;
			int minValue48 = 90;
			int maxValue48 = 1600;
			bool requiresRestart48 = false;
			Dictionary<string, object> dictionary48 = new Dictionary<string, object>();
			dictionary48["freeform"] = 1600;
			Config.SkillTotalTier1 = Config.CreateOption<int>(section48, name48, key48, hint48, default48, minValue48, maxValue48, requiresRestart48, dictionary48, false, 0);
			Func<string> section49 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name49 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "2").ToString();
			string key49 = "SkillTotalTier2";
			Func<string> hint49 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "2").ToString();
			int default49 = 210;
			int minValue49 = 90;
			int maxValue49 = 1600;
			bool requiresRestart49 = false;
			Dictionary<string, object> dictionary49 = new Dictionary<string, object>();
			dictionary49["freeform"] = 1600;
			Config.SkillTotalTier2 = Config.CreateOption<int>(section49, name49, key49, hint49, default49, minValue49, maxValue49, requiresRestart49, dictionary49, false, 0);
			Func<string> section50 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name50 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "3").ToString();
			string key50 = "SkillTotalTier3";
			Func<string> hint50 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "3").ToString();
			int default50 = 360;
			int minValue50 = 90;
			int maxValue50 = 1600;
			bool requiresRestart50 = false;
			Dictionary<string, object> dictionary50 = new Dictionary<string, object>();
			dictionary50["freeform"] = 1600;
			Config.SkillTotalTier3 = Config.CreateOption<int>(section50, name50, key50, hint50, default50, minValue50, maxValue50, requiresRestart50, dictionary50, false, 0);
			Func<string> section51 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name51 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "4").ToString();
			string key51 = "SkillTotalTier4";
			Func<string> hint51 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "4").ToString();
			int default51 = 555;
			int minValue51 = 90;
			int maxValue51 = 1600;
			bool requiresRestart51 = false;
			Dictionary<string, object> dictionary51 = new Dictionary<string, object>();
			dictionary51["freeform"] = 1600;
			Config.SkillTotalTier4 = Config.CreateOption<int>(section51, name51, key51, hint51, default51, minValue51, maxValue51, requiresRestart51, dictionary51, false, 0);
			Func<string> section52 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name52 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "5").ToString();
			string key52 = "SkillTotalTier5";
			Func<string> hint52 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "5").ToString();
			int default52 = 780;
			int minValue52 = 90;
			int maxValue52 = 1600;
			bool requiresRestart52 = false;
			Dictionary<string, object> dictionary52 = new Dictionary<string, object>();
			dictionary52["freeform"] = 1600;
			Config.SkillTotalTier5 = Config.CreateOption<int>(section52, name52, key52, hint52, default52, minValue52, maxValue52, requiresRestart52, dictionary52, false, 0);
			Func<string> section53 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name53 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "6").ToString();
			string key53 = "SkillTotalTier6";
			Func<string> hint53 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "6").ToString();
			int default53 = 1015;
			int minValue53 = 90;
			int maxValue53 = 1600;
			bool requiresRestart53 = false;
			Dictionary<string, object> dictionary53 = new Dictionary<string, object>();
			dictionary53["freeform"] = 1600;
			Config.SkillTotalTier6 = Config.CreateOption<int>(section53, name53, key53, hint53, default53, minValue53, maxValue53, requiresRestart53, dictionary53, false, 0);
			Func<string> section54 = () => L.S("mcm_section_skill_totals", "Skill Totals");
			Func<string> name54 = () => L.T("mcm_option_skill_total", "Tier {TIER} Skill Total").SetTextVariable("TIER", "7+").ToString();
			string key54 = "SkillTotalTier7Plus";
			Func<string> hint54 = () => L.T("mcm_option_skill_total_hint", "The total available skill points for tier {TIER} troops.").SetTextVariable("TIER", "7+").ToString();
			int default54 = 1600;
			int minValue54 = 90;
			int maxValue54 = 1600;
			bool requiresRestart54 = false;
			Dictionary<string, object> dictionary54 = new Dictionary<string, object>();
			dictionary54["freeform"] = 1600;
			Config.SkillTotalTier7Plus = Config.CreateOption<int>(section54, name54, key54, hint54, default54, minValue54, maxValue54, requiresRestart54, dictionary54, false, 0);
			Config._all = new List<IOption>();
			Config._byKey = new Dictionary<string, IOption>(StringComparer.OrdinalIgnoreCase);
			Config._values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			Config._ordinalByKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Config._exportName = Config.SuggestDefaultExportName();
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0002FF20 File Offset: 0x0002E120
		[CompilerGenerated]
		internal static void <BuildMcmMenu>g__ApplyPreset|103_4(ISettingsPresetBuilder p, IReadOnlyDictionary<string, object> map)
		{
			foreach (KeyValuePair<string, object> keyValuePair in map)
			{
				p.SetPropertyValue(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x0400025F RID: 607
		public static readonly Option<float> MaxEliteRetinueRatio;

		// Token: 0x04000260 RID: 608
		public static readonly Option<float> MaxBasicRetinueRatio;

		// Token: 0x04000261 RID: 609
		public static readonly Option<int> RankUpCostPerTier;

		// Token: 0x04000262 RID: 610
		public static readonly Option<int> GoldConversionCostPerTier;

		// Token: 0x04000263 RID: 611
		public static readonly Option<int> InfluenceConversionCostPerTier;

		// Token: 0x04000264 RID: 612
		public static readonly Option<int> RenownRequiredPerTier;

		// Token: 0x04000265 RID: 613
		public static readonly Option<bool> NoFiefRequirements;

		// Token: 0x04000266 RID: 614
		public static readonly Option<bool> NoDoctrineRequirements;

		// Token: 0x04000267 RID: 615
		public static readonly Option<bool> DisableKingdomTroops;

		// Token: 0x04000268 RID: 616
		public static readonly Option<bool> CopyAllSetsOnUnlock;

		// Token: 0x04000269 RID: 617
		public static readonly Option<float> CustomVolunteersProportion;

		// Token: 0x0400026A RID: 618
		public static readonly Option<float> KingdomVolunteersInClanFiefsProportion;

		// Token: 0x0400026B RID: 619
		public static readonly Option<float> ClanVolunteersInKingdomFiefsProportion;

		// Token: 0x0400026C RID: 620
		public static readonly Option<bool> RestrictToOwnedSettlements;

		// Token: 0x0400026D RID: 621
		public static readonly Option<bool> RestrictToSameCultureSettlements;

		// Token: 0x0400026E RID: 622
		public static readonly Option<bool> VassalLordsCanRecruitCustomTroops;

		// Token: 0x0400026F RID: 623
		public static readonly Option<bool> VassalLordsRecruitCustomTroopsAnywhere;

		// Token: 0x04000270 RID: 624
		public static readonly Option<bool> AllLordsCanRecruitCustomTroops;

		// Token: 0x04000271 RID: 625
		public static readonly Option<bool> EnableGlobalEditor;

		// Token: 0x04000272 RID: 626
		public static readonly Option<bool> VanillaUpgradeRequirements;

		// Token: 0x04000273 RID: 627
		public static readonly Option<bool> RestrictEditingToFiefs;

		// Token: 0x04000274 RID: 628
		public static readonly Option<int> MaxEliteUpgrades;

		// Token: 0x04000275 RID: 629
		public static readonly Option<int> MaxBasicUpgrades;

		// Token: 0x04000276 RID: 630
		public static readonly Option<bool> EnableDoctrines;

		// Token: 0x04000277 RID: 631
		public static readonly Option<bool> EnableFeatRequirements;

		// Token: 0x04000278 RID: 632
		public static readonly Option<float> DoctrineGoldCostMultiplier;

		// Token: 0x04000279 RID: 633
		public static readonly Option<float> DoctrineInfluenceCostMultiplier;

		// Token: 0x0400027A RID: 634
		public static readonly Option<bool> EquippingTroopsCostsGold;

		// Token: 0x0400027B RID: 635
		public static readonly Option<float> EquipmentCostMultiplier;

		// Token: 0x0400027C RID: 636
		public static readonly Option<float> EquipmentCostReductionPerPurchase;

		// Token: 0x0400027D RID: 637
		public static readonly Option<bool> EquippingTroopsTakesTime;

		// Token: 0x0400027E RID: 638
		public static readonly Option<int> EquipmentTimeMultiplier;

		// Token: 0x0400027F RID: 639
		public static readonly Option<bool> RestrictItemsToTownInventory;

		// Token: 0x04000280 RID: 640
		public static readonly Option<int> AllowedTierDifference;

		// Token: 0x04000281 RID: 641
		public static readonly Option<bool> DisallowMountsForT1Troops;

		// Token: 0x04000282 RID: 642
		public static readonly Option<bool> ForceMainBattleSetInCombat;

		// Token: 0x04000283 RID: 643
		public static readonly Option<bool> AllowFormationOverrides;

		// Token: 0x04000284 RID: 644
		public static readonly Option<bool> AdditionalFormationOverrides;

		// Token: 0x04000285 RID: 645
		public static readonly Option<bool> NoCivilianSetUpgradeRequirements;

		// Token: 0x04000286 RID: 646
		public static readonly Option<bool> NoNobleHorseUpgradeRequirements;

		// Token: 0x04000287 RID: 647
		public static readonly Option<bool> TrainingTroopsTakesTime;

		// Token: 0x04000288 RID: 648
		public static readonly Option<int> TrainingTimeMultiplier;

		// Token: 0x04000289 RID: 649
		public static readonly Option<int> BaseSkillXpCost;

		// Token: 0x0400028A RID: 650
		public static readonly Option<int> SkillXpCostPerPoint;

		// Token: 0x0400028B RID: 651
		public static readonly Option<bool> SharedXpPool;

		// Token: 0x0400028C RID: 652
		public static readonly Option<bool> ForceXpRefunds;

		// Token: 0x0400028D RID: 653
		public static readonly Option<bool> CannotRaiseSkillAboveUpgradeLevel;

		// Token: 0x0400028E RID: 654
		public static readonly Option<bool> AllEquipmentUnlocked;

		// Token: 0x0400028F RID: 655
		public static readonly Option<bool> AllCultureEquipmentUnlocked;

		// Token: 0x04000290 RID: 656
		public static readonly Option<bool> UnlockItemsFromKills;

		// Token: 0x04000291 RID: 657
		public static readonly Option<int> RequiredKillsPerItem;

		// Token: 0x04000292 RID: 658
		public static readonly Option<bool> UnlockItemsFromDiscards;

		// Token: 0x04000293 RID: 659
		public static readonly Option<int> RequiredDiscardsPerItem;

		// Token: 0x04000294 RID: 660
		public static readonly Option<bool> PlayerCultureUnlockBonus;

		// Token: 0x04000295 RID: 661
		public static readonly Option<bool> UnlockPopup;

		// Token: 0x04000296 RID: 662
		public static readonly Option<bool> DebugMode;

		// Token: 0x04000297 RID: 663
		public static readonly Option<bool> EnableEditorHotkey;

		// Token: 0x04000298 RID: 664
		public static readonly Option<bool> EnableItemComparisonIcons;

		// Token: 0x04000299 RID: 665
		public static readonly Option<bool> EnableTroopCustomization;

		// Token: 0x0400029A RID: 666
		public static readonly Option<int> MaxEquipmentRowsPerPage;

		// Token: 0x0400029B RID: 667
		public static readonly Option<int> RetinueSkillCapBonus;

		// Token: 0x0400029C RID: 668
		public static readonly Option<int> SkillCapTier0;

		// Token: 0x0400029D RID: 669
		public static readonly Option<int> SkillCapTier1;

		// Token: 0x0400029E RID: 670
		public static readonly Option<int> SkillCapTier2;

		// Token: 0x0400029F RID: 671
		public static readonly Option<int> SkillCapTier3;

		// Token: 0x040002A0 RID: 672
		public static readonly Option<int> SkillCapTier4;

		// Token: 0x040002A1 RID: 673
		public static readonly Option<int> SkillCapTier5;

		// Token: 0x040002A2 RID: 674
		public static readonly Option<int> SkillCapTier6;

		// Token: 0x040002A3 RID: 675
		public static readonly Option<int> SkillCapTier7Plus;

		// Token: 0x040002A4 RID: 676
		public static readonly Option<int> SkillCapHeroes;

		// Token: 0x040002A5 RID: 677
		public static readonly Option<int> RetinueSkillTotalBonus;

		// Token: 0x040002A6 RID: 678
		public static readonly Option<int> SkillTotalTier0;

		// Token: 0x040002A7 RID: 679
		public static readonly Option<int> SkillTotalTier1;

		// Token: 0x040002A8 RID: 680
		public static readonly Option<int> SkillTotalTier2;

		// Token: 0x040002A9 RID: 681
		public static readonly Option<int> SkillTotalTier3;

		// Token: 0x040002AA RID: 682
		public static readonly Option<int> SkillTotalTier4;

		// Token: 0x040002AB RID: 683
		public static readonly Option<int> SkillTotalTier5;

		// Token: 0x040002AC RID: 684
		public static readonly Option<int> SkillTotalTier6;

		// Token: 0x040002AD RID: 685
		public static readonly Option<int> SkillTotalTier7Plus;

		// Token: 0x040002AE RID: 686
		private const string McmId = "Retinues.Settings";

		// Token: 0x040002AF RID: 687
		private const string McmDisplay = "Retinues";

		// Token: 0x040002B0 RID: 688
		private const string McmFolder = "Retinues";

		// Token: 0x040002B1 RID: 689
		private const string McmFormat = "xml";

		// Token: 0x040002B2 RID: 690
		private static FluentGlobalSettings _mcmSettings;

		// Token: 0x040002B3 RID: 691
		private static object _mcmSettingsInstance;

		// Token: 0x040002B4 RID: 692
		private static Type _mcmSettingsType;

		// Token: 0x040002B5 RID: 693
		private static bool _isSyncingWithMcm;

		// Token: 0x040002B6 RID: 694
		private static readonly List<IOption> _all;

		// Token: 0x040002B7 RID: 695
		private static readonly Dictionary<string, IOption> _byKey;

		// Token: 0x040002B8 RID: 696
		private static readonly Dictionary<string, object> _values;

		// Token: 0x040002B9 RID: 697
		private static readonly Dictionary<string, int> _ordinalByKey;

		// Token: 0x040002BB RID: 699
		private static string _exportName;

		// Token: 0x020001DF RID: 479
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000520 RID: 1312
			public static Action<string, object> <0>__SyncOptionToMcm;
		}
	}
}
