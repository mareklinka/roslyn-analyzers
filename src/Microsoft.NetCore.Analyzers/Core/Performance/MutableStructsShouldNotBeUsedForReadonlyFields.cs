// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Analyzer.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.NetCore.Analyzers.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString s_localizableTitle =
            new LocalizableResourceString(
                nameof(MicrosoftNetCoreAnalyzersResources.MutableStructsShouldNotBeUsedForReadonlyFieldsTitle),
                MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));

        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.MutableStructsShouldNotBeUsedForReadonlyFieldsMessage), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));
        private static readonly LocalizableString s_localizableDescription = new LocalizableResourceString(nameof(MicrosoftNetCoreAnalyzersResources.MutableStructsShouldNotBeUsedForReadonlyFieldsDescription), MicrosoftNetCoreAnalyzersResources.ResourceManager, typeof(MicrosoftNetCoreAnalyzersResources));

        private static readonly ImmutableList<string> MutableValueTypesOfInterest = new List<string>
        {
            "System.Threading.SpinLock", "System.Runtime.InteropServices.GCHandle"
        }.ToImmutableList();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var typesOfInterest = MutableValueTypesOfInterest.Select(typeName =>
                    compilationContext.Compilation.GetTypeByMetadataName(typeName)).Where(type => type != null).ToList();

                if (!typesOfInterest.Any())
                {
                    return;
                }

                compilationContext.RegisterSymbolAction((symbolContext) => AnalyzeField(symbolContext, typesOfInterest), SymbolKind.Field);
            });
        }

        private static void AnalyzeField(SymbolAnalysisContext context, List<INamedTypeSymbol> typesOfInterest)
        {
            var fieldSymbol = (IFieldSymbol)context.Symbol;

            if (!fieldSymbol.IsReadOnly || !typesOfInterest.Contains(fieldSymbol.Type))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, fieldSymbol.Locations.First(),
                string.Format(s_localizableMessage.ToString(), fieldSymbol.Name, fieldSymbol.Type.Name)));
        }

        internal const string RuleId = "CA2011";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(RuleId,
            s_localizableTitle,
            "{0}",
            DiagnosticCategory.Reliability,
            DiagnosticHelpers.DefaultDiagnosticSeverity,
            isEnabledByDefault: DiagnosticHelpers.EnabledByDefaultIfNotBuildingVSIX,
            description: s_localizableDescription,
            helpLinkUri: null);    // TODO: add MSDN url
    }
}
