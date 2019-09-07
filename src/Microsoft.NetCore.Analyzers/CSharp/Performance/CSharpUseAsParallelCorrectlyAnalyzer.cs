using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.NetCore.Analyzers.Performance;

namespace Microsoft.NetCore.CSharp.Analyzers.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CSharpUseAsParallelCorrectlyAnalyzer : UseAsParallelCorrectlyAnalyzer
    {
        protected override void RegisterDiagnosticAction(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(NodeAction, SyntaxKind.ForEachStatement);
        }

        private static void NodeAction(SyntaxNodeAnalysisContext context)
        {
            var foreachStatement = (ForEachStatementSyntax) context.Node;

            if (!(foreachStatement.Expression is InvocationExpressionSyntax invocationExpression))
            {
                return;
            }

            if (!(invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpression))
            {
                return;
            }

            var methodSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpression.Name);

            var parallelEnumerableType = context.Compilation.GetTypeByMetadataName("System.Linq.ParallelEnumerable");

            var isFromParallelEnumerable = methodSymbol.Symbol.ContainingType.Equals(parallelEnumerableType);

            if (!isFromParallelEnumerable ||
                !string.Equals(methodSymbol.Symbol.Name, "AsParallel", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            ReportDiagnostic(context, memberAccessExpression.Name.GetLocation());
        }
    }
}