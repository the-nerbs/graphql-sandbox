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
        var xFields = x.Fields.OrderBy(f => f.Name.Value).ToArray();
        var yFields = y.Fields.OrderBy(f => f.Name.Value).ToArray();

        bool areSame = xFields.Length == yFields.Length;

        if (areSame)
        {
            for (int i = 0; areSame && i < xFields.Length; i++)
            {
                FieldDefinitionNode xf = xFields[i];
                FieldDefinitionNode yf = yFields[i];

                // note: GraphQL field names are case-sensitive
                areSame = xf.Name.Value == yf.Name.Value
                       && AreSameType(xf.Type, yf.Type);
            }
        }

        return areSame;
    }

    private static bool AreSameType(ITypeNode x, ITypeNode y)
    {
        if (x is ListTypeNode xList)
        {
            return y is ListTypeNode yList
                && AreSameType(xList.Type, yList.Type);
        }
        else if (x is NamedTypeNode xNamed)
        {
            // note: GraphQL types are case sensitive
            return y is NamedTypeNode yNamed
                && xNamed.Name.Value == yNamed.Name.Value;
        }
        else if (x is NonNullTypeNode xNotNull)
        {
            return y is NonNullTypeNode yNotNull
                && AreSameType(xNotNull.Type, yNotNull.Type);
        }
        
        throw new ArgumentException($"Unexpected type node kind {x.GetType().FullName}");
    }
}