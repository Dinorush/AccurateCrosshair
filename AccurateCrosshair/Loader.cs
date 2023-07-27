using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AccurateCrosshair
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.2")]
    internal class Loader : BasePlugin
    {
        public const string MODNAME = "AccurateCrosshair";

#if DEBUG
        public static ManualLogSource Logger;
#endif

        public override void Load()
        {
#if DEBUG
        `   Logger = Log;
#endif
            Log.LogMessage("Loading " + MODNAME);
            Configuration.CreateAndBindAll();
            new Harmony(MODNAME).PatchAll(typeof(CrosshairPatches));
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}