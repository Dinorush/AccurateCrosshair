using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace AccurateCrosshair
{
    public static class Configuration
    {
        public static bool crosshairPopEnabled = false;
        public static float crosshairMinSize = 10.0f;

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
            string key = "CrosshairPopEnabled";
            bool defaultPop = false;
            string description = "Enables or disables the visual crosshair bloom when shooting.\r\nNote: Pop is not scaled to the accurate size and may be disproportionately large.";
            crosshairPopEnabled = config.Bind(new ConfigDefinition(section, key), defaultPop, new ConfigDescription(description, null)).Value;

            key = "CrosshairMinSize";
            float defaultMinSize = 10.0f;
            description = "The minimum size of the reticle. Does not scale with field of view.";
            crosshairMinSize = config.Bind(new ConfigDefinition(section, key), defaultMinSize, new ConfigDescription(description, null)).Value;
        }
    }
}
