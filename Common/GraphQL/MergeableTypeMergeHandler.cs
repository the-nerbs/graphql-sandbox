using HotChocolate.Language;
using HotChocolate.Stitching.Merge;

namespace Common.GraphQL;

class MergeableTypeMergeHandler : ITypeMergeHandler
{
    private MergeTypeRuleDelegate _nextHandler;


    public MergeableTypeMergeHandler(MergeTypeRuleDelegate nextHandler)
    {
        _nextHandler = nextHandler;
    }


    public void Merge(ISchemaMergeContext context, IReadOnlyList<ITypeInfo> types)
    {
        List<ITypeInfo> mergeableTypes = types
            .Where(t => t.Definition is ObjectTypeDefinitionNode)
            .Where(t => t.Definition.Directives.Any(d => d.Name.Value == MergeableDirective.DirectiveName))
            .ToList();

        List<ITypeInfo> mergeResults = new();
        List<ITypeInfo> merged = new();

        if (mergeableTypes.Count > 0)
        {
            while (mergeableTypes.Count > 0)
            {
                var type = mergeableTypes[0];
                mergeResults.Add(type);
                merged.Add(type);

                for (int i = mergeableTypes.Count - 1; i > 0; i--)
                {
                    if (AreSameShape(type, mergeableTypes[i]))
                    {
                        merged.Add(mergeableTypes[i]);
                        mergeableTypes.RemoveAt(i);
                    }
                }

                mergeableTypes.RemoveAt(0);
            }
        }

        var remainder = types.Except(merged).ToArray();
        if (remainder.Length > 0)
        {
            _nextHandler?.Invoke(context, remainder);
        }
    }


    private static bool AreSameShape(ITypeInfo x, ITypeInfo y)
    {
        return AreSameShape(
            (ObjectTypeDefinitionNode)x.Definition,
            (ObjectTypeDefinitionNode)y.Definition
        );
    }

    private static bool AreSameShape(ObjectTypeDefinitionNode x, ObjectTypeDefinitionNode y)
    {
        //TODO: actually check that these types are the same
        return true;
    }
}