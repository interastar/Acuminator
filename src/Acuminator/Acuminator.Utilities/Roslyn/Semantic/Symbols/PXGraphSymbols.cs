﻿using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Acuminator.Utilities.Roslyn.Semantic.Symbols
{
    public class PXGraphSymbols
    {
		private const string InitCacheMappingMethodName = nameof(PX.Data.PXGraph.InitCacheMapping);

		public class InstanceCreatedEventsSymbols
	    {
		    public INamedTypeSymbol Type { get; }
			public IMethodSymbol AddHandler { get; }

		    internal InstanceCreatedEventsSymbols(Compilation compilation)
		    {
			    Type = compilation.GetTypeByMetadataName(typeof(PX.Data.PXGraph.InstanceCreatedEvents).FullName);
				AddHandler = Type.GetMethods(nameof(PX.Data.PXGraph.InstanceCreatedEvents.AddHandler)).First();
		    }
	    }

        public INamedTypeSymbol Type { get; }
		public INamedTypeSymbol GenericTypeGraph { get; }
		public INamedTypeSymbol GenericTypeGraphDac { get; }
		public INamedTypeSymbol GenericTypeGraphDacField { get; }

		public ImmutableArray<IMethodSymbol> CreateInstance { get; }

	    public InstanceCreatedEventsSymbols InstanceCreatedEvents { get; }

		public IMethodSymbol InitCacheMapping => Type.GetMembers(InitCacheMappingMethodName)
													 .OfType<IMethodSymbol>()
													 .FirstOrDefault(method => method.ReturnsVoid && method.Parameters.Length == 1);

		internal PXGraphSymbols(Compilation compilation)
        {
            Type = compilation.GetTypeByMetadataName(typeof(PX.Data.PXGraph).FullName);
			GenericTypeGraph = compilation.GetTypeByMetadataName(typeof(PX.Data.PXGraph<>).FullName);
			GenericTypeGraphDac = compilation.GetTypeByMetadataName(typeof(PX.Data.PXGraph<,>).FullName);
			GenericTypeGraphDacField = compilation.GetTypeByMetadataName(typeof(PX.Data.PXGraph<,,>).FullName);

			CreateInstance = Type.GetMethods(nameof(PX.Data.PXGraph.CreateInstance));
			InstanceCreatedEvents = new InstanceCreatedEventsSymbols(compilation);
        }
    }
}
