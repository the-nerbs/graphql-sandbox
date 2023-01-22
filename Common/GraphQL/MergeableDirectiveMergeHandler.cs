using HotChocolate.Language;
using HotChocolate.Stitching.Merge;

namespace Common.GraphQL;

class MergeableDirectiveMergeHandler : IDirectiveMergeHandler
{
    private MergeDirectiveRuleDelegate _nextHandler;


    public MergeableDirectiveMergeHandler(MergeDirectiveRuleDelegate nextHandler)
    {
        _nextHandler = nextHandler;
    }


    public void Merge(ISchemaMergeContext context, IReadOnlyList<IDirectiveTypeInfo> directives)
    {
        if (!directives.All(IsMergeableDirective))
        {
            _nextHandler?.Invoke(context, directives);
        }
    }

    private static bool IsMergeableDirective(IDirectiveTypeInfo info)
    {
        DirectiveDefinitionNode def = info.Definition;

        return def.Name.Value == MergeableDirective.DirectiveName
            && def.Arguments.Count == 0;
    }
}