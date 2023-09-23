using salary_api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

var employees = new[] { "Nick", "Max", "Linkoln", "Daruk", "Migel", "Viktor", "Gadot", "Michael", "John"};
var ranks = new[] { "Engineer", "Developer", "Accountant", "Cleaner", "Director", "Intern" };

app.MapGet(
    "/salary/get",
    () => new GetSalary.Response(
        Enumerable.Range(0, 5)
            .Select(x => new GetSalary.Item(employees[Random.Shared.Next(employees.Length)], ranks[Random.Shared.Next(ranks.Length)], Random.Shared.NextSingle() * 5000))
    )
);

app.UseHealthChecks("/health-check");

app.Run();