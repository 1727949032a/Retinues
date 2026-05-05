using System;
using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using Retinues.Configuration;
using Retinues.Doctrines;
using Retinues.Doctrines.Effects;
using Retinues.Features.Agents;
using Retinues.Features.AutoJoin;
using Retinues.Features.Equipments;
using Retinues.Features.Experience;
using Retinues.Features.Staging;
using Retinues.Features.Statistics;
using Retinues.Features.Stocks;
using Retinues.Features.Swaps;
using Retinues.Features.Unlocks;
using Retinues.Features.Volunteers.Patches;
using Retinues.Game;
using Retinues.Game.Wrappers;
using Retinues.GUI.Editor;
using Retinues.Mods;
using Retinues.Safety.Fixes;
using Retinues.Safety.Legacy;
using Retinues.Safety.Sanitizer;
using Retinues.Safety.Version;
using Retinues.Troops;
using Retinues.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace Retinues
{
	// Token: 0x0200001D RID: 29
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x0600004B RID: 75 RVA: 0x0000254F File Offset: 0x0000074F
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			this.TryRegisterWithMCM();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000255D File Offset: 0x0000075D
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			this.TruncateLogFile();
			this.EnableUIExtender();
			this.ApplyHarmonyPatches();
			this.LogModuleInfo();
			ModCompatibility.IncompatibilityCheck();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002584 File Offset: 0x00000784
		protected override void OnGameStart(Game game, IGameStarter gameStarter)
		{
			base.OnGameStart(game, gameStarter);
			CampaignGameStarter campaignGameStarter = gameStarter as CampaignGameStarter;
			if (campaignGameStarter != null)
			{
				SubModule.ClearAll();
				this.AddBehaviors(campaignGameStarter);
				VolunteerSwapForPlayer.Initialize();
			}
			Log.Debug(L.S("loc_smoke_test", "Localization test: default fallback (EN)."));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000025C8 File Offset: 0x000007C8
		protected override void OnSubModuleUnloaded()
		{
			base.OnSubModuleUnloaded();
			this.RemoveHarmonyPatches();
			this.DisableUIExtender();
			Log.Debug("SubModule unloaded.");
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000025E6 File Offset: 0x000007E6
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			SubModule.TryHandleEditorHotkeys(dt);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000025F8 File Offset: 0x000007F8
		[SafeMethod(null, true, null)]
		private void TryRegisterWithMCM()
		{
			if (!this._mcmRegistered && this._mcmRetryCount < 300)
			{
				this._mcmRetryCount++;
				this._mcmRegistered = Config.RegisterWithMCM();
				if (this._mcmRegistered)
				{
					Log.Info("MCM: registration succeeded.");
					SubModule.MCMRegistered = true;
				}
				Config.LogDump();
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002650 File Offset: 0x00000850
		private void ApplyHarmonyPatches()
		{
			try
			{
				this._harmony = new Harmony("Retinues");
				this._harmony.PatchAll(Assembly.GetExecutingAssembly());
				SafeMethodPatcher.ApplyAll(this._harmony, new Assembly[]
				{
					Assembly.GetExecutingAssembly()
				});
				ModCompatibility.AddPatches(this._harmony);
				Log.Debug("Harmony patches applied.");
				SubModule.HarmonyPatchesApplied = true;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Error while applying Harmony patches.", null);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000026D0 File Offset: 0x000008D0
		private void RemoveHarmonyPatches()
		{
			try
			{
				Harmony harmony = this._harmony;
				if (harmony != null)
				{
					harmony.UnpatchAll("Retinues");
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Error while removing existing Harmony patches.", null);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002714 File Offset: 0x00000914
		[SafeMethod(null, true, null)]
		public void EnableUIExtender()
		{
			try
			{
				this._extender = UIExtender.Create("Retinues");
				this._extender.Register(typeof(SubModule).Assembly);
				this._extender.Enable();
				Log.Debug("UIExtender enabled & assembly registered.");
				SubModule.UIExtenderExEnabled = true;
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "UIExtender enabling failed.", null);
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002788 File Offset: 0x00000988
		[SafeMethod(null, true, null)]
		public void DisableUIExtender()
		{
			try
			{
				UIExtender extender = this._extender;
				if (extender != null)
				{
					extender.Disable();
				}
				Log.Debug("Disabling UIExtender...");
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "UIExtender disabling failed.", null);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000027D0 File Offset: 0x000009D0
		[SafeMethod(null, true, null)]
		private static void TryHandleEditorHotkeys(float dt)
		{
			try
			{
				if (Config.EnableEditorHotkey)
				{
					Game game = Game.Current;
					if (game != null)
					{
						if (game.GameStateManager.ActiveState is MapState)
						{
							if (Campaign.Current != null)
							{
								if (Input.IsKeyDown(InputKey.LeftShift))
								{
									if (Input.IsKeyReleased(InputKey.R))
									{
										Log.Info("EditorMapHotkey: Shift+R pressed on map (OnApplicationTick).");
										ClanScreen.LaunchEditor(EditorMode.Personal);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "", null);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002858 File Offset: 0x00000A58
		[SafeMethod(null, true, null)]
		private void TruncateLogFile()
		{
			if (Log.LogFileLength > 20000)
			{
				Log.Truncate(10000);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002870 File Offset: 0x00000A70
		[SafeMethod(null, true, null)]
		private void LogModuleInfo()
		{
			Log.Info(string.Format("Bannerlord version: {0}.{1}.{2}", BannerlordVersion.Version.Major, BannerlordVersion.Version.Minor, BannerlordVersion.Version.Revision));
			Log.Info("Modules:");
			foreach (ModuleChecker.ModuleEntry moduleEntry in ModuleChecker.GetActiveModules())
			{
				Log.Info(string.Concat(new string[]
				{
					"    ",
					moduleEntry.IsOfficial ? "[Official]" : "[Community]",
					" ",
					moduleEntry.Id,
					" ",
					moduleEntry.Version
				}));
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000295C File Offset: 0x00000B5C
		private void AddBehaviors(CampaignGameStarter cs)
		{
			Log.Info("Registering behaviors...");
			SubModule.AddBehavior<TroopXpBehavior>(cs, null);
			SubModule.AddBehavior<BattleSimulationXpBehavior>(cs, null);
			SubModule.AddBehavior<TrainStagingBehavior>(cs, null);
			SubModule.AddBehavior<EquipStagingBehavior>(cs, null);
			SubModule.AddBehavior<TroopEquipBehavior>(cs, null);
			SubModule.AddBehavior<TroopTrainBehavior>(cs, null);
			SubModule.AddBehavior<TroopBehavior>(cs, null);
			SubModule.AddBehavior<FactionBehavior>(cs, null);
			SubModule.AddBehavior<SanitizerBehavior>(cs, null);
			SubModule.AddBehavior<VersionBehavior>(cs, null);
			SubModule.AddBehavior<DependenciesBehavior>(cs, null);
			SubModule.AddBehavior<PartyLeaderFixBehavior>(cs, null);
			SubModule.AddBehavior<UnlocksBehavior>(cs, null);
			SubModule.AddBehavior<StocksBehavior>(cs, null);
			SubModule.AddBehavior<EquipmentRebateBehavior>(cs, null);
			SubModule.AddBehavior<PartySwapBehavior>(cs, null);
			SubModule.AddBehavior<AutoJoinBehavior>(cs, null);
			SubModule.AddBehavior<CombatAgentBehavior>(cs, null);
			SubModule.AddBehavior<TroopStatisticsBehavior>(cs, null);
			if (Config.EnableDoctrines)
			{
				SubModule.AddBehavior<DoctrineServiceBehavior>(cs, null);
				SubModule.AddBehavior<DoctrineEffectRuntimeBehavior>(cs, null);
				if (Config.EnableFeatRequirements)
				{
					SubModule.AddBehavior<FeatServiceBehavior>(cs, null);
					SubModule.AddBehavior<FeatNotificationBehavior>(cs, null);
				}
			}
			ModCompatibility.AddBehaviors(cs);
			Log.Debug("Behaviors registered.");
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002A3C File Offset: 0x00000C3C
		public static void RegisterBehavior<TBehavior>(Func<TBehavior> factory) where TBehavior : CampaignBehaviorBase
		{
			if (factory == null)
			{
				Log.Warn("RegisterBehavior<" + typeof(TBehavior).Name + "> ignored: null factory.");
				return;
			}
			object behaviorLock = SubModule._behaviorLock;
			lock (behaviorLock)
			{
				bool flag2 = SubModule._behaviorFactories.ContainsKey(typeof(TBehavior));
				SubModule._behaviorFactories[typeof(TBehavior)] = (() => factory());
				Log.Info((flag2 ? "Replaced" : "Registered") + " behavior factory for " + typeof(TBehavior).Name + ".");
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00002B14 File Offset: 0x00000D14
		private static TBehavior ResolveBehavior<TBehavior>(Func<TBehavior> defaultFactory) where TBehavior : CampaignBehaviorBase
		{
			object behaviorLock = SubModule._behaviorLock;
			lock (behaviorLock)
			{
				Func<CampaignBehaviorBase> func;
				if (SubModule._behaviorFactories.TryGetValue(typeof(TBehavior), out func))
				{
					try
					{
						TBehavior tbehavior = func() as TBehavior;
						if (tbehavior != null)
						{
							return tbehavior;
						}
						Log.Warn("Factory for " + typeof(TBehavior).Name + " returned incompatible instance; falling back.");
					}
					catch (Exception ex)
					{
						Log.Exception(ex, "Factory for " + typeof(TBehavior).Name + " threw; falling back.", null);
					}
				}
			}
			return defaultFactory();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002BE4 File Offset: 0x00000DE4
		private static void AddBehavior<TBehavior>(CampaignGameStarter cs, Func<TBehavior> defaultFactory = null) where TBehavior : CampaignBehaviorBase, new()
		{
			Func<TBehavior> df = defaultFactory;
			if (defaultFactory == null && (df = SubModule.<>c__26<TBehavior>.<>9__26_0) == null)
			{
				df = (SubModule.<>c__26<TBehavior>.<>9__26_0 = (() => Activator.CreateInstance<TBehavior>()));
			}
			Func<TBehavior> df2 = defaultFactory;
			if (defaultFactory == null && (df2 = SubModule.<>c__26<TBehavior>.<>9__26_1) == null)
			{
				df2 = (SubModule.<>c__26<TBehavior>.<>9__26_1 = (() => Activator.CreateInstance<TBehavior>()));
			}
			TBehavior tbehavior = SubModule.ResolveBehavior<TBehavior>(df, df2);
			cs.AddBehavior(tbehavior);
			Log.Debug(string.Concat(new string[]
			{
				"Behavior active: ",
				tbehavior.GetType().FullName,
				" (as ",
				typeof(TBehavior).Name,
				")."
			}));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002C98 File Offset: 0x00000E98
		private static TBehavior ResolveBehavior<TBehavior>(Func<TBehavior> df1, Func<TBehavior> df2) where TBehavior : CampaignBehaviorBase
		{
			return SubModule.ResolveBehavior<TBehavior>(df1);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002CA0 File Offset: 0x00000EA0
		private static void ClearAll()
		{
			Log.Debug("Clearing all static properties.");
			Player.Reset();
			WCharacter.ActiveStubIds.Clear();
			WCharacter.VanillaStringIdMap.Clear();
			WCharacter.UpgradeMap.Clear();
			WCharacter.ClearCaptainCaches();
			WCharacter.EditedVanillaRootIds.Clear();
			WCharacter.NavalTraitHelper.Reset();
			WCharacter.ClearSkillCaches();
			BaseFaction.TroopFactionMap.Clear();
			BaseFaction.TroopFactionMapVersion = 0;
			Log.Debug("All static properties cleared.");
		}

		// Token: 0x04000007 RID: 7
		public static bool HarmonyPatchesApplied = false;

		// Token: 0x04000008 RID: 8
		public static bool UIExtenderExEnabled = false;

		// Token: 0x04000009 RID: 9
		public static bool MCMRegistered = false;

		// Token: 0x0400000A RID: 10
		private bool _mcmRegistered;

		// Token: 0x0400000B RID: 11
		private int _mcmRetryCount;

		// Token: 0x0400000C RID: 12
		private const int _mcmMaxRetries = 300;

		// Token: 0x0400000D RID: 13
		private Harmony _harmony;

		// Token: 0x0400000E RID: 14
		private UIExtender _extender;

		// Token: 0x0400000F RID: 15
		private static readonly object _behaviorLock = new object();

		// Token: 0x04000010 RID: 16
		private static readonly Dictionary<Type, Func<CampaignBehaviorBase>> _behaviorFactories = new Dictionary<Type, Func<CampaignBehaviorBase>>(EqualityComparer<Type>.Default);
	}
}
