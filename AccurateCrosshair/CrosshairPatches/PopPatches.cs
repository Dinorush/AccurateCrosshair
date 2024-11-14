using HarmonyLib;

namespace AccurateCrosshair.CrosshairPatches
{
    [HarmonyPatch]
    internal static class PopPatches
    {
        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.PopCircleCrosshair))]
        [HarmonyPrefix]
        public static bool CancelRecoilPop()
        {
            return Configuration.PopEnabled;
        }
    }
}
