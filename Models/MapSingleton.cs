using Maui.GoogleMaps;

public sealed class MapSingleton
{
    // Singleton instance of the Map
    private static readonly Lazy<MapSingleton> instance = new(() => new MapSingleton());

    public Maui.GoogleMaps.Map myMap { get; private set; }

    // Private constructor to prevent external instantiation
    private MapSingleton()
    {
        myMap = new Maui.GoogleMaps.Map();
    }

    // Public static property to get the singleton instance
    public static MapSingleton Instance => instance.Value;
}
