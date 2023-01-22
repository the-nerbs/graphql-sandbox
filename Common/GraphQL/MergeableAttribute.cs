using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace Common.GraphQL;

/// <summary>
/// Indicates that the GraphQL representation for this type should be decorated
/// with the `@mergeable` directive.
/// </summary>
/// <remarks>
/// This directive is useful for types that appear in shared libraries, and may
/// be exposed through multiple schemas that get stitched together later.
/// </remarks>
/// <seealso cref="GraphQLExtensions.AddMergeable" />
public sealed class MergeableAttribute : ObjectTypeDescriptorAttribute
{
    public override void OnConfigure(
        IDescriptorContext context,
        IObjectTypeDescriptor descriptor, 
        Type type)
    {
        descriptor.Directive<MergeableDirective>();
    }
}