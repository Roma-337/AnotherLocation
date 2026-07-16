using RandomizerMod.Settings;

namespace AnotherLocation.ConditionChecks
{
    //vanilla skip settings check
    internal static class ShriekPogoCheck
    {
        internal static bool ShouldEnableShriekPogoLocations(GenerationSettings gs)
        {
            return gs != null && gs.SkipSettings != null && gs.SkipSettings.ShriekPogos;
        }
    }
}