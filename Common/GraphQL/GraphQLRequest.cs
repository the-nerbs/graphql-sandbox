namespace Common.GraphQL;

record GraphQLRequest(string? operationName, string query, object variables);
