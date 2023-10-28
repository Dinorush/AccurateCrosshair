using Gear;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using GameData;
using CollectionExtensions = BepInEx.Unity.IL2CPP.Utils.Collections.CollectionExtensions;
using System;
using static GameData.GD;

namespace AccurateCrosshair.CrosshairPatches
{
    internal class FirstShotPatches
    {
        private static Coroutine? firstShotRoutine = null;
        private static float firstShotTime = 0f;
        private static float fireRecoilCooldown = 2f;
        private static float storedCrosshairSize = -1f;

        private static void EnableSmallCrosshair(bool forceGuiOn = false)
        {
            if (Configuration.firstShotType == FirstShotType.Match)
                SpreadPatches.SetCrosshairSize(storedCrosshairSize * 0.2f);
            else if (Configuration.firstShotType == FirstShotType.Inner)
                FirstShotGuiPatches.Enable(storedCrosshairSize, forceGuiOn);
        }

        private static IEnumerator MinimizeAfterDelay()
        {
            // Adjust firstShotTime to keep the coroutine alive instead of making a new coroutine with every shot fired.
            while (firstShotTime > Clock.Time)
                yield return new WaitForSeconds(firstShotTime - Clock.Time);

            firstShotRoutine = null;

            // If we swapped to an invalid weapon, stop before setting the crosshair size.
            if (storedCrosshairSize < 0)
                yield break;

            EnableSmallCrosshair();
        }


        public static void SetStoredCrosshair(BulletWeapon weapon, ref float crosshairSize)
        {
            ArchetypeDataBlock? archetypeData = weapon.ArchetypeData;
            if (Configuration.firstShotType == FirstShotType.Match &&
                archetypeData.FireMode != 0 && archetypeData.ShotDelay < Configuration.firstShotMinDelay)
                return;

            storedCrosshairSize = crosshairSize;
            fireRecoilCooldown = weapon.m_fireRecoilCooldown;
            firstShotTime = weapon.m_lastFireTime + fireRecoilCooldown;

            if (firstShotTime > Clock.Time)
                firstShotRoutine ??= CoroutineManager.StartCoroutine(CollectionExtensions.WrapToIl2Cpp(MinimizeAfterDelay()));
            else
            {
                // EnableSmallCrosshair runs before ShowSpreadCircle does so its Match changes get overriden unless we modify this.
                crosshairSize = Math.Max(crosshairSize * 0.2f, Configuration.minSize);

                // Called on prefix to ShowSpreadCircle, so need to force Gui on since the main Crosshair is not yet visible.
                EnableSmallCrosshair(true);
            }
        }

        public static void ResetStoredCrosshair()
        {
            // Coroutine will end on its own if needed if size < 0.
            storedCrosshairSize = -1;
            FirstShotGuiPatches.Disable();
        }

        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void ResetFirstShotTimer(BulletWeapon __instance)
        {
            firstShotTime = Clock.Time + fireRecoilCooldown;
            SpreadPatches.SetCrosshairSize(storedCrosshairSize);
            FirstShotGuiPatches.Disable();
            firstShotRoutine ??= CoroutineManager.StartCoroutine(CollectionExtensions.WrapToIl2Cpp(MinimizeAfterDelay()));
        }
    }

    public enum FirstShotType
    {
        None,
        Match,
        Inner
    }
}
