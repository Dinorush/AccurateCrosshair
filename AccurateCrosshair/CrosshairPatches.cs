using GameData;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Player;
using System;

namespace AccurateCrosshair
{
    internal class CrosshairPatches
    {
        private const float BASE_CROSSHAIR_SIZE = 20.0f;   // The base size (from testing) that the crosshair should be at 90 FOV (when tan(FoV/2) = 1)
        private const float EXTRA_BUFFER_SIZE = 10.0f; // Flat value added to account for the actual reticle, so as to not cover possible shot locations.

        private static PlayerAgent? cachedPlayer = null;

        [HarmonyPatch(typeof(GameDataInit), nameof(GameDataInit.Initialize))]
        [HarmonyPostfix]
        private static void RemovePopFromDatablocks()
        {
            if (Configuration.crosshairPopEnabled)
                return;

            foreach (var block in RecoilDataBlock.Wrapper.Blocks)
            {
                block.hipFireCrosshairRecoilPop = 0;
            }
        }

        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.ShowSpreadCircle))]
        [HarmonyPrefix]
        private static void AdjustCrosshairSize(CrosshairGuiLayer __instance, ref float crosshairSize)
        {
            cachedPlayer ??= PlayerManager.GetLocalPlayerAgent();
            float playerFoV = CellSettingsManager.SettingsData.Video.Fov.Value;

            ItemEquippable heldItem = cachedPlayer.Inventory.m_wieldedItem;

            bool isShotgun = false;
            ItemDataBlock itemData = heldItem.ItemDataBlock;
            if (itemData != null)
            {
                List<string> prefabs = itemData.FirstPersonPrefabs;
                // Hardcoded check for Shotgun weapons since type comparison with assignable from didn't appear to work
                if (prefabs.Count > 0 && prefabs[0] == "Assets/AssetPrefabs/Items/Weapons/GearSetup/ShotgunWeaponFirstPerson.prefab")
                {
                    isShotgun = true;
                }
            }

            ArchetypeDataBlock archetypeData = heldItem.ArchetypeData;
            if (archetypeData == null)
                return;

            float sizeMultiplier = (float)(BASE_CROSSHAIR_SIZE / Math.Tan(Math.PI / 180.0 * playerFoV / 2));
            float spread = isShotgun ? archetypeData.ShotgunConeSize + archetypeData.ShotgunBulletSpread : archetypeData.HipFireSpread;
            float newSize = Math.Max(Configuration.crosshairMinSize, spread * sizeMultiplier + EXTRA_BUFFER_SIZE);

            __instance.m_dotSize = (crosshairSize = newSize);
        }
    }
}