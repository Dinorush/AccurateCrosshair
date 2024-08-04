using GameData;
using Gear;
using HarmonyLib;
using Player;
using System;

namespace AccurateCrosshair.CrosshairPatches
{
    internal static class SpreadPatches
    {
        private const float BASE_CROSSHAIR_SIZE = 20.0f; // The base size (from testing) that the crosshair should be at 90 FOV (when tan(FoV/2) = 1)
        private const float EXTRA_BUFFER_SIZE = 10.0f;   // Flat value added to account for the actual reticle, so as to not cover possible shot locations.
        public static bool isShotgun = false;

        public static void SetCrosshairSize(float crosshairSize)
        {
            if (Configuration.firstShotType != FirstShotType.Match || crosshairSize < 0)
                return;

            crosshairSize = Math.Max(Configuration.minSize, crosshairSize);
            GuiManager.CrosshairLayer.ScaleToSize(GuiManager.CrosshairLayer.m_neutralCircleSize = crosshairSize);
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.ShowPrecisionDot))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void ShowAimCrosshair(CrosshairGuiLayer __instance)
        {
            __instance.ShowSpreadCircle(__instance.m_dotSize);
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.ShowSpreadCircle))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void AdjustCrosshairSize(CrosshairGuiLayer __instance, ref float crosshairSize)
        {
            if (Configuration.firstShotType != FirstShotType.None)
                FirstShotPatches.ResetStoredCrosshair();

            PlayerAgent player = PlayerManager.GetLocalPlayerAgent();
            BulletWeapon? weapon = player.Inventory.m_wieldedItem.TryCast<BulletWeapon>();
            if (weapon == null)
                return;
            bool aim = player.FPItemHolder.ItemAimTrigger;

            isShotgun = weapon.TryCast<Shotgun>() != null;

            float playerFoV = CellSettingsManager.SettingsData.Video.Fov.Value;
            if (aim)
                playerFoV = weapon.GetWorldCameraZoomFov();

            ArchetypeDataBlock? archetypeData = weapon.ArchetypeData;
            if (archetypeData == null) // We need a spread value to do anything
                return;

            float sizeMultiplier = (float)(BASE_CROSSHAIR_SIZE / Math.Tan(Math.PI / 180.0 * playerFoV / 2));
            float spread;
            if (isShotgun)
                spread = archetypeData.ShotgunConeSize + archetypeData.ShotgunBulletSpread;
            else
                spread = aim ? archetypeData.AimSpread : archetypeData.HipFireSpread;
            float newSize = Math.Max(Configuration.minSize, spread * sizeMultiplier + EXTRA_BUFFER_SIZE);

            crosshairSize = newSize;

            if (!isShotgun && Configuration.firstShotType != FirstShotType.None)
                FirstShotPatches.SetStoredCrosshair(weapon, ref crosshairSize);
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.Setup))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void AdjustResizeSpeed(CrosshairGuiLayer __instance)
        {
            __instance.m_circleSpeed *= Configuration.speedScalar;
        }
    }
}