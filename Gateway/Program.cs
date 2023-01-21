using HotChocolate.AspNetCore;
using Gateway;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient("alpha", c => c.BaseAddress = new Uri("http://alpha:80/graphql"));
builder.Services.AddHttpClient("bravo", c => c.BaseAddress = new Uri("http://bravo:80/graphql"));

builder.Services.AddGraphQLServer()
    //.AddQueryType(d => d.Name("Query"))
    .AddQueryType<Query>()
    .AddRemoteSchema("alpha")
    .AddRemoteSchema("bravo");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.UseGraphQLAltair();

app.Run();
