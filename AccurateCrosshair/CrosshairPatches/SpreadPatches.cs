﻿using BepInEx.Logging;
using GameData;
using Gear;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
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

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.ShowSpreadCircle))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void AdjustCrosshairSize(CrosshairGuiLayer __instance, ref float crosshairSize)
        {
            if (Configuration.firstShotType != FirstShotType.None)
                FirstShotPatches.ResetStoredCrosshair();

            BulletWeapon? weapon = PlayerManager.GetLocalPlayerAgent().Inventory.m_wieldedItem.TryCast<BulletWeapon>();
            if (weapon == null)
                return;

            isShotgun = weapon.TryCast<Shotgun>() != null;

            float playerFoV = CellSettingsManager.SettingsData.Video.Fov.Value;
            ArchetypeDataBlock? archetypeData = weapon.ArchetypeData;
            if (archetypeData == null) // We need a spread value to do anything
                return;

            float sizeMultiplier = (float)(BASE_CROSSHAIR_SIZE / Math.Tan(Math.PI / 180.0 * playerFoV / 2));
            float spread = isShotgun ? archetypeData.ShotgunConeSize + archetypeData.ShotgunBulletSpread : archetypeData.HipFireSpread;
            float newSize = Math.Max(Configuration.minSize, spread * sizeMultiplier + EXTRA_BUFFER_SIZE);

            crosshairSize = newSize;

            if (!isShotgun && Configuration.firstShotType != FirstShotType.None)
                FirstShotPatches.SetStoredCrosshair(weapon, ref crosshairSize);
        }
    }
}