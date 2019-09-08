// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    Microsoft.NetCore.Analyzers.Performance.MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer,
    Microsoft.NetCore.CSharp.Analyzers.Performance.CSharpMutableStructsShouldNotBeUsedForReadonlyFieldsFixer>;
using VerifyVB = Microsoft.CodeAnalysis.VisualBasic.Testing.XUnit.CodeFixVerifier<
    Microsoft.NetCore.Analyzers.Performance.MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer,
    Microsoft.NetCore.VisualBasic.Analyzers.Performance.BasicMutableStructsShouldNotBeUsedForReadonlyFieldsFixer>;

namespace Microsoft.NetCore.Analyzers.Performance.UnitTests
{
    public class MutableStructsShouldNotBeUsedForReadonlyFieldsFixerTests
    {
        #region CodeFix Tests

        [Fact]
        public async Task CSharpReadonlyFieldsOfKnownMutableTypes_RemovesReadonlyModifier()
        {
            var initial = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class C
{
    private readonly SpinLock [|_sl|] = new SpinLock();
    private readonly GCHandle [|_gch|] = new GCHandle();

    private readonly SpinLock [|_sl_noinit|];
    private readonly GCHandle [|_gch_noinit|];

    private readonly SpinLock [|_sl1|] = new SpinLock(), [|_sl2|] = new SpinLock();
    private readonly GCHandle [|_gch1|] = new GCHandle(), [|_gch2|] = new GCHandle();

    private readonly SpinLock [|_sl1_noinit|], [|_sl2_noinit|];
    private readonly GCHandle [|_gch1_noinit|], [|_gch2_noinit|];
}
";

            var expected = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class C
{
    private SpinLock _sl = new SpinLock();
    private GCHandle _gch = new GCHandle();

    private SpinLock _sl_noinit;
    private GCHandle _gch_noinit;

    private SpinLock _sl1 = new SpinLock(), _sl2 = new SpinLock();
    private GCHandle _gch1 = new GCHandle(), _gch2 = new GCHandle();

    private SpinLock _sl1_noinit, _sl2_noinit;
    private GCHandle _gch1_noinit, _gch2_noinit;
}
";
            await VerifyCS.VerifyCodeFixAsync(initial, expected);
        }

        [Fact]
        public async Task CSharpWritableFieldsOfKnownMutableType_NoDiagnosticNoFix()
        {
            var initial = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class C
{
    private SpinLock _sl = new SpinLock();
    private GCHandle _gch = new GCHandle();

    private SpinLock _sl_noinit;
    private GCHandle _gch_noinit;

    private SpinLock _sl1 = new SpinLock(), _sl2 = new SpinLock();
    private GCHandle _gch1 = new GCHandle(), _gch2 = new GCHandle();

    private SpinLock _sl1_noinit, _sl2_noinit;
    private GCHandle _gch1_noinit, _gch2_noinit;
}
";

            var expected = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class C
{
    private SpinLock _sl = new SpinLock();
    private GCHandle _gch = new GCHandle();

    private SpinLock _sl_noinit;
    private GCHandle _gch_noinit;

    private SpinLock _sl1 = new SpinLock(), _sl2 = new SpinLock();
    private GCHandle _gch1 = new GCHandle(), _gch2 = new GCHandle();

    private SpinLock _sl1_noinit, _sl2_noinit;
    private GCHandle _gch1_noinit, _gch2_noinit;
}
";
            await VerifyCS.VerifyCodeFixAsync(initial, expected);
        }

        [Fact]
        public async Task CSharpReadonlyFieldsOfCustomType_NoDiagnosticNoFix()
        {
            var initial = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public struct S 
{
}

public class C
{
    private readonly S _s = new S();
    private readonly S _s1 = new S(), _s2 = new S();
    private readonly S _s3, _s4;
}
";

            var expected = @"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public struct S 
{
}

public class C
{
    private readonly S _s = new S();
    private readonly S _s1 = new S(), _s2 = new S();
    private readonly S _s3, _s4;
}
";
            await VerifyCS.VerifyCodeFixAsync(initial, expected);
        }

        [Fact]
        public async Task BasicReadonlyFieldsOfKnownMutableTypes_RemovesReadonlyModifier()
        {
            var initial = @"
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class Class1
    Public ReadOnly [|_sl|] As SpinLock = New SpinLock()
    Public ReadOnly [|_gch|] As GCHandle = New GCHandle()

    Public ReadOnly [|_sl_noinit|] As SpinLock
    Public ReadOnly [|_gch_noinit|] As GCHandle

    Public Readonly [|_sl1|], [|_sl2|] As SpinLock
    Public Readonly [|_gch1|], [|_gch2|] As GCHandle

    Public Readonly [|_sl3|] As New SpinLock()
    Public Readonly [|_gch3|] As New GCHandle()
End Class
";

            var expected = @"
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class Class1
    Public _sl As SpinLock = New SpinLock()
    Public _gch As GCHandle = New GCHandle()

    Public _sl_noinit As SpinLock
    Public _gch_noinit As GCHandle

    Public _sl1, _sl2 As SpinLock
    Public _gch1, _gch2 As GCHandle

    Public _sl3 As New SpinLock()
    Public _gch3 As New GCHandle()
End Class
";
            await VerifyVB.VerifyCodeFixAsync(initial, expected);
        }

        [Fact]
        public async Task BasicWritableFieldsOfKnownMutableTypes_NoDiagnosticNoFix()
        {
            var initial = @"
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class Class1
    Public _sl As SpinLock = New SpinLock()
    Public _gch As GCHandle = New GCHandle()

    Public _sl_noinit As SpinLock
    Public _gch_noinit As GCHandle

    Public _sl1, _sl2 As SpinLock
    Public _gch1, _gch2 As GCHandle

    Public _sl3 As New SpinLock()
    Public _gch3 As New GCHandle()
End Class
";

            var expected = @"
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class Class1
    Public _sl As SpinLock = New SpinLock()
    Public _gch As GCHandle = New GCHandle()

    Public _sl_noinit As SpinLock
    Public _gch_noinit As GCHandle

    Public _sl1, _sl2 As SpinLock
    Public _gch1, _gch2 As GCHandle

    Public _sl3 As New SpinLock()
    Public _gch3 As New GCHandle()
End Class
";
            await VerifyVB.VerifyCodeFixAsync(initial, expected);
        }

        [Fact]
        public async Task BasicReadonlyFieldsOfCustomTypes_NoDiagnosticNoFix()
        {
            var initial = @"
Public Structure S
End Structure

Public Class Class1
    Public ReadOnly _sl As S = New S()
    Public ReadOnly _sl_noinit As S

    Public ReadOnly _s3, _s4 As S
    Public Readonly _s5 As New S()
End Class
";

            var expected = @"
Public Structure S
End Structure

Public Class Class1
    Public ReadOnly _sl As S = New S()
    Public ReadOnly _sl_noinit As S

    Public ReadOnly _s3, _s4 As S
    Public Readonly _s5 As New S()
End Class
";
            await VerifyVB.VerifyCodeFixAsync(initial, expected);
        }

        #endregion
    }
}