// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Microsoft.NetCore.Analyzers.Performance
{
    public abstract class MutableStructsShouldNotBeUsedForReadonlyFieldsFixer<TFieldDeclarationSyntax> : CodeFixProvider
        where TFieldDeclarationSyntax : SyntaxNode
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer.RuleId);

        public override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic == null)
            {
                return;
            }

            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var targetNode = root.FindNode(context.Span).FirstAncestorOrSelf<TFieldDeclarationSyntax>();

            if (targetNode == null)
            {
                return;
            }

            var removeReadonlyAction = CodeAction.Create(
                MicrosoftNetCoreAnalyzersResources.MutableStructsShouldNotBeUsedForReadonlyFieldsTitle,
                async ct => await RemoveReadonlyKeyword(context, root, targetNode).ConfigureAwait(false),
                "MutableStructsShouldNotBeUserForReadonlyFields");

            context.RegisterCodeFix(removeReadonlyAction, context.Diagnostics);
        }

        private static Task<Document> RemoveReadonlyKeyword(CodeFixContext context, SyntaxNode root, TFieldDeclarationSyntax targetNode)
        {
            var generator = SyntaxGenerator.GetGenerator(context.Document);
            var nodeWithoutReadonly = generator.WithModifiers(targetNode, generator.GetModifiers(targetNode).WithIsReadOnly(false));

            var newRoot = root.ReplaceNode(targetNode, nodeWithoutReadonly);

            return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
        }
    }
}