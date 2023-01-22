using HotChocolate.Types;

namespace Common.GraphQL;

class MergeableDirective : DirectiveType
{
    public const string DirectiveName = "mergeable";

    protected override void Configure(IDirectiveTypeDescriptor descriptor)
    {
        descriptor.Public();
        descriptor.Name(DirectiveName);
        descriptor.Location(DirectiveLocation.Object);
        descriptor.Description(
            "Indicates that the type this is applied to can be merged when performing schema "
            + "stitching. Useful for types defined in common libraries that may be published "
            + "by multiple schemas."
        );
    }
}