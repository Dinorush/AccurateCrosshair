using HarmonyLib;
using UnityEngine;

namespace AccurateCrosshair.CrosshairPatches
{
    internal static class RecoilPatches
    {
        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.ShowSpreadCircle))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void EnableCrosshairMovement(CrosshairGuiLayer __instance, ref float crosshairSize)
        {
            // For some reason, shotguns completely ignore world view blend and always aim where the camera points.
            __instance.m_moveCircleCrosshair = !SpreadPatches.isShotgun;
        }

        [HarmonyPatch(typeof(FPS_RecoilSystem), nameof(FPS_RecoilSystem.FPS_Update))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void TightCrosshairMovement(FPS_RecoilSystem __instance)
        {
            // Aside from the crosshair spread affecting recoil pos as described below, there appears to be some issue with the math
            // on the y-axis of the layer. I couldn't figure it out, so I just left in a scalar that appears to work.

            // The spread of the crosshair is actually the entire layer scaling size.
            // Need to adjust the offset of recoil by the scale to get it to map properly to UI.
            float scale = SpreadPatches.CrosshairLayer.m_circleCrosshair.GetScale();
            Vector2 centeredPos = __instance.CurrentVSPos - new Vector2(0.5f, 0.5f);

            // For some reason, the y component of the recoil is higher than it should be.
            centeredPos.y *= .78f; // Based on testing. Is probably related to 1440/2560 (the crosshair GUI layer size). 1-(1-1440/2560)/2 is about this value.
            SpreadPatches.CrosshairLayer.SetCrosshairPosition(centeredPos / scale + new Vector2(0.5f, 0.5f));
        }
    }
}
