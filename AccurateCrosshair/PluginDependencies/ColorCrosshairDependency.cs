using BepInEx.Unity.IL2CPP;
using UnityEngine;
using ColorCrosshair.API;
using AccurateCrosshair.CrosshairPatches;

namespace AccurateCrosshair.PluginDependencies
{
    internal static class ColorCrosshairDependency
    {
        public static bool HasColorCrosshair
        {
            get
            {
                return IL2CPPChainloader.Instance.Plugins.ContainsKey("Dinorush.ColorCrosshair");
            }
        }

        public static void Init()
        {
            // If using the inner reticle to show first shot accuracy, need to match its color accordingly
            if (HasColorCrosshair && Configuration.firstShotType == FirstShotType.Inner)
            {
                FirstShotGuiPatches.RefreshCrosshairColor();
                ColorCrosshairAPI.OnReload += ApplyColorChanges;
            }
        }

        private static void ApplyColorChanges()
        {
            FirstShotGuiPatches.RefreshCrosshairColor();
        }

        public static Color DefaultColor => ColorCrosshairAPI.DefaultColor;
        public static Color ChargeColor => ColorCrosshairAPI.ChargeColor;
        public static Color ChargeBlinkColor => ColorCrosshairAPI.ChargeBlinkColor;
        public static Color ChargeWarningColor => ColorCrosshairAPI.ChargeWarningColor;
        public static Color EnemyBlinkColor => ColorCrosshairAPI.EnemyBlinkColor;
    }
}
