﻿using R2API;
using BepInEx;
using RoR2;
using R2API.Utils;
using RiskyMod.Items.Uncommon;
using RiskyMod.Items.Common;
using RiskyMod.SharedHooks;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.Legendary;
using UnityEngine;
using RiskyMod.Tweaks;
using RiskyMod.Fixes;
using RiskyMod.Items;
using RiskyMod.Allies;
using RiskyMod.Items.Equipment;
using UnityEngine.Networking;
using RiskyMod.Moon;
using Zio;
using Zio.FileSystems;
using System.Collections.Generic;
using RiskyMod.Survivors;
using RiskyMod.Enemies;
using RiskyMod.Survivors.Captain;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;
using RiskyMod.Survivors.Croco;
using RiskyMod.Enemies.Mobs.Lunar;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Survivors.Merc;
using System.Runtime.CompilerServices;
using RiskyMod.Content;
using System.Reflection;
using RiskyMod.Items.DLC1.Legendary;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Survivors.DLC1.VoidFiend;
using System.IO;
using RiskyMod.VoidLocus;
using RiskyMod.Survivors.DLC1.Railgunner;
using RiskyMod.Items.DLC1.Uncommon;
using RiskyMod.Tweaks.RunScaling;
using RiskyMod.Tweaks.Holdouts;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.Artifacts;
using RiskyMod.MonoBehaviours;
using RiskyMod.VoidFields;

namespace RiskyMod
{
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MobileTurretBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixPlayercount", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoLevelupHeal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.wildbook.multitudes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.ArtifactReliquaryHealingFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RaiseMonsterLevelCap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.PlasmaCore.AlwaysAllowSupplyDrops", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Hypercrit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TheTimeSweeper.AcridHitboxBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AcridBlightStack", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.LunarWispFalloff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BeetleQueenPlus", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoVoidAllies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Charzard4261.CaptainAbilitiesOffworld", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EnigmaBlacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.GestureEnigma", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BackstabRework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.TeleExpansion", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.InteractableLimit", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod Beta", "0.7.4")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DamageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        public static bool disableProcChains = true;

        public static bool ClassicItemsScepterLoaded = false;
        public static bool ScepterPluginLoaded = false;
        public static bool AIBlacklistLoaded = false;
        public static bool AIBlacklistUseVanillaBlacklist = true;
        public static bool ZetTweaksLoaded = false;

        public static bool ShareSuiteLoaded = false;
        public static bool ShareSuiteCommon = false;
        public static bool ShareSuiteUncommon = false;
        public static bool ShareSuiteLegendary = false;
        public static bool ShareSuiteBoss = false;
        public static bool ShareSuiteLunar = false;

        public static bool EnableProjectileCloning = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static AssistManager assistManager = null;

        public static PluginInfo pluginInfo;
        public static FileSystem fileSystem { get; private set; }

        public static GameModeIndex classicRunIndex;
        public static GameModeIndex simulacrumIndex;

        public void Awake()
        {
            On.RoR2.GameModeCatalog.LoadGameModes += (orig) =>
            {
                orig();
                simulacrumIndex = GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
                classicRunIndex = GameModeCatalog.FindGameModeIndex("ClassicRun");
            };

            pluginInfo = Info;
            Assets.Init();
            ConfigFiles.Init();
            CheckDependencies();

            ProjectileZapChainOnExplosion.Init();

            ContentCore.Init();
            new SharedDamageTypes();
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new AlliesCore();
            new VoidFieldsCore();
            new MoonCore();
            new VoidLocusCore();
            new SurvivorsCore();
            new FireSelect();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
        }

        private void CheckDependencies()
        {
            ZetTweaksLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetTweaks");
            if (ZetTweaksLoaded) ZetTweaksCompat();

            NoLevelupHeal.enabled = NoLevelupHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal");
            RemoveLevelCap.enabled = RemoveLevelCap.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RaiseMonsterLevelCap");

            FixPlayercount.enabled = FixPlayercount.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount");
            FixPlayercount.MultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");
            FixPlayercount.ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");

            VoidInfestor.noVoidAllies = VoidInfestor.noVoidAllies && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoVoidAllies");

            AIBlacklistLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            if (AIBlacklistLoaded) HandleAIBlacklist();
            FixVengeanceLeveling.enabled = FixVengeanceLeveling.enabled && !AIBlacklistLoaded;

            PreventArtifactHeal.enabled = PreventArtifactHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.ArtifactReliquaryHealingFix");
            CaptainOrbitalHiddenRealms.enabled = CaptainOrbitalHiddenRealms.enabled
                && !(BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.PlasmaCore.AlwaysAllowSupplyDrops")|| BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Charzard4261.CaptainAbilitiesOffworld"));
            CritHud.enabled = CritHud.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");   //Effect is already a part of hypercrit

            ScepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            ClassicItemsScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");

