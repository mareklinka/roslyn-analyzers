using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.NetCore.Analyzers;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharpExtensions;

namespace Microsoft.NetCore.CSharp.Analyzers.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp), Shared]
    public sealed class CSharpUseAsParallelCorrectlyFixer : NetCore.Analyzers.Performance.UseAsParallelCorrectlyFixer
    {
        protected override void AnalyzeCodeFix(CodeFixContext context, SyntaxNode targetNode)
        {
            var invocationExpression = targetNode.FirstAncestorOrSelf<InvocationExpressionSyntax>();

            var removeAsParallelAction = CodeAction.Create(
                "Use AsParallel correctly",
                async ct => await RemoveAsParallelInvocation(context, invocationExpression).ConfigureAwait(false),
                EquivalencyKey);

            context.RegisterCodeFix(removeAsParallelAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveAsParallelInvocation(CodeFixContext context,
            InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, context.CancellationToken).ConfigureAwait(false);

            if (invocationExpressionSyntax.ArgumentList.Arguments.Any())
            {
                // static invocation
                editor.ReplaceNode(invocationExpressionSyntax, invocationExpressionSyntax.ArgumentList.Arguments.First().Expression);
            }
            else
            {
                // extension method invocation
                editor.ReplaceNode(invocationExpressionSyntax, invocationExpressionSyntax.Expression.ChildNodes().First());
            }

            return editor.GetChangedDocument();
        }
    }
}