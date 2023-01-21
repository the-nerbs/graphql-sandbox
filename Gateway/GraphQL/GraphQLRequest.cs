namespace Gateway.GraphQL;

record GraphQLRequest(string? operationName, string query, object variables);
