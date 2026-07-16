namespace AnotherLocation.Rando
{
    internal static class RandoMenuProxy
    {
        public static RandoSettings RS
        {
            get => new RandoSettings
            {
                BugPrince = AnotherLocation.GlobalSettings.BugPrince
            };
            set
            {
                AnotherLocation.GlobalSettings.BugPrince = value.BugPrince;
            }
        }
    }
}