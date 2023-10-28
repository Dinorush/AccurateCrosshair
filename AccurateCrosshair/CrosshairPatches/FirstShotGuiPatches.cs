using HarmonyLib;
using System;
using UnityEngine;

namespace AccurateCrosshair.CrosshairPatches
{
    internal class FirstShotGui : GuiLayer
    {
        private CircleCrosshair? smallCrosshair;

        public override void OnLevelCleanup()
        {
            smallCrosshair?.SetVisible(v: false);
        }

        public override void Setup(Transform root, string name)
        {
            base.Setup(root, name);
            GuiLayerComp temp = AddComp("Gui/Crosshairs/CanvasCircleCrosshair");
            smallCrosshair = temp.TryCast<CircleCrosshair>();

            if (smallCrosshair != null)
            {
                smallCrosshair.UpdateAlphaMul(CellSettingsManager.SettingsData.HUD.Player_CrosshairOpacity.Value);
                smallCrosshair.SetVisible(v: false);
            }
        }

        public void Disable()
        {
            smallCrosshair?.SetVisible(v: false);
        }

        public void Enable(float size)
        {
            // Don't generate a reticle if size is invalid or it is no different (i.e. it would just duplicate the crosshair)
            if (smallCrosshair == null || size < 0 || size == Configuration.minSize)
                return;

            float crosshairSize = Math.Max(size * 0.2f, Configuration.minSize);

            smallCrosshair.SetVisible(v: true);
            smallCrosshair.SetScale(crosshairSize / smallCrosshair.m_circleRadius);
        }
    }

    internal class FirstShotGuiPatches
    {
        public static FirstShotGui crosshairGui = new FirstShotGui();

        public static void Disable()
        {
            crosshairGui.Disable();
        }

        public static void Enable(float size, bool forceOn = false)
        {
            // SpreadPatches uses a prefix so circleCrosshair might not be visible yet in that case.
            if (forceOn || GuiManager.CrosshairLayer.m_circleCrosshair.GetVisible())
                crosshairGui.Enable(size);
        }

        [HarmonyPatch(typeof(GuiManager), nameof(GuiManager.Setup))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void SetupSmallCrosshair(GuiManager __instance)
        {
            crosshairGui.Setup(__instance.m_root, "SmallCrosshairLayer");
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.OnSetVisible))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void MatchLayerVisibility(CrosshairGuiLayer __instance, bool visible)
        {
            crosshairGui.SetVisible(visible);
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.HideCircleCrosshair))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void MatchCrosshairVisibility(CrosshairGuiLayer __instance)
        {
            crosshairGui.Disable();
        }
    }
}
