This repository contains a small reproduction of an issue I encountered using the HotChocolate
GraphQL Server to stitch together GraphQL schemas from multiple services to create a single API
gateway service.


## The Issue

While I am still not sure of the exact root cause of the issue, I'll try my best to explain how it
manifests.

When a common datatype is published separately by multiple services ("Alpha" and "Bravo" here), and
then the same type is published by a gateway that stitches the others together, HotChocolate throws
an error trying to stitch them together similar to:
```plaintext
1. The name `ComponentInfo` was already registered by another type. (HotChocolate.Types.ObjectType<ComponentInfo>)

   at HotChocolate.Configuration.TypeRegistry.Register(NameString typeName, RegisteredType registeredType)
   at HotChocolate.Configuration.TypeInitializer.CompleteTypeName(RegisteredType registeredType)
   at HotChocolate.Configuration.TypeInitializer.ProcessTypes(TypeDependencyKind kind, Func`2 action)
   at HotChocolate.Configuration.TypeInitializer.CompleteNames()
   at HotChocolate.Configuration.TypeInitializer.Initialize()
   at HotChocolate.SchemaBuilder.Setup.InitializeTypes(SchemaBuilder builder, IDescriptorContext context, IReadOnlyList`1 types, LazySchema lazySchema)
   at HotChocolate.SchemaBuilder.Setup.Create(SchemaBuilder builder, LazySchema lazySchema, IDescriptorContext context)
   at HotChocolate.SchemaBuilder.Create(IDescriptorContext context)
   at HotChocolate.SchemaBuilder.HotChocolate.ISchemaBuilder.Create(IDescriptorContext context)
   at HotChocolate.Execution.RequestExecutorResolver.CreateSchemaAsync(NameString schemaName, RequestExecutorSetup options, RequestExecutorOptions executorOptions, IServiceProvider serviceProvider, TypeModuleChangeMonitor typeModuleChangeMonitor, CancellationToken cancellationToken)
   at HotChocolate.Execution.RequestExecutorResolver.CreateSchemaServicesAsync(NameString schemaName, RequestExecutorSetup options, CancellationToken cancellationToken)
   at HotChocolate.Execution.RequestExecutorResolver.GetRequestExecutorNoLockAsync(NameString schemaName, CancellationToken cancellationToken)
   at HotChocolate.Execution.RequestExecutorResolver.GetRequestExecutorAsync(NameString schemaName, CancellationToken cancellationToken)
   at HotChocolate.Execution.RequestExecutorProxy.GetRequestExecutorAsync(CancellationToken cancellationToken)
   at HotChocolate.AspNetCore.HttpPostMiddlewareBase.HandleRequestAsync(HttpContext context, AllowedContentType contentType)
   at HotChocolate.AspNetCore.HttpPostMiddlewareBase.InvokeAsync(HttpContext context)
   at Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.<>c__DisplayClass13_0.<<UseCancellation>b__1>d.MoveNext()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
```

When figuring out through the workaround here, I saw that the order things happen is:
  1. The gateway's `ComponentInfo` is published.
  2. A request is made to the Gateway, causing HotChocolate to pull the stitched schemas.
  3. The gateway receives a schema containing its own `ComponentInfo`.
  4. HotChocolate invokes any registered `ITypeMergeHandler`s to merge the `ComponentInfo` from the
     remote schema with the one from the gateway.
  5. None of the default handlers can merge the duplicated types.
  6. A `SchemaError` is thrown.


## The workaround

The workaround implemented here is to add a new directive (`@mergeable`) and then support that in
the gateway (at a minimum) to merge types with the same names and structures.

This new directive is configured by the class `MergeableDirective`, and an attribute
(`MergeableAttribute`) is provided for .NET types that get exposed to GraphQL. This directive is
then used by the `MergeableTypeMergeHandler`, which will merge any types decorated with this
directive that have the same name and fields (names + types).
