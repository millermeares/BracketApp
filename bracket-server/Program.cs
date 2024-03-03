using MillerAPI;
using MillerAPI.DataAccess;
using bracket_server.Routing;
using UserManagement.UserDataAccess;
using bracket_server.Tournaments;

var builder = WebApplication.CreateBuilder(args);

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


foreach(EndpointManager endpoints in EndpointManager.GetEndpointMangers())
{
    endpoints.RegisterRoutes(app);
}

app.Run();
