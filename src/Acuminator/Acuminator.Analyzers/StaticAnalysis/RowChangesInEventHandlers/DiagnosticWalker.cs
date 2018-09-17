﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Acuminator.Utilities.Common;
using Acuminator.Utilities.Roslyn;
using Acuminator.Utilities.Roslyn.Semantic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Acuminator.Analyzers.StaticAnalysis.RowChangesInEventHandlers
{
	public partial class RowChangesInEventHandlersAnalyzer
	{
		private class DiagnosticWalker : NestedInvocationWalker
		{
			private static readonly ISet<string> MethodNames = new HashSet<string>(StringComparer.Ordinal)
			{
				"SetValue" ,
				"SetValueExt",
				"SetDefaultExt",
			};

			private readonly SymbolAnalysisContext _context;
			private readonly SemanticModel _semanticModel;
			private readonly PXContext _pxContext;
			private readonly Func<CSharpSyntaxNode, bool> _predicate;
			private readonly ImmutableHashSet<ILocalSymbol> _rowVariables;
			private readonly object[] _messageArgs;

			public DiagnosticWalker(SymbolAnalysisContext context, SemanticModel semanticModel, PXContext pxContext, 
				ImmutableArray<ILocalSymbol> rowVariables, // variables which were assigned with e.Row
				Func<CSharpSyntaxNode, bool> predicate,
				params object[] messageArgs)
				:base(context.Compilation, context.CancellationToken)
			{
				pxContext.ThrowOnNull(nameof (pxContext));

				_context = context;
				_semanticModel = semanticModel;
				_pxContext = pxContext;
				_predicate = predicate;
				_rowVariables = rowVariables.ToImmutableHashSet();
				_messageArgs = messageArgs;
			}

			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				_context.CancellationToken.ThrowIfCancellationRequested();

				var methodSymbol = _semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

				if (methodSymbol != null && IsMethodForbidden(methodSymbol))
				{
					bool found = node.ArgumentList.Arguments
						.Where(arg => arg.Expression != null)
						.Select(arg => _semanticModel.GetSymbolInfo(arg.Expression).Symbol as ILocalSymbol)
						.Any(variable => variable != null && _rowVariables.Contains(variable));

					if (!found)
					{
						found = _predicate(node.ArgumentList);
					}

					if (found)
					{
						ReportDiagnostic(_context.ReportDiagnostic, Descriptors.PX1047_RowChangesInEventHandlers, node, _messageArgs);
					}
				}
				else // TODO: add condition (go to external method only if acceps e.Row as an argument)
				{
					base.VisitInvocationExpression(node);
				}
			}

			public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
			{
				if (node.Left != null)
				{
					bool found = _predicate(node.Left);

					if (!found)
					{
						var varWalker = new VariableMemberAccessWalker(_rowVariables, _semanticModel);
						node.Left.Accept(varWalker);
						found = varWalker.Success;
					}
					
					if (found)
					{
						ReportDiagnostic(_context.ReportDiagnostic, Descriptors.PX1047_RowChangesInEventHandlers, node, _messageArgs);
					}
				}
			}

			public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
			{
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
			}

			public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
			{
			}


			private bool IsMethodForbidden(IMethodSymbol symbol)
			{
				return symbol.ContainingType?.OriginalDefinition != null
				       && symbol.ContainingType.OriginalDefinition.InheritsFromOrEquals(_pxContext.PXCacheType)
				       && MethodNames.Contains(symbol.Name);
			}
		}

	}
}
