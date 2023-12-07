using BepInEx.Configuration;

namespace BetterClock
{
    internal static class Settings
    {
        internal static ConfigEntry<bool> compact;
        internal static ConfigEntry<bool> leadingZero;
        internal static ConfigEntry<bool> darkZero;
        internal static ConfigEntry<bool> fasterUpdate;
        internal static ConfigEntry<bool> raiseClock;
        internal static ConfigEntry<bool> hours24;
        internal static ConfigEntry<bool> properTime;

        internal static ConfigEntry<float> visibilityShip;
        internal static ConfigEntry<float> visibilityOutside;
        internal static ConfigEntry<float> visibilityInside;
        internal static ConfigEntry<float> visibilityOverride;

        internal static ConfigEntry<KeyboardShortcut> overrideKeybind;
        internal static ConfigEntry<bool> overrideToggle;

        public static void InitConfig(ConfigFile config)
        {
            compact = config.Bind("Clock", "CompactClock", true, "Makes the clock more compact");
            leadingZero = config.Bind("Clock", "LeadingZero", true, "Adds a leading zero to hours before 10");
            darkZero = config.Bind("Clock", "DarkZero", true, "Leading zeros are dark");
            fasterUpdate = config.Bind("Clock", "FasterUpdate", true, "Update the clock more often");
            raiseClock = config.Bind("Clock", "RaiseClock", true, "Raise the clock near the top of the screen");
            hours24 = config.Bind("Clock", "24Hours", false, "Use 24 hour time");
            properTime = config.Bind("Clock", "ProperTime", true, "Fix time formatting caused by the game or other mods");

            visibilityShip = config.Bind("Clock", "VisibilityShip", 1f, "Visibility of clock inside ship");
            visibilityOutside = config.Bind("Clock", "VisibilityOutside", 1f, "Visibility of clock outside");
            visibilityInside = config.Bind("Clock", "VisibilityInside", 0.25f, "Visibility of clock inside factory");
            visibilityOverride = config.Bind("Clock", "VisibilityOverride", 1f, "Visibility when using override keybind");

            overrideKeybind = config.Bind("Clock", "OverrideKeybind", KeyboardShortcut.Empty, "Keybind to trigger visibility override");
            overrideToggle = config.Bind("Clock", "OverrideToggle", false, "Switch the override keybind between toggle or hold");
        }
    }
}
