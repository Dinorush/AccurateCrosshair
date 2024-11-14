using Gear;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using GameData;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace AccurateCrosshair.CrosshairPatches
{
    internal static class FirstShotPatches
    {
        private static Coroutine? firstShotRoutine = null;
        private static float firstShotTime = 0f;
        private static float fireRecoilCooldown = 2f;

        private static void EnableSmallCrosshair(bool forceGuiOn = false)
        {
            if (Configuration.firstShotType == FirstShotType.Match)
                SpreadPatches.UpdateSpreadScalar(0.2f);
            else if (Configuration.firstShotType == FirstShotType.Inner)
                FirstShotGuiPatches.Enable(forceGuiOn);
        }

        private static IEnumerator MinimizeAfterDelay()
        {
            // Adjust firstShotTime to keep the coroutine alive instead of making a new coroutine with every shot fired.
            while (firstShotTime > Clock.Time)
                yield return new WaitForSeconds(firstShotTime - Clock.Time);

            firstShotRoutine = null;
            EnableSmallCrosshair();
        }

        public static void SetStoredCrosshair(BulletWeapon weapon)
        {
            ArchetypeDataBlock? archetypeData = weapon.ArchetypeData;
            if (Configuration.firstShotType == FirstShotType.Match &&
                archetypeData.FireMode != 0 && archetypeData.ShotDelay < Configuration.firstShotMinDelay)
                return;

            fireRecoilCooldown = weapon.m_fireRecoilCooldown;
            firstShotTime = weapon.m_lastFireTime + fireRecoilCooldown;

            if (firstShotTime > Clock.Time)
                firstShotRoutine ??= CoroutineManager.StartCoroutine(MinimizeAfterDelay().WrapToIl2Cpp());
            else
            {
                // Called on prefix to ShowSpreadCircle, so need to force Gui on since the main Crosshair is not yet visible.
                EnableSmallCrosshair(true);
            }
        }

        public static void ResetStoredCrosshair()
        {
            if (firstShotRoutine != null)
            {
                CoroutineManager.StopCoroutine(firstShotRoutine);
                firstShotRoutine = null;
            }
            SpreadPatches.UpdateSpreadScalar(1f);
            FirstShotGuiPatches.Disable();
        }

        [HarmonyPatch(typeof(BulletWeapon), nameof(BulletWeapon.Fire))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void ResetFirstShotTimer()
        {
            firstShotTime = Clock.Time + fireRecoilCooldown;
            SpreadPatches.UpdateSpreadScalar(1f);
            FirstShotGuiPatches.Disable();
            firstShotRoutine ??= CoroutineManager.StartCoroutine(MinimizeAfterDelay().WrapToIl2Cpp());
        }
    }
}
