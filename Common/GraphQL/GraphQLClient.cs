using System.Linq.Expressions;
using System.Net.Http.Json;

namespace Common.GraphQL;

public class GraphQLClient
{
    private HttpClient _httpClient;
    private string _endpoint;


    public GraphQLClient(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }

    public Task<TResult> Query<TResult>(string query, Expression<Func<TResult>> resultTemplate)
    {
        return Query<TResult>(query);
    }

    public async Task<TResult> Query<TResult>(string query)
    {
        var request = new GraphQLRequest(null, query, new{ });

        var responseMessage = await _httpClient.PostAsync(_endpoint, JsonContent.Create(request));

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"GraphQL query returned failure status {(int)responseMessage.StatusCode}.");
        }

        var result = await responseMessage.Content.ReadFromJsonAsync<GraphQLResult<TResult>>();

        if (result is null)
        {
            throw new Exception("Unexpected null response.");
        }
        else if (result.HasErrors)
        {
            if (result.Errors!.Length == 1)
            {
                throw FromError(result.Errors[0]);
            }
            
            throw new AggregateException("GraphQL query returned multiple errors.", result.Errors.Select(FromError));
        }

        return result.Data!;
    }

    private static Exception FromError(GraphQLError error)
    {
        return new Exception(error.Message)
        {
            Data = {
                ["GraphQL.OriginalError"] = error
            }
        };
    }
}