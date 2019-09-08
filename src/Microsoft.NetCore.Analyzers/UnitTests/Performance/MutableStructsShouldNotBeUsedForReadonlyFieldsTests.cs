// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Test.Utilities;
using Xunit;

namespace Microsoft.NetCore.Analyzers.Performance.UnitTests
{
    public class MutableStructsShouldNotBeUsedForReadonlyFieldsTests : DiagnosticAnalyzerTestBase
    {

        #region HelperMethods

        private static readonly string MessageTemplate = MicrosoftNetCoreAnalyzersResources.MutableStructsShouldNotBeUsedForReadonlyFieldsMessage;

        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer();
        }

        private static DiagnosticResult GetCA1829CSharpResultAt(int line, int column, string fieldName, string fieldType)
        {
            return GetCSharpResultAt(line, column, MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer.RuleId,
                string.Format(MessageTemplate, fieldName, fieldType));
        }

        private static DiagnosticResult GetCA1829BasicResultAt(int line, int column, string fieldName, string fieldType)
        {
            return GetBasicResultAt(line, column, MutableStructsShouldNotBeUsedForReadonlyFieldsAnalyzer.RuleId,
                string.Format(MessageTemplate, fieldName, fieldType));
        }

        #endregion


        #region Diagnostic Tests

        [Fact]
        public void CSharpReadonlyKnownMutableTypes_DiagnosticFires()
        {
            VerifyCSharp(@"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class C
{
    private readonly SpinLock _sl = new SpinLock();
    private readonly GCHandle _gch = new GCHandle();

    private readonly SpinLock _sl_noinit;
    private readonly GCHandle _gch_noinit;

    private readonly SpinLock _sl1 = new SpinLock(), _sl2 = new SpinLock();
    private readonly GCHandle _gch1 = new GCHandle(), _gch2 = new GCHandle();

    private readonly SpinLock _sl1_noinit, _sl2_noinit;
    private readonly GCHandle _gch1_noinit, _gch2_noinit;
}
",
                GetCA1829CSharpResultAt(8, 31, "_sl", "SpinLock"),
                GetCA1829CSharpResultAt(9, 31, "_gch", "GCHandle"),
                GetCA1829CSharpResultAt(11, 31, "_sl_noinit", "SpinLock"),
                GetCA1829CSharpResultAt(12, 31, "_gch_noinit", "GCHandle"),
                GetCA1829CSharpResultAt(14, 31, "_sl1", "SpinLock"),
                GetCA1829CSharpResultAt(14, 54, "_sl2", "SpinLock"),
                GetCA1829CSharpResultAt(15, 31, "_gch1", "GCHandle"),
                GetCA1829CSharpResultAt(15, 55, "_gch2", "GCHandle"),
                GetCA1829CSharpResultAt(17, 31, "_sl1_noinit", "SpinLock"),
                GetCA1829CSharpResultAt(17, 44, "_sl2_noinit", "SpinLock"),
                GetCA1829CSharpResultAt(18, 31, "_gch1_noinit", "GCHandle"),
                GetCA1829CSharpResultAt(18, 45, "_gch2_noinit", "GCHandle"));
        }

        [Fact]
        public void CSharpWritableKnownMutableTypes_DiagnosticIgnored()
        {
            VerifyCSharp(@"
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
");
        }

        [Fact]
        public void CSharpReadonlyCustomType_DiagnosticIgnored()
        {
            VerifyCSharp(@"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public struct S
{
}

public class C
{
    private readonly S _sl = new S();
    private readonly S _sl2;

    private readonly S _s3 = new S(), _s4 = new S();
    private readonly S _s5, _s6;
}
");
        }

        [Fact]
        public void CSharpCustomType_DiagnosticIgnored()
        {
            VerifyCSharp(@"
using System;
using System.Threading;
using System.Runtime.InteropServices;

public struct S
{
}

public class C
{
    private S _sl = new S();
    private S _sl2;

    private S _s3 = new S(), _s4 = new S();
    private S _s5, _s6;
}
");
        }

        [Fact]
        public void BasicReadonlyKnownMutableTypes_DiagnosticFires()
        {
            VerifyBasic(@"
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class Class1
    Public ReadOnly _sl As SpinLock = New SpinLock()
    Public ReadOnly _gch As GCHandle = New GCHandle()

    Public ReadOnly _sl_noinit As SpinLock
    Public ReadOnly _gch_noinit As GCHandle

    Public Readonly _sl1, _sl2 As SpinLock
    Public Readonly _gch1, _gch2 As GCHandle

    Public Readonly _sl3 As New SpinLock()
    Public Readonly _gch3 As New GCHandle()
End Class
",
                GetCA1829BasicResultAt(6, 21, "_sl", "SpinLock"),
                GetCA1829BasicResultAt(7, 21, "_gch", "GCHandle"),
                GetCA1829BasicResultAt(9, 21, "_sl_noinit", "SpinLock"),
                GetCA1829BasicResultAt(10, 21, "_gch_noinit", "GCHandle"),
                GetCA1829BasicResultAt(12, 21, "_sl1", "SpinLock"),
                GetCA1829BasicResultAt(12, 27, "_sl2", "SpinLock"),
                GetCA1829BasicResultAt(13, 21, "_gch1", "GCHandle"),
                GetCA1829BasicResultAt(13, 28, "_gch2", "GCHandle"),
                GetCA1829BasicResultAt(15, 21, "_sl3", "SpinLock"),
                GetCA1829BasicResultAt(16, 21, "_gch3", "GCHandle"));
        }

        [Fact]
        public void BasicWritableKnownMutableTypes_DiagnosticIgnored()
        {
            VerifyBasic(@"
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
");
        }

        [Fact]
        public void BasicReadonlyCustomType_DiagnosticIgnored()
        {
            VerifyBasic(@"
Imports System

Public Structure S

End Structure

Public Class Class1
    Public ReadOnly _s As S = New S()
    Public ReadOnly _s2 As S

    Public ReadOnly _s3, _s4 As S
    Public Readonly _s5 As New S()
End Class
");
        }

        [Fact]
        public void BasicCustomType_DiagnosticIgnored()
        {
            VerifyBasic(@"
Imports System

Public Structure S

End Structure

Public Class Class1
    Public _s As S = New S()
    Public _s2 As S

    Public _s3, _s4 As S
    Public _s5 As New S()
End Class
");
        }

        #endregion
    }
}
