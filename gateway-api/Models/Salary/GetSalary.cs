namespace gateway_api.Models.Salary;

public class GetSalary
{
    public record Response(IEnumerable<Item> Items);

    public record Item(string Employee, string Rank, float SalaryUsd);
}