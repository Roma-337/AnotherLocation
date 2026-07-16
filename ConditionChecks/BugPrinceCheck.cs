using System;
using System.Reflection;
using Modding;

namespace AnotherLocation.ConditionChecks
{
    internal static class BugPrinceCheck
    {
        internal static bool ShouldEnableBugPrinceAdvancedLocation()
        {
            return AnotherLocation.GlobalSettings.BugPrince
                && IsInstalled()
                && AdvancedLocationsEnabled();
        }

        internal static bool IsInstalled()
        {
            return ModHooks.GetMod("BugPrince") is Mod;
        }

        internal static bool AdvancedLocationsEnabled()
        {
            return TryGetSettings(out var advancedLocations) && advancedLocations;
        }

        internal static bool TryGetSettings(out bool advancedLocations)
        {
            advancedLocations = false;

            try
            {
                if (ModHooks.GetMod("BugPrince") is not Mod mod)
                    return false;

                var modType = mod.GetType();
                var rsProp = modType.GetProperty("RS", BindingFlags.Public | BindingFlags.Static);
                var randoSettings = rsProp?.GetValue(null);
                if (randoSettings == null)
                    return false;

                var settingsType = randoSettings.GetType();
                var advField = settingsType.GetField("AdvancedLocations");
                if (advField?.GetValue(randoSettings) is bool adv)
                {
                    advancedLocations = adv;
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}