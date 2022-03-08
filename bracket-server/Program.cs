using MillerAPI;
using MillerAPI.DataAccess;
using bracket_server.Routing;
using UserManagement.UserDataAccess;
using bracket_server.Tournaments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IDataAccess, DataAccessor>();
builder.Services.AddSingleton<IUserDAL, UserDAL>();
builder.Services.AddSingleton<ITournamentDAL, TournamentDAL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();


UserEndpoints user_endpoints = new UserEndpoints();
BracketEndpoints bracket_endpoints = new BracketEndpoints();
TournamentEndpoints tournament_endpoints = new TournamentEndpoints();
user_endpoints.RegisterRoutes(app);
bracket_endpoints.RegisterRoutes(app);
tournament_endpoints.RegisterRoutes(app);

app.Run();


internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

