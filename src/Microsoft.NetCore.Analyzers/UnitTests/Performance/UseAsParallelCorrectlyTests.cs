using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.NetCore.CSharp.Analyzers.Performance;
using Test.Utilities;
using Xunit;

namespace Microsoft.NetCore.Analyzers.Performance.UnitTests
{
    public class UseAsParallelCorrectlyTests : DiagnosticAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            throw new NotSupportedException();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CSharpUseAsParallelCorrectlyAnalyzer();
        }

        #region Diagnostic Tests

        [Fact]
        public void CSharpAsParallelAtEnd_DiagnosticFires()
        {
            VerifyCSharp(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class C
{
    public void Method()
    {
        var list = new List<string>();
        foreach (var value in list.AsParallel())
        {
        }

        foreach (var value in ParallelEnumerable.AsParallel(ToString()))
        {
        }

        foreach (var value in list.AsParallel())
        {
        }

        foreach (var value in ToString().AsParallel())
        {
        }

        foreach (var value in new List<string>().AsParallel())
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).AsParallel())
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).Select(_ => _.Length).AsParallel())
        {
        }
    }
}
",
                GetCA1830CSharpResultAt(11, 36),
                GetCA1830CSharpResultAt(15, 50),
                GetCA1830CSharpResultAt(19, 36),
                GetCA1830CSharpResultAt(23, 42),
                GetCA1830CSharpResultAt(27, 50),
                GetCA1830CSharpResultAt(31, 61),
                GetCA1830CSharpResultAt(35, 83));
        }

        [Fact]
        public void CSharpAsParallelInMiddle_DiagnosticIgnored()
        {
            VerifyCSharp(@"
using System;
using System.Collections.Generic;
using System.Linq;

public class C
{
    public void Method()
    {
        var list = new List<string>();
        foreach (var value in list)
        {
        }

        foreach (var value in ParallelEnumerable.AsParallel(ToString()).Select(_ => _))
        {
        }

        foreach (var value in list.AsParallel().Select(_ => _.Length))
        {
        }

        foreach (var value in ToString().AsParallel().Select(_ => _))
        {
        }

        foreach (var value in new List<string>().AsParallel().Select(_ => _.Length))
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).AsParallel().Select(_ => _.Length))
        {
        }
    }
}
");
        }

        #endregion

        private static DiagnosticResult GetCA1830CSharpResultAt(int line, int column)
        {
            return GetCSharpResultAt(line, column, UseAsParallelCorrectlyAnalyzer.RuleId,
                "");
        }

        private static DiagnosticResult GetCA1830BasicResultAt(int line, int column)
        {
            return GetBasicResultAt(line, column, UseAsParallelCorrectlyAnalyzer.RuleId,
                "");
        }
    }
}