using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AccurateCrosshair
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.0")]
    internal class Loader : BasePlugin
    {
        public const string MODNAME = "AccurateCrosshair";

        public static ManualLogSource Logger;

        public override void Load()
        {
            Logger = Log;
            Logger.LogMessage("Loading " + MODNAME);
            Configuration.CreateAndBindAll();
            new Harmony(MODNAME).PatchAll(typeof(CrosshairPatches));
            Logger.LogMessage("Loaded " + MODNAME);
        }
    }
}