            //Croco
            BiggerMeleeHitbox.enabled = BiggerMeleeHitbox.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.AcridHitboxBuff");
            BlightStack.enabled = BlightStack.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AcridBlightStack");

            //Bandit2
            BackstabRework.enabled = BackstabRework.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BackstabRework");

            //Enemies
            BeetleQueen.enabled = BeetleQueen.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BeetleQueenPlus");
            LunarWisp.enableFalloff = LunarWisp.enableFalloff && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.LunarWispFalloff");

            FixEnigmaBlacklist.enabled = FixEnigmaBlacklist.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EnigmaBlacklist");
            Gesture.enabled = Gesture.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.GestureEnigma");
            NerfVoidtouched.enabled = NerfVoidtouched.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");

            ShareSuiteLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
            if (ShareSuiteLoaded) HandleShareSuite();

            SpawnLimits.enabled = SpawnLimits.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.InteractableLimit");

            //No need for this to be static since this doesn't get referenced anywhere else
            bool teleExpansionLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.TeleExpansion");
            SmallHoldoutCharging.enabled = SmallHoldoutCharging.enabled && !teleExpansionLoaded;
            TeleExpandOnBossKill.enabled = TeleExpandOnBossKill.enabled && !teleExpansionLoaded;
            VoidLocus.ModifyHoldout.enabled = VoidLocus.ModifyHoldout.enabled && !teleExpansionLoaded;
            Moon.ModifyHoldout.enabled = Moon.ModifyHoldout.enabled && !teleExpansionLoaded;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ZetTweaksCompat()
        {
            if (ZetTweaksLoaded)
            {
                Allies.MegaDrone.allowRepair = false;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private bool CheckClassicItemsScepter()
        {
            return ThinkInvisible.ClassicItems.Scepter.instance.enabled;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleAIBlacklist()
        {
            AIBlacklistUseVanillaBlacklist = AI_Blacklist.AIBlacklist.useVanillaAIBlacklist;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleShareSuite()
        {
            ShareSuiteCommon = ShareSuite.ShareSuite.WhiteItemsShared.Value;
            ShareSuiteUncommon = ShareSuite.ShareSuite.GreenItemsShared.Value;
            ShareSuiteLegendary = ShareSuite.ShareSuite.RedItemsShared.Value;
            ShareSuiteBoss = ShareSuite.ShareSuite.BossItemsShared.Value;
            ShareSuiteLunar = ShareSuite.ShareSuite.LunarItemsShared.Value;
        }

        private void RunTweaks()
        {
            //RunScaling
            new ModdedScaling();
            new RemoveLevelCap();
            new NoLevelupHeal();
            new SceneDirectorMonsterRewards();
            new MonsterGoldRewards();
            new CombatDirectorMultiplier();
            new LoopBossArmor();

            //Holdouts
            new SmallHoldoutCharging();
            new TeleExpandOnBossKill();

            //Interactables
            new BloodShrineMinReward();
            new ShrineCombatItems();
            new SpawnLimits();

            //Character Mechanics
            new TrueOSP();
            new ShieldGating();
            new CloakBuff();
            new Shock();
            new FixSlayer();
            new BarrierDecay();
            new FreezeChampionExecute();
            new NerfVoidtouched();
            new PlayerControlledMonsters();

            //Artifacts
            new VengeancePercentHeal();
            new EnigmaBlacklist();

            //Misc
            new AIBlacklistItems();
        }

        private void RunFixes()
        {
            new FixPlayercount();
            new FixVengeanceLeveling();
            new FixFocusCrystalSelfDamage();
            new PreventArtifactHeal();
            new TreebotFruitingNullref();
            new FixLightningStrikeOrbProcCoefficient();
            new FixCrocoPoisonAchievement();
            new FixWormFallDeath();
            new GhostDunestriderFriendlyFire();
            new GhostGrandparentFriendlyFire();
        }
        
        private void AddHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.OnHitAll += OnHitAll.GlobalEventManager_OnHitAll;

            new ModifyFinalDamage();
        }

        private void SetupAssists()
        {
            AssistManager.initialized = true;
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    RiskyMod.assistManager = self.gameObject.GetComponent<AssistManager>();
                    if (!RiskyMod.assistManager)
                    {
                        RiskyMod.assistManager = self.gameObject.AddComponent<AssistManager>();
                    }
                }
            };
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            if (NetworkServer.active)
            {
                RiskyMod.assistManager = obj.gameObject.GetComponent<AssistManager>();
                if (!RiskyMod.assistManager)
                {
                    RiskyMod.assistManager = obj.gameObject.AddComponent<AssistManager>();
                }
            }
        }

        public delegate void FireMode();
        public static FireMode FireModeActions = FireModeMethod;
        private static void FireModeMethod() { }
        private void Update()
        {
            FireModeActions.Invoke();
        }
    }
}
