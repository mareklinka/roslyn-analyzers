using System.Threading.Tasks;
using Microsoft.NetCore.CSharp.Analyzers.Performance;
using Test.Utilities;
using Xunit;

namespace Microsoft.NetCore.Analyzers.Performance.UnitTests
{
    public class UseAsParallelCorrectlyFixerTests
    {
        [Fact]
        public async Task CSharpReadonlyFieldsOfKnownMutableTypes_RemovesReadonlyModifier()
        {
            var initial = @"
using System;
using System.Collections.Generic;
using System.Linq;

public class C
{
    public void Method()
    {
        var list = new List<string>();
        foreach (var value in list.[|AsParallel|]())
        {
        }

        foreach (var value in ParallelEnumerable.[|AsParallel|](ToString()))
        {
        }

        foreach (var value in list.[|AsParallel|]())
        {
        }

        foreach (var value in ToString().[|AsParallel|]())
        {
        }

        foreach (var value in new List<string>().[|AsParallel|]())
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).[|AsParallel|]())
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).Select(_ => _.Length).[|AsParallel|]())
        {
        }
    }
}
";

            var expected = @"
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

        foreach (var value in ToString())
        {
        }

        foreach (var value in list)
        {
        }

        foreach (var value in ToString())
        {
        }

        foreach (var value in new List<string>())
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0))
        {
        }

        foreach (var value in list.Where(_ => _.Length > 0).Select(_ => _.Length))
        {
        }
    }
}
";
            await CSharpCodeFixVerifier<CSharpUseAsParallelCorrectlyAnalyzer, CSharpUseAsParallelCorrectlyFixer>
                .VerifyCodeFixAsync(initial, expected);
        }
    }
}