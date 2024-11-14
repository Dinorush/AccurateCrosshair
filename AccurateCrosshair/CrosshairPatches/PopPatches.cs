using HarmonyLib;

namespace AccurateCrosshair.CrosshairPatches
{
    internal static class PopPatches
    {
        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.PopCircleCrosshair))]
        [HarmonyPrefix]
        public static bool CancelRecoilPop()
        {
            return false;
        }
    }
}
