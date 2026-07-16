using ItemChanger.Locations;

namespace AnotherLocation.LocationSpecifics
{
    internal class ShinyLocation : CoordinateLocation
    {
        public ShinyLocation(string internalName, string scene, float x, float y)
        {
            name = internalName;
            sceneName = scene;
            this.x = x;
            this.y = y;
            elevation = 0;
        }
    }
}