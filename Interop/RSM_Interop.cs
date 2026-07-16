using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using AnotherLocation.Rando;

namespace AnotherLocation.Interop
{
    internal static class RSM_Interop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new AnotherLocationSettingsProxy());
        }
    }

    internal class AnotherLocationSettingsProxy : RandoSettingsProxy<RandoSettings, string>
    {
        public override string ModKey => AnotherLocation.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(AnotherLocation.Instance.GetVersion());

        public override bool TryProvideSettings(out RandoSettings settings)
        {
            settings = RandoSettings.FromGlobal(AnotherLocation.GlobalSettings);
            return true;
        }

        public override void ReceiveSettings(RandoSettings settings)
        {
            if (settings != null)
            {
                AnotherLocation.GlobalSettings.BugPrince = settings.BugPrince;
                ConnectionMenu.Instance?.Apply(settings);
            }
            else
            {
                ConnectionMenu.Instance?.Disable();
            }
        }
    }
}