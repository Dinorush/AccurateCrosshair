using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AccurateCrosshair
{
    [BepInPlugin("Dinorush.AccurateCrosshair", "AccurateCrosshair", "1.0.0")]
    public class Loader : BasePlugin
    {
        public const string MODNAME = "AccurateCrosshair";

        public static ManualLogSource Logger;

        public override void Load()
        {
            Logger = base.Log;
            base.Log.LogMessage("Loading " + MODNAME);
            Configuration.CreateAndBindAll();
            new Harmony(MODNAME).PatchAll(typeof(CrosshairPatches));
            base.Log.LogMessage("Loaded " + MODNAME);
        }
    }
}