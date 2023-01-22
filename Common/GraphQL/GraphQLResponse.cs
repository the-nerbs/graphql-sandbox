using System.Text.Json.Serialization;

namespace Common.GraphQL;

record GraphQLErrorLocation(int Line, int Column);
record GraphQLError(string Message, GraphQLErrorLocation[] locations, object[] path);

class GraphQLResult<TData>
{
    public TData? Data { get; set; }
    public GraphQLError[]? Errors { get; set; }


    public bool HasErrors
    {
        get { return Errors is not null && Errors.Length > 0; }
    }
}