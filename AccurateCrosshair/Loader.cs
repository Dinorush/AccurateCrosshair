using AccurateCrosshair.CrosshairPatches;
using AccurateCrosshair.PluginDependencies;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GTFO.API;
using HarmonyLib;
using System.Diagnostics;

namespace AccurateCrosshair
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.4.1")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Dinorush.ColorCrosshair", BepInDependency.DependencyFlags.SoftDependency)]
    internal sealed class Loader : BasePlugin
    {
        public const string MODNAME = "AccurateCrosshair";

#if DEBUG
        private static ManualLogSource Logger;
#endif

        [Conditional("DEBUG")]
        public static void DebugLog(object data)
        {
#if DEBUG
            Logger.LogMessage(data);
#endif
        }

        public override void Load()
        {
#if DEBUG
            Logger = Log;
#endif
            Log.LogMessage("Loading " + MODNAME);
            Configuration.CreateAndBindAll();

            Harmony harmonyInstance = new Harmony(MODNAME);
            harmonyInstance.PatchAll(typeof(SpreadPatches));
            if (!Configuration.popEnabled)
                harmonyInstance.PatchAll(typeof(PopPatches));
            if (Configuration.followsRecoil)
                harmonyInstance.PatchAll(typeof(RecoilPatches));
            if (Configuration.firstShotType != FirstShotType.None)
                harmonyInstance.PatchAll(typeof(FirstShotPatches));
            if (Configuration.firstShotType == FirstShotType.Inner)
                harmonyInstance.PatchAll(typeof(FirstShotGuiPatches));

            AssetAPI.OnStartupAssetsLoaded += AssetAPI_OnStartupAssetsLoaded;
            LevelAPI.OnLevelCleanup += LevelAPI_OnLevelCleanup;
            Log.LogMessage("Loaded " + MODNAME);
        }

        private void AssetAPI_OnStartupAssetsLoaded()
        {
            ColorCrosshairDependency.Init();
        }

        private void LevelAPI_OnLevelCleanup()
        {
            SpreadPatches.OnCleanup();
        }
    }
}