namespace weather_api;

public static class GetWeather
{
    public record Response(IEnumerable<Item> Items);

    public record Item(string City, DateTime Date, float Temperature);
}