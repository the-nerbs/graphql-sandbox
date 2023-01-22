using ServiceB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .PublishSchemaDefinition(opts =>
    {
        opts.SetName("bravo");
        opts.IgnoreRootTypes();
        opts.AddTypeExtensionsFromString(@"extend type Query {
    bravoData: [BravoData!]! @delegate(schema: ""bravo"", path: ""data"")
    bravoInfo: ComponentInfo! @delegate(schema: ""bravo"", path: ""componentInfo"")
}");
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

app.Run();
