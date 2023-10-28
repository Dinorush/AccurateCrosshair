using System.IO;
using AccurateCrosshair.CrosshairPatches;
using BepInEx;
using BepInEx.Configuration;

namespace AccurateCrosshair
{
    public static class Configuration
    {
        public static bool followsRecoil = true;
        public static FirstShotType firstShotType = FirstShotType.Inner;
        public static float firstShotMinDelay = 0.15f;
        public static bool popEnabled = false;
        public static float minSize = 10.0f;

        public static void CreateAndBindAll()
        {
            BindAll(new ConfigFile(Path.Combine(Paths.ConfigPath, "AccurateCrosshair.cfg"), saveOnInit: true));
        }

        private static void BindAll(ConfigFile config)
        {
            BindCrosshairPopEnabled(config);
        }

        private static void BindCrosshairPopEnabled(ConfigFile config)
        {
            string section = "General Settings";
            string key = "Follow Recoil";
            string description = "Enables or disables the crosshair moving to match weapon recoil that is independent of your aim.";
            followsRecoil = config.Bind(new ConfigDefinition(section, key), followsRecoil, new ConfigDescription(description, null)).Value;

            key = "First Shot Display Mode";
            description = "Determines how the crosshair shows the hidden bonus accuracy after waiting 2 seconds to shoot.\r\nInner: Shows another circle. Match: Reduces crosshair size to match. None: Turns this off.";
            firstShotType = config.Bind(new ConfigDefinition(section, key), firstShotType, new ConfigDescription(description, null)).Value;

            key = "First Shot Min Delay";
            description = "If First Shot Display Mode is set to Match, the minimum shot delay non-semi automatic weapons need for the first shot effect to appear.";
            firstShotMinDelay = config.Bind(new ConfigDefinition(section, key), firstShotMinDelay, new ConfigDescription(description, null)).Value;

            key = "Pop Enabled";
            description = "Enables or disables the visual crosshair bloom when shooting.\r\nNote: Pop is not scaled to the accurate size and may be disproportionately large.";
            popEnabled = config.Bind(new ConfigDefinition(section, key), popEnabled, new ConfigDescription(description, null)).Value;

            key = "Minimum Size";
            description = "The minimum size of the reticle. Does not scale with field of view.";
            minSize = config.Bind(new ConfigDefinition(section, key), minSize, new ConfigDescription(description, null)).Value;
        }
    }
}
