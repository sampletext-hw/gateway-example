namespace gateway_api.Models.Weather;

public static partial class GetWeather
{
    public record Response(IEnumerable<Item> Items);

    public record Item(string City, DateTime Date, float Temperature);
}