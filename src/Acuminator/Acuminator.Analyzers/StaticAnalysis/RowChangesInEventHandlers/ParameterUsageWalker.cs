﻿using System.Collections.Immutable;
using Acuminator.Utilities.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Acuminator.Analyzers.StaticAnalysis.RowChangesInEventHandlers
{
	public partial class RowChangesInEventHandlersAnalyzer
	{
		/// <summary>
		/// Searches for usages of a method parameter
		/// </summary>
		private class ParameterUsageWalker : CSharpSyntaxWalker
		{
			protected IParameterSymbol Parameter { get; private set; }
			protected SemanticModel SemanticModel { get; private set; }

			public bool Success { get; protected set; }

			public ParameterUsageWalker(IParameterSymbol parameter, SemanticModel semanticModel)
			{
				semanticModel.ThrowOnNull(nameof(semanticModel));

				Parameter = parameter;
				SemanticModel = semanticModel;
			}

			public override void Visit(SyntaxNode node)
			{
				if (!Success)
					base.Visit(node);
			}

			public override void VisitIdentifierName(IdentifierNameSyntax node)
			{
				if (Parameter != null && node != null
					&& SemanticModel.GetSymbolInfo(node).Symbol is IParameterSymbol parameter
					&& Parameter.Equals(parameter))
				{
					Success = true;
				}
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
			}

			public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
			{
			}
		}

	}
}
