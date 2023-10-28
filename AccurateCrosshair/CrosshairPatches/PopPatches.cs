using GameData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccurateCrosshair.CrosshairPatches
{
    internal class PopPatches
    {
        [HarmonyPatch(typeof(CrosshairGuiLayer), nameof(CrosshairGuiLayer.PopCircleCrosshair))]
        [HarmonyPrefix]
        public static bool CancelRecoilPop(CrosshairGuiLayer __instance, float pop, float sizeMax)
        {
            return false;
        }
    }
}
