using Common;
using Common.GraphQL;

namespace Gateway;

public record SystemInfo(ComponentInfo[] components);

public class Query
{
    IHttpClientFactory _httpClientFactory;


    public Query(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<SystemInfo> GetSystemInfoAsync()
    {
        var components = await Task.WhenAll(
            Task.FromResult(new ComponentInfo("gateway", "1.1.1.1")),
            QueryServiceInfoAsync("alpha", "http://alpha:80/graphql"),
            QueryServiceInfoAsync("bravo", "http://bravo:80/graphql")
        );

        return new SystemInfo(components);
    }


    private async Task<ComponentInfo> QueryServiceInfoAsync(string schema, string endpoint)
    {
        var httpClient = _httpClientFactory.CreateClient(schema);
        var graphQLClient = new GraphQLClient(httpClient, endpoint);

        var data = await graphQLClient.Query(
            @"query { componentInfo { name version } }",
            () => new { componentInfo = default(ComponentInfo)! }
        );

        return data.componentInfo;
    }
}