using System.Collections.Immutable;
using Analyzer.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.NetCore.Analyzers.Performance
{
    public abstract class UseAsParallelCorrectlyAnalyzer : DiagnosticAnalyzer
    {
        private static readonly LocalizableString s_localizableTitle = "Use AsParallel correctly";

        private static readonly LocalizableString s_localizableMessage = "";
        private static readonly LocalizableString s_localizableDescription = "";

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            RegisterDiagnosticAction(context);
        }

        protected abstract void RegisterDiagnosticAction(AnalysisContext context);

        protected static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location readonlyLocation)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, readonlyLocation,
                s_localizableMessage.ToString()));
        }

        internal const string RuleId = "CA1830";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(RuleId,
            s_localizableTitle,
            "{0}",
            DiagnosticCategory.Performance,
            DiagnosticHelpers.DefaultDiagnosticSeverity,
            isEnabledByDefault: DiagnosticHelpers.EnabledByDefaultIfNotBuildingVSIX,
            description: s_localizableDescription,
            helpLinkUri: null);    // TODO: add MSDN url

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
    }
}