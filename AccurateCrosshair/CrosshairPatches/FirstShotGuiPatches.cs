using HarmonyLib;
using System;
using UnityEngine;
using AccurateCrosshair.PluginDependencies;

namespace AccurateCrosshair.CrosshairPatches
{
    internal sealed class FirstShotGui : GuiLayer
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
                RefreshColor();
                smallCrosshair.SetVisible(v: false);
            }
        }

        public void Disable()
        {
            smallCrosshair?.SetVisible(v: false);
        }

        public void Enable()
        {
            // Don't generate a reticle if size is invalid or it is no different (i.e. it would just duplicate the crosshair)
            if (smallCrosshair == null)
                return;

            smallCrosshair.SetVisible(v: true);
            smallCrosshair.SetScale(SpreadPatches.GetCrosshairSize(0.2f) / smallCrosshair.m_circleRadius);
        }

        public void RefreshAlpha()
        {
            smallCrosshair?.UpdateAlphaMul(CellSettingsManager.SettingsData.HUD.Player_CrosshairOpacity.Value);
        }

        public void RefreshColor()
        {
            if (smallCrosshair == null || !ColorCrosshairDependency.HasColorCrosshair) return;

            smallCrosshair.SetColor(smallCrosshair.m_crosshairColOrg = ColorCrosshairDependency.DefaultColor);
        }

        public void RefreshSize()
        {
            smallCrosshair?.SetScale(SpreadPatches.GetCrosshairSize(0.2f) / smallCrosshair.m_circleRadius);
        }
    }

    internal static class FirstShotGuiPatches
    {
        public static FirstShotGui crosshairGui = new();

        public static void Disable()
        {
            crosshairGui.Disable();
        }

        public static void Enable(bool forceOn = false)
        {
            // SpreadPatches uses a prefix so circleCrosshair might not be visible yet in that case.
            if (forceOn || SpreadPatches.CrosshairLayer.m_circleCrosshair.GetVisible())
                crosshairGui.Enable();
        }

        public static void RefreshCrosshairColor()
        {
            crosshairGui.RefreshColor();
        }

        public static void RefreshCrosshairSize()
        {
            crosshairGui.RefreshSize();
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

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.UpdateAlphaFromSettings))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void MatchCrosshairAlpha(CrosshairGuiLayer __instance)
        {
            crosshairGui.RefreshAlpha();
        }
    }
}
