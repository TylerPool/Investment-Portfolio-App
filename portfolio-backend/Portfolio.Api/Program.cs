using Portfolio.Api.Contracts;
using Portfolio.Api.Models;
using Portfolio.Api.Services;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "AllowAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddSingleton<PortfolioStore>();

var app = builder.Build();
app.UseCors(CorsPolicy);

// Optional: fix port for predictability
app.Urls.Add("http://localhost:5200");

#region Portfolio and Account Data
app.MapGet("/api/portfolio/{id}",(PortfolioStore store, string id) =>
{
    var portfolio = store.GetPortfolio(id);
    return portfolio is null ? Results.NotFound() : Results.Ok(portfolio);
});

app.MapPost("/api/account", (PortfolioStore store, AccountDto accountDto) =>
{
    var account = accountDto.ToModel();
    store.SaveAccount(account);
    return Results.Accepted();
});

app.MapGet("api/account/{id}", (PortfolioStore store, string id) =>
{
    var account = store.GetAccount(id);
    return Results.Ok(account);
});

app.MapPost("/api/accounts/test", (PortfolioStore store) =>
{
    store.SaveAccountTest();
    return Results.Accepted();
});
#endregion

#region Stock Market Data
app.MapGet("/api/quotes/{symbol}", (PortfolioStore store, string symbol) =>
{
    var price = store.GetQuote(symbol);
    return price is null ? Results.NotFound() : Results.Ok(new { symbol = symbol.ToUpperInvariant(), price });
});
#endregion

#region Testing
app.MapGet("/api/health", () => Results.Ok(new { ok = true }));
#endregion

app.Run("http://localhost:5200");
