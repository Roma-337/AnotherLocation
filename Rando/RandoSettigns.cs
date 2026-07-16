
namespace AnotherLocation.Rando
{
    public class RandoSettings
    {
        public bool BugPrince { get; set; }

        public static RandoSettings FromGlobal(GlobalSettings g)
        {
            return new RandoSettings
            {
                BugPrince = g.BugPrince
            };
        }

        public bool IsEnabled() => BugPrince;
    }
}