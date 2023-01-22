using HotChocolate;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.GraphQL;

/// <summary>
/// Extensions for the GraphQL execution pipeline.
/// </summary>
public static class GraphQLExtensions
{
    /// <summary>
    /// Adds the `@mergeable` directive and support for it.
    /// </summary>
    public static IRequestExecutorBuilder AddMergeable(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddDirectiveType<MergeableDirective>()
            .AddDirectiveMergeHandler<MergeableDirectiveMergeHandler>()
            .AddTypeMergeHandler<MergeableTypeMergeHandler>();
    }
}