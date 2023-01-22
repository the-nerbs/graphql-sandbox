using HotChocolate.AspNetCore;
using HotChocolate.Stitching;
using ServiceA;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .PublishSchemaDefinition(opts =>
    {
        opts.SetName("alpha");
        opts.IgnoreRootTypes();
        opts.AddTypeExtensionsFromString(@"extend type Query {
    alphaData: [AlphaData!]! @delegate(schema: ""alpha"", path: ""data"")
    alphaInfo: ComponentInfo! @delegate(schema: ""alpha"", path: ""componentInfo"")
}");
    })
    ;

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
