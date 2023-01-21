namespace Gateway.GraphQL;

class GraphQLClient
{
    private HttpClient _httpClient;
    private string _endpoint;


    public GraphQLClient(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }


    public async Task<TResult> Query<TResult>(string query)
    {
        var request = new GraphQLRequest(null, query, new{ });

        var responseMessage = await _httpClient.PostAsync(_endpoint, JsonContent.Create(request));

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"GraphQL query returned failure status {(int)responseMessage.StatusCode}.");
        }

        return await responseMessage.Content.ReadFromJsonAsync<TResult>()!;
    }
}