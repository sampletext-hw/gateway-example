namespace salary_api;

public static class GetSalary
{
    public record Response(IEnumerable<Item> Items);

    public record Item(string Employee, string Rank, float SalaryUsd);
}