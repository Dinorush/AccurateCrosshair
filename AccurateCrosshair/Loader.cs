using AccurateCrosshair.CrosshairPatches;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Diagnostics;

namespace AccurateCrosshair
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.1.0")]
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

            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